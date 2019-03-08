/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

namespace Sce.PlayStation.HighLevel.UI
{
    /// @if LANG_JA
    /// <summary>画像アセットクラス</summary>
    /// <remarks>テクスチャの読み込みなどを隠蔽するクラスです。
    /// 画像ファイルの非同期の読み込みやキャッシュをサポートします。
    /// 画像ファイルから ImageAsset を作成した場合は、テクスチャがキャッシュされます。
    /// このキャッシュにより、同一ファイル名の ImageAsset を作成した場合、テクスチャのアンマネージドリソースは共有されます。
    /// なお、キャッシュは Dispose やガーベージコレクションだけでは解放されません。
    /// 使用しなくなった ImageAsset は UnloadFromCache を呼び出したあと、同一画像ファイルのImageAssetすべての Dispose を呼び出してください。</remarks>
    /// @endif
    /// @if LANG_EN
    /// <summary>Image asset class</summary>
    /// <remarks>Class concealing texture loading.
    /// Asynchronous loading of image files and caching is supported.
    /// When ImageAsset has been created from an image file, the texture is cached.
    /// If an ImageAsset with the same filename is created from this cache, the unmanaged resources of the texture will be shared.
    /// Note that the cache will not be freed with just Dispose or garbage collection.
    /// For ImageAssets that are no longer used, after calling UnloadFromCache, call all Disposes for the ImageAssets of the same image files.</remarks>
    /// @endif
    public sealed class ImageAsset : IDisposable
    {
        private Texture2D unsharedTexture = null;
        internal string filename = null;

        #region constructor

        /// @if LANG_JA
        /// <summary>指定した画像ファイルから新しいインスタンスを生成する</summary>
        /// <remarks>一度読み込むとキャッシュされます。使用しなくなった場合は UnloadFromCache を呼び出してください。</remarks>
        /// <param name="filename">画像ファイル名</param>
        /// <param name="asyncLoad">非同期で読み込むかどうか</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Creates a new instance from a specified image file</summary>
        /// <remarks>Once loaded, it will be cached. For those that are no longer used, call UnloadFromCache.</remarks>
        /// <param name="filename">Image filename</param>
        /// <param name="asyncLoad">Whether or not asynchronous loading is performed</param>
        /// @endif
        public ImageAsset(string filename, bool asyncLoad)
        {
            this.filename = filename;
            AssetManager.LoadTexture(filename, asyncLoad);
        }

        /// @if LANG_JA
        /// <summary>指定した画像ファイルから新しいインスタンスを生成する</summary>
        /// <remarks>同期で読み込みます。一度読み込むとキャッシュされます。使用しなくなった場合は UnloadFromCache を呼び出してください。</remarks>
        /// <param name="filename">画像ファイル名</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Creates a new instance from a specified image file</summary>
        /// <remarks>Performs synchronous loading. Once loaded, it will be cached. For those that are no longer used, call UnloadFromCache.</remarks>
        /// <param name="filename">Image filename</param>
        /// @endif
        public ImageAsset(string filename)
            : this(filename, false)
        {
        }

        /// @if LANG_JA
        /// <summary>指定したシステム画像アセットの新しいインスタンスを生成する</summary>
        /// <remarks>標準のウィジェットなどに使用される画像アセットです。非同期で読み込みます。</remarks>
        /// <param name="name">システム画像アセット</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Creates a new instance for a specified system image asset</summary>
        /// <remarks>Image assets used for standard widgets, etc. Performs asynchronous loading.</remarks>
        /// <param name="name">System image asset</param>
        /// @endif
        public ImageAsset(SystemImageAsset name)
        {
            this.filename = AssetManager.GetSystemFileName(name);
#if LOAD_ASSETS_FROM_FILE_SYSTEM
            UIDebug.AssetLog("loading " + filename + " from the file system . . .");
            AssetManager.LoadTexture(filename, AssetManager.AsyncLoadingSystemAsset);
#else
            AssetManager.LoadTextureFromAssembly(filename, AssetManager.AsyncLoadingSystemAsset);
#endif
            var imgSize = AssetManager.GetImageSize(name);
            this.width = imgSize.Width;
            this.height = imgSize.Height;
        }

        /// @if LANG_JA
        /// <summary>指定した既存のテクスチャから新しいインスタンスを生成する</summary>
        /// <remarks>キャッシュはされません。ImageAsset内ではテクスチャの ShallowClone を保持するため、引数で指定したテクスチャのインスタンスは必要に応じて Dispose してください。</remarks>
        /// <param name="texture">テクスチャ</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Creates a new instance from a specified existing texture</summary>
        /// <remarks>Will not be cached. To hold the ShallowClone of a texture in ImageAsset, use Dispose on the instance for the texture specified in the argument as required.</remarks>
        /// <param name="texture">Textures</param>
        /// @endif
        public ImageAsset(Texture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");
            if (texture.Width <= 0 || texture.Height <= 0)
                throw new ArgumentException("texture size is 0", "texture");

            this.unsharedTexture = texture.ShallowClone() as Texture2D;
            this.width = texture.Width;
            this.height = texture.Height;

            UIDebug.recordUncachedImageAsset(this);
        }

        /// @if LANG_JA
        /// <summary>指定した既存のイメージから新しいインスタンスを生成する</summary>
        /// <remarks>キャッシュはされません。引数で指定したイメージのインスタンスは必要に応じて Dispose してください。</remarks>
        /// <param name="image">既存の画像</param>
        /// <param name="format">指定した既存の画像のピクセルフォーマット</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Creates a new instance from a specified existing image</summary>
        /// <remarks>Will not be cached. Use Dispose on the instance for the image specified in the argument as required.</remarks>
        /// <param name="image">Existing image</param>
        /// <param name="format">Pixel format of specified existing image</param>
        /// @endif
        [Obsolete("use ImageAsset(Image image) constructor")]
        public ImageAsset(Image image, PixelFormat format)
            : this(image)
        {
        }
        
        /// @if LANG_JA
        /// <summary>指定した既存のイメージから新しいインスタンスを生成する</summary>
        /// <remarks>キャッシュはされません。引数で指定したイメージのインスタンスは必要に応じて Dispose してください。</remarks>
        /// <param name="image">既存の画像</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Creates a new instance from a specified existing image</summary>
        /// <remarks>Will not be cached. Use Dispose on the instance for the image specified in the argument as required.</remarks>
        /// <param name="image">Existing image</param>
        /// @endif
        public ImageAsset(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");
            if (image.Size.Width <= 0 || image.Size.Height <= 0)
                throw new ArgumentException("image size is 0", "image");

            byte[] buffer = image.ToBuffer();
            
            // TODO: Image Decode check
            if (buffer.Length <= 0)
                throw new ArgumentException("image is not decoded", "image");

            // TODO: can not get ImageMode directly
            UIDebug.Assert(buffer.Length == image.Size.Width * image.Size.Height
                           || buffer.Length == image.Size.Width * image.Size.Height * 4);
            var format = PixelFormat.Rgba;
            if(buffer.Length <= image.Size.Width * image.Size.Height)
                format = PixelFormat.Alpha;

            this.unsharedTexture = new Texture2D(image.Size.Width, image.Size.Height, false, format);
            this.unsharedTexture.SetPixels(0, buffer);
            this.width = this.unsharedTexture.Width;
            this.height = this.unsharedTexture.Height;

            UIDebug.recordUncachedImageAsset(this);
        }

