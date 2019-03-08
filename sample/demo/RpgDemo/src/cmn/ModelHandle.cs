/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;
using DemoModel;

namespace AppRpg { namespace Common {


///***************************************************************************
/// モデル操作クラス
///***************************************************************************
public class ModelHandle
{
    private DemoModel.BasicModel            useModel;
    private DemoModel.TexContainer          useTexCnr;
    private DemoModel.ShaderContainer       useShaderCnr;

    /// アニメーション関連
    private    int         animPlayId;
    private    bool        animPlayLoopFlg;
    private    float       animPlayTime;
    private    int         animPlayFrameCnt;
    private    int         animPlayFrameMax;
    private    bool        animPlayStartFlg;
    private    bool        animPlayEndFlg;
    private    Vector4     eyeVec;


    /// コンストラクタ
    public ModelHandle()
    {
        useModel        = null;
        useTexCnr       = null;
        useShaderCnr    = null;
    }

/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public void Init()
    {
        Term();
        animPlayId            = -1;
        eyeVec = new Vector4( 0.0f, 0.0f, 0.0f, 1.0f);
    }

    /// 破棄
    public void Term()
    {
        useModel        = null;
        useTexCnr       = null;
        useShaderCnr    = null;
    }

    /// 使用するデータのセット
    public void Start( DemoModel.BasicModel useModel, DemoModel.TexContainer useTexCnr, DemoModel.ShaderContainer useShaderCnr )
    {
        this.useModel        = useModel;
        this.useTexCnr       = useTexCnr;
        this.useShaderCnr    = useShaderCnr;
 
        this.useModel.BindTextures( this.useTexCnr );
   }


    /// 使用するデータのセット
    /// 速度を早くするためにテクスチャのバインドを分離
    public void StartNoBindTex( DemoModel.BasicModel useModel, DemoModel.TexContainer useTexCnr, DemoModel.ShaderContainer useShaderCnr )
    {
        this.useModel        = useModel;
        this.useTexCnr       = useTexCnr;
        this.useShaderCnr    = useShaderCnr;
   }

    /// モデルにテクスチャのバインドを行う
    public void SetBindTextures()
    {
        this.useModel.BindTextures( this.useTexCnr );
    }


    /// 終了
    public void End()
    {
        useModel        = null;
        useTexCnr       = null;
        useShaderCnr    = null;
    }


    /// アニメーションのセット
    public void SetPlayAnim( int playId, bool loopFlg )
    {
        animPlayId          = playId;
        animPlayLoopFlg     = loopFlg;
        animPlayTime        = 0.0f;
        animPlayFrameCnt    = 0;
        animPlayFrameMax    = (int)GetAnimLength( playId );
        animPlayStartFlg    = true;
        animPlayEndFlg      = false;
    }

    /// アニメーションの更新
    public void UpdateAnim()
    {
        if( animPlayId < 0 ){
            return ;
        }
                    
        if( animPlayStartFlg == true ){
            animPlayStartFlg = false;
            return ;
        }

        animPlayFrameCnt ++;
        if( animPlayFrameCnt < animPlayFrameMax ){
            animPlayTime = animPlayFrameCnt;
        }
        else{
            if( animPlayLoopFlg == false ){
                animPlayTime      = GetAnimLength( animPlayId );
                animPlayEndFlg    = true;
            }
            else{
                animPlayFrameCnt  = 0;
                animPlayTime      = 0.0f;
            }
        }
    }

    /// アニメーションをモデルのボーンに適応
    public void UpdateBindAnim()
    {
        if( animPlayId >= 0 ){
            useModel.SetAnimFrame( animPlayId, animPlayTime );
        }
    }



    /// 通常描画（描画時にモデルにアニメーションをセット）
    public bool Render( DemoGame.GraphicsDevice graphDev, Matrix4 matrix )
    {
        useModel.WorldMatrix = matrix;

        if( animPlayId >= 0 ){
            useModel.SetAnimFrame( animPlayId, animPlayTime );
        }

        useModel.Update();
        useModel.Draw( graphDev.Graphics, useShaderCnr, graphDev.GetCurrentCamera().ViewProjection, eyeVec );
        return true;
    }


    /// 描画（事前にモデルにアニメーションをセット）
    public bool RenderNoAnim( DemoGame.GraphicsDevice graphDev, Matrix4 matrix )
    {
        useModel.WorldMatrix = matrix;
        useModel.Update();
        useModel.Draw( graphDev.Graphics, useShaderCnr, graphDev.GetCurrentCamera().ViewProjection, eyeVec );
        return true;
    }


    /// 再生中のアニメーションの長さを取得
    public float GetAnimLength()
    {
        return animPlayFrameMax;
    }

    /// アニメーションの長さを取得
    public float GetAnimLength( int animIdx )
    {
        return useModel.GetMotionFrameMax( animIdx );
    }

    /// ボーンの姿勢を取得
    public Matrix4 GetBoneMatrix( int boneId )
    {
        return useModel.Bones[boneId].WorldMatrix;
    }

    /// ボーンのIDを取得
    public int GetBoneId( string name )
    {
		for( int i=0; i<useModel.Bones.Length; i++ ){
			if( useModel.Bones[i].Name == name ){
				return i;
			}
		}
        return -1;
    }

    /// アニメーション再生中かのチェック
    public bool IsAnimation()
    {
        return !animPlayEndFlg;
    }


}

}} // namespace
