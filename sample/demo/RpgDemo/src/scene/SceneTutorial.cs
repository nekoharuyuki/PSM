/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;


namespace AppRpg {


///***************************************************************************
/// シーン：チュートリアル画面
///***************************************************************************
public class SceneTutorial : DemoGame.IScene
{
    /// 画面上に表示する構成要素
    private struct cPageData
    {
		public DemoGame.Sprite            sprBg;
		public DemoGame.Sprite[]          sprObj;
		public Texture2D[]                imgObj;
    };

    private DemoGame.SceneManager       useSceneMgr;
    private int                         taskId;
    private StateId                     stateId;

    private EveStateId                  eventState;
    private Font                   	    useFont;
	private int                         pageNow;
	private const int                   pageMax = 10;
	private const int                   pageImgMax = 3;		/// 1ページに表示される最大イメージ画数

    private Texture2D                   imageBg;
    private cPageData                   pageTbl;

	private int                         startPosIdx;
	private int                         touchStartX;
	private int                         touchMoveX;
	private bool                        touchMoveFlg;

	private TutorialData[]				trlDataTbl;



    ///---------------------------------------------------------------------------
    /// 表示座標テーブル
    ///---------------------------------------------------------------------------
    private int[] pageImgPosTbl = {
		317,72,		// 1/1
		197,72,		// 1/2
		437,72,		// 2/2
		77,72,		// 1/3
		317,72,		// 2/3
		557,72		// 3/3
	};
    private int[] pageFontPosTbl = {
		427,276,		// 1/1
		307,276,		// 1/2
		547,276,		// 2/2
		187,276,		// 1/3
		427,276,		// 2/3
		667,276			// 3/3
	};
    private int[] pageBgPosTbl = {
		49,44
	};


    ///---------------------------------------------------------------------------
    /// 入力イベントID
    ///---------------------------------------------------------------------------
    public enum EveStateId{
        PageNext    = (1 << 0),        /// ページ進む
        PageBack    = (1 << 1),        /// ページ戻る
        TitleBack   = (1 << 2),        /// タイトルに戻る
    };

    ///---------------------------------------------------------------------------
    /// 状態ID
    ///---------------------------------------------------------------------------
    public enum StateId{
        Start,
        Normal,
        PageMove,
        End,
    };



/// public メソッド
///---------------------------------------------------------------------------

    /// シーンの初期化
    public bool Init( DemoGame.SceneManager sceneMgr )
    {
        useSceneMgr = sceneMgr;

        AppLyout.GetInstance().ClearSpriteAll();

	    useFont = new Font( FontAlias.System, 20, FontStyle.Regular );
		DemoGame.Graphics2D.SetFont( useFont );

        imageBg        = new Texture2D( "/Application/res/data/2D/2d_02.png", false);

		pageTbl        = new cPageData();
		pageTbl.sprBg  = new DemoGame.Sprite( imageBg, 0, 0, imageBg.Width, imageBg.Height,
														pageBgPosTbl[0]+AppLyout.GetInstance().OffsetW,
														pageBgPosTbl[1]+AppLyout.GetInstance().OffsetH );
		pageTbl.sprObj = new DemoGame.Sprite[pageImgMax];
		pageTbl.imgObj = new Texture2D[pageImgMax];

        taskId         = 0;
		pageNow        = 0;
		touchMoveX     = 0;
		touchMoveFlg   = false;
 
		trlDataTbl		= new TutorialData[pageMax];
		trlDataTbl[0]	= new TutorialDataP01();
		trlDataTbl[1]	= new TutorialDataP02();
		trlDataTbl[2]	= new TutorialDataP03();
		trlDataTbl[3]	= new TutorialDataP04();
		trlDataTbl[4]	= new TutorialDataP05();
		trlDataTbl[5]	= new TutorialDataP06();
		trlDataTbl[6]	= new TutorialDataP07();
		trlDataTbl[7]	= new TutorialDataP08();
		trlDataTbl[8]	= new TutorialDataP09();
		trlDataTbl[9]	= new TutorialDataP10();


		loadPageImage( pageNow );

		setStateTask( StateId.Start );

        return true;
    }

