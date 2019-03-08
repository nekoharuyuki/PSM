/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

#define FOCUS_MOVE_MERGE_BLINK

using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.UI
{

    internal class FocusScene : Scene
    {
        // Alpha and visible
        // + FocusScene
        //   + RootWidget      - Visible and Alpha are used by focus visible animation
        //     + RootUIElement - Alpha is used by focus move animation
        //       + prim        - Alpha is used by focus blink
        //                     - vertex color alpha is used by FocusScene.FilterColor (UISystem.FocusFilterColor)


        // const
        const float posAnimationTime = 200;
#if !FOCUS_MOVE_MERGE_BLINK
        const float posAnimationAlpha = 0.3f;
#endif
        const float showAlphaAnimationTime = 100;
        const float showPosAnimationTime = 250;
        const float hideAnimationTime = 100;
        const float blinkIntervalTime = 1200;
        const float blinkOutAlpha = 0.3f;
        internal static readonly UIColor defaultFilterColor = new UIColor(0.1f, 0.8f, 1f, 1f);
        internal const BlendMode defaultBlendMode = BlendMode.Premultiplied;


        float posTime;
        float alphaTime;

        Vector4 currentPos;
        Vector4 targetPos;
        Vector4 diffPos;

        float startAlpha;

        ImageAsset[] defaultImages;
        NinePatchMargin[] defaultNinePatchs;
        int[] defaultPaddings;

        ImageAsset nextImage;

        NinePatchMargin currentNinePatch;
        NinePatchMargin nextNinePatch;

        int currentPadding;
        int nextPadding;

        AnimationState state = AnimationState.None;

        UIPrimitive prim;

        public FocusScene()
        {
            prim = new UIPrimitive(DrawMode.TriangleStrip, 16, 28);
            prim.ShaderType = ShaderType.Texture;
            this.RootWidget.RootUIElement.AddChildLast(prim);

            Focusable = false;

            const int styleLength = 5;
            UIDebug.Assert(styleLength == Enum.GetValues(typeof(FocusStyle)).Length);
            defaultImages = new ImageAsset[styleLength];
            defaultNinePatchs = new NinePatchMargin[styleLength];
            defaultPaddings = new int[styleLength];

            defaultImages[(int)FocusStyle.RoundedCorner] =
                new ImageAsset(SystemImageAsset.FocusRoundedCorner);
            defaultNinePatchs[(int)FocusStyle.RoundedCorner] =
                AssetManager.GetNinePatchMargin(SystemImageAsset.FocusRoundedCorner);
            defaultPaddings[(int)FocusStyle.RoundedCorner] = 6;

            defaultImages[(int)FocusStyle.Rectangle] =
                new ImageAsset(SystemImageAsset.FocusRectangle);
            defaultNinePatchs[(int)FocusStyle.Rectangle] =
                AssetManager.GetNinePatchMargin(SystemImageAsset.FocusRectangle);
            defaultPaddings[(int)FocusStyle.Rectangle] = 6;

            defaultImages[(int)FocusStyle.Circle] =
                new ImageAsset(SystemImageAsset.FocusCircle);
            defaultNinePatchs[(int)FocusStyle.Circle] =
                AssetManager.GetNinePatchMargin(SystemImageAsset.FocusCircle);
            defaultPaddings[(int)FocusStyle.Circle] = 10;

            defaultImages[(int)FocusStyle.ListItem] =
                new ImageAsset(SystemImageAsset.FocusListItem);
            defaultNinePatchs[(int)FocusStyle.ListItem] =
                AssetManager.GetNinePatchMargin(SystemImageAsset.FocusListItem);
            defaultPaddings[(int)FocusStyle.ListItem] = 1;

            defaultImages[(int)FocusStyle.None] = null;
            defaultNinePatchs[(int)FocusStyle.None] = new NinePatchMargin();
            defaultPaddings[(int)FocusStyle.None] = 0;


            nextImage = defaultImages[(int)FocusStyle.RoundedCorner];
            currentNinePatch = defaultNinePatchs[(int)FocusStyle.RoundedCorner];
            currentPadding = defaultPaddings[(int)FocusStyle.RoundedCorner];

            prim.Image = nextImage;
            nextNinePatch = currentNinePatch;
            nextPadding = currentPadding;

            this.FilterColor = defaultFilterColor;
            this.BlendMode = defaultBlendMode;
            
            this.RootWidget.Visible = false;
            this.RootWidget.Alpha = 0;
        }


        public void SetPosition(Rectangle rect, bool animation)
        {
            if (animation)
            {
                state |= AnimationState.Position;
                posTime = 0;

                rectToVec4(ref rect, out targetPos);
                diffPos = currentPos - targetPos;
                UIDebug.LogFocus("SetPosition rect={0} diff={1}", rect.ToVector4(), diffPos);
            }
            else
            {
                UIDebug.LogFocus("SetPosition quick rect={0} diff={1}", rect.ToVector4(), diffPos);
                state &= ~AnimationState.Position;

                rectToVec4(ref rect, out currentPos);
                targetPos = currentPos;
                diffPos = new Vector4();
                updatePosition();
            }
        }

        public bool FocusVisible
        {
            get
            {
                return this.RootWidget.Visible;
            }
        }

        public void SetVisible(bool visible, bool animation)
        {
            if (animation)
            {
                // check to need animation
                bool check = false;
                bool appearance = false;
                if ((state & AnimationState.VisibleFadeIn) != 0)
                {
                    check = !visible;
                }
                else if ((state & AnimationState.VisibleFadeOut) != 0)
                {
                    check = visible;
                }
                else
                {
                    if (visible == true && this.RootWidget.Visible == false)
                    {
                        appearance = true;
                    }
                    //else
                        check = (visible != this.RootWidget.Visible);
                }

                if (appearance)
                {
                    //currentPos.Zw = targetPos.Zw * 0.3f;
                    //currentPos.Xy = targetPos.Xy + targetPos.Zw * ((1 - 0.3f) / 2);
                    //diffPos = currentPos - targetPos;
                    //state = AnimationState.Appearance;
                    //alphaTime = 0;
                    //startAlpha = 0;
                    //posTime = 0;
                    blinkTime = -posAnimationTime;
                    updateImage();

                    UIDebug.LogFocus("Appear");
                }
                if (check)
                {
                    state &= ~(AnimationState.VisibleFadeIn | AnimationState.VisibleFadeOut);
                    state |= visible ? AnimationState.VisibleFadeIn : AnimationState.VisibleFadeOut;
                    alphaTime = 0;
                    startAlpha = this.RootWidget.Alpha;

                    UIDebug.LogFocus("SetVisible: " + visible);
                }
            }
            else
            {
                state &= ~(AnimationState.VisibleFadeIn | AnimationState.VisibleFadeOut);

                this.RootWidget.Visible = visible;
                this.RootWidget.Alpha = visible ? 1 : 0;
                updateImage();

                UIDebug.LogFocus("SetVisible quick: " + visible);
            }
        }

        public void SetImage(FocusStyle style, ImageAsset customImage,
            NinePatchMargin customNinePatch, int customPadding, bool animation)
        {
            bool update = false;

            if (customImage == null)
            {
                var image = defaultImages[(int)style];
                var ninepatch = defaultNinePatchs[(int)style];
                var padding = defaultPaddings[(int)style];

                if (nextImage != image)
                {
                    nextImage = image;
                    nextNinePatch = ninepatch;
                    nextPadding = padding;
                    update = true;
                }
            }
            else
            {
                // custom
                if (!ImageAsset.ImageEquals(customImage, nextImage))
                {
                    nextImage = customImage;
                    nextNinePatch = customNinePatch;
                    nextPadding = customPadding;
                    update = true;
                }
            }

            if (update)
            {
                // TODO:
#if true
                updateImage();
#else
                if (animation && this.RootWidget.Visible)
                {
                    state |= AnimationState.ChangingImage;
                }
                else
                {
                    state &= ~AnimationState.ChangingImage;
                    updateImage();
                }
#endif
            }
        }

        private void updatePosition()
        {
            float paddingH = currentPadding;
            float paddingV = currentPadding;
            if (prim.Image != null)
            {
                if (currentNinePatch.Left == 0 && currentNinePatch.Right == 0)
                    paddingH *= currentPos.Z / prim.Image.Width;
                if (currentNinePatch.Top == 0 && currentNinePatch.Bottom == 0)
                    paddingV *= currentPos.W / prim.Image.Height;
            }

            prim.SetPosition(currentPos.X - paddingH, currentPos.Y - paddingV);

            UIPrimitiveUtility.SetupNinePatch(prim,
                currentPos.Z + paddingH * 2,
                currentPos.W + paddingV * 2,
                0, 0,
                currentNinePatch);
        }

        private void updateImage()
        {
            if (prim.Image != nextImage)
            {
                prim.Image = nextImage;
                currentNinePatch = nextNinePatch;
                currentPadding = nextPadding;
                updatePosition();
            }
        }

        private void updateFilterColor(UIColor color)
        {
            for (int i = 0; i < prim.MaxVertexCount; i++)
            {
                prim.GetVertex(i).Color = color;
            }
            // todo: not be updated vertex colors. bug?
            int c = prim.VertexCount; prim.VertexCount = 0; prim.VertexCount = c;
        }
  
        float blinkTime = 0;
        protected override void OnUpdate(float elapsedTime)
        {
            // blink
#if FOCUS_MOVE_MERGE_BLINK
            if (this.RootWidget.Visible && prim.Visible && state == AnimationState.None)
#else
            if (this.RootWidget.Visible && prim.Visible)
#endif
            {
#if false
                double time = UISystem.CurrentTime.TotalMilliseconds;

                var color = filterColor;
                color.R *= (float)(Math.Abs(Math.Sin((time + 0) / 1000) * 0.7) + 0.3);
                color.G *= (float)(Math.Abs(Math.Sin((time + 300) / 1000) * 0.7) + 0.3);
                color.B *= (float)(Math.Abs(Math.Sin((time + 600) / 1000) * 0.7) + 0.3);
                updateFilterColor(color);
#else
                blinkTime += elapsedTime;
                if(blinkTime <= 0)
                {
                    prim.Alpha = 1.0f;
                }
                else if (blinkTime < blinkIntervalTime/2)
                {
                    // fade out
                    prim.Alpha = AnimationUtility.EaseInQuadInterpolator(1.0f, blinkOutAlpha,
                        blinkTime / (blinkIntervalTime / 2));
                }
                else if (blinkTime < blinkIntervalTime)
                {
                    // fade in
                    prim.Alpha = AnimationUtility.EaseOutQuadInterpolator(blinkOutAlpha, 1.0f,
                        blinkTime / (blinkIntervalTime / 2) - 1.0f);
                }
                else
                {
                    blinkTime = 0;
                    prim.Alpha = 1.0f;
                }
#endif
            }

            if (state == AnimationState.None)
                return;

            // position and size animation
            if ((state & AnimationState.Position) != 0)
            {
                posTime += elapsedTime;
                if (posTime < posAnimationTime)
                {
                    float rate = posTime / posAnimationTime;
                    var r = AnimationUtility.EaseOutQuartInterpolator(
                            1, 0, posTime / posAnimationTime);
                    currentPos = targetPos + r * diffPos;
#if FOCUS_MOVE_MERGE_BLINK
                    if (rate < 0.2f)
                        prim.Alpha = Math.Min(1 - (1 - blinkOutAlpha) * rate / 0.2f, prim.Alpha);
                    else
                        prim.Alpha = blinkOutAlpha;
#else
                    if (rate < 0.2f)
                        this.RootWidget.RootUIElement.Alpha =
                            Math.Min(1 - (1 - posAnimationAlpha) * rate / 0.2f,
                                     this.RootWidget.RootUIElement.Alpha);
                    else if (rate < 0.8f)
                        this.RootWidget.RootUIElement.Alpha = posAnimationAlpha;
                    else
                        this.RootWidget.RootUIElement.Alpha = 1 - (1 - posAnimationAlpha) * (1 - rate) / 0.2f;
#endif
                }
                else
                {
                    currentPos = targetPos;
#if FOCUS_MOVE_MERGE_BLINK
                    blinkTime = blinkIntervalTime / 2f;
#else
                    this.RootWidget.RootUIElement.Alpha = 1;
#endif
                    state ^= AnimationState.Position;
                }
                updatePosition();
            }

            // visible (alpha) animation
            if ((state & AnimationState.VisibleFadeIn) != 0)
            {
                alphaTime += elapsedTime;
                this.RootWidget.Visible = true;
                if (alphaTime < showAlphaAnimationTime)
                {
                    this.RootWidget.Alpha = AnimationUtility.EaseOutQuartInterpolator(
                        startAlpha, 1.0f, alphaTime / showAlphaAnimationTime);
                }
                else
                {
                    this.RootWidget.Alpha = 1;
                    state ^= AnimationState.VisibleFadeIn;
                    updateImage();
                }
            }
            else if ((state & AnimationState.VisibleFadeOut) != 0)
            {
                alphaTime += elapsedTime;
                if (alphaTime < hideAnimationTime)
                {
                    this.RootWidget.Alpha = AnimationUtility.EaseOutQuartInterpolator(
                        startAlpha, 0.0f, alphaTime / hideAnimationTime);
                    this.RootWidget.Visible = true;
                }
                else
                {
                    this.RootWidget.Alpha = 0;
                    this.RootWidget.Visible = false;
                    state ^= AnimationState.VisibleFadeOut;
                    updateImage();
                }
            }

            // Appearance is stand alone animation
            if (state == AnimationState.Appearance)
            {
                posTime += elapsedTime;
                alphaTime += elapsedTime;
                
                this.RootWidget.Visible = true;
                if (alphaTime < showAlphaAnimationTime)
                {
                    this.RootWidget.Alpha = AnimationUtility.EaseOutQuartInterpolator(
                        0.0f, 1.0f, alphaTime / showAlphaAnimationTime);
                }
                else
                {
                    this.RootWidget.Alpha = 1;
                }
                if (posTime < showPosAnimationTime)
                {
                    var r = AnimationUtility.OvershootInterpolator(
                            1, 0, posTime / posAnimationTime);
                    currentPos = targetPos + r * diffPos;
                }
                else
                {
                    currentPos = targetPos;
                    this.RootWidget.Alpha = 1;
                    state = AnimationState.None;
                    updateImage();
                }
                updatePosition();
            }
        }

        public void Dispose()
        {
            this.RootWidget.Dispose();
        }

        void setState(AnimationState flag)
        {
            state |= flag;
        }

        void unsetState(AnimationState flag)
        {
            state &= ~flag;
        }

        public UIColor FilterColor
        {
            get { return filterColor; }
            set
            {
                if (filterColor != value)
                {
                    filterColor = value;
                    updateFilterColor(filterColor);
                }
            }
        }
        private UIColor filterColor;

        public BlendMode BlendMode
        {
            get { return this.prim.BlendMode; }
            set { this.prim.BlendMode = value; }
        }

        static private void rectToVec4(ref Rectangle rect, out Vector4 v)
        {
            v.X = rect.X;
            v.Y = rect.Y;
            v.Z = rect.Width;
            v.W = rect.Height;
        }

        [Flags]
        enum AnimationState
        {
            None = 0,
            Position = 1<<0,
            VisibleFadeOut = 1 << 1,
            VisibleFadeIn = 1 << 2,
            Appearance = 1 << 3,
            ChangingImage = 1 << 4,
        }
    }

    /// @if LANG_JA
    /// <summary>フォーカスの表示スタイル</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>The display style of focus</summary>
    /// @endif
    public enum FocusStyle : int
    {
        // int value is used by FocusScene

        /// @if LANG_JA
        /// <summary>角が丸い四角形</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Rectangle with rounded corners</summary>
        /// @endif
        RoundedCorner = 0,
        /// @if LANG_JA
        /// <summary>四角形</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Rectangle</summary>
        /// @endif
        Rectangle,
        /// @if LANG_JA
        /// <summary>円形</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Circle</summary>
        /// @endif
        Circle,
        /// @if LANG_JA
        /// <summary>リストアイテム</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>List item</summary>
        /// @endif
        ListItem,
        /// @if LANG_JA
        /// <summary>表示しない</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Hide</summary>
        /// @endif
        None,
    }

    /// @if LANG_JA
    /// <summary>フォーカスのカスタム設定</summary>
    /// <remarks>各メンバは、デフォルト値(nullまたはゼロ)であれば標準の設定に沿って処理を行います。</remarks>
    /// @endif
    /// @if LANG_EN
    /// <summary>Focus custom setting</summary>
    /// <remarks>If each member is the default value (null or zero), processing is performed following the standard setting.</remarks>
    /// @endif
    public class FocusCustomSettings
    {
        /// @if LANG_JA
        /// <summary>対象ウィジェットで左キーを押した場合のフォーカスの移動先ウィジェットを取得・設定する</summary>
        /// <remarks>値が null もしくは 同一のシーン上に存在しないウィジェットの場合は標準のフォーカス検索が行われます。
        /// 指定したウィジェットはFocusable、Visible、Enabledプロパティおよび表示範囲内にあるかにかかわらずフォーカスが設定されます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the destination widget of the focus when the left key is pressed for the target widget</summary>
        /// <remarks>A standard focus search is performed when the value is null or for widgets that do not exist in the same scene.
        /// The focus is set for the specified widget regardless of the Focusable, Visible, and Enabled properties and whether it is within the display range.</remarks>
        /// @endif
        public Widget LeftCandidate { get; set; }

        /// @if LANG_JA
        /// <summary>対象ウィジェットで右キーを押した場合のフォーカスの移動先ウィジェットを取得・設定する</summary>
        /// <remarks>値が null もしくは 同一のシーン上に存在しないウィジェットの場合は標準のフォーカス検索が行われます。
        /// 指定したウィジェットはFocusable、Visible、Enabledプロパティおよび表示範囲内にあるかにかかわらずフォーカスが設定されます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the destination widget of the focus when the right key is pressed for the target widget</summary>
        /// <remarks>A standard focus search is performed when the value is null or for widgets that do not exist in the same scene.
        /// The focus is set for the specified widget regardless of the Focusable, Visible, and Enabled properties and whether it is within the display range.</remarks>
        /// @endif
        public Widget RightCandidate { get; set; }

        /// @if LANG_JA
        /// <summary>対象ウィジェットで上キーを押した場合のフォーカスの移動先ウィジェットを取得・設定する</summary>
        /// <remarks>値が null もしくは 同一のシーン上に存在しないウィジェットの場合は標準のフォーカス検索が行われます。
        /// 指定したウィジェットはFocusable、Visible、Enabledプロパティおよび表示範囲内にあるかにかかわらずフォーカスが設定されます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the destination widget of the focus when the up key is pressed for the target widget</summary>
        /// <remarks>A standard focus search is performed when the value is null or for widgets that do not exist in the same scene.
        /// The focus is set for the specified widget regardless of the Focusable, Visible, and Enabled properties and whether it is within the display range.</remarks>
        /// @endif
        public Widget UpCandidate { get; set; }

        /// @if LANG_JA
        /// <summary>対象ウィジェットで下キーを押した場合のフォーカスの移動先ウィジェットを取得・設定する</summary>
        /// <remarks>値が null もしくは 同一のシーン上に存在しないウィジェットの場合は標準のフォーカス検索が行われます。
        /// 指定したウィジェットはFocusable、Visible、Enabledプロパティおよび表示範囲内にあるかにかかわらずフォーカスが設定されます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the destination widget of the focus when the down key is pressed for the target widget</summary>
        /// <remarks>A standard focus search is performed when the value is null or for widgets that do not exist in the same scene.
        /// The focus is set for the specified widget regardless of the Focusable, Visible, and Enabled properties and whether it is within the display range.</remarks>
        /// @endif
        public Widget DownCandidate { get; set; }

        /// @if LANG_JA
        /// <summary>フォーカスを検索する場合の矩形を取得・設定する</summary>
        /// <remarks>サイズが0以下の場合は対象ウィジェット自身のサイズを使用します。
        /// 位置とサイズは対象ウィジェット自身のローカル座標で指定します。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets a rectangle when searching for the focus</summary>
        /// <remarks>If the size is equal to or less than 0, the size of the target widget itself is used.
        /// Specifies the position and size using the local coordinates of the target widget itself.</remarks>
        /// @endif
        public Rectangle SearchHintRectangle { get; set; }

        /// @if LANG_JA
        /// <summary>フォーカスイメージを非表示にするかどうか取得・設定する</summary>
        /// <remarks>true の場合は標準またはカスタムのフォーカスイメージを表示しないようにします。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to hide the focus image</summary>
        /// <remarks>If true, the standard or custom focus image is not displayed.</remarks>
        /// @endif
        public bool HideFocusImage { get; set; }

        /// @if LANG_JA
        /// <summary>フォーカスを表示する際の矩形(位置とサイズ)を取得・設定する</summary>
        /// <remarks>サイズが0以下の場合は対象ウィジェット自身のサイズを使用します。
        /// 位置とサイズは対象ウィジェット自身のローカル座標で指定します。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets a rectangle (position and size) when displaying the focus</summary>
        /// <remarks>If the size is equal to or less than 0, the size of the target widget itself is used.
        /// Specifies the position and size using the local coordinates of the target widget itself.</remarks>
        /// @endif
        public Rectangle FocusImageRectangle { get; set; }

        /// @if LANG_JA
        /// <summary>フォーカスのカスタムイメージを取得・設定する</summary>
        /// <remarks>nullの場合はデフォルトのフォーカスイメージを使用します。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the custom image that has focus</summary>
        /// <remarks>When there is null, the default focus image is used.</remarks>
        /// @endif
        public ImageAsset FocusImage { get; set; }

        /// @if LANG_JA
        /// <summary>フォーカスのカスタムイメージの9パッチマージンを取得・設定する</summary>
        /// <remarks>FocusImage が null の場合はこの値は無視されます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the 9-patch margin of the custom image that has focus</summary>
        /// <remarks>This value is ignored when FocusImage is null.</remarks>
        /// @endif
        public NinePatchMargin FocusImageNinePatchMargin { get; set; }

        /// @if LANG_JA
        /// <summary>フォーカスのカスタムイメージを外側に広げる大きさを取得・設定する</summary>
        /// <remarks>FocusImage が null の場合はこの値は無視されます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the size to enlarge the outer side of the custom image that has focus</summary>
        /// <remarks>This value is ignored when FocusImage is null.</remarks>
        /// @endif
        public int FocusImagePadding { get; set; }


        internal Widget getFourwayWidget(FourWayDirection direction)
        {
            switch (direction)
            {
                case FourWayDirection.Left:
                    return this.LeftCandidate;
                case FourWayDirection.Up:
                    return this.UpCandidate;
                case FourWayDirection.Right:
                    return this.RightCandidate;
                case FourWayDirection.Down:
                    return this.DownCandidate;
                default:
                    return null;
            }
        }

        internal bool hasSearchHintRectangle
        {
            get
            {
                return SearchHintRectangle.Width > float.Epsilon
                    && SearchHintRectangle.Height > float.Epsilon;
            }
        }
        internal bool hasFocusImageRectangle
        {
            get
            {
                return FocusImageRectangle.Width > float.Epsilon
                    && FocusImageRectangle.Height > float.Epsilon;
            }
        }
    }

}

