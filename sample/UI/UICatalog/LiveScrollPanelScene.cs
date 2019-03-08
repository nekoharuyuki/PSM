/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class LiveScrollPanelScene : ContentsScene
    {
        public LiveScrollPanelScene()
        {
            InitializeWidget();

            this.Name = "LiveScrollPanel";
        }
        
        protected override void OnShowing()
        {
            base.OnShowing();

            if (liveScrollPanel.Children.GetEnumerator().Current == null)
            {
                int img_width = (int)(liveScrollPanel.Width / 2.0f);
                int img_height = (int)(img_width * 2f / 3f);
    
                // Scroll Panel
                liveScrollPanel.PanelWidth = liveScrollPanel.Width;
                liveScrollPanel.PanelHeight = img_height * 5.0f;
                if (UISystem.GraphicsContext.Caps.MaxTextureSize <= liveScrollPanel.PanelHeight + 2)
                {
                    liveScrollPanel.PanelHeight = UISystem.GraphicsContext.Caps.MaxTextureSize - 2;
                }
                liveScrollPanel.PanelColor = new UIColor(0f, 0f, 0.5f, 1f);
    
                // inner contents
                ImageBox img1 = new ImageBox();
                img1.Image = new ImageAsset("/Application/assets/photo01.png");
                img1.X = 0.0f;
                img1.Y = 0.0f;
                img1.Width = img_width;
                img1.Height = img_height;
                liveScrollPanel.AddChildLast(img1);
    
                ImageBox img2 = new ImageBox();
                img2.Image = new ImageAsset("/Application/assets/photo02.png");
                img2.X = this.RootWidget.Width - img_width;
                img2.Y = 0.0f;
                img2.Width = img_width;
                img2.Height = img_height;
                liveScrollPanel.AddChildLast(img2);
    
                ImageBox img3 = new ImageBox();
                img3.Image = new ImageAsset("/Application/assets/photo03.png");
                img3.X = img1.X;
                img3.Y = img1.Y + img1.Height;
                img3.Width = img_width;
                img3.Height = img_height;
                liveScrollPanel.AddChildLast(img3);
    
                ImageBox img4 = new ImageBox();
                img4.Image = new ImageAsset("/Application/assets/photo04.png");
                img4.X = img2.X;
                img4.Y = img2.Y + img2.Height;
                img4.Width = img_width;
                img4.Height = img_height;
                liveScrollPanel.AddChildLast(img4);
    
                ImageBox img5 = new ImageBox();
                img5.Image = new ImageAsset("/Application/assets/photo05.png");
                img5.X = img1.X;
                img5.Y = img3.Y + img3.Height;
                img5.Width = img_width;
                img5.Height = img_height;
                liveScrollPanel.AddChildLast(img5);
    
                ImageBox img6 = new ImageBox();
                img6.Image = new ImageAsset("/Application/assets/photo06.png");
                img6.X = img2.X;
                img6.Y = img4.Y + img4.Height;
                img6.Width = img_width;
                img6.Height = img_height;
                liveScrollPanel.AddChildLast(img6);
    
                ImageBox img7 = new ImageBox();
                img7.Image = new ImageAsset("/Application/assets/photo07.png");
                img7.X = img1.X;
                img7.Y = img5.Y + img5.Height;
                img7.Width = img_width;
                img7.Height = img_height;
                liveScrollPanel.AddChildLast(img7);
    
                ImageBox img8 = new ImageBox();
                img8.Image = new ImageAsset("/Application/assets/photo08.png");
                img8.X = img2.X;
                img8.Y = img6.Y + img6.Height;
                img8.Width = img_width;
                img8.Height = img_height;
                liveScrollPanel.AddChildLast(img8);
    
                ImageBox img9 = new ImageBox();
                img9.Image = new ImageAsset("/Application/assets/photo09.png");
                img9.X = img1.X;
                img9.Y = img7.Y + img7.Height;
                img9.Width = img_width;
                img9.Height = img_height;
                liveScrollPanel.AddChildLast(img9);
    
                ImageBox img10 = new ImageBox();
                img10.Image = new ImageAsset("/Application/assets/photo10.png");
                img10.X = img2.X;
                img10.Y = img8.Y + img8.Height;
                img10.Width = img_width;
                img10.Height = img_height;
                liveScrollPanel.AddChildLast(img10);
            }
        }
    }
}