    /// シーンの破棄
    public void Term()
    {
		DemoGame.Graphics2D.SetDefaultFont();

        AppLyout.GetInstance().ClearSpriteAll();
        useSceneMgr = null;
    }

    /// シーンの継続切り替え時の再開処理
    public bool Restart()
    {
        return true;
    }

    /// シーンの継続切り替え時の停止処理
    public bool Pause()
    {
        return true;
    }

    /// サスペンド＆レジューム処理
    public void Suspend()
    {
    }
    public void Resume()
    {
    }

    /// フレーム処理
    public bool Update()
    {
		switch( stateId ){
		case StateId.Start:     updateStart();    break;
		case StateId.Normal:    updateNormal();   break;
		case StateId.PageMove:  updatePageMove(); break;
		case StateId.End:       updateEnd();      break;
		}

        GameCtrlManager.GetInstance().FrameTitle();
		return true;
	}


    /// 描画処理
    public bool Render()
    {
		GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        useGraphDev.Graphics.SetClearColor( 0.5f, 0.5f, 0.5f, 0.0f ) ;
        useGraphDev.Graphics.Clear() ;

        GameCtrlManager.GetInstance().DrawTitle();
		DemoGame.Graphics2D.FillRect( 0x80000040, 0, 0, useGraphDev.DisplayWidth, useGraphDev.DisplayHeight );


		/// BGの描画
		if( pageTbl.sprBg != null ){
			DemoGame.Graphics2D.DrawSprite( pageTbl.sprBg, touchMoveX, 0 );
		}

		/// イメージ画の描画
		for( int i=0; i<trlDataTbl[pageNow].GetInfoNum(); i++ ){
			if( pageTbl.sprObj[i] != null ){
				DemoGame.Graphics2D.DrawSprite( pageTbl.sprObj[i], touchMoveX, 0 );
			}
		}

		/// フォントの表示
		for( int i=0; i<trlDataTbl[pageNow].GetInfoNum(); i++ ){
			DemoGame.Graphics2D.FillRect( 0x20000040, pageFontPosTbl[(startPosIdx+i)*2+0]+AppLyout.GetInstance().OffsetW-110+touchMoveX,
													  pageFontPosTbl[(startPosIdx+i)*2+1]+AppLyout.GetInstance().OffsetH-62,
													  220, 124 );

			for( int j=0; j<trlDataTbl[pageNow].GetMessLen(i); j++ ){
				int strW = DemoGame.Graphics2D.CurrentFont.GetTextWidth( trlDataTbl[pageNow].GetMess(i, j) ); 
				int strH = DemoGame.Graphics2D.CurrentFont.Size;
				int posX = pageFontPosTbl[(startPosIdx+i)*2+0]+AppLyout.GetInstance().OffsetW-strW/2;
				int posY = pageFontPosTbl[(startPosIdx+i)*2+1]+AppLyout.GetInstance().OffsetH-(trlDataTbl[pageNow].GetMessLen(i)*strH/2)+j*strH;

		        DemoGame.Graphics2D.AddSprite( "Mess"+i*10+j, trlDataTbl[pageNow].GetMess(i, j), 0xffffffff,
												touchMoveX + posX, posY );
			}
		}


		/// スプライトの描画

        AppLyout.GetInstance().Render();

		for( int i=0; i<trlDataTbl[pageNow].GetInfoNum(); i++ ){
			for( int j=0; j<trlDataTbl[pageNow].GetMessLen(i); j++ ){
		        DemoGame.Graphics2D.RemoveSprite( "Mess"+i*10+j );
			}
		}
        AppDispEff.GetInstance().Draw( useGraphDev );

        useGraphDev.Graphics.SwapBuffers();

        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

	/// 状態タスクのセット
    private void setStateTask( StateId stateId )
    {
		this.stateId = stateId;
		taskId      = 0;
	}


    /// フレーム処理：開始
    private bool updateStart()
    {
		setStateTask( StateId.Normal );
		return true;
	}


    /// フレーム処理：通常
    private bool updateNormal()
    {
		/// 初期セット
		if( taskId == 0 ){
			setPageArrow( pageNow );
			taskId ++;
		}

		checkInputButtons();

		if( (eventState & EveStateId.TitleBack) != 0 ){
			setStateTask( StateId.End );
            return true;
        }

		if( (eventState & EveStateId.PageNext) != 0 || (eventState & EveStateId.PageBack) != 0 ){
			setStateTask( StateId.PageMove );
            return true;
        }

		return true;
	}


    /// フレーム処理：ページ移動
    private bool updatePageMove()
    {
		GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

		switch( taskId ){

		/// 掃ける
		///--------------------------------------
		case 0:
			setPageArrow( -1 );

			if( (eventState & EveStateId.PageNext) != 0 ){
				touchMoveX -= (useGraphDev.DisplayWidth/6);
			}
			else{
				touchMoveX += (useGraphDev.DisplayWidth/6);
			}
			if( abs( touchMoveX ) > useGraphDev.DisplayWidth ){
				taskId ++;
			}
			break;


		/// 読み替え
		///--------------------------------------
		case 1:
			if( (eventState & EveStateId.PageNext) != 0 ){
				touchMoveX = useGraphDev.DisplayWidth;	
				pageNow ++;
				if( pageNow >= pageMax ){
					pageNow = pageMax-1;
				}
			}
			else{
				touchMoveX = -useGraphDev.DisplayWidth;	
				pageNow --;
				if( pageNow < 0 ){
					pageNow = 0;
				}
			}

			loadPageImage( pageNow );
			touchMoveFlg = false;

			taskId ++;
			break;


		/// フレームイン
		///--------------------------------------
		case 2:
			if( touchMoveX < 0 ){
				touchMoveX += (useGraphDev.DisplayWidth/6);
				if( touchMoveX > 0 ){
					touchMoveX = 0;
					setStateTask( StateId.Normal );
				}
			}
			else{
				touchMoveX -= (useGraphDev.DisplayWidth/6);
				if( touchMoveX < 0 ){
					touchMoveX = 0;
					setStateTask( StateId.Normal );
				}
			}
			break;
		}

		return true;
	}


    /// フレーム処理：終了
    private bool updateEnd()
    {
		if( taskId == 0 ){
			setPageArrow( -1 );
   		    AppLyout.GetInstance().ClearSprite( AppLyout.SpriteId.Back_on );
   		    AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Back_off );

            taskId ++;
            return true;
		}

		if( AppDispEff.GetInstance().NowEffId != AppDispEff.EffId.FadeOut ){
			useSceneMgr.Prev();
		}
        return true;
    }



