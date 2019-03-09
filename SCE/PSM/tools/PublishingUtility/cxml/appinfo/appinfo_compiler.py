# -*- coding: utf_8 -*-
#
# SCE CONFIDENTIAL
#
#
#      Copyright (C) 2010 Sony Computer Entertainment Inc.
#                        All Rights Reserved.
#
#                  Cxml Compiler - sample compiler
#
#	Usage:
#	python cxml_compilertest.py [-o output file(*.rco)] input file(*.xml)
#

import os
import sys
import struct

# 別ディレクトリにあるcxmlモジュールをimportするために、sys.pathにディレクトリを追加する
this_module_dir = os.path.dirname( os.path.join( os.getcwd(), sys.argv[0] ) )
sys.path.append( os.path.normpath(os.path.join( this_module_dir, "../compiler" )) )
#sys.path.append( os.path.normpath(os.path.join( this_module_dir, "./" )) )

import cxml


# CXMLコンパイラを継承したサンプルコンパイラ
class sample_compiler( cxml.xmlcompiler ):

    # コンストラクタ
    def __init__( self, reader, writer, file_provider=None, reporter=None, base_path="" ):

        if file_provider==None:
            def file_provide_handler( path, node, attr ):
                return file( path, "rb" )
            file_provider = file_provide_handler

        if reporter==None:
            reporter = sys.stdout


        # 基底クラスのコンストラクタ
        cxml.xmlcompiler.__init__( self, reader, writer, file_provider, reporter, base_path )

        # マジックナンバーを設定
        self.set_magic( "PSMA" )

        # バージョン番号を設定
        self.set_version( 0x0100 )


        # スキーマを追加する
        # <application>
        self.append_schema(
            cxml.schemaitem( "application", None, None,
                             {
                                "project_name" : cxml.attr_type_string,
                                "version" : cxml.attr_type_string,
                                "runtime_version" : cxml.attr_type_string,
                                "sdk_version" : cxml.attr_type_string,
                                "default_locale" : cxml.attr_type_string,
                             }
                           )
        )

        # <name>
        # []でこの要素の親を指定する。
        # {}でこの要素の属性を指定する。
        self.append_schema(
            cxml.schemaitem( "name", [ "application", "developer", "product" ], None,
                             {
                                "value"   : cxml.attr_type_string,
                             }
                           )
        )

        # <localized_item>
        self.append_schema(
            cxml.schemaitem( "localized_item", [ "name", "short_name" ], None,
                             {
                                "locale"  : cxml.attr_type_string,
                                "value"   : cxml.attr_type_string,
                             }
                           )
        )

        # <short_name>
        self.append_schema(
            cxml.schemaitem( "short_name", [ "application" ], None,
                             {
                                "value"   : cxml.attr_type_string,
                             }
                           )
        )

        # <parental_control>
        self.append_schema(
            cxml.schemaitem( "parental_control", [ "application" ], None,
                             {
                                "lock_level"      : cxml.attr_type_int,
                             }
                           )
        )

        # <rating_list>
        self.append_schema(
            cxml.schemaitem( "rating_list", [ "application" ], None,
                             {
                                 "highest_age_limit"   : cxml.attr_type_int,
                                 "has_online_features" : cxml.attr_type_string,
                             }
                           )
        )
        

        # <online_features>
        self.append_schema(
            cxml.schemaitem( "online_features", [ "rating_list" ], None,
                             {
                                "chat"              : cxml.attr_type_string,
                                "personal_info"     : cxml.attr_type_string,
                                "user_location"     : cxml.attr_type_string,
                                "exchange_content"  : cxml.attr_type_string,
                                "minimum_age"       : cxml.attr_type_int,
                             }
                           )
        )


        # <rating>
        self.append_schema(
            cxml.schemaitem( "rating", [ "rating_list" ], None,
                             {
                                "type"      : cxml.attr_type_string,
                                "value"     : cxml.attr_type_string,
                                "age"       : cxml.attr_type_string,
                                "code"      : cxml.attr_type_string,
                             }
                           )
        )

        # <descriptor>
        self.append_schema(
            cxml.schemaitem( "descriptor", [ "rating" ], None,
                             {
                                "__text__"      : cxml.attr_type_string,
                             }
                           )
        )


        # <images>
        self.append_schema(
            cxml.schemaitem( "images", [ "application" ], None,
                             {
                                "icon_128x128"      : cxml.attr_type_filename,
                                "icon_256x256"      : cxml.attr_type_filename,
                                "icon_512x512"      : cxml.attr_type_filename,
                                "splash_854x480"    : cxml.attr_type_filename,
                             }
                           )
        )

        # <genre_list>
        self.append_schema(
            cxml.schemaitem( "genre_list", [ "application" ], None,
                             {
                             }
                           )
        )

        # <genre>
        self.append_schema(
            cxml.schemaitem( "genre", [ "genre_list" ], None,
                             {
                                "value"      : cxml.attr_type_string,
                             }
                           )
        )

        # <developer>
        self.append_schema(
            cxml.schemaitem( "developer", [ "application" ], None,
                             {
                             }
                           )
        )

        # <website>
        self.append_schema(
            cxml.schemaitem( "website", [ "application" ], None,
                             {
                                "href" : cxml.attr_type_string,
                             }
                           )
        )

        # <copyright>
        self.append_schema(
            cxml.schemaitem( "copyright", [ "application" ], None,
                             {
                                "author" : cxml.attr_type_string,
                                "text"   : cxml.attr_type_filename,
                             }
                           )
        )

        # <purchase>
        self.append_schema(
            cxml.schemaitem( "purchase", [ "application" ], None,
                             {
                             }
                           )
        )

        # <product_list>
        self.append_schema(
            cxml.schemaitem( "product_list", [ "purchase" ], None,
                             {
                             }
                           )
        )

        # <product>
        self.append_schema(
            cxml.schemaitem( "product", [ "product_list" ], None,
                             {
                                "label" : cxml.attr_type_string,
                                "type"  : cxml.attr_type_string,
                             }
                           )
        )

        # <runtime_config>
        self.append_schema(
            cxml.schemaitem( "runtime_config", [ "application" ], None,
                             {
                             }
                           )
        )

        # <memory>
        self.append_schema(
            cxml.schemaitem( "memory", [ "runtime_config" ], None,
                             {
                                 "resource_heap_size"   : cxml.attr_type_int,
                                 "managed_heap_size"   : cxml.attr_type_int,
                             }
                           )
        )


        # <display>
        self.append_schema(
            cxml.schemaitem( "display", [ "runtime_config" ], None,
                             {
                                 "max_screen_size"   : cxml.attr_type_string,
                             }
                           )
        )

        # <camera>
        self.append_schema(
            cxml.schemaitem( "camera", [ "runtime_config" ], None,
                             {
                                 "max_capture_resolution"   : cxml.attr_type_string,
                             }
                           )
        )



        # <feature_list>
        self.append_schema(
            cxml.schemaitem( "feature_list", [ "application" ], None,
                             {
                             }
                           )
        )



        # <feature>
        self.append_schema(
            cxml.schemaitem( "feature", [ "feature_list" ], None,
                             {
                                 "value"   : cxml.attr_type_string,
                             }
                           )
        )


        # <psn_service_list>
        self.append_schema(
            cxml.schemaitem( "psn_service_list", [ "application" ], None,
                             {
                             }
                           )
        )


        # <feature>
        self.append_schema(
            cxml.schemaitem( "psn_service", [ "psn_service_list" ], None,
                             {
                                 "value"   : cxml.attr_type_string,
                             }
                           )
        )



    def preprocess(self):
        pass