        /// @if LANG_JA
        /// <summary>指定した画像ファイルから新しいインスタンスを生成する</summary>
        /// <remarks>一度読み込むとキャッシュされます。使用しなくなった場合は UnloadFromCache を呼び出してください。</remarks>
        /// <param name="filename">画像ファイル名</param>
        /// <param name="asyncLoad">非同期で読み込むかどうか</param>
        /// <param name="scaledPixelDensity">ピクセル密度に応じてスケールするかどうか</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Creates a new instance from a specified image file</summary>
        /// <remarks>Once loaded, it will be cached. For those that are no longer used, call UnloadFromCache.</remarks>
        /// <param name="filename">Image filename</param>
        /// <param name="asyncLoad">Whether or not asynchronous loading is performed</param>
        /// <param name="scaledPixelDensity">Whether or not to scale according to the pixel density</param>
        /// @endif
        public ImageAsset(string filename, bool asyncLoad, bool scaledPixelDensity)
            : this(filename, asyncLoad)
        {
            this.ScaledPixelDensity = scaledPixelDensity;
        }

        /// @if LANG_JA
        /// <summary>指定した既存のテクスチャから新しいインスタンスを生成する</summary>
        /// <remarks>キャッシュはされません。ImageAsset内ではテクスチャの ShallowClone を保持するため、引数で指定したテクスチャのインスタンスは必要に応じて Dispose してください。</remarks>
        /// <param name="texture">テクスチャ</param>
        /// <param name="scaledPixelDensity">ピクセル密度に応じてスケールするかどうか</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Creates a new instance from a specified existing texture</summary>
        /// <remarks>Will not be cached. To hold the ShallowClone of a texture in ImageAsset, use Dispose on the instance for the texture specified in the argument as required.</remarks>
        /// <param name="texture">Textures</param>
        /// <param name="scaledPixelDensity">Whether or not to scale according to the pixel density</param>
        /// @endif
        public ImageAsset(Texture2D texture, bool scaledPixelDensity)
            : this(texture)
        {
            this.ScaledPixelDensity = scaledPixelDensity;
        }

        /// @if LANG_JA
        /// <summary>指定した既存のイメージから新しいインスタンスを生成する</summary>
        /// <remarks>キャッシュはされません。引数で指定したイメージのインスタンスは必要に応じて Dispose してください。</remarks>
        /// <param name="image">既存の画像</param>
        /// <param name="format">指定した既存の画像のピクセルフォーマット</param>
        /// <param name="scaledPixelDensity">ピクセル密度に応じてスケールするかどうか</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Creates a new instance from a specified existing image</summary>
        /// <remarks>Will not be cached. Use Dispose on the instance for the image specified in the argument as required.</remarks>
        /// <param name="image">Existing image</param>
        /// <param name="format">Pixel format of specified existing image</param>
        /// <param name="scaledPixelDensity">Whether or not to scale according to the pixel density</param>
        /// @endif
        [Obsolete("use ImageAsset(Image image, bool scaledPixelDensity) constructor")]
        public ImageAsset(Image image, PixelFormat format, bool scaledPixelDensity)
            : this(image)
        {
            this.ScaledPixelDensity = scaledPixelDensity;
        }

        /// @if LANG_JA
        /// <summary>指定した既存のイメージから新しいインスタンスを生成する</summary>
        /// <remarks>キャッシュはされません。引数で指定したイメージのインスタンスは必要に応じて Dispose してください。</remarks>
        /// <param name="image">既存の画像</param>
        /// <param name="scaledPixelDensity">ピクセル密度に応じてスケールするかどうか</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Creates a new instance from a specified existing image</summary>
        /// <remarks>Will not be cached. Use Dispose on the instance for the image specified in the argument as required.</remarks>
        /// <param name="image">Existing image</param>
        /// <param name="scaledPixelDensity">Whether or not to scale according to the pixel density</param>
        /// @endif
        public ImageAsset(Image image, bool scaledPixelDensity)
            : this(image)
        {
            this.ScaledPixelDensity = scaledPixelDensity;
        }

        #endregion

        /// @if LANG_JA
        /// <summary>テクスチャの複製を取得</summary>
        /// <remarks>テクスチャのShallowCloneを返します。非同期読み込みで、まだテクスチャの読み込みが完了していない場合はnullを返します。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains texture duplicates</summary>
        /// <remarks>Returns the ShallowClone of the texture. If texture loading is not yet completed during asynchronous loading, null is returned.</remarks>
        /// @endif
        public Texture2D CloneTexture()
        {
            Texture2D texture = getTexture();
            if (texture != null)
            {
                return texture.ShallowClone() as Texture2D;
            }
            else
            {
                return null;
            }
        }

        private Texture2D getTexture()
        {
            if (filename != null)
            {
                return AssetManager.GetTexture(this.filename);
            }
            else
            {
                return this.unsharedTexture;
            }
        }

        /// @if LANG_JA
        /// <summary>画像アセットの読み込みが完了したかどうかを取得する</summary>
        /// <remarks>画像ファイル以外や同期で読み込んだ場合は true が返ります。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains whether image asset loading is completed</summary>
        /// <remarks>"true" returns if a non-image file is loaded or if loaded synchronously.</remarks>
        /// @endif
        public bool Ready
        {
            get
            {
                if (filename != null)
                {
                    return AssetManager.IsLoadedTexture(this.filename);
                }
                else
                {
                    return (this.unsharedTexture != null);
                }
            }
        }

        internal bool ScaledPixelDensity { get; set; }

        /// @if LANG_JA
        /// <summary>ファイルが読み込まれるまで待機する</summary>
        /// <remarks>同期で読み込んだ場合やすでに読み込みが完了している場合はすぐに戻ります。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Waits until the file is loaded</summary>
        /// <remarks>Returns quickly if loaded synchronously or if already loaded.</remarks>
        /// @endif
        public void WaitForLoad()
        {
            if (this.filename != null)
            {
                AssetManager.WaitForLoad(filename);
            }
        }

        /// @if LANG_JA
        /// <summary>画像アセットの幅を取得する</summary>
        /// <remarks>非同期読み込みでまだ読み込まれていない場合は0を返します</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the width of an image asset</summary>
        /// <remarks>If loading is not yet completed during asynchronous loading, 0 is returned</remarks>
        /// @endif
        public int Width
        {
            get
            {
                if (this.width <= 0)
                {
                    ImageSize size = AssetManager.GetImageSize(this.filename);
                    this.width = size.Width;
                    this.height = size.Height;
                }
                if (this.ScaledPixelDensity && UISystem.IsScaledPixelDensity)
                {
                    return (int)(this.width / UISystem.PixelDensity);
                }
                return this.width;
            }
        }
        private int width = 0;


        /// @if LANG_JA
        /// <summary>画像アセットの高さを取得する</summary>
        /// <remarks>非同期読み込みでまだ読み込まれていない場合は0を返します。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the height of an image asset</summary>
        /// <remarks>If loading is not yet completed during asynchronous loading, 0 is returned.</remarks>
        /// @endif
        public int Height
        {
            get
            {
                if (this.height <= 0 && this.filename != null)
                {
                    ImageSize size = AssetManager.GetImageSize(this.filename);
                    this.width = size.Width;
                    this.height = size.Height;
                }
                if (this.ScaledPixelDensity && UISystem.IsScaledPixelDensity)
                {
                    return (int)(this.height / UISystem.PixelDensity);
                }
                return this.height;
            }
        }
        private int height = 0;

