#!/usr/bin/python

import re
from optparse import OptionParser
from sys import stdout, stderr, argv, exit
from itertools import chain


class ShaderVariableInfo:
    """ shader argument/parameter info we can find in glsl files output by cgc """

    def __init__(self,line):
    
        # replace non-indexed semantics with 0-indexed version
        replace_non_indexed = True
        if replace_non_indexed:
            semantics = [ 'TEXCOORD', 'COLOR', 'DIFFUSE', 'SPECULAR', 'FOG', 'FOGC', 'FOGCOORD' ]
            for s in semantics:
                line = line.replace( s + ',', s + '0,' )
                line = line.replace( s + ' ', s + '0 ' )

        # replace cg3.0 attributes with cg2.2 equivalents for now
        # http://http.developer.nvidia.com/Cg/vp40.html
        # "The two sets act as aliases to each other on NVIDIA GPUs excluding Apple Macs. ATI GPUs and NVIDIA Mac GPUs do not alias the conventional vertex attributes with the generic attributes."
        enable_cg3_0_to_cg2_2_conversion=False
        if enable_cg3_0_to_cg2_2_conversion:
            convert_3_0_to_2_2 = {
                'ATTR0' : 'POSITION',
                'ATTR1' : 'BLENDWEIGHT',
                'ATTR2' : 'NORMAL',
                'ATTR3' : 'DIFFUSE',
                'ATTR4' : 'SPECULAR',
                'ATTR5' : 'FOGCOORD',
                'ATTR6' : 'PSIZE',
                'ATTR7' : 'BLENDINDICES',
                'ATTR8' : 'TEXCOORD0',
                'ATTR9' : 'TEXCOORD1',
                'ATTR10' : 'TEXCOORD2',
                'ATTR11' : 'TEXCOORD3',
                'ATTR12' : 'TEXCOORD4',
                'ATTR13' : 'TEXCOORD5',
                'ATTR14' : 'TEXCOORD6', # WARNING: also used for TANGENT
                'ATTR15' : 'TEXCOORD7', # WARNING: also used for BINORMAL
            }

            for kv in convert_3_0_to_2_2.items():
                line = line.replace(kv[0], kv[1])

        # remember the full line of glsl code for debug
        self.m_line = line
    
        self.m_words = []
    
        #print >> stderr, "line = [%s]" % ( line )
    
        # remove front "var_head" and split on :
        self.m_words = line[len(var_head):].split(":")

        # NOTE: not necessary at the moment, we use the data as-is
        # TEXUNIT attributes have on less ':' than other variables
        # simply replace the empty spot so we are aligned
        #if 'TEXUNIT' in self.m_words[1]:
            #old = self.m_words[:]
            #self.m_words = list(chain(self.m_words[0:1], [''], self.m_words[1:]))

        # strip front/end spaces on each bit
        for i in range(0, len(self.m_words)):
            self.m_words[i] = self.m_words[i].strip()
    
        return
    
    def dump (self,f):
        print >> f, ""
        print >> f, "// %s" % self.m_line
        print >> f, "// ",
        for w in self.m_words:
            print >> f, "[%s]" % w,
        print >> f, ""
        print >> f, "// type      = [%s]" % self.get_cg_type()
        print >> f, "// name      = [%s]" % self.get_name()
        print >> f, "// pure name = [%s]" % self.get_pure_name()
        print >> f, "// glsl name = [%s]" % self.get_glsl_name()
        print >> f, "// vin/vout  = [%s]" % self.get_vin_vout()

    def strip_array_index(self, name):
        if '[' in name:
            return name[0:name.find('[')]
        return name
    
    def get_pure_name(self):
        tmp = self.get_name().split(".")
        result = tmp[len(tmp)-1]
        result = self.strip_array_index(result)
        return result
    
    def get_name(self):
        return self.m_words[0].split()[1]
    
    def get_cg_type(self):
        return self.m_words[0].split()[0]

    def get_glsl_type(self):
        typemap = {'float':'float', 'float4':'vec4', 'float3':'vec3', 'float2':'vec2','float4x4':'mat4', 'float3x3':'mat3', 'float2x2':'mat2'}
        return typemap[self.get_cg_type()]
    
    def get_glsl_name(self):
        word = self.m_words[2]

        # workaround for DIFFUSE0 and SPECULAR0 and FOCGOORD0 producing glsl names with a $ prefix
        word = word.replace( '$DIFFUSE0', 'DIFFUSE0' )
        word = word.replace( '$SPECULAR0', 'SPECULAR0' )
        word = word.replace( '$FOGCOORD', 'FOGCOORD' )
        word = word.replace( '$FOGCOORD0', 'FOGCOORD0' )
        word = word.replace( '$FOGC', 'FOGC' )
        word = word.replace( '$FOGC0', 'FOGC0' )
        word = word.replace( '$BINORMAL', 'BINORMAL' )
        word = word.replace( '$BINORMAL0', 'BINORMAL0' )
        word = word.replace( '$TANGENT', 'TANGENT' )
        word = word.replace( '$BLENDINDICES', 'BLENDINDICES' )
        word = word.replace( '$BLENDINDICES0', 'BLENDINDICES0' )
        word = word.replace( '$BLENDWEIGHT', 'BLENDWEIGHT' )
        word = word.replace( '$BLENDWEIGHT0', 'BLENDWEIGHT0' )
        word = word.replace( '$TESSFACTOR', 'TESSFACTOR' )
        word = word.replace( '$PSIZE', 'PSIZE' )
        word = word.replace( '$PSIZE0', 'PSIZE0' )

        # glsl name does not include array index
        word = self.strip_array_index(word)
        # matrices generate different syntax and no mangled name
        if word.strip().startswith(','):
            return ''

        # uniform samplers generate syntax a [2] like '_Texture0 0'
        # the first value is the name, which is missing when the value is unused
        # the second value is the TEXUNIT, which is missing when unspecified
        # we need to check the names and extract the correct one
        # when unused, they simply generate '0', so we mustn't inadvertently treat that as the 
        # name to replace (otherwise we replace all 0s)
        if self.m_words[0].startswith('sampler'):
            texture_words = self.m_words[2].split(' ')

            # texname + texunit
            if len(texture_words) > 1:
                return texture_words[0]

            if len(texture_words) == 1:
                # texunit only
                if texture_words[0].isdigit():
                    return ""
                # texname only
                else:
                    return texture_words[0]

        # no semantic uniforms generate no name for [2]
        if word == '':
            return self.m_words[0].split()[1]

        return word.split()[0]

    def get_vin_vout(self):
        tmp = self.m_words[1].split(".")[0]
        if tmp == '$vin': return 'in'
        if tmp == '$vout': return 'out'
        return ''

