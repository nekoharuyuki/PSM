/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Services;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
    public static class AppMain
    {
        private static GraphicsContext graphics;
        private static MyStore store;

        private static InAppPurchaseDialog dialog;
        private static bool dialogIsBusy;
        private static bool[] itemIsSelected ;

        //  Main program

        public static void Main(string[] args)
        {
            Initialize();
            while (true) {
                SystemEvents.CheckEvents();
                Update();
                Render();
            }
        }
        public static void Initialize()
        {
            graphics = new GraphicsContext();

            UISystem.Initialize(graphics);
            store = new MyStore();
            UISystem.SetScene(store, null);

            InitializeDialog();
        }
        public static void Update()
        {
            List<TouchData> touchDataList = Touch.GetData(0);
            UISystem.Update(touchDataList);

            UpdateDialog();
        }
        public static void Render()
        {
            graphics.SetClearColor(0.3f, 0.5f, 1.0f, 0.0f);
            graphics.Clear();
            UISystem.Render();
            graphics.SwapBuffers();
        }

        //  Dialog functions

        public static void InitializeDialog()
        {
            dialog = new InAppPurchaseDialog();
            dialogIsBusy = false ;

            int count = dialog.ProductList.Count;
            itemIsSelected = new bool[count];

            store.SetItemCount(count);
            store.UpdateItems();
        }
        public static void UpdateDialog()
        {
            if (!dialogIsBusy) return;

            if (dialog.State == CommonDialogState.Finished) {
                dialogIsBusy = false;
                if (dialog.Result != CommonDialogResult.OK) {
                    new MyStoreMessage("Dialog result is \"" + dialog.Result.ToString() + "\"");
                }

                Array.Clear(itemIsSelected, 0, itemIsSelected.Length);
                store.SetItemCount(dialog.ProductList.Count);
                store.UpdateItems();
            }
        }

        //  Button events

        public static void OnGetProductInfo(object sender, TouchEventArgs e)
        {
            try {
                dialog.GetProductInfo(GetSelectedItems()) ;
                dialogIsBusy = true;
            } catch (Exception exception) {
                new MyStoreMessage(exception);
            }
        }
        public static void OnGetTicketInfo(object sender, TouchEventArgs e)
        {
            try {
                dialog.GetTicketInfo();
                dialogIsBusy = true;
            } catch (Exception exception) {
                new MyStoreMessage(exception);
            }
        }
        public static void OnPurchase(object sender, TouchEventArgs e)
        {
            try {
                dialog.Purchase(GetSelectedItem());
                dialogIsBusy = true;
            } catch (Exception exception) {
                new MyStoreMessage(exception);
            }
        }
        public static void OnConsume(object sender, TouchEventArgs e)
        {
            try {
                dialog.Consume(GetSelectedItem());
                dialogIsBusy = true;
            } catch (Exception exception) {
                new MyStoreMessage(exception);
            }
        }

        //  Item events

        public static ListPanelItem OnCreateItem()
        {
            return new MyStoreItem(OnCheckItem);
        }
        public static void OnCheckItem(object sender, TouchEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var storeItem = checkBox.Parent as MyStoreItem;
            itemIsSelected[storeItem.Index] = checkBox.Checked;
        }
        public static void OnUpdateItem(ListPanelItem item)
        {
            var storeItem = item as MyStoreItem;
            var product = dialog.ProductList[item.Index];
            storeItem.ItemCheck = itemIsSelected[item.Index];
            storeItem.ItemIndex = item.Index.ToString();
            storeItem.ItemLabel = product.Label;
            storeItem.ItemName = product.Name;

            storeItem.ItemPrice = (product.Price == "") ? "---" : product.Price;

            if (product.TicketType == InAppPurchaseTicketType.Normal) {
                storeItem.ItemTicket = !dialog.IsTicketInfoComplete ? "---" :
                                            (product.IsTicketValid ? "Yes" : "No");
            } else {
                storeItem.ItemTicket = !dialog.IsTicketInfoComplete ? "---" :
                                            product.ConsumableTicketCount.ToString();
            }
        }

        //  Subroutines

        public static string[] GetSelectedItems()
        {
            var items = new List<string>();
            for (int i = 0; i < itemIsSelected.Length; i ++) {
                if (itemIsSelected[i]) items.Add(dialog.ProductList[i].Label);
            }
            return (items.Count == 0) ? null : items.ToArray();
        }
        public static string GetSelectedItem()
        {
            var items = GetSelectedItems();
            if (items == null || items.Length != 1) {
                throw new ArgumentOutOfRangeException("", "Please select 1 item\n");
            }
            return items[0];
        }
    }
}