        /// @if LANG_JA
        /// <summary>画像ファイルのパスを取得する</summary>
        /// <remarks>ファイルから生成していない場合は null が返ります。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the image file path</summary>
        /// <remarks>Returns null when a file is not created.</remarks>
        /// @endif
        public string FilePath
        {
            get { return filename; }
        }

        
        /// @if LANG_JA
        /// <summary>使用されているリソースを解放する</summary>
        /// <remarks>このインスタンスが保持しているテクスチャの Dispose を呼びます。
        /// 画像ファイルから読み込んだImageAssetの場合、キャッシュされているテクスチャは解放されません。
        /// 完全に解放するためには UnloadFromCache を呼び出す必要があります。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Frees used resources</summary>
        /// <remarks>Calls Dispose for a texture held by this instance.
        /// When ImageAsset is loaded from an image file, the cached texture is not freed.
        /// For complete release, UnloadFromCache must be called.</remarks>
        /// @endif
        public void Dispose()
        {
            if (this.unsharedTexture != null)
            {
                this.unsharedTexture.Dispose();
                this.unsharedTexture = null;
            }
        }

        /// @if LANG_JA
        /// <summary>画像ファイルから読み込んだImageAssetのキャッシュを削除する</summary>
        /// <remarks>キャッシュされているテクスチャを解放します。
        /// キャッシュ内のテクスチャのみ解放されるので、このインスタンスは引き続き使用することができます。
        /// 対象のテクスチャのアンマネージドリソースをガーベージコレクションを待たずに解放するには、Dispose を呼ぶ必要があります。</remarks>
        /// <returns>false:キャッシュに存在していなかった</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Deletes the ImageAsset cache loaded from an image file</summary>
        /// <remarks>Frees a cached texture.
        /// Only textures in a cache are freed, so it is possible to continue using this instance.
        /// To free unmanaged resources of the target texture without waiting for garbage collection, Dispose must be called.</remarks>
        /// <returns>false: Did not exist in the cache</returns>
        /// @endif
        public bool UnloadFromCache()
        {
            if (filename != null)
            {
                return AssetManager.UnloadTexture(filename);
            }
            return false;
        }

        #region static

        /// @if LANG_JA
        /// <summary>非同期で読み込んでいるすべての画像が読み込まれるまで待機する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Waits until the asynchronous loading of all images completes.</summary>
        /// @endif
        public static void WaitForLoadAll()
        {
            AssetManager.WaitForLoadAll();
        }

        /// @if LANG_JA
        /// <summary>ファイルから読み込んだImageAssetのキャッシュを削除する</summary>
        /// <param name="filename">ファイル名</param>
        /// <returns>false:キャッシュに存在していなかった</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Deletes the ImageAsset cache loaded from a file</summary>
        /// <param name="filename">Filename</param>
        /// <returns>false: Did not exist in the cache</returns>
        /// @endif
        public static bool UnloadFromCache(string filename)
        {
            return AssetManager.UnloadTexture(filename);
        }

        internal static bool ImageEquals(ImageAsset x, ImageAsset y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x.filename == null)
            {
                // TODO: compare unsharedTexture
                return false;
            }

            return x.filename == y.filename;
        }

