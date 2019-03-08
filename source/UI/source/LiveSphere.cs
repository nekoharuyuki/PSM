/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {

        /// @if LANG_JA
        /// <summary>球体型のウィジェット</summary>
        /// <remarks>フリックで回転させることが出来る。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Spherical widget</summary>
        /// <remarks>It is possible to rotate by flicking.</remarks>
        /// @endif
        public class LiveSphere : Widget
        {
            AnimationState animationState = AnimationState.None;
            bool animation = false;
            enum AnimationState
            {
                None,
                Flip,
                FlickFlip,
                Stopping,
                Drag,
            }


            /// @if LANG_JA
            /// <summary>回転数を取得・設定する。（半回転＝１）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the number of rotations. (Half rotation = 1)</summary>
            /// @endif
            public int TurnCount
            {
                get { return turnCount; }
                set { turnCount = value; }
            }
            private int turnCount = 16;

            /// @if LANG_JA
            /// <summary>Z軸回転の角度(ラジアン)を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the angle (radian) of the Z-axis rotation.</summary>
            /// @endif
            public float ZAxis
            {
                get { return zAxis; }
                set
                {
                    zAxis = value;
                    UpdateRotateMatrix();
                }
            }
            private float zAxis = 0.0f;

            /// @if LANG_JA
            /// <summary>y軸回転の角度(ラジアン)を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the angle (radian) of the Y-axis rotation.</summary>
            /// @endif
            public float YAxis
            {
                get { return yAxis; }
                set
                {
                    Stop();
                    yAxis = value;
                    UpdateRotateMatrix();
                }
            }
            private float yAxis = 0.0f;

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
                        this.AddGestureDetector(flickGesture);
                        this.AddGestureDetector(dragGesture);
                    }
                    else
                    {
                        this.RemoveGestureDetector(flickGesture);
                        this.RemoveGestureDetector(dragGesture);
                    }
                }
            }
            private bool touchEnabled = true;

            /// @if LANG_JA
            /// <summary>トグル操作を有効にするかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to enable the toggle operation.</summary>
            /// @endif
            public bool ToggleEnabled
            {
                get { return toggleEnabled; }
                set { toggleEnabled = value; }
            }
            private bool toggleEnabled = false;

            private float bounceTime = 1500;
            private float acceleration = -0.000005f;
            private int flick = 100;
            float flipTime;
            float firstOmega;
            float secondOmega;
            float startAngle;
            float flipEndAngle;
            float endAngle;
            float totalTime;
            bool isStopAnimate;

            FlickGestureDetector flickGesture;
            DragGestureDetector dragGesture;

            int sphereDiv = 16;
            float radius = 50;
            private UIPrimitive spherePrim;

            Vector4 LightDirection;
            float LightZAngle = 3.8f;
            float LightYAngle = 0.44f;
            float Shininess = 100f;
            float Specular = 0.6f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public LiveSphere()
            {
                int zAngleDiv = sphereDiv;
                int yAngleDiv = sphereDiv * 2;

                spherePrim = new UIPrimitive(Sce.PlayStation.Core.Graphics.DrawMode.TriangleStrip, (zAngleDiv+1) * (yAngleDiv + 1), zAngleDiv * 2 * (yAngleDiv + 1));
                spherePrim.Culling = true;
                spherePrim.InternalShaderType = InternalShaderType.LiveSphere;
                spherePrim.ShaderUniforms = new Dictionary<string, float[]>(4);

                float zAngleDelta = (float)Math.PI / zAngleDiv;
                float yAngleDelta = 2 * (float)Math.PI / yAngleDiv;
                float uDelta = 1.0f / yAngleDiv;
                float vDelta = 1.0f / zAngleDiv;

                for (int i = 0; i < zAngleDiv + 1; i++)
                {
                    for (int j = 0; j < yAngleDiv + 1; j++)
                    {
                        float x, y, z;
                        x = -(float)Math.Sin(i * zAngleDelta) * (float)Math.Cos(j * yAngleDelta);
                        y = -(float)Math.Cos(i * zAngleDelta);
                        z = -(float)Math.Sin(i * zAngleDelta) * (float)Math.Sin(j * yAngleDelta);

                        UIPrimitiveVertex v1 = spherePrim.GetVertex(i * (yAngleDiv+1) + j);
                        v1.Position3D = new Vector3(x, y, z);
                        v1.U = uDelta * j;
                        v1.V = vDelta * i;
                    }
                }


                ushort[] indices = new ushort[zAngleDiv * 2 * (yAngleDiv + 1)];
                for (int i = 0; i < zAngleDiv; i++)
                {
                    for (int j = 0; j < yAngleDiv+1; j++)
                    {
                        indices[i * (yAngleDiv + 1) * 2 + j * 2] = (ushort)(i * (yAngleDiv + 1) + j);
                        indices[i * (yAngleDiv + 1) * 2 + j * 2 + 1] = (ushort)((i + 1) * (yAngleDiv + 1) + j);
                    }
                }
                spherePrim.SetIndices(indices);
                this.RootUIElement.AddChildLast(spherePrim);


                LightDirection = new Vector4(
                    (float)(Math.Sin(LightZAngle) * Math.Cos(LightYAngle)),
                    (float)(Math.Cos(LightZAngle)),
                    (float)(Math.Sin(LightZAngle) * Math.Sin(LightYAngle)), 0);

                spherePrim.ShaderUniforms["Shininess"] = new float[] { Shininess };
                spherePrim.ShaderUniforms["Specular"] = new float[] { Specular };

                this.SetSize(radius * 2, radius * 2);

                flickGesture = new FlickGestureDetector();
                flickGesture.Direction = FlickDirection.All;
                flickGesture.FlickDetected += new EventHandler<FlickEventArgs>(FlickEventHandler);
                this.AddGestureDetector(flickGesture);

                dragGesture = new DragGestureDetector();
                dragGesture.DragDetected += new EventHandler<DragEventArgs>(DragEventHandler);
                this.AddGestureDetector(dragGesture);

                this.PriorityHit = true;
                //this.Focusable = true;
            }

            /// @if LANG_JA
            /// <summary>幅を取得・設定する。</summary>
            /// <remarks>球の直径は Width と Height の小さいほうの値になり、中央に表示される。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width.</summary>
            /// <remarks>The sphere diameter is a value of the smaller Width or Height, and is displayed in the center.</remarks>
            /// @endif
            public override float Width
            {
                get
                {
                    return base.Width;
                }
                set
                {
                    if (value > 0)
                    {
                        base.Width = value;
                        updateRadius();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>高さを取得・設定する。</summary>
            /// <remarks>球の直径は Width と Height の小さいほうの値になり、中央に表示される。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height.</summary>
            /// <remarks>The sphere diameter is a value of the smaller Width or Height, and is displayed in the center.</remarks>
            /// @endif
            public override float Height
            {
                get
                {
                    return base.Height;
                }
                set
                {
                    if (value > 0)
                    {
                        base.Height = value;
                        updateRadius();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>サイズを設定する。</summary>
            /// <remarks>球の直径は Width と Height の小さいほうの値になり、中央に表示される。</remarks>
            /// <param name="width">幅</param>
            /// <param name="height">高さ</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets the size.</summary>
            /// <remarks>The sphere diameter is a value of the smaller Width or Height, and is displayed in the center.</remarks>
            /// <param name="width">Width</param>
            /// <param name="height">Height</param>
            /// @endif
            public override void SetSize(float width, float height)
            {
                base.Width = width;
                base.Height = height;
                updateRadius();
            }

            void updateRadius()
            {
                spherePrim.X = base.Width / 2f;
                spherePrim.Y = base.Height / 2f;

                this.radius = spherePrim.X < spherePrim.Y ? spherePrim.X : spherePrim.Y;

                UpdateRotateMatrix();
            }

            /// @if LANG_JA
            /// <summary>画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the image.</summary>
            /// @endif
            public ImageAsset Image
            {
                get { return spherePrim.Image; }
                set { spherePrim.Image = value; }
            }

            /// @if LANG_JA
            /// <summary>回転させる。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Rotates.</summary>
            /// @endif
            public void Start()
            {
                if (toggleEnabled)
                {
                    if (this.FrontFace)
                    {
                        Start(1, false);
                    }
                    else
                    {
                        Start(-1, false);
                    }
                }
                else
                {
                    Start(this.turnCount, false);
                }
            }

            private void Start(int revo, bool isFlick)
            {
                if (revo == 0) return;

                startAngle = yAxis;
                endAngle = startAngle + revo * (float)Math.PI;
                endAngle = ((float)(Math.Round(endAngle / (float)Math.PI))) * (float)Math.PI;

                if (toggleEnabled)
                {
                    acceleration *= 3.0f;
                }

                if (revo > 0)
                {
                    flipEndAngle = endAngle - (float)Math.PI / 2f;
                    acceleration = -0.000005f;
                    secondOmega = 10.88f / bounceTime;
                    firstOmega = (float)Math.Sqrt(secondOmega * secondOmega + 2 * acceleration * (startAngle - flipEndAngle));
                }
                else
                {
                    flipEndAngle = endAngle + (float)Math.PI / 2f;
                    acceleration = 0.000005f;
                    secondOmega = -10.88f / bounceTime;
                    firstOmega = -(float)Math.Sqrt(secondOmega * secondOmega + 2 * acceleration * (startAngle - flipEndAngle));
                }

                totalTime = 0;
                flipTime = (secondOmega - firstOmega) / acceleration;

                isStopAnimate = false;
                this.animation = true;
                if (isFlick)
                {
                    this.animationState = AnimationState.FlickFlip;
                }
                else{
                    this.animationState = AnimationState.Flip;
                }
            }

            /// @if LANG_JA
            /// <summary>回転を止める。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stops the rotation.</summary>
            /// @endif
            public void Stop()
            {
                Stop(false);
            }

            /// @if LANG_JA
            /// <summary>回転を止める</summary>
            /// <param name="withAnimate">停止時にスナップのアニメーションをするかどうかを決定するフラグ</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stops the rotation.</summary>
            /// <param name="withAnimate">Flag determining whether to perform a snap animation when stopped</param>
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
                        if (((int)(Math.Round(yAxis / (float)Math.PI))) % 2 == 0)
                        {
                            this.yAxis = 0.0f;
                        }
                        else
                        {
                            this.yAxis = (float)Math.PI;
                        }

                        this.animation = false;
                        this.animationState = AnimationState.None;
                        UpdateRotateMatrix();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>球の前面が表示されているかどうかを取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains whether the front of the sphere is displayed.</summary>
            /// @endif
            public bool FrontFace
            {
                get { return ((int)(Math.Round(yAxis / (float)Math.PI))) % 2 == 0; }
            }


            /// @if LANG_JA
            /// <summary>タッチイベントハンドラ</summary>
            /// <param name="touchEvents">タッチイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Touch event handler</summary>
            /// <param name="touchEvents">Touch event</param>
            /// @endif
            //protected internal override void OnTouchEvent(TouchEvent touchEvent)
            protected internal override void OnTouchEvent(TouchEventCollection touchEvents)
            {
                base.OnTouchEvent(touchEvents);
                TouchEvent touchEvent = touchEvents.PrimaryTouchEvent;

                if (touchEvent.Type == TouchEventType.Down)
                {
                    this.animation = false;
                    
                    Matrix4 mat = this.spherePrim.Transform3D;
                    mat.M43 = radius * 0.2f;
                    this.spherePrim.Transform3D = mat;
                }
                else if (touchEvent.Type == TouchEventType.Enter)
                {
                    Matrix4 mat = this.spherePrim.Transform3D;
                    mat.M43 = radius * 0.2f;
                    this.spherePrim.Transform3D = mat;
                }
                else if (touchEvent.Type == TouchEventType.Leave)
                {
                    Matrix4 mat = this.spherePrim.Transform3D;
                    mat.M43 = 0;
                    this.spherePrim.Transform3D = mat;
                }
                else if (touchEvent.Type == TouchEventType.Up)
                {
                    Matrix4 mat = this.spherePrim.Transform3D;
                    mat.M43 = 0;
                    this.spherePrim.Transform3D = mat;

                    if (this.animationState == AnimationState.None && !this.animation)
                    {
                        if (this.ButtonAction != null)
                        {
                            this.ButtonAction(this, new TouchEventArgs(touchEvents));
                        }
                    }
                    else if (animationState == AnimationState.Drag)
                    {
                        totalTime = 0;
                        endAngle = (float)Math.Round(yAxis / (float)Math.PI) * (float)Math.PI;
                        flipEndAngle = yAxis;
                        isStopAnimate = true;
                        this.animation = true;
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
                    totalTime = 0;
                    endAngle = (float)Math.Round(yAxis / (float)Math.PI) * (float)Math.PI;
                    flipEndAngle = yAxis;
                    isStopAnimate = true;
                    this.animation = true;
                }
                else
                {
                    Stop(false);
                }
            }

            private void FlickEventHandler(object sender, FlickEventArgs e)
            {
                if (toggleEnabled)
                {
                    if (e.Speed.X > 0 && !this.FrontFace)
                    {
                        Start(-1, true);
                    }
                    else if (e.Speed.X < 0 && this.FrontFace)
                    {
                        Start(1, true);
                    }
                }
                else{
                    int revo = -(int)(e.Speed.X * (float)Math.Cos(zAxis) / flick + e.Speed.Y * (float)Math.Sin(zAxis) / flick);
                    Start(revo, true);
                }

                Matrix4 mat = this.spherePrim.Transform3D;
                mat.M43 = 0;
                this.spherePrim.Transform3D = mat;
            }

            private void DragEventHandler(object sender, DragEventArgs e)
            {
                if (animationState != AnimationState.Drag)
                {
                    animationState = AnimationState.Drag;
                }
                else
                {
                    yAxis += -(e.Distance.X * FMath.Cos(zAxis) + e.Distance.Y * FMath.Sin(zAxis)) * FMath.PI / 180;
                    if (toggleEnabled)
                    {
                        if (yAxis < 0.0f)
                        {
                            yAxis = 0.0f;
                        }
                        else if (yAxis > (float)Math.PI)
                        {
                            yAxis = (float)Math.PI;
                        }
                    }
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

                if (animation)
                {
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
                                    endAngle = (float)Math.Round(yAxis / (float)Math.PI) * (float)Math.PI;
                                    flipEndAngle = yAxis;
                                }
                                else if (totalTime < flipTime + bounceTime)
                                {
                                    yAxis = ElasticInterpolator(flipEndAngle, endAngle, (totalTime - flipTime) / bounceTime);
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
                                    yAxis = acceleration / 2f * totalTime * totalTime + firstOmega * totalTime + startAngle;
                                    UpdateRotateMatrix();
                                }
                                else if (totalTime < flipTime + bounceTime)
                                {
                                    yAxis = ElasticInterpolator(flipEndAngle, endAngle, (totalTime - flipTime) / bounceTime);
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
                                    yAxis = ElasticInterpolator(flipEndAngle, endAngle, totalTime / bounceTime);
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

                updateShaderUniform();
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
                Matrix4 mat = spherePrim.Transform3D;
                Matrix4 mat1 = Matrix4.Scale(new Vector3(radius, radius, radius));
                mat1.M41 = mat.M41;
                mat1.M42 = mat.M42;
                mat1.M43 = mat.M43;
                Matrix4 mat2 = Matrix4.RotationZ(this.zAxis);
                mat1.Multiply(ref mat2, out mat);

                mat1 = mat;
                mat2 = Matrix4.RotationY(this.yAxis);
                mat1.Multiply(ref mat2, out mat);

                spherePrim.Transform3D = mat;
            }

            void updateShaderUniform()
            {
                //var lightDirection = Matrix4.RotationY(-this.angle) * LightDirection;
                //spherePrim.ShaderUniforms["u_LightDirection"] = new float[] { lightDirection.X, lightDirection.Y, lightDirection.Z };

                Matrix4 world = spherePrim.Transform3D;
                for (Widget wgt = this; wgt != null; wgt = wgt.Parent)
                {
                    Matrix4 localToParent = wgt.Transform3D;

                    switch (wgt.PivotType)
                    {
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
                    switch (wgt.PivotType)
                    {
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
                Matrix4 worldInverse = world.Inverse();


                var lightDirection = (worldInverse * LightDirection).Normalize();
                spherePrim.ShaderUniforms["LightDirection"] = new float[] { lightDirection.X, lightDirection.Y, lightDirection.Z };

                Vector4 eyePosition = new Vector4(UISystem.CurrentScreenWidth / 2.0f, UISystem.CurrentScreenHeight / 2.0f, -1000.0f, 1);
                eyePosition -= world.ColumnW;
                eyePosition = worldInverse * eyePosition;
                spherePrim.ShaderUniforms["EyePosition"] = new float[] { eyePosition.X, eyePosition.Y, eyePosition.Z };

                // test
                LightDirection = new Vector4(
                    (float)(Math.Sin(LightZAngle) * Math.Cos(LightYAngle)),
                    (float)(Math.Cos(LightZAngle)),
                    (float)(Math.Sin(LightZAngle) * Math.Sin(LightYAngle)), 0);
                spherePrim.ShaderUniforms["Shininess"] = new float[] { Shininess };
                spherePrim.ShaderUniforms["Specular"] = new float[] { Specular };
            }
            /// @if LANG_JA
            /// <summary>ボタンアクション発火時に呼び出されるハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Handler called when a button action is fired</summary>
            /// @endif
            public event EventHandler<TouchEventArgs> ButtonAction;
        }


    }
}
