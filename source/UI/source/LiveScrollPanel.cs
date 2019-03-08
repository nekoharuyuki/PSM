/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {



        /// @if LANG_JA
        /// <summary>形状が歪みながらスクロールするコンテナウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>A container widget scrolled while the shape is distorted</summary>
        /// @endif
        public class LiveScrollPanel : ContainerWidget
        {
            private enum AnimationState
            {
                None = 0,
                Drag,
                Up,
                Migrate,
            }

            UISprite animationSprt;
            Panel frontPanel;
            AnimationState animationState;
            ImageAsset texImage;
            Texture2D cacheTexture;

            Vector2 touchPos;
            Vector2 touchDownPanelPos;

            Vector2 touchPanelPos
            {
                get { return touchPos - new Vector2(frontPanel.X, frontPanel.Y); }
            }
            Vector2 touchDownPos
            {
                get { return touchDownPanelPos + new Vector2(frontPanel.X, frontPanel.Y); }
                set { touchDownPanelPos = value - new Vector2(frontPanel.X, frontPanel.Y); }
            }
            Vector2 tension
            {
                get { return touchPos - touchDownPos; }
            }

            Vector2 panelVelocity;
            Vector2 touchVelocity;

            float maxTextureWidth;
            float maxTextureHeight;

            bool needPartialTexture;
            float textureOffsetX;
            float textureOffsetY;

            // private const
            float staticFrictionSq = 5000;
            float kineticFriction = 10;
            float viscousFriction = 75;
            float forceCoeff = 0.0002f;
            float flickCoeff = 1.0f;
            float tensionCoeff = 0.5f;
            float panelTouchRatio = 0.1f;
            float touchFrictionRatio = 0.5f;

            float flickElapsedTime = 0f;

            const float textureMarginRatio = 0.25f;
            float defaultMaxTextureWidth = UISystem.GraphicsContext.Caps.MaxTextureSize / UISystem.PixelDensity;
            float defaultMaxtextureHeight = UISystem.GraphicsContext.Caps.MaxTextureSize / UISystem.PixelDensity;

            Vector2 migrateDownPanelPos;
            Vector2 migratePos;
   
            Vector2 migrateDownPos
            {
                get { return migrateDownPanelPos + new Vector2(frontPanel.X, frontPanel.Y); }
                set { migrateDownPanelPos = value - new Vector2(frontPanel.X, frontPanel.Y); }
            }
            
            bool isFlick = false;
            bool isDrag = false;


            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public LiveScrollPanel()
            {
                animationSprt = new UISprite(1);
                animationSprt.InternalShaderType = InternalShaderType.LiveScrollPanel;
                animationSprt.TextureWrapMode = TextureWrapMode.ClampToEdge;
                animationSprt.BlendMode = BlendMode.Premultiplied;
                animationSprt.Visible = false;
                animationSprt.ShaderUniforms = new Dictionary<string, float[]>(3);
                this.RootUIElement.AddChildLast(animationSprt);

                MaxTextureWidth = defaultMaxTextureWidth;
                MaxTextureHeight = defaultMaxtextureHeight;
                needPartialTexture = false;
                textureOffsetX = 0.0f;
                textureOffsetY = 0.0f;

                frontPanel = new Panel();
                base.AddChildLast(frontPanel);
                frontPanel.Clip = true;
                frontPanel.X = 0.0f;
                frontPanel.Y = 0.0f;

                DragGestureDetector drag = new DragGestureDetector();
                drag.DragDetected += new EventHandler<DragEventArgs>(DragEventHandler);
                this.AddGestureDetector(drag);

                FlickGestureDetector flick = new FlickGestureDetector();
                flick.FlickDetected += new EventHandler<FlickEventArgs>(FlickEventHandler);
                this.AddGestureDetector(flick);

                this.Clip = true;
                //this.Focusable = true;
                this.HorizontalScroll = true;
                this.VerticalScroll = true;
                this.animationState = AnimationState.None;
                this.HookChildTouchEvent = true;
                base.Width = 600;
                base.Height = 300;

                UpdateView();

            }

            #region property

            /// @if LANG_JA
            /// <summary>ゆがみの弾性率を取得・設定する。</summary>
            /// <remarks>0より大きい値で、大きくなればゆれが大きくなる。初期値は1。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the distortion elasticity.</summary>
            /// <remarks>The vibration increases as the value above 0 increases. Default value is 1.</remarks>
            /// @endif
            public float Elasticity
            {
                get { return 0.5f / touchFrictionRatio; }
                set
                {
                    if (value <= 0.0f)
                    {
                        throw new ArgumentOutOfRangeException("Elasticity");
                    }
                    touchFrictionRatio = 0.5f / value;
                }
            }


            /// @if LANG_JA
            /// <summary>幅を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width.</summary>
            /// @endif
            public override float Width
            {
                get
                {
                    return base.Width;
                }
                set
                {
                    base.Width = value;
                    UpdateView();
                }
            }

            /// @if LANG_JA
            /// <summary>高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height.</summary>
            /// @endif
            public override float Height
            {
                get
                {
                    return base.Height;
                }
                set
                {
                    base.Height = value;
                    UpdateView();
                }
            }

            /// @if LANG_JA
            /// <summary>親の座標系でのパネルのX座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the X coordinate of the panel in the parent coordinate system.</summary>
            /// @endif
            public float PanelX
            {
                get
                {
                    return frontPanel.X;
                }
                set
                {
                    if (frontPanel != null)
                    {
                        frontPanel.X = FMath.Clamp(value, base.Width - frontPanel.Width, 0.0f);
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>親の座標系でのパネルのY座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the Y coordinate of the panel in the parent coordinate system.</summary>
            /// @endif
            public float PanelY
            {
                get
                {
                    return frontPanel.Y;
                }
                set
                {
                    if (frontPanel != null)
                    {
                        frontPanel.Y = FMath.Clamp(value, base.Height - frontPanel.Height, 0.0f);
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>パネルの幅を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width of the panel.</summary>
            /// @endif
            public float PanelWidth
            {
                get
                {
                    return frontPanel.Width;
                }
                set
                {
                    if (frontPanel != null)
                    {
                        frontPanel.Width = value;
                        updateNeedPartialTexture();
                    }
                    UpdateView();
                }
            }

            /// @if LANG_JA
            /// <summary>パネルの高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height of the panel.</summary>
            /// @endif
            public float PanelHeight
            {
                get
                {
                    return frontPanel.Height;
                }
                set
                {
                    if (frontPanel != null)
                    {
                        frontPanel.Height = value;
                        updateNeedPartialTexture();
                    }
                    UpdateView();
                }
            }

            /// @if LANG_JA
            /// <summary>テクスチャの幅の最大値を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the maximum value of the texture width.</summary>
            /// @endif
            public float MaxTextureWidth
            {
                get
                {
                    return maxTextureWidth;
                }
                set
                {
                    maxTextureWidth = value;
                    if (frontPanel != null)
                    {
                        updateNeedPartialTexture();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>テクスチャの高さの最大値を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the maximum value of the texture height.</summary>
            /// @endif
            public float MaxTextureHeight
            {
                get
                {
                    return maxTextureHeight;
                }
                set
                {
                    maxTextureHeight = value;
                    if (frontPanel != null)
                    {
                        updateNeedPartialTexture();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>パネルの色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the color of the panel.</summary>
            /// @endif
            public UIColor PanelColor
            {
                get
                {
                    return frontPanel.BackgroundColor;
                }
                set
                {
                    if (frontPanel != null)
                    {
                        frontPanel.BackgroundColor = value;
                    }
                }
            }


            /// @if LANG_JA
            /// <summary>水平方向のスクロールをするかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to perform horizontal scrolling.</summary>
            /// @endif
            public bool HorizontalScroll
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>垂直方向のスクロールをするかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to perform vertical scrolling.</summary>
            /// @endif
            public bool VerticalScroll
            {
                get;
                set;
            }

            #endregion

            #region tree operation

            /// @if LANG_JA
            /// <summary>子ウィジェットを取得する。</summary>
            /// <remarks>コレクションを反復処理する列挙子を返す。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the child widget.</summary>
            /// <remarks>Returns the enumerator that repetitively handles the collection.</remarks>
            /// @endif
            public override IEnumerable<Widget> Children
            {
                get
                {
                    return frontPanel.Children;
                }
            }

            /// @if LANG_JA
            /// <summary>子ウィジェットを先頭に追加する。</summary>
            /// <param name="child">追加する子ウィジェット</param>
            /// <remarks>既に追加されている場合は先頭に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds the child widget to the beginning.</summary>
            /// <param name="child">Child widget to be added</param>
            /// <remarks>Moves it to the beginning if it has already been added.</remarks>
            /// @endif
            public override void AddChildFirst(Widget child)
            {
                frontPanel.AddChildFirst(child);
            }

            /// @if LANG_JA
            /// <summary>子ウィジェットを末尾に追加する。</summary>
            /// <param name="child">追加する子ウィジェット</param>
            /// <remarks>既に追加されている場合は末尾に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds the child widget to the end.</summary>
            /// <param name="child">Child widget to be added</param>
            /// <remarks>Moves it to the end if it has already been added.</remarks>
            /// @endif
            public override void AddChildLast(Widget child)
            {
                frontPanel.AddChildLast(child);
            }

            /// @if LANG_JA
            /// <summary>指定した子ウィジェットの直前に挿入する。</summary>
            /// <param name="child">挿入する子ウィジェット</param>
            /// <param name="nextChild">挿入する子ウィジェットの直後となる子ウィジェット</param>
            /// <remarks>既に追加されている場合は指定した子ウィジェットの直前に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts it immediately in front of the specified child widget.</summary>
            /// <param name="child">Child widget to be inserted</param>
            /// <param name="nextChild">Child widget immediately after the inserted child widget</param>
            /// <remarks>Moves it immediately in front of the specified child widget if it has already been added.</remarks>
            /// @endif
            public override void InsertChildBefore(Widget child, Widget nextChild)
            {
                frontPanel.InsertChildBefore(child, nextChild);
            }

            /// @if LANG_JA
            /// <summary>指定した子ウィジェットの直後に挿入する。</summary>
            /// <param name="child">挿入する子ウィジェット</param>
            /// <param name="prevChild">挿入する子ウィジェットの直前となる子ウィジェット</param>
            /// <remarks>既に追加されている場合は指定した子ウィジェットの直後に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts it immediately after the specified child widget.</summary>
            /// <param name="child">Child widget to be inserted</param>
            /// <param name="prevChild">Child widget immediately in front of the inserted child widget</param>
            /// <remarks>Moves it immediately after the specified child widget if it has already been added.</remarks>
            /// @endif
            public override void InsertChildAfter(Widget child, Widget prevChild)
            {
                frontPanel.InsertChildAfter(child, prevChild);
            }

            /// @if LANG_JA
            /// <summary>指定された子ウィジェットを削除する。</summary>
            /// <param name="child">削除する子ウィジェット</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes the specified child widget.</summary>
            /// <param name="child">Child widget to be deleted</param>
            /// @endif
            public override void RemoveChild(Widget child)
            {
                frontPanel.RemoveChild(child);
            }

            #endregion

            #region misc

            private void UpdateView()
            {
                if (animationSprt == null || frontPanel == null)
                {
                    return;
                }

                animationSprt.GetUnit(0).Width = base.Width;
                animationSprt.GetUnit(0).Height = base.Height;
            }

            /// @if LANG_JA
            /// <summary>状態をリセットする。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Resets the state.</summary>
            /// @endif
            protected internal override void OnResetState()
            {
                animationState = AnimationState.None;

                unsetTexture();

                touchPos = new Vector2();
                panelVelocity = new Vector2();
                touchVelocity = new Vector2();
                flickElapsedTime = 0;
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                if (animationSprt.Image != null)
                {
                    animationSprt.Image.Dispose();
                }

                if (cacheTexture != null)
                {
                    cacheTexture.Dispose();
                }

                base.DisposeSelf();
            }

            #endregion

            /// @if LANG_JA
            /// <summary>更新処理</summary>
            /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
            /// <remarks>elapsedTimeが10秒以上の場合、更新処理はスキップされます。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Update processing</summary>
            /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
            /// <remarks>If elapsedTime is 10 seconds or more, the update processing is skipped.</remarks>
            /// @endif
            protected override void OnUpdate(float elapsedTime)
            {
                base.OnUpdate(elapsedTime);

                if (elapsedTime > 10000f)
                {
                    return;
                }

                while (elapsedTime > 35f)
                {
                    internalUpdate(35f);
                    elapsedTime -= 35f;
                }
                internalUpdate(elapsedTime);


                updateSprt();

                if (touchVelocity.LengthSquared() + panelVelocity.LengthSquared() > 100f || flickElapsedTime > 10000)
                {
                    OnResetState();
                }
            }
            void internalUpdate(float elapsedTime)
            {
                if (animationState == AnimationState.Drag || animationState == AnimationState.Migrate)
                {
                    float newOffsetX;
                    float newOffsetY;

                    if (needPartialTexture == true)
                    {
                        newOffsetX = CalculateTextureOffsetX();
                        newOffsetY = CalculateTextureOffsetY();
                    }
                    else
                    {
                        newOffsetX = 0.0f;
                        newOffsetY = 0.0f;
                    }

                    if (texImage == null)
                    {
                        textureOffsetX = newOffsetX;
                        textureOffsetY = newOffsetY;

                        setTexture();
                    }
                    else
                    {
                        if (textureOffsetX != newOffsetX ||
                            textureOffsetY != newOffsetY)
                        {
                            textureOffsetX = newOffsetX;
                            textureOffsetY = newOffsetY;

                            unsetTexture();
                            setTexture();
                        }
                    }

                    if (animationState == AnimationState.Migrate)
                    {
                        var done = true;
                        var diffPos = migratePos - touchPos;
                        if (diffPos.LengthSquared() > 4)
                        {
                            touchPos += diffPos * (elapsedTime / 100f) + diffPos.Normalize() * 2;
                            done = false;
                        }
                        else
                        {
                            touchPos = migratePos;
                        }

                        var diffDownPos = migrateDownPos - touchDownPos;
                        if (diffDownPos.LengthSquared() > 4)
                        {
                            touchDownPos += diffDownPos * (elapsedTime / 100f) + diffDownPos.Normalize() * 2;
                            done = false;
                        }
                        else
                        {
                            touchDownPos = migrateDownPos;
                        }
                        if (done)
                        {
                            animationState = AnimationState.Drag;
                        }
                    }

                    if (panelVelocity.LengthSquared() > 0.000000001f)
                    {
                        Vector2 friction = -panelVelocity.Normalize() * kineticFriction - panelVelocity * viscousFriction;
                        panelVelocity += (tension + friction) * elapsedTime * forceCoeff;
                    }
                    else if (tension.LengthSquared() > (isFlick ? staticFrictionSq / 100 : staticFrictionSq))
                    {
                        panelVelocity += tension * elapsedTime * forceCoeff;
                    }


                    if (this.HorizontalScroll == true)
                    {
                        frontPanel.X += panelVelocity.X * elapsedTime;
                    }
                    if (this.VerticalScroll == true)
                    {
                        frontPanel.Y += panelVelocity.Y * elapsedTime;
                    }

                    adjustPanelEdge();
                }
                else if (animationState == AnimationState.Up)
                {
                    if (needPartialTexture == true)
                    {
                        float newOffsetX;
                        float newOffsetY;

                        newOffsetX = CalculateTextureOffsetX();
                        newOffsetY = CalculateTextureOffsetY();

                        if (textureOffsetX != newOffsetX ||
                            textureOffsetY != newOffsetY)
                        {
                            textureOffsetX = newOffsetX;
                            textureOffsetY = newOffsetY;

                            unsetTexture();
                            setTexture();
                        }
                    }

                    if (texImage == null || tension.LengthSquared() < 4 && touchVelocity.LengthSquared() < 0.01f && panelVelocity.LengthSquared() < 0.01f)
                    {
                        OnResetState();
                    }
                    else
                    {
                        if (panelVelocity.LengthSquared() > 0.0001f)
                        {
                            Vector2 friction = -panelVelocity.Normalize() * kineticFriction - panelVelocity * viscousFriction;
                            panelVelocity += (tension + friction) * elapsedTime * forceCoeff * panelTouchRatio;
                        }
                        else if (tension.LengthSquared() > staticFrictionSq || isFlick)
                        {
                            panelVelocity += tension * elapsedTime * forceCoeff * panelTouchRatio;
                        }

                        if (!isFlick)
                        {
                            if (frontPanel.X > -10f && panelVelocity.X < 0f)
                            {
                                panelVelocity.X = (frontPanel.X + 10f) * elapsedTime * forceCoeff;
                            }
                            else if (frontPanel.X < this.Width - frontPanel.Width + 10f && panelVelocity.X > 0f)
                            {
                                panelVelocity.X = (frontPanel.X - (this.Width - frontPanel.Width + 10f)) * elapsedTime * forceCoeff;
                            }

                            if (frontPanel.Y > -10f && panelVelocity.Y < 0f)
                            {
                                panelVelocity.Y = (frontPanel.Y + 10f) * elapsedTime * forceCoeff;
                            }
                            else if (frontPanel.Y < this.Height - frontPanel.Height + 10f && panelVelocity.Y > 0f)
                            {
                                panelVelocity.Y = (frontPanel.Y - (this.Height - frontPanel.Height + 10f)) * elapsedTime * forceCoeff;
                            }
                        }
                        isFlick = false;
                        
                        Vector2 friction2 = new Vector2();

                        if (touchVelocity.LengthSquared() > 0.0001f)
                        {
                            friction2 = -touchVelocity.Normalize() * kineticFriction - touchVelocity * viscousFriction;
                            friction2 *= touchFrictionRatio;
                        }

                        if (touchVelocity.LengthSquared() < 0.01f && tension.LengthSquared() < 50)
                        {
                            touchVelocity += (friction2 - tension * (1.0f + 100.0f / tension.LengthSquared())) * elapsedTime * forceCoeff * (1 - panelTouchRatio);
                            if (tension.LengthSquared() < float.Epsilon || touchVelocity.Dot(tension) / tension.LengthSquared() * elapsedTime < -1.0f)
                            {
                                touchVelocity = -tension / elapsedTime;
                            }
                        }
                        else
                        {
                            touchVelocity += (friction2 - tension) * elapsedTime * forceCoeff * (1 - panelTouchRatio);
                        }

                        if (tension.LengthSquared() > float.Epsilon)
                        {
                            touchVelocity = tension * touchVelocity.Dot(tension) / tension.LengthSquared();
                        }

                        if (this.HorizontalScroll == true)
                        {
                            frontPanel.X += panelVelocity.X * elapsedTime;
                        }
                        if (this.VerticalScroll == true)
                        {
                            frontPanel.Y += panelVelocity.Y * elapsedTime;
                        }

                        touchPos += touchVelocity * elapsedTime;

                        adjustPanelEdge();
                    }
                }
            }

            void adjustPanelEdge()
            {
                if (frontPanel.X > 0)
                {
                    frontPanel.X = 0;
                    if (panelVelocity.X > 0)
                    {
                        panelVelocity.X = 0;
                    }
                }
                if (frontPanel.X < this.Width - frontPanel.Width)
                {
                    frontPanel.X = this.Width - frontPanel.Width;
                    if (panelVelocity.X < 0)
                    {
                        panelVelocity.X = 0;
                    }
                }
                if (frontPanel.Y > 0)
                {
                    frontPanel.Y = 0;
                    if (panelVelocity.Y > 0)
                    {
                        panelVelocity.Y = 0;
                    }
                }
                if (frontPanel.Y < this.Height - frontPanel.Height)
                {
                    frontPanel.Y = this.Height - frontPanel.Height;
                    if (panelVelocity.Y < 0)
                    {
                        panelVelocity.Y = 0;
                    }
                }
            }

            #region sprite and texture

            void setTexture()
            {
                float textureWidth;
                float textureHeight;

                if (needPartialTexture == true)
                {
                    textureWidth = FMath.Min(frontPanel.Width, MaxTextureWidth) * UISystem.PixelDensity;
                    textureHeight = FMath.Min(frontPanel.Height, MaxTextureHeight) * UISystem.PixelDensity;
                }
                else
                {
                    textureWidth = frontPanel.Width * UISystem.PixelDensity;
                    textureHeight = frontPanel.Height * UISystem.PixelDensity;
                }

                if (cacheTexture == null || cacheTexture.Width != (int)textureWidth || cacheTexture.Height != (int)textureHeight)
                {
                    if (cacheTexture != null)
                    {
                        cacheTexture.Dispose();
                    }

                    cacheTexture
                        = new Texture2D(
                            (int)textureWidth,
                            (int)textureHeight,
                             false, PixelFormat.Rgba, PixelBufferOption.Renderable);
                }

                Matrix4 mat = new Matrix4(1f, 0f, 0f, 0f,
                                          0f, 1f, 0f, 0f,
                                          0f, 0f, 1f, 0f,
                                          -textureOffsetX, -textureOffsetY, 0f, 1f);
                
                var fb = new FrameBuffer();
                fb.SetColorTarget(cacheTexture, 0);
                frontPanel.RenderToFrameBuffer(fb, ref mat, true);
                fb.Dispose();

                if (animationSprt.Image != null)
                    animationSprt.Image.Dispose();

                animationSprt.Image = new ImageAsset(cacheTexture, true);
                this.texImage = animationSprt.Image;

                animationSprt.Visible = true;
                frontPanel.Visible = false;
            }

            void unsetTexture()
            {
                animationSprt.Visible = false;
                frontPanel.Visible = true;
                texImage = null;
                animationSprt.ShaderUniforms["u_touchCoord"] = new float[] { 0f, 0f };
                animationSprt.ShaderUniforms["u_tension"] = new float[] { 0f, 0f };
            }

            void updateSprt()
            {
                if (this.texImage != null)
                {
                    var uni = animationSprt.GetUnit(0);
                    uni.Width = this.Width;
                    uni.Height = this.Height;
                    uni.U1 = (-frontPanel.X - textureOffsetX) / this.texImage.Width;
                    uni.V1 = (-frontPanel.Y - textureOffsetY) / this.texImage.Height;
                    uni.U2 = (-frontPanel.X - textureOffsetX + this.Width) / this.texImage.Width;
                    uni.V2 = (-frontPanel.Y - textureOffsetY + this.Height) / this.texImage.Height;

                    animationSprt.ShaderUniforms["u_touchCoord"] = new float[] {
                        (touchPos.X - frontPanel.X - textureOffsetX) / this.texImage.Width,
                        (touchPos.Y - frontPanel.Y - textureOffsetY) / this.texImage.Height
                    };

                    animationSprt.ShaderUniforms["u_tension"] = new float[] {
                        tension.X / this.texImage.Width * tensionCoeff,
                        tension.Y / this.texImage.Height * tensionCoeff
                    };

                    animationSprt.ShaderUniforms["u_clipRect"] = new float[] {
                        this.Width / this.texImage.Width,
                        this.Height / this.texImage.Height
                    };
                }
            }

            #endregion

            #region Touch Event

            /// @if LANG_JA
            /// <summary>タッチイベントハンドラ</summary>
            /// <param name="touchEvents">タッチイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Touch event handler</summary>
            /// <param name="touchEvents">Touch event</param>
            /// @endif
            protected internal override void OnTouchEvent(TouchEventCollection touchEvents)
            {
                base.OnTouchEvent(touchEvents);

                TouchEvent touchEvent = touchEvents.PrimaryTouchEvent;

                if (touchEvent.Type == TouchEventType.Down)
                {
                    isFlick = false;
                    if (animationState == AnimationState.Up)
                    {
                        animationState = AnimationState.Migrate;
                        migrateDownPos = touchEvent.LocalPosition;
                        migratePos = migrateDownPos;
                        //migratetension = tension;
                    }
                    else
                    {
                        touchDownPos = touchEvent.LocalPosition;
                        touchPos = touchDownPos;
                        touchEvents.Forward = true;
                    }
                }
                else if (touchEvent.Type == TouchEventType.Up)
                {
                    isDrag = false;
                }

                if (!isDrag)//this.animationState == AnimationState.Drag)
                {
                    touchEvents.Forward = true;
                }

                if (touchEvent.Type == TouchEventType.Up)
                {
                    animationState = AnimationState.Up;
                    flickElapsedTime = 0f;
                }
            }

            private void DragEventHandler(object sender, DragEventArgs e)
            {
                ResetState(false);

                isDrag = true;

                if (animationState == AnimationState.None)
                {
                    animationState = AnimationState.Migrate;
                    migrateDownPos = e.LocalPosition;
                    migratePos = migrateDownPos;
                }
                else if (animationState == AnimationState.Migrate)
                {
                    migratePos = e.LocalPosition;
                }
                else
                {
                    animationState = AnimationState.Drag;
                    touchPos = e.LocalPosition;
                }
            }

            private void FlickEventHandler(object sender, FlickEventArgs e)
            {
                ResetState(false);
                this.animationState = AnimationState.Up;
                isFlick = true;

                touchVelocity = e.Speed / 1000f * flickCoeff;
                panelVelocity += e.Speed / 1000f * flickCoeff;
            }

            private void updateNeedPartialTexture()
            {
                if (frontPanel.Width > MaxTextureWidth ||
                    frontPanel.Height > MaxTextureHeight)
                {
                    needPartialTexture = true;
                }
                else
                {
                    needPartialTexture = false;
                }
            }

            private float CalculateTextureOffsetX()
            {
                float newOffsetX;
                float spriteX;
                float marginTextureX;

                spriteX = -frontPanel.X;
                marginTextureX = this.Width * textureMarginRatio;

                newOffsetX = CalculateTextureOffset(spriteX, this.Width,
                    textureOffsetX, marginTextureX, MaxTextureWidth,
                    frontPanel.Width);

                return newOffsetX;
            }

            private float CalculateTextureOffsetY()
            {
                float newOffsetY;
                float spriteY;
                float marginTextureY;

                spriteY = -frontPanel.Y;
                marginTextureY = this.Height * textureMarginRatio;

                newOffsetY = CalculateTextureOffset(spriteY, this.Height,
                    textureOffsetY, marginTextureY, MaxTextureHeight,
                    frontPanel.Height);

                return newOffsetY;
            }

            static private float CalculateTextureOffset(float spritePosition, float spriteSize,
                float offsetPosition, float textureMarginSize, float maxTextureSize,
                float frontPanelSize)
            {
                float newOffset;

                if (spritePosition < maxTextureSize - textureMarginSize - spriteSize)
                {
                    newOffset = 0.0f;
                }
                else if (spritePosition > frontPanelSize - spriteSize - textureMarginSize)
                {
                    newOffset = FMath.Max(frontPanelSize - maxTextureSize, 0.0f);
                }
                else
                {
                    if (spritePosition > offsetPosition + textureMarginSize &&
                        spritePosition < offsetPosition + maxTextureSize - textureMarginSize - spriteSize)
                    {
                        newOffset = offsetPosition;
                    }
                    else
                    {
                        newOffset = spritePosition + spriteSize / 2 - maxTextureSize / 2;
                    }
                }

                return newOffset;
            }

            #endregion
        }


    }
}