    /// 入力イベントのチェック
    private void checkInputButtons()
    {
		GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

		int devPosX = AppInput.GetInstance().DevicePosX;
		int devPosY = AppInput.GetInstance().DevicePosY;

		eventState = 0;

		/// 戻る
		///--------------------------------------------------------------
		if( AppInput.GetInstance().DeviceInputId >= 0 &&
			AppLyout.GetInstance().CheckRect( AppLyout.SpriteId.Back_on, devPosX, devPosY ) ){

   		    AppLyout.GetInstance().ClearSprite( AppLyout.SpriteId.Back_off );
   		    AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Back_on );

            if( AppInput.GetInstance().CheckDeviceSingleTouchUp() == true ){
				eventState = EveStateId.TitleBack;
   	        }
		}
		else {
   		    AppLyout.GetInstance().ClearSprite( AppLyout.SpriteId.Back_on );
   		    AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Back_off );
		}


		/// ページ送り
		///--------------------------------------------------------------
		if( eventState == 0 && abs( touchMoveX ) < useGraphDev.DisplayWidth/20 ){
			if( AppInput.GetInstance().DeviceInputId >= 0 && devPosX < ((useGraphDev.DisplayWidth / 2) - 374) ){
				if( pageNow > 0 ){
		            if( AppInput.GetInstance().CheckDeviceSingleTouchUp() == true ){
						eventState = EveStateId.PageBack;
		   	        }
				}
			}
			else if( AppInput.GetInstance().DeviceInputId >= 0 && devPosX >= ((useGraphDev.DisplayWidth / 2) + 374) ){
				if( pageNow+1 < pageMax ){
		            if( AppInput.GetInstance().CheckDeviceSingleTouchUp() == true ){
						eventState = EveStateId.PageNext;
		   	        }
				}
			}
		}


