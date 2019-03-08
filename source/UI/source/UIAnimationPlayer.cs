/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using System.IO;

using Sce.PlayStation.HighLevel.UI;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {
        /// @if LANG_JA
        /// <summary>uia 形式のアニメーションを再生するクラス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Class that plays back uia format animation</summary>
        /// @endif
        public class UIAnimationPlayer : Panel
        {
            
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            private UIAnimationPlayer() : this(null)
            {
            }
            
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="filePath">uia ファイルのパス</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="filePath">uia file path</param>
            /// @endif
            public UIAnimationPlayer(string filePath)
            {
                Clip = true;

                if (filePath != null)
                {
                    UIAnimationData data = new UIAnimationData();
                    data.ReadFromFile(filePath);

                    this.SetSize(data.header.width, data.header.height);
                    this.BackgroundColor = new UIColor(data.header.backgroundColorR / 255.0f,
                                                       data.header.backgroundColorG / 255.0f,
                                                       data.header.backgroundColorB / 255.0f,
                                                       1.0f);

                    if (data.rootSymbol != null && data.rootSymbol.timeLines != null
                        && data.rootSymbol.timeLines[0] != null && data.rootSymbol.timeLines[0].layers != null)
                    {
                        loadImageAssets(data);

                        rootLayer = new Layer();
                        rootLayer.timeLineForChildren.FrameRate = data.header.frameRate;
                        rootLayer.AnimationStopped += OnRootLayoutAnimationStoped;
                        foreach (var layerData in data.rootSymbol.timeLines[0].layers)
                        {
                            Layer layer = new Layer();
                            uint childDuration = layer.InitLayer(rootLayer, layerData, 0, 0, data, imageAssets);
                            rootLayer.transformRoot.AddChildLast(layer);
                            rootLayer.childLayers.Add(layer);
    
                            if (rootLayer.timeLineForChildren.Duration < childDuration)
                            {
                                rootLayer.timeLineForChildren.Duration = childDuration;
                            }
                        }
                        rootLayer.Repeating = data.header.repeating;
                        this.Width = data.header.width;
                        this.Height = data.header.height;
                        
                        this.AddChildLast(rootLayer);
                    }
                }
            }

            private void loadImageAssets(UIAnimationData data)
            {
                UIDebug.Assert(imageAssets == null);
                imageAssets = new ImageAsset[data.bitmaps.Count];
                for (int i = 0; i < data.bitmaps.Count; i++)
                {
                    Texture2D tex = new Texture2D((int)data.bitmaps[i].header.width, (int)data.bitmaps[i].header.height, false, PixelFormat.Rgba);
                    tex.SetPixels(0, data.bitmaps[i].rawData);
                    ImageAsset imageAsset = new ImageAsset(tex);
                    tex.Dispose();
                    data.bitmaps[i].rawData = null;
                    imageAssets[i] = imageAsset;
                }
            }

            /// @if LANG_JA
            /// <summary>uia ファイルからUIAnimationPlayerを生成し、アニメーションを再生する。</summary>
            /// <param name="filePath">uia ファイルのパス</param>
            /// <returns>UIAnimationPlayerのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>UIAnimationPlayer is generated from an uia file and the animation is played back.</summary>
            /// <param name="filePath">uia file path</param>
            /// <returns>UIAnimationPlayer instance</returns>
            /// @endif
            public static UIAnimationPlayer CreateAndPlay(string filePath)
            {
                UIAnimationPlayer player = new UIAnimationPlayer(filePath);
                player.Play();
                return player;
            }

            /// @if LANG_JA
            /// <summary>アニメーションを再生する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Plays back the animation.</summary>
            /// @endif
            public void Play()
            {
                rootLayer.Start(0);
            }

            /// @if LANG_JA
            /// <summary>アニメーションを停止する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stops the animation.</summary>
            /// @endif
            public void Stop()
            {
                rootLayer.Stop();
            }

            /// @if LANG_JA
            /// <summary>アニメーションを中断する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Pauses the animation.</summary>
            /// @endif
            public void Pause()
            {
                rootLayer.Pause();
            }

            /// @if LANG_JA
            /// <summary>アニメーションを再開する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Resumes the animation.</summary>
            /// @endif
            public void Resume()
            {
                rootLayer.Resume();
            }

            /// @if LANG_JA
            /// <summary>指定したフレームに移動し、アニメーションを再生する。</summary>
            /// <param name="label">移動先のフレーム名</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Moves to the specified frame and plays back the animation.</summary>
            /// <param name="label">Frame name for the move destination</param>
            /// @endif
            private void GotoAndPlay(string label)
            {
                // TODO
            }
            
            /// @if LANG_JA
            /// <summary>ウィジェットを置き換える。</summary>
            /// <param name="name">削除するウィジェット名</param>
            /// <param name="widget">追加するウィジェット</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Replaces the widget.</summary>
            /// <param name="name">Name of the widget to be deleted</param>
            /// <param name="widget">Widget to be added</param>
            /// @endif
            public void ReplaceWidget(string name, Widget widget)
            {
                if (name == null || name.Equals(""))
                {
                    return;
                }
                
                rootLayer.ReplaceWidget(name, widget);
            }

            /// @if LANG_JA
            /// <summary>アニメーション中かどうかを取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains whether the animation is operating.</summary>
            /// @endif
            public bool Playing
            {
                get
                {
                    return rootLayer.Playing;
                }
            }

            /// @if LANG_JA
            /// <summary>アニメーションが中断されているかどうかを取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains whether the animation is paused.</summary>
            /// @endif
            public bool Paused
            {
                get
                {
                    return rootLayer.Paused;
                }
            }

            /// @if LANG_JA
            /// <summary>アニメーションをリピート再生するかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether the animation will be repeatedly played back.</summary>
            /// @endif
            public bool Repeating
            {
                get
                {
                    return rootLayer.Repeating;
                }
                set
                {
                    rootLayer.Repeating = value;
                }
            }

            private void OnRootLayoutAnimationStoped(object sender, EventArgs e)
            {
                if (sender is Layer)
                {
                    if (AnimationStopped != null)
                    {
                        AnimationStopped(this, EventArgs.Empty);
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>このウィジェットの所有するアンマネージドリソースを解放する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees the unmanaged resources held by this widget</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                if (imageAssets != null)
                {
                    foreach (var image in imageAssets)
                    {
                        if (image != null) image.Dispose();
                    }
                    imageAssets = null;
                }
                base.DisposeSelf();
            }

            /// @if LANG_JA
            /// <summary>アニメーションを停止したときに呼び出されるハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Handler called when the animation is stopped</summary>
            /// @endif
            public event EventHandler<EventArgs> AnimationStopped;

            private Layer rootLayer;
            private ImageAsset[] imageAssets;

            internal class Layer : ContainerWidget
            {
                public Layer()
                {
                    timeLineForChildren = new TimeLine();
                    timeLineForChildren.TimeLineRepeated += OnTimeLineRepeated;
                    timeLineForChildren.TimeLineStopped += OnTimeLineStoped;
                    childLayers = new List<Layer>();
                    motions = new List<Effect>();
                    startedMotions = new List<Effect>();
                    motionStartTimes = new Dictionary<Effect, float>();
                    transformRoot = new ContainerWidget();
                    initialTransform3Dtroot = transformRoot.Transform3D;
                    initialTransform3Droot = this.Transform3D;
                    this.AddChildLast(transformRoot);
                    Repeating = true;
                }

                protected override void OnUpdate(float elapsedTime)
                {
                    base.OnUpdate(elapsedTime);

                    if (!Playing)
                    {
                        return;
                    }

                    timeLineForChildren.OnUpdate(elapsedTime);

                    foreach (var child in childLayers)
                    {
                        foreach (var motion in child.motions)
                        {
                            if (child.startedMotions.Contains(motion))
                            {
                                continue;
                            }

                            float startTime = child.motionStartTimes[motion];
                            if (startTime <= timeLineForChildren.CurrentTime)
                            {
                                motion.Start();
    
                                if (child.startedMotions.Count == 0)
                                {
                                    child.timeLineForChildren.Start(timeLineForChildren.CurrentTime - startTime);
                                }
                                child.startedMotions.Add(motion);
                            }
                        }
                    }
                }

                public uint InitLayer(Layer parent, UIAnimationData.Layer layerData, float x, float y, UIAnimationData data, ImageAsset[] imageAssets)
                {
                    uint duration = 0;
                    List<uint> createdBitmapIds = new List<uint>();
                    List<uint> createdSymbolIds = new List<uint>();
                    UIMotion prevMotion = null;
                    UIMotion lastMotion = null;
                    
                    timeLineForChildren.FrameRate = data.header.frameRate;
                    this.SetPosition(x, y);

                    foreach (UIAnimationData.Frame frame in layerData.frames)
                    {
                        if (frame.instance == null)
                        {
                            continue;
                        }

                        UIAnimationData.Instance instance = frame.instance;

                        if (instance.header.hasBitmap && !createdBitmapIds.Contains(instance.header.bitmapId))
                        {
                            UIDebug.Assert(0 <= instance.header.bitmapId && instance.header.bitmapId < imageAssets.Length, "instance.header.bitmapId is out of range");

                            this.SetPosition(x + instance.header.translationX + instance.header.matrix41, y + instance.header.translationY + instance.header.matrix42);
                            Widget bitmap = CreateBitmap(imageAssets[(int)instance.header.bitmapId]);
                            bitmap.SetSize(bitmap.Width * instance.header.scaleX, bitmap.Height * instance.header.scaleY);
                            transformRoot.AddChildLast(bitmap);
                            createdBitmapIds.Add(instance.header.bitmapId);
                        }

                        if(instance.header.hasSymbol && !createdSymbolIds.Contains(instance.header.symbolId))
                        {
                            transformRoot.SetPosition(instance.header.transPointX, instance.header.transPointY);
                            this.SetPosition(x + instance.header.translationX + instance.header.matrix41, y + instance.header.translationY + instance.header.matrix42);
                            UIAnimationData.Symbol symbol = data.symbols[(int)instance.header.symbolId];

                            foreach (var childLayerInfo in symbol.timeLines[0].layers)
                            {
                                Layer layer = new Layer();
                                uint childDuration = layer.InitLayer(this, childLayerInfo, -instance.header.transPointX, -instance.header.transPointY, data, imageAssets);
                                transformRoot.AddChildLast(layer);
                                childLayers.Add(layer);
                                
                                if (timeLineForChildren.Duration < childDuration)
                                {
                                    timeLineForChildren.Duration = childDuration;
                                }
                            }
                            createdSymbolIds.Add(instance.header.symbolId);
                        }

                        if (frame.header.hasMotion)
                        {
                            UIMotion motion = new UIMotion(transformRoot, frame.motion);
                            motions.Add(motion);

                            if (prevMotion != null)
                            {
                                prevMotion.EffectStopped -= OnEffectStopped;
                            }
                            motion.EffectStopped += OnEffectStopped;
                            motionStartTimes.Add(motion, timeLineForChildren.GetTimeFromFrameIndex(frame.header.index));

                            lastMotion = motion;
                            prevMotion = motion;
                        }
                        else
                        {
                            prevMotion = null;
                        }
                        
                        instanceName = instance.name;
                        
                        timeLineForChildren.Repeating = instance.header.repeating;

                        uint lastIndex = frame.header.index + frame.header.duration;
                        if (duration < lastIndex)
                        {
                            duration = lastIndex;
                        }
                    }

                    if (lastMotion != null)
                    {
                        lastMotion.EffectStopped -= OnEffectStopped;
                    }
     
                    initialTransform3Droot = this.Transform3D;
                    initialTransform3Dtroot = transformRoot.Transform3D;

                    return duration;
                }
                
                public bool ReplaceWidget(string name, Widget widget)
                {
                    if (instanceName.Equals(name))
                    {
                        float childLeft = GetChildrenLeft();
                        float childRight = GetChildrenRight();
                        float childTop = GetChildrenTop();
                        float childBottom = GetChildrenBottom();
                        widget.SetPosition(childLeft, childTop);
                        widget.SetSize(childRight - childLeft, childBottom - childTop);
                        
                        foreach (var child in childLayers)
                        {
                            transformRoot.RemoveChild(child);
                        }
                        childLayers.Clear();
                        
                        transformRoot.AddChildLast(widget);
                        return true;
                    }
     
                    bool ret = false;
                    foreach (var layer in childLayers)
                    {
                        ret = layer.ReplaceWidget(name, widget);
                        if (ret)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                
                private float GetChildrenLeft()
                {
                    float minLeft = float.MaxValue;
                    foreach (var child in transformRoot.Children)
                    {
                        float childLeft = child.X;
                        if (child is Layer)
                        {
                            childLeft += (child as Layer).GetChildrenLeft();
                        }
                        if (childLeft < minLeft)
                        {
                            minLeft = childLeft;
                        }
                    }
                    return minLeft;
                }

                private float GetChildrenRight()
                {
                    float maxRight  = -float.MaxValue;
                    foreach (var child in transformRoot.Children)
                    {
                        float childRight = child.X + child.Width;
                        if (child is Layer)
                        {
                            childRight += (child as Layer).GetChildrenRight();
                        }
                        if (maxRight < childRight)
                        {
                            maxRight = childRight;
                        }
                    }
                    return maxRight;
                }

                private float GetChildrenTop()
                {
                    float minTop = float.MaxValue;
                    foreach (var child in transformRoot.Children)
                    {
                        float childTop = child.Y;
                        if (child is Layer)
                        {
                            childTop += (child as Layer).GetChildrenTop();
                        }
                        if (childTop < minTop)
                        {
                            minTop = childTop;
                        }
                    }
                    return minTop;
                }

                private float GetChildrenBottom()
                {
                    float maxBottom  = -float.MaxValue;
                    foreach (var child in transformRoot.Children)
                    {
                        float childBottom = child.Y + child.Height;
                        if (child is Layer)
                        {
                            childBottom += (child as Layer).GetChildrenBottom();
                        }
                        if (maxBottom < childBottom)
                        {
                            maxBottom = childBottom;
                        }
                    }
                    return maxBottom;
                }

                private ImageBox CreateBitmap(ImageAsset imageAsset)
                {
                    ImageBox imageBox = new ImageBox();
                    imageBox.Image = imageAsset;
                    imageBox.Width = imageAsset.Width;
                    imageBox.Height = imageAsset.Height;
                    return imageBox;
                }

                public void Start(float offset)
                {
                    Stop();
                    timeLineForChildren.Start(offset);
                }

                public void Stop()
                {
                    timeLineForChildren.Stop();
                    
                    foreach (var motion in motions)
                    {
                        motion.Stop();
                    }

                    foreach (var layer in childLayers)
                    {
                        layer.Stop();
                    }
                    
                    startedMotions.Clear();
                    transformRoot.Transform3D = initialTransform3Dtroot;
                }

                public void Pause()
                {
                    timeLineForChildren.Pause();

                    foreach (var motion in motions)
                    {
                        motion.Pause();
                    }

                    foreach (var layer in childLayers)
                    {
                        layer.Pause();
                    }
                }

                public void Resume()
                {
                    timeLineForChildren.Resume();

                    foreach (var motion in motions)
                    {
                        motion.Resume();
                    }

                    foreach (var layer in childLayers)
                    {
                        layer.Resume();
                    }
                }

                public bool Playing
                {
                    get
                    {
                        return timeLineForChildren.Playing;
                    }
                }

                public bool Paused
                {
                    get
                    {
                        return timeLineForChildren.Paused;
                    }
                }

                public bool Repeating
                {
                    get
                    {
                        return timeLineForChildren.Repeating;
                    }
                    set
                    {
                        timeLineForChildren.Repeating = value;
                    }
                }

                private void OnEffectStopped(object sender, EventArgs e)
                {
                    if (sender is Effect)
                    {
                        Effect motion = sender as Effect;
                        motion.Widget.Visible = false;
                    }
                }

                private void OnTimeLineRepeated(object sender, EventArgs e)
                {
                    if (sender is TimeLine)
                    {
                        foreach (var layer in childLayers)
                        {
                            layer.Start((sender as TimeLine).CurrentTime);
                        }
                    }
                }

                private void OnTimeLineStoped(object sender, EventArgs e)
                {
                    if (sender is TimeLine)
                    {
                        if (AnimationStopped != null)
                        {
                            AnimationStopped(this, EventArgs.Empty);
                        }
                    }
                }
                
                public event EventHandler<EventArgs> AnimationStopped;
                public TimeLine timeLineForChildren;
                public ContainerWidget transformRoot;
                public List<Layer> childLayers;
                public string instanceName = "";

                private List<Effect> motions;
                private List<Effect> startedMotions;
                private Dictionary<Effect, float> motionStartTimes;
                private Matrix4 initialTransform3Dtroot;
                private Matrix4 initialTransform3Droot;
            }

            internal class TimeLine
            {
                public TimeLine()
                {
                    this.CurrentTime = 0;
                    this.Repeating = false;
                    this.Playing = false;
                    this.Paused = false;
                    this.labels = new Dictionary<string, uint>();
                }

                public void OnUpdate(float elapsedTime)
                {
                    if (Playing)
                    {
                        CurrentTime += elapsedTime;

                        if (CurrentTime > durationTime)
                        {
                            if (Repeating)
                            {
                                CurrentTime -= durationTime;
                                if (TimeLineRepeated != null)
                                {
                                    TimeLineRepeated(this, EventArgs.Empty);
                                }
                            }
                            else
                            {
                                Playing = false;
                                Paused = false;
                                if (TimeLineStopped != null)
                                {
                                    TimeLineStopped(this, EventArgs.Empty);
                                }
                            }
                        }
                    }
                }
                
                public uint Duration
                {
                    get
                    {
                        return this.durationFrame;
                    }
                    set
                    {
                        this.durationFrame = value;
                        this.durationTime = value * this.frameToTime;
                    }
                }
                
                public uint FrameRate
                {
                    get
                    {
                        return this.frameRate;
                    }
                    set
                    {
                        this.frameRate = value;
                        this.frameToTime = 1000.0f / value;
                    }
                }
                
                public void Reset()
                {
                    CurrentTime = 0;
                    Playing = false;
                    Paused = false;
                }

                public void Start(float offsetTime)
                {
                    CurrentTime = offsetTime;
                    Playing = true;
                    Paused = false;
                }

                public void Stop()
                {
                    if (TimeLineStopped != null)
                    {
                        TimeLineStopped(this, EventArgs.Empty);
                    }
                    Playing = false;
                    Paused = false;
                }

                public void Pause()
                {
                    if (Playing)
                    {
                        Playing = false;
                        Paused = true;
                    }
                }

                public void Resume()
                {
                    if (Paused)
                    {
                        Playing = true;
                        Paused = false;
                    }
                }

                public void SetLabel(string labelName, uint frameIndex)
                {
                    if (labels.ContainsKey(labelName))
                    {
                        labels.Remove(labelName);
                    }
                    labels.Add(labelName, frameIndex);
                }

                public void Goto(string labelName)
                {
                    if (labels.ContainsKey(labelName))
                    {
                        CurrentFrame = labels[labelName];
                    }
                }

                public float GetTimeFromFrameIndex(uint frameIndex)
                {
                    return (float)frameIndex * frameToTime;
                }

                public float CurrentTime
                {
                    get;
                    private set;
                }

                public uint CurrentFrame
                {
                    get
                    {
                        return (uint)((CurrentTime / durationTime) * durationFrame);
                    }
                    private set
                    {
                        CurrentTime = ((float)value / (float)durationFrame) * durationTime;
                    }
                }

                public bool Playing
                {
                    get;
                    private set;
                }

                public bool Paused
                {
                    get;
                    private set;
                }

                public bool Repeating
                {
                    get;
                    set;
                }

                public event EventHandler<EventArgs> TimeLineRepeated;
                public event EventHandler<EventArgs> TimeLineStopped;

                private Dictionary<String, uint> labels;
                private uint frameRate;
                private uint durationFrame;
                private float durationTime;
                private float frameToTime;
            }

            internal class UIAnimationData
            {
                private string SIGNATURE = "_uia";
                private string VERSION = "0001";
                
                public Header header;
                public List<Bitmap> bitmaps = new List<Bitmap>();
                public List<Symbol> symbols = new List<Symbol>();
                public Symbol rootSymbol = new Symbol();
               
                public struct Header
                {
                    public uint width;
                    public uint height;
                    public uint backgroundColorR;
                    public uint backgroundColorG;
                    public uint backgroundColorB;
                    public uint frameRate;
                    public bool repeating;
                    public uint bitmapNum;
                    public uint symbolNum;
                }
        
                public class Bitmap
                {
                    public struct BitmapHeader
                    {
                        public uint width;
                        public uint height;
                        public uint byteSize;
                    }
        
                    public BitmapHeader header;
                    public byte[] rawData;
        
                    public void Read(FileStream fs)
                    {
                        header = (BitmapHeader)ReadObject(fs, typeof(BitmapHeader));
                        rawData = ReadRawImageFromFileStream(fs, (int)header.byteSize);
                    }
                }

                public class Symbol
                {
                    public struct SymbolHeader
                    {
                        public uint timeLineNum;
                    }
        
                    SymbolHeader header;
                    public List<TimeLine> timeLines = new List<TimeLine>();
        
                    public void Read(FileStream fs)
                    {
                        header = (SymbolHeader)ReadObject(fs, typeof(SymbolHeader));
        
                        for (int i = 0; i < header.timeLineNum; i++)
                        {
                            TimeLine timeLine = new TimeLine();
                            timeLine.Read(fs);
                            timeLines.Add(timeLine);
                        }
                    }            
                }

                public class TimeLine
                {
                    public struct TimeLienHeader
                    {
                        public uint layerNum;
                    }
        
                    public TimeLienHeader header;
                    public List<Layer> layers = new List<Layer>();
        
                    public void Read(FileStream fs)
                    {
                        header = (TimeLienHeader)ReadObject(fs, typeof(TimeLienHeader));
        
                        for (int i = 0; i < header.layerNum; i++)
                        {
                            Layer layer = new Layer();
                            layer.Read(fs);
                            layers.Add(layer);
                        }
                    }
                }

                public class Layer
                {
                    public struct LayerHeader
                    {
                        public uint frameNum;
                    }
        
                    public LayerHeader header;
                    public List<Frame> frames = new List<Frame>();
        
                    public void Read(FileStream fs)
                    {
                        header = (LayerHeader)ReadObject(fs, typeof(LayerHeader));
        
                        for (int i = 0; i < header.frameNum; i++)
                        {
                            Frame frame = new Frame();
                            frame.Read(fs);
                            frames.Add(frame);
                        }
                    }
                }

                public class Frame
                {
                    public struct FrameHeader
                    {
                        public uint index;
                        public uint duration;
                        public bool hasMotion;
                        public bool hasInstance;
                        public uint nameByteSize;
                    }
        
                    public FrameHeader header = new FrameHeader();
                    public string name;
                    public UIMotionData motion;
                    public Instance instance;
        
                    public Frame()
                    {
                        header.duration = 0;
                        header.hasMotion = false;
                        header.hasInstance = false;
                    }
        
                    public void Read(FileStream fs)
                    {
                        header = (FrameHeader)ReadObject(fs, typeof(FrameHeader));
                        
                        if (header.nameByteSize > 0)
                        {
                            byte[] buf = new byte[header.nameByteSize];
                            if (fs.Read(buf, 0, buf.Length) == buf.Length)
                            {
                                name = Encoding.Unicode.GetString(buf);
                            }
                        }

                        if (header.hasMotion)
                        {
                            motion = new UIMotionData();
                            motion.Read(fs);
                        }
                        
                        if (header.hasInstance)
                        {
                            instance = new Instance();
                            instance.Read(fs);
                        }
                    }
                }

                public class Instance
                {
                    public struct InstanceHeader
                    {
                        public bool hasSymbol;
                        public uint symbolId;
                        public bool hasBitmap;
                        public uint bitmapId;
                        public bool repeating;
                        public float transPointX;
                        public float transPointY;
                        public float translationX;
                        public float translationY;
                        public float scaleX;
                        public float scaleY;
                        public float rotationX;
                        public float rotationY;
                        public float rotationZ;
                        public float centerPointX;
                        public float centerPointY;
                        public float centerPointZ;
                        public float matrix11;
                        public float matrix12;
                        public float matrix13;
                        public float matrix14;
                        public float matrix21;
                        public float matrix22;
                        public float matrix23;
                        public float matrix24;
                        public float matrix31;
                        public float matrix32;
                        public float matrix33;
                        public float matrix34;
                        public float matrix41;
                        public float matrix42;
                        public float matrix43;
                        public float matrix44;
                        public uint nameByteSize;
                    }
        
                    public InstanceHeader header;
                    public string name = "";
        
                    public Instance()
                    {
                        header.hasSymbol = false;
                        header.hasBitmap = false;
                        header.repeating = true;
                    }
        
                    public void Read(FileStream fs)
                    {
                        header = (InstanceHeader)ReadObject(fs, typeof(InstanceHeader));
                        if (header.nameByteSize > 0)
                        {
                            byte[] buf = new byte[header.nameByteSize];
                            if (fs.Read(buf, 0, buf.Length) == buf.Length)
                            {
                                name = Encoding.Unicode.GetString(buf);
                            }
                        }
                    }
                }

                public void ReadFromFile(String fileName)
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    if (fs != null)
                    {
                        CheckSignatureAndVersion(fs);
                        ReadHeader(fs);
                        ReadBitmaps(fs);
                        ReadSymbols(fs);
                        
                        rootSymbol = new Symbol();
                        rootSymbol.Read(fs);
                        fs.Close();
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
                
                private void ReadBitmaps(FileStream fs)
                {
                    for (int i = 0; i < header.bitmapNum; i++)
                    {
                        Bitmap bitmap = new Bitmap();
                        bitmap.Read(fs);
                        bitmaps.Add(bitmap);
                    }
                }
                
                private void ReadSymbols(FileStream fs)
                {
                    for (int i = 0; i < header.symbolNum; i++)
                    {
                        Symbol symbol = new Symbol();
                        symbol.Read(fs);
                        symbols.Add(symbol);
                    }
                }
                
                private static byte[] ReadRawImageFromFileStream(FileStream fs, int size)
                {
                    byte[] buf = new byte[size];
                    if (fs.Read(buf, 0, buf.Length) == buf.Length)
                    {
                        return buf;
                    }
                    return null;
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
                        case "Boolean":
                            buf = new byte[4];
                            if (fs.Read(buf, 0, buf.Length) == buf.Length)
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
}


