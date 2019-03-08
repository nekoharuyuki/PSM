using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Services;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
    public partial class MyApp : Scene
    {
        private MyStore store;

        //  Constructor

        public MyApp()
        {
            InitializeWidget();

            Button_1.ButtonAction += OnStart;

            UISystem.ScenePushTransition = new PushTransition(300, FourWayDirection.Left, PushTransitionInterpolator.EaseOutQuad);
            UISystem.ScenePopTransition = new PushTransition(300, FourWayDirection.Right, PushTransitionInterpolator.EaseOutQuad);
        }

        //  Button events

        public void OnStart(object sender, TouchEventArgs e)
        {
            if (store == null) store = new MyStore(OnPurchased, OnConsumed);
            UISystem.PushScene(store);
        }

        //  Store events

        public void OnPurchased(InAppPurchaseProduct product)
        {
            // TODO : product purchase is complete.
            // TODO : entitle the player to use the product.

            new MyStoreMessage("You purchased \"" + product.Name + "\"");
        }
        public void OnConsumed(InAppPurchaseProduct product, int count)
        {
            // TODO : product consumption is complete.
            // TODO : entitle the player to use the product.

            new MyStoreMessage("You consumed \"" + product.Name + "\"");
        }
    }
}
