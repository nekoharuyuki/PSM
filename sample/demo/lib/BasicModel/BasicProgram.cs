// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace DemoModel{

public class BasicProgram : ShaderProgram
{
    // uniform
    private int idViewProj;
    private int idWorld00;
    private int idWorld01;
    private int idWorld02;
    private int idWorld03;
    private int idWorldInverse;
    private int idLocalEyePosition;
    private int idLocalLightPosition00;
    private int idLocalLightPosition01;
    private int idLocalLightPosition02;
    private int idTexMtx00;
    private int idTexMtx01;
    private int idTexMtx02;
    private int idIKAmbient;
    private int idIDiffuse;
    private int idISpecular;
    private int idShininess;
    private int idKDiffuse00;
    private int idKSpecular00;
    private int idKDiffuse01;
    private int idKSpecular01;
    private int idKDiffuse02;
    private int idKSpecular02;
    private int idTangentID;
    private int idAlphaThreshold;

    // 特殊 uniform
    private int idView;
    private int idMinDepth;
    private int idMaxDepth;
    private int idLocalLightDir;

    // test private
    // [note] Shader分割後は必要なくなるが、分割前のシェーダのために必要になった変数
    /*
     * private int idENABLE_TEXANIM;
     * private int idENABLE_ALPHA_THRESHHOLD;
     * private int idENABLE_VERTEX_COLOR;
     */
    private int idMTXPLT_COUNT;
    private int idTEX_COUNT;
    private int idLIGHT_COUNT;
    private int idLIGHTING_TYPE;
    private int idNORMALMAP_TYPE;
    private int idTEXCOORD00;
    private int idTEXTYPE00;
    private int idBLENDFUNC00;
    private int idTEXCOORD01;
    private int idTEXTYPE01;
    private int idBLENDFUNC01;
    private int idTEXCOORD02;
    private int idTEXTYPE02;
    private int idBLENDFUNC02;

    // 旧フォーマット
    private int idBlendCount;

    private struct AttrData{
        public int ID;
        public int Bind;
        public string Name;
    }

    private AttrData[] attr = new AttrData[ 7 ];

    /// コンストラクタ
	public BasicProgram( string vshFileName, string fshFileName )
    : base( vshFileName, fshFileName )
    {
        string[] names = {
            "a_Position",
            "a_Normal",
            "a_Color",
            "a_TexCoord0",
            "a_TexCoord1",
            "a_TexCoord2",
            "a_Weight"
        };

        for( int i = 0; i < names.Length; i++ ){
            attr[ i ].ID = this.FindAttribute( names[ i ] );
            attr[ i ].Bind = i;
            attr[ i ].Name = names[ i ];
            if( attr[ i ].ID >= 0 ) this.SetAttributeBinding( i, attr[ i ].Name );
        }
        //        this.SetAttributeBinding( 0, "a_Position" );
        //        this.SetAttributeBinding( 1, "a_Normal" );
        //        this.SetAttributeBinding( 2, "a_Color" );
        //        this.SetAttributeBinding( 3, "a_TexCoord0" );
        //        this.SetAttributeBinding( 4, "a_TexCoord1" );
        //        this.SetAttributeBinding( 5, "a_TexCoord2" );
        //        this.SetAttributeBinding( 6, "a_Weight" );

        setUniformID();
        setSpecificUniformID();
        setOldUniformID();
        setTestUniformID();
    }

    public void setSpecificUniformID()
    {
        idView = this.FindUniform( "View" );
        idMinDepth = this.FindUniform( "MinDepth" );
        idMaxDepth = this.FindUniform( "MaxDepth" );
        idLocalLightDir = this.FindUniform( "LocalLightDir" );

    }

