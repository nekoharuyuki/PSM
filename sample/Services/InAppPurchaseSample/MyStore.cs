using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
    public partial class MyStore : Scene
    {
        //  Constructor

        public MyStore()
        {
            InitializeWidget();

            Button_1.ButtonAction += AppMain.OnGetProductInfo;
            Button_2.ButtonAction += AppMain.OnGetTicketInfo;
            Button_3.ButtonAction += AppMain.OnPurchase;
            Button_4.ButtonAction += AppMain.OnConsume;

            ListPanel_1.SetListItemCreator(AppMain.OnCreateItem);
            ListPanel_1.SetListItemUpdater(AppMain.OnUpdateItem);
            ListPanel_1.Sections = new ListSectionCollection { new ListSection("Section", 0) };
        }

        //  Public Methods

        public void SetItemCount(int count) {
            var listSection = ListPanel_1.Sections[0];
            if (listSection.ItemCount != count) listSection.ItemCount = count;
        }
        public void UpdateItems() {
            ListPanel_1.UpdateItems();
        }
    }
}
