/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;

namespace RssApp
{

    class Program
    {
        static GraphicsContext graphics = new GraphicsContext();

        static private ImageAsset feedItemBackImage = new ImageAsset("/Application/assets/feedlist_back.png");
        static private ImageAsset feedFocusItemBackImage = new ImageAsset("/Application/assets/feedlist_back_selected.png");
        static private List<ImageAsset> articleItemBackImage = new List<ImageAsset>();
        static private int prevBackImageIndex = -1;
        static private float articleUnreadAlpha = 1.0f;
        static private float articleAlreadyreadAlpha = 0.5f;

        static private FeedManager feedManager = new FeedManager();
        static private FeedListPanelItem currentSelectedFeedItem = null;
        static private RssFeed currentRssFeed;
        static private int currentSelectedFeedIndex = -1;

        static private Widget currentArticlePanel = null;
        static private Widget outPanel = null;

        static private MainScene scene;
        static private ArticlePanel articlePanel = null;

        static private FadeOutEffect outEffect;
        static private BunjeeJumpEffect inEffect;

        static private float articleItemHeight;

        const float articleVGap = 5.0f;
        private const float crossFadeTime = 4000.0f;
        private const float flipBoradTime = 2500.0f;
        private const float bunjeeJumpElasticity = 0.5f;
        private const float liveListPanelItemTiltAngle = 0.05f;

        private const float zeroPosition = 0.0f;
        private const float articlePanelDefaultScale = 0.1f;
        private const float articlePanelNormalScale = 1.0f;
        private const float articlePanelInEffectTime = 500.0f;
        private const float articlePanelOutEffectTime = 1000.0f;

        static void Main(string[] args)
        {
            // Initialize
            UISystem.Initialize(graphics, getPixelDensity());

            // BlankScene
            BlankScene blankScene = new BlankScene();
            UISystem.SetScene(blankScene, null);

            // TitleScene
            TitleScene titleScene = new TitleScene();

            CrossFadeTransition crossFadeTransition
                = new CrossFadeTransition(crossFadeTime, CrossFadeTransitionInterpolator.Linear);
            crossFadeTransition.Interpolator = CrossFadeTransitionInterpolator.Custom;
            crossFadeTransition.CustomNextSceneInterpolator
                = new AnimationInterpolator(CrossFadeInterpolator);
            crossFadeTransition.TransitionStopped += new EventHandler<EventArgs>(stopSceneTransition);
            UISystem.SetScene(titleScene, crossFadeTransition);

            // Scene
            scene = new MainScene();

            scene.RssFeedPanel.BackgroundColor = new UIColor(0f / 255f, 64f / 255f, 208f / 255f, 255f / 255f);
            scene.RssFeedPanel.SetListItemCreator(RssFeedItemCreator);
            scene.RssFeedPanel.SetListItemUpdater(RssFeedItemUpdator);
            scene.RssFeedPanel.ShowSection = false;
            scene.RssFeedPanel.ScrollBarVisibility = ScrollBarVisibility.ScrollingVisible;

            ListSectionCollection section = new ListSectionCollection {
                        new ListSection("Purchased", feedManager.FeedCount)
            };
            scene.RssFeedPanel.Sections = section;

            ArticleListPanelItem articleListPanelItem = new ArticleListPanelItem();
            articleItemHeight = articleListPanelItem.Height;

            articleItemBackImage.Add(new ImageAsset("/Application/assets/itemBack01.png"));
            articleItemBackImage.Add(new ImageAsset("/Application/assets/itemBack02.png"));
            articleItemBackImage.Add(new ImageAsset("/Application/assets/itemBack03.png"));
            articleItemBackImage.Add(new ImageAsset("/Application/assets/itemBack04.png"));
            articleItemBackImage.Add(new ImageAsset("/Application/assets/itemBack05.png"));

            currentRssFeed = feedManager.GetRssFeed(0);

            // main loop
            for (; ; )
            {
                // check system events
                SystemEvents.CheckEvents();

                // update
                {
                    // update UI
                    List<TouchData> touchDataList = Touch.GetData(0);
                    UISystem.Update(touchDataList);
                }

                // draw
                {
                    // clear
                    graphics.SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height);
                    graphics.SetClearColor(0.0f, 0.0f, 0.0f, 1.0f);
                    graphics.Clear();

                    // draw UI
                    UISystem.Render();

                    graphics.SwapBuffers();
                }
            }
        }

