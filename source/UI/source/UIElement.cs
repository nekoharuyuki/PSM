/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.UI
{


    /// @if LANG_JA
    /// <summary>ブレンドモード</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Blend mode</summary>
    /// @endif
    public enum BlendMode
    {
        /// @if LANG_JA
        /// <summary>ソースのアルファ成分の乗算によるブレンド</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Blend by multiplying the alpha components of the source</summary>
        /// @endif
        Half = 0,

        /// @if LANG_JA
        /// <summary>加算によるブレンド</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Blend by adding</summary>
        /// @endif
        Add,

        /// @if LANG_JA
        /// <summary>プリマルチプライドアルファのブレンド</summary>
        /// <remarks>Widget.RenderToTextureにより作成されたテクスチャを描画するための特殊なブレンドモード。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Premultiplied alpha blend</summary>
        /// <remarks>Special blend mode for rendering a texture created with Widget.RenderToTexture</remarks>
        /// @endif
        Premultiplied,

        /// @if LANG_JA
        /// <summary>ブレンドを行わない（不透明）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Do not blend (opaque)</summary>
        /// @endif
        Off,
    }

    /// @if LANG_JA
    /// <summary>描画要素の基底クラス</summary>
    /// <remarks>各ウィジェットの下にシーングラフを構成する。</remarks>
    /// @endif
    /// @if LANG_EN
    /// <summary>Base class of rendering element</summary>
    /// <remarks>Constructs a scene graph below each widget.</remarks>
    /// @endif
    public class UIElement : IDisposable
    {
#if DEBUG_UIELEMENT_INSTANCE_COUNT
        static public int _instanceCount = 0;
#endif

        internal const float MinimumRenderableAlpah = 1f / 255f;
        internal const float MaximumOpaqueAlpah = 254f / 255f;
        bool _needUpdateLocalToWorld = true;

        internal bool NeedUpdateLocalToWorld
        {
            get
            {
                return _needUpdateLocalToWorld;
            }
            set
            {
                // set all desendants needUpdateLocalToWorld to true
                if (_needUpdateLocalToWorld == false && value == true)
                {
                    var end = this.linkedTree.LastDescendant.NextAsList;
                    for (var child = this.linkedTree.NextAsList; child != end; child = child.NextAsList)
                    {
                        child.Value._needUpdateLocalToWorld = true;
                    }
                }
                _needUpdateLocalToWorld = value;
            }
        }


        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public UIElement()
        {
#if DEBUG_UIELEMENT_INSTANCE_COUNT
            if(!(this is RootUIElement)) _instanceCount++;
#endif

            this.linkedTree = new LinkedTree<UIElement>(this);

            this.Visible = true;

            this.BlendMode = BlendMode.Half;
            this.TextureFilterMode = TextureFilterMode.Linear;
            this.TextureWrapMode = TextureWrapMode.ClampToEdge;
            this.ZSort = false;
            this.transform3D = Matrix4.Identity;
            this.ZSortOffset = 0.0f;
            this.Culling = false;
        }


        /// @if LANG_JA
        /// <summary>このUIElement、および子UIElementの所有するアンマネージドリソースを解放する</summary>
        /// <remarks>このUIElementとそのツリー以下のすべてのUIElementを再帰的に解放します。
        /// 一度 Dispose() が呼ばれたUIElementは再利用することはできません。
        /// UIElement派生クラスで Dispose が必要なオブジェクトを作成した場合は、 DisposeSelf() をオーバーライドしてそれらの Dispose を呼ばなければなりません。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Frees the unmanaged resources held by this UIElement and child UIElements.</summary>
        /// <remarks>Recursively frees this UIElement and all UIElements in and below its tree.
        /// Once Dispose() has been called for a UIElement, it cannot be reused.
        /// If objects that require Dispose have been created with UIElement derivative classes, DisposeSelf() must be overridden and Dispose must be called for them.</remarks>
        /// @endif
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;

            Dispose(true);

#if DEBUG_UIELEMENT_INSTANCE_COUNT
            if(!(this is RootUIElement)) _instanceCount--;
#endif
        }

        /// @if LANG_JA
        /// <summary>このUIElement、および子UIElementの所有するアンマネージドリソースを解放する</summary>
        /// <remarks>親UIElementから自身を削除し、子UIElementに対して DisposeSelf() を再帰的に呼び出します。
        /// 派生クラスで Dispose を実装する場合は DisposeSelf() をオーバーライドしてください。</remarks>
        /// <param name="disposing">Dispose() から呼び出された場合 true</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Frees the unmanaged resources held by this UIElement and child UIElements.</summary>
        /// <remarks>Deletes itself from a parent UIElement and recursively calls DisposeSelf() for child UIElements.
        /// When implementing Dispose with a derivative class, override DisposeSelf().</remarks>
        /// <param name="disposing">true when called from Dispose()</param>
        /// @endif
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.linkedTree != null)
                {
                    this.linkedTree.RemoveChild();

                    for (var node = linkedTree; node != null; node = node.NextAsList)
                    {
                        if (node.Value != null)
                        {
                            node.Value.DisposeSelf();
                        }
                    }
                }
            }
        }

        /// @if LANG_JA
        /// <summary>このUIElementの所有するアンマネージドリソースを解放する</summary>
        /// <remarks>このメソッドは Dispose() からその子UIElementに対して再帰的に呼び出されます。
        /// 派生クラスで Dispose が必要なオブジェクトを作成した場合は、 このメソッドをオーバーライドしてそれらの Dispose を呼ばなければなりません。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Frees the unmanaged resources held by this UIElement</summary>
        /// <remarks>This method is recursively called from Dispose() for the child UIElements.
        /// If objects that require Dispose have been created with derivative classes, this method must be overridden and Dispose must be called for them.</remarks>
        /// @endif
        protected virtual void DisposeSelf()
        {
            if (this.texture != null)
            {
                this.texture.Dispose();
                this.texture = null;
            }
        }


        /// @if LANG_JA
        /// <summary>すでにDisposeされたかどうかを取得する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains whether Dispose is already performed</summary>
        /// @endif
        internal bool Disposed
        {
            get;
            private set;
        }


        /// @if LANG_JA
        /// <summary>親の座標系でのX座標を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the X coordinate in the parent coordinate system.</summary>
        /// @endif
        public float X
        {
            get
            {
                return transform3D.M41;
            }
            set
            {
                transform3D.M41 = value;
                NeedUpdateLocalToWorld = true;
            }
        }


        /// @if LANG_JA
        /// <summary>親の座標系でのY座標を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the Y coordinate in the parent coordinate system.</summary>
        /// @endif
        public float Y
        {
            get
            {
                return transform3D.M42;
            }
            set
            {
                transform3D.M42 = value;
                NeedUpdateLocalToWorld = true;
            }
        }


        /// @if LANG_JA
        /// <summary>親の座標系での位置を設定する。</summary>
        /// <param name="x">親の座標系でのX座標</param>
        /// <param name="y">親の座標系でのY座標</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets the position in the parent coordinate system.</summary>
        /// <param name="x">X coordinate in the parent coordinate system</param>
        /// <param name="y">Y coordinate in the parent coordinate system</param>
        /// @endif
        public void SetPosition(float x, float y)
        {
            transform3D.M41 = x;
            transform3D.M42 = y;
            NeedUpdateLocalToWorld = true;
        }


        /// @if LANG_JA
        /// <summary>透明度を取得・設定する。（０～１）</summary>
        /// <remarks>すべての子要素に透明度が乗算されて表示される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the transparency. (0 - 1)</summary>
        /// <remarks>Multiplies the transparency by all child elements and displays it.</remarks>
        /// @endif
        public float Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        private float alpha = 1.0f;


        /// @if LANG_JA
        /// <summary>表示するかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to display.</summary>
        /// @endif
        public bool Visible
        {
            get;
            set;
        }


        /// @if LANG_JA
        /// <summary>テクスチャフィルタのモードを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the texture filter mode.</summary>
        /// @endif
        public TextureFilterMode TextureFilterMode
        {
            get
            {
                return this.textureFilterMode;
            }
            set
            {
                this.textureFilterMode = value;
            }
        }

        private TextureFilterMode textureFilterMode;


        /// @if LANG_JA
        /// <summary>テクスチャラップのモードを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the texture wrap mode.</summary>
        /// @endif
        public TextureWrapMode TextureWrapMode
        {
            get
            {
                return this.textureWrapMode;
            }
            set
            {
                this.textureWrapMode = value;
            }
        }

        private TextureWrapMode textureWrapMode;


        /// @if LANG_JA
        /// <summary>画像を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the image.</summary>
        /// @endif
        public ImageAsset Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (this.texture != null)
                {
                    this.texture.Dispose();
                    this.texture = null;
                }

                this.image = value;
            }
        }

        private ImageAsset image = null;

        /// @if LANG_JA
        /// <summary>テクスチャを取得する</summary>
        /// <returns>テクスチャ</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the texture</summary>
        /// <returns>Textures</returns>
        /// @endif
        protected Texture2D GetTexture()
        {
            if (this.texture == null && this.image != null)
            {
                this.texture = this.image.CloneTexture();
            }
            if (this.texture != null)
            {
                this.texture.SetFilter(this.TextureFilterMode);
                this.texture.SetWrap(this.TextureWrapMode);
            }
            return this.texture;
        }

        private Texture2D texture = null;

        /// @if LANG_JA
        /// <summary>Zソートするかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to Z sort.</summary>
        /// @endif
        public bool ZSort
        {
            get;
            set;
        }


        /// @if LANG_JA
        /// <summary>親エレメントを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the parent element.</summary>
        /// @endif
        public UIElement Parent
        {
            get
            {
                if (linkedTree.Parent != null)
                {
                    return linkedTree.Parent.Value;
                }
                else
                {
                    return null;
                }
            }
        }


        /// @if LANG_JA
        /// <summary>子エレメントを取得する。</summary>
        /// <remarks>コレクションを反復処理する列挙子を返す。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the child element.</summary>
        /// <remarks>Returns the enumerator that repetitively handles the collection.</remarks>
        /// @endif
        public IEnumerable<UIElement> Children
        {
            get
            {
                for (LinkedTree<UIElement> target = this.linkedTree.FirstChild;
                    target != null;
                    target = target.NextSibling)
                {
                    if (target.Value != null)
                    {
                        yield return target.Value;
                    }
                    else
                    {
                        yield break;
                    }

                }
            }
        }


        /// @if LANG_JA
        /// <summary>子エレメントを先頭に追加する。</summary>
        /// <param name="child">追加する子エレメント</param>
        /// <remarks>既に追加されている場合は先頭に移動する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Adds a child element to the beginning.</summary>
        /// <param name="child">Child element to be added</param>
        /// <remarks>Moves it to the beginning if it has already been added.</remarks>
        /// @endif
        public void AddChildFirst(UIElement child)
        {
            linkedTree.AddChildFirst(child.linkedTree);
            child.NeedUpdateLocalToWorld = true;
        }


        /// @if LANG_JA
        /// <summary>子エレメントを末尾に追加する。</summary>
        /// <param name="child">追加する子エレメント</param>
        /// <remarks>既に追加されている場合は末尾に移動する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Adds a child element to the end.</summary>
        /// <param name="child">Child element to be added</param>
        /// <remarks>Moves it to the end if it has already been added.</remarks>
        /// @endif
        public void AddChildLast(UIElement child)
        {
            linkedTree.AddChildLast(child.linkedTree);
            child.NeedUpdateLocalToWorld = true;
        }


        /// @if LANG_JA
        /// <summary>指定した子エレメントを直前に挿入する。</summary>
        /// <param name="child">挿入する子エレメント</param>
        /// <param name="nextChild">挿入する子エレメントの直後となる子エレメント</param>
        /// <remarks>既に追加されている場合は指定した子エレメントの直前に移動する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Inserts a specified child element immediately before.</summary>
        /// <param name="child">Child element to be inserted</param>
        /// <param name="nextChild">Child element to come immediately after the child element to be inserted</param>
        /// <remarks>Moves it immediately before the specified child element if it has already been added.</remarks>
        /// @endif
        public void InsertChildBefore(UIElement child, UIElement nextChild)
        {
            child.linkedTree.InsertChildBefore(nextChild.linkedTree);
            child.NeedUpdateLocalToWorld = true;
        }


        /// @if LANG_JA
        /// <summary>指定した子エレメントを直後に挿入する。</summary>
        /// <param name="child">挿入する子エレメント</param>
        /// <param name="prevChild">挿入する子エレメントの直前となる子エレメント</param>
        /// <remarks>既に追加されている場合は指定した子エレメントの直後に移動する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Inserts a specified child element immediately after.</summary>
        /// <param name="child">Child element to be inserted</param>
        /// <param name="prevChild">Child element to come immediately before the child element to be inserted</param>
        /// <remarks>Moves it immediately after the specified child element if it has already been added.</remarks>
        /// @endif
        public void InsertChildAfter(UIElement child, UIElement prevChild)
        {
            child.linkedTree.InsertChildAfter(prevChild.linkedTree);
            child.NeedUpdateLocalToWorld = true;
        }


        /// @if LANG_JA
        /// <summary>指定された子エレメントを削除する。</summary>
        /// <param name="child">削除する子エレメント</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Deletes the specified child element.</summary>
        /// <param name="child">Child element to be deleted</param>
        /// @endif
        public void RemoveChild(UIElement child)
        {
            child.linkedTree.RemoveChild();
            child.NeedUpdateLocalToWorld = true;
        }


        /// @if LANG_JA
        /// <summary>ブレンドモードを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the blend mode.</summary>
        /// @endif
        public BlendMode BlendMode
        {
            get;
            set;
        }


        /// @if LANG_JA
        /// <summary>親の座標系への変換行列を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the transformation matrix to the parent coordinate system.</summary>
        /// @endif
        public Matrix4 Transform3D
        {
            get { return transform3D; }
            set
            {
                transform3D = value;
                NeedUpdateLocalToWorld = true;
            }
        }

        internal Matrix4 transform3D;

        /// @if LANG_JA
        /// <summary>Zソートオフセットを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the Z-sort offset.</summary>
        /// @endif
        public float ZSortOffset
        {
            get;
            set;
        }


        /// @if LANG_JA
        /// <summary>カリングするかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to perform culling.</summary>
        /// @endif
        public bool Culling
        {
            get;
            set;
        }


        /// @if LANG_JA
        /// <summary>描画状態を設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets the rendering status.</summary>
        /// @endif
        protected internal virtual void SetupDrawState()
        {
            GraphicsContext graphicsContext = UISystem.GraphicsContext;

            if (UISystem.IsOffScreenRendering)
            {
                switch (this.BlendMode)
                {
                    case BlendMode.Off:
                        graphicsContext.Enable(EnableMode.Blend, true);
                        graphicsContext.SetBlendFuncRgb(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.Zero);
                        graphicsContext.SetBlendFuncAlpha(BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.Zero);
                        break;
                    case BlendMode.Half:
                        graphicsContext.Enable(EnableMode.Blend, true);
                        graphicsContext.SetBlendFuncRgb(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
                        graphicsContext.SetBlendFuncAlpha(BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.OneMinusSrcAlpha);
                        break;
                    case BlendMode.Add:
                        graphicsContext.Enable(EnableMode.Blend, true);
                        graphicsContext.SetBlendFuncRgb(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.One);
                        graphicsContext.SetBlendFuncAlpha(BlendFuncMode.Add, BlendFuncFactor.Zero, BlendFuncFactor.One);
                        break;
                    case BlendMode.Premultiplied:
                        graphicsContext.Enable(EnableMode.Blend, true);
                        graphicsContext.SetBlendFuncRgb(BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.OneMinusSrcAlpha);
                        graphicsContext.SetBlendFuncAlpha(BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.OneMinusSrcAlpha);
                        break;
                }
            }
            else
            {
                switch (this.BlendMode)
                {
                    case BlendMode.Off:
                        graphicsContext.Enable(EnableMode.Blend, false);
                        break;
                    case BlendMode.Half:
                        graphicsContext.Enable(EnableMode.Blend, true);
                        graphicsContext.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
                        break;
                    case BlendMode.Add:
                        graphicsContext.Enable(EnableMode.Blend, true);
                        graphicsContext.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.One);
                        break;
                    case BlendMode.Premultiplied:
                        graphicsContext.Enable(EnableMode.Blend, true);
                        graphicsContext.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.OneMinusSrcAlpha);
                        break;
                }
            }

            graphicsContext.Enable(EnableMode.CullFace, this.Culling);

            // as-is
            if (this.BlendMode == BlendMode.Premultiplied)
            {
                if(internalShaderType == InternalShaderType.TextureRgba)
                    internalShaderType = InternalShaderType.PremultipliedTexture;
            }
            else
            {
                if (internalShaderType == InternalShaderType.PremultipliedTexture)
                    internalShaderType = InternalShaderType.TextureRgba;
            }
        }


        /// @if LANG_JA
        /// <summary>エレメントを描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders an element.</summary>
        /// @endif
        protected internal virtual void Render()
        {
        }


        /// @if LANG_JA
        /// <summary>最終的なアルファ値を計算する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Calculates the final alpha value.</summary>
        /// @endif
        protected internal virtual void SetupFinalAlpha()
        {
            this.finalAlpha = this.Alpha * this.Parent.finalAlpha;
        }


        /// @if LANG_JA
        /// <summary>Zソートで使用する値を設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets the value to use with Z sort.</summary>
        /// @endif
        protected internal void SetupSortValue()
        {
            // TODO: LeftTop regardless of parentWidget.PivotType??
            updateLocalToWorld();
            this.zSortValue = this.localToWorld.M43 + this.ZSortOffset;
        }


        /// @if LANG_JA
        /// <summary>シェーダープログラムを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the shader program.</summary>
        /// @endif
        public ShaderType ShaderType
        {
            get
            {
                switch (internalShaderType)
                {
                    case InternalShaderType.SolidFill:
                        return ShaderType.SolidFill;

                    case InternalShaderType.TextureAlpha:
                        return ShaderType.TextTexture;

                    case InternalShaderType.TextureRgba:
                    default:
                        return ShaderType.Texture;
                }
            }
            set
            {
                this.InternalShaderType = (InternalShaderType)value;
            }
        }

        /// @if LANG_JA
        /// <summary>内部で使う用のシェーダータイプ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Shader type for use inside</summary>
        /// @endif
        internal InternalShaderType InternalShaderType
        {
            get { return internalShaderType; }
            set
            {
                if (internalShaderType != value)
                {
                    if(ShaderUniforms != null)
                        ShaderUniforms.Clear();
                    internalShaderType = value;
                }
            }
        }

        private InternalShaderType internalShaderType = InternalShaderType.SolidFill;


        /// @if LANG_JA
        /// <summary>シェーダープログラムで使用するUniformのテーブルを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the Uniform table to use with the shader program.</summary>
        /// @endif
        internal Dictionary<string, float[]> ShaderUniforms
        {
            get;
            set;
        }

        internal LinkedTree<UIElement> linkedTree;
        internal float finalAlpha = 1.0f;
        internal Matrix4 localToWorld;

        /// @if LANG_JA
        /// <summary>ローカル座標系からワールド座標系に変換する行列を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the matrix to be converted from the local coordinate system to the world coordinate system.</summary>
        /// @endif
        public Matrix4 LocalToWorld
        {
            get
            {
                updateLocalToWorld();
                return this.localToWorld;
            }
        }

        internal virtual void updateLocalToWorld()
        {
            if (_needUpdateLocalToWorld)
            {
                if (Parent != null)
                {
                    Parent.updateLocalToWorld();
                    Parent.localToWorld.Multiply(ref this.transform3D, out this.localToWorld);
                }
                else
                {
                    this.localToWorld = this.transform3D;
                }
                NeedUpdateLocalToWorld = false;
            }
        }

        internal float zSortValue = 0.0f;
        internal UIElement nextZSortElement = null;
    }


}
