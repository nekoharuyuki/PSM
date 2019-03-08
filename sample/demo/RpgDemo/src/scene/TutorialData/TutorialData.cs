/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//チュートリアルデータ基底
///***************************************************************************
public class TutorialData
{
	static protected string	filePath = "/Application/res/data/2D/";

    protected string[] pageImgTbl;
    protected string[] pageMessStrTbl;
	protected int[]    pageMessNumTbl;


	/// コンストラクタ
	public TutorialData()
    {
		Init();
    }
	/// デストラクタ
	~TutorialData()
    {
		Term();
    }
		
	/// 作成
    public virtual bool Init()
    {
		pageImgTbl     = null;
		pageMessStrTbl = null;
		pageMessNumTbl = null;
		return true;
	}

    /// 破棄
    public virtual void Term()
    {
		pageImgTbl     = null;
		pageMessStrTbl = null;
		pageMessNumTbl = null;
	}


    /// 情報数取得
    public virtual int GetInfoNum()
    {
		if( pageMessStrTbl != null ){
	        return pageImgTbl.Length;
		}
        return 0;
    }

    /// イメージ画のファイル名取得
    public virtual string GetImgName( int idx )
    {
		if( pageImgTbl != null ){
	        return( filePath+pageImgTbl[idx] );
		}
        return "";
    }

    /// メッセージ取得
    public virtual string GetMess( int idx, int len )
    {
		if( pageMessStrTbl != null ){
			int startPosIdx = 0;
			for( int i=0; i<idx; i++ ){
				startPosIdx += GetMessLen(i);
			}
	        return pageMessStrTbl[startPosIdx+len];
		}
        return "";
    }

    /// メッセージ数取得
    public virtual int GetMessLen( int idx )
    {
		if( pageMessNumTbl != null ){
	        return pageMessNumTbl[idx];
		}
        return 0;
    }

}

} // namespace