    /// BasicProgram としての規定 Uniform
    public void setUniformID()
    {
        idViewProj = this.FindUniform( "ViewProj" );
        idWorld00 = this.FindUniform( "World00" );
        idWorld01 = this.FindUniform( "World01" );
        idWorld02 = this.FindUniform( "World02" );
        idWorld03 = this.FindUniform( "World03" );
        idWorldInverse = this.FindUniform( "WorldInverse" );
        idLocalEyePosition = this.FindUniform( "LocalEyePosition" );
        idLocalLightPosition00 = this.FindUniform( "LocalLightPosition00" );
        idLocalLightPosition01 = this.FindUniform( "LocalLightPosition01" );
        idLocalLightPosition02 = this.FindUniform( "LocalLightPosition02" );
        idTexMtx00 = this.FindUniform( "TexMtx00" );
        idTexMtx01 = this.FindUniform( "TexMtx01" );
        idTexMtx02 = this.FindUniform( "TexMtx02" );
        idIKAmbient = this.FindUniform( "IKAmbient" );
        idIDiffuse = this.FindUniform( "IDiffuse" );
        idISpecular = this.FindUniform( "ISpecular" );
        idShininess = this.FindUniform( "Shininess" );
        idKDiffuse00 = this.FindUniform( "KDiffuse00" );
        idKSpecular00 = this.FindUniform( "KSpecular00" );
        idKDiffuse01 = this.FindUniform( "KDiffuse01" );
        idKSpecular01 = this.FindUniform( "KSpecular01" );
        idKDiffuse02 = this.FindUniform( "KDiffuse02" );
        idKSpecular02 = this.FindUniform( "KSpecular02" );
        idTangentID = this.FindUniform( "TangentID" );
        idAlphaThreshold = this.FindUniform( "AlphaThreshold" );
    }
    public void setOldUniformID()
    {
        idBlendCount = this.FindUniform( "BlendCount" );
    }


    /// テスト用の uniform
    /**
     * 定数として分割するもの
     */
    public void setTestUniformID()
    {
        // [note] Shader分割後は必要なくなるが、分割前のシェーダのために必要になった変数
        /*
         * idENABLE_TEXANIM = this.FindUniform( "ENABLE_TEXANIM" );
         * idENABLE_ALPHA_THRESHHOLD = this.FindUniform( "ENABLE_ALPHA_THRESHHOLD" );
         * idENABLE_VERTEX_COLOR = this.FindUniform( "ENABLE_VERTEX_COLOR" );
         */
        idMTXPLT_COUNT = this.FindUniform( "MTXPLT_COUNT" );
        idTEX_COUNT = this.FindUniform( "TEX_COUNT" );
        idLIGHT_COUNT = this.FindUniform( "LIGHT_COUNT" );
        idLIGHTING_TYPE = this.FindUniform( "LIGHTING_TYPE" );
        idNORMALMAP_TYPE = this.FindUniform( "NORMALMAP_TYPE" );
        idTEXCOORD00 = this.FindUniform( "TEXCOORD00" );
        idTEXTYPE00 = this.FindUniform( "TEXTYPE00" );
        idBLENDFUNC00 = this.FindUniform( "BLENDFUNC00" );
        idTEXCOORD01 = this.FindUniform( "TEXCOORD01" );
        idTEXTYPE01 = this.FindUniform( "TEXTYPE01" );
        idBLENDFUNC01 = this.FindUniform( "BLENDFUNC01" );
        idTEXCOORD02 = this.FindUniform( "TEXCOORD02" );
        idTEXTYPE02 = this.FindUniform( "TEXTYPE02" );
        idBLENDFUNC02 = this.FindUniform( "BLENDFUNC02" );
    }

    /// [特殊]View 行列の設定
    public void SetView( Matrix4 view )
    {
        if( idView >= 0 ){
            this.SetUniformValue( idView, ref view );
        }
    }

    /// [特殊]DepthRange の設定
    public void SetDepthRenge( float min, float max )
    {
        if( idMinDepth >= 0 ){
            this.SetUniformValue( idMinDepth, min );
        }
        if( idMaxDepth >= 0 ){
            this.SetUniformValue( idMaxDepth, max );
        }
    }

    public void SetViewProj( Matrix4 viewProj )
    {
        if( idViewProj >= 0 ){
            this.SetUniformValue( idViewProj, ref viewProj );
        }

    }

    public void SetWorldCount( int worldCount )
    {
        if( idMTXPLT_COUNT >= 0 ){
            this.SetUniformValue( idMTXPLT_COUNT, worldCount );
        }
    }

    public void SetMatrixPalette( int idx, ref Matrix4 mtx )
    {
        if( idx == 0 ) this.SetUniformValue( idWorld00, ref mtx );
        else if( idx == 1 ) this.SetUniformValue( idWorld01, ref mtx );
        else if( idx == 2 ) this.SetUniformValue( idWorld02, ref mtx );
        else if( idx == 3 ) this.SetUniformValue( idWorld03, ref mtx );
    }
        