# メイン関数
if __name__ == '__main__':

    import os
    import getopt
    
    print "==appinfo_compiler.py=="
    
    # 引数を解釈する
    try:
        options, args = getopt.getopt( sys.argv[1:], 'o:', [ "depend=", "endian=" ] )
    except getopt.GetoptError, e:
        print e
        sys.exit(1)

    dst = None
    depend_file = None
    big_endian = False  # WIN32環境ではリトルエンディアン

    # オプション設定
    for option in options :
        if option[0] in ( "-o", ) :
            dst = option[1]
        elif option[0] in ( "--depend", ) :
            depend_file = option[1]
        elif option[0] in ( "--endian", ) :
            if option[1]=="little":
                big_endian = False
            elif option[1]=="big":
                big_endian = True
            else:
                print "unknown endian name", option[1]
        else:
            print "unknown option ", option[0]

    src = args[0]

    if not dst:
        dst = os.path.splitext(src)[0]+".cxml"

    tmp_file = os.path.splitext(dst)[0]+".tmp"

    writer = file( tmp_file, "wb" )
    file_provider=None

    # sample_compilerオブジェクトの作成
    compiler = sample_compiler(
                file(src,"r"),
                writer,
                file_provider,
                base_path=unicode(os.path.dirname(src), "mbcs")
               )

    # エンディアンを設定
    if big_endian:
        compiler.set_big_endian()


    # コンパイル
    result = compiler.compile()


    if depend_file:
        fd = file( depend_file, "w" )
        fd.write( dst + ": " + src )
        for path in compiler.get_file_list():
            fd.write( " \\\n  " + path )
        del fd

    del compiler
    writer.close()

    if result:
        os.remove(tmp_file)
        sys.exit(result)

    try:
        os.remove(dst)
    except:
        pass
    os.rename(tmp_file,dst)

    sys.exit(result)
