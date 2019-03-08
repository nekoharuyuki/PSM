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
    public partial class ListPanelScene : ContentsScene
    {
        private const int imageAssetCount = 10;
        readonly private ImageAsset[] listItemImageAssets;

        readonly private List<string> listItemStr = new List<string> {
            "Item 1-1", "Item 1-2", "Item 1-3", "Item 1-4", "Item 1-5",
            "Item 2-1", "Item 2-2", 
            "Item 3-1", "Item 3-2", "Item 3-3", "Item 3-4", "Item 3-5", "Item 3-6", "Item 3-7", "Item 3-8",
            "Item 4-1", "Item 4-2", "Item 4-3", "Item 4-4", "Item 4-5",
            "Item 5-1", "Item 5-2", "Item 5-3", "Item 5-4", "Item 5-5", "Item 5-6", "Item 5-7",
            "Item 6-1", "Item 6-2", "Item 6-3"
            };

        readonly private ListSectionCollection section = new ListSectionCollection {
            new ListSection("Section1", 5),
            new ListSection("Section2", 2),
            new ListSection("Section3", 8),
            new ListSection("Section4", 5),
            new ListSection("Section5", 7),
            new ListSection("Section6", 3),
            };

        public ListPanelScene()
        {
            InitializeWidget();

            this.Name = "ListPanel";

            // Setup ImageAsset
            listItemImageAssets = new ImageAsset[imageAssetCount];
            for (int i = 0; i < imageAssetCount; i++)
            {
                listItemImageAssets[i] = new ImageAsset("/Application/assets/photo" + (i + 1).ToString("00") + "_s.png");
            }

            listPanel.SelectItemChanged += new EventHandler<ListPanelItemSelectChangedEventArgs>(listPanel_SelectItemChanged);
        }

        void listPanel_SelectItemChanged(object sender, ListPanelItemSelectChangedEventArgs e)
        {
            this.Label_SelectItem.Text = string.Format("Index = {0}\nSectionIndex = {1}\nIndexInSection = {2}",
                e.Index, e.SectionIndex, e.IndexInSection);
        }

        protected override void OnShowing()
        {
            base.OnShowing();

            // ListPanel
            listPanel.BackgroundColor = new UIColor(0.1f, 0.1f, 0.1f, 1.0f);
            listPanel.SetListItemCreator(ListItemCreator);
            listPanel.SetListItemUpdater(ListItemUpdator);
            listPanel.Sections = section;
        }

        private ListPanelItem ListItemCreator()
        {
            return new MyListPanelItem(listPanel.Width, listPanel.Height / 8.0f);
        }

        private void ListItemUpdator(ListPanelItem item)
        {
            if (item is MyListPanelItem)
            {
                MyListPanelItem myItem = (item as MyListPanelItem);
                myItem.Text = listItemStr[item.Index];
                myItem.Image = listItemImageAssets[item.Index % 10];
            }
        }
        
        private class MyListPanelItem : ListPanelItem 
        {
            const float margin = 5.0f;
            readonly UIColor normalBGColor = new UIColor(0.12f, 0.12f, 0.12f, 1.0f);
            readonly UIColor pressedBGColor = new UIColor(0.12f, 0.3f, 0.3f, 1.0f);

            public MyListPanelItem(float itemWidth, float itemHeight)
            {
                this.Width = itemWidth;
                this.Height = itemHeight;
                this.BackgroundColor = normalBGColor;
                
                image = new ImageBox();
                image.X = margin;
                image.Y = margin;
                image.Height = itemHeight - (margin * 2.0f);
                image.Width = image.Height * 3.0f / 2.0f;
 
                this.AddChildLast(image);

                label = new Label();
                label.X = image.X + image.Width + margin;
                label.Y = margin;
                label.Width = (label.X * 3.0f) - (margin * 2.0f);
                label.Height = itemHeight - (margin * 2.0f);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                this.AddChildLast(label);
            }

            public ImageAsset Image
            {
                get
                {
                    return image.Image;
                }
                set
                {
                    image.Image = value;
                }
            }

            public string Text
            {
                get
                {
                    return label.Text;
                }
                set
                {
                    label.Text = value;
                }
            }

            protected override void OnPressStateChanged(PressStateChangedEventArgs e)
            {
                base.OnPressStateChanged(e);

                if (PressState == PressState.Pressed)
                    this.BackgroundColor = pressedBGColor;
                else
                    this.BackgroundColor = normalBGColor;
            }

            private Label label;
            private ImageBox image;
        }
    }
}
