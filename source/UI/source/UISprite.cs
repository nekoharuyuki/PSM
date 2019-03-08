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
    /// <summary>複数の矩形をまとめて描画する</summary>
    /// <remarks>UI作成時に最も多用されるクラス</remarks>
    /// @endif
    /// @if LANG_EN
    /// <summary>Renders multiple rectangles at the same time</summary>
    /// <remarks>Class used most often to create UI</remarks>
    /// @endif
    public class UISprite : UIElement
    {
        bool needUpdateVertexAll = true;

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// <param name="maxUnitCount">矩形の最大数（1～）</param>
        /// <remarks>maxUnitCountは1以上の値にクランプされる。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// <param name="maxUnitCount">Maximum number of rectangles (1 to )</param>
        /// <remarks>maxUnitCount is clamped to a value of 1 or more.</remarks>
        /// @endif
        public UISprite(int maxUnitCount)
        {
            maxUnitCount = (maxUnitCount < 1) ? 1 : maxUnitCount;

            this.MaxUnitCount = maxUnitCount;
            unitCount = maxUnitCount;

            this.units = new UISpriteUnit[maxUnitCount];
            for (int i = 0; i < maxUnitCount; i++)
            {
                this.units[i] = new UISpriteUnit();
            }

            this.vertexBuffer = new VertexBuffer(4 * maxUnitCount, 6 * maxUnitCount - 2,
                VertexFormat.Float3, VertexFormat.Float4, VertexFormat.Float2);
        }

        /// @if LANG_JA
        /// <summary>このUISpriteで使用されているアンマネージドリソースを解放する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Frees the unmanaged resources being used with this UISprite.</summary>
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

        private int unitCount;

        /// @if LANG_JA
        /// <summary>表示する矩形の数を取得・設定する。</summary>
        /// <exception cref="ArgumentOutOfRangeException">負、もしくは MaxUnitCount より大きい値が設定されている。</exception>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the number of rectangles to display.</summary>
        /// <exception cref="ArgumentOutOfRangeException">A negative value or a value greater than MaxUnitount is set.</exception>
        /// @endif
        public int UnitCount
        {
            get { return unitCount; }
            set
            {
                if (value < 0 || value > MaxUnitCount)
                {
                    throw new ArgumentOutOfRangeException("UnitCount");
                }

                unitCount = value;
                needUpdateVertexAll = true;
            }
        }

        /// @if LANG_JA
        /// <summary>矩形の最大数を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the maximum number of rectangles.</summary>
        /// @endif
        public int MaxUnitCount
        {
            get;
            private set;
        }

        /// @if LANG_JA
        /// <summary>矩形を取得する。</summary>
        /// <param name="index">取得する矩形のインデックス</param>
        /// <returns>矩形</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the rectangle.</summary>
        /// <param name="index">Index of rectangle to be obtained</param>
        /// <returns>Rectangle</returns>
        /// @endif
        public UISpriteUnit GetUnit(int index)
        {
            return units[index];
        }

        private UISpriteUnit[] units;

        /// @if LANG_JA
        /// <summary>矩形を描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders a rectangle.</summary>
        /// @endif
        protected internal override void Render()
        {
            if (!isExistRenderedUnit())
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
            for (int i = 0; i < unitCount; i++)
            {
                var unit = units[i];
                needUpdatePosition |= unit.NeedUpdatePosition;
                needUpdateColor |= unit.NeedUpdateColor;
                needUpdateTexcoord |= unit.NeedUpdateTexcoord;
                unit.NeedUpdatePosition = false;
                unit.NeedUpdateColor = false;
                unit.NeedUpdateTexcoord = false;
            }

            // setup vertex positions
            if (needUpdateVertexAll | needUpdatePosition)
            {
                float[] positions = new float[3 * 4 * UnitCount];
                int j = 0;
                UISpriteUnit unit;
                for (int i = 0; i < this.unitCount; i++)
                {
                    unit = this.GetUnit(i);
                    positions[j++] = unit.X;
                    positions[j++] = unit.Y;
                    positions[j++] = unit.Z;
                    positions[j++] = unit.X;
                    positions[j++] = unit.Y + unit.Height;
                    positions[j++] = unit.Z;
                    positions[j++] = unit.X + unit.Width;
                    positions[j++] = unit.Y;
                    positions[j++] = unit.Z;
                    positions[j++] = unit.X + unit.Width;
                    positions[j++] = unit.Y + unit.Height;
                    positions[j++] = unit.Z;
                }
                this.vertexBuffer.SetVertices(0, positions, 0, 0, 4 * UnitCount);
            }

            // setup color
            if (needUpdateVertexAll | needUpdateColor)
            {
                UIColor[] colors = new UIColor[4 * UnitCount];
                int j = 0;
                UISpriteUnit unit;
                for (int i = 0; i < this.unitCount; i++)
                {
                    unit = units[i];
                    colors[j++] = unit.Color;
                    colors[j++] = unit.Color;
                    colors[j++] = unit.Color;
                    colors[j++] = unit.Color;
                }
                this.vertexBuffer.SetVertices(1, colors, 0, 0, 4 * UnitCount);
            }

            // setup texcoords
            if (texture != null)
            {
                if (needUpdateVertexAll | needUpdateTexcoord)
                {
                    float[] texcoords = new float[2 * 4 * UnitCount];
                    int j = 0;
                    UISpriteUnit unit;
                    for (int i = 0; i < this.unitCount; i++)
                    {
                        unit = units[i];
                        texcoords[j++] = unit.U1;
                        texcoords[j++] = unit.V1;
                        texcoords[j++] = unit.U1;
                        texcoords[j++] = unit.V2;
                        texcoords[j++] = unit.U2;
                        texcoords[j++] = unit.V1;
                        texcoords[j++] = unit.U2;
                        texcoords[j++] = unit.V2;
                    }
                    this.vertexBuffer.SetVertices(2, texcoords, 0, 0, 4 * UnitCount);
                }
            }
            else
            {
                // carry updating next time
                if ((needUpdateVertexAll || needUpdateTexcoord) && this.unitCount > 0)
                {
                    units[0].NeedUpdateTexcoord = true;
                }
            }

            // setup index
            if (needUpdateVertexAll)
            {
                ushort[] indices = new ushort[this.UnitCount * 6 - 2];
                ushort indexValue = 0;
                for (int i = 0; ;)
                {
                    // indices = {0,1,2,3,3,4, 4,5,6,7,7,8, 8,9,10,11...}
                    indices[i++] = indexValue++;
                    indices[i++] = indexValue++;
                    indices[i++] = indexValue++;
                    indices[i++] = indexValue;
                    if (i >= indices.Length)
                        break;
                    indices[i++] = indexValue++;
                    indices[i++] = indexValue;
                }
                this.vertexBuffer.SetIndices(indices, 0, 0, indices.Length);
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

            graphicsContext.DrawArrays(Sce.PlayStation.Core.Graphics.DrawMode.TriangleStrip, 0, this.UnitCount * 6 - 2);
        }

        bool isExistRenderedUnit()
        {
            if (unitCount == 0)
            {
                return false;
            }
            if (finalAlpha < MinimumRenderableAlpah)
            {
                return false;
            }


            bool exist = false;
            for (int i = 0; i < unitCount; i++)
            {
                var unit = units[i];
                if (unit.Color.A * finalAlpha >= MinimumRenderableAlpah
                    && unit.Width > 0f && unit.Height > 0f)
                {
                    exist = true;
                    break;
                }
            }
            return exist;
        }

        private VertexBuffer vertexBuffer;

    }


}
