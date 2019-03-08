/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;

using Sce.PlayStation.Core;
using System.Diagnostics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>スライダーの方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider direction</summary>
        /// @endif
        public enum SliderOrientation
        {
            /// @if LANG_JA
            /// <summary>水平方向</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Horizontal</summary>
            /// @endif
            Horizontal = 0,

            /// @if LANG_JA
            /// <summary>垂直方向</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Vertical</summary>
            /// @endif
            Vertical
        }


        /// @if LANG_JA
        /// <summary>数値などの調整を行うためのウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>A widget for adjusting numerical values, etc.</summary>
        /// @endif
        public class Slider : Widget
        {

            private const float defaultSliderHorizontalWidth = 362.0f;
            private const float defaultSliderHorizontalHeight = 58.0f;

            private const float defaultSliderVerticalWidth = 58.0f;
            private const float defaultSliderVerticalHeight = 232.0f;

            private const float barMargin = 5.0f;
            private const int SliderOrientationCount = 2;

            private readonly Rectangle horizontalFocusRectangle = new Rectangle(4, 4, 32, 46);
            private readonly Rectangle verticalFocusRectangle = new Rectangle(6, 3, 46, 32);

            private float tempValue;
            private float lastValue;
            private Vector2 touchDownPos;

            // if Step==0 then this value is not used
            private const int keyFocusMovePixel = 5;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public Slider()
            {
                // setup image asset
                this.baseImageAssets = new ImageAsset[SliderOrientationCount, PressStateChangedEventArgs.PressStateCount];
                this.baseImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Normal] = new ImageAsset(SystemImageAsset.SliderVerticalBaseNormal);
                this.baseImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Pressed] = this.baseImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Normal];
                this.baseImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Disabled] = new ImageAsset(SystemImageAsset.SliderHorizontalBaseDisabled);
                this.baseImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Normal] = new ImageAsset(SystemImageAsset.SliderHorizontalBaseNormal);
                this.baseImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Pressed]  = this.baseImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Normal];
                this.baseImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Disabled] = new ImageAsset(SystemImageAsset.SliderHorizontalBaseDisabled);

                this.barImageAssets = new ImageAsset[SliderOrientationCount, PressStateChangedEventArgs.PressStateCount];
                this.barImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Normal] = new ImageAsset(SystemImageAsset.SliderVerticalBarNormal);
                this.barImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Pressed] = this.barImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Normal];
                this.barImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Disabled] = new ImageAsset(SystemImageAsset.SliderVerticalBarDisabled);
                this.barImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Normal] = new ImageAsset(SystemImageAsset.SliderHorizontalBarNormal);
                this.barImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Pressed] = this.barImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Normal];
                this.barImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Disabled] = new ImageAsset(SystemImageAsset.SliderHorizontalBarDisabled);

                this.barNinePatchMargins = new NinePatchMargin[SliderOrientationCount];
                this.barNinePatchMargins[(int)SliderOrientation.Vertical] = AssetManager.GetNinePatchMargin(SystemImageAsset.SliderVerticalBaseNormal);
                this.barNinePatchMargins[(int)SliderOrientation.Horizontal] = AssetManager.GetNinePatchMargin(SystemImageAsset.SliderHorizontalBaseNormal);

                this.handleImageAssets = new ImageAsset[SliderOrientationCount, PressStateChangedEventArgs.PressStateCount];
                this.handleImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Normal] = new ImageAsset(SystemImageAsset.SliderVerticalHandleNormal);
                this.handleImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Pressed] = new ImageAsset(SystemImageAsset.SliderVerticalHandlePressed);
                this.handleImageAssets[(int)SliderOrientation.Vertical, (int)PressState.Disabled] = new ImageAsset(SystemImageAsset.SliderVerticalHandleDisabled);
                this.handleImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Normal] = new ImageAsset(SystemImageAsset.SliderHorizontalHandleNormal);
                this.handleImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Pressed] = new ImageAsset(SystemImageAsset.SliderHorizontalHandlePressed);
                this.handleImageAssets[(int)SliderOrientation.Horizontal, (int)PressState.Disabled] = new ImageAsset(SystemImageAsset.SliderHorizontalHandleDisabled);

                // initialize member
                this.baseImage = new ImageBox();
                this.baseImage.ImageScaleType = ImageScaleType.NinePatch;
                this.baseImage.TouchResponse = false;
                this.AddChildLast(this.baseImage);

                this.barImage = new ImageBox();
                this.barImage.ImageScaleType = ImageScaleType.NinePatch;
                this.barImage.TouchResponse = false;
                this.AddChildLast(this.barImage);

                this.handleImage = new HandleWidget();
                this.AddChildLast(this.handleImage);

                this.value = 0.0f;
                this.minValue = 0.0f;
                this.maxValue = 1.0f;
                this.step = 0.0f;

                this.orientation = SliderOrientation.Horizontal;

                this.HookChildTouchEvent = true;

                this.PriorityHit = true;
                this.Focusable = true;
                this.avoidFocusFromChildren = true;

                this.handleImage.PressStateChanged += new EventHandler<PressStateChangedEventArgs>(handleImagePressStateChanged);

                updateOrientation();

                this.ValueChangeEventEnabled = true;
            }

            /// @if LANG_JA
            /// <summary>タッチイベントハンドラ</summary>
            /// <param name="touchEvents">タッチイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Touch event handler</summary>
            /// <param name="touchEvents">Touch event</param>
            /// @endif
            internal protected override void OnTouchEvent(Sce.PlayStation.HighLevel.UI.TouchEventCollection touchEvents)
            {
                base.OnTouchEvent(touchEvents);

                if (touchEvents.PrimaryTouchEvent.Type == TouchEventType.Down)
                {
                    lastValue = value;
                    touchDownPos = touchEvents.PrimaryTouchEvent.LocalPosition;
                }
                if (this.handleImage.PressState == PressState.Pressed &&
                   touchEvents.PrimaryTouchEvent.Type == Sce.PlayStation.HighLevel.UI.TouchEventType.Move)
                {
                    float diff;
                    if (this.orientation == SliderOrientation.Vertical)
                    {
                        diff = touchEvents.PrimaryTouchEvent.LocalPosition.Y - touchDownPos.Y;
                        diff *= - (this.maxValue - this.minValue) / (this.Height - this.handleImage.Height);
                    }
                    else
                    {
                        diff = touchEvents.PrimaryTouchEvent.LocalPosition.X - touchDownPos.X;
                        diff *= (this.maxValue - this.minValue) / (this.Width - this.handleImage.Width);
                    }

                    tempValue = calcValue(this.lastValue + diff);
                    setupImagePosByValue(tempValue);

                    // TODO
                    this.value = tempValue;

                    if (this.ValueChanging != null && this.ValueChangeEventEnabled)
                    {
                        this.ValueChanging(this, new SliderValueChangeEventArgs(tempValue));
                    }
                }

                touchEvents.Forward = true;
            }

            /// @if LANG_JA
            /// <summary>事前キーイベントのハンドラ</summary>
            /// <param name="keyEvent">キーイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Advance key event handler</summary>
            /// <param name="keyEvent">Key event</param>
            /// @endif
            protected internal override void OnPreviewKeyEvent(KeyEvent keyEvent)
            {
                base.OnPreviewKeyEvent(keyEvent);

                // four-way-key operation:
                // - if step == 0 then move by keyFocusMovePixel
                // - if step != 0 then move by step pixel 
                //   - when key down, move by step pixel
                //   - when key repeat, move by keyFocusMovePixel(round by step)
                bool pressValueChangeKey = false;
                if(this.Orientation == SliderOrientation.Horizontal)
                    pressValueChangeKey = (keyEvent.KeyType == KeyType.Left || keyEvent.KeyType == KeyType.Right);
                else
                    pressValueChangeKey = (keyEvent.KeyType == KeyType.Up || keyEvent.KeyType == KeyType.Down);

                if (!keyEvent.Handled && pressValueChangeKey)
                {
                    if (keyEvent.KeyEventType == KeyEventType.Down ||
                        keyEvent.KeyEventType == KeyEventType.Repeat)
                    {
                        tempValue = this.value;
                        float keyStep = this.step;
                        float minKeyStep = (this.maxValue - this.minValue) * keyFocusMovePixel;
                        if (this.orientation == SliderOrientation.Horizontal)
                            minKeyStep /= this.Width - this.handleImage.Width;
                        else
                            minKeyStep /= this.Height - this.handleImage.Height;


                        if (keyStep <= float.Epsilon)
                        {
                            keyStep = minKeyStep;
                        }
                        else if (keyEvent.KeyEventType == KeyEventType.Repeat)
                        {
                            keyStep = ((int)(minKeyStep / keyStep) + 1) * keyStep;
                        }


                        if (keyEvent.KeyType == KeyType.Left || keyEvent.KeyType == KeyType.Down)
                            tempValue -= keyStep;
                        else
                            tempValue += keyStep;

                        tempValue = FMath.Clamp(tempValue, this.minValue, this.maxValue);

                        // todo:
                        this.value = tempValue;

                        UpdateView();
                        if (this.ValueChanging != null && this.ValueChangeEventEnabled)
                        {
                            ValueChanging(this, new SliderValueChangeEventArgs(tempValue));
                        }

                    }
                    else if (keyEvent.KeyEventType == KeyEventType.Up)
                    {
                        //todo
                        this.Value = this.value;
                    }

                    this.handleImage.SetFocus(true);

                    keyEvent.Handled = true;
                }
            }

            void handleImagePressStateChanged(object sender, PressStateChangedEventArgs e)
            {
                if (e.NewState == PressState.Normal && e.OldState == PressState.Pressed)
                {
                    this.Value = tempValue;
                }
                else
                {
                    UpdateView();
                }
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                foreach (ImageAsset asset in this.baseImageAssets)
                {
                    asset.Dispose();
                }

                foreach (ImageAsset asset in this.barImageAssets)
                {
                    asset.Dispose();
                }

                foreach (ImageAsset asset in this.handleImageAssets)
                {
                    asset.Dispose();
                }

                base.DisposeSelf();
            }

            /// @if LANG_JA
            /// <summary>スライダーの方向を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the slider direction.</summary>
            /// @endif
            public SliderOrientation Orientation
            {
                get
                {
                    return this.orientation;
                }
                set
                {
                    if (this.orientation != value)
                    {
                        this.orientation = value;
                        updateOrientation();
                    }
                }
            }
            private SliderOrientation orientation;

            private void updateOrientation()
            {
                this.baseImage.Image = this.baseImageAssets[(int)this.orientation, (int)this.handleImage.PressState];
                this.baseImage.NinePatchMargin = this.barNinePatchMargins[(int)this.orientation];

                this.barImage.Image = this.barImageAssets[(int)this.orientation, (int)this.handleImage.PressState];
                this.barImage.NinePatchMargin = this.barNinePatchMargins[(int)this.orientation];

                this.handleImage.Image = this.handleImageAssets[(int)this.orientation, (int)this.handleImage.PressState];

                if (this.handleImage.FocusCustomSettings == null)
                    this.handleImage.FocusCustomSettings = new FocusCustomSettings();


                switch (this.orientation)
                {
                    case SliderOrientation.Vertical:
                        base.Width = defaultSliderVerticalWidth;
                        base.Height = defaultSliderVerticalHeight;

                        this.baseImage.Width = this.baseImage.Image.Width;
                        this.barImage.Width = this.baseImage.Width;
                        this.baseImage.X = (defaultSliderVerticalWidth - this.baseImage.Width - 2) / 2;
                        this.barImage.X = this.baseImage.X;

                        this.baseImage.Height = defaultSliderVerticalHeight - barMargin * 2;
                        this.barImage.Height = this.baseImage.Height;
                        this.baseImage.Y = barMargin;
                        this.barImage.Y = barMargin;

                        this.handleImage.FocusCustomSettings.FocusImageRectangle = verticalFocusRectangle;

                        break;

                    case SliderOrientation.Horizontal:
                        base.Width = defaultSliderHorizontalWidth;
                        base.Height = defaultSliderHorizontalHeight;

                        this.baseImage.Width = defaultSliderHorizontalWidth - barMargin * 2;
                        this.barImage.Width = this.baseImage.Width;
                        this.baseImage.X = barMargin;
                        this.barImage.X = barMargin;

                        this.baseImage.Height = this.baseImage.Image.Height;
                        this.barImage.Height = this.baseImage.Height;
                        this.baseImage.Y = (defaultSliderHorizontalHeight - this.baseImage.Height - 2) / 2;
                        this.barImage.Y = this.baseImage.Y;

                        this.handleImage.FocusCustomSettings.FocusImageRectangle = horizontalFocusRectangle;
                        break;
                }
                UpdateView();
            }


            /// @if LANG_JA
            /// <summary>緑色のバーを表示するかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to display the green bar.</summary>
            /// @endif
            public bool FillValue
            {
                get
                {
                    return this.barImage.Visible;
                }
                set { this.barImage.Visible = value; }
            }


            /// @if LANG_JA
            /// <summary>現在の位置を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the current position.</summary>
            /// @endif
            public float Value
            {
                get
                {
                    return this.value;
                }
                set
                {
                    this.value = calcValue(value);
                    this.lastValue = this.value;
                    this.tempValue = this.value;

                    UpdateView();

                    if (this.ValueChanged != null && this.ValueChangeEventEnabled)
                    {
                        this.ValueChanged(this, new SliderValueChangeEventArgs(this.value));
                    }
                }
            }
            private float value;

            /// @if LANG_JA
            /// <summary>最小値を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the minimum value.</summary>
            /// @endif
            public float MinValue
            {
                get
                {
                    return this.minValue;
                }
                set
                {
                    this.minValue = value;

                    if (this.minValue > this.maxValue)
                    {
                        this.maxValue = this.minValue;
                    }

                    if (this.value < this.minValue)
                    {
                        this.value = this.minValue;
                    }

                    UpdateView();
                }
            }
            private float minValue;

            /// @if LANG_JA
            /// <summary>最大値を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the maximum value.</summary>
            /// @endif
            public float MaxValue
            {
                get
                {
                    return this.maxValue;
                }
                set
                {
                    this.maxValue = value;

                    if (this.minValue > this.maxValue)
                    {
                        this.minValue = this.maxValue;
                    }

                    if (this.value > this.maxValue)
                    {
                        this.value = this.maxValue;
                    }

                    UpdateView();
                }
            }
            private float maxValue;

            /// @if LANG_JA
            /// <summary>現在の位置が変化したときのイベントを有効にするかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to enable an event when the current position has changed.</summary>
            /// @endif
            public bool ValueChangeEventEnabled
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>機能を有効にするかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to enable the feature.</summary>
            /// @endif
            public override bool Enabled
            {
                get { return handleImage.Enabled; }
                set { this.handleImage.Enabled = value; }
            }

            /// @if LANG_JA
            /// <summary>幅を取得・設定する。</summary>
            /// <remarks>OrientationがVerticalの場合、設定された値は無視される。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width.</summary>
            /// <remarks>The set value is ignored if Orientation is Vertical.</remarks>
            /// @endif
            public override float Width
            {
                get
                {
                    return base.Width;
                }
                set
                {
                    if (this.Orientation == SliderOrientation.Horizontal)
                    {
                        base.Width = value;

                        if (baseImage != null)
                        {
                            this.baseImage.Width = value - barMargin * 2;
                            UpdateView();
                        }
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>高さを取得・設定する。</summary>
            /// <remarks>OrientationがHorizontalの場合、設定された値は無視される。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height.</summary>
            /// <remarks>The set value is ignored if Orientation is Horizontal.</remarks>
            /// @endif
            public override float Height
            {
                get
                {
                    return base.Height;
                }
                set
                {
                    if (this.Orientation == SliderOrientation.Vertical)
                    {
                        base.Height = value;

                        if (baseImage != null)
                        {
                            this.baseImage.Height = value - barMargin * 2;
                            UpdateView();
                        }
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>優先してタッチに反応するかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to prioritize and respond to a touch.</summary>
            /// @endif
            public override bool PriorityHit
            {
                get
                {
                    return base.PriorityHit;
                }
                set
                {
                    base.PriorityHit = value;
                    if (this.handleImage != null)
                    {
                        this.handleImage.PriorityHit = value;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>Valueの離散値</summary>
            /// <remarks>0を指定するとValueは連続的な値になる。正の値を指定すると、Valueのとりうる値はMinValue+Step*n（nは整数）とMaxValueになる。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Discrete value of Value</summary>
            /// <remarks>When 0 is specified, Value becomes a continuous value. When a positive value is specified, the possible value of Value is MinValue+Step*n (where n is an integer) and MaxValue.</remarks>
            /// @endif
            public float Step
            {
                get { return step; }
                set { step = value; }
            }
            private float step;

            /// @if LANG_JA
            /// <summary>このウィジェットがフォーカスを取得または喪失したときに呼び出される</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Called when this widget obtains or loses focus</summary>
            /// @endif
            internal protected override void OnFocusChanged(FocusChangedEventArgs args)
            {
                base.OnFocusChanged(args);
                if (this.Focused)
                {
                    this.handleImage.SetFocus(false);
                }
            }

            private float calcValue(float tempValue)
            {
                if (this.step > float.Epsilon)
                {
                    // discrete value by this.step
                    float resultValue = tempValue - ((tempValue + this.step * 0.5f - this.minValue) % this.step - this.step * 0.5f);
                    if (resultValue > this.maxValue - ((this.maxValue - this.minValue) % this.step) / 2f)
                    {
                        resultValue = this.maxValue;
                    }
                    return resultValue > this.minValue ? resultValue : this.minValue;
                }
                else
                {
                    // continuous value
                    return FMath.Clamp(tempValue, this.minValue, this.maxValue);
                }
            }

            private void setupImagePosByValue(float value)
            {
                float currentValue = value - this.MinValue;
                float x, y;
                if (this.Orientation == SliderOrientation.Vertical)
                {
                    // invert
                    currentValue = this.MaxValue - value;

                    float yMax = Math.Max(0.0f, this.Height - this.handleImage.Height);

                    x = (this.Width - this.handleImage.Width) / 2.0f;

                    if ((this.MaxValue - this.MinValue) > 0)
                    {
                        y = yMax / (this.MaxValue - this.MinValue) * currentValue;
                    }
                    else
                    {
                        y = 0.0f;
                    }

                    this.barImage.Y = y + this.handleImage.Height / 2.0f;
                    this.barImage.Height = this.Height - barMargin - this.barImage.Y;
                }
                else
                {
                    float xMax = Math.Max(0.0f, this.Width - this.handleImage.Width);

                    y = (this.Height - this.handleImage.Height) / 2.0f;

                    if ((this.MaxValue - this.MinValue) > 0)
                    {
                        x = xMax / (this.MaxValue - this.MinValue) * currentValue;
                    }
                    else
                    {
                        x = 0.0f;
                    }

                    this.barImage.Width = x + this.handleImage.Width / 2.0f;
                }

                this.handleImage.SetPosition(x, y);
            }

            private void UpdateView()
            {
                this.handleImage.Image = this.handleImageAssets[(int)this.Orientation, (int)this.handleImage.PressState];

                setupImagePosByValue(this.value);
            }

            /// @if LANG_JA
            /// <summary>現在の位置が変化しているときに呼び出されるハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Handler called when the current position is being changed</summary>
            /// @endif
            public event EventHandler<SliderValueChangeEventArgs> ValueChanging;

            /// @if LANG_JA
            /// <summary>現在の位置が変化したときに呼び出されるハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Handler called when the current position has changed</summary>
            /// @endif
            public event EventHandler<SliderValueChangeEventArgs> ValueChanged;


            private ImageBox baseImage;
            private ImageAsset[,] baseImageAssets;
            private NinePatchMargin[] barNinePatchMargins;

            private ImageBox barImage;
            private ImageAsset[,] barImageAssets;

            private HandleWidget handleImage;
            private ImageAsset[,] handleImageAssets;

            private class HandleWidget : Widget
            {
                UISprite bgSprite;

                public HandleWidget()
                {
                    bgSprite = new UISprite(1);
                    bgSprite.ShaderType = ShaderType.Texture;
                    this.RootUIElement.AddChildFirst(bgSprite);

                    this.Pressable = true;
                    this.TouchLeaveBehavior = PressStateTouchLeaveBehavior.KeepPressed;
                }

                public ImageAsset Image
                {
                    get { return bgSprite.Image; }
                    set
                    {
                        var unit = bgSprite.GetUnit(0);
                        this.SetSize(value.Width, value.Height);
                        unit.SetSize(value.Width, value.Height);
                        bgSprite.Image = value;
                    }
                }

                public new PressState PressState
                {
                    get { return base.PressState; }
                }

                public event EventHandler<PressStateChangedEventArgs> PressStateChanged;

                /// @if LANG_JA
                /// <summary>プレス状態が変化したときに呼び出される</summary>
                /// <param name="e">イベント引数</param>
                /// @endif
                /// @if LANG_EN
                /// <summary>Called when the press status changes</summary>
                /// <param name="e">Event argument</param>
                /// @endif
                protected override void OnPressStateChanged(PressStateChangedEventArgs e)
                {
                    if (PressStateChanged != null)
                        PressStateChanged(this, e);
                }
            }
        }


        /// @if LANG_JA
        /// <summary>現在の位置が変化したときのイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event argument when the current position has changed</summary>
        /// @endif
        public class SliderValueChangeEventArgs : EventArgs
        {
            float value;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public SliderValueChangeEventArgs(float value)
            {
                this.value = value;
            }

            /// @if LANG_JA
            /// <summary>現在の位置を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the current position.</summary>
            /// @endif
            public float Value
            {
                get { return this.value; }
            }

        }


    }
}
