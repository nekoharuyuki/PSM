/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
//using System.Xml;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;

namespace Sce.PlayStation.HighLevel.UI
{

    /// @if LANG_JA
    /// <summary>UI Toolkit のデバッグに使用するユーティリティクラス</summary>
    /// <remarks>このクラスのほとんどのメソッドは、条件付きコンパイル シンボルに "DEBUG" が定義されている場合にのみ動作します。</remarks>
    /// @endif
    /// @if LANG_EN
    /// <summary>Utility class used in UI Toolkit debugging</summary>
    /// <remarks>Most methods of this class operate only when "DEBUG" is defined in a conditional compilation symbol.</remarks>
    /// @endif
    public static class UIDebug
    {
        #region Assert

        /// @if LANG_JA
        /// <summary>アサーション失敗のメッセージとコールスタックをコンソール画面に出力する</summary>
        /// <remarks>System.Diagnostics.Debug.Assert が動作しないため、その代替のメソッドです。
        /// UIライブラリのコンパイルシンボルに"BREAK_UIDEBUG_ASSERT"を定義すると、デバッグ時に中断しますが、デバッグなし実行した場合に実行を継続できなくなります。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Outputs assertion failure messages and call stacks to the console screen</summary>
        /// <remarks>System.Diagnostics.Debug.Assert does not operate, so this is an alternative method.
        /// When "BREAK_UIDEBUG_ASSERT" is defined to the compilation symbol of the UI library, operation is interrupted during debugging, but execution cannot be continued when executed without debugging.</remarks>
        /// @endif
        static void assertLog(string message, string detailMessage)
        {
            Console.WriteLine("[UIDebug Assertion Failed] " + message);
            if (!string.IsNullOrWhiteSpace(detailMessage))
                Console.WriteLine("  detail: " + detailMessage);

            // can not use StacTrace.ctor(int), StacTrace.ctor(int, bool)...
#if false
            try {
                var st = new StacTrace(2);
                Console.WriteLine(st.ToString());
            } catch (Exception) { }
#else
            Console.WriteLine("  trace:");
            for (int i = 2; i < 100; i++)
            {
                try
                {
                    var sf = new StackFrame(i);
                    if (sf == null) break;
                    var method = sf.GetMethod();
                    if (method == null) break;
                    Console.WriteLine("    {0}  offset: {1}", new StackTrace(sf), sf.GetILOffset());
                }
                catch (Exception) { }
            }
#endif

#if BREAK_UIDEBUG_ASSERT
            System.Diagnostics.Debugger.Break();
#endif
        }

        /// @if LANG_JA
        /// <summary>条件がfalseの場合、コンソール画面にアサーションの失敗のメッセージを出力する</summary>
        /// <remarks>"DEBUG" コンパイルシンボルが定義されていない場合は、このメソッドの呼び出しは無視されます。</remarks>
        /// <param name="condition">条件</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>When the condition is false, an assertion failure message is output to the console screen</summary>
        /// <remarks>If the "DEBUG" compilation symbol is not defined, the calling of this method is ignored.</remarks>
        /// <param name="condition">Condition</param>
        /// @endif
        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            if (!condition)
                assertLog(null, null);
        }

