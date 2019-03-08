/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {

        /// @if LANG_JA
        /// <summary>LiveSpringPanelのバネの種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Type of spring for LiveSpringPanel</summary>
        /// @endif
        public enum SpringType : int
        {
            /// @if LANG_JA
            /// <summary>X軸回転に作用するバネ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Spring that acts on the X-axis rotation</summary>
            /// @endif
            AngleAxisX = 0,

            /// @if LANG_JA
            /// <summary>Y軸回転に作用するバネ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Spring that acts on the Y-axis rotation</summary>
            /// @endif
            AngleAxisY = 1,

            /// @if LANG_JA
            /// <summary>Z軸回転に作用するバネ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Spring that acts on the Z-axis rotation</summary>
            /// @endif
            AngleAxisZ = 2,

            /// @if LANG_JA
            /// <summary>X軸方向の移動に作用するバネ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Spring that acts on the movement in the X-axis direction</summary>
            /// @endif
            PositionX = 3,

            /// @if LANG_JA
            /// <summary>Y軸方向の移動に作用するバネ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Spring that acts on the movement in the Y-axis direction</summary>
            /// @endif
            PositionY = 4,
            
            /// @if LANG_JA
            /// <summary>Z軸方向の移動に作用するバネ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Spring that acts on the movement in the Z-axis direction</summary>
            /// @endif
            PositionZ = 5,

            /// @if LANG_JA
            /// <summary>すべてのバネ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>All springs</summary>
            /// @endif
            All = 6
        }

        /// @if LANG_JA
        /// <summary>加速度センサーやパネルの動きに応じて子ウィジェットが揺れるパネル</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Panel that vibrates a child widget according to the acceleration sensor or panel movement</summary>
        /// @endif
        public class LiveSpringPanel : Panel
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public LiveSpringPanel()
            {
                this.widgetInfos = new Dictionary<Widget, WidgetInfo>();

                defaultDampingConstant = 0.2f;
                defaultSpringConstant = 0.3f;

                ReflectSensorAcceleration = true;
                ReflectMotionAcceleration = true;

                initPrevPanelPos = false;
                initPrevPanelVelocity = false;
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
                
                if (elapsedTime > 100.0f)
                {
                    return;
                }

                CalculateMoveAcceleration(elapsedTime);

                foreach (Widget widget in Children)
                {
                    UpdateWidgetsPosition(widget, elapsedTime);
                }
            }
            
            /// @if LANG_JA
            /// <summary>モーションイベントハンドラ</summary>
            /// <param name="motionEvent">モーションイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Motion event handler</summary>
            /// <param name="motionEvent">Motion event</param>
            /// @endif
            protected internal override void OnMotionEvent(MotionEvent motionEvent)
            {
                base.OnMotionEvent(motionEvent);

                if (ReflectSensorAcceleration)
                {
                    sensorAcceleration = motionEvent.Acceleration;
                    sensorAcceleration.Y = -sensorAcceleration.Y;
                    sensorAcceleration.Z = -sensorAcceleration.Z;
                }
            }

            private void CalculateMoveAcceleration(float elapsedTime)
            {
                if (ReflectMotionAcceleration && elapsedTime > float.Epsilon)
                {
                    Vector3 curPanelPos = this.LocalToWorld.AxisW;
                    if (initPrevPanelPos)
                    {
                        Vector3 curPanelVelocity = Vector3.Multiply(Vector3.Subtract(curPanelPos, prevPanelPos), 1.0f / elapsedTime);
                        if (initPrevPanelVelocity)
                        {
                            moveAcceleration = Vector3.Subtract(curPanelVelocity, prevPanelVelocity);
                            moveAcceleration.X = ClipAcceleration(moveAcceleration.X);
                            moveAcceleration.Y = ClipAcceleration(moveAcceleration.Y);
                            moveAcceleration.Z = ClipAcceleration(moveAcceleration.Z);
                        }
                        prevPanelVelocity = curPanelVelocity;
                        initPrevPanelVelocity = true;
                    }
                    prevPanelPos = curPanelPos;
                    initPrevPanelPos = true;
                }
            }

            private void UpdateWidgetsPosition(Widget widget, float elapsedTime)
            {
                WidgetInfo info = widgetInfos[widget];

                // Save pivot type
                info.originalPivot = widget.PivotType;
                widget.PivotType = PivotType.MiddleCenter;

                // Check widget position
                if (info.prevTransform3D != widget.Transform3D)
                {
                    info.originalPos = ( Matrix4.Translation(
                                        -info.prevDisplacements[(int)SpringType.PositionX],
                                        -info.prevDisplacements[(int)SpringType.PositionY],
                                        -info.prevDisplacements[(int)SpringType.PositionZ])
                                        * widget.Transform3D
                                        * Matrix4.RotationXyz(
                                        -info.prevDisplacements[(int)SpringType.AngleAxisX],
                                        -info.prevDisplacements[(int)SpringType.AngleAxisY],
                                        -info.prevDisplacements[(int)SpringType.AngleAxisZ])).ColumnW.Xyz;
                }

                // Calculate displacements
                foreach (SpringType type in Enum.GetValues(typeof(SpringType)))
                {
                    if (info.useSpecifiedValues[(int)type])
                    {
                        info.useSpecifiedValues[(int)type] = false;
                    }
                    else
                    {
                        float externalAcceleration = CalculateExternalAcceleration(info, type);
                        float acceleration = (- info.springConstants[(int)type] * info.displacements[(int)type]
                                                + info.dampingConstants[(int)type] * info.velocities[(int)type]) / info.weight
                                                 + externalAcceleration;
                        info.velocities[(int)type] += acceleration * elapsedTime;
                        info.displacements[(int)type] += info.velocities[(int)type];
                        info.displacements[(int)type] *= info.springLimitations[(int)type];
                    }

                    info.prevDisplacements[(int)type] = info.displacements[(int)type];
                }
               
                float positionX = info.displacements[(int)SpringType.PositionX];
                float positionY = info.displacements[(int)SpringType.PositionY];
                float positionZ = info.displacements[(int)SpringType.PositionZ];
                float angleAxisX = info.displacements[(int)SpringType.AngleAxisX];
                float angleAxisY = info.displacements[(int)SpringType.AngleAxisY];
                float angleAxisZ = info.displacements[(int)SpringType.AngleAxisZ];

                widget.Transform3D = Matrix4.Translation(positionX, positionY, positionZ)
                                    * Matrix4.Translation(info.originalPos)
                                    * Matrix4.RotationXyz(angleAxisX, angleAxisY, angleAxisZ);

                info.prevTransform3D = widget.Transform3D;

                // Restore pivot type
                widget.PivotType = info.originalPivot;
            }

            private float CalculateExternalAcceleration(WidgetInfo info, SpringType type)
            {
                float externalAcceleration = 0.0f;
                switch(type)
                {
                case SpringType.AngleAxisY:
                    externalAcceleration = -(sensorAcceleration.X + moveAcceleration.X + userAcceleration.X);
                    break;

                case SpringType.PositionX:
                    externalAcceleration = sensorAcceleration.X + moveAcceleration.X + userAcceleration.X;
                    break;

                case SpringType.AngleAxisX:
                case SpringType.PositionY:
                    externalAcceleration = sensorAcceleration.Y + moveAcceleration.Y + userAcceleration.Y;
                    break;

                case SpringType.PositionZ:
                    externalAcceleration = sensorAcceleration.Z + moveAcceleration.Z + userAcceleration.Z;
                    break;

                case SpringType.AngleAxisZ:
                    bool skipRotationZ = false;
                    foreach (bool useSpecifiedValue in info.useSpecifiedValues)
                    {
                        skipRotationZ |= useSpecifiedValue;
                    }
                    if (!skipRotationZ)
                    {
                        float posX = info.displacements[(int)SpringType.PositionX];
                        float posY = info.displacements[(int)SpringType.PositionY];
                        float absPosX = FMath.Abs(posX);
                        float absPosY = FMath.Abs(posY);
                        bool plus = false;
                        float diff = 0.0f;
                        if(posX * posY > 0)
                        {
                            plus = absPosX > absPosY;
                        }
                        else
                        {
                            plus = absPosX < absPosY;
                        }
                        diff = FMath.Min(FMath.Abs(absPosX - absPosY), FMath.Min(absPosX, absPosY)) / 200.0f;
                        float externalAccelerationPos = plus ? diff : -diff;

                        float axisX = info.displacements[(int)SpringType.AngleAxisX];
                        float axisY = info.displacements[(int)SpringType.AngleAxisY];
                        float absAxisX = FMath.Abs(axisX);
                        float absAxisY = FMath.Abs(axisY);
                        plus = false;
                        diff = 0.0f;
                        if(-axisX * axisY > 0)
                        {
                            plus = absAxisX < absAxisY;
                        }
                        else
                        {
                            plus = absAxisX > absAxisY;
                        }
                        diff = FMath.Min(FMath.Abs(absAxisX - absAxisY), FMath.Min(absAxisX, absAxisY)) / 20.0f;
                        float externalAccelerationAngle = plus ? diff : -diff;
                        externalAcceleration = externalAccelerationPos + externalAccelerationAngle;
                    }
                    break;
                }
                
                if (type == SpringType.PositionX || type == SpringType.PositionY || type == SpringType.PositionZ)
                {
                    return ClipAcceleration(externalAcceleration);
                }
                else
                {
                    return ClipAcceleration(externalAcceleration) * distanceToAngleRate;
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
                base.AddChildFirst(child);
                AddWidgetInfo(child);
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
                base.AddChildLast(child);
                AddWidgetInfo(child);
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
                base.InsertChildBefore(child, nextChild);
                AddWidgetInfo(child);
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
                base.InsertChildAfter(child, prevChild);
                AddWidgetInfo(child);
            }

            /// @if LANG_JA
            /// <summary>指定された子ウィジェットを削除する。</summary>
            /// <param name="child">削除するウィジェット</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes the specified child widget.</summary>
            /// <param name="child">Widget to be deleted</param>
            /// @endif
            public override void RemoveChild(Widget child)
            {
                base.RemoveChild(child);
                removeWidgetInfo(child);
            }

            private void AddWidgetInfo(Widget widget)
            {
                int springTypeCount = Enum.GetNames(typeof(SpringType)).Length;
                WidgetInfo info = new WidgetInfo()
                {
                    originalPivot = widget.PivotType,
                    weight = 200.0f,
                    springConstants = new float[springTypeCount],
                    dampingConstants = new float[springTypeCount],
                    springLimitations = new float[springTypeCount],
                    velocities = new float[springTypeCount],
                    displacements = new float[springTypeCount],
                    useSpecifiedValues = new bool[springTypeCount],
                    prevDisplacements = new float[springTypeCount]
                };
                this.widgetInfos.Add(widget, info);

                foreach (SpringType type in Enum.GetValues(typeof(SpringType)))
                {
                    SetSpringConstant(widget, type, defaultSpringConstant);
                    SetDampingConstant(widget, type, defaultDampingConstant);
                }

                widget.PivotType = PivotType.MiddleCenter;
                info.originalPos = widget.Transform3D.ColumnW.Xyz;
                info.prevTransform3D = widget.Transform3D;
                widget.PivotType = info.originalPivot;
            }

            private void removeWidgetInfo(Widget widget)
            {
                widgetInfos.Remove(widget);
            }

            /// @if LANG_JA
            /// <summary>加速度センサーの値を子ウィジェットの動きに反映させるかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to apply the acceleration sensor value to the child widget movement.</summary>
            /// @endif
            public bool ReflectSensorAcceleration
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>パネルの動きを子ウィジェットの動きに反映させるかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to apply the panel movement to the child widget movement.</summary>
            /// @endif
            public bool ReflectMotionAcceleration
            {
                get
                {
                    return reflectMotionAcceleration;
                }
                set
                {
                    initPrevPanelPos = false;
                    initPrevPanelVelocity = false;
                    reflectMotionAcceleration = value;
                }
            }
            private bool reflectMotionAcceleration;

            /// @if LANG_JA
            /// <summary>バネの減衰定数（揺れが収束までの早さ）を設定する。</summary>
            /// <remarks>0以上、1以下の値で、大きくなればゆれが収束するまでの時間が短くなる。デフォルトは0.2。</remarks>
            /// <param name="widget">減衰定数を設定するウィジェット。nullを指定するとすべてのウィジェットの減衰定数を変更する。</param>
            /// <param name="type">減衰定数を設定するバネの種類。SpringType.Allを指定するとすべてのバネの減衰定数を変更する。</param>
            /// <param name="dampingConstant">減衰定数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets the spring damping constant (the speed until the vibration converges).</summary>
            /// <remarks>When this value is between 0 and 1, the larger the value, the shorter the time until the vibration converges. Default is 0.2.</remarks>
            /// <param name="widget">Widget that sets the damping constant. Changes the damping constant of all the widgets when null is specified.</param>
            /// <param name="type">Type of spring that sets the damping constant. Changes the damping constant of all the springs when SpringType.All is specified.</param>
            /// <param name="dampingConstant">Damping constant</param>
            /// @endif
            public void SetDampingConstant(Widget widget, SpringType type, float dampingConstant)
            {
                SetValue(widget, type, dampingConstant, (Widget w, SpringType t, float value) =>
                {
                    value = FMath.Clamp(value, 0.0f, 1.0f);
                    widgetInfos[w].dampingConstants[(int)t] = -10.0f * value;
                });
            }

            /// @if LANG_JA
            /// <summary>バネの減衰定数（揺れが収束までの早さ）を取得する。</summary>
            /// <remarks>0以上、1以下の値で、大きくなればゆれが収束するまでの時間が短くなる。デフォルトは0.2。</remarks>
            /// <param name="widget">減衰定数を取得するウィジェット</param>
            /// <param name="type">減衰定数を取得するバネの種類</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the spring damping constant (the speed until the vibration converges).</summary>
            /// <remarks>When this value is between 0 and 1, the larger the value, the shorter the time until the vibration converges. Default is 0.2.</remarks>
            /// <param name="widget">Widget that obtains the damping constant</param>
            /// <param name="type">Type of spring that obtains the damping constant</param>
            /// @endif
            public float GetDampingConstant(Widget widget, SpringType type)
            {
                if (widgetInfos.ContainsKey(widget) && type != SpringType.All)
                {
                    return widgetInfos[widget].dampingConstants[(int)type];
                }
                return 0;
            }

            /// @if LANG_JA
            /// <summary>バネ定数（バネが元の位置に戻ろうとする強さ）を設定する。</summary>
            /// <remarks>0以上、1以下の値で、大きくなればゆれが小さくなる。デフォルトは0.3。</remarks>
            /// <param name="widget">バネ定数を設定するウィジェット。nullを指定するとすべてのWidgetのバネ定数を変更する。</param>
            /// <param name="type">バネ定数を設定するバネの種類。SpringType.Allを指定するとすべてのバネの減衰定数を変更する。</param>
            /// <param name="springConstant">バネ定数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets the spring constant (the strength at which the spring tries to return to its original position).</summary>
            /// <remarks>The vibration decreases as the value between 0 and 1 increases. Default is 0.3.</remarks>
            /// <param name="widget">Widget that sets the spring constant. Changes the spring constant of all the widgets when null is specified.</param>
            /// <param name="type">Type of spring that sets the spring constant. Changes the damping constant of all the springs when SpringType.All is specified.</param>
            /// <param name="springConstant">Spring constant</param>
            /// @endif
            public void SetSpringConstant(Widget widget, SpringType type, float springConstant)
            {
                SetValue(widget, type, springConstant, (Widget w, SpringType t, float value) =>
                {
                    value = FMath.Clamp(value, 0.0f, 1.0f);
                    widgetInfos[w].springConstants[(int)t] = 4.0f * value + 1.0f;
                    widgetInfos[w].springLimitations[(int)t] = (1.0f - FMath.Pow(value, 5.0f));
                });
            }

            /// @if LANG_JA
            /// <summary>バネ定数（バネが元の位置に戻ろうとする強さ）を取得する。</summary>
            /// <remarks>0以上、1以下の値で、大きくなればゆれが小さくなる。デフォルトは0.3。</remarks>
            /// <param name="widget">バネ定数を取得するウィジェット</param>
            /// <param name="type">バネ定数を取得するバネの種類</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the spring constant (the strength at which the spring tries to return to its original position).</summary>
            /// <remarks>The vibration decreases as the value between 0 and 1 increases. Default is 0.3.</remarks>
            /// <param name="widget">Widget that obtains the spring constant</param>
            /// <param name="type">Type of spring that obtains the spring constant</param>
            /// @endif
            public float GetSpringConstant(Widget widget, SpringType type)
            {
                if (widgetInfos.ContainsKey(widget) && type != SpringType.All)
                {
                    return widgetInfos[widget].springConstants[(int)type];
                }
                return 0;
            }

            /// @if LANG_JA
            /// <summary>バネの変化量（初期位置からのずれ）を設定する。</summary>
            /// <param name="widget">変化量を設定するウィジェット</param>
            /// <param name="type">変化量を設定するバネの種類</param>
            /// <param name="displacement">変化量</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets the spring displacement (the deviation from the default position).</summary>
            /// <param name="widget">Widget for setting the displacement</param>
            /// <param name="type">Type of spring that sets the displacement</param>
            /// <param name="displacement">Displacement</param>
            /// @endif
            public void SetDisplacement(Widget widget, SpringType type, float displacement)
            {
                SetValue(widget, type, displacement, (Widget w, SpringType t, float value) =>
                {
                    widgetInfos[w].displacements[(int)t] = value;
                    widgetInfos[w].useSpecifiedValues[(int)t] = true;
                });
            }

            /// @if LANG_JA
            /// <summary>バネの変化量（初期位置からのずれ）を取得する。</summary>
            /// <param name="widget">変化量を取得するウィジェット</param>
            /// <param name="type">変化量を取得するバネの種類</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the spring displacement (the deviation from the default position).</summary>
            /// <param name="widget">Widget for obtaining the displacement</param>
            /// <param name="type">Type of spring that obtains the displacement</param>
            /// @endif
            public float GetDisplacement(Widget widget, SpringType type)
            {
                if (widgetInfos.ContainsKey(widget) && type != SpringType.All)
                {
                    return widgetInfos[widget].displacements[(int)type];
                }
                return 0;
            }

            private void SetValue(Widget widget, SpringType type, float value, Action<Widget, SpringType, float> setAction)
            {
                foreach (Widget w in this.Children)
                {
                    if (widget != null && widget != w)
                    {
                        continue;
                    }

                    foreach (SpringType t in Enum.GetValues(typeof(SpringType)))
                    {
                        if (type != SpringType.All && type != t)
                        {
                            continue;
                        }

                        if (widgetInfos.ContainsKey(w))
                        {
                            setAction(w, t, value);
                        }
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>子ウィジェットに加速度を加える</summary>
            /// <param name="x">X方向の加速度</param>
            /// <param name="y">Y方向の加速度</param>
            /// <param name="z">Z方向の加速度</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds the acceleration to the child widget</summary>
            /// <param name="x">Acceleration in the X-axis direction</param>
            /// <param name="y">Acceleration in the Y-axis direction</param>
            /// <param name="z">Acceleration in the Z-axis direction</param>
            /// @endif
            public void AddAcceleraton(float x, float y, float z)
            {
                userAcceleration.X = ClipAcceleration(x);
                userAcceleration.Y = ClipAcceleration(y);
                userAcceleration.Z = ClipAcceleration(z);
            }

            private float ClipAcceleration(float acceleration)
            {
                return FMath.Clamp(acceleration, -1.0f, 1.0f);
            }

            private class WidgetInfo
            {
                public Vector3 originalPos;
                public PivotType originalPivot;
                public float weight;
                public Matrix4 prevTransform3D;

                public float[] springConstants;
                public float[] dampingConstants;
                public float[] springLimitations;
                public float[] velocities;
                public float[] displacements;
                public bool[] useSpecifiedValues;
                public float[] prevDisplacements;
            }

            private float defaultDampingConstant;
            private float defaultSpringConstant;
            private const float distanceToAngleRate = FMath.PI / 250.0f;
            private Dictionary<Widget, WidgetInfo> widgetInfos;
            private Vector3 prevPanelPos;
            private Vector3 prevPanelVelocity;
            private Vector3 userAcceleration;
            private Vector3 sensorAcceleration;
            private Vector3 moveAcceleration;
            private bool initPrevPanelPos;
            private bool initPrevPanelVelocity;
        }
    }
}
