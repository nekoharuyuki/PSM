/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.UI;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {
        internal enum PropertyType
        {
            MotionX   = 0,
            MotionY   = 1,
            MotionZ   = 2,
            RotationX = 3,
            RotationY = 4,
            RotationZ = 5,
            ScaleX    = 6,
            ScaleY    = 7,
            ScaleZ    = 8,
            SkewX     = 9,
            SkewY     = 10,
            SkewZ     = 11,
            Alpha     = 12
        }

        internal enum EaseType
        {
            Default          = 0,
            Quadratic        = 1,
            Cubic            = 2,
            Quartic          = 3,
            Quintic          = 4,
            DualQuadratic    = 5,
            DualCubic        = 6,
            DualQuartic      = 7,
            DualQuintic      = 8,
            Bounce           = 9,
            BounceIn         = 10,
            Spring           = 11,
            SineWave         = 12,
            SawtoothWave     = 13,
            SquareWave       = 14,
            RandomSquareWave = 15,
            DampedWave       = 16,
            Custom           = 17
        }

        /// @if LANG_JA
        /// <summary>uim 形式のモーションを再生するクラス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Class that plays back uim format motion</summary>
        /// @endif
        public class UIMotion : Effect
        {
            private float[] propertyValues;
            private float duration;
            private Matrix4 motionStartTransform3D;
            private readonly Dictionary<PropertyType,AnimationUtility.CubicBezierCurveSequence> timelines = new Dictionary<PropertyType, AnimationUtility.CubicBezierCurveSequence>();
            
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            private UIMotion() : this(null, (string)null)
            {
            }

            internal UIMotion(Widget widget, UIMotionData data) : this(widget, (string)null)
            {
                SetMotionData(data);
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="widget">エフェクト対象のウィジェット</param>
            /// <param name="filePath">uim ファイルのパス</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="filePath">uim file path</param>
            /// @endif
            public UIMotion(Widget widget, string filePath)
            {
                Widget = widget;
                propertyValues = new float[Enum.GetValues(typeof(PropertyType)).Length];

                if (filePath != null && !filePath.Equals(""))
                {
                    UIMotionData data = new UIMotionData();
                    data.Read(filePath);
                    SetMotionData(data);
                }
            }
            
            /// @if LANG_JA
            /// <summary>uim ファイルからUIMotionを生成し、エフェクトを開始する。</summary>
            /// <param name="widget">エフェクト対象のウィジェット</param>
            /// <param name="filePath">uim ファイルのパス</param>
            /// <returns>UIMotionのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Generates UIMotion from an uim file and starts the effect.</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="filePath">uim file path</param>
            /// <returns>UIMotion instance</returns>
            /// @endif
            public static UIMotion CreateAndStart(Widget widget, string filePath)
            {
                UIMotion uim = new UIMotion(widget, filePath);
                uim.Start();
                return uim;
            }

            private void SetMotionData(UIMotionData data)
            {
                float timeScale = (float)data.header.timeScale;
                Func<float,float> timeValueToMilliSecond = (timeValue) => timeValue / timeScale * 1000.0f;

                duration = timeValueToMilliSecond((float)data.header.duration);

                foreach (UIMotionData.Property property in data.properties)
                {
                    AnimationInterpolator interpolator;
                    if (property.header.useTimeMap)
                    {
                        var strength = data.timeMaps[(int)property.header.timeMapIndex].header.strength;
                        switch(data.timeMaps[(int)property.header.timeMapIndex].header.easeType)
                        {
                        case EaseType.Quadratic:
                            interpolator = AnimationUtility.GetQuadInterpolator(strength);
                            break;
                        case EaseType.Cubic:
                            interpolator = AnimationUtility.GetCubicInterpolator(strength);
                            break;
                        case EaseType.Quartic:
                            interpolator = AnimationUtility.GetQuarticInterpolator(strength);
                            break;
                        case EaseType.Quintic:
                            interpolator = AnimationUtility.GetQuinticInterpolator(strength);
                            break;
                        case EaseType.DualQuadratic:
                            interpolator = AnimationUtility.GetDualQuadInterpolator(strength);
                            break;
                        case EaseType.DualCubic:
                            interpolator = AnimationUtility.GetDualCubicInterpolator(strength);
                            break;
                        case EaseType.DualQuartic:
                            interpolator = AnimationUtility.GetDualQuarticInterpolator(strength);
                            break;
                        case EaseType.DualQuintic:
                            interpolator = AnimationUtility.GetDualQuinticInterpolator(strength);
                            break;
                        case EaseType.Bounce:
                            interpolator = AnimationUtility.GetBounceInterpolator(strength);
                            break;
                        case EaseType.BounceIn:
                            interpolator = AnimationUtility.GetBounceInInterpolator(strength);
                            break;
                        case EaseType.Spring:
                            interpolator = AnimationUtility.GetSpringInterpolator(strength);
                            break;
                        case EaseType.SineWave:
                            interpolator = AnimationUtility.GetSineWaveInterpolator(strength);
                            break;
                        case EaseType.SawtoothWave:
                            interpolator = AnimationUtility.GetSawtoothWaveInterpolator(strength);
                            break;
                        case EaseType.SquareWave:
                            interpolator = AnimationUtility.GetSquareWaveInterpolator(strength);
                            break;
                        case EaseType.RandomSquareWave:
                            interpolator = AnimationUtility.GetRandomSquareWaveInterpolator(strength);
                            break;
                        case EaseType.DampedWave:
                            interpolator = AnimationUtility.GetDampedWaveInterpolator(strength);
                            break;
                        default:
                            interpolator = AnimationUtility.LinearInterpolator;
                            break;
                        }
                    }
                    else
                    {
                        interpolator = AnimationUtility.LinearInterpolator;
                    }

                    if(property.keyframes.Count > 0)
                    {
                        var firstKeyFrame = property.keyframes[0];
                        var timeline = new AnimationUtility.CubicBezierCurveSequence(timeValueToMilliSecond(firstKeyFrame.time), firstKeyFrame.anchorY);
                        timeline.EasingCurve = interpolator;
                        for (int i = 0; i < property.keyframes.Count - 1; i++)
                        {
                            int curKeyFrameIndex = i;
                            int nextKeyFrameIndex = i + 1;
    
                            var currentFrame = property.keyframes[curKeyFrameIndex];
                            var nextFrame = property.keyframes[nextKeyFrameIndex];
                            var currentTimeValue = (float)currentFrame.time;
                            var nextTimeValue = (float)nextFrame.time;
          
                            var control1X = timeValueToMilliSecond((float)currentFrame.nextX + currentTimeValue);
                            var control1Y = (float)currentFrame.nextY;
                            var control2X = timeValueToMilliSecond((float)nextFrame.previousX + nextTimeValue);
                            var control2Y = (float)nextFrame.previousY;
                            var nextAnchorX = timeValueToMilliSecond((float)nextFrame.time);
                            var nextAnchorY = (float)nextFrame.anchorY;
    
                            timeline.AppendSegment(control1X, control1Y, control2X, control2Y, nextAnchorX, nextAnchorY);
                        }
                        timelines.Add(property.header.propertyType, timeline);
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>開始処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Start processing</summary>
            /// @endif
            protected override void OnStart()
            {
                propertyValues[(int)PropertyType.ScaleX] = 1.0f;
                propertyValues[(int)PropertyType.ScaleY] = 1.0f;
                propertyValues[(int)PropertyType.ScaleZ] = 1.0f;
                propertyValues[(int)PropertyType.MotionX] = 0.0f;
                propertyValues[(int)PropertyType.MotionY] = 0.0f;
                propertyValues[(int)PropertyType.MotionZ] = 0.0f;
                propertyValues[(int)PropertyType.RotationX] = 0.0f;
                propertyValues[(int)PropertyType.RotationY] = 0.0f;
                propertyValues[(int)PropertyType.RotationZ] = 0.0f;
                propertyValues[(int)PropertyType.SkewX] = 0.0f;
                propertyValues[(int)PropertyType.SkewY] = 0.0f;
                propertyValues[(int)PropertyType.SkewZ] = 0.0f;
                propertyValues[(int)PropertyType.Alpha] = 100.0f;

                motionStartTransform3D = Widget.Transform3D;
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
                foreach(var propertyInfo in timelines)
                {
                    propertyValues[(int)propertyInfo.Key] = propertyInfo.Value.GetValue(TotalElapsedTime);
                }
                Matrix4 scaleMat;
                Matrix4.Scale(propertyValues[(int)PropertyType.ScaleX],
                                propertyValues[(int)PropertyType.ScaleY],
                                propertyValues[(int)PropertyType.ScaleZ],
                                out scaleMat);

                Matrix4 motionMat;
                Matrix4.Translation(motionStartTransform3D.M41 + propertyValues[(int)PropertyType.MotionX],
                                    motionStartTransform3D.M42 + propertyValues[(int)PropertyType.MotionY],
                                    motionStartTransform3D.M43 + propertyValues[(int)PropertyType.MotionZ],
                                    out motionMat);

                Matrix4 rotationMat;
                Matrix4.RotationXyz(propertyValues[(int)PropertyType.RotationX],
                                    propertyValues[(int)PropertyType.RotationY],
                                    propertyValues[(int)PropertyType.RotationZ],
                                    out rotationMat);

                Matrix4 matA, matB;
                motionMat.Multiply(ref rotationMat, out matA);
                matA.Multiply(ref scaleMat, out matB);
                Widget.Transform3D = matB;

                Widget.Alpha = propertyValues[(int)PropertyType.Alpha] / 100.0f;

                if (TotalElapsedTime >= duration)
                {
                    return EffectUpdateResponse.Finish;
                }
                return EffectUpdateResponse.Continue;
            }

            /// @if LANG_JA
            /// <summary>エフェクトを停止する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stops the effect.</summary>
            /// @endif
            public new void Stop()
            {
                base.Stop();
                Widget.Transform3D = motionStartTransform3D;
            }

            /// @if LANG_JA
            /// <summary>停止処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stop processing</summary>
            /// @endif
            protected override void OnStop()
            {
            }

            /// @if LANG_JA
            /// <summary>リピート処理</summary>
            /// <remarks>派生クラスでリピート処理を実装する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Repeat processing</summary>
            /// <remarks>Implements the repeat processing with the derived class.</remarks>
            /// @endif
            protected override void OnRepeat()
            {
                Widget.Transform3D = motionStartTransform3D;
            }
        }

        internal class UIMotionData
        {
            private const string SIGNATURE = "_uim";
            private const string VERSION = "0001";

            public struct Header
            {
                public uint timeScale;
                public uint duration;
                public bool orientToPath;
                public float xformPtXOffsetPct;
                public float xformPtYOffsetPct;
                public float xformPtZOffsetPixels;
                public uint propertyNum;
                public uint timeMapNum;
            }

            public class TimeMap
            {
                public struct TimeMapHeader
                {
                    public EaseType easeType;
                    public int strength;
                    public uint customPointNum;
                }

                public struct CustomPoint
                {
                    public float anchorX;
                    public float anchorY;
                    public float nextX;
                    public float nextY;
                    public float previousX;
                    public float previousY;
                }

                public TimeMapHeader header;
                public List<CustomPoint> customPoints = new List<CustomPoint>();

                public void Read(FileStream fs)
                {
                    header = (TimeMapHeader)ReadObject(fs, typeof(TimeMapHeader));

                    if (header.easeType == EaseType.Custom)
                    {
                        for (int i = 0; i < header.customPointNum; i++)
                        {
                            CustomPoint customPoint = (CustomPoint)ReadObject(fs, typeof(CustomPoint));
                            customPoints.Add(customPoint);
                        }
                    }
                }
            }

            public class Property
            {
                public struct PropertyHeader
                {
                    public PropertyType propertyType;
                    public bool useTimeMap;
                    public uint timeMapIndex;
                    public uint keyframeNum;
                }

                public struct Keyframe
                {
                    public float anchorX;
                    public float anchorY;
                    public float nextX;
                    public float nextY;
                    public float previousX;
                    public float previousY;
                    public bool roving;
                    public uint time;
                }

                public PropertyHeader header = new PropertyHeader();
                public List<Keyframe> keyframes = new List<Keyframe>();

                public void Read(FileStream fs)
                {
                    header = (PropertyHeader)ReadObject(fs, typeof(PropertyHeader));

                    for (int i = 0; i < header.keyframeNum; i++)
                    {
                        Keyframe keyframe = (Keyframe)ReadObject(fs, typeof(Keyframe));
                        keyframes.Add(keyframe);
                    }
                }
            }

            internal Header header;
            public List<TimeMap> timeMaps = new List<TimeMap>();
            public List<Property> properties = new List<Property>();

            public void Read(String fileName)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    if (fs != null)
                    {
                        Read(fs);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }

            public void Read(FileStream fs)
            {
                if (fs != null)
                {
                    CheckSignatureAndVersion(fs);
                    ReadHeader(fs);
                    ReadTimeMaps(fs);
                    ReadProperties(fs);
                }
            }

            private void CheckSignatureAndVersion(FileStream fs)
            {
                byte[] buf = new byte[sizeof(byte) * 4];

                string signature = "";
                if (fs.Read(buf, 0, buf.Length) == buf.Length)
                {
                    signature = Encoding.ASCII.GetString(buf);
                }

                string version = "";
                if (fs.Read(buf, 0, buf.Length) == buf.Length)
                {
                    version = Encoding.ASCII.GetString(buf);
                }
                
                if (!signature.Equals(SIGNATURE) || !version.Equals(VERSION))
                {
                    throw new FileLoadException("Invalid file format");
                }
            }

            private void ReadHeader(FileStream fs)
            {
                this.header = (Header)ReadObject(fs, typeof(Header));
            }

            private void ReadTimeMaps(FileStream fs)
            {
                for (int i = 0; i < this.header.timeMapNum; i++)
                {
                    TimeMap timeMap = new TimeMap();
                    timeMap.Read(fs);
                    timeMaps.Add(timeMap);
                }
            }

            private void ReadProperties(FileStream fs)
            {
                for (int i = 0; i < this.header.propertyNum; i++)
                {
                    Property property = new Property();
                    property.Read(fs);
                    properties.Add(property);
                }
            }

            private static object ReadObject(FileStream fs, Type type)
            {
                object obj = type.InvokeMember(null,
                                System.Reflection.BindingFlags.CreateInstance,
                                null, null, null);

                foreach(var field in type.GetFields())
                {
                    byte[] buf;

                    switch (field.FieldType.Name)
                    {
                    case "UInt32":
                        buf = new byte[sizeof(UInt32)];
                        if (fs.Read(buf, 0, buf.Length) == buf.Length)
                        {
                            field.SetValue(obj, BitConverter.ToUInt32(buf, 0));
                        }
                        break;
                    case "Int32":
                        buf = new byte[sizeof(Int32)];
                        if (fs.Read(buf, 0, buf.Length) == buf.Length)
                        {
                            field.SetValue(obj, BitConverter.ToInt32(buf, 0));
                        }
                        break;
                    case "Char":
                        buf = new byte[sizeof(Char)];
                        if (fs.Read(buf, 0, buf.Length) == buf.Length)
                        {
                            field.SetValue(obj, BitConverter.ToChar(buf, 0));
                        }
                        break;
                    case "Single":
                        buf = new byte[sizeof(Single)];
                        if (fs.Read(buf, 0, buf.Length) == buf.Length)
                        {
                            field.SetValue(obj, BitConverter.ToSingle(buf, 0));
                        }
                        break;
                    case "EaseType":
                        buf = new byte[sizeof(EaseType)];
                        if (fs.Read(buf, 0, buf.Length) == buf.Length)
                        {
                            field.SetValue(obj, (EaseType)BitConverter.ToInt32(buf, 0));
                        }
                        break;
                    case "PropertyType":
                        buf = new byte[sizeof(PropertyType)];
                        if (fs.Read(buf, 0, buf.Length) == buf.Length)
                        {
                            field.SetValue(obj, (PropertyType)BitConverter.ToInt32(buf, 0));
                        }
                        break;
                    case "Boolean":
                        buf = new byte[4];
                        if (fs.Read(buf, 0, buf.Length) >= buf.Length)
                        {
                            field.SetValue(obj, BitConverter.ToBoolean(buf, 0));
                        }
                        break;
                    }
                }
                return obj;
            }
        }
    }
}
