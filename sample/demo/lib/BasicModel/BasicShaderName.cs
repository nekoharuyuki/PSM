// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

namespace DemoModel{

/// ShaderContainer に BasicShader の派生シェーダを格納するためのキー
public class BasicShaderName
{
    /// ライト種別
    public
    enum LightingType{
        kLIGHTING_NONE     = 0,
        kLIGHTING_VERTEX   = 1,
        kLIGHTING_FLAGMENT = 2,
    }

    /// 法線マップ種別
    public
    enum NormalMapType{
        kNORMALMAP_NONE    = 0,
        kNORMALMAP_VERTEX  = 1,
        kNORMALMAP_IMAGE   = 2,
    }


    private int MTXPLT_COUNT;
    private bool ENABLE_VERTEX_COLOR;
    private int TEX_COUNT;
    private bool ENABLE_TEXANIM;
    private int LIGHT_COUNT;
    private LightingType LIGHTING_TYPE;
    private NormalMapType NORMALMAP_TYPE;
    private int[] TEXCOORD = { 0, 0, 0 };
    private TexType[] TEXTYPE = { TexType.kTEXTYPE_NONE, TexType.kTEXTYPE_NONE, TexType.kTEXTYPE_NONE };
    private TexBlendFunc[] BLENDFUNC = { TexBlendFunc.kBLEND_NONE, TexBlendFunc.kBLEND_NONE, TexBlendFunc.kBLEND_NONE };

    private bool ENABLE_ALPHA_THRESHHOLD;

    private string key;
    private string vsName;
    private string fsName;

    /// コンストラクタ
    public BasicShaderName()
    {
    }


    public BasicShaderName( int worldCount, int lightCount, BasicMaterial material )
    {
        BasicLayer[] layers = material.Layers;

        int texCount = (layers.Length <= 3) ? layers.Length : 3;
        // bool texAnim = material.UseTexAnim;

        bool a_useTexAnim = false;
        // basic
        int a_lightCount = 0;
        LightingType a_lightType = LightingType.kLIGHTING_NONE;
        NormalMapType a_normalMapType = NormalMapType.kNORMALMAP_NONE;
        if( material.LightEnable != 0 ){
            a_lightCount = lightCount;
            a_lightType = LightingType.kLIGHTING_FLAGMENT;
            a_normalMapType = NormalMapType.kNORMALMAP_NONE;
        }

        
        int[] texCoords = { 0, 0, 0 };
        TexType[] texTypes = { 0, 0, 0 };
        TexBlendFunc[] texBlendFuncs = { 0, 0, 0 };
        int layerCount = (layers.Length <= 3) ? layers.Length : 3;
        for( int i = 0; i < layerCount; i++ ){
            BasicLayer layer = layers[ i ];

            // ここが無駄
            if( (TexType)layer.TexType != TexType.kTEXTYPE_NONE ){
                if( (TexType)layer.TexType == TexType.kTEXTYPE_VERTEXNORMAL ){
                    a_normalMapType = NormalMapType.kNORMALMAP_VERTEX;
                }else if( (TexType)layer.TexType == TexType.kTEXTYPE_IMAGENORMALL ){
                    a_normalMapType = NormalMapType.kNORMALMAP_IMAGE;
                }
            }
            texCoords[ i ] = layer.TexCoord;
            texTypes[ i ] = (TexType)layer.TexType;
            texBlendFuncs[ i ] = (TexBlendFunc)layer.TexBlendFunc;
            if( layer.TexAnimBoneIdx >= 0 ){
                a_useTexAnim = true;
            }
        }
        
        this.Set(
                 worldCount,
                 material.VertexColorEnable != 0,
                 texCount,
                 a_useTexAnim,
                 a_lightCount,
                 a_lightType,
                 a_normalMapType,
                 texCoords,
                 texTypes,
                 texBlendFuncs,
                 material.AlphaThreshold > 0.0f
                 );
    }
    /// 引数付コンストラクタ
    public BasicShaderName(
                           int MTXPLT_COUNT,
                           bool ENABLE_VERTEX_COLOR,
                           int TEX_COUNT,
                           bool ENABLE_TEXANIM,
                           int LIGHT_COUNT,
                           LightingType LIGHTING_TYPE,
                           NormalMapType NORMALMAP_TYPE,
                           int[] TEXCOORD,
                           TexType[] TEXTYPE,
                           TexBlendFunc[] BLENDFUNC,
                           bool ENABLE_ALPHA_THRESHHOLD
                           )
    {
        this.Set( MTXPLT_COUNT,
                  ENABLE_VERTEX_COLOR,
                  TEX_COUNT,
                  ENABLE_TEXANIM,
                  LIGHT_COUNT,
                  LIGHTING_TYPE,
                  NORMALMAP_TYPE,
                  TEXCOORD,
                  TEXTYPE,
                  BLENDFUNC,
                  ENABLE_ALPHA_THRESHHOLD );
    }

