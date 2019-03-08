/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {
        /// @if LANG_JA
        /// <summary>イベントにより回転するコンテナウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>A container widget that rotates depending on an event</summary>
        /// @endif
        public class LiveFlipPanel : Widget
        {

            AnimationState animationState = AnimationState.None;

            /// @if LANG_JA
            /// <summary>回転数を取得・設定する。（半回転＝1）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the number of rotations. (Half rotation = 1)</summary>
            /// @endif
            public int FlipCount
            {
                get { return flipCount; }
                set { flipCount = value; }
            }
            private int flipCount = 16;

            /// @if LANG_JA
            /// <summary>タッチ操作（ドラッグ、フリック）を有効にするかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to enable touch operations (drag, flick).</summary>
            /// @endif
            public bool TouchEnabled
            {
                get { return touchEnabled; }
                set
                {
                    touchEnabled = value;
                    if (touchEnabled)
                    {
                        this.HookChildTouchEvent = true;
                        this.AddGestureDetector(flickGesture);
                    }
                    else
                    {
                        this.HookChildTouchEvent = false;
                        this.RemoveGestureDetector(flickGesture);
                    }
                }
            }
            private bool touchEnabled = true;

            private float bounceTime = 750;
            private float acceleration = -0.0001f;
            float flipTime;
            float firstOmega;
            float secondOmega;
            float angle;
            float startAngle;
            float flipEndAngle;
            float endAngle;
            float totalTime;
            bool isStopAnimate;
            bool isUseSprite = false;

            FlickGestureDetector flickGesture;
            DragGestureDetector dragGesture;

            enum AnimationState
            {
                None,
                Flip,
                FlickFlip,
                Stopping,
                Drag,
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public LiveFlipPanel()
            {
                this.frontPanel = new Panel();
                if (!isUseSprite) {
                    this.frontPanel.PivotType = PivotType.MiddleCenter;
                }
                base.AddChildLast(this.frontPanel);

                this.backPanel = new Panel();
                if (!isUseSprite) {
                    this.backPanel.PivotType = PivotType.MiddleCenter;
                }
                base.AddChildLast(this.backPanel);
                this.backPanel.Visible = false;

                this.frontSprt = new UISprite(1);
                this.frontSprt.ShaderType = ShaderType.Texture;
                this.frontSprt.Culling = true;
                this.frontSprt.Visible = false;
                this.RootUIElement.AddChildLast(this.frontSprt);

                this.backSprt = new UISprite(1);
                this.backSprt.ShaderType = ShaderType.Texture;
                this.backSprt.Culling = true;
                this.backSprt.Visible = false;
                this.RootUIElement.AddChildLast(this.backSprt);

                this.HookChildTouchEvent = true;

                flickGesture = new FlickGestureDetector();
                flickGesture.Direction = FlickDirection.Horizontal;
                flickGesture.FlickDetected += new EventHandler<FlickEventArgs>(FlickEventHandler);
                this.AddGestureDetector(flickGesture);

                dragGesture = new DragGestureDetector();
                dragGesture.Direction = DragDirection.Horizontal;
                dragGesture.DragDetected += new EventHandler<DragEventArgs>(DragEventHandler);
                this.AddGestureDetector(dragGesture);

                this.Width = 100;
                this.Height = 100;

                //this.Focusable = true;
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                removeTextures();

                base.DisposeSelf();
            }

            #region size
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
                    if (this.frontPanel != null && this.backPanel != null &&
                        this.frontSprt != null && this.frontSprt != null)
                    {
                        this.frontPanel.Width = value;
                        this.backPanel.Width = value;
                        if (!isUseSprite)
                        {
                            frontPanel.SetPosition(frontPanel.Width / 2, frontPanel.Height /2);
                            backPanel.SetPosition(frontPanel.Width / 2, frontPanel.Height /2);
                        }
                        updateSprtSize();
                    }
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
                    if (this.frontPanel != null && this.backPanel != null &&
                        this.frontSprt != null && this.frontSprt != null)
                    {
                        this.frontPanel.Height = value;
                        this.backPanel.Height = value;
                        if (!isUseSprite)
                        {
                            frontPanel.SetPosition(frontPanel.Width / 2, frontPanel.Height /2);
                            backPanel.SetPosition(frontPanel.Width / 2, frontPanel.Height /2);
                        }
                        updateSprtSize();
                    }
                }
            }

            void updateSprtSize()
            {
                removeTextures();

                // front sprite
                frontSprt.X = base.Width / 2;
                frontSprt.Y = base.Height / 2;
                var uni = this.frontSprt.GetUnit(0);
                uni.Width = base.Width;
                uni.Height = base.Height;
                uni.X = -frontSprt.X;
                uni.Y = -frontSprt.Y;

                // back sprite
                backSprt.X = frontSprt.X;
                backSprt.Y = frontSprt.Y;
                uni = this.backSprt.GetUnit(0);
                uni.Width = base.Width;
                uni.Height = base.Height;
                uni.X = -backSprt.X;
                uni.Y = -backSprt.Y;
            }

            void removeTextures()
            {
                // dispose ImageAsset
                if (this.frontSprt != null && this.frontSprt.Image != null)
                {
                    this.frontSprt.Image.Dispose();
                    this.frontSprt.Image = null;
                }
                if (this.backSprt != null && this.backSprt.Image != null)
                {
                    this.backSprt.Image.Dispose();
                    this.backSprt.Image = null;
                }

                // dispose Texture2D
                if (frontTexture != null)
                {
                    this.frontTexture.Dispose();
                    this.frontTexture = null;
                }
                if (this.backTexture != null)
                {
                    this.backTexture.Dispose();
                    this.backTexture = null;
                }
            }
            #endregion


            #region other property


            /// @if LANG_JA
            /// <summary>表面のパネルを取得・設定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the front panel.</summary>
            /// @endif
            public Panel FrontPanel
            {
                get { return frontPanel; }
                set
                {
                    if (value != null)
                    {
                        this.RemoveChild(frontPanel);
                        frontPanel = value;
                        frontPanel.Width = base.Width;
                        frontPanel.Height = base.Height;
                        if (!isUseSprite)
                        {
                            frontPanel.PivotType = PivotType.MiddleCenter;
                            frontPanel.SetPosition(frontPanel.Width / 2, frontPanel.Height / 2);
                        }
                        this.AddChildFirst(frontPanel);
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>裏面のパネルを取得・設定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the back panel.</summary>
            /// @endif
            public Panel BackPanel
            {
                get { return backPanel; }
                set
                {
                    if (value != null)
                    {
                        this.RemoveChild(backPanel);
                        backPanel = value;
                        backPanel.Width = base.Width;
                        backPanel.Height = base.Height;
                        backPanel.Visible = false;
                        if (!isUseSprite)
                        {
                            backPanel.PivotType = PivotType.MiddleCenter;
                            backPanel.SetPosition(frontPanel.Width / 2, frontPanel.Height / 2);
                        }
                        this.AddChildLast(backPanel);
                    }
                }
            }

            #endregion


            /// @if LANG_JA
            /// <summary>アニメーションを開始する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Starts the animation.</summary>
            /// @endif
            public void Start()
            {
                Start(this.flipCount, false);
            }

            private void Start(int revo, bool isFlick)
            {
                if (revo == 0) return;

                startAngle = angle;
                endAngle = startAngle + revo * (float)Math.PI;
                endAngle = ((float)(Math.Round(endAngle / (float)Math.PI))) * (float)Math.PI;

                if (revo > 0)
                {
                    flipEndAngle = endAngle - (float)Math.PI / 2f;
                    acceleration = -1.0f * Math.Abs(acceleration);
                    secondOmega = 10.88f / bounceTime;
                    firstOmega = (float)Math.Sqrt(secondOmega * secondOmega + 2 * acceleration * (startAngle - flipEndAngle));
                }
                else
                {
                    flipEndAngle = endAngle + (float)Math.PI / 2f;
                    acceleration = Math.Abs(acceleration);
                    secondOmega = -10.88f / bounceTime;
                    firstOmega = -(float)Math.Sqrt(secondOmega * secondOmega + 2 * acceleration * (startAngle - flipEndAngle));
                }

                totalTime = 0;
                flipTime = (secondOmega - firstOmega) / acceleration;
                isStopAnimate = false;

                if (isUseSprite)
                {
                    setupSprite();
                }

                this.animation = true;
                if (isFlick)
                {
                    this.animationState = AnimationState.FlickFlip;
                }
                else{
                    this.animationState = AnimationState.Flip;
                }
            }

            void setupSprite()
            {
                if (frontTexture == null || backTexture == null)
                {
                    int w, h;

                    w = (int)(this.Width * UISystem.PixelDensity);
                    h = (int)(this.Height * UISystem.PixelDensity);

                    frontTexture = new Texture2D(w, h, false, PixelFormat.Rgba, PixelBufferOption.Renderable);
                    this.frontSprt.Image = new ImageAsset(frontTexture, true);

                    backTexture = new Texture2D(w, h, false, PixelFormat.Rgba, PixelBufferOption.Renderable);
                    this.backSprt.Image = new ImageAsset(backTexture, true);
                }

                this.frontPanel.Visible = true;
                this.backPanel.Visible = true;

                this.frontPanel.RenderToTexture(frontTexture);
                this.backPanel.RenderToTexture(backTexture);

                this.frontPanel.Visible = false;
                this.backPanel.Visible = false;
                this.frontSprt.Visible = true;
                this.frontSprt.BlendMode = BlendMode.Premultiplied;
                this.backSprt.Visible = true;
                this.backSprt.BlendMode = BlendMode.Premultiplied;
            }

            /// @if LANG_JA
            /// <summary>アニメーションを停止する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stops the animation.</summary>
            /// @endif
            public void Stop()
            {
                this.Stop(false);
            }

            /// @if LANG_JA
            /// <summary>アニメーションを停止する。</summary>
            /// <param name="withAnimate">アニメーションするかどうか</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stops the animation.</summary>
            /// <param name="withAnimate">Whether to execute the animation</param>
            /// @endif
            public void Stop(bool withAnimate)
            {
                if (withAnimate)
                {
                    isStopAnimate = true;
                }
                else
                {
                    if (this.animation == true)
                    {
                        UpdateRotateMatrix();
                        if (frontPanel.Visible) {
                            this.angle = 0.0f;
                        }
                        else {
                            this.angle = (float)Math.PI;
                        }

                        this.frontSprt.Visible = false;
                        this.backSprt.Visible = false;
                        this.animation = false;
                        this.animationState = AnimationState.None;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>タッチイベントハンドラ</summary>
            /// <param name="touchEvents">タッチイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Touch event handler</summary>
            /// <param name="touchEvents">Touch event</param>
            /// @endif
            protected internal override void OnTouchEvent (TouchEventCollection touchEvents)
            {
                base.OnTouchEvent(touchEvents);

                TouchEvent touchEvent = touchEvents.PrimaryTouchEvent;

                if (touchEvent.Type == TouchEventType.Down)
                {
                    this.animation = false;
                }
                else if (touchEvent.Type == TouchEventType.Up)
                {
                    if (animationState == AnimationState.Drag)
                    {
                        dragReset();
                    }
                    else if (animationState == AnimationState.Flip)
                    {
                        this.animation = true;
                        Stop(true);
                    }
                    else if (animationState == AnimationState.FlickFlip)
                    {
                        animationState = AnimationState.Flip;
                    }
                }
                
                if (animationState != AnimationState.None)
                {
                    touchEvents.Forward = true;
                }
            }

            private void dragReset()
            {
                totalTime = 0;
                endAngle = FMath.Round(angle / FMath.PI) * FMath.PI;
                flipEndAngle = angle;
                if (isUseSprite)
                {
                    setupSprite();
                }
                isStopAnimate = true;
                this.animation = true;
            }

            /// @if LANG_JA
            /// <summary>状態リセットハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Status reset handler</summary>
            /// @endif
            protected internal override void OnResetState()
            {
                base.OnResetState();

                if (animationState == AnimationState.Drag)
                {
                    dragReset();
                }
                else
                {
                    Stop(true);
                }
            }

            private void FlickEventHandler(object sender, FlickEventArgs e)
            {
                float revoFlick = 0.0f;
                int minRevoFlick = 1;
                int maxRevoFlick = this.flipCount;
                float a = (maxRevoFlick - minRevoFlick) / ((flickGesture.MaxSpeed - flickGesture.MinSpeed) * (flickGesture.MaxSpeed - flickGesture.MinSpeed));
                revoFlick = a * ((float)Math.Abs(e.Speed.X) - flickGesture.MinSpeed) * ((float)Math.Abs(e.Speed.X) - flickGesture.MinSpeed) + minRevoFlick;
                Start(e.Speed.X > 0 ? -(int)revoFlick : (int)revoFlick, true);
            }

            private void DragEventHandler(object sender, DragEventArgs e)
            {
                if (animationState != AnimationState.Drag)
                {
                    if (animationState != AnimationState.Flip)
                    {
                        ResetState(false);
                        if (isUseSprite)
                        {
                            setupSprite();
                        }
                    }
                    animationState = AnimationState.Drag;
                }
                else
                {
                    angle += -e.Distance.X * (float)Math.PI / 180;
                    UpdateRotateMatrix();
                }
            }

            /// @if LANG_JA
            /// <summary>更新処理</summary>
            /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Update processing</summary>
            /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
            /// @endif
            protected override void OnUpdate(float elapsedTime)
            {
                base.OnUpdate(elapsedTime);

                if (!animation) return;

                switch (animationState)
                {
                    case AnimationState.Flip:
                    case AnimationState.FlickFlip:

                        totalTime += elapsedTime;
                        if (isStopAnimate)
                        {
                            if (totalTime < flipTime)
                            {
                                totalTime = flipTime;
                                endAngle = (float)Math.Round(angle / (float)Math.PI) * (float)Math.PI;
                                flipEndAngle = angle;
                            }
                            else if (totalTime < flipTime + bounceTime)
                            {
                                angle = ElasticInterpolator(flipEndAngle, endAngle, (totalTime - flipTime) / bounceTime);
                                UpdateRotateMatrix();
                            }
                            else
                            {
                                Stop();
                                isStopAnimate = false;
                            }
                        }
                        else
                        {
                            if (totalTime < flipTime)
                            {
                                angle = acceleration / 2f * totalTime * totalTime + firstOmega * totalTime + startAngle;
                                UpdateRotateMatrix();
                            }
                            else if (totalTime < flipTime + bounceTime)
                            {
                                angle = ElasticInterpolator(flipEndAngle, endAngle, (totalTime - flipTime) / bounceTime);
                                UpdateRotateMatrix();
                            }
                            else
                            {
                                Stop();
                            }
                        }

                        break;

                    case AnimationState.Drag:
                        if (isStopAnimate)
                        {
                            totalTime += elapsedTime;
                            if (totalTime < bounceTime)
                            {
                                angle = ElasticInterpolator(flipEndAngle, endAngle, totalTime / bounceTime);
                                UpdateRotateMatrix();
                            }
                            else
                            {
                                Stop();
                                isStopAnimate = false;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            static float ElasticInterpolator(float from, float to, float ratio)
            {
                float p = .3f;
                float a = to - from;
                if (ratio == 0) return from;
                if (ratio == 1) return to;
                return to - (float)(a * Math.Pow(2, -10 * ratio) * Math.Cos(ratio * (2 * Math.PI) / p));
            }


            private void UpdateRotateMatrix()
            {
                if (isUseSprite)
                {
                    Matrix4 mat;
                    mat = Matrix4.RotationY(angle);
                    mat.ColumnW = frontSprt.Transform3D.ColumnW;
                    frontSprt.Transform3D = mat;
                    mat = Matrix4.RotationY(angle + (float)Math.PI);
                    mat.ColumnW = backSprt.Transform3D.ColumnW;
                    backSprt.Transform3D = mat;
                }
                else
                {
                    Matrix4 mat;
                    mat = Matrix4.RotationY(angle);
                    mat.ColumnW = frontPanel.Transform3D.ColumnW;
                    frontPanel.Transform3D = mat;
                    mat = Matrix4.RotationY(angle + (float)Math.PI);
                    mat.ColumnW = backPanel.Transform3D.ColumnW;
                    backPanel.Transform3D = mat;

                    // TODO: As-Is
                    Matrix4 world = frontPanel.Transform3D;
                    for (Widget wgt = frontPanel.Parent; wgt != null; wgt = wgt.Parent) {
                        Matrix4 localToParent = wgt.Transform3D;

                        switch (wgt.PivotType) {
                            case PivotType.TopCenter:
                            case PivotType.MiddleCenter:
                            case PivotType.BottomCenter:
                                localToParent.M41 -= wgt.Width / 2.0f;
                                break;
                            case PivotType.TopRight:
                            case PivotType.MiddleRight:
                            case PivotType.BottomRight:
                                localToParent.M41 -= wgt.Width;
                                break;
                            default:
                            break;
                        }
                        switch (wgt.PivotType) {
                            case PivotType.MiddleLeft:
                            case PivotType.MiddleCenter:
                            case PivotType.MiddleRight:
                                localToParent.M42 -= wgt.Height / 2.0f;
                                break;
                            case PivotType.BottomLeft:
                            case PivotType.BottomCenter:
                            case PivotType.BottomRight:
                                localToParent.M42 -= wgt.Height;
                                break;
                            default:
                                break;
                        }

                        world = localToParent * world;
                    }
                    Vector3 cameraTrans = new Vector3(UISystem.FramebufferWidth / 2.0f, UISystem.FramebufferHeight / 2.0f, -1000.0f);
                    Vector3 worldTrans = world.ColumnW.Xyz;
                    Vector3 worldZAxis = world.ColumnZ.Xyz;
                    float dot = worldZAxis.Dot(cameraTrans - worldTrans);

                    if (dot < 0)
                    {
                        frontPanel.Visible = true;
                        backPanel.Visible = false;
                    }
                    else
                    {
                        frontPanel.Visible = false;
                        backPanel.Visible = true;
                    }
                }
            }

            private Panel frontPanel;
            private Panel backPanel;
            private UISprite frontSprt;
            private UISprite backSprt;
            private Texture2D frontTexture;
            private Texture2D backTexture;
            private bool animation = false;
        }


    }
}
