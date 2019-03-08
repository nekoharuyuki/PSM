/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using System.Reflection;
using System.IO;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>シェーダープログラムの種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Type of shader program</summary>
        /// @endif
        public enum ShaderType : int
        {
            /// @if LANG_JA
            /// <summary>単色塗り、または頂点カラーの描画</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>One-color fill or vertex color rendering</summary>
            /// @endif
            SolidFill = 0,

            /// @if LANG_JA
            /// <summary>通常のテクスチャ描画 (RGBAフォーマットのテクスチャ)</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Normal texture rendering (RGBA format texture)</summary>
            /// @endif
            Texture = 1,

            /// @if LANG_JA
            /// <summary>テキストの描画 (Alphaフォーマットのテクスチャ)</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Text rendering (Alpha format texture)</summary>
            /// @endif
            TextTexture = 2,

            /// @if LANG_JA
            /// <summary>オフスクリーンテクスチャの描画（RenderToTextureで描画したテクスチャ）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Offscreen texture rendering (texture rendered with RenderToTexture)</summary>
            /// @endif
            [Obsolete("use ShaderType.Texture and BlendMode.Premultiplied")]
            OffscreenTexture = ShaderType.Texture,
        }

        /// @internal
        /// @if LANG_JA
        /// <summary>シェーダープログラムの種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Type of shader program</summary>
        /// @endif
        internal enum InternalShaderType : int
        {
            /// @if LANG_JA
            /// <summary>単色塗り</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>One-color fill</summary>
            /// @endif
            SolidFill = 0,

            /// @if LANG_JA
            /// <summary>テクスチャ描画</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Render texture</summary>
            /// @endif
            TextureRgba = 1,

            /// @if LANG_JA
            /// <summary>テキスト描画</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Render text</summary>
            /// @endif
            TextureAlpha = 2,

            /// @if LANG_JA
            /// <summary>オフスクリーンテクスチャの描画（RenderToTextureで描画したテクスチャ）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Offscreen texture rendering (texture rendered with RenderToTexture)</summary>
            /// @endif
            PremultipliedTexture = 3,

            /// @if LANG_JA
            /// <summary>影付きのテキスト描画</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Render text with shadow</summary>
            /// @endif
            TextureAlphaShadow = 4,

            /// @if LANG_JA
            /// <summary>LiveScrollPanel専用</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Only for LiveScrollPanel</summary>
            /// @endif
            LiveScrollPanel = 5,

            /// @if LANG_JA
            /// <summary>LiveSphere専用</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Only for LiveSphere</summary>
            /// @endif
            LiveSphere = 6,

            /// <summary>dummy</summary>
            _count
        }


        /// @internal
        /// @if LANG_JA
        /// <summary>シェーダープログラムの管理を行う</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Manage shader program</summary>
        /// @endif
        internal static class ShaderProgramManager
        {
#if LOAD_ASSETS_FROM_FILE_SYSTEM
            private static string shaderFileNamePrifix = "shaders/";
#else
            private static string shaderFileNamePrifix = "Sce.PlayStation.HighLevel.UI.shaders.";
#endif

            /// @if LANG_JA
            /// <summary>シェーダープログラムのファイル名</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Shader program filename</summary>
            /// @endif
            private static string[,] shaderFileName = new string[,]
            {
                {"basic.vp.cgx", "solid_fill.fp.cgx"},                     //0
                {"basic.vp.cgx", "texture_rgba.fp.cgx"},                   //1
                {"basic.vp.cgx", "texture_a8.fp.cgx"},                     //2
                {"premultiplied.vp.cgx", "texture_rgba.fp.cgx"},           //3
                {"basic.vp.cgx", "texture_a8_shadow.fp.cgx"},              //4
                {"premultiplied.vp.cgx", "live_scroll.fp.cgx"},            //5
                {"live_sphere.vp.cgx", "live_sphere.fp.cgx"},              //6
            };

            const string aPosition = "a_Position";
            const string aColor = "a_Color";
            const string aTexcoord = "a_TexCoord";

            const string uModelViewProjection = "u_WorldMatrix";
            const string uAlpha = "u_Alpha";
            
            const string sTexture = "s_Texture";

            /// @if LANG_JA
            /// <summary>初期化する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Initializes.</summary>
            /// @endif
            internal static void Initialize()
            {
                shaderPrograms = new ShaderProgramUnit[(int)InternalShaderType._count];

#if !SHADER_DELAYED_LOAD
                for (int i = 0; i < shaderPrograms.Length; i++)
                {
                    loadShaderProgramUnit(i);
                }
#endif
            }

            
            internal static ShaderProgramUnit GetShaderProgramUnit(InternalShaderType type)
            {
                int index = (int)type;
                UIDebug.Assert(index >= 0 && index < shaderPrograms.Length);
                
#if SHADER_DELAYED_LOAD
                if(shaderPrograms[index] == null)
                {
                    loadShaderProgramUnit(index);
                }
#endif
                return shaderPrograms[index];
            }
            
            private static void loadShaderProgramUnit(int index)
            {
                UIDebug.Assert(shaderPrograms[index] == null);
                
                ShaderProgramUnit unit = new ShaderProgramUnit();

#if LOAD_ASSETS_FROM_FILE_SYSTEM
                UIDebug.AssetLog("loading " + shaderFileName[index, 0] + ", " + shaderFileName[index, 1] + " from the file system . . . ");
                unit.ShaderProgram = new ShaderProgram(
                    shaderFileNamePrifix + shaderFileName[index, 0],
                    shaderFileNamePrifix + shaderFileName[index, 1]);
#else
                UIDebug.LogAsset("loading " + shaderFileName[index, 0] + ", " + shaderFileName[index, 1] + " from the assembly file . . . ");

                Assembly resourceAssembly = Assembly.GetExecutingAssembly();

                Byte[] dataBufferVertex = null;
                Byte[] dataBufferFragment = null;
                
                using(Stream fileStreamVertex = 
                      resourceAssembly.GetManifestResourceStream(shaderFileNamePrifix + shaderFileName[index, 0]))
                {
                    if(fileStreamVertex == null)
                        throw new FileNotFoundException("Shader file not found.", shaderFileName[index, 0]);
                    dataBufferVertex = new Byte[fileStreamVertex.Length];
                    fileStreamVertex.Read(dataBufferVertex, 0, dataBufferVertex.Length);
                }

                using(Stream fileStreamFragment = 
                      resourceAssembly.GetManifestResourceStream(shaderFileNamePrifix + shaderFileName[index, 1]))
                {
                    if(fileStreamFragment == null)
                        throw new FileNotFoundException("Shader file not found.", shaderFileName[index, 1]);
                    dataBufferFragment = new Byte[fileStreamFragment.Length];
                    fileStreamFragment.Read(dataBufferFragment, 0, dataBufferFragment.Length);
                }

                unit.ShaderProgram = new ShaderProgram(dataBufferVertex, dataBufferFragment);
#endif

                unit.ShaderProgram.SetAttributeBinding(0, aPosition);
                unit.AttributeIndexOfPosition = 0;
                unit.ShaderProgram.SetAttributeBinding(1, aColor);
                unit.AttributeIndexOfColor = 1;
                if (unit.ShaderProgram.FindAttribute(aTexcoord) >= 0)
                {
                    unit.ShaderProgram.SetAttributeBinding(2, aTexcoord);
                    unit.AttributeIndexOfTexcoord = 2;
                }

                int uniformCount = unit.ShaderProgram.UniformCount;
                unit.Uniforms = new Dictionary<string, int>(uniformCount);
                unit.OtherUniformNames = new List<string>(Math.Min(uniformCount - 2, 0));
                for (int j = 0; j < unit.ShaderProgram.UniformCount; j++)
                {
                    string name = unit.ShaderProgram.GetUniformName(j);
                    if (!unit.Uniforms.ContainsKey(name))
                    {
                        unit.Uniforms.Add(name, j);
                        if (name == uModelViewProjection)
                        {
                            unit.UniformIndexOfModelViewProjection = j;
                        }
                        else if (name == uAlpha)
                        {
                            unit.UniformIndexOfAlpha = j;
                        }
                        else if (name != sTexture)
                        {
                            unit.OtherUniformNames.Add(name);
                        }
                    }
                }

                shaderPrograms[index] = unit;
            }


            /// @if LANG_JA
            /// <summary>終了する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Terminates.</summary>
            /// @endif
            internal static void Terminate(GraphicsContext graphics)
            {
                ShaderProgram currentShaderProgram = null;
                if (graphics != null)
                {
                    currentShaderProgram = graphics.GetShaderProgram();
                }

                foreach (ShaderProgramUnit unit in shaderPrograms)
                {
                    if(unit != null)
                    {
                        if(unit.ShaderProgram == currentShaderProgram)
                        {
                            graphics.SetShaderProgram(null);
                        }
                        unit.ShaderProgram.Dispose();
                    }
                }
            }


            /// @if LANG_JA
            /// <summary>シェーダープログラムを取得する。</summary>
            /// <param name="type">シェーダープログラムの種類</param>
            /// <returns>シェーダープログラムのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the shader program.</summary>
            /// <param name="type">Type of shader program</param>
            /// <returns>Shader program instance</returns>
            /// @endif
            public static ShaderProgram GetShaderProgram(InternalShaderType type)
            {
                return GetShaderProgramUnit(type).ShaderProgram;
            }


            /// @if LANG_JA
            /// <summary>Uniformのテーブルを取得する。</summary>
            /// <param name="type">シェーダープログラムの種類</param>
            /// <returns>Uniformのテーブル</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains a Uniform table.</summary>
            /// <param name="type">Type of shader program</param>
            /// <returns>Uniform table</returns>
            /// @endif
            public static Dictionary<string, int> GetUniforms(InternalShaderType type)
            {
                return GetShaderProgramUnit(type).Uniforms;
            }

            private static ShaderProgramUnit[] shaderPrograms;


            internal class ShaderProgramUnit
            {
                public ShaderProgram ShaderProgram;
                public Dictionary<string, int> Uniforms;
                public List<string> OtherUniformNames;
                public int UniformIndexOfModelViewProjection = -1;
                public int UniformIndexOfAlpha = -1;
                public int AttributeIndexOfPosition = -1;
                public int AttributeIndexOfColor = -1;
                public int AttributeIndexOfTexcoord = -1;
            }

        }


    }
}