    public void Set(
                    int MTXPLT_COUNT,
                    bool ENABLE_VERTEX_COLOR,
                    int TEX_COUNT,
                    bool ENABLE_TEXANIM,
                    int LIGHT_COUNT,
                    LightingType LIGHTING_TYPE,
                    NormalMapType NORMALMAP_TYPE,
                    int[] TEXCOORD,
                    TexType[] TEXTYPE,
                    TexBlendFunc[] BLENDFUNC,
                    bool ENABLE_ALPHA_THRESHHOLD
                    )
    {
        this.MTXPLT_COUNT        = MTXPLT_COUNT;
        this.ENABLE_VERTEX_COLOR = ENABLE_VERTEX_COLOR;
        this.TEX_COUNT           = TEX_COUNT;
        this.ENABLE_TEXANIM      = ENABLE_TEXANIM;
        this.LIGHT_COUNT         = LIGHT_COUNT;
        this.LIGHTING_TYPE       = LIGHTING_TYPE;

        if( this.LIGHT_COUNT <= 0 ){
            this.LIGHTING_TYPE       = LightingType.kLIGHTING_NONE;
        }
        this.NORMALMAP_TYPE      = NORMALMAP_TYPE;
        for( int i = 0; i < this.TEXCOORD.Length; i++ ){
            if( i < TEX_COUNT ){
                this.TEXCOORD[i]          = TEXCOORD[i];
                this.TEXTYPE[i]           = TEXTYPE[i];
                this.BLENDFUNC[i]         = BLENDFUNC[i];
            }else{
                this.TEXCOORD[i]  = 0;
                this.TEXTYPE[i]   = TexType.kTEXTYPE_NONE;
                this.BLENDFUNC[i] = TexBlendFunc.kBLEND_NONE;
            }
        }
        this.ENABLE_ALPHA_THRESHHOLD = ENABLE_ALPHA_THRESHHOLD;
        makeNames();
    }

    public bool SetWorldAndLight( int worldCount, int lightCount )
    {
        bool isUpdate = false;
        if( this.MTXPLT_COUNT != worldCount ){
            this.MTXPLT_COUNT = worldCount;
            isUpdate = true;
        }
        if( this.LIGHT_COUNT != lightCount ){
            this.LIGHT_COUNT = lightCount;
            isUpdate = true;
        }

        if( isUpdate ){
            makeNames();
        }
        return true;
    }

    private void makeNames()
    {
        key = makeKey();
        vsName = makeVSName();
        fsName = makeFSName();
    }

    /// BasicProgram の名称のエンコード
    public string Key()
    {
        return key;
    }

    /// VertexShader のファイル名
    public string VSName()
    {
        return vsName;
    }

    /// Flagment Shader のファイル名
    public string FSName()
    {
        return fsName;
    }

