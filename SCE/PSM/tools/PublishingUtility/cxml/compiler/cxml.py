# -*- coding: utf-8 -*-
# SCE CONFIDENTIAL
# $PSLibId$
#
#       Copyright (C) 2011 Sony Computer Entertainment Inc.
#                         All Rights Reserved.
#
#                    Cxml Compiler - cxml compiler
#

import os
import sys
import struct
import fnmatch
import xml.dom.minidom as minidom

# python24対応のため
try:
    import hashlib
    sha1 = hashlib.sha1
except ImportError:
    import sha
    sha1 = sha.new

# --- const ----------------------------------

## データ格納タイプ
attr_type_int = 1         # 32bit整数
attr_type_float = 2       # 32bit浮動小数
attr_type_string = 3      # 文字列
attr_type_wstring = 4     # ワイド文字列
attr_type_hash = 5        # ハッシュ
attr_type_intarray = 6    # 32bit整数配列
attr_type_floatarray = 7  # 32bit浮動小数配列
attr_type_filename = 8    # ファイル
attr_type_id = 9          # ID
attr_type_idref = 10       # IDへの参照
attr_type_idhash = 11     # IDハッシュ
attr_type_idhashref = 12  # IDハッシュへの参照

## ヘッダセクション
header_bin_fmt = "4siiiiiiiiiiiiiiiiiii"
header_bin_size = struct.calcsize(header_bin_fmt)
assert( (header_bin_size%16)==0 )

## マジックナンバーとバージョン
default_header_magic = "CXML"
default_header_version = 0x0100

## IDハッシュシステム予約値
default_idhash_invalid = 0x00000000

## ハッシュテーブルに格納される元のKey(文字列)リスト
hash_table_originkey_list = []


## 要素ツリーセクション
element_bin_fmt = "iiiiiii"
element_bin_size = struct.calcsize(element_bin_fmt)
element_data_tagname = 0
element_data_attrnum = 1
element_data_parent = 2
element_data_prev = 3
element_data_next = 4
element_data_first_child = 5
element_data_last_child = 6


## XMLの要素名、属性名を定義するためのクラス
class schemaitem:
    def __init__( self, element_name, parent_element_name=[], parent_schema_name=None, attrtype={} ):
        self.element_name = element_name
        self.parent_element_name = parent_element_name
        self.parent_schema_name = parent_schema_name
        self.attrtype = attrtype

## id 属性のテーブル
class idtableitem:
    def __init__( self, id, id_offset, entity_offset ):
        self.id = id
        self.id_offset = id_offset
        self.entity_offset = entity_offset

## idhash属性のテーブル
class idhashtableitem:
    def __init__( self, idhash, idhash_offset, entity_offset ):
        self.idhash = idhash
        self.idhash_offset = idhash_offset
        self.entity_offset = entity_offset