        private static ListPanelItem RssFeedItemCreator()
        {
            FeedListPanelItem item = new FeedListPanelItem();
            item.TouchEventReceived += new EventHandler<TouchEventArgs>(RssListItemTouchEventReceived);
            return item;
        }

        private static void RssFeedItemUpdator(ListPanelItem item)
        {
            if (item is FeedListPanelItem)
            {
                FeedListPanelItem targetItem = (item as FeedListPanelItem);
                targetItem.BackImage = feedItemBackImage;
                RssFeed feed = feedManager.GetRssFeed(item.Index);
                if( feed != null )
                {
                    targetItem.Text = feed.RssTitle + "(" + feed.UnreadCount + ")";
                }
                if (item.Index == currentSelectedFeedIndex)
                {
                    targetItem.BackImage = feedFocusItemBackImage;
                }
            }
        }

        static void RssListItemTouchEventReceived(object sender, TouchEventArgs e)
        {
            FeedListPanelItem item = (sender as FeedListPanelItem);
            if (item != null)
            {
                switch (e.TouchEvents.PrimaryTouchEvent.Type)
                {
                    case TouchEventType.Up:
                        if( currentSelectedFeedItem != item )
                        {
                            if( currentSelectedFeedItem != null)
                            {
                                currentSelectedFeedItem.BackImage = feedItemBackImage;
                            }
                            item.BackImage = feedFocusItemBackImage;
                            currentSelectedFeedItem = item;
                            currentSelectedFeedIndex = currentSelectedFeedItem.Index;
                        }
                        if( currentArticlePanel != null )
                        {
                            if (inEffect != null)
                            {
                                inEffect.Stop();
                            }
                            if( outEffect != null )
                            {
                                outEffect.Stop();
                            }
                            outEffect = new FadeOutEffect();
                            outEffect.Widget = currentArticlePanel;
                            outEffect.EffectStopped += new EventHandler<EventArgs>(EffectStop);;
                            outEffect.Start();
                            outPanel = currentArticlePanel;
                        }

                        currentRssFeed = feedManager.GetRssFeed(item.Index);
                        scene.CurrentRssFeed = currentRssFeed;
                        scene.RemoveButtonEnabled = true;

                        LiveListPanel nextLiveListPanel = new LiveListPanel();
                        scene.BaseLiveListPanel.AddChildLast(nextLiveListPanel);
                        nextLiveListPanel.X = 0.0f;
                        nextLiveListPanel.Y = 0.0f;
                        nextLiveListPanel.Width = scene.BaseLiveListPanel.Width;
                        nextLiveListPanel.Height = scene.BaseLiveListPanel.Height;
                        nextLiveListPanel.BackgroundColor = new UIColor(0f, 0f, 0f, 0.5f);
                        nextLiveListPanel.ItemHeight = articleItemHeight + articleVGap;
                        nextLiveListPanel.ItemCount = currentRssFeed.ArticleCount;
                        nextLiveListPanel.SetListItemCreator(ListItemCreator);
                        nextLiveListPanel.SetListItemUpdater(ListItemUpdator);
                        nextLiveListPanel.ItemTiltAngle = liveListPanelItemTiltAngle;
                        nextLiveListPanel.StartItemRequest();

                        currentArticlePanel = nextLiveListPanel;

                        inEffect = new BunjeeJumpEffect(currentArticlePanel, bunjeeJumpElasticity);
                        inEffect.EffectStopped += new EventHandler<EventArgs>(InEffectStop);
                        inEffect.Start();
                        break;
                }
            }
        }

