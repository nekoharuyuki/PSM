/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.UI
{


    /// @if LANG_JA
    /// <summary>プリミティブ</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Primitive</summary>
    /// @endif
    public class UIPrimitive : UIElement
    {
        bool needUpdateVertexAll = true;
        bool needUpdateIndices = true;

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// <param name="drawMode">描画モード</param>
        /// <param name="maxVertexCount">頂点の数</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// <param name="drawMode">Rendering mode</param>
        /// <param name="maxVertexCount">Number of vertices</param>
        /// @endif
        public UIPrimitive(DrawMode drawMode, int maxVertexCount)
            : this(drawMode, maxVertexCount, 0)
        {
        }

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// <param name="drawMode">描画モード</param>
        /// <param name="maxVertexCount">頂点の数</param>
        /// <param name="maxIndexCount">インデックスの数（０ならばインデクス配列を使用しない）</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// <param name="drawMode">Rendering mode</param>
        /// <param name="maxVertexCount">Number of vertices</param>
        /// <param name="maxIndexCount">Number of indices (do not use the index array if 0)</param>
        /// @endif
        public UIPrimitive(DrawMode drawMode, int maxVertexCount, int maxIndexCount)
        {
            this.DrawMode = drawMode;

            maxVertexCount = (maxVertexCount > 0) ? maxVertexCount : 0;

            this.vertexCount = maxVertexCount;
            this.MaxVertexCount = maxVertexCount;

            this.vertices = new UIPrimitiveVertex[maxVertexCount];
            for (int i = 0; i < maxVertexCount; i++)
            {
                this.vertices[i] = new UIPrimitiveVertex();
            }

            this.indexCount = maxIndexCount;
            this.maxIndexCount = maxIndexCount;
            this.indices = new ushort[this.maxIndexCount];

            this.vertexBuffer = new VertexBuffer(maxVertexCount, maxIndexCount,
                VertexFormat.Float3, VertexFormat.Float4, VertexFormat.Float2);
        }

        /// @if LANG_JA
        /// <summary>このUIPrimitiveで使用されているアンマネージドリソースを解放する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Frees the unmanaged resources being used with this UIPrimitive.</summary>
        /// @endif
        protected override void DisposeSelf()
        {
            if (this.vertexBuffer != null)
            {
                this.vertexBuffer.Dispose();
                this.vertexBuffer = null;
            }
            base.DisposeSelf();
        }

        /// @if LANG_JA
        /// <summary>描画モードを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the rendering mode.</summary>
        /// @endif
        public DrawMode DrawMode
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>頂点数を取得・設定する。</summary>
        /// <exception cref="ArgumentOutOfRangeException">負、もしくはMaxVertexCountより大きい値が設定されている。</exception>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the number of vertices.</summary>
        /// <exception cref="ArgumentOutOfRangeException">A negative value or a value greater than MaxVertexCount is set.</exception>
        /// @endif
        public int VertexCount
        {
            get { return vertexCount; }
            set
            {
                if (value < 0 || value > MaxVertexCount)
                {
                    throw new ArgumentOutOfRangeException();
                }
                vertexCount = value;
                needUpdateVertexAll = true;
            }
        }

        private int vertexCount;

        /// @if LANG_JA
        /// <summary>頂点数の最大値を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the maximum value of the number of vertices.</summary>
        /// @endif
        public int MaxVertexCount
        {
            get;
            private set;
        }

        private int maxIndexCount;

        /// @if LANG_JA
        /// <summary>インデックス数の最大値を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the maximum value of the number of indices.</summary>
        /// @endif
        public int MaxIndexCount
        {
            get { return maxIndexCount; }
        }

        private ushort[] indices;
        private int indexCount;

        /// @if LANG_JA
        /// <summary>インデックス数を取得・設定する。</summary>
        /// <exception cref="ArgumentOutOfRangeException">負、もしくは MaxIndexCount より大きい値が設定されている。</exception>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the number of indices.</summary>
        /// <exception cref="ArgumentOutOfRangeException">A negative value or a value greater than MaxIndexCount is set.</exception>
        /// @endif
        public int IndexCount
        {
            get { return indexCount; }
            set
            {
                if (value < 0 || value > maxIndexCount)
                {
                    throw new ArgumentOutOfRangeException();
                }
                indexCount = value;
                needUpdateIndices = true;
            }
        }

        /// @if LANG_JA
        /// <summary>インデックス配列を設定する。</summary>
        /// <param name="indices">インデックス配列</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets the index array.</summary>
        /// <param name="indices">Index array</param>
        /// @endif
        public void SetIndices(ushort[] indices)
        {
            int length = maxIndexCount > indices.Length ? indices.Length : MaxIndexCount;
            for (int i = 0; i < length; i++)
            {
                this.indices[i] = indices[i];
            }
            needUpdateIndices = true;
        }

        /// @if LANG_JA
        /// <summary>頂点を取得する。</summary>
        /// <param name="index">インデックス</param>
        /// <returns>頂点</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the vertex.</summary>
        /// <param name="index">Index</param>
        /// <returns>Vertex</returns>
        /// @endif
        public UIPrimitiveVertex GetVertex(int index)
        {
            return vertices[index];
        }

        private UIPrimitiveVertex[] vertices;

        /// @if LANG_JA
        /// <summary>プリミティブを描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders the primitive.</summary>
        /// @endif
        protected internal override void Render()
        {
            if (!isExistRenderedVertex())
            {
                return;
            }

            GraphicsContext graphicsContext = UISystem.GraphicsContext;

            var shader = ShaderProgramManager.GetShaderProgramUnit(InternalShaderType);
            graphicsContext.SetShaderProgram(shader.ShaderProgram);

            // texture
            Texture2D texture = this.GetTexture();
            graphicsContext.SetTexture(0, texture);

            // check VertexBuffer update
            bool needUpdatePosition = false;
            bool needUpdateColor = false;
            bool needUpdateTexcoord = false;
            for (int i = 0; i < vertexCount; i++)
            {
                var vrtx = vertices[i];
                needUpdatePosition |= vrtx.NeedUpdatePosition;
                needUpdateColor |= vrtx.NeedUpdateColor;
                needUpdateTexcoord |= vrtx.NeedUpdateTexcoord;
                vrtx.NeedUpdatePosition = false;
                vrtx.NeedUpdateColor = false;
                vrtx.NeedUpdateTexcoord = false;
            }

            // setup vertex positions
            if (needUpdateVertexAll || needUpdatePosition)
            {
                float[] positions = new float[VertexCount * 3];
                int j = 0;
                UIPrimitiveVertex vertex;
                for (int i = 0; i < VertexCount; i++)
                {
                    vertex = vertices[i];
                    positions[j++] = vertex.X;
                    positions[j++] = vertex.Y;
                    positions[j++] = vertex.Z;
                }
                this.vertexBuffer.SetVertices(0, positions, 0, 0, VertexCount);
            }

            // setup color
            if (needUpdateVertexAll || needUpdatePosition)
            {
                float[] colors = new float[VertexCount * 4];
                int j = 0;
                UIPrimitiveVertex vertex;
                for (int i = 0; i < VertexCount; i++)
                {
                    vertex = vertices[i];
                    colors[j++] = vertex.Color.R;
                    colors[j++] = vertex.Color.G;
                    colors[j++] = vertex.Color.B;
                    colors[j++] = vertex.Color.A;
                }
                this.vertexBuffer.SetVertices(1, colors, 0, 0, VertexCount);
            }

            // setup texcoords
            if (texture != null)
            {
                if (needUpdateVertexAll || needUpdateTexcoord)
                {
                    float[] texcoords = new float[VertexCount * 2];
                    int j = 0;
                    UIPrimitiveVertex vertex;
                    for (int i = 0; i < VertexCount; i++)
                    {
                        vertex = vertices[i];
                        texcoords[j++] = vertex.U;
                        texcoords[j++] = vertex.V;
                    }
                    this.vertexBuffer.SetVertices(2, texcoords, 0, 0, VertexCount);
                }
            }
            else
            {
                // carry updating next time
                if ((needUpdateVertexAll || needUpdateTexcoord) && this.vertexCount > 0)
                {
                    vertices[0].NeedUpdateTexcoord = true;
                }
            }

            // setup index
            if (needUpdateIndices)
            {
                if (indexCount > 0)
                {
                    this.vertexBuffer.SetIndices(indices, 0, 0, indexCount);
                }
                needUpdateIndices = false;
            }

            needUpdateVertexAll = false;
            graphicsContext.SetVertexBuffer(0, this.vertexBuffer);


            // Uniforms are set to ShaderProgram.
            // setup local to clip matrix
            updateLocalToWorld();
            Matrix4 mvpMatrix;
            UISystem.viewProjectionMatrix.Multiply(ref this.localToWorld, out mvpMatrix);
            shader.ShaderProgram.SetUniformValue(shader.UniformIndexOfModelViewProjection, ref mvpMatrix);
            // alpha
            shader.ShaderProgram.SetUniformValue(shader.UniformIndexOfAlpha, this.finalAlpha);
            // other uniforms
            if(ShaderUniforms != null)
            {
                float[] values;
                foreach (var name in shader.OtherUniformNames)
                {
                    if (ShaderUniforms.TryGetValue(name, out values))
                    {
                        shader.ShaderProgram.SetUniformValue(shader.Uniforms[name], values);
                    }
                }
            }

            int count = indexCount > 0 ? indexCount : VertexCount;
            graphicsContext.DrawArrays(DrawMode, 0, count);
        }

        bool isExistRenderedVertex()
        {
            if (this.VertexCount == 0)
            {
                return false;
            }
            if (this.finalAlpha < MinimumRenderableAlpah)
            {
                return false;
            }

            return true;
        }

        private VertexBuffer vertexBuffer;

    }


}