var_head = "//var "


def parse_shader_variables_info( input_filename, verbose_output ):
    """ Parse *.glsl files generated by cgc to extract parameters informations.  """

    input_file = file( input_filename, "r" )
   
    shader_variables = []

    try:

        for line in input_file:
            line = line.replace("\n","")
            #print >> stderr, "line = [%s]" % ( line )
            if line.startswith(var_head):
                var_info = ShaderVariableInfo(line)
                shader_variables.append( var_info )
                if verbose_output:
                    var_info.dump(stdout)                
    finally:
        input_file.close()

    return shader_variables


def cg_glsl_to_es20_glsl( input_filename, verbose_output ):
    """
    Convert the glsl files output by cgc into OpenGL ES2.0 glsl code

     - "attribute and varying" lines declarations are inserted in glsl code

     - replace the old glsl attributes by the proper user variables: the obsolete gl_Vertex, gl_Normal etc are replaced
       by user variables in ES2.0

     - comment non OpenGL ES2.0 bits, for example following lines of code will be commented
       since they are irrelevant to ES2.0:

        gl_FrontColor = vec4( 1.00000000E+000, 0.00000000E+000, 0.00000000E+000, 1.00000000E+000);
        gl_TexCoord[0].xy = gl_MultiTexCoord0.xy;

    """

    delwords = []

    shader_variables = parse_shader_variables_info( input_filename, verbose_output )

    input_file = file( input_filename, "r" )

    try:

        vin_semantics = {}
        vout_semantics = {}

        header_comment=True

        # transformations to workaround bad/akward cgc output
        demangle_names=True
        convert_vec_to_mat=True
        remove_texcoord_float_x=True
        enable_dynamic_symbol_indexing_fixup=True

        profile = ""

        for line in input_file:

            stripped_line = line.replace("\n","").strip()
            if stripped_line == "//profile glslv": profile = "glslv" 
            if stripped_line == "//profile glslf": profile = "glslf" 

            if header_comment and not (stripped_line == "" or stripped_line.startswith('//')):

                header_comment = False

                precision_specifier = " "
                #if profile == "glslf": precision_specifier = " mediump "
                #if profile == "glslf": print "precision mediump float;"
                if profile == "glslf": print "precision highp float;"
                if profile == "glslf": print "precision highp int;"

                # cg generated comment is finished, add devlaration for attributes, OpenGL ES2.0 style

                ## print
                ## print "// --------- attribute section added by cg_glsl_to_es20_glsl.py"
                ## print

                # TODO: find out if this list is accurate for NGP
                # POSITION              done
                # BLENDWEIGHT           todo
                # NORMAL                done
                # COLOR                 done
                # TESSFACTOR            todo
                # PSIZE                 done
                # BLENDINDICES          todo
                # TEXCOORD              todo
                # TANGENT               todo
                # BINORMAL              todo
                # POSITION              done
                # FOG                   todo
                # COLOR0-COLOR1         
                # TEXCOORD0-TEXCOORD7   done

                # TODO: find documentation and extensive list of supported semantics
                semantic_list = [
                    'POSITION', 'HPOS',
                    'COLOR0', 'COL0', 'DIFFUSE0',
                    'COLOR1', 'COL1', 'SPECULAR0',
                    'TEXCOORD0', 'TEX0', 'TEXCOOD0_CENTROID',
                    'TEXCOORD1', 'TEX1', 'TEXCOOD1_CENTROID',
                    'TEXCOORD2', 'TEX2', 'TEXCOOD2_CENTROID',
                    'TEXCOORD3', 'TEX3', 'TEXCOOD3_CENTROID',
                    'TEXCOORD4', 'TEX4', 'TEXCOOD4_CENTROID',
                    'TEXCOORD5', 'TEX5', 'TEXCOOD5_CENTROID',
                    'TEXCOORD6', 'TEX6', 'TEXCOOD6_CENTROID',
                    'TEXCOORD7', 'TEX7', 'TEXCOOD7_CENTROID',
                    'TEXCOORD8', 'TEX8', 'TEXCOOD8_CENTROID',
                    'TEXCOORD9', 'TEX9', 'TEXCOOD9_CENTROID',
                    'FOG', 'FOGC0', 'FOGCOORD', 'FOGCOORD0',
                    'CLP0',
                    'CLP1',
                    'CLP2',
                    'CLP3',
                    'CLP4',
                    'CLP5',
                    'CLP6',
                    'CLP7',
                    'NORMAL',
                    'BINORMAL', 'BINORMAL0',
                    'TANGENT', 'TANGENT0',
                    #'TESSFACTOR', # not supported on NGP
                    'BLENDWEIGHT', 'BLENDWEIGHT0',
                    'BLENDINDICES', 'BLENDINDICES0',
                    'PSIZE', 'PSIZE0',

                    'ATTR0',  # POSITION
                    'ATTR1',  # BLENDWEIGHT
                    'ATTR2',  # NORMAL
                    'ATTR3',  # DIFFUSE
                    'ATTR4',  # SPECULAR
                    'ATTR5',  # TESSFACTOR
                    'ATTR6',  # PSIZE
                    'ATTR7',  # BLENDINDICES
                    'ATTR8',  # TEXCOORD0
                    'ATTR9',  # TEXCOORD1
                    'ATTR10', # TEXCOORD2
                    'ATTR11', # TEXCOORD3
                    'ATTR12', # TEXCOORD4
                    'ATTR13', # TEXCOORD5
                    'ATTR14', # TEXCOORD6, TANGENT
                    'ATTR15', # TEXCOORD7, BINORMAL
                    ]

                for v in shader_variables:
                    if v.get_glsl_name() in semantic_list and v.get_vin_vout() == 'in':
                        if profile == "glslv": print "attribute " + v.get_glsl_type() + " " + v.get_pure_name() + ";"
                        if profile == "glslf": print "varying" + precision_specifier + v.get_glsl_type() + " " + v.get_pure_name() + ";"
                        vin_semantics [ v.get_glsl_name() ] = v.get_pure_name()

                for v in shader_variables:
                    if v.get_glsl_name() in semantic_list and v.get_vin_vout() == 'out':
                        if profile == "glslv": print "varying" + precision_specifier + v.get_glsl_type() + " " + v.get_pure_name() + ";"
                        vout_semantics [ v.get_glsl_name() ] = v.get_pure_name()

                #print vin_semantics
                #print vout_semantics

                ## print
                ## print '// ---------'
                ## print

            if header_comment:

                # 'line' is a cg header comment line

                print line.replace("\n","")

            else:
                # 'line' is a shader code line

                not_supported = "// Not supported in OpenGL ES2.0: "

                #if stripped_line.startswith('gl_FrontColor')):
                #    print not_supported + "//" + line.replace("\n","")
                #    continue

                #if stripped_line.startswith('gl_TexCoord')):
                #    print not_supported + "//" + line.replace("\n","")
                #    continue
                
                filtered_line = line.replace("\n","")

                # reference (see above for more details)
                #'ATTR0',  # POSITION
                #'ATTR1',  # BLENDWEIGHT
                #'ATTR2',  # NORMAL
                #'ATTR3',  # DIFFUSE
                #'ATTR4',  # SPECULAR
                #'ATTR5',  # TESSFACTOR
                #'ATTR6',  # PSIZE
                #'ATTR7',  # BLENDINDICES
                #'ATTR8',  # TEXCOORD0
                #'ATTR9',  # TEXCOORD1
                #'ATTR10', # TEXCOORD2
                #'ATTR11', # TEXCOORD3
                #'ATTR12', # TEXCOORD4
                #'ATTR13', # TEXCOORD5
                #'ATTR14', # TEXCOORD6, TANGENT
                #'ATTR15', # TEXCOORD7, BINORMAL

                # cg3.0 support test
                if vin_semantics.has_key('ATTR0'): filtered_line = filtered_line.replace("gl_Vertex",vin_semantics['ATTR0'])
                #if vin_semantics.has_key('ATTR1'): filtered_line = filtered_line.replace("gl_???",vin_semantics['ATTR1'])
                if vin_semantics.has_key('ATTR2'): filtered_line = filtered_line.replace("gl_Normal",vin_semantics['ATTR2'])
                if vin_semantics.has_key('ATTR3'): filtered_line = filtered_line.replace("gl_Color",vin_semantics['ATTR3'])
                if vin_semantics.has_key('ATTR4'): filtered_line = filtered_line.replace("gl_SecondaryColor",vin_semantics['ATTR4'])
                #if vin_semantics.has_key('ATTR5'): filtered_line = filtered_line.replace("gl_???",vin_semantics['ATTR5'])
                #if vin_semantics.has_key('ATTR6'): filtered_line = filtered_line.replace("gl_???",vin_semantics['ATTR6'])
                #if vin_semantics.has_key('ATTR7'): filtered_line = filtered_line.replace("gl_???",vin_semantics['ATTR7'])
                if vin_semantics.has_key('ATTR8'): filtered_line = filtered_line.replace("gl_MultiTexCoord0",vin_semantics['ATTR8'])
                if vin_semantics.has_key('ATTR9'): filtered_line = filtered_line.replace("gl_MultiTexCoord1",vin_semantics['ATTR9'])
                if vin_semantics.has_key('ATTR10'): filtered_line = filtered_line.replace("gl_MultiTexCoord2",vin_semantics['ATTR10'])
                if vin_semantics.has_key('ATTR11'): filtered_line = filtered_line.replace("gl_MultiTexCoord3",vin_semantics['ATTR11'])
                if vin_semantics.has_key('ATTR12'): filtered_line = filtered_line.replace("gl_MultiTexCoord4",vin_semantics['ATTR12'])
                if vin_semantics.has_key('ATTR13'): filtered_line = filtered_line.replace("gl_MultiTexCoord5",vin_semantics['ATTR13'])
                if vin_semantics.has_key('ATTR14'): filtered_line = filtered_line.replace("gl_MultiTexCoord6",vin_semantics['ATTR14'])
                if vin_semantics.has_key('ATTR15'): filtered_line = filtered_line.replace("gl_MultiTexCoord7",vin_semantics['ATTR15'])

                # if same variable detected. remove declaration
                if 'attribute' in filtered_line:
                    rep = filtered_line.replace(';', '')
                    delwords = rep.split(' ')
                    filtered_line = filtered_line.replace(filtered_line, '')

                # Measures Incorrect conversion float to float4 in source code.
                noa = len(delwords)
                if noa > 0:
                    if filtered_line.find(delwords[2]) != -1:
                        no = filtered_line.count('.x,')
                        no += filtered_line.count('.x)')
                        if no == 4:
                            filtered_line = filtered_line.replace('.x', '')

                if vin_semantics.has_key('POSITION'): 
                    filtered_line = filtered_line.replace("gl_Vertex",vin_semantics['POSITION'])

                if vin_semantics.has_key('COLOR0'): 
                    filtered_line = filtered_line.replace("gl_Color",vin_semantics['COLOR0'])

                if vin_semantics.has_key('COLOR1'): 
                    filtered_line = filtered_line.replace("gl_SecondaryColor",vin_semantics['COLOR1'])

                if vin_semantics.has_key('DIFFUSE0'): 
                    filtered_line = filtered_line.replace("gl_Color",vin_semantics['DIFFUSE0'])

                if vin_semantics.has_key('SPECULAR0'): 
                    filtered_line = filtered_line.replace("gl_SecondaryColor",vin_semantics['SPECULAR0'])

                if vin_semantics.has_key('NORMAL'): 
                    filtered_line = filtered_line.replace("gl_Normal",vin_semantics['NORMAL'])

                if vin_semantics.has_key('FOGC0'): 
                    filtered_line = filtered_line.replace("gl_FogFragCoord",vin_semantics['FOGC0'])

                if vin_semantics.has_key('FOGCOORD'): 
                    filtered_line = filtered_line.replace("gl_FogCoord",vin_semantics['FOGCOORD'])

                if vin_semantics.has_key('FOGCOORD0'): 
                    filtered_line = filtered_line.replace("gl_FogCoord",vin_semantics['FOGCOORD0'])

                for index in range(16):
                    texcoord_semantic = "TEXCOORD" + str(index)
                    texcoord_vert_semantic = "gl_MultiTexCoord" + str(index)
                    texcoord_frag_semantic = "gl_TexCoord[" + str(index) + "]"
                    if vin_semantics.has_key(texcoord_semantic):
                        if profile == "glslv": filtered_line = filtered_line.replace(texcoord_vert_semantic,vin_semantics[texcoord_semantic])
                        if profile == "glslf": filtered_line = filtered_line.replace(texcoord_frag_semantic,vin_semantics[texcoord_semantic])
                    if vout_semantics.has_key(texcoord_semantic):
                        filtered_line = filtered_line.replace(texcoord_frag_semantic,vout_semantics[texcoord_semantic])

                if vout_semantics.has_key('COLOR'):
                    filtered_line = filtered_line.replace("gl_FrontColor",vout_semantics['COLOR'])

                if vout_semantics.has_key('COLOR0'):
                    filtered_line = filtered_line.replace("gl_FrontColor",vout_semantics['COLOR0'])

                if vout_semantics.has_key('COLOR1'):
                    filtered_line = filtered_line.replace("gl_FrontSecondaryColor",vout_semantics['COLOR1'])

                if vout_semantics.has_key('NORMAL'): 
                    filtered_line = filtered_line.replace("gl_Normal",vout_semantics['NORMAL'])

                if vout_semantics.has_key('FOGC0'): 
                    filtered_line = filtered_line.replace("gl_FogFragCoord",vout_semantics['FOGC0'])

                if vout_semantics.has_key('FOGCOORD0'): 
                    filtered_line = filtered_line.replace("gl_FogFragCoord",vout_semantics['FOGCOORD0'])

                # this is special. Its not a varying that can be read by the frag shader and needs to stay as-is
                #if vout_semantics.has_key('PSIZE'): 
                #    filtered_line = filtered_line.replace("gl_PointSize",vout_semantics['PSIZE'])

                # replace mangled names with unmangled names
                if demangle_names:
                    for v in shader_variables:
                        glsl = v.get_glsl_name()
                        if glsl != '':
                            mangled = glsl.split()[0]
                            if mangled in filtered_line:
                                if verbose_output:
                                    print "// replaced", mangled, "with", v.get_pure_name()
                                filtered_line = filtered_line.replace(mangled, v.get_pure_name())

                # replace vecN with matN
                if convert_vec_to_mat:
                    # change declaration from vecN to matN
                    declaration_re = re.compile('(.*)vec([2-4])([^\[]+)\[([0-9]+)\];')
                    decl = declaration_re.match(filtered_line)
                    if decl:
                        filler0 = decl.group(1)
                        dimensions = int(decl.group(2))
                        filler1 = decl.group(3)

                        for v in shader_variables:
                            if v.get_pure_name() == filler1.strip():
                                matrix_types = [ 'float4x4', 'float3x3', 'float2x2' ]
                                if v.get_cg_type() in matrix_types:
                                    elements = int(decl.group(4)) / dimensions
                                    elements_suffix = ''
                                    if elements != 1:
                                        elements_suffix = '[%d]' % (elements)
                                    filtered_line = '%smat%d%s%s;' % (filler0, dimensions, filler1, elements_suffix)

                    '''
                    # replace var[(4*N + M)] indexing with var[N][(M)]
                    for v in shader_variables:
                        if v.get_pure_name() in filtered_line:
                            cg_types = { 'float4x4':4, 'float3x3':3, 'float2x2':2 }
                            if cg_types.has_key(v.get_cg_type()):
                                dimension = cg_types[v.get_cg_type()]
                                for index in xrange(0, 128):
                                    src = '%s[(%d*%d + ' % (v.get_pure_name(), dimension, index)
                                    dst = '%s[%d][(' % (v.get_pure_name(), index)
                                    filtered_line = filtered_line.replace(src, dst)

                    # replace var[(4*symbol + M)] indexing with var[symbol][(M)]
                    if enable_dynamic_symbol_indexing_fixup:
                        for v in shader_variables:
                            if v.get_pure_name() in filtered_line:
                                cg_types = { 'float4x4':4, 'float3x3':3, 'float2x2':2 }
                                if cg_types.has_key(v.get_cg_type()):
                                    dimension = cg_types[v.get_cg_type()]

                                    # Generate line initialize
                                    target = filtered_line                # copy original
                                    no0 = target.find(' = ')              # search unnecessary character
                                    no1 = target.find(' + ')              # search index No
                                    indexNo = target[no1+3]               # set index No

                                    # Change string formatting
                                    l0 = target[0:no0] + ' = '            # set non target string
                                    target = target.lstrip()              # remove space
                                    rmstr = target[0:no0]                 # set remove string
                                    target = target.replace(rmstr, '')    # remove string

                                    # check if there is a symbol name using this scary regex
                                    symbol_finder = re.compile('[a-zA-Z0-9]+\[\([1-4]+\*([a-zA-Z0-9.]+)\ \+')
                                    match_ = symbol_finder.match(target)
                                    if match_ != None:
                                        symbol = match_.groups()
                                        if match_ != None and symbol != None:
                                            d0 = v.get_pure_name()
                                            d1 = '[%s]' % (symbol)
                                            d2 = '[%s];' % (indexNo)
                                            d3 = d0 + d1 + d2
                                            filtered_line = l0 + d3
                    '''

                    # replace var[(4*N + M)] indexing with var[N][(M)]
                    search = re.search(r'\[\(\d\*.+ \+ \d\)\]',filtered_line)          # array index
                    if search != None:
                        search2 = re.search(r'\w+\Z',filtered_line[0:search.start()])   # array name
                        if search2 != None:
                            for v in shader_variables:
                                if v.get_pure_name() == search2.group():
                                    cg_types = { 'float4x4':4, 'float3x3':3, 'float2x2':2 }
                                    if cg_types.has_key(v.get_cg_type()):               # is matrix
                                        filtered_line = filtered_line[0:search.start()+1] + search.group()[4:-6] + '][(' + filtered_line[search.end()-3:]
                                    break

                # replace var.x with var, for single dimension texcoord floats
                if remove_texcoord_float_x:
                    for v in shader_variables:
                        if v.get_pure_name() in filtered_line:
                            if 'TEXCOORD' in v.get_glsl_name():
                                if v.get_cg_type() == 'float':
                                    if verbose_output:
                                        print '// replacing %s.x with %s' % (v.get_pure_name(), v.get_pure_name())
                                    filtered_line = filtered_line.replace('%s.x' % v.get_pure_name(), '%s' % v.get_pure_name())

                # saturate the color semantic varying variables
                if filtered_line == '} // main end' and profile == "glslv":
                    for v in shader_variables:
                        if v.get_vin_vout() == 'out' and v.get_glsl_name().startswith('COLOR'):
                            print '    ' + v.get_pure_name() + ' = clamp(' + v.get_pure_name() + ',0.0,1.0);'

                print filtered_line

        
        #for var in shader_variables:
            #var.dump(stdout)

    finally:
        input_file.close()


def main():

    parser = OptionParser()
    parser.add_option('-v', '--verbose', action="store_true", dest="verbose", default=False, help="verbose output")

    (options, files) = parser.parse_args()

    for f in files:
        try:
            if options.verbose:
                print 'processing %s...' % f
            cg_glsl_to_es20_glsl(f, options.verbose)
            if options.verbose:
                print 'success'

        except Exception, e:
            print 'failed: %s' % str(e)
            exit(1)

    return

if __name__ == "__main__":
    main()