        private static ListPanelItem ListItemCreator()
        {
            ArticleListPanelItem item = new ArticleListPanelItem();
            item.TouchEventReceived += new EventHandler<TouchEventArgs>(ListItemTouchEventReceived);
            return item;
        }

        private static void ListItemUpdator(ListPanelItem item)
        {
            if (item is ArticleListPanelItem)
            {
                ArticleListPanelItem targetItem = (item as ArticleListPanelItem);

                Random random = new Random();
                int articleBackImageCount = articleItemBackImage.Count;
                for (; ; )
                {
                    int randomIndex = random.Next(articleBackImageCount);
                    if (randomIndex != prevBackImageIndex)
                    {
                        targetItem.BackImage = articleItemBackImage[randomIndex];
                        prevBackImageIndex = randomIndex;
                        break;
                    }
                }

                if( currentRssFeed != null )
                {
                    RssArticle article = currentRssFeed.GetArticle(item.Index);
                    if( article != null )
                    {
                        targetItem.Text = article.ArticleTitle;
                        if (article.AlreadyRead )
                        {
                            targetItem.Alpha = articleAlreadyreadAlpha;
                        }
                        else
                        {
                            targetItem.Alpha = articleUnreadAlpha;
                        }
                    }
                }
            }
        }

        static void ListItemTouchEventReceived(object sender, TouchEventArgs e)
        {
            TouchEvent touchEvent = e.TouchEvents.PrimaryTouchEvent;
            ArticleListPanelItem item = (sender as ArticleListPanelItem);
            if (item != null)
            {
                switch (touchEvent.Type)
                {
                    case TouchEventType.Up:
                        if(currentRssFeed != null && articlePanel == null)
                        {
                            currentRssFeed.SetArticleRead(item.Index);
                            item.Alpha = 0.5f;

                            if (currentSelectedFeedItem.Index == currentSelectedFeedIndex)
                            {
                                currentSelectedFeedItem.Text
                                    = currentRssFeed.RssTitle + "(" + currentRssFeed.UnreadCount + ")";
                            }

                            scene.BaseLiveListPanel.TouchResponse = false;

                            articlePanel = new ArticlePanel(currentRssFeed, item.Index);
                            articlePanel.ChangeArticle += new ArticlePanel.ArticleChangeEventHandler(PanelArticleChange);
                            articlePanel.CloseButtonAction += new EventHandler(ArticlePanelCloseButtonAction);
                            articlePanel.BackImage = item.BackImage;
                            scene.RootWidget.AddChildLast(articlePanel);

                            Matrix4 mat;
                            Matrix4 baseTransformMat = articlePanel.Transform3D;
                            Matrix4 scaleMat
                                = Matrix4.Scale(new Vector3(articlePanelDefaultScale, articlePanelDefaultScale, articlePanelDefaultScale));
                            Matrix4.Multiply(ref baseTransformMat, ref scaleMat, out mat);
                            articlePanel.Transform3D = mat;

////////////////////////////////////////////////////////////////////
                            Matrix4 world = item.Transform3D;
                            world.M41 -= item.Width /2;
                            world.M42 -= item.Height /2;
                             for (Widget wgt = item.Parent; wgt != null; wgt = wgt.Parent) {
                                 Matrix4 localToParent = wgt.Transform3D;
                
                                 switch (wgt.PivotType) {
                                     case PivotType.TopCenter:
                                     case PivotType.MiddleCenter:
                                     case PivotType.BottomCenter:
                                         localToParent.M41 -= wgt.Width / 2.0f;
                                         break;
                                     case PivotType.TopRight:
                                     case PivotType.MiddleRight:
                                     case PivotType.BottomRight:
                                         localToParent.M41 -= wgt.Width;
                                         break;
                                     default:
                                         break;
                                 }
                                 switch (wgt.PivotType) {
                                     case PivotType.MiddleLeft:
                                     case PivotType.MiddleCenter:
                                     case PivotType.MiddleRight:
                                         localToParent.M42 -= wgt.Height / 2.0f;
                                         break;
                                     case PivotType.BottomLeft:
                                     case PivotType.BottomCenter:
                                     case PivotType.BottomRight:
                                         localToParent.M42 -= wgt.Height;
                                         break;
                                     default:
                                         break;
                                 }

                                 world = localToParent * world;
                             }
/////////////////////////////////////////////////////

                            articlePanel.X = world.M41 + touchEvent.LocalPosition.X;
                            articlePanel.Y = world.M42 + touchEvent.LocalPosition.Y;
                            articlePanel.Width = scene.RootWidget.Width;
                            articlePanel.Height = scene.RootWidget.Height;

                            ZoomEffect articlePanelZoomEffect
                                = new ZoomEffect(articlePanel, articlePanelInEffectTime, articlePanelNormalScale,
                                    ZoomEffectInterpolator.EaseOutQuad);
                            articlePanelZoomEffect.Start();

                            MoveEffect articlePanelMoveEffect
                                = new MoveEffect(articlePanel, articlePanelInEffectTime, zeroPosition, zeroPosition,
                                    MoveEffectInterpolator.EaseOutQuad);
                            articlePanelMoveEffect.Start();
                        }
                        break;
                }
            }
        }

