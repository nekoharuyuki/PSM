/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {
        /// @if LANG_JA
        /// <summary>JumpFlipEffectの回転軸</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>JumpFlipEffect rotation axis</summary>
        /// @endif
        public enum JumpFlipEffectAxis
        {
            /// @if LANG_JA
            /// <summary>X軸</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>X-axis</summary>
            /// @endif
            X = 0,

            /// @if LANG_JA
            /// <summary>Y軸</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Y-axis</summary>
            /// @endif
            Y
        }

        /// @if LANG_JA
        /// <summary>画面手前にジャンプし反転して切り替わるエフェクト</summary>
        /// <remarks>表面が古いウィジェットで裏面が新しいウィジェットとなる。それぞれ同じサイズとする。ジャンプして反転しながら新しい位置に落ちていくことも可能。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Effect that jumps forward, flips, and then changes</summary>
        /// <remarks>The front is the old widget and the back is the new widget. Each has the same size. It can also fall to a new position while jumping and flipping.</remarks>
        /// @endif
        public class JumpFlipEffect : Effect
        {
            private float time = 500.0f;

            private int revolution = 1;
            private JumpFlipEffectAxis rotationAxis = JumpFlipEffectAxis.Y;

            const float fromPosZ = 0.0f;
            private float toPosZ = -350.0f;

            const float fromDegree = 0.0f;

            private float jumpDelay = 0.15f;
            private float toDegreeDelay = 0.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public JumpFlipEffect()
            {
                Widget = null;
                NextWidget = null;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="currentWidget">表示中のウィジェット</param>
            /// <param name="nextWidget">次に表示するウィジェット</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="currentWidget">Widget being displayed</param>
            /// <param name="nextWidget">Next widget to be displayed</param>
            /// @endif
            public JumpFlipEffect(Widget currentWidget, Widget nextWidget)
            {
                Widget = currentWidget;
                NextWidget = nextWidget;
            }

            /// @if LANG_JA
            /// <summary>インスタンスを作成しエフェクトを開始する。</summary>
            /// <param name="currentWidget">表示中のウィジェット</param>
            /// <param name="nextWidget">次に表示するウィジェット</param>
            /// <returns>エフェクトのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Creates an instance and starts the effect.</summary>
            /// <param name="currentWidget">Widget being displayed</param>
            /// <param name="nextWidget">Next widget to be displayed</param>
            /// <returns>Effect instance</returns>
            /// @endif
            public static JumpFlipEffect CreateAndStart(Widget currentWidget, Widget nextWidget)
            {
                JumpFlipEffect effect = new JumpFlipEffect(currentWidget, nextWidget);
                effect.Start();
                return effect;
            }

            /// @if LANG_JA
            /// <summary>開始処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Start processing</summary>
            /// @endif
            protected override void OnStart()
            {
                if ((Widget.Width != NextWidget.Width) || (Widget.Height != NextWidget.Height))
                {
                    System.Console.WriteLine("Not supported different widget size.");
                    return;
                }

                nonJumpTime = time * jumpDelay;

                orgCurrentPivotType = Widget.PivotType;
                orgNextPivotType = NextWidget.PivotType;
                Widget.PivotType = PivotType.MiddleCenter;
                NextWidget.PivotType = PivotType.MiddleCenter;

                Widget.ZSort = true;
                NextWidget.ZSort = true;

                fromPosX = Widget.X;
                fromPosY = Widget.Y;
                toPosX = NextWidget.X;
                toPosY = NextWidget.Y;
                toDegree = 180.0f;

                Widget.Visible = true;
                NextWidget.Visible = false;
            }

            /// @if LANG_JA
            /// <summary>更新処理</summary>
            /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
            /// <returns>エフェクトの更新の応答</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Update processing</summary>
            /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
            /// <returns>Response of effect update</returns>
            /// @endif
            protected override EffectUpdateResponse OnUpdate(float elapsedTime)
            {
                if ((Widget.Width != NextWidget.Width) || (Widget.Height != NextWidget.Height))
                {
                    return EffectUpdateResponse.Finish;
                }

                if (TotalElapsedTime >= time)
                {
                    float transX = toPosX;
                    float transY = toPosY;
                    float transZ = fromPosZ;
                    float degree = toDegree;

                    Widget.Visible = false;
                    NextWidget.Visible = true;

                    Widget.Transform3D = GetTransform3D(transX, transY, transZ, degree);
                    NextWidget.Transform3D = GetTransform3D(transX, transY, transZ, degree + 180.0f);

                    UpdateWidgetVisible();

                    return EffectUpdateResponse.Finish;
                }
                else
                {
                    float timeTemp = TotalElapsedTime / time;
                    float transX = (toPosX - fromPosX) * timeTemp * timeTemp + fromPosX;
                    float transY = (toPosY - fromPosY) * timeTemp * timeTemp + fromPosY;
                    float timeTemp2 = TotalElapsedTime / (time / 2.0f) - 1.0f;
                    float transZ = -(toPosZ - fromPosZ) * timeTemp2 * timeTemp2 - (-(toPosZ - fromPosZ));

                    float degree;
                    if (TotalElapsedTime < nonJumpTime)
                    {
                        degree = FMath.Lerp(fromDegree, toDegreeDelay, TotalElapsedTime / nonJumpTime);
                    }
                    else if (TotalElapsedTime >= nonJumpTime && TotalElapsedTime <= time - nonJumpTime)
                    {
                        degree = FMath.Lerp(toDegreeDelay, toDegree - toDegreeDelay, (TotalElapsedTime - nonJumpTime) / (time - nonJumpTime * 2.0f));
                    }
                    else
                    {
                        degree = FMath.Lerp(toDegree - toDegreeDelay, toDegree, (TotalElapsedTime - (time - nonJumpTime)) / nonJumpTime);
                    }

                    transZ = (transZ <= toPosZ) ? (toPosZ * 2.0f - transZ) : transZ;

                    Widget.Transform3D = GetTransform3D(transX, transY, transZ, degree);
                    NextWidget.Transform3D = GetTransform3D(transX, transY, transZ, degree + 180.0f);

                    UpdateWidgetVisible();

                    return EffectUpdateResponse.Continue;
                }
            }

            private Matrix4 GetTransform3D(float transX, float transY, float transZ, float degree)
            {
                Matrix4 mat;
                Matrix4 mat1 = Matrix4.Translation(new Vector3(transX, transY, transZ));
                Matrix4 mat2;
                if (rotationAxis == JumpFlipEffectAxis.X)
                {
                    mat2 = Matrix4.RotationX(degree / 360.0f * 2.0f * (float)Math.PI * revolution);
                }
                else
                {
                    mat2 = Matrix4.RotationY(degree / 360.0f * 2.0f * (float)Math.PI * revolution);
                }
                mat1.Multiply(ref mat2, out mat);

                return mat;
            }

            private void UpdateWidgetVisible()
            {
                // TODO: As-Is
                Matrix4 world = Widget.Transform3D;
                for (Widget wgt = Widget.Parent; wgt != null; wgt = wgt.Parent) {
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
                Vector3 cameraTrans = new Vector3(UISystem.CurrentScreenWidth / 2.0f, UISystem.CurrentScreenHeight / 2.0f, -1000.0f);
                Vector3 worldTrans = world.ColumnW.Xyz;
                Vector3 worldZAxis = world.ColumnZ.Xyz;
                float dot = worldZAxis.Dot(cameraTrans - worldTrans);

                if (dot < 0)
                {
                    Widget.Visible = true;
                    NextWidget.Visible = false;
                }
                else
                {
                    Widget.Visible = false;
                    NextWidget.Visible = true;
                }
            }

            /// @if LANG_JA
            /// <summary>停止処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stop processing</summary>
            /// @endif
            protected override void OnStop()
            {
                if ((Widget.Width != NextWidget.Width) || (Widget.Height != NextWidget.Height))
                {
                    return;
                }

                Widget.ZSort = false;
                Widget.PivotType = orgCurrentPivotType;
                NextWidget.ZSort = false;
                NextWidget.PivotType = orgNextPivotType;
            }

            /// @if LANG_JA
            /// <summary>次に表示するウィジェットを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the next widget to be displayed.</summary>
            /// @endif
            public Widget NextWidget
            {
                get;
                set;
            }
   
            /// @if LANG_JA
            /// <summary>回転数を取得・設定する。（半回転＝1）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the number of rotations. (Half rotation = 1)</summary>
            /// @endif
            public int Revolution
            {
                get { return revolution; }
                set { revolution = value; }
            }

            /// @if LANG_JA
            /// <summary>ジャンプの高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height of the jump.</summary>
            /// @endif
            public float JumpHeight
            {
                get { return toPosZ; }
                set { toPosZ = value; }
            }

            /// @if LANG_JA
            /// <summary>エフェクトの時間を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the effect time.</summary>
            /// @endif
            public float Time
            {
                get { return time; }
                set { time = value; }
            }

            /// @if LANG_JA
            /// <summary>回転開始までの時間を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the time until rotation starts.</summary>
            /// @endif
            public float JumpDelay
            {
                get { return jumpDelay; }
                set { jumpDelay = value; }
            }

            /// @if LANG_JA
            /// <summary>回転開始時の角度を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the angle when rotation starts.</summary>
            /// @endif
            public float ToDegreeDelay
            {
                get { return toDegreeDelay; }
                set { toDegreeDelay = value; }
            }

            /// @if LANG_JA
            /// <summary>回転軸を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the rotation axis.</summary>
            /// @endif
            public JumpFlipEffectAxis RotationAxis
            {
                get { return rotationAxis; }
                set { rotationAxis = value; }
            }

            private float nonJumpTime;

            private PivotType orgCurrentPivotType;
            private PivotType orgNextPivotType;

            private float fromPosX;
            private float fromPosY;

            private float toPosX;
            private float toPosY;

            private float toDegree;
        }
    }
}