		/// ページ切り替え入力判定
		///--------------------------------------------------------------
		if( AppInput.GetInstance().CheckDeviceSingleTouchDown() ){
			touchStartX  = devPosX;
			touchMoveX   = devPosX - touchStartX;
			touchMoveFlg = true;
		}

		if( touchMoveFlg == true && AppInput.GetInstance().CheckDeviceSingleTouching() ){
			touchMoveX = devPosX - touchStartX;
			if( (pageNow+1 >= pageMax && touchMoveX < 0) || (pageNow <= 0 && touchMoveX > 0) ){
				touchMoveX = 0;
			}
		}
		else if( touchMoveX > 0 ){
			touchMoveX -= (useGraphDev.DisplayWidth/10);
			if( touchMoveX < 0 ){
				touchMoveX = 0;
			}
		}
		else if( touchMoveX < 0 ){
			touchMoveX += (useGraphDev.DisplayWidth/10);
			if( touchMoveX > 0 ){
				touchMoveX = 0;
			}
		}

        if( AppInput.GetInstance().CheckDeviceSingleTouchUp() == true ){
			if( touchMoveX >= (useGraphDev.DisplayWidth/3) ){
				eventState = EveStateId.PageBack;
			}
			else if( touchMoveX < -(useGraphDev.DisplayWidth/3) ){
				eventState = EveStateId.PageNext;
			}
		}
    }


    /// 画面の表示する画像の選択
    private void loadPageImage( int page )
    {
		for( int i=0; i<pageImgMax; i++ ){
			if( pageTbl.imgObj[i] != null ){
		        pageTbl.imgObj[i].Dispose();
			}
			if( pageTbl.sprObj[i] != null ){
		        pageTbl.sprObj[i].Dispose();
			}
	        pageTbl.imgObj[i] = null;
	        pageTbl.sprObj[i] = null;
		}

		startPosIdx = 0;
		for( int i=0; i<trlDataTbl[page].GetInfoNum(); i++ ){
			startPosIdx += i;
		}

		for( int i=0; i<trlDataTbl[page].GetInfoNum(); i++ ){
			/// 表示限界をオーバー
			if( i >= pageImgMax ){
				break;
			}

	        pageTbl.imgObj[i] = new Texture2D( trlDataTbl[page].GetImgName(i), false);
	        pageTbl.sprObj[i] = new DemoGame.Sprite( pageTbl.imgObj[i],
													0, 0, pageTbl.imgObj[i].Width, pageTbl.imgObj[i].Height,
													pageImgPosTbl[(startPosIdx+i)*2+0]+AppLyout.GetInstance().OffsetW,
													pageImgPosTbl[(startPosIdx+i)*2+1]+AppLyout.GetInstance().OffsetH );
		}
	}

	/// ページ送りの表示セット
    private void setPageArrow( int page )
    {
		if( page < 0 ){
   		    AppLyout.GetInstance().ClearSprite( AppLyout.SpriteId.LeftA );
   		    AppLyout.GetInstance().ClearSprite( AppLyout.SpriteId.RightA );
			return ;
		}

		/// ページ矢印の表示設定
		if( page > 0 ){
  		    AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.LeftA );
		}
		else{
   		    AppLyout.GetInstance().ClearSprite( AppLyout.SpriteId.LeftA );
		}
		if( page+1 < pageMax ){
  		    AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.RightA );
		}
		else{
   		    AppLyout.GetInstance().ClearSprite( AppLyout.SpriteId.RightA );
		}
	}

    /// 絶対値
    private int abs( int val )
    {
		if( val < 0 ){
			return( val*-1 );
		}
		return val;
	}
}

} // namespace