        static void EffectStop(object sender, EventArgs e)
        {
            scene.RootWidget.RemoveChild(outPanel);
            outEffect = null;
        }

        static void InEffectStop(object sender, EventArgs e)
        {
            inEffect = null;
        }

        static void stopSceneTransition(object sender, EventArgs e)
        {
            FlipBoardTransition flipBoardTrantision = new FlipBoardTransition();
            flipBoardTrantision.Time = flipBoradTime;
            UISystem.SetScene(scene, flipBoardTrantision);
        }

        static float CrossFadeInterpolator(float from, float to, float ratio)
        {
            // easeOutCubic
            return AnimationUtility.EaseOutCubicInterpolator(from, to, ratio);
        }

        static private void PanelArticleChange(object sender, ArticleChangeEventArgs e)
        {

            System.Console.WriteLine("e.NewIndex: " + e.NewIndex);

            if (currentSelectedFeedItem.Index == currentSelectedFeedIndex)
            {
                currentSelectedFeedItem.Text
                    = currentRssFeed.RssTitle + "(" + currentRssFeed.UnreadCount + ")";
            }

        }

        static void ArticlePanelCloseButtonAction(object sender, EventArgs e)
        {
            if (articlePanel != null)
            {
                FadeOutEffect articlePanelOutEffect = new FadeOutEffect();
                articlePanelOutEffect.Time = articlePanelOutEffectTime;
                articlePanelOutEffect.Widget = articlePanel;
                articlePanelOutEffect.EffectStopped += new EventHandler<EventArgs>(ArticlePanelCloseEffectStopped);

                articlePanel.TouchResponse = false;
                articlePanelOutEffect.Start();
            }
        }

        static void ArticlePanelCloseEffectStopped(object sender, EventArgs e)
        {
            scene.RootWidget.RemoveChild(articlePanel);
            articlePanel = null;

            scene.BaseLiveListPanel.TouchResponse = true;
        }

        static float getPixelDensity()
        {
            float pixelDensity;

            float w = graphics.Screen.Width / SystemParameters.DisplayDpiX;
            float h = graphics.Screen.Height / SystemParameters.DisplayDpiY;
            float inchDiagSq = w * w + h * h;

            if (inchDiagSq < 6 * 6)
            {
                // normal size display ( < 6 inch)
                if (SystemParameters.DisplayDpiX < 300)
                {
                    // normal resolution
                    pixelDensity = 1.0f;
                }
                else
                {
                    // high resolution
                    pixelDensity = 1.5f;
                }
            }
            else
            {
                // large size display ( > 6 inch)
                if (SystemParameters.DisplayDpiX < 200)
                {
                    // normal resolution
                    pixelDensity = 1.0f;
                }
                else
                {
                    // high resolution
                    pixelDensity = 1.5f;
                }
            }
            return pixelDensity;
        }
    }

}
