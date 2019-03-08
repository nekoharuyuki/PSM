using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
    public partial class MyStoreMessage : Dialog
    {
        //  Constructor ( with text, close handler )

        public MyStoreMessage(string text, EventHandler<DialogEventArgs> onClose = null)
            : base(null, null)
        {
            InitializeWidget();

            Button_1.ButtonAction += (sender, e)=>{ Hide(); };
            if (onClose != null) Hiding += onClose;

            Label_1.Text = text;
            Show();
        }

        //  Constructor ( with exception, close handler )

        public MyStoreMessage(Exception e, EventHandler<DialogEventArgs> onClose = null)
            : this(e.GetType().ToString() + "\n\n" + e.Message, onClose)
        {
        }
    }
}
