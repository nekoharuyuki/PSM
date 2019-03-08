using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Services;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
    public partial class MyStore : Scene
    {
        private InAppPurchaseDialog dialog;
        private bool dialogIsBusy;
        private int selectedItem;

        private enum DialogSequence { Stop, Start, Ready, Action };
        private DialogSequence dialogSequence;

        private InAppPurchaseCommand actionCommand;
        private string actionLabel;
        private bool actionTicketIsOK;
        private int actionTicketCount;

        public delegate void PurchaseHandler(InAppPurchaseProduct product);
        public delegate void ConsumeHandler(InAppPurchaseProduct product, int count);
        public /*event*/ PurchaseHandler Purchased;
        public /*event*/ ConsumeHandler Consumed;

        //  Constructor ( with purchase handler, consumption handler )

        public MyStore(PurchaseHandler onPurchased = null, ConsumeHandler onConsumed = null)
        {
            InitializeWidget();

            Button_1.ButtonAction += OnPurchase;
            Button_2.ButtonAction += OnConsume;
            Button_3.ButtonAction += OnClose;

            ListPanel_1.SetListItemCreator(OnCreateItem);
            ListPanel_1.SetListItemUpdater(OnUpdateItem);
            ListPanel_1.Sections = new ListSectionCollection { new ListSection("Section", 0) };

            if (onPurchased != null) Purchased = onPurchased;
            if (onConsumed != null) Consumed = onConsumed;

            InitializeDialog();
        }

        //  Public methods

        public void SetItemCount(int count)
        {
            var listSection = ListPanel_1.Sections[0];
            if (listSection.ItemCount != count) listSection.ItemCount = count;
        }
        public void UpdateItems()
        {
            ListPanel_1.UpdateItems();
            SwitchButtons();
        }

        //  Override events

        protected override void OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);
            UpdateDialog();
        }
        protected override void OnShown()
        {
            base.OnShown();
            dialogSequence = DialogSequence.Start;
            dialogIsBusy = false;
        }

        //  Dialog functions

        private void InitializeDialog()
        {
            dialog = new InAppPurchaseDialog();
            dialogSequence = DialogSequence.Stop;
            dialogIsBusy = false;
            selectedItem = 0;

            SetItemCount(dialog.ProductList.Count);
            UpdateItems();
        }
        private void UpdateDialog()
        {
            bool finished = false;
            if (dialogIsBusy) {
                if (dialog.State == CommonDialogState.Running) return;
                if (dialog.Result == CommonDialogResult.OK) {
                    SetItemCount(dialog.ProductList.Count);
                    UpdateItems();
                }
                dialogIsBusy = false;
                finished = true;
            }
            switch (dialogSequence) {
                case DialogSequence.Stop:
                    break;
                case DialogSequence.Start:
                    if (finished) {
                        if (dialog.Result != CommonDialogResult.OK) {
                            new MyStoreMessage("Sorry. We cannot access the store now.", OnClose);
                            dialogSequence = DialogSequence.Stop;
                            break;
                        }
                    }
                    if (!dialog.IsProductInfoComplete) {
                        dialog.GetProductInfo(null);
                        dialogIsBusy = true;
                    } else if (!dialog.IsTicketInfoComplete) {
                        dialog.GetTicketInfo();
                        dialogIsBusy = true;
                    } else {
                        //  confirm the result of previous action ( if exists )
                        CheckActionResult();
                        dialogSequence = DialogSequence.Ready;
                    }
                    break;
                case DialogSequence.Ready:
                    break;
                case DialogSequence.Action:
                    if (finished) {
                        if (dialog.IsTicketInfoComplete) {
                            //  confirm the result
                            CheckActionResult();
                            dialogSequence = DialogSequence.Ready;
                        } else {
                            //  cannot confirm the result until followup query
                            dialogSequence = DialogSequence.Start;
                        }
                    }
                    break;
            }
        }

        //  Button events

        public void OnPurchase(object sender, EventArgs e)
        {
            try {
                if (selectedItem < 0) return;
                CheckActionStart(InAppPurchaseCommand.Purchase);
                dialog.Purchase(dialog.ProductList[selectedItem].Label);
                dialogSequence = DialogSequence.Action;
                dialogIsBusy = true;
            } catch (Exception exception) {
                new MyStoreMessage(exception);
            }
        }
        public void OnConsume(object sender, EventArgs e)
        {
            try {
                if (selectedItem < 0) return;
                CheckActionStart(InAppPurchaseCommand.Consume);
                dialog.Consume(dialog.ProductList[selectedItem].Label);
                dialogSequence = DialogSequence.Action;
                dialogIsBusy = true;
            } catch (Exception exception) {
                new MyStoreMessage(exception);
            }
        }
        public void OnClose(object sender, EventArgs e)
        {
            UISystem.PopScene();
        }

        //  Item events

        public ListPanelItem OnCreateItem()
        {
            return new MyStoreItem(OnCheckItem);
        }
        public void OnCheckItem(object sender, EventArgs e)
        {
            var checkBox = sender as CheckBox;
            var storeItem = checkBox.Parent as MyStoreItem;
            selectedItem = (storeItem.Index == selectedItem) ? -1 : storeItem.Index;
            UpdateItems();
        }
        public void OnUpdateItem(ListPanelItem item)
        {
            var storeItem = item as MyStoreItem;
            var product = dialog.ProductList[item.Index];
            storeItem.ItemCheck = (item.Index == selectedItem);
            storeItem.ItemIndex = (item.Index + 1).ToString();
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

        //  Button state

        private void SwitchButtons()
        {
            bool purchaseable = false;
            bool consumable = false;
            if (selectedItem >= 0 && dialog.IsProductInfoComplete && dialog.IsTicketInfoComplete) {
                var product = dialog.ProductList[selectedItem];
                if (product.TicketType == InAppPurchaseTicketType.Normal) {
                    purchaseable = !product.IsTicketValid;
                } else {
                    purchaseable = true;
                    consumable = product.IsTicketValid && (product.ConsumableTicketCount > 0);
                }
            }
            Button_1.Enabled = purchaseable;
            Button_2.Enabled = consumable;
        }

        //  Action report

        private void CheckActionStart(InAppPurchaseCommand command)
        {
            var product = dialog.ProductList[selectedItem];
            actionCommand = command;
            actionLabel = product.Label;
            actionTicketIsOK = product.IsTicketValid;
            actionTicketCount = product.ConsumableTicketCount;
        }
        private void CheckActionResult()
        {
            var command = actionCommand;
            actionCommand = InAppPurchaseCommand.None;
            if (command == InAppPurchaseCommand.None) return;

            var product = dialog.ProductList[actionLabel];
            bool consumable = (product.TicketType == InAppPurchaseTicketType.Consumable);
            int count1 = !actionTicketIsOK ? 0 : (!consumable ? 1 : actionTicketCount);
            int count2 = !product.IsTicketValid ? 0 : (!consumable ? 1 : product.ConsumableTicketCount);
            bool purchasing = (command == InAppPurchaseCommand.Purchase);
            int count = purchasing ? (count2 - count1): (count1 - count2);
            if (count > 0) {
                if (purchasing && Purchased != null) Purchased(product);
                if (!purchasing && Consumed != null) Consumed(product, count);
            }

            #if DEBUG
            if (count > 0 || dialog.Result != CommonDialogResult.Canceled) {
                string result = (count > 0) ? " completed." : " failed.";
                string message = command.ToString() + " " + product.Label + result;
                // new MyStoreMessage(message);
                Console.WriteLine(message);
            }
            #endif
        }
    }
}