    public void SetMatrixPalette( int blendCount, ref Matrix4[] matrixPalette )
    {
        if( idMTXPLT_COUNT >= 0 ){
            this.SetUniformValue( idMTXPLT_COUNT, blendCount );
        }
        if( idBlendCount >= 0 ){
            idBlendCount = this.FindUniform( "BlendCount" );
        }


        this.SetUniformValue( idWorld00, ref matrixPalette[0] );
        if( blendCount >= 2 ) this.SetUniformValue( idWorld01, ref matrixPalette[1] );
        if( blendCount >= 3 ) this.SetUniformValue( idWorld02, ref matrixPalette[2] );
        if( blendCount >= 4 ) this.SetUniformValue( idWorld03, ref matrixPalette[3] );
    }

    public void SetMaterial( ref BasicMaterial material )
    {
        if( idIKAmbient >= 0 ){
            this.SetUniformValue( idIKAmbient, ref material.Ambient );
        }
        if( idIDiffuse >= 0 ){
            this.SetUniformValue( idIDiffuse, ref material.Diffuse );
        }
        if( idISpecular >= 0 ){
            this.SetUniformValue( idISpecular, ref material.Specular );
        }
        if( idShininess >= 0 ){
            this.SetUniformValue( idShininess, 80.8f );
        }

        if( idAlphaThreshold >= 0 ){
            this.SetUniformValue( idAlphaThreshold, material.AlphaThreshold );
        }

    }

    public void SetTexMtx( Matrix4[] texAnims )
    {
        if( idTexMtx00 >= 0 ){
            this.SetUniformValue( idTexMtx00, ref texAnims[ 0 ] );
            }
        if( idTexMtx01 >= 0 ){
            this.SetUniformValue( idTexMtx01, ref texAnims[ 1 ] );
        }
        if( idTexMtx02 >= 0 ){
            this.SetUniformValue( idTexMtx02, ref texAnims[ 2 ] );
        }
    }

    public void SetTangentID( int tangentID )
    {
        if( idTangentID >= 0 ){
            this.SetUniformValue( idTangentID, tangentID );
        }
    }

    /// [setting] テクスチャの枚数設定
    public void SetTexCount( int texCount )
    {
        if( idTEX_COUNT >= 0){
            this.SetUniformValue( idTEX_COUNT, texCount );
        }
    }

    /// [setting] テクスチャのブレンドモード設定
    public void SetTexBlendFunc( int idx, TexBlendFunc TexBlendMode )
    {
        if( idx == 0 && idBLENDFUNC00 >= 0 ){
            this.SetUniformValue( idBLENDFUNC00, (int)TexBlendMode );
        }else if( idx == 1 && idBLENDFUNC01 >= 0 ){
            this.SetUniformValue( idBLENDFUNC01, (int)TexBlendMode );
        }else if( idx == 2 && idBLENDFUNC02 >= 0 ){
            this.SetUniformValue( idBLENDFUNC02, (int)TexBlendMode );
        }
    }

    /// [setting] layer に対して利用するuvのインデックス設定
    public void SetTexCoord( int idx, int TexCoordIdx )
    {
        if( idx == 0 && idTEXCOORD00 >= 0 ){
            this.SetUniformValue( idTEXCOORD00, TexCoordIdx );
        }else if( idx == 1 && idTEXCOORD01 >= 0 ){
            this.SetUniformValue( idTEXCOORD01, TexCoordIdx );
        }else if( idx == 2 && idTEXCOORD02 >= 0 ){
            this.SetUniformValue( idTEXCOORD02, TexCoordIdx );
        }
    }

    /// [setting] ライトの種別設定
    public void SetLightType( int type, int normalMapType )
    {
        if( idLIGHTING_TYPE >= 0 ){
            this.SetUniformValue( idLIGHTING_TYPE, 1 );  // kLIGHTING_FLAGMENT
        }
        
        if( idNORMALMAP_TYPE >= 0 ){
            this.SetUniformValue( idNORMALMAP_TYPE, 1 ); // kNORMALMAP_VERTEX
        }
    }