        /// @if LANG_JA
        /// <summary>条件がfalseの場合、コンソール画面にアサーションの失敗のメッセージを出力する</summary>
        /// <remarks>"DEBUG" コンパイルシンボルが定義されていない場合は、このメソッドの呼び出しは無視されます。</remarks>
        /// <param name="condition">条件</param>
        /// <param name="message">メッセージ</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>When the condition is false, an assertion failure message is output to the console screen</summary>
        /// <remarks>If the "DEBUG" compilation symbol is not defined, the calling of this method is ignored.</remarks>
        /// <param name="condition">Condition</param>
        /// <param name="message">Message</param>
        /// @endif
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
                assertLog(message, null);
        }

        /// @if LANG_JA
        /// <summary>条件がfalseの場合、コンソール画面にアサーションの失敗のメッセージを出力する</summary>
        /// <remarks>"DEBUG" コンパイルシンボルが定義されていない場合は、このメソッドの呼び出しは無視されます。</remarks>
        /// <param name="condition">条件</param>
        /// <param name="message">メッセージ</param>
        /// <param name="detailMessage">詳細なメッセージ</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>When the condition is false, an assertion failure message is output to the console screen</summary>
        /// <remarks>If the "DEBUG" compilation symbol is not defined, the calling of this method is ignored.</remarks>
        /// <param name="condition">Condition</param>
        /// <param name="message">Message</param>
        /// <param name="detailMessage">Detailed message</param>
        /// @endif
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message, string detailMessage)
        {
            if (!condition)
                assertLog(message, detailMessage);
        }

        /// @if LANG_JA
        /// <summary>条件がfalseの場合、コンソール画面にアサーションの失敗のメッセージを出力する</summary>
        /// <remarks>"DEBUG" コンパイルシンボルが定義されていない場合は、このメソッドの呼び出しは無視されます。</remarks>
        /// <param name="condition">条件</param>
        /// <param name="message">メッセージ</param>
        /// <param name="detailMessageFormat">詳細なメッセージの書式文字列</param>
        /// <param name="args">>詳細なメッセージの書式パラメータ</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>When the condition is false, an assertion failure message is output to the console screen</summary>
        /// <remarks>If the "DEBUG" compilation symbol is not defined, the calling of this method is ignored.</remarks>
        /// <param name="condition">Condition</param>
        /// <param name="message">Message</param>
        /// <param name="detailMessageFormat">Format string of detailed message</param>
        /// <param name="args">&gt;Format parameter of detailed message</param>
        /// @endif
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message, string detailMessageFormat, params Object[] args)
        {
            if (!condition)
                assertLog(message, string.Format(detailMessageFormat, args));
        }

        #endregion


        #region ExportImage

        private static string imageExportAlbumName = "PSM_UIDebug";
        private static List<WeakReference> uncachedTextures = null;
        private static int uncachedWholeCount = 0;


        /// @if LANG_JA
        /// <summary>ImageAssetをExportする際のアルバム名を設定、取得する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets and obtains the album name when exporting ImageAsset</summary>
        /// @endif
        public static string ImageExportAlbumName
        {
            get { return imageExportAlbumName; }
            set { imageExportAlbumName = value; }
        }

        /// @if LANG_JA
        /// <summary>キャッシュされているImageAssetをすべて画像ファイルに出力します。</summary>
        /// <remarks>"DEBUG" コンパイルシンボルが定義されていない場合は、このメソッドの呼び出しは無視されます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Outputs all cached ImageAsset to image files.</summary>
        /// <remarks>If the "DEBUG" compilation symbol is not defined, the calling of this method is ignored.</remarks>
        /// @endif
        [Conditional("DEBUG")]
        public static void ExportAllImageAsset()
        {
            float scale = 1.0f;
            int count = 0;
            int failCount = 0;

            Console.WriteLine("[UIDebug] Start to export all image assets.");
            var stopwatch = Stopwatch.StartNew();

            // cached ImageAssets
            foreach (var keyValue in AssetManager.cache)
            {
                string filename = keyValue.Key;
                filename = filename.Replace('/', '_');
                filename = String.Format("{1}_{0:0000}.png", count, filename);

                Console.WriteLine("  export: " + filename);

                var texture = keyValue.Value;
                var result = ExportTexture(texture, filename, scale);
                count++;
                if (!result) failCount++;
            }

            // uncached ImageAssets
            if (uncachedTextures != null)
            {
                foreach (var weak in uncachedTextures)
                {
                    var imageAsset = weak.Target as ImageAsset;
                    if (imageAsset != null)
                    {
                        var texture = imageAsset.CloneTexture();
                        // if imageAsset was disposed then texture is null
                        if (texture != null)
                        {
                            string filename = String.Format("uncached_{0:0000}.png", count);

                            Console.WriteLine("  export: " + filename);

                            var result = ExportTexture(texture, filename, scale);
                            count++;
                            if (!result) failCount++;
                        }
                    }
                }
            }

            Console.WriteLine("[UIDebug] Exported success:{1} failed:{2} {0}ms", stopwatch.ElapsedMilliseconds, count - failCount, failCount);
        }

        /// @if LANG_JA
        /// <summary>キャッシュされないImageAssetもExportAllImageAssetで出力できるようにする</summary>
        /// <remarks>このメソッドはUISystem.Initializeを呼び出す前に呼び出すことができます。
        /// "DEBUG" コンパイルシンボルが定義されていない場合は、このメソッドの呼び出しは無視されます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>ImageAsset not cached can also be output with ExportAllImageAsset</summary>
        /// <remarks>This method can be called before calling UISystem.Initialize.
        /// If the "DEBUG" compilation symbol is not defined, the calling of this method is ignored.</remarks>
        /// @endif
        [Conditional("DEBUG")]
        public static void EnableRecordUncachedTexture()
        {
            if (uncachedTextures == null)
            {
                uncachedTextures = new List<WeakReference>(128);
            }
        }

        internal static void recordUncachedImageAsset(ImageAsset imageAsset)
        {
            if (uncachedTextures != null)
            {
                // compaction at 32 intervals 
                if (((++uncachedWholeCount) & 0x1f) == 0)
                {
                    for (int i = uncachedTextures.Count - 1; i >= 0; i--)
                    {
                        if (!uncachedTextures[i].IsAlive)
                            uncachedTextures.RemoveAt(i);
                    }
                }
                uncachedTextures.Add(new WeakReference(imageAsset));
            }
        }

        /// @if LANG_JA
        /// <summary>ImageAssetを画像ファイルに出力する</summary>
        /// <remarks>"DEBUG" コンパイルシンボルが定義されていない場合は、このメソッドの呼び出しは無視されます。</remarks>
        /// <param name="imageAsset">出力するImageAsset</param>
        /// <param name="filename">ファイル名</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Outputs ImageAsset to an image file</summary>
        /// <remarks>If the "DEBUG" compilation symbol is not defined, the calling of this method is ignored.</remarks>
        /// <param name="imageAsset">Output ImageAsset</param>
        /// <param name="filename">Filename</param>
        /// @endif
        [Conditional("DEBUG")]
        public static void ExportImageAsset(ImageAsset imageAsset, string filename)
        {
            // check
            if (imageAsset == null)
            {
                UIDebug.Assert(false);
                Console.WriteLine("[UIDebug TextureExport Error] imageAsset is null.  filename=" + filename);
                return;
            }

            var texture = imageAsset.CloneTexture();
            if (texture == null)
            {
                Console.WriteLine("[UIDebug TextureExport Error] imageAsset is not load yet.  filename=" + filename);
                return;
            }

            var result = ExportTexture(texture, filename, 1.0f);
            texture.Dispose();

            if (result)
                Console.WriteLine("[UIDebug] Success ExportImageAsset filename=" + filename);
        }

        internal static bool ExportTexture(Texture2D texture, string filename, float scale)
        {
            string albumname = imageExportAlbumName;
            Vector4 clearColor = new Vector4(0f, 0f, 0f, 1f);

            Vector4 vertexColor;
            if (texture != null && texture.Format == PixelFormat.Alpha)
                vertexColor = new Vector4(1f, 0f, 0f, 1f);
            else
                vertexColor = new Vector4(1f, 1f, 1f, 1f);

            // check
            if (texture == null)
            {
                UIDebug.Assert(false);
                Console.WriteLine("[UIDebug TextureExport Error] texture is null.  filename=" + filename);
                return false;
            }
            if (texture.Width <= 0 || texture.Height <= 0)
            {
                UIDebug.Assert(false);
                Console.WriteLine("[UIDebug TextureExport Error] texture size is zero.  filename=" + filename);
                return false;
            }

            UIDebug.Assert(texture.Format == PixelFormat.Rgba || texture.Format == PixelFormat.Alpha,
                "[UIDebug TextureExport Warning] not support texture format '" + texture.Format + "'.  filename=" + filename);


            int width = texture.Width;
            int height = texture.Height;
            if (scale < 1.0f)
            {
                width = (int)(width * scale);
                height = (int)(height * scale);
            }
            if (width < 1) width = 1;
            if (height < 1) height = 1;
            GraphicsContext graphics = UISystem.GraphicsContext;


            FrameBuffer frameBuffer = null;
            Image image = null;

            if (texture.IsRenderable)
            {
                try
                {
                    frameBuffer = new FrameBuffer();
                    frameBuffer.SetColorTarget(texture, 0);
                    graphics.SetFrameBuffer(frameBuffer);
                    byte[] data = new byte[width * height * 4];
                    UISystem.GraphicsContext.ReadPixels(data, PixelFormat.Rgba, 0, 0, texture.Width, texture.Height);
                    image = new Image(ImageMode.Rgba, new ImageSize(texture.Width, texture.Height), data);
                    if (scale < 1.0f)
                    {
                        var temp = image.Resize(new ImageSize(width, height));
                        image.Dispose();
                        image = temp;
                    }
                    image.Export(albumname, filename);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[UIDebug TextureExport Error] {0} has been thrown. filename={1}", ex.GetType().Name, filename);
                    Console.WriteLine(ex.ToString());
                    return false;
                }
                finally
                {
                    graphics.SetFrameBuffer(null);
                    if (frameBuffer != null) frameBuffer.Dispose();
                    if (image != null) image.Dispose();
                }
            }
            else
            {
                // not rentarable texture can not be read pixels
                // so temporarily render to color buffer
                ColorBuffer colorBuffer = null;
                try
                {
                    // FrameBuffer
                    frameBuffer = new FrameBuffer();
                    colorBuffer = new ColorBuffer(width, height, PixelFormat.Rgba);
                    frameBuffer.SetColorTarget(colorBuffer);
                    graphics.SetFrameBuffer(frameBuffer);

                    // setup graphics state
                    graphics.SetViewport(0, 0, width, height);
                    graphics.Disable(EnableMode.ScissorTest);
                    graphics.Disable(EnableMode.Blend);
                    graphics.Disable(EnableMode.CullFace);
                    graphics.SetColorMask(ColorMask.Rgba);
                    graphics.SetClearColor(clearColor);

                    graphics.Clear();

                    // setup texture
                    texture.SetFilter(TextureFilterMode.Nearest);
                    graphics.SetTexture(0, texture);

                    // setup shader
                    var shaderType = (texture.Format == PixelFormat.Alpha) ? ShaderType.TextTexture : ShaderType.Texture;
                    var shader = UISystem.GetShaderProgram(shaderType);
                    graphics.SetShaderProgram(shader);
                    var mat = Matrix4.Identity;
                    shader.SetUniformValue(shader.FindUniform("u_WorldMatrix"), ref mat);
                    shader.SetUniformValue(shader.FindUniform("u_Alpha"), 1.0f);

                    // setup vertex
                    VertexBuffer vertexBuffer = new VertexBuffer(4, VertexFormat.Float3, VertexFormat.Float4, VertexFormat.Float2);
                    // position
                    vertexBuffer.SetVertices(0, new float[] {
                        -1f, -1f,  0f,
                        -1f,  1f,  0f,
                         1f, -1f,  0f,
                         1f,  1f,  0f,
                    });
                    // color
                    vertexBuffer.SetVertices(1, new float[] {
                        vertexColor.R, vertexColor.G, vertexColor.B, vertexColor.A,
                        vertexColor.R, vertexColor.G, vertexColor.B, vertexColor.A,
                        vertexColor.R, vertexColor.G, vertexColor.B, vertexColor.A,
                        vertexColor.R, vertexColor.G, vertexColor.B, vertexColor.A,
                    });
                    // texcoord
                    vertexBuffer.SetVertices(2, new float[] {
                        0.0f, 0.0f,
                        0.0f, 1.0f,
                        1.0f, 0.0f,
                        1.0f, 1.0f,
                    });
                    graphics.SetVertexBuffer(0, vertexBuffer);

                    // draw
                    graphics.DrawArrays(DrawMode.TriangleStrip, 0, 4);

                    // make image
                    byte[] data = new byte[width * height * 4];
                    UISystem.GraphicsContext.ReadPixels(data, PixelFormat.Rgba, 0, 0, width, height);
                    image = new Image(ImageMode.Rgba, new ImageSize(width, height), data);

                    image.Export(albumname, filename);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("[UIDebug TextureExport Error] {0} has been thrown. filename={1}", ex.GetType().Name, filename);
                    Console.WriteLine(ex.ToString());
                    return false;
                }
                finally
                {
                    // restore graphics state and termination
                    graphics.SetFrameBuffer(null);
                    graphics.SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height);
                    graphics.Enable(EnableMode.ScissorTest);
                    graphics.Enable(EnableMode.Blend);
                    graphics.Enable(EnableMode.CullFace);
                    graphics.SetFrameBuffer(null);
                    if (frameBuffer != null) frameBuffer.Dispose();
                    if (colorBuffer != null) colorBuffer.Dispose();
                    if (image != null) image.Dispose();
                }
            }

            return true;
        }

        
        /// @internal
        /// @if LANG_JA
        /// <summary>現在のフレームバッファを画像ファイルに出力する</summary>
        /// <remarks>"DEBUG" コンパイルシンボルが定義されていない場合は、このメソッドの呼び出しは無視されます。</remarks>
        /// <param name="filename">ファイル名</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Output the current frame buffer to the image file</summary>
        /// <remarks>If the "DEBUG" compilation symbol is not defined, the calling of this method is ignored.</remarks>
        /// <param name="filename">Filename</param>
        /// @endif
        [Conditional("DEBUG")]
        internal static void ExportFrameBuffer(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                filename = string.Format("framebuffer_{0:MMddHHmmss}.png", DateTime.Now);
            }
            bool result = ExportFrameBuffer(filename,
                UISystem.GraphicsContext.GetFrameBuffer() == UISystem.GraphicsContext.Screen);
            Console.WriteLine("[UIDebug] Success ExportFrameBuffer filename=" + filename);
        }

        internal static bool ExportFrameBuffer(string filename, bool flipVertical)
        {
            Image image = null;
            try
            {
                var fb = UISystem.GraphicsContext.GetFrameBuffer();
                byte[] data = new byte[fb.Width * fb.Height * 4];
                UISystem.GraphicsContext.ReadPixels(data, PixelFormat.Rgba, 0, 0, fb.Width, fb.Height);
                //SaveBitmap("/Documents/" + filename + ".bmp", data, fb.Width, fb.Height, !flipVertical);
                for (int i = 3; i < data.Length; i += 4) data[i] = 0xff;
                if (flipVertical)
                {
                    var line = new byte[fb.Width * 4];
                    for (int i = 0; i < fb.Height / 2; i++)
                    {
                        Array.Copy(data, i * line.Length, line, 0, line.Length);
                        Array.Copy(data, (fb.Height - i - 1) * line.Length, data, i * line.Length, line.Length);
                        Array.Copy(line, 0, data, (fb.Height - i - 1) * line.Length, line.Length);
                    }
                }
                image = new Image(ImageMode.Rgba, new ImageSize(fb.Width, fb.Height), data);
                image.Export(imageExportAlbumName, filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UIDebug ExportFrameBuffer Error] {0} has been thrown. filename={1}",
                                  ex.GetType().Name, filename);
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                if (image != null)
                    image.Dispose();
            }

            return true;
        }

        #endregion


        #region DumpSceneGraph

        readonly static string uiNamespace = typeof(UISystem).Namespace;
        static int propertyNestCount = 0;

        /// @if LANG_JA
        /// <summary>現在のシーンのウィジェットと描画要素のツリー、および各要素のプロパティをXMLとしてダンプする</summary>
        /// <remarks>filenameにnullを指定した場合はコンソール画面に出力します。
        /// 出力されるXMLのフォーマットは暫定です。また、すべてのプロパティメンバが出力されるわけではありません。
        /// "DEBUG" コンパイルシンボルが定義されていない場合は、このメソッドの呼び出しは無視されます。</remarks>
        /// <param name="filename">出力するファイル名(nullの場合はコンソール画面に出力)</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Dumps the current scene widget and rendering element tree and the properties of each element as XML</summary>
        /// <remarks>Outputs to the console screen when null is specified for filename.
        /// The output XML format is tentative. Also, not all property members are output.
        /// If the "DEBUG" compilation symbol is not defined, the calling of this method is ignored.</remarks>
        /// <param name="filename">Output filename (output to console screen when null)</param>
        /// @endif
        [Conditional("DEBUG")]
        public static void DumpSceneGraph(string filename)
        {
            dumpSceneGraph(filename, false);
        }

        internal static void dumpSceneGraph(string filename, bool includeNonPublic)
        {
            Console.WriteLine("[UIDebug] Start to dump.");
            var stopwatch = Stopwatch.StartNew();

            WidgetDumpWriter writer = null;
            try
            {
                writer = new WidgetDumpWriter(filename);
                writer.IsOutputNonPublic = includeNonPublic;
                writer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }

            Console.WriteLine("[UIDebug] end dump {0}ms", stopwatch.ElapsedMilliseconds);
        }

        private class WidgetDumpWriter : IDisposable
        {
            XmlTextWriter writer = null;
            public bool IsOutputNonPublic = false;

            public WidgetDumpWriter(string filename)
            {
                if (String.IsNullOrEmpty(filename))
                {
                    // output to console
                    this.writer = new XmlTextWriter(new ConsoleWriter());
                }
                else
                {
                    this.writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
                }
            }
            public void Dispose()
            {
                if (writer != null)
                    writer.Close();
            }

            public void Start()
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("UISystem");

                // scene layer
                foreach (var scene in UISystem.sceneLayers)
                {
                    if (scene != null)
                    {
                        Type sceneType = scene.GetType();
                        writer.WriteStartElement(convertNameString(sceneType.Name));

                        // scene prop
                        writer.WriteStartElement("Property");
                        dumpProperty(scene, sceneType);
                        writer.WriteEndElement();

                        // widget tree
                        dumpWidget(scene.RootWidget);

                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            private void dumpWidget(Widget widget)
            {
                Type type = widget.GetType();
                writer.WriteStartElement(convertNameString(type.Name));

                // property
                writer.WriteStartElement("Property");
                dumpProperty(widget, type);
                writer.WriteEndElement();

                // UIElement
                dumpElement(widget.RootUIElement);

                // child node
                var childNode = widget.LinkedTree.FirstChild;
                if (childNode != null)
                {
                    writer.WriteStartElement("Children");
                    for (; childNode != null; childNode = childNode.NextSibling)
                    {
                        dumpWidget(childNode.Value);
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            private void dumpElement(UIElement element)
            {
                Type type = element.GetType();
                writer.WriteStartElement(convertNameString(type.Name));

                // property
                writer.WriteStartElement("Property");
                dumpProperty(element, type);
                writer.WriteEndElement();

                // UISpriteUnits
                if (type == typeof(UISprite) || type.IsSubclassOf(typeof(UISprite)))
                {
                    var sprite = element as UISprite;

                    writer.WriteStartElement("UISpriteUnits");
                    for (int i = 0; i < sprite.UnitCount; i++)
                    {
                        var unit = sprite.GetUnit(i);
                        writer.WriteStartElement("UISpriteUnit");
                        dumpProperty(unit, typeof(UISpriteUnit));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                // UIPrimitiveVertices
                if (type == typeof(UIPrimitive) || type.IsSubclassOf(typeof(UIPrimitive)))
                {
                    var primitive = element as UIPrimitive;

                    writer.WriteStartElement("UIPrimitiveVertices");
                    for (int i = 0; i < primitive.VertexCount; i++)
                    {
                        var vertex = primitive.GetVertex(i);
                        writer.WriteStartElement("UIPrimitiveVertex");
                        dumpProperty(vertex, typeof(UIPrimitiveVertex));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                // child node
                var childNode = element.linkedTree.FirstChild;
                if (childNode != null)
                {
                    writer.WriteStartElement("Children");
                    for (; childNode != null; childNode = childNode.NextSibling)
                    {
                        dumpElement(childNode.Value);
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            private void dumpProperty(object obj, Type parentType)
            {
                //if (parentType.Namespace != uiNamespace)
                //{
                //    writer.WriteStartAttribute("Namespace");
                //    writer.WriteString(parentType.Namespace);
                //    writer.WriteEndAttribute();
                //}

                // write public properties and fields
                dumpProperty(obj, parentType, false);

                // write non-public properties and fields
                if (IsOutputNonPublic)
                {
                    writer.WriteStartElement("NonPublic");
                    dumpProperty(obj, parentType, true);
                    writer.WriteEndElement();
                }
            }

            private void dumpProperty(object obj, Type parentType, bool nonPublic)
            {
                BindingFlags binding = BindingFlags.Instance | (nonPublic ? BindingFlags.NonPublic : BindingFlags.Public);
                var writeAsXmlElementInfos = new List<PropertyInfo>();
                var table = new Dictionary<string, object>();

                // write properties as XML attribute
                foreach (var info in parentType.GetProperties(binding))
                {
                    if (!info.CanRead)
                        continue;
                    if (info.GetIndexParameters().Length > 0)
                        continue;
                    if (info.Name == "Children")
                        continue;
                    if (table.ContainsKey(info.Name))
                        continue;
                    table.Add(info.Name, null);
                    if (isOutputingAsXmlElement(info.PropertyType))
                    {
                        writeAsXmlElementInfos.Add(info);
                        continue;
                    }

                    writer.WriteStartAttribute(convertNameString(info.Name));
                    try
                    {
                        var value = info.GetValue(obj, null);
                        writeValue(value, info.PropertyType);
                    }
                    catch (Exception ex)
                    {
                        writer.WriteString("[" + ex.GetType().Name + "]");
                    }

                    writer.WriteEndAttribute();
                }

                // write remaining properties as nested XML element
                propertyNestCount++;
                foreach (var info in writeAsXmlElementInfos)
                {
                    if (propertyNestCount > 2)
                    {
                        writer.WriteStartElement(convertNameString(info.Name));
                        writer.WriteString("[NestOver]");
                        writer.WriteEndElement();
                        continue;
                    }

                    if (info.GetIndexParameters().Length > 0)
                        continue;

                    object nestObj = null;
                    try
                    {
                        nestObj = info.GetValue(obj, null);
                    }
                    catch (Exception ex)
                    {
                        writer.WriteStartElement(convertNameString(info.Name));
                        writer.WriteString("[" + ex.GetType().Name + "]");
                        writer.WriteEndElement();
                    }
                    if (nestObj != null)
                    {
                        writer.WriteStartElement(convertNameString(info.Name));
                        dumpProperty(nestObj, info.PropertyType);
                        writer.WriteEndElement();
                    }
                }
                propertyNestCount--;

                // write fields
                var fields = parentType.GetFields(binding);
                if (fields.Length > 0)
                {
                    writer.WriteStartElement("Field");
                    foreach (var info in fields)
                    {
                        if (table.ContainsKey(info.Name))
                            continue;
                        table.Add(info.Name, null);

                        writer.WriteStartAttribute(convertNameString(info.Name));
                        try
                        {
                            var value = info.GetValue(obj);
                            writeValue(value, info.FieldType);
                        }
                        catch (Exception ex)
                        {
                            writer.WriteString("[" + ex.GetType().Name + "]");
                        }

                        writer.WriteEndAttribute();
                    }
                    writer.WriteEndElement();
                }

                // write collection items
                if (!nonPublic && parentType.GetInterface("System.Collections.IEnumerable") != null)
                {
                    writer.WriteStartElement("Items");
                    propertyNestCount++;

                    foreach (object item in (obj as System.Collections.IEnumerable))
                    {
                        if (item == null)
                        {
                            writer.WriteStartElement("null");
                        }
                        else
                        {
                            Type t = item.GetType();

                            writer.WriteStartElement(convertNameString(t.Name));
                            if (isOutputingAsXmlElement(t))
                            {
                                dumpProperty(item, t);
                            }
                            else
                            {
                                writeValue(item, t);
                            }
                        }
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    propertyNestCount--;
                }
            }

            private bool isOutputingAsXmlElement(Type type)
            {
                return type.IsClass &&
                       type != typeof(String) &&
                       type != typeof(Widget) &&
                       !type.IsSubclassOf(typeof(Widget)) &&
                       type != typeof(UIElement) &&
                       !type.IsSubclassOf(typeof(UIElement)) &&
                       type != typeof(Scene) &&
                       !type.IsSubclassOf(typeof(Scene)) &&
                       !(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LinkedTree<>));
            }

            private void writeValue(object obj, Type type)
            {
                if (obj == null)
                {
                    writer.WriteString("null");
                }
                else if (type == typeof(Matrix4))
                {
                    var m = (Matrix4)obj;
                    writer.WriteString(String.Format(
                        "({0},{1},{2},{3}),({4},{5},{6},{7}),({8},{9},{10},{11}),({12},{13},{14},{15})",
                        m.M11, m.M12, m.M13, m.M14,
                        m.M21, m.M22, m.M23, m.M24,
                        m.M31, m.M32, m.M33, m.M34,
                        m.M41, m.M42, m.M43, m.M44));
                }
                else if (type == typeof(Vector4))
                {
                    var v = (Vector4)obj;
                    writer.WriteString(String.Format(
                        "{0},{1},{2},{3}",
                        v.X, v.Y, v.Z, v.W));
                }
                else if (type == typeof(Vector3))
                {
                    var v = (Vector3)obj;
                    writer.WriteString(String.Format(
                        "{0},{1},{2}",
                        v.X, v.Y, v.Z));
                }
                else if (type == typeof(Vector2))
                {
                    var v = (Vector2)obj;
                    writer.WriteString(String.Format(
                        "{0},{1}",
                        v.X, v.Y));
                }
                else if (type == typeof(Quaternion))
                {
                    var v = (Quaternion)obj;
                    writer.WriteString(String.Format(
                        "{0},{1},{2},{3}",
                        v.X, v.Y, v.Z, v.W));
                }
                else if (type == typeof(UIColor))
                {
                    var c = (UIColor)obj;
                    writer.WriteString(String.Format(
                        "#{0:X2}{1:X2}{2:X2}{3:X2}",
                        (int)(c.A * 255), (int)(c.R * 255), (int)(c.G * 255), (int)(c.B * 255)));
                }
                else
                {
                    writer.WriteString(obj.ToString());
                }

            }

            string convertNameString(string name)
            {
                char[] ret = new char[name.Length];
                for (int i = 0; i < name.Length; i++)
                {
                    char c = name[i];
                    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                        ret[i] = c;
                    else
                        ret[i] = '_';
                }

                return new string(ret);
            }
        }

        private class ConsoleWriter : TextWriter
        {
            // on adb logcat, Console.Out output many lines
            // so this class output with each 2kb (2kb is max buffer size?)
            // this class was specialized for UIDebug.XmlTextWriter

            StringBuilder sb = new StringBuilder(2048);
            public override void Write(string value)
            {
                if (sb.Length + value.Length >= sb.Capacity)
                    Flush();
                sb.Append(value);
            }
            public override void Write(char[] buffer, int index, int count)
            {
                if (sb.Length + count >= sb.Capacity)
                    Flush();
                sb.Append(buffer, index, count);
            }
            public override System.Text.Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
            public override void Flush()
            {
                Console.Write(sb.ToString());
                sb.Length = 0;
            }
        }

        private class XmlTextWriter
        {
            List<string> elementStack = new List<string>(64);
            bool inElementName = false;
            bool inAttribute = false;

            System.IO.TextWriter stream = null;

            public XmlTextWriter(string filename, System.Text.Encoding encoding)
            {
                stream = new System.IO.StreamWriter(filename, false, encoding);
            }
            public XmlTextWriter(System.IO.TextWriter writer)
            {
                stream = writer;
            }
            public void WriteStartDocument()
            {
                stream.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
            }
            public void WriteEndDocument()
            {
                UIDebug.Assert(elementStack.Count == 0 && !inAttribute && !inElementName);
                if (stream != null)
                {
                    stream.Write("\r\n");
                    stream.Flush();
                    stream.Close();
                    stream = null;
                }
            }
            public void WriteString(string text)
            {
                if (text == null) text = "";

                if (inElementName && !inAttribute)
                {
                    stream.Write(">");
                    inElementName = false;
                }

                var textChars = text.ToCharArray();
                int index = 0;
                int length = textChars.Length;
                for (int i = 0; i < textChars.Length; i++)
                {
                    char c = textChars[i];
                    string replace = null;
                    if (c == '&')
                        replace = "&amp;";
                    else if (c == '<')
                        replace = "&lt;";
                    else if (c == '>')
                        replace = "&gt;";
                    else if (c == '\'')
                        replace = "&apos;";
                    else if (c == '"')
                        replace = "&quot;";

                    if (replace != null)
                    {
                        if (i > index)
                            stream.Write(textChars, index, i - index);
                        stream.Write(replace);
                        index = i + 1;
                    }
                }
                if (index < textChars.Length)
                {
                    stream.Write(textChars, index, length - index);
                }

                if (inAttribute)
                {
                    stream.Write("\"");
                    inAttribute = false;
                    inElementName = true;
                }
            }
            public void WriteStartElement(string name)
            {
                UIDebug.Assert(!inAttribute);
                UIDebug.Assert(!String.IsNullOrEmpty(name));

                elementStack.Add(name);

                if (inElementName)
                    stream.Write("><" + name);
                else
                    stream.Write("<" + name);

                inElementName = true;
            }
            public void WriteEndElement()
            {
                UIDebug.Assert(!inAttribute);
                UIDebug.Assert(elementStack.Count > 0);

                string name = elementStack[elementStack.Count - 1];
                elementStack.RemoveAt(elementStack.Count - 1);

                if (inElementName)
                {
                    stream.Write(" />");
                }
                else
                {
                    stream.Write("</" + name + ">");
                }
                inElementName = false;
            }
            public void WriteStartAttribute(string name)
            {
                UIDebug.Assert(inElementName);

                stream.Write(" " + name + "=\"");
                inAttribute = true;
                inElementName = false;
            }
            public void WriteEndAttribute()
            {
            }

            public void Close()
            {
                UIDebug.Assert(stream == null);

                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        #endregion

        #region misc

        
        /// @internal
        /// @if LANG_JA
        /// <summary>同期、非同期のモード</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Synchronous and asynchronous modes</summary>
        /// @endif
        internal enum ForceMode
        {
            /// @if LANG_JA
            /// <summary>通常の設定に従う</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Follow standard settings</summary>
            /// @endif
            Normal = 0,
            /// @if LANG_JA
            /// <summary>強制的にすべて有効にする</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Forcefully enable everything</summary>
            /// @endif
            ForceEnabled,
            /// @if LANG_JA
            /// <summary>強制的にすべて無効にする</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Forcefully disable everything</summary>
            /// @endif
            ForceDisabled,
        }

        
        /// @internal
        /// @if LANG_JA
        /// <summary>ImageAssetの非同期読み込みを制御する</summary>
        /// <value>ForceEnabled: すべて非同期で読み込む  ForceDisabled: すべて同期で読み込む</value>
        /// @endif
        /// @if LANG_EN
        /// <summary>Control asynchronous read of ImageAsset</summary>
        /// <value>ForceEnabled: read everything asynchronously  ForceDisabled: read everything synchronously</value>
        /// @endif
        internal static ForceMode ImageAssetAsyncLoad { get; set; }

        
        /// @internal
        /// @if LANG_JA
        /// <summary>WidgetのClipを制御する</summary>
        /// <value>ForceEnabled: すべてのウィジェットのClipを有効にする  ForceDisabled: すべてのウィジェットのClipを無効にする</value>
        /// @endif
        /// @if LANG_EN
        /// <summary>Control Clip of Widget</summary>
        /// <value>ForceEnabled: enable Clip of all widgets
        /// ForceDisabled: disable Clip of all widgets</value>
        /// @endif
        internal static ForceMode WidgetClipping { get; set; }

        #endregion

        #region Internal Log
        [Conditional("LOG_FOCUS")]
        internal static void LogFocus(string format, params Object[] args)
        {
            Console.WriteLine("[FOCUS] ({0:mm:ss.fff}) {1}", DateTime.Now, string.Format(format, args));
        }
        [Conditional("LOG_FOCUS")]
        internal static void LogFocusIf(bool condition, string format, params Object[] args)
        {
            if (condition)
                LogFocus(format, args);
        }

        [Conditional("LOG_ASSET")]
        internal static void LogAsset(string format, params Object[] args)
        {
            Console.WriteLine("[ASSET] ({0:mm:ss.fff}) {1}", DateTime.Now, string.Format(format, args));
        }

        #endregion
    }
}
