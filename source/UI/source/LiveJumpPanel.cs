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
        /// <summary>子ウィジェットが画面手前にジャンプするコンテナウィジェット。</summary>
        /// <remarks>指定した位置に近いウィジェットから順番にジャンプさせることができます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>A container widget that jumps a child widget to the foreground of the screen.</summary>
        /// <remarks>Making them jump in order from the widget close to a specified position is possible.</remarks>
        /// @endif
        public class LiveJumpPanel : Panel
        {
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public LiveJumpPanel()
            {
                this.jumpWidgetInfoList = new List<JumpWidgetInfo>();
            }

            /// @if LANG_JA
            /// <summary>ジャンプを開始する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Starts the jump.</summary>
            /// @endif
            public void Jump()
            {
                Jump(this.Width / 2.0f, this.Height / 2.0f);
            }

            /// @if LANG_JA
            /// <summary>ジャンプを開始する。</summary>
            /// <param name="x">ローカル座標系でのX座標</param>
            /// <param name="y">ローカル座標系でのY座標</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Starts the jump.</summary>
            /// <param name="x">X coordinate in the local coordinate system</param>
            /// <param name="y">Y coordinate in the local coordinate system</param>
            /// @endif
            public void Jump(float x, float y)
            {
                if (this.jumpWidgetInfoList.Count > 0)
                {
                    return;
                }

                foreach (Widget widget in this.Children)
                {
                    Vector2 v = GetCenterPos(widget) - new Vector2(x, y);

                    JumpWidgetInfo info = new JumpWidgetInfo();
                    info.originalPivot = widget.PivotType;
                    info.Widget = widget;
                    widget.PivotType = PivotType.MiddleCenter;
                    widget.ZSort = true;
                    info.JumpDlayTime = v.Length() * this.jumpDelayTime;
                    info.Axes = (new Vector3(v.Y, -v.X, 0.0f).Normalize());
                    this.jumpWidgetInfoList.Add(info);
                }
                
                this.totalElapsedTime = 0.0f;
                
                if (jumpTime < 1000.0f)
                {
                    this.maxBlurTime = 480.0f;
                    this.blurBoundTime = 170.0f;
                }
                else
                {
                    this.blurBoundDistance += (jumpTime - 1000.0f) / boundDistanceN;
                    this.maxBlurTime = jumpTime / 2.0f;
                    this.blurBoundTime = maxBlurTime / 3.0f;
                }
            }

            private Vector2 GetCenterPos(Widget widget)
            {
                return new Vector2(widget.X, widget.Y);
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

                if (this.jumpWidgetInfoList.Count > 0)
                {

                    List<JumpWidgetInfo> removeList = new List<JumpWidgetInfo>();
                    this.totalElapsedTime += elapsedTime;

                    foreach (JumpWidgetInfo info in this.jumpWidgetInfoList)
                    {
                        float time = this.totalElapsedTime - info.JumpDlayTime;
                        if (time < 0.0f)
                        {
                            continue;
                        }
                        else if (time < this.jumpTime + this.maxBlurTime)
                        {
                            Widget widget = info.Widget;
                            Matrix4 mat = Matrix4.RotationAxis(info.Axes, GetRotationAngle(time));
                            mat.ColumnW = new Vector4(widget.X, widget.Y, GetJumpDistance(time), 1f);
                            widget.Transform3D = mat;
                        }
                        else
                        {
                            removeList.Add(info);
                        }
                    }

                    foreach (JumpWidgetInfo info in removeList)
                    {
                        info.Widget.Transform3D = Matrix4.Translation(new Vector3(info.Widget.X, info.Widget.Y, 0));
                        info.Widget.PivotType = info.originalPivot;
                        info.Widget.ZSort = false;
                        this.jumpWidgetInfoList.Remove(info);
                    }
                }
            }

            private float GetJumpDistance(float elapsedTime)
            {
                if (elapsedTime < this.jumpTime)
                {
                    float time = elapsedTime / (this.jumpTime / 2.0f) - 1.0f;
                    return this.jumpHeight * time * time - this.jumpHeight;
                }
                else
                {
                    float time = elapsedTime - this.jumpTime;
                    return -this.blurBoundDistance / 2.0f * ((1.0f - time / this.maxBlurTime) * (1.0f - (float)Math.Cos(FMath.PI * 2 * time / this.blurBoundTime)));
                }
            }

            private float GetRotationAngle(float elapsedTime)
            {
                if (elapsedTime < this.jumpTime)
                {
                    float t = (elapsedTime / jumpTime) * 2 * (float)Math.PI;
                    return this.tiltAngle * (float)Math.Sin(t);
                }
                else
                {
                    float time = elapsedTime - this.jumpTime;
                    return -this.tiltAngle * ((1.0f - time / this.maxBlurTime) * (float)Math.Sin(time / this.blurBoundTime*Math.PI*2f));
                }
            }

            private class JumpWidgetInfo
            {
                public Widget Widget;
                public float JumpDlayTime;
                public Vector3 Axes;
                public PivotType originalPivot;
            }

            private List<JumpWidgetInfo> jumpWidgetInfoList;
            private float totalElapsedTime;

            private float jumpDelayTime = 2.0f;
            private float jumpTime = 500.0f;
            private float jumpHeight = 150.0f;

            private float maxBlurTime = 480.0f;
            private float blurBoundTime = 170.0f;
            private float blurBoundDistance = 12.0f;
            private float tiltAngle = ((float)Math.PI) / 18.0f;
            
            private float boundDistanceN = 40.0f;

            /// @if LANG_JA
            /// <summary>ジャンプする高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height of the jump.</summary>
            /// @endif
            public float JumpHeight
            {
                get { return jumpHeight; }
                set { jumpHeight = value; }
            }

            /// @if LANG_JA
            /// <summary>それぞれの子ウィジェットがジャンプするまでの遅延時間を取得・設定する。（ミリ秒/ピクセル）</summary>
            /// <remarks>ジャンプ中心から子ウィジェットまでの距離に対して、１ピクセルあたり何ミリ秒遅れるかを指定できる。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the delay time until each child widget jumps. (ms/pixel)</summary>
            /// <remarks>It is possible to specify the delay in ms per pixel for the distance from the jump center to the child widget.</remarks>
            /// @endif
            public float JumpDelayTime
            {
                get { return jumpDelayTime; }
                set { jumpDelayTime = value; }
            }

            /// @if LANG_JA
            /// <summary>各子ウィジェットがジャンプして最初に着地するまでの時間を取得・設定する。（ミリ秒）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the time until each child widget jumps and the first one lands. (ms)</summary>
            /// @endif
            public float JumpTime
            {
                get { return jumpTime; }
                set { jumpTime = value; }
            }

            /// @if LANG_JA
            /// <summary>子ウィジェットの最大傾き角度を取得・設定する。（ラジアン）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the maximum tilting angle of the child widget. (Radian)</summary>
            /// @endif
            public float TiltAngle
            {
                get { return tiltAngle; }
                set { tiltAngle = value; }
            }

        }


    }
}