    /// BasicProgram の名称のエンコード
    private string makeKey()
    {
        string name = "";
        name += "W" + MTXPLT_COUNT;
        name += "C" + (ENABLE_VERTEX_COLOR ? 1 : 0 );
        name += "L" + LIGHT_COUNT;
        name += toLightTypeCode( LIGHTING_TYPE ) + toNormalMapTypeCode( NORMALMAP_TYPE );
        name += "A" + (ENABLE_TEXANIM ? 1 : 0 );
        name += "T" + TEX_COUNT;
        name += toTexTypeCode( TEXTYPE[0] ) + TEXCOORD[0] + toTexBlendFuncCode( BLENDFUNC[0] );
        name += toTexTypeCode( TEXTYPE[1] ) + TEXCOORD[1] + toTexBlendFuncCode( BLENDFUNC[1] );
        name += toTexTypeCode( TEXTYPE[2] ) + TEXCOORD[2] + toTexBlendFuncCode( BLENDFUNC[2] );
        name += "A" + (ENABLE_ALPHA_THRESHHOLD ? 1 : 0);

        return name;
    }

    /// VertexShader のファイル名
    private string makeVSName()
    {
        string name = "";
        name += "W" + MTXPLT_COUNT;
        name += "C" + (ENABLE_VERTEX_COLOR ? 1 : 0 );
        name += "L" + LIGHT_COUNT;
        name += toLightTypeCode( LIGHTING_TYPE ) + toNormalMapTypeCode( NORMALMAP_TYPE );
        name += "A" + (ENABLE_TEXANIM ? 1 : 0 );
        name += "T" + TEX_COUNT;
        return name;
    }

    /// Flagment Shader のファイル名
    private string makeFSName()
    {
        string name = "";
        name += "C" + (ENABLE_VERTEX_COLOR ? 1 : 0 );
        name += "L" + LIGHT_COUNT;
        name += toLightTypeCode( LIGHTING_TYPE ) + toNormalMapTypeCode( NORMALMAP_TYPE );
        name += "T" + TEX_COUNT;
        name += toTexTypeCode( TEXTYPE[0] ) + TEXCOORD[0] + toTexBlendFuncCode( BLENDFUNC[0] );
        name += toTexTypeCode( TEXTYPE[1] ) + TEXCOORD[1] + toTexBlendFuncCode( BLENDFUNC[1] );
        name += toTexTypeCode( TEXTYPE[2] ) + TEXCOORD[2] + toTexBlendFuncCode( BLENDFUNC[2] );

        name += "A" + (ENABLE_ALPHA_THRESHHOLD ? 1 : 0);

        return name;
    }

    /// キーとして利用する　LightType に符号化する
    string toLightTypeCode( LightingType flg ){ 
        if( flg == LightingType.kLIGHTING_NONE ){
            return "N";
        }else if( flg == LightingType.kLIGHTING_VERTEX ){
            return "V";
        }else if( flg == LightingType.kLIGHTING_FLAGMENT ){
            return "F";
        }
        return "N";
    }

    /// キーとして利用する NormalMapType に符号化する
    string toNormalMapTypeCode( NormalMapType flg )
    {

        if( flg == NormalMapType.kNORMALMAP_NONE ){
            return "N";
        }else if( flg == NormalMapType.kNORMALMAP_VERTEX ){
            return "V";
        }else if( flg == NormalMapType.kNORMALMAP_IMAGE ){
            return "I";
        }
        return "N";
    }   

    /// キーとして利用する TexType に符号化する
    string toTexTypeCode( TexType flg )
    {

        if( flg == TexType.kTEXTYPE_IMAGE ){
            return "I";
        }else if( flg == TexType.kTEXTYPE_VERTEXNORMAL ){
            return "V";
        }else if( flg == TexType.kTEXTYPE_IMAGENORMALL ){
            return "M";
        }
        return "N";
    }

    
    /// キーとして利用する TexBlendFunc に符号化する
    string toTexBlendFuncCode( TexBlendFunc flg )
    {
        if( flg == TexBlendFunc.kBLEND_REPLACE ){
            return "R";
        }else if( flg == TexBlendFunc.kBLEND_ADD ){
            return "A";
        }else if( flg == TexBlendFunc.kBLEND_BLEND ){
            return "B";
        }else if( flg == TexBlendFunc.kBLEND_DECAL ){
            return "D";
        }else if( flg == TexBlendFunc.kBLEND_MODULATE ){
            return "M";
        }
        return "N";
    }

}

} // end ns DemoModel
//===
// EOF
//===