    /// [setting] ライトの数設定
    public void SetLightCount( int count )
    {
        if( count > 0 ){
        }
        if( idLIGHT_COUNT >= 0 ){
            this.SetUniformValue( idLIGHT_COUNT, count );
        }
    }

    /// ライトの設定
    public void SetLights( 
                          ref Matrix4 target, ///< モデルローカルに変換するための、モデルの位置
                          ref Vector4 eyePos, ///< ワールド上での視点の位置
                          ref Light[] lights
                           )
    {
        if( lights != null ){
            Matrix4 invWorld = target.Inverse();
            Vector4 localEyePosistion = invWorld * eyePos;
            
            if( idWorldInverse >= 0 ){
                this.SetUniformValue( idWorldInverse, ref invWorld );
            }

            if( idLocalEyePosition >= 0 ){
                this.SetUniformValue( idLocalEyePosition, ref localEyePosistion );
            }

            
            // ライト : 0
            if( lights[ 0 ] != null ){
                Vector4 localLightPosition = invWorld * lights[ 0 ].Position;
                if( idLocalLightPosition00 >= 0 ){
					// Vector4 V = localLightPosition.Normalize();
                    this.SetUniformValue( idLocalLightPosition00, ref localLightPosition );
                }

                // 並行光源(トゥーン用)
                if( idLocalLightDir >= 0 ){
                    Vector4 localLightPos = invWorld * lights[ 0 ].Position;
                    Vector3 localLightDir = new Vector3( -localLightPos.X, -localLightPos.Y, -localLightPos.Z );
                    localLightDir = localLightDir.Normalize();
                    
                    this.SetUniformValue( idLocalLightDir, ref localLightDir );
                }

                if( idKDiffuse00 >= 0 ){
                    this.SetUniformValue( idKDiffuse00, ref lights[ 0 ].KDiffuse );
                }
                if( idKSpecular00 >= 0 ){
                    this.SetUniformValue( idKSpecular00, ref lights[ 0 ].KSpecular );
                }
            }
            // ライト : 1
            if( lights[ 1 ] != null ){
                if( idLocalLightPosition01 >= 0 ){
                    Vector4 localLightPosition = invWorld * lights[ 1 ].Position;
                    this.SetUniformValue( idLocalLightPosition01, ref localLightPosition );
                }
                if( idKDiffuse01 >= 0 ){
                    this.SetUniformValue( idKDiffuse01, ref lights[ 1 ].KDiffuse );
                }
                if( idKSpecular01 >= 0 ){
                    this.SetUniformValue( idKSpecular01, ref lights[ 1 ].KSpecular );
                }
            }

            // ライト : 2
            if( lights[ 2 ] != null ){
                if( idLocalLightPosition02 >= 0 ){
                    Vector4 localLightPosition = invWorld * lights[ 2 ].Position;
                    this.SetUniformValue( idLocalLightPosition02, ref localLightPosition );
                }
                if( idKDiffuse02 >= 0 ){
                    this.SetUniformValue( idKDiffuse02, ref lights[ 2 ].KDiffuse );
                }
                if( idKSpecular02 >= 0 ){
                    this.SetUniformValue( idKSpecular02, ref lights[ 2 ].KSpecular );
                }
            }

        }else{
            if( idLIGHT_COUNT >= 0 ){
                this.SetUniformValue( idLIGHT_COUNT, 0 );
            }
            
        }
    }

    /**
     * TexType
     * 0 ... 通常テクスチャ
     * 1 ... 頂点座標系変換済みノーマルマップ
     * 2 ... テクスチャ座標系ノーマルマップ
     */
    public void SetTexType( int idx, TexType TexType )
    {
        if( idx == 0 && idTEXTYPE00 >= 0 ){
            this.SetUniformValue( idTEXTYPE00, (int)TexType );
        }else if( idx == 1 && idTEXTYPE01 >= 0 ){
            this.SetUniformValue( idTEXTYPE01, (int)TexType );
        }else if( idx == 2 && idTEXTYPE01 >= 0 ){
            this.SetUniformValue( idTEXTYPE02, (int)TexType );
        }
    }

    /// 事後計算用
    public void Update()
    {
        ; // not imp.
    }

    public void SetViewPos( Matrix4 target, Vector4 pos )
    {
    }


}
} // end ns DemoModel
//===
// EOF
//===