        #endregion
    }

    /// @internal
    /// @if LANG_JA
    /// <summary>アセットの管理クラス</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Asset management class</summary>
    /// @endif
    internal static class AssetManager
    {
        internal static Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();
        private static Dictionary<SystemImageAsset, string> imageAssetFilename;
        private static Dictionary<SystemImageAsset, NinePatchMargin> imageAssetNinePatchMargin;
        private static Dictionary<SystemImageAsset, ImageSize> imageAssetSize;

        static Dictionary<string, object> loadingImageFilename = new Dictionary<string, object>();
        
        // sync member
        private static object lockObj = new Object();
        static Dictionary<string, ImageData> imageCache = new Dictionary<string, ImageData>();

        internal static bool AsyncLoadingSystemAsset = true;

        internal static void Initialize()
        {
            lock (lockObj)
            {
                if (imageCache == null)
                    imageCache = new Dictionary<string, ImageData>();
            }
        }

        internal static void Terminate()
        {
            UnloadAllTexture();
            lock (lockObj)
            {
                imageCache = null;
            }
        }

        /// @if LANG_JA
        /// <summary>システムアセットを全て読み込む。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Reads all system assets.</summary>
        /// @endif
        internal static void LoadSystemAsset()
        {
        }

        /// @if LANG_JA
        /// <summary>テクスチャを非同期で読み込みキャッシュする。</summary>
        /// <param name="isAsync"></param>
        /// <param name="filename">ファイル名</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Loads asynchronously and caches the textures.</summary>
        /// <param name="isAsync"></param>
        /// <param name="filename">Filename</param>
        /// @endif
        internal static void LoadTexture(string filename, bool isAsync)
        {
            // for debug
            if(UIDebug.ImageAssetAsyncLoad != UIDebug.ForceMode.Normal)
                isAsync = (UIDebug.ImageAssetAsyncLoad == UIDebug.ForceMode.ForceEnabled);

            if (!cache.ContainsKey(filename))
            {
                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException("File not found.", filename);
                }

                if (isAsync)
                {
                    if (!loadingImageFilename.ContainsKey(filename))
                    {
                        UIDebug.LogAsset("reserve to load " + filename);

                        loadingImageFilename.Add(filename, null);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(asyncLoad), filename);
                    }
                }
                else
                {
                    UIDebug.LogAsset("loading " + filename + " from the file system . . .");

                    loadingImageFilename.Remove(filename);
                    Texture2D texture2d = new Texture2D(filename, false);
                    cache[filename] = texture2d;

                    UIDebug.LogAsset("loaded  " + filename);
                }
            }
        }

        /// @if LANG_JA
        /// <summary>アセンブリに埋め込まれたファイルからテクスチャを非同期で読み込みキャッシュする。</summary>
        /// <param name="isAsync"></param>
        /// <param name="filename">ファイル名</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Caches the textures asynchronously loaded from files embedded in the assembly.</summary>
        /// <param name="isAsync"></param>
        /// <param name="filename">Filename</param>
        /// @endif
        internal static void LoadTextureFromAssembly(string filename, bool isAsync)
        {
            // for debug
            if(UIDebug.ImageAssetAsyncLoad != UIDebug.ForceMode.Normal)
                isAsync = (UIDebug.ImageAssetAsyncLoad == UIDebug.ForceMode.ForceEnabled);

            if (!cache.ContainsKey(filename))
            {
                if (isAsync)
                {
                    if (!loadingImageFilename.ContainsKey(filename))
                    {
                        UIDebug.LogAsset("reserve to load " + filename);

                        loadingImageFilename.Add(filename, null);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(asyncLoadFromAssembly), filename);
                    }
                }
                else
                {
                    UIDebug.LogAsset("loading " + filename + " from the assembly file . . .");

                    Assembly resourceAssembly = Assembly.GetExecutingAssembly();
                    string manifestResourceName = typeof(UISystem).Namespace + "." + filename.Replace("/", ".");

                    loadingImageFilename.Remove(filename);

                    using (Stream fileStream = resourceAssembly.GetManifestResourceStream(manifestResourceName))
                    {
                        if (fileStream == null)
                        {
                            throw new FileNotFoundException("File not found.", filename);
                        }

                        Byte[] dataBuffer = new Byte[fileStream.Length];
                        fileStream.Read(dataBuffer, 0, dataBuffer.Length);

                        Texture2D texture2d = new Texture2D(dataBuffer, false);
                        cache[filename] = texture2d;
                    }
                    UIDebug.LogAsset("loaded  " + filename + " from the assembly file . . .");
                }
            }
        }

        /// @if LANG_JA
        /// <summary>テクスチャを取得する。</summary>
        /// <param name="filename">ファイル名</param>
        /// <returns>まだ読み込んでいない場合はnullを返す。</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the texture.</summary>
        /// <param name="filename">Filename</param>
        /// <returns>Returns null if not loaded.</returns>
        /// @endif
        internal static Texture2D GetTexture(string filename)
        {
            if (cache.ContainsKey(filename))
            {
                return cache[filename];
            }
            else if (loadingImageFilename.ContainsKey(filename))
            {
                ImageData imgData = null;

                lock (lockObj)
                {
                    if (imageCache != null && imageCache.ContainsKey(filename))
                    {
                        imgData = imageCache[filename];
                        imageCache.Remove(filename);
                    }
                }

                // AsyncLoad
                if (imgData != null)
                {
                    var tex = new Texture2D(imgData.Size.Width, imgData.Size.Height, false, PixelFormat.Rgba);
                    tex.SetPixels(0, imgData.Buffer);
                    cache.Add(filename, tex);
                    loadingImageFilename.Remove(filename);
                    return tex;
                }
                else
                {
                    // not load yet
                    return null;
                }
            }
            else
            {
                // unloaded
                LoadTexture(filename, false);
                return cache[filename];
            }
        }

        internal static Texture2D GetTexture(SystemImageAsset name)
        {
            return GetTexture(imageAssetFilename[name]);
        }

        internal static string GetSystemFileName(SystemImageAsset name)
        {
            return imageAssetFilename[name];
        }

        internal static void WaitForLoad(string filename)
        {
            for (; ; )
            {
                var tex = GetTexture(filename);
                if (tex != null)
                {
                    break;
                }
                Thread.Sleep(0);
            }
        }

        internal static void WaitForLoadAll()
        {
            while (loadingImageFilename.Count > 0)
            {
                foreach (var key in loadingImageFilename.Keys)
                {
                    WaitForLoad(key);
                    break;
                }
            }
        }

        internal static bool UnloadTexture(string filename)
        {
            if (cache.ContainsKey(filename))
            {
                Texture2D texture = cache[filename];
                cache.Remove(filename);
                texture.Dispose();

                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void UnloadAllTexture()
        {
            lock (lockObj)
            {
                if (imageCache != null)
                {
                    imageCache.Clear();
                }
            }
            loadingImageFilename.Clear();
            foreach (Texture2D texture in cache.Values)
            {
                texture.Dispose();
            }
            cache.Clear();
        }

        internal static NinePatchMargin GetNinePatchMargin(SystemImageAsset name)
        {
            return imageAssetNinePatchMargin[name];
        }

        internal static ImageSize GetImageSize(SystemImageAsset name)
        {
            return imageAssetSize[name];
        }

        /// @if LANG_JA
        /// <summary>テクスチャを取得する。</summary>
        /// <param name="filename">ファイル名</param>
        /// <returns>まだ読み込んでいない場合はnullを返す。</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the texture.</summary>
        /// <param name="filename">Filename</param>
        /// <returns>Returns null if not loaded.</returns>
        /// @endif
        internal static ImageSize GetImageSize(string filename)
        {
            var tex = GetTexture(filename);
            if (tex != null)
            {
                return new ImageSize(tex.Width, tex.Height);
            }
            else
            {
                return new ImageSize();
            }
        }

        internal static bool IsLoadedTexture(string filename)
        {
            bool result = cache.ContainsKey(filename);
            if (!result)
            {
                lock (lockObj)
                {
                    if (imageCache == null)
                        return false; // AssetManager has terminated

                    result = imageCache.ContainsKey(filename);
                }
            }
            return result;
        }

        private static void asyncLoad(object state)
        {
            string name = state as string;
            lock (lockObj)
            {
                if (imageCache == null)
                    return; // AssetManager has terminated

                if (imageCache.ContainsKey(name))
                {
                    name = null;
                }
            }

            if (name != null)
            {
                UIDebug.LogAsset("async loading " + name + " from the file system . . .");

                var data = new ImageData();

                using (var img = new Image(name))
                {
                    img.Decode();
                    data.Size = img.Size;
                    data.Buffer = img.ToBuffer();
                }

                lock (lockObj)
                {
                    if (imageCache == null)
                        return; // AssetManager has terminated

                    imageCache.Add(name, data);
                }

                UIDebug.LogAsset("async loaded  " + name);
            }
        }

        private static void asyncLoadFromAssembly(object state)
        {
            string name = state as string;
            UIDebug.Assert(name != null);

            lock (lockObj)
            {
                if (imageCache == null)
                    return; // AssetManager has terminated

                if (imageCache.ContainsKey(name))
                {
                    name = null;
                }
            }

            if (name != null)
            {
                UIDebug.LogAsset("async loading " + name + " from the assembly file . . .");

                Assembly resourceAssembly = Assembly.GetExecutingAssembly();
                string manifestResourceName = typeof(UISystem).Namespace + "." + name.Replace("/", ".");
                
                Byte[] dataBuffer;
                using (Stream fileStream = resourceAssembly.GetManifestResourceStream(manifestResourceName))
                {
                    if (fileStream == null)
                    {
                        throw new FileNotFoundException("File not found.", name);
                    }

                    dataBuffer = new Byte[fileStream.Length];
                    fileStream.Read(dataBuffer, 0, dataBuffer.Length);
                }

                var data = new ImageData();

                using (var img = new Image(dataBuffer))
                {
                    img.Decode();
                    data.Size = img.Size;
                    data.Buffer = img.ToBuffer();
                }

                lock (lockObj)
                {
                    if (imageCache == null)
                        return; // AssetManager has terminated

                    imageCache.Add(name, data);
                }

                UIDebug.LogAsset("async loaded  " + name);
            }
        }

        static AssetManager()
        {
            imageAssetFilename = new Dictionary<SystemImageAsset, string>
            {
                {SystemImageAsset.ButtonBackgroundNormal,           "system_assets/button_9patch_normal.png"},
                {SystemImageAsset.ButtonBackgroundPressed,          "system_assets/button_9patch_press.png"},
                {SystemImageAsset.ButtonBackgroundDisabled,         "system_assets/button_9patch_disable.png"},
                {SystemImageAsset.CheckBoxCheckedNormal,            "system_assets/check_box_active.png"},
                {SystemImageAsset.CheckBoxUncheckedNormal,          "system_assets/check_box_normal.png"},
                {SystemImageAsset.CheckBoxCheckedPressed,           "system_assets/check_box_active_press.png"},
                {SystemImageAsset.CheckBoxUncheckedPressed,         "system_assets/check_box_normal_press.png"},
                {SystemImageAsset.CheckBoxCheckedDisabled,          "system_assets/check_box_active_disable.png"},
                {SystemImageAsset.CheckBoxUncheckedDisabled,        "system_assets/check_box_normal_disable.png"},
                {SystemImageAsset.RadioButtonCheckedNormal,         "system_assets/radio_bt_selected.png"},
                {SystemImageAsset.RadioButtonUncheckedNormal,       "system_assets/radio_bt_normal.png"},
                {SystemImageAsset.RadioButtonCheckedPressed,        "system_assets/radio_bt_selected_press.png"},
                {SystemImageAsset.RadioButtonUncheckedPressed,      "system_assets/radio_bt_normal_press.png"},
                {SystemImageAsset.RadioButtonCheckedDisabled,       "system_assets/radio_bt_selected_disable.png"},
                {SystemImageAsset.RadioButtonUncheckedDisabled,     "system_assets/radio_bt_normal_disable.png"},
                {SystemImageAsset.SliderHorizontalBaseNormal,       "system_assets/slider_bar_normal_9patch.png"},
                {SystemImageAsset.SliderHorizontalBaseDisabled,     "system_assets/slider_bar_normal_9patch_disable.png"},
                {SystemImageAsset.SliderHorizontalBarNormal,        "system_assets/slider_bar_active_9patch.png"},
                {SystemImageAsset.SliderHorizontalBarDisabled,      "system_assets/slider_bar_active_9patch_disable.png"},
                {SystemImageAsset.SliderHorizontalHandleNormal,     "system_assets/slider.png"},
                {SystemImageAsset.SliderHorizontalHandlePressed,    "system_assets/slider_press.png"},
                {SystemImageAsset.SliderHorizontalHandleDisabled,   "system_assets/slider_disable.png"},
                {SystemImageAsset.SliderVerticalBaseNormal,         "system_assets/slider_bar_verti_normal_9patch.png"},
                {SystemImageAsset.SliderVerticalBaseDisabled,       "system_assets/slider_bar_verti_normal_9patch_disable.png"},
                {SystemImageAsset.SliderVerticalBarNormal,          "system_assets/slider_bar_verti_active_9patch.png"},
                {SystemImageAsset.SliderVerticalBarDisabled,        "system_assets/slider_bar_verti_active_9patch_disable.png"},
                {SystemImageAsset.SliderVerticalHandleNormal,       "system_assets/slider_verti.png"},
                {SystemImageAsset.SliderVerticalHandlePressed,      "system_assets/slider_verti_press.png"},
                {SystemImageAsset.SliderVerticalHandleDisabled,     "system_assets/slider_verti_disable.png"},
                {SystemImageAsset.ProgressBarBase,                  "system_assets/slider_bar_normal_9patch.png"},
                {SystemImageAsset.ProgressBarNormal,                "system_assets/slider_bar_active_9patch.png"},
                {SystemImageAsset.ProgressBarAccelerator,           "system_assets/progress_accelerator.png"},
                {SystemImageAsset.ScrollBarHorizontalBackground,    "system_assets/scroll_bar_horiz_normal_9patch.png"},
                {SystemImageAsset.ScrollBarHorizontalBar,           "system_assets/scroll_bar_horiz_active_9patch.png"},
                {SystemImageAsset.ScrollBarVerticalBackground,      "system_assets/scroll_bar_verti_normal_9patch.png"},
                {SystemImageAsset.ScrollBarVerticalBar,             "system_assets/scroll_bar_verti_active_9patch.png"},
                {SystemImageAsset.EditableTextBackgroundNormal,     "system_assets/text_field_9patch_normal.png"},
                {SystemImageAsset.EditableTextBackgroundDisabled,   "system_assets/text_field_9patch_disable.png"},
                {SystemImageAsset.BusyIndicator,                    "system_assets/busy48.png"},
                {SystemImageAsset.DialogBackground,                 "system_assets/panel_9patch.png"},
                {SystemImageAsset.PagePanelNormal,                  "system_assets/pagepanel_point_normal.png"},
                {SystemImageAsset.PagePanelActive,                  "system_assets/pagepanel_point_active.png"},
                {SystemImageAsset.ListPanelSeparatorTop,            "system_assets/list_separator_top_9patch.png"},
                {SystemImageAsset.ListPanelSeparatorBottom,         "system_assets/list_separator_bottom_9patch.png"},
                {SystemImageAsset.ListPanelSectionSeparator,        "system_assets/separator.png"},
                {SystemImageAsset.MessageDialogBackground,          "system_assets/panel_9patch.png"},
                {SystemImageAsset.MessageDialogSeparator,           "system_assets/title_separator_9patch.png"},
                {SystemImageAsset.SpinBoxBase,                      "system_assets/spin_base.png"},
                {SystemImageAsset.SpinBoxCenter,                    "system_assets/spin_center.png"},
                {SystemImageAsset.PopupListBackgroundNormal,        "system_assets/popup_9patch_normal.png"},
                {SystemImageAsset.PopupListBackgroundPressed,       "system_assets/popup_9patch_press.png"},
                {SystemImageAsset.PopupListBackgroundDisabled,      "system_assets/popup_9patch_disable.png"},
                {SystemImageAsset.PopupListItemFocus,               "system_assets/popup_focus_9patch.png"},
                {SystemImageAsset.BackButtonBackgroundNormal,       "system_assets/back_button_9patch_normal.png"},
                {SystemImageAsset.BackButtonBackgroundPressed,      "system_assets/back_button_9patch_press.png"},
                {SystemImageAsset.BackButtonBackgroundDisabled,     "system_assets/back_button_9patch_disable.png"},
                {SystemImageAsset.NavigationBarBackground,          "system_assets/titlebar_9patch.png"},
                {SystemImageAsset.FocusRoundedCorner,               "system_assets/focus_rounded_corner.png"},
                {SystemImageAsset.FocusRectangle,                   "system_assets/focus_rectangle.png"},
                {SystemImageAsset.FocusCircle,                      "system_assets/focus_circle.png"},
                {SystemImageAsset.FocusListItem,                    "system_assets/focus_list.png"},
            };

            imageAssetSize = new Dictionary<SystemImageAsset, ImageSize>
            {
                {SystemImageAsset.ButtonBackgroundNormal,           new ImageSize(86,56)},
                {SystemImageAsset.ButtonBackgroundPressed,          new ImageSize(86,56)},
                {SystemImageAsset.ButtonBackgroundDisabled,         new ImageSize(86,56)},
                {SystemImageAsset.CheckBoxCheckedNormal,            new ImageSize(56,56)},
                {SystemImageAsset.CheckBoxUncheckedNormal,          new ImageSize(56,56)},
                {SystemImageAsset.CheckBoxCheckedPressed,           new ImageSize(56,56)},
                {SystemImageAsset.CheckBoxUncheckedPressed,         new ImageSize(56,56)},
                {SystemImageAsset.CheckBoxCheckedDisabled,          new ImageSize(56,56)},
                {SystemImageAsset.CheckBoxUncheckedDisabled,        new ImageSize(56,56)},
                {SystemImageAsset.RadioButtonCheckedNormal,         new ImageSize(39,39)},
                {SystemImageAsset.RadioButtonUncheckedNormal,       new ImageSize(39,39)},
                {SystemImageAsset.RadioButtonCheckedPressed,        new ImageSize(39,39)},
                {SystemImageAsset.RadioButtonUncheckedPressed,      new ImageSize(39,39)},
                {SystemImageAsset.RadioButtonCheckedDisabled,       new ImageSize(39,39)},
                {SystemImageAsset.RadioButtonUncheckedDisabled,     new ImageSize(39,39)},
                {SystemImageAsset.SliderHorizontalBaseNormal,       new ImageSize(17,16)},
                {SystemImageAsset.SliderHorizontalBaseDisabled,     new ImageSize(17,16)},
                {SystemImageAsset.SliderHorizontalBarNormal,        new ImageSize(17,16)},
                {SystemImageAsset.SliderHorizontalBarDisabled,      new ImageSize(17,16)},
                {SystemImageAsset.SliderHorizontalHandleNormal,     new ImageSize(40,58)},
                {SystemImageAsset.SliderHorizontalHandlePressed,    new ImageSize(40,58)},
                {SystemImageAsset.SliderHorizontalHandleDisabled,   new ImageSize(40,58)},
                {SystemImageAsset.SliderVerticalBaseNormal,         new ImageSize(16,17)},
                {SystemImageAsset.SliderVerticalBaseDisabled,       new ImageSize(16,17)},
                {SystemImageAsset.SliderVerticalBarNormal,          new ImageSize(16,17)},
                {SystemImageAsset.SliderVerticalBarDisabled,        new ImageSize(16,17)},
                {SystemImageAsset.SliderVerticalHandleNormal,       new ImageSize(58,40)},
                {SystemImageAsset.SliderVerticalHandlePressed,      new ImageSize(58,40)},
                {SystemImageAsset.SliderVerticalHandleDisabled,     new ImageSize(58,40)},
                {SystemImageAsset.ProgressBarBase,                  new ImageSize(17,16)},
                {SystemImageAsset.ProgressBarNormal,                new ImageSize(17,16)},
                {SystemImageAsset.ProgressBarAccelerator,           new ImageSize(64,8)},
                {SystemImageAsset.ScrollBarHorizontalBackground,    new ImageSize(11,10)},
                {SystemImageAsset.ScrollBarHorizontalBar,           new ImageSize(11,10)},
                {SystemImageAsset.ScrollBarVerticalBackground,      new ImageSize(10,11)},
                {SystemImageAsset.ScrollBarVerticalBar,             new ImageSize(10,11)},
                {SystemImageAsset.EditableTextBackgroundNormal,     new ImageSize(86,56)},
                {SystemImageAsset.EditableTextBackgroundDisabled,   new ImageSize(86,56)},
                {SystemImageAsset.BusyIndicator,                    new ImageSize(192,96)},
                {SystemImageAsset.DialogBackground,                 new ImageSize(69,69)},
                {SystemImageAsset.PagePanelNormal,                  new ImageSize(24,24)},
                {SystemImageAsset.PagePanelActive,                  new ImageSize(24,24)},
                {SystemImageAsset.ListPanelSeparatorTop,            new ImageSize(139,1)},
                {SystemImageAsset.ListPanelSeparatorBottom,         new ImageSize(139,1)},
                {SystemImageAsset.ListPanelSectionSeparator,        new ImageSize(15,25)},
                {SystemImageAsset.MessageDialogBackground,          new ImageSize(69,69)},
                {SystemImageAsset.MessageDialogSeparator,           new ImageSize(139,2)},
                {SystemImageAsset.SpinBoxBase,                      new ImageSize(40,200)},
                {SystemImageAsset.SpinBoxCenter,                    new ImageSize(9,30)},
                {SystemImageAsset.PopupListBackgroundNormal,        new ImageSize(86,56)},
                {SystemImageAsset.PopupListBackgroundPressed,       new ImageSize(86,56)},
                {SystemImageAsset.PopupListBackgroundDisabled,      new ImageSize(86,56)},
                {SystemImageAsset.PopupListItemFocus,               new ImageSize(139,1)},
                {SystemImageAsset.BackButtonBackgroundNormal,       new ImageSize(86,56)},
                {SystemImageAsset.BackButtonBackgroundPressed,      new ImageSize(86,56)},
                {SystemImageAsset.BackButtonBackgroundDisabled,     new ImageSize(86,56)},
                {SystemImageAsset.NavigationBarBackground,          new ImageSize(64,64)},
                {SystemImageAsset.FocusRoundedCorner,               new ImageSize(36,36)},
                {SystemImageAsset.FocusRectangle,                   new ImageSize(36,36)},
                {SystemImageAsset.FocusCircle,                      new ImageSize(96,96)},
                {SystemImageAsset.FocusListItem,                    new ImageSize(400,62)},
            };

            imageAssetNinePatchMargin = new Dictionary<SystemImageAsset, NinePatchMargin>
            {
                {SystemImageAsset.ButtonBackgroundNormal,           new NinePatchMargin{Left = 42, Top = 27, Right = 42, Bottom = 27}},
                {SystemImageAsset.ButtonBackgroundPressed,          new NinePatchMargin{Left = 42, Top = 27, Right = 42, Bottom = 27}},
                {SystemImageAsset.ButtonBackgroundDisabled,         new NinePatchMargin{Left = 42, Top = 27, Right = 42, Bottom = 27}},
                {SystemImageAsset.SliderHorizontalBaseNormal,       new NinePatchMargin{Left = 8, Top = 0, Right = 8, Bottom = 0}},
                {SystemImageAsset.SliderHorizontalBaseDisabled,     new NinePatchMargin{Left = 8, Top = 0, Right = 8, Bottom = 0}},
                {SystemImageAsset.SliderHorizontalBarNormal,        new NinePatchMargin{Left = 8, Top = 0, Right = 8, Bottom = 0}},
                {SystemImageAsset.SliderHorizontalBarDisabled,      new NinePatchMargin{Left = 8, Top = 0, Right = 8, Bottom = 0}},
                {SystemImageAsset.SliderVerticalBaseNormal,         new NinePatchMargin{Left = 0, Top = 8, Right = 0, Bottom = 8}},
                {SystemImageAsset.SliderVerticalBaseDisabled,       new NinePatchMargin{Left = 0, Top = 8, Right = 0, Bottom = 8}},
                {SystemImageAsset.SliderVerticalBarNormal,          new NinePatchMargin{Left = 0, Top = 8, Right = 0, Bottom = 8}},
                {SystemImageAsset.SliderVerticalBarDisabled,        new NinePatchMargin{Left = 0, Top = 8, Right = 0, Bottom = 8}},
                {SystemImageAsset.ProgressBarBase,                  new NinePatchMargin{Left = 8, Top = 0, Right = 8, Bottom = 0}},
                {SystemImageAsset.ProgressBarNormal,                new NinePatchMargin{Left = 8, Top = 0, Right = 8, Bottom = 0}},
                {SystemImageAsset.ScrollBarHorizontalBackground,    new NinePatchMargin{Left = 5, Top = 0, Right = 5, Bottom = 0}},
                {SystemImageAsset.ScrollBarHorizontalBar,           new NinePatchMargin{Left = 5, Top = 0, Right = 5, Bottom = 0}},
                {SystemImageAsset.ScrollBarVerticalBackground,      new NinePatchMargin{Left = 0, Top = 5, Right = 0, Bottom = 5}},
                {SystemImageAsset.ScrollBarVerticalBar,             new NinePatchMargin{Left = 0, Top = 5, Right = 0, Bottom = 5}},
                {SystemImageAsset.EditableTextBackgroundNormal,     new NinePatchMargin{Left = 10, Top = 10, Right = 10, Bottom = 10}},
                {SystemImageAsset.EditableTextBackgroundDisabled,   new NinePatchMargin{Left = 10, Top = 10, Right = 10, Bottom = 10}},
                {SystemImageAsset.DialogBackground,                 new NinePatchMargin{Left = 34, Top = 34, Right = 34, Bottom = 34}},
                {SystemImageAsset.ListPanelSeparatorTop,            new NinePatchMargin{Left = 68, Top = 0, Right = 68, Bottom = 0}},
                {SystemImageAsset.ListPanelSeparatorBottom,         new NinePatchMargin{Left = 68, Top = 0, Right = 68, Bottom = 0}},
                {SystemImageAsset.ListPanelSectionSeparator,        new NinePatchMargin{Left = 7, Top = 0, Right = 7, Bottom = 0}},
                {SystemImageAsset.MessageDialogBackground,          new NinePatchMargin{Left = 34, Top = 34, Right = 34, Bottom = 34}},
                {SystemImageAsset.MessageDialogSeparator,           new NinePatchMargin{Left = 68, Top = 0, Right = 68, Bottom = 0}},
                {SystemImageAsset.SpinBoxBase,                      new NinePatchMargin{Left = 19, Top = 99, Right = 19, Bottom = 99}},
                {SystemImageAsset.SpinBoxCenter,                    new NinePatchMargin{Left = 4, Top = 14, Right = 4, Bottom = 14}},
                {SystemImageAsset.PopupListBackgroundNormal,        new NinePatchMargin{Left = 42, Top = 0, Right = 42, Bottom = 0}},
                {SystemImageAsset.PopupListItemFocus,               new NinePatchMargin{Left = 68, Top = 0, Right = 68, Bottom = 0}},
                {SystemImageAsset.BackButtonBackgroundNormal,       new NinePatchMargin{Left = 42, Top = 27, Right = 42, Bottom = 27}},
                {SystemImageAsset.BackButtonBackgroundPressed,      new NinePatchMargin{Left = 42, Top = 27, Right = 42, Bottom = 27}},
                {SystemImageAsset.BackButtonBackgroundDisabled,     new NinePatchMargin{Left = 42, Top = 27, Right = 42, Bottom = 27}},
                {SystemImageAsset.NavigationBarBackground,          new NinePatchMargin{Left = 31, Top = 31, Right = 31, Bottom = 31}},
                {SystemImageAsset.FocusRoundedCorner,               new NinePatchMargin{Left = 17, Top = 17, Right = 17, Bottom = 17}},
                {SystemImageAsset.FocusRectangle,                   new NinePatchMargin{Left = 17, Top = 17, Right = 17, Bottom = 17}},
                {SystemImageAsset.FocusCircle,                      new NinePatchMargin{Left = 0, Top = 0, Right = 0, Bottom = 0}},
                {SystemImageAsset.FocusListItem,                    new NinePatchMargin{Left = 0, Top = 0, Right = 0, Bottom = 0}},
            };
        }

        class ImageData
        {
            public ImageSize Size;
            public Byte[] Buffer;
        }
    }

    /// @if LANG_JA
    /// <summary>システム画像アセット</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>System image asset</summary>
    /// @endif
    public enum SystemImageAsset
    {
        /// @if LANG_JA
        /// <summary>通常時のボタン</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Button normally displayed</summary>
        /// @endif
        ButtonBackgroundNormal = 0,

        /// @if LANG_JA
        /// <summary>押下時のボタン</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Button displayed when pressed</summary>
        /// @endif
        ButtonBackgroundPressed,

        /// @if LANG_JA
        /// <summary>無効時のボタン</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Button displayed when disabled</summary>
        /// @endif
        ButtonBackgroundDisabled,

        /// @if LANG_JA
        /// <summary>通常時のチェックボックス（チェック状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Check box normally displayed (checked)</summary>
        /// @endif
        CheckBoxCheckedNormal,

        /// @if LANG_JA
        /// <summary>通常時のチェックボックス（未チェック状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Check box normally displayed (unchecked)</summary>
        /// @endif
        CheckBoxUncheckedNormal,

        /// @if LANG_JA
        /// <summary>押下時のチェックボックス（チェック状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Check box displayed when button is pressed (checked)</summary>
        /// @endif
        CheckBoxCheckedPressed,

        /// @if LANG_JA
        /// <summary>押下時のチェックボックス（未チェック状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Check box displayed when button is pressed (unchecked)</summary>
        /// @endif
        CheckBoxUncheckedPressed,

        /// @if LANG_JA
        /// <summary>無効時のチェックボックス（チェック状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Check box displayed when disabled (checked)</summary>
        /// @endif
        CheckBoxCheckedDisabled,

        /// @if LANG_JA
        /// <summary>無効時のチェックボックス（未チェック状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Check box displayed when disabled (unchecked)</summary>
        /// @endif
        CheckBoxUncheckedDisabled,

        /// @if LANG_JA
        /// <summary>通常時のラジオボタン（選択状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Radio button normally displayed (selected)</summary>
        /// @endif
        RadioButtonCheckedNormal,

        /// @if LANG_JA
        /// <summary>通常時のラジオボタン（未選択状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Radio button normally displayed (unselected)</summary>
        /// @endif
        RadioButtonUncheckedNormal,

        /// @if LANG_JA
        /// <summary>押下時のラジオボタン（選択状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Radio button displayed when button is pressed (selected)</summary>
        /// @endif
        RadioButtonCheckedPressed,

        /// @if LANG_JA
        /// <summary>押下時のラジオボタン（未選択状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Radio button displayed when button is pressed (unselected)</summary>
        /// @endif
        RadioButtonUncheckedPressed,

        /// @if LANG_JA
        /// <summary>無効時のラジオボタン（選択状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Radio button displayed when disabled (selected)</summary>
        /// @endif
        RadioButtonCheckedDisabled,

        /// @if LANG_JA
        /// <summary>無効時のラジオボタン（未選択状態）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Radio button displayed when disabled (unselected)</summary>
        /// @endif
        RadioButtonUncheckedDisabled,

        /// @if LANG_JA
        /// <summary>有効時のスライダーのベース（水平方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider base when enabled (horizontal)</summary>
        /// @endif
        SliderHorizontalBaseNormal,

        /// @if LANG_JA
        /// <summary>無効時のスライダーのベース（水平方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider base when disabled (horizontal)</summary>
        /// @endif
        SliderHorizontalBaseDisabled,

        /// @if LANG_JA
        /// <summary>有効時のスライダーのバー（水平方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider bar when enabled (horizontal)</summary>
        /// @endif
        SliderHorizontalBarNormal,

        /// @if LANG_JA
        /// <summary>無効時のスライダーのバー（水平方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider bar when disabled (horizontal)</summary>
        /// @endif
        SliderHorizontalBarDisabled,

        /// @if LANG_JA
        /// <summary>通常時のスライダーのハンドル（水平方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider handle normally displayed (horizontal)</summary>
        /// @endif
        SliderHorizontalHandleNormal,

        /// @if LANG_JA
        /// <summary>押下時のスライダーのハンドル（水平方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider handle displayed when button is pressed (horizontal)</summary>
        /// @endif
        SliderHorizontalHandlePressed,

        /// @if LANG_JA
        /// <summary>無効時のスライダーのハンドル（水平方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider handle displayed when disabled (horizontal)</summary>
        /// @endif
        SliderHorizontalHandleDisabled,

        /// @if LANG_JA
        /// <summary>有効時のスライダーのベース（垂直方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider base when enabled (vertical)</summary>
        /// @endif
        SliderVerticalBaseNormal,

        /// @if LANG_JA
        /// <summary>無効時のスライダーのベース（垂直方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider base when disabled (vertical)</summary>
        /// @endif
        SliderVerticalBaseDisabled,

        /// @if LANG_JA
        /// <summary>有効時のスライダーのバー（垂直方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider bar when enabled (vertical)</summary>
        /// @endif
        SliderVerticalBarNormal,

        /// @if LANG_JA
        /// <summary>無効時のスライダーのバー（垂直方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider bar when disabled (vertical)</summary>
        /// @endif
        SliderVerticalBarDisabled,

        /// @if LANG_JA
        /// <summary>通常時のスライダーのハンドル（垂直方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider handle normally displayed (vertical)</summary>
        /// @endif
        SliderVerticalHandleNormal,

        /// @if LANG_JA
        /// <summary>押下時のスライダーのハンドル（垂直方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider handle displayed when button is pressed (vertical)</summary>
        /// @endif
        SliderVerticalHandlePressed,

        /// @if LANG_JA
        /// <summary>無効時のスライダーのハンドル（垂直方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Slider handle displayed when disabled (vertical)</summary>
        /// @endif
        SliderVerticalHandleDisabled,

        /// @if LANG_JA
        /// <summary>プログレスバーのベース</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Progress bar base</summary>
        /// @endif
        ProgressBarBase,

        /// @if LANG_JA
        /// <summary>標準のプログレスバー</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Standard progress bar</summary>
        /// @endif
        ProgressBarNormal,

        /// @if LANG_JA
        /// <summary>アニメーション付きのプログレスバー</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Progress bar with animation</summary>
        /// @endif
        ProgressBarAccelerator,

        /// @if LANG_JA
        /// <summary>スクロールバーの背景（水平方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Scroll bar background (horizontal)</summary>
        /// @endif
        ScrollBarHorizontalBackground,

        /// @if LANG_JA
        /// <summary>スクロールバー（水平方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Scroll bar (horizontal)</summary>
        /// @endif
        ScrollBarHorizontalBar,

        /// @if LANG_JA
        /// <summary>スクロールバーの背景（垂直方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Scroll bar background (vertical)</summary>
        /// @endif
        ScrollBarVerticalBackground,

        /// @if LANG_JA
        /// <summary>スクロールバー（垂直方向）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Scroll bar (vertical)</summary>
        /// @endif
        ScrollBarVerticalBar,

        /// @if LANG_JA
        /// <summary>有効時のエディットテキストの背景</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Edit text background when enabled</summary>
        /// @endif
        EditableTextBackgroundNormal,

        /// @if LANG_JA
        /// <summary>無効時のエディットテキストの背景</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Edit text background when disabled</summary>
        /// @endif
        EditableTextBackgroundDisabled,

        /// @if LANG_JA
        /// <summary>ビジーインジケータ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Busy indicator</summary>
        /// @endif
        BusyIndicator,

        /// @if LANG_JA
        /// <summary>ダイアログの背景</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Dialog background</summary>
        /// @endif
        DialogBackground,

        /// @if LANG_JA
        /// <summary>非表示中のページパネルのページ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Page of page panel when not displayed</summary>
        /// @endif
        PagePanelNormal,

        /// @if LANG_JA
        /// <summary>表示中のページパネルのページ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Page of page panel when displayed</summary>
        /// @endif
        PagePanelActive,

        /// @if LANG_JA
        /// <summary>リストアイテムのセパレータ（上端）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>List item separator (top edge)</summary>
        /// @endif
        ListPanelSeparatorTop,

        /// @if LANG_JA
        /// <summary>リストアイテムのセパレータ（下端）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>List item separator (bottom edge)</summary>
        /// @endif
        ListPanelSeparatorBottom,

        /// @if LANG_JA
        /// <summary>セクション</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Section</summary>
        /// @endif
        ListPanelSectionSeparator,

        /// @if LANG_JA
        /// <summary>メッセージダイアログの背景</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Message dialog background</summary>
        /// @endif
        MessageDialogBackground,

        /// @if LANG_JA
        /// <summary>メッセージダイアログのセパレータ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Message dialog separator</summary>
        /// @endif
        MessageDialogSeparator,

        /// @if LANG_JA
        /// <summary>スピンボックスの背景</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Background of spin box</summary>
        /// @endif
        SpinBoxBase,

        /// @if LANG_JA
        /// <summary>スピンボックスの中央</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Center of spin box</summary>
        /// @endif
        SpinBoxCenter,

        /// @if LANG_JA
        /// <summary>通常時のポップアップリストの背景</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Background of normal pop-up list</summary>
        /// @endif
        PopupListBackgroundNormal,

        /// @if LANG_JA
        /// <summary>押下時のポップアップリストの背景</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Background of pop-up list when pressed</summary>
        /// @endif
        PopupListBackgroundPressed,

        /// @if LANG_JA
        /// <summary>無効時のポップアップリストの背景</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Background of pop-up list when disabled</summary>
        /// @endif
        PopupListBackgroundDisabled,

        /// @if LANG_JA
        /// <summary>ポップアップリストの項目のフォーカス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Focus of the item in the pop-up list</summary>
        /// @endif
        PopupListItemFocus,

        /// @if LANG_JA
        /// <summary>通常時のバックボタン</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Back button normally displayed</summary>
        /// @endif
        BackButtonBackgroundNormal,

        /// @if LANG_JA
        /// <summary>無効時のバックボタン</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Back button displayed when disabled</summary>
        /// @endif
        BackButtonBackgroundDisabled,

        /// @if LANG_JA
        /// <summary>押下時のバックボタン</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Back button displayed when pressed</summary>
        /// @endif
        BackButtonBackgroundPressed,

        /// @if LANG_JA
        /// <summary>ナビゲーションバーの背景</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Background of navigation bar</summary>
        /// @endif
        NavigationBarBackground,

        /// @if LANG_JA
        /// <summary>角が丸い四角形のフォーカス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Focus of the rectangle with rounded corners</summary>
        /// @endif
        FocusRoundedCorner,

        /// @if LANG_JA
        /// <summary>四角形のフォーカス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Focus of the rectangle</summary>
        /// @endif
        FocusRectangle,

        /// @if LANG_JA
        /// <summary>円形のフォーカス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Focus of the circle</summary>
        /// @endif
        FocusCircle,

        /// @if LANG_JA
        /// <summary>リストアイテムのフォーカス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Focus of the list item</summary>
        /// @endif
        FocusListItem,
    }
}