## CXMLコンパイラの基底クラス
class xmlcompiler:

    ## CXMLコンパイラの基底クラスのコンストラクタ
    #  @param reader [in] XMLファイルオブジェクト
    #  @param writer [in] CXMLファイルオブジェクト
    #  @param file_provider [in] 外部ファイル提供関数
    #  @param reporter [in] エラーメッセージのレポート関数
    #  @param base_path [in] 外部ファイルのベースパス
    def __init__( self, reader, writer, file_provider, reporter, base_path ):

        self.reader = reader
        self.writer = writer
        self.file_provider = file_provider
        self.reporter = reporter
        self.base_path = base_path

        self.endian_fmt = '<'; # デフォルトlittle
        self.wstring_size = 2  # デフォルト2bytes 

        self.doc = None
        self.id_table = {}
        self.idhash_table = {}
        self.string_table = {}
        self.wstring_table = {}
        self.hash_table = {}
        self.file_table = {}
        self.schema = {}
        self.header_magic = default_header_magic
        self.header_version = default_header_version

        self.header_bin = ""
        self.tree_bin = ""
        self.idtable_bin = ""
        self.idhashtable_bin = ""
        self.stringtable_bin = ""
        self.wstringtable_bin = ""
        self.hashtable_bin = ""
        self.intarraytable_bin = ""
        self.floatarraytable_bin = ""
        self.filetable_bin = ""

        self.error_count = 0

    # dataの末尾に0を挿入することによって長さがalignの倍数になるようにする
    def _align( self, align, data ):
        if ( len(data) % align ) != 0:
            data += '\0' * ( align - ( len(data) % align ) )
        return data

    # idテーブルからkeyを検索し、見つからなかった場合は末尾に登録する
    # 見つかった場合、未解決な参照があれば解決する
    def register_id_table_entity( self, key, entity_offset ):
        if self.id_table.has_key(key):
            item = self.id_table[key]
            if item.entity_offset!=None:
                self.reporter.write( "Error: duplicate id '%s'\n" % key )
                self.set_error()
                return -1
            item.entity_offset = entity_offset

            # idtable中のentity_offsetを更新する
            self.idtable_bin = self.idtable_bin[:item.id_offset] + struct.pack( self.get_endian_fmt() + "i", entity_offset ) + self.idtable_bin[item.id_offset + 4:]

            assert( item.id_offset!=None )
            return item.id_offset
        else:
            offset = len(self.idtable_bin)
            key_utf8 = key.encode('utf_8')
            self.id_table[key] = idtableitem(key,offset,entity_offset)
            self.idtable_bin += struct.pack( self.get_endian_fmt() + "i", entity_offset ) + key_utf8 + '\0'

            # intのアラインメントに揃える
            self.idtable_bin = self._align( 4, self.idtable_bin ) 
            return offset

    # idテーブルからkeyを検索し、見つからなかった場合は未解決モードで末尾に登録する
    def register_id_table_ref( self, key, ref_offset ):
        if self.id_table.has_key(key):
            item = self.id_table[key]
            return item.id_offset
        else:
            offset = len(self.idtable_bin)
            key_utf8 = key.encode('utf_8')
            self.id_table[key] = idtableitem( key, offset, None )
            self.idtable_bin += struct.pack( self.get_endian_fmt() + "i", -1 ) + key_utf8 + '\0'
            return offset

    # idhashテーブルからkeyを検索し、見つからなかった場合は末尾に登録する
    # 見つかった場合、未解決な参照があれば解決する
    def register_idhash_table_entity( self, key, entity_offset ):

        # 検索及び登録のためにkeyのsha-1ハッシュ値上位4byteの整数値を計算する
        key_hashval_upper4 = self.calc_id_hash_upper4(key)    
        # print "key = %s : key_hashval_upper4 = 0x%x" % (key, key_hashval_upper4)

        # 求めたsha-1ハッシュ値がシステム予約値と一致している場合、無効なものとする
        if key_hashval_upper4 == default_idhash_invalid:
            self.reporter.write( "Error: this idhash '0x%x' (key = %s) is reserved for system idhash value.\n" % (key_hashval_upper4, key) )
            self.set_error()
            return -1

        if self.idhash_table.has_key(key_hashval_upper4):
            item = self.idhash_table[key_hashval_upper4]
            if item.entity_offset!=None:
                self.reporter.write( "Error: duplicate idhash '0x%x' (key = %s)\n" % (key_hashval_upper4, key) )
                self.set_error()
                return -1
            item.entity_offset = entity_offset

            # idhashtable中のentity_offsetを更新する
            self.idhashtable_bin = self.idhashtable_bin[:item.idhash_offset] + struct.pack( self.get_endian_fmt() + "i", entity_offset ) + self.idhashtable_bin[item.idhash_offset + 4:]

            assert( item.idhash_offset!=None )
            return item.idhash_offset
        else:
            offset = len(self.idhashtable_bin)
            self.idhash_table[key_hashval_upper4] = idhashtableitem(key,offset,entity_offset)
            self.idhashtable_bin += struct.pack( self.get_endian_fmt() + "iI", entity_offset, key_hashval_upper4 )
            return offset

    # idhashテーブルからkeyを検索し、見つからなかった場合は未解決モードで末尾に登録する
    def register_idhash_table_ref( self, key, ref_offset ):
    
        # 検索及び登録のためにkeyのsha-1ハッシュ値上位4byteの整数値を計算する
        key_hashval_upper4 = self.calc_id_hash_upper4(key)    
        # print "key = %s : key_hashval_upper4 = 0x%x" % (key, key_hashval_upper4)
    
        if self.idhash_table.has_key(key_hashval_upper4):
            item = self.idhash_table[key_hashval_upper4]
            return item.idhash_offset
        else:
            offset = len(self.idhashtable_bin)
            self.idhash_table[key_hashval_upper4] = idhashtableitem( key, offset, None )
            self.idhashtable_bin += struct.pack( self.get_endian_fmt() + "iI", -1, key_hashval_upper4 )
            return offset

    # 文字列テーブルからkeyを検索し、見つからなかった場合は末尾に登録する
    def register_string_table( self, key ):
        if self.string_table.has_key(key):
            return self.string_table[key]
        else:
            offset = len(self.stringtable_bin)
            key_utf8 = key.encode('utf_8')
            self.string_table[key] = (offset,len(key_utf8))
            self.stringtable_bin += key_utf8 + '\0'
            return offset, len(key_utf8)
                
    # ワイド文字列テーブルからkeyを検索し、見つからなかった場合は末尾に登録する
    def register_wstring_table( self, key ):
        if self.wstring_table.has_key(key):
            return self.wstring_table[key]
        else:
            offset = len(self.wstringtable_bin) / self.wstring_size

            # 2bytesベースの場合
            if self.wstring_size == 2:            
                # エンディアンに従ってエンコード方法を設定する
                if self.endian_fmt == '<':
                    key_utf16 = key.encode('utf_16_le')
                elif self.endian_fmt == '>':
                    key_utf16 = key.encode('utf_16_be')

                self.wstring_table[key] = (offset,len(key_utf16))
                self.wstringtable_bin += key_utf16 + '\0' + '\0'
                return offset, len(key_utf16)

            # 4bytesベースの場合
            elif self.wstring_size == 4:
                # エンディアンに従ってエンコード方法を設定する
                # @note python2.5xでは32bitsエンコーディングをサポートしていないようである
                if self.endian_fmt == '<':
                    key_utf32 = key.encode('utf_32_le')
                elif self.endian_fmt == '>':
                    key_utf32 = key.encode('utf_32_be')
                
                self.wstring_table[key] = (offset,len(key_utf32))
                self.wstringtable_bin += key_utf32 + '\0' + '\0' + '\0' + '\0'
                return offset, len(key_utf32)

            else:
                assert(0)
            
    # ハッシュテーブルからkeyを検索し、見つからなかった場合は末尾に登録する
    def register_hash_table( self, key ):
        # 検索及び登録のためにkeyのsha-1ハッシュ値上位4byteの整数値を計算する
        key_hashval_upper4 = self.calc_id_hash_upper4(key)    
        # print "key = %s : key_hashval_upper4 = 0x%x" % (key, key_hashval_upper4)

        if self.hash_table.has_key(key_hashval_upper4):
            # ハッシュ値が登録されていて元のkey(文字列)が異なっている場合は警告を出力する
            if key not in hash_table_originkey_list:
                print "Warning: id:%s hash key is duplicated."
        
            return self.hash_table[key_hashval_upper4]
        else:
            offset = len(self.hashtable_bin) / 4
            self.hash_table[key_hashval_upper4] = (offset,4)
            self.hashtable_bin += struct.pack( self.get_endian_fmt() + "I", int(key_hashval_upper4) ) 
            hash_table_originkey_list.append( key )
            return offset, 4

    # 整数配列テーブルからkeyをバイナリ化したものを検索し、見つからなかった場合は末尾に登録する
    def register_intarray_table( self, key ):
        intarray_str = key.split(',')
        int_bin = ""
        num_element = len(intarray_str)
        for s in intarray_str:
            int_bin += struct.pack( self.get_endian_fmt() + "i", int(s) )

        start_pos = 0
        while 1:
            found_pos = self.intarraytable_bin.find( int_bin, start_pos )
            if found_pos<0 : break
            if (found_pos % 4)==0 : break
            start_pos = found_pos + (4-found_pos%4)
            if start_pos>= len(self.intarraytable_bin):
                found_pos = -1
                break

        if found_pos>=0:
            return found_pos/4, num_element
        else:
            offset = len(self.intarraytable_bin)/4
            self.intarraytable_bin += int_bin
            return offset, num_element

    # 浮動小数点配列テーブルからkeyをバイナリ化したものを検索し、見つからなかった場合は末尾に登録する
    def register_floatarray_table( self, key ):
        floatarray_str = key.split(',')
        float_bin = ""
        num_element = len(floatarray_str)
        for s in floatarray_str:
            float_bin += struct.pack( self.get_endian_fmt() + "f", float(s) )

        start_pos = 0
        while 1:
            found_pos = self.floatarraytable_bin.find( float_bin, start_pos )
            if found_pos<0 : break
            if (found_pos % 4)==0 : break
            start_pos = found_pos + (4-found_pos%4)
            if start_pos>= len(self.floatarraytable_bin):
                found_pos = -1
                break

        if found_pos>=0:
            return found_pos/4, num_element
        else:
            offset = len(self.floatarraytable_bin)/4
            self.floatarraytable_bin += float_bin
            return offset, num_element

    # ファイルテーブルからファイル属性を検索し、見つからなかった場合は末尾に登録する
    #def register_file_table( self, path ):
    def register_file_table( self, node, attr ):
        
        path = node.getAttribute(attr.name)
        path = os.path.normpath(os.path.join( self.base_path, path ))

        if self.file_table.has_key(path):
            return self.file_table[path]
        else:
            offset = len(self.filetable_bin)

            # ファイルを読み込み、filetable_binに連結する
            try:
                fd = self.file_provider( path, node, attr )
            except IOError:
                self.reporter.write( "Error: cannot open file '%s'\n" % path )
                self.set_error()
                return 0, 0

            fileimage = fd.read()
            size = len(fileimage)
            fileimage = self._align( 16, fileimage )
            self.filetable_bin += fileimage
            self.file_table[path] = ( offset, size )

            return offset, size

    # 子供の要素の数を調べる
    def count_child_element( self, node ):
        count = 0
        child = node.firstChild
        while child:
            if child.nodeType==child.ELEMENT_NODE:
                count += 1
            child = child.nextSibling
        return count

    # 次の要素のoffset値を更新する
    def update_next_elm_offset( self, node, next ):
        elelemt_bin = self.tree_bin[ node.offset : node.offset + element_bin_size ]
        elm_data = list(struct.unpack( self.get_endian_fmt() + element_bin_fmt, elelemt_bin ))
        elm_data[element_data_next] = next.offset
        elelemt_bin = struct.pack( self.get_endian_fmt() + element_bin_fmt, *elm_data )
        self.tree_bin = self.tree_bin[:node.offset] + elelemt_bin + self.tree_bin[node.offset + element_bin_size:]

    # 最初の子要素のoffset値を更新する
    def update_first_child_elm_offset( self, node, first_child ):
        elelemt_bin = self.tree_bin[ node.offset : node.offset + element_bin_size ]
        elm_data = list(struct.unpack( self.get_endian_fmt() + element_bin_fmt, elelemt_bin ))
        elm_data[element_data_first_child] = first_child.offset
        elelemt_bin = struct.pack( self.get_endian_fmt() + element_bin_fmt, *elm_data )
        self.tree_bin = self.tree_bin[:node.offset] + elelemt_bin + self.tree_bin[node.offset + element_bin_size:]

    # 最後の子要素のoffset値を更新する
    def update_last_child_elm_offset( self, node, last_child ):
        elelemt_bin = self.tree_bin[ node.offset : node.offset + element_bin_size ]
        elm_data = list(struct.unpack( self.get_endian_fmt() + element_bin_fmt, elelemt_bin ))
        elm_data[element_data_last_child] = last_child.offset
        elelemt_bin = struct.pack( self.get_endian_fmt() + element_bin_fmt, *elm_data )
        self.tree_bin = self.tree_bin[:node.offset] + elelemt_bin + self.tree_bin[node.offset + element_bin_size:]

    # 親要素を見つける
    def find_parent_element( self, node ):
        assert( node.nodeType==node.ELEMENT_NODE )
        if node.parentNode and node.parentNode.nodeType==node.ELEMENT_NODE:
            return node.parentNode
        else:
            return None

    # 前の要素を見つける
    def find_prev_element( self, node ):
        assert( node.nodeType==node.ELEMENT_NODE )
        prev = node.previousSibling
        while prev and prev.nodeType!=node.ELEMENT_NODE:
            prev = prev.previousSibling
        return prev

    # 次の要素を見つける
    def find_next_element( self, node ):
        assert( node.nodeType==node.ELEMENT_NODE )
        next = node.nextSibling
        while next and next.nodeType!=node.ELEMENT_NODE:
            next = next.nextSibling
        return next

    # 最初の子要素を見つける
    def find_first_child_element( self, node ):
        next = node.firstChild
        while next and next.nodeType!=node.ELEMENT_NODE:
            next = next.nextSibling
        return next

    # 最後の子要素を見つける
    def find_last_child_element( self, node ):
        prev = node.lastChild
        while prev and prev.nodeType!=node.ELEMENT_NODE:
            prev = prev.previousSibling
        return prev

    ## 出力ファイルのエンディアンをBIGに設定する
    #  @note 出力ファイルのエンディアンのデフォルト設定はリトルになっています
    def set_big_endian( self ):
        self.endian_fmt = '>'    

    # 出力ファイルのエンディアンフォーマット文字('<'or'>')を取得する
    def get_endian_fmt( self ):
        return self.endian_fmt

    ## 出力ファイルのワイド文字列テーブル内の値を1文字4bytesに設定する
    def set_wstring_ucs4( self ):
        self.wstring_size = 4
    
    ## 出力ファイルのワイド文字列のサイズ[byte]を取得する
    def get_wstring_size( self ):
        return self.wstring_size    

    ## スキーマを追加する
    #  @param item [in] 追加したいスキーマを表すschemaitemクラスのオブジェクト
    def append_schema( self, item ):
        if self.schema.has_key(item.element_name):
            raise KeyError, "schema duplicated."
        
        # スキーマの継承元が指定されていない場合    
        if not item.parent_schema_name:
            self.schema[item.element_name] = item
        else:
            # まずスキーマを追加する
            self.schema[item.element_name] = item
            # 次に継承元スキーマの属性を追加する
            self.extend_schema( item, self.schema[item.parent_schema_name].attrtype )

    ## スキーマの属性を追加する
    #  @param item [in] 追加したいスキーマを表すschemaitemクラスのオブジェクト
    #  @param extattr [in] 追加するスキーマの属性ディクショナリ
    def extend_schema( self, item, extattr ):
        # 追加属性の重複を確認
        for i in range( len(extattr) ):
            if item.attrtype.has_key( extattr.keys()[i] ):
                raise KeyError, "attribute duplicated."
    
        self.schema[item.element_name].attrtype.update( extattr )

    ## ヘッダのマジックをカスタマイズする
    #  @param magic [in] マジックナンバーとして設定する4byteの文字列
    def set_magic( self, magic ):
        if isinstance(magic,str) and len(magic)==4:
            self.header_magic = magic
        else:
            raise TypeError


    ## ヘッダのバージョン番号をカスタマイズする
    #  @param magic [in] バージョン番号として設定する 32bit hex 数値
    def set_version( self, version ):
            self.header_version = version


    # 階層構造の正当性チェック
    def check_tree_structure( self, node ):

        # タグ名チェック
        if not self.schema.has_key(node.tagName):
            self.reporter.write( "unknown tagname '%s'\n" % node.tagName )
            raise KeyError

        schema = self.schema[node.tagName]
        parent = self.find_parent_element(node)

        if schema.parent_element_name:
            # 親要素の名前に'*'が指定されている場合は階層構造の正当性チェックはせずにそのまま戻る
            if "*" in schema.parent_element_name:
                return

            if not parent or parent.tagName not in schema.parent_element_name:
                self.reporter.write( "element '%s' is used in unexpected place.\n" % node.tagName )
                raise TypeError
        else:
            if parent:
                self.reporter.write( "element '%s' is used in unexpected place.\n" % node.tagName )
                raise TypeError

    # XMLの要素を再帰的にバイナリ形式で出力する
    def make_tree_bin( self, node ):

        node.offset = len( self.tree_bin )

        try:
            self.check_tree_structure(node)
        except:
            self.set_error()
            return

        # 前の要素の この node に関する offset を解決させる
        prev_elm = self.find_prev_element(node)
        if prev_elm:
            self.update_next_elm_offset( prev_elm, node )

        # 親要素の この node に関する offset を解決させる
        parent_elm = self.find_parent_element(node)
        if parent_elm and not self.find_prev_element(node):
            self.update_first_child_elm_offset( parent_elm, node )

        # 親要素の この node に関する offset を解決させる
        if parent_elm and not self.find_next_element(node):
            self.update_last_child_elm_offset( parent_elm, node )

        # タグ名を登録する
        name_handle, size = self.register_string_table( node.tagName )

        # 親要素のoffsetを取得
        if node.parentNode and node.parentNode.nodeType==node.ELEMENT_NODE:
            parent_elm_offset = node.parentNode.offset
        else:
            parent_elm_offset = -1

        # 前の要素のoffsetを取得
        if prev_elm:
            prev_elm_offset = prev_elm.offset
        else:
            prev_elm_offset = -1

        # 次の要素、子供の要素、の offsetはこの時点では不明なので、-1 を入れておき、後で解決する
        next_elm_offset = -1
        first_child_elm_offset = -1
        last_child_elm_offset = -1
        
        # 属性の数を調べる。TEXT_NODEも加味して。
        num_attributes = node.attributes.length
        child = node.firstChild
        if child and child.nodeType==child.TEXT_NODE and child.nodeValue.strip():
            num_attributes += 1

        # 要素のバイナリ形式の表現を作成する
        self.tree_bin += struct.pack( self.get_endian_fmt() + element_bin_fmt,
                                      name_handle,
                                      num_attributes,
                                      parent_elm_offset,
                                      prev_elm_offset,
                                      next_elm_offset,
                                      first_child_elm_offset,
                                      last_child_elm_offset,
                                    )

        # 属性のバイナリ形式の表現を作成する
        for i in xrange(node.attributes.length):

            attr = node.attributes.item(i)

            # 属性名を登録する
            attrname_handle, size = self.register_string_table( attr.name )

            # タグ名と属性名から、属性の型を取得する
            try:
                attr_type = self.schema[ node.tagName ].attrtype[ attr.name ]

            except KeyError:
               
                # スキーマに定義されているkeyとXML内に記述されている属性名のパターンマッチングを行い、
                # 一致していれば定義されている型でバイナリ表現に追加する
                for key in self.schema[ node.tagName ].attrtype:
                    if '*' in key and fnmatch.fnmatch( attr.name, key ):
                        attr_type = self.schema[ node.tagName ].attrtype[ key ]
                        break
                else:
                    self.reporter.write( "Error: '%s' has no attribute named '%s'\n" % ( node.tagName, attr.name ) )
                    self.set_error()
                    continue
                    
                    
            # 型に応じたバイナリ表現を追加する
            if attr_type==attr_type_int:
                i = int( node.getAttribute(attr.name) )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iii4x", attrname_handle, attr_type, i )

            elif attr_type==attr_type_float:
                f = float( node.getAttribute(attr.name) )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iif4x", attrname_handle, attr_type, f )

            elif attr_type==attr_type_string:
                offset, size = self.register_string_table( node.getAttribute(attr.name) )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iiii", attrname_handle, attr_type, offset, size )

            elif attr_type==attr_type_wstring:
                offset, size = self.register_wstring_table( node.getAttribute(attr.name) )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iiii", attrname_handle, attr_type, offset, size )

            elif attr_type==attr_type_hash:
                offset, size = self.register_hash_table( node.getAttribute(attr.name) )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iiii", attrname_handle, attr_type, offset, size )

            elif attr_type==attr_type_intarray:
                offset, size = self.register_intarray_table( node.getAttribute(attr.name) )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iiii", attrname_handle, attr_type, offset, size )

            elif attr_type==attr_type_floatarray:
                offset, size = self.register_floatarray_table( node.getAttribute(attr.name) )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iiii", attrname_handle, attr_type, offset, size )

            elif attr_type==attr_type_filename:
                offset, size = self.register_file_table( node, attr )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iiii", attrname_handle, attr_type, offset, size )

            elif attr_type==attr_type_id:
                offset = self.register_id_table_entity( node.getAttribute(attr.name), node.offset )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iii4x", attrname_handle, attr_type, offset )

            elif attr_type==attr_type_idref:
                ref_offset = len( self.tree_bin )
                offset = self.register_id_table_ref( node.getAttribute(attr.name), ref_offset )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iii4x", attrname_handle, attr_type, offset )
                
            elif attr_type==attr_type_idhash:
                offset = self.register_idhash_table_entity( node.getAttribute(attr.name), node.offset )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iii4x", attrname_handle, attr_type, offset )

            elif attr_type==attr_type_idhashref:
                ref_offset = len( self.tree_bin )
                offset = self.register_idhash_table_ref( node.getAttribute(attr.name), ref_offset )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iii4x", attrname_handle, attr_type, offset )

            else:
                assert(0)


        # TEXT_NODE を 属性として扱う
        child = node.firstChild
        if child and child.nodeType==child.TEXT_NODE and child.nodeValue.strip():

            attr_name = "__text__"

            # 属性名を登録する
            attrname_handle, size = self.register_string_table(attr_name)

            # タグ名と属性名から、属性の型を取得する
            try:
                attr_type = self.schema[ node.tagName ].attrtype[ attr_name ]
            except KeyError:
                self.reporter.write( "Error: '%s' has no attribute named '%s'\n" % ( node.tagName, attr_name ) )
                self.set_error()
                return

            if attr_type==attr_type_string:
                offset, size = self.register_string_table( child.nodeValue )
                self.tree_bin += struct.pack( self.get_endian_fmt() + "iiii", attrname_handle, attr_type, offset, size )
            else:
                assert(0)

        child = node.firstChild
        while child:
            if child.nodeType==child.ELEMENT_NODE:
                self.make_tree_bin(child)
                if self.error_count : return
            child = child.nextSibling

    ## XMLファイルのパースのあとCXMLファイルの出力の前の段階で呼び出されるプリプロセス関数
    #  @note preprocess() 仮想関数は、XMLのパース結果をチェックしたり、変更を加えたりするのに最適な場所です。
    #  @note この関数の中でXMLのパース結果にアクセスするには、self.doc を参照します。
    def preprocess(self):
        pass

    ## XMLファイルをパースし、CXMLファイルを書き出す
    def compile( self ):

        self.header_bin = ""
        self.tree_bin = ""
        self.idtable_bin = ""
        self.idhashtable_bin = ""
        self.stringtable_bin = ""
        self.wstringtable_bin = ""
        self.hashtable_bin = ""
        self.intarraytable_bin = ""
        self.floatarraytable_bin = ""
        self.filetable_bin = ""

        # DOMを読み込む
        self.doc = minidom.parse(self.reader)

        # 前処理を呼び出して、必要に応じてDOMを変更する
        self.preprocess()

        # ツリーを辿ってバイナリ形式に変換する
        self.make_tree_bin( self.doc.documentElement )

        # アライメントをそろえる前のサイズをヘッダに格納するために、記憶する
        tree_size = len( self.tree_bin )
        idtable_size = len( self.idtable_bin )
        idhashtable_size = len( self.idhashtable_bin )
        stringtable_size = len( self.stringtable_bin )
        wstringtable_size = len( self.wstringtable_bin )
        hashtable_size = len( self.hashtable_bin )
        intarraytable_size = len( self.intarraytable_bin )
        floatarraytable_size = len( self.floatarraytable_bin )
        filetable_size = len( self.filetable_bin )

        # 最後のファイルテーブルセクションを除くセクションを16byteの倍数の長さにする
        self.tree_bin = self._align( 16, self.tree_bin )
        self.idtable_bin = self._align( 16, self.idtable_bin )
        self.idhashtable_bin = self._align( 16, self.idhashtable_bin )
        self.stringtable_bin = self._align( 16, self.stringtable_bin )
        self.wstringtable_bin = self._align( 16, self.wstringtable_bin )
        self.hashtable_bin = self._align( 16, self.hashtable_bin )
        self.intarraytable_bin = self._align( 16, self.intarraytable_bin )
        self.floatarraytable_bin = self._align( 16, self.floatarraytable_bin )

        tree_offset = header_bin_size
        idtable_offset = tree_offset + len( self.tree_bin )
        idhashtable_offset = idtable_offset + len( self.idtable_bin )
        stringtable_offset = idhashtable_offset + len( self.idhashtable_bin )
        wstringtable_offset = stringtable_offset + len( self.stringtable_bin )
        hashtable_offset = wstringtable_offset + len( self.wstringtable_bin )
        intarraytable_offset = hashtable_offset + len( self.hashtable_bin )
        floatarraytable_offset = intarraytable_offset + len( self.intarraytable_bin )
        filetable_offset = floatarraytable_offset + len( self.floatarraytable_bin )

        self.header_bin += struct.pack( self.get_endian_fmt() + header_bin_fmt,
                                        self.header_magic,
                                        self.header_version,
                                        tree_offset,
                                        tree_size,
                                        idtable_offset,
                                        idtable_size,
                                        idhashtable_offset,
                                        idhashtable_size,
                                        stringtable_offset,
                                        stringtable_size,
                                        wstringtable_offset,
                                        wstringtable_size,
                                        hashtable_offset,
                                        hashtable_size,
                                        intarraytable_offset,
                                        intarraytable_size,
                                        floatarraytable_offset,
                                        floatarraytable_size,
                                        filetable_offset,
                                        filetable_size
                                      )

        if self.error_count == 0:
            self.writer.write( self.header_bin )
            self.writer.write( self.tree_bin )
            self.writer.write( self.idtable_bin )
            self.writer.write( self.idhashtable_bin )
            self.writer.write( self.stringtable_bin )
            self.writer.write( self.wstringtable_bin )
            self.writer.write( self.hashtable_bin )
            self.writer.write( self.intarraytable_bin )
            self.writer.write( self.floatarraytable_bin )
            self.writer.write( self.filetable_bin )
            self.writer.flush()
            return 0

        else:
            return 1

    # エラーが発生したことを記録しておく
    def set_error(self):
        self.error_count += 1

    # ファイルテーブルに格納するファイルのリストを取得する
    def get_file_list(self):
        return self.file_table.keys()

    # keyのsha-1ハッシュ値上位4byteを計算する
    def calc_id_hash_upper4( self, key ):
        hashval = sha1(key).hexdigest()
        hashval_upper4 = int(hashval[0:8], 16)
        return hashval_upper4        
