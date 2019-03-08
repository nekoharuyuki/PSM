/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// @if LANG_EN
	/// <summary>
	/// A simple particle system effect.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>シンプルなパーティクルシステムエフェクト。
	/// </summary>
	/// @endif
	public class ParticleSystem : System.IDisposable
	{
		/// @if LANG_EN
		/// <summary>
		/// Default shader, texture modulated by a color.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>デフォルトのシェーダー。テクスチャの色は変調されます。
		/// </summary>
		/// @endif
		public class ParticleShaderDefault : IParticleShader, System.IDisposable
		{
			public ShaderProgram m_shader_program;

			public ParticleShaderDefault()
			{
				m_shader_program = Common.CreateShaderProgram( "cg/particles.cgx" );

				m_shader_program.SetUniformBinding( 0, "MVP" );
				m_shader_program.SetUniformBinding( 1, "Color" );
//				m_shader_program.SetUniformBinding( 2, "WorldScale" );

				m_shader_program.SetAttributeBinding( 0, "vin_data" );
				m_shader_program.SetAttributeBinding( 1, "vin_color" );

				Matrix4 identity = Matrix4.Identity;
				SetMVP( ref identity );
				SetColor( ref Colors.White );
//				SetWorldScale( ref  );
			}

			/// @if LANG_EN
			/// <summary>
			/// Dispose implementation.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>Disposeの実装。
			/// </summary>
			/// @endif
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if(disposing)
				{
					Common.DisposeAndNullify< ShaderProgram >( ref m_shader_program );
				}
			}

			public ShaderProgram GetShaderProgram() { return m_shader_program;}
			public void SetMVP( ref Matrix4 value ) { m_shader_program.SetUniformValue( 0, ref value );}
			public void SetColor( ref Vector4 value ) { m_shader_program.SetUniformValue( 1, ref value );}
//			public void SetWorldScale( ref float value ) { m_shader_program.SetUniformValue( 2, ref value );}
		}

		/// @if LANG_EN
		/// <summary>
		/// Particle object returned by CreateParticle.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>CreateParticle で返される、パーティクルオブジェクト。
		/// </summary>
		/// @endif
		public class Particle
		{
			/// @if LANG_EN
			/// <summary>
			/// Position.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>位置。
			/// </summary>
			/// @endif
			public Vector2 Position;

			/// @if LANG_EN
			/// <summary>
			/// Velocity.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>速度。
			/// </summary>
			/// @endif
			public Vector2 Velocity;

			/// @if LANG_EN
			/// <summary>
			/// Age (particle dies when Age >= LifeSpan).
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>年齢 (Age)。 ( Age >= LifeSpan でパーティクルは死亡します)。
			/// </summary>
			/// @endif
			public float Age;
			
			/// @if LANG_EN
			/// <summary>
			/// Life span
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>寿命。
			/// </summary>
			/// @endif
			public float LifeSpan;

			/// @if LANG_EN
			/// <summary>
			/// Life span reciprocal - if you don't use the automatic init, you must make sure to set this to 1.0f/LifeSpan
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>寿命 (LifeSpan) の逆数。自動の初期化を使わない場合、この値に1.0f/LifeSpanをセットすることを確認してください。
			/// </summary>
			/// @endif
			public float LifeSpanRcp;

			/// @if LANG_EN
			/// <summary>
			/// Rotation angle
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>回転角。
			/// </summary>
			/// @endif
			public float Angle;	// Vector2?

			/// @if LANG_EN
			/// <summary>
			/// Angular velocity
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>角速度。
			/// </summary>
			/// @endif
			public float AngularVelocity; // Vector2?
			
			/// @if LANG_EN
			/// <summary>
			/// Scale when Age=0.0
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>Age=0.0時のスケール。
			/// </summary>
			/// @endif
			public float ScaleStart;
			
			/// @if LANG_EN
			/// <summary>
			/// ScaleDelta must be initialized to ( scale_end - ScaleStart ) / LifeSpan
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>ScaleDelta は ( scale_end - ScaleStart ) / LifeSpan で初期化する必要があります。
			/// </summary>
			/// @endif
			public float ScaleDelta;
			
			/// @if LANG_EN
			/// <summary>
			/// Color when Age=0.0
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>Age=0.0時の色。
			/// </summary>
			/// @endif
			public Vector4 ColorStart;

			/// @if LANG_EN
			/// <summary>
			/// ColorDelta must be initialized to ( color_end - ColorStart ) / LifeSpan
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>ColorDelta は ( color_end - ColorStart ) / LifeSpan で初期化する必要があります。
			/// </summary>
			/// @endif
			public Vector4 ColorDelta;

			/// @if LANG_EN
			/// <summary>
			/// Scale
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>スケール。
			/// </summary>
			/// @endif
			public float Scale { get { return ScaleStart + ScaleDelta * Age;}}
			/// @if LANG_EN
			/// <summary>
			/// Color
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>色。
			/// </summary>
			/// @endif
			public Vector4 Color { get { return ColorStart + ColorDelta * Age;}}

			/// @if LANG_EN
			/// <summary>
			/// Check if the particle is still alive
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>パーティクルが生きているかチェックします。
			/// </summary>
			/// @endif
			public bool Dead { get { return Age >= LifeSpan;}}

			/// @if LANG_EN
			/// <summary>Return the string representation of this Particle.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>このパーティクルの文字列表現を返します。
			/// </summary>
			/// @endif
			public override string ToString() 
			{
				return "Position=" + Position + " Velocity=" + Velocity + " Age=" + Age + " LifeSpan=" + LifeSpan + " Scale=" + Scale + " Color=" + Color;
			}
		};

		// particle data for the VertexBuffer
		struct Vertex
		{
			public Vector4 XYUV;
			public Vector4 Color;

			public Vertex( Vector4 a_data1, Vector4 a_data2 )
			{
				XYUV = a_data1;
				Color = a_data2;
			}
		}

		/// @if LANG_EN
		/// <summary>ParticleSystem's shader interface.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ParticleSystem のシェーダーインターフェース。
		/// </summary>
		/// @endif
		public interface IParticleShader
		{
			/// @if LANG_EN
			/// <summary>Set the Projection*View*Model matrix.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>Projection * View * Model 行列の設定。
			/// </summary>
			/// @endif
			void SetMVP( ref Matrix4 value );
			
			/// @if LANG_EN
			/// <summary>Set a global color.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>グローバルカラーの設定。
			/// </summary>
			/// @endif
			void SetColor( ref Vector4 value );
			
			/// @if LANG_EN
			/// <summary>Return the shader's ShaderProgram object.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>シェーダーの ShaderProgram オブジェクトを返します。
			/// </summary>
			/// @endif
			ShaderProgram GetShaderProgram();
		}

		/// @if LANG_EN
		/// <summary>
		/// This class regroups all the parameters needed to initialize a particle.
		/// Most values have a variance associated to them (-Var suffix), to give a 
		/// randomization range. When the variance is a "relative" value (-RelVar suffix), 
		/// a value between  0,1 is expected. For example: 0.2f means the corresponding 
		/// value will be randomized -+20% at creation time.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このクラスは、パーティクルの初期化に必要な全てのパラメータを再グループ化します。
		/// ランダムの範囲を与えるため、多くの値は分散しています(-Var の接尾辞)。
		/// 分散が "相対"値（-RelVarの接尾辞）である場合、0,1の間の値が期待されています。
		/// 例: 0.2f は、作成時に対応する値は -+20%でランダム化されることを意味します。
		/// </summary>
		/// @endif
		public class EmitterParams
		{
			/// @if LANG_EN
			/// <summary>
			/// The generated position, velocity, angle are transformed by this.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>生成される位置・速度・角度はこの行列によって変換されます。
			/// </summary>
			/// @endif
			public Matrix3 Transform = Matrix3.Identity;
			
			/// @if LANG_EN
			/// <summary>
			/// The transform matrix we use to estimate a velocity and an angular velocity.
			/// You probably want to set it everyframe to something relevant (the current 
			/// transform of the object node for example).
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>速度と角速度の推定に使用する変換行列。
			/// </summary>
			/// @endif
			public Matrix3 TransformForVelocityEstimate = Matrix3.Identity;
			
			/// @if LANG_EN
			/// <summary>
			/// Control how much of the "observed velocity" we add to the created particles.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>作成したパーティクルに追加する速度のコントロール。
			/// </summary>
			/// @endif
			public float ForwardMomentum = 0.0f;
			
			/// @if LANG_EN
			/// <summary>
			/// Control how much of the "observed angular velocity" we add to the created particles.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>作成したパーティクルに追加する角速度のコントロール。
			/// </summary>
			/// @endif
			public float AngularMomentun = 0.0f;
			
			/// @if LANG_EN
			/// <summary>
			/// The time to wait until the next particle gets created, in seconds.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>次のパーティクルが生成されるまで待機する時間。秒単位。
			/// </summary>
			/// @endif
			public float WaitTime = 1.0f;
			
			/// @if LANG_EN
			/// <summary>
			/// WaitTime's variance (relative)
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>待機時間の分散 (相対)。
			/// </summary>
			/// @endif
			public float WaitTimeRelVar = 0.15f;

			/// @if LANG_EN
			/// <summary>
			/// Created particles's life span in seconds.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>作成されたパーティクルの寿命。秒単位。
			/// </summary>
			/// @endif
			public float LifeSpan = 5.0f;
			
			/// @if LANG_EN
			/// <summary>
			/// LifeSpan's variance (relative)
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>寿命の長さの分散(相対)。
			/// </summary>
			/// @endif
			public float LifeSpanRelVar;
			
			/// @if LANG_EN
			/// <summary>
			/// Created particles's initial position (see also InLocalSpace)
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>作成されたパーティクルの初期位置 ( InLocalSpaceも参考にしてください )。
			/// </summary>
			/// @endif
			public Vector2 Position = GameEngine2D.Base.Math._00;
			
			/// @if LANG_EN
			/// <summary>
			/// Position's variance
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>位置の分散。
			/// </summary>
			/// @endif
			public Vector2 PositionVar = GameEngine2D.Base.Math._11 * 1.5f;
			
			/// @if LANG_EN
			/// <summary>
			/// Created particles's initial velocity given to created particles (see also InLocalSpace) 
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>作成したパーティクルの初期速度 ( InLocalSpaceも参考にしてください )。
			/// </summary>
			/// @endif
			public Vector2 Velocity = GameEngine2D.Base.Math._01;
			
			/// @if LANG_EN
			/// <summary>
			/// Velocity's variance
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>速度の分散。
			/// </summary>
			/// @endif
			public Vector2 VelocityVar = GameEngine2D.Base.Math._11 * 0.2f;
			
			/// @if LANG_EN
			/// <summary>
			/// Created particles's initial angular velocity, in radians (see also InLocalSpace) 
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>作成したパーティクルの初期角速度。ラジアン単位 ( InLocalSpaceも参考にしてください )。
			/// </summary>
			/// @endif
			public float AngularVelocity;
			
			/// @if LANG_EN
			/// <summary>
			/// AngularVelocity's variance
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>AngularVelocityの分散。
			/// </summary>
			/// @endif
			public float AngularVelocityVar;
			
			/// @if LANG_EN
			/// <summary>
			/// Created particles's initial rotation angle in radians (see also InLocalSpace) 
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>作成したパーティクルの初期回転角度。ラジアン単位 ( InLocalSpaceも参考にしてください )。
			/// </summary>
			/// @endif
			public float Angle;
			
			/// @if LANG_EN
			/// <summary>
			/// Angle's variance
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>角度の分散。
			/// </summary>
			/// @endif
			public float AngleVar;
			
			/// @if LANG_EN
			/// <summary>
			/// Created particles's initial color
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>作成したパーティクルの初期色。
			/// </summary>
			/// @endif
			public Vector4 ColorStart = Colors.White;
			
			/// @if LANG_EN
			/// <summary>
			/// ColorStart's variance
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>初期色の分散。
			/// </summary>
			/// @endif
			public Vector4 ColorStartVar = GameEngine2D.Base.Math._0000;
			
			/// @if LANG_EN
			/// <summary>
			/// Color the particle will have when they reach their life span
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>寿命に達したときのパーティクルの色。
			/// </summary>
			/// @endif
			public Vector4 ColorEnd = Colors.White;
			
			/// @if LANG_EN
			/// <summary>
			/// ColorEnd's variance
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>ColorEndの分散。
			/// </summary>
			/// @endif
			public Vector4 ColorEndVar = GameEngine2D.Base.Math._0000;
			
			/// @if LANG_EN
			/// <summary>
			/// Created particles's initial size 
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>作成したパーティクルの初期サイズ。
			/// </summary>
			/// @endif
			public float ScaleStart = 1.0f;
			
			/// @if LANG_EN
			/// <summary>
			/// ScaleStart's variance (relative)
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>ScaleStartの分散。
			/// </summary>
			/// @endif
			public float ScaleStartRelVar;
			
			/// @if LANG_EN
			/// <summary>
			/// Size the particle will have when they reach their life span
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>寿命に達したときの粒子のサイズ。
			/// </summary>
			/// @endif
			public float ScaleEnd = 1.0f;
			
			/// @if LANG_EN
			/// <summary>
			/// ScaleEnd's variance (relative)
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>ScaleEndの分散。
			/// </summary>
			/// @endif
			public float ScaleEndRelVar;

			/// @if LANG_EN
			/// <summary>EmitterParams constructor.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>コンストラクタ。
			/// </summary>
			/// @endif
			public EmitterParams()
			{
			}

			/// @if LANG_EN
			/// <summary>Return the string representation of this EmitterParams.</summary>
			/// <param name="prefix">A prefix string added to the beginning of each line.</param> 
			/// @endif
			/// @if LANG_JA
			/// <summary>EmitterParams の文字列表現を返します。
			/// </summary>
			/// <param name="prefix">それぞれの行の最初に追加される接頭辞の文字列。</param> 
			/// @endif
			public string ToString( string prefix ) 
			{
				return 
				prefix + "WaitTime           " + WaitTime           + "\n" +
				prefix + "WaitTimeVar        " + WaitTimeRelVar     + "\n" +
				prefix + "Position           " + Position           + "\n" +
				prefix + "PositionVar        " + PositionVar        + "\n" +
				prefix + "Velocity           " + Velocity           + "\n" +
				prefix + "VelocityVar        " + VelocityVar        + "\n" +
				prefix + "AngularVelocity    " + AngularVelocity    + "\n" +
				prefix + "AngularVelocityVar " + AngularVelocityVar + "\n" +
				prefix + "Angle              " + Angle              + "\n" +
				prefix + "AngleVar           " + AngleVar           + "\n" +
				prefix + "ColorStart         " + ColorStart         + "\n" +
				prefix + "ColorStartVar      " + ColorStartVar      + "\n" +
				prefix + "ColorEnd           " + ColorEnd           + "\n" +
				prefix + "ColorEndVar        " + ColorEndVar        + "\n" +
				prefix + "ScaleStart         " + ScaleStart         + "\n" +
				prefix + "ScaleStartRelVar   " + ScaleStartRelVar   + "\n" +
				prefix + "ScaleEnd           " + ScaleEnd           + "\n" +
				prefix + "ScaleEndRelVar     " + ScaleEndRelVar     + "\n" +
				prefix + "LifeSpan           " + LifeSpan           + "\n" +
				prefix + "LifeSpanVar        " + LifeSpanRelVar     + "\n" ;
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// SimulationParams regroups all the parameters needed by the Update and Draw 
		/// to advance and render the particles.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>SimulationParams は、Update と Draw で必要とされる全てのパラメータを、再グループ化します。
		/// </summary>
		/// @endif
		public class SimulationParams
		{
			/// @if LANG_EN
			/// <summary>
			/// A global friction applied to all particles.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>全てのパーティクルに適用される摩擦。
			/// </summary>
			/// @endif
			public float Friction = 0.99f;
			
			/// @if LANG_EN
			/// <summary>
			/// Gravity direction (unit vector).
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>重力の方向 (単位ベクトル)。
			/// </summary>
			/// @endif
			public Vector2 GravityDirection = -GameEngine2D.Base.Math._01;
			
			/// @if LANG_EN
			/// <summary>
			/// Gravity amount.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>重力量。
			/// </summary>
			/// @endif
			public float Gravity = 0.8f;
			
			/// @if LANG_EN
			/// <summary>
			/// Wind direction (unit vector).
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>風の方向。
			/// </summary>
			/// @endif
			public Vector2 WindDirection;
			
			/// @if LANG_EN
			/// <summary>
			/// Amount of wind.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>風量。
			/// </summary>
			/// @endif
			public float Wind;
			
			/// @if LANG_EN
			/// <summary>
			/// Amount of brownian motion you want to add to each particle.
			/// For each particle, a different random direction is added to 
			/// the wind everyframe. You can use that to add a touch of gas/dust 
			/// effects.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>各パーティクルに追加するブラウン運動の量です。
			/// 各パーティクルに対し、毎フレーム、異なったランダム方向を風に追加します。
			/// ガス/塵のエフェクトを追加することができます。
			/// </summary>
			/// @endif
			public float BrownianScale = 5.0f;
			
			/// @if LANG_EN
			/// <summary>
			/// The Fade value is a value in 0,1 that controls how fast you want the 
			/// particle to fade in/fade out (when it dies)
			/// The particle's color's alpha value is multiplied by a symmetric fade
			/// curve; Fade is the length of the start and end fade areas, for example 
			/// if Fade is 0.25, the particle will be fully visible at 25% of its age.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>Fade の値は、0,1の範囲の値です。この値は、パーティクルがフェイドイン/フェイドアウトする速さ(つまり死亡する時期)をコントロールします。
			/// パーティクルの色のアルファ値は、対称フェイドカーブで乗算します。
			/// Fade はフェイドスタートとフェイドエンドの長さです。
			/// 例えば、Fade が0.25であれば、粒子はその年齢の25％で完全に見えるようになります。
			/// </summary>
			/// @endif
			public float Fade = 0.1f;
		}

		/// @if LANG_EN
		/// <summary>
		/// The texture information for this particle system. All particles use the same image.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このパーティクルシステムのテクスチャ情報。全てのパーティクルは同じイメージを使用します。
		/// </summary>
		/// @endif
		public TextureInfo TextureInfo;
		
		/// @if LANG_EN
		/// <summary>
		/// A global multiplier color for the particle system.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>パーティクルシステムのためのグローバルな乗算色。
		/// </summary>
		/// @endif
		public Vector4 Color = Colors.White;
		
		/// @if LANG_EN
		/// <summary>
		/// The blend mode used by the particle system.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>パーティクルによって使用されるブレンドモード。
		/// </summary>
		/// @endif
		public BlendMode BlendMode = BlendMode.Normal;
		
		/// @if LANG_EN
		/// <summary>
		/// Parameters used for controlling the emission and initial parameters of particles.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>パラメータは、パーティクルの放出とパラメータの初期化の管理に使用します。
		/// </summary>
		/// @endif
		public EmitterParams Emit;
		
		/// @if LANG_EN
		/// <summary>
		/// Parameters used for controlling the simulation.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>シミュレーションのコントロールに使われるパラメータ。
		/// </summary>
		/// @endif
		public SimulationParams Simulation;

		/// @if LANG_EN
		/// <summary>
		/// The modelview matrix used for rendering the particle system.
		/// RenderTransform overrides base class's Node.GetTransform() to render the local geometry.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>パーティクルシステムを描画するためのモデルビュー行列
		/// </summary>RenderTransform は ローカルの幾何情報を描画するため、基底クラスの Node.GetTransform() を上書きします。
		/// @endif
		public Matrix3 RenderTransform = Matrix3.Identity;
		
		/// @if LANG_EN
		/// <summary>
		/// The maximum number of particles the system can deal with.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>システムが取り扱うパーティクルの数。
		/// </summary>
		/// @endif
		public int MaxParticles { get { return m_particles.Length;}}
		
		/// @if LANG_EN
		/// <summary>
		/// The current number of particles alive.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>生存しているパーティクルの数。
		/// </summary>
		/// @endif
		public int ParticlesCount { get { return m_particles_count;}}

		/// @if LANG_EN
		/// <summary>
		/// Return true if the number of active particles has reached maximum capacity.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アクティブなパーティクルが最大数に達すると、trueを返します。
		/// </summary>
		/// @endif
		public bool IsFull { get { return MaxParticles == m_particles_count;}}
		/// particle system's default shader (texture * color).
		static ParticleShaderDefault m_default_shader;

		/// @if LANG_EN
		/// <summary>
		/// The particle system's default shader (texture * color).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>パーティクルシステムのデフォルトシェーダー ( texture * color )。
		/// </summary>
		/// @endif
		static public ParticleShaderDefault DefaultShader 
		{ 
			get 
			{
				if ( m_default_shader == null )
					m_default_shader = new ParticleShaderDefault();
				return m_default_shader;
			}
		}
		/// @if LANG_EN
		/// <summary>
		/// User can set an external shader. 
		/// The Label class won't dispose of shaders set by user (other than ParticleSystem.DefaultShader).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>開発者がセットすることのできる外部シェーダー。
		/// Label クラスは開発者がセットしたシェーダーを破棄しません。(ParticleSystem.DefaultShaderは除く)
		/// </summary>
		/// @endif
		public IParticleShader Shader;

		bool m_disposed = false;
		/// @if LANG_EN
		/// <summary>Return true if this object been disposed.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このオブジェクトが破棄されていれば、trueを返します。
		/// </summary>
		/// @endif
		public bool Disposed { get { return m_disposed; } }

		ImmediateModeQuads< Vertex > m_imm_quads;
		double m_elapsed;
		float m_emit_timer;
		Vector2 m_observed_velocity;
		Matrix3 m_tracking_transform_prev = Matrix3.Identity;
		float m_observed_angular_velocity;
		GameEngine2D.Base.Math.RandGenerator m_random;
		Vertex m_v0;
		Vertex m_v1;
		Vertex m_v2;
		Vertex m_v3;
		int m_particles_count;
		Particle[] m_particles;
		GraphicsContextAlpha GL;

		/// @if LANG_EN
		/// <summary>
		/// ParticleSystem constructor.
		/// </summary>
		/// <param name="max_particles">The maximum number of particles for this particle system.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// <param name="max_particles">パーティクルシステムのパーティクルの最大数。</param>
		/// @endif
		public ParticleSystem( int max_particles )
		{
			GL = Director.Instance.GL;
			m_imm_quads = new ImmediateModeQuads< Vertex >( GL, (uint)max_particles, VertexFormat.Float4, VertexFormat.Float4 );
			m_particles = new Particle[ max_particles ];
			for ( int i=0; i < m_particles.Length; ++i )
				m_particles[i] = new Particle();
			m_random = new GameEngine2D.Base.Math.RandGenerator();
			Shader = DefaultShader;
			Emit = new EmitterParams();
			Simulation = new SimulationParams();

			m_v0.XYUV.Zw = GameEngine2D.Base.Math._00;
			m_v1.XYUV.Zw = GameEngine2D.Base.Math._10;
			m_v2.XYUV.Zw = GameEngine2D.Base.Math._01;
			m_v3.XYUV.Zw = GameEngine2D.Base.Math._11;
		}

		/// @if LANG_EN
		/// <summary>
		/// Dispose implementation.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Disposeの実装。
		/// </summary>
		/// @endif
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
//				System.Console.WriteLine( "ParticleSystem Dispose() called!" );
				m_imm_quads.Dispose();
				m_disposed = true;
			}
		}

		/// @if LANG_EN
		/// <summary>Dispose of static resources.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>静的リソースの破棄。
		/// </summary>
		/// @endif
		static public void Terminate()
		{
			Common.DisposeAndNullify< ParticleShaderDefault >( ref m_default_shader );
		}

		// initialize a particle that has just been created
		void init_auto_particle( Particle p )
		{
			p.LifeSpan = FMath.Max( 0.0f, Emit.LifeSpan * ( 1.0f + Emit.LifeSpanRelVar * m_random.NextFloatMinus1_1() ) );
			p.Age = 0.0f;
			p.LifeSpanRcp = 1.0f / p.LifeSpan;

			p.Position = Emit.Position + Emit.PositionVar * m_random.NextVector2( -1.0f, 1.0f );
			p.Position = ( Emit.Transform * p.Position.Xy1 ).Xy;

			p.Velocity = Emit.Velocity + Emit.VelocityVar * m_random.NextVector2( -1.0f, 1.0f );
			p.Velocity = ( Emit.Transform * p.Velocity.Xy0 ).Xy;

			p.Angle = FMath.Max( 0.0f, Emit.Angle + Emit.AngleVar * m_random.NextFloatMinus1_1() );
			p.Angle += GameEngine2D.Base.Math.Angle( Emit.Transform.X.Xy.Normalize() );

			p.AngularVelocity = FMath.Max( 0.0f, Emit.AngularVelocity + Emit.AngularVelocityVar * m_random.NextFloatMinus1_1() );

			p.Velocity += Emit.ForwardMomentum * m_observed_velocity;
			p.AngularVelocity += Emit.AngularMomentun * m_observed_angular_velocity;

			p.ScaleStart = FMath.Max( 0.0f, Emit.ScaleStart * ( 1.0f + Emit.ScaleStartRelVar * m_random.NextFloatMinus1_1() ) );
			float scale_end = FMath.Max( 0.0f, Emit.ScaleEnd * ( 1.0f + Emit.ScaleEndRelVar * m_random.NextFloatMinus1_1() ) );
			p.ScaleDelta = ( scale_end - p.ScaleStart ) / p.LifeSpan;

			p.ColorStart = ( Emit.ColorStart + Emit.ColorStartVar * m_random.NextFloatMinus1_1() ).Clamp( 0.0f, 1.0f );
			Vector4 color_end = ( Emit.ColorEnd + Emit.ColorEndVar * m_random.NextFloatMinus1_1() ).Clamp( 0.0f, 1.0f );
			p.ColorDelta = ( color_end - p.ColorStart ) / p.LifeSpan;
		}

		void update( Particle p, float dt, Vector2 forces ) 
		{
			p.Velocity += ( forces - p.Velocity * Simulation.Friction ) * dt;
			p.Position += p.Velocity * dt;
			p.Angle += p.AngularVelocity * dt;
			p.Age += dt;
		}

		/// @if LANG_EN
		/// <summary>
		/// Particles get created automatically using Emit parameters,
		/// but if needed you can also create and init particles yourself.
		/// </summary>
		/// <param name="skip_auto_init">
		/// If false, the created particle gets initialized using the Emit parameters, just 
		/// like the particles randomly created everyframe (this is in case you want to use 
		/// a random variation as a starting point).
		/// </param>
		/// <returns>A handle to the particle just created</returns>
		/// @endif
		/// @if LANG_JA
		/// <summary>Emit パラメータを使って自動で作成されるパーティクル。
		/// 必要に応じて作成でき、開発者がパーティクルを初期化できます。
		/// </summary>
		/// <param name="skip_auto_init">
		/// falseなら、作成されたパーティクルを Emit パラメータを使って初期化します。パーティクルは毎フレーム、ランダムに作成されます。
		/// ( 開始時にランダムの分散を使用した場合)。
		/// </param>
		/// <returns>作成されたパーティクルのハンドラ。</returns>
		/// @endif
		public Particle CreateParticle( bool skip_auto_init = false )
		{
			if ( IsFull ) 
				return null;
			
			if ( !skip_auto_init  ) 
				init_auto_particle( m_particles[ m_particles_count ] );

			++m_particles_count;

			return m_particles[ m_particles_count - 1 ];
		}

		/// @if LANG_EN
		/// <summary>The update function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Update関数。
		/// </summary>
		/// @endif
		public void Update( float dt )
		{
			m_observed_velocity = ( Emit.TransformForVelocityEstimate.Z - m_tracking_transform_prev.Z ).Xy / dt;
			m_observed_angular_velocity = ( m_tracking_transform_prev.X.Xy.Normalize().Angle( Emit.TransformForVelocityEstimate.X.Xy.Normalize() ) ) / dt;
			m_tracking_transform_prev = Emit.TransformForVelocityEstimate;

//			System.Console.WriteLine( m_observed_velocity );
//			System.Console.WriteLine( Emit.TransformForVelocityEstimate );

			m_elapsed += (double)dt;

			float emit_wait_time = FMath.Max( 0.0f, Emit.WaitTime * ( 1.0f + Emit.WaitTimeRelVar * m_random.NextFloatMinus1_1() ) );

			m_emit_timer += dt;
			while ( m_emit_timer > emit_wait_time && !IsFull )
			{
				init_auto_particle( m_particles[ m_particles_count ] );
				++m_particles_count;
				m_emit_timer -= emit_wait_time;

				// generate a new wait time
				emit_wait_time = FMath.Max( 0.0f, Emit.WaitTime * ( 1.0f + Emit.WaitTimeRelVar * m_random.NextFloatMinus1_1() ) );
			}

			Vector2 global_forces = Simulation.Gravity * Simulation.GravityDirection + Simulation.Wind * Simulation.WindDirection;

			for ( int i=0; i < m_particles_count; )
			{
				if ( m_particles[i].Dead )
				{
					// this will shuffle the draw order but reduces the number of copies
					Common.Swap( ref m_particles[i], ref m_particles[m_particles_count-1] );
					--m_particles_count;
				}
				else
				{
					Vector2 forces = global_forces;
					if ( Simulation.BrownianScale != 0.0f )
						forces += m_random.NextVector2Minus1_1() * Simulation.BrownianScale;

					update( m_particles[i], dt, forces );
					++i;
				}
			}

//			Dump();
		}

		// x is the particle age normalized to 0,1
		// dx is the len of the linear fade in/out zone at the beginning and the end (the curve is symmetric)
		// particle's Color alpha gets multiplied by age_to_alpha
		float age_to_alpha( float x, float dx )
		{
			x = FMath.Abs( x - 0.5f );
			float mi = 0.5f - dx;
			return 1.0f - FMath.Max( 0.0f, ( x - mi ) * ( 1.0f / dx ) );
		}

		/// @if LANG_EN
		/// <summary>The draw function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画関数。
		/// </summary>
		/// @endif
		public void Draw()
		{
			////Common.Profiler.Push("ParticleSystem.BeginParticles");
		
			GL.SetDepthMask( false );

			GL.ModelMatrix.Push();
			GL.ModelMatrix.Set( RenderTransform.Matrix4() );

			Matrix4 mvp = GL.GetMVP();

			Shader.SetMVP( ref mvp );
			Shader.SetColor( ref Color );

			GL.SetBlendMode( BlendMode );

			Common.Assert( TextureInfo != null, "TextureInfo has not been set." );

			GL.Context.SetShaderProgram( Shader.GetShaderProgram() );
			GL.Context.SetTexture( 0, TextureInfo.Texture );

			m_imm_quads.ImmBeginQuads( (uint)m_particles_count );

			for ( int i=0; i<m_particles_count; ++i )
			{
				Vector2 x = Vector2.Rotation( m_particles[i].Angle ) * m_particles[i].Scale;
				Vector2 y = GameEngine2D.Base.Math.Perp( x );
				Vector2 p = m_particles[i].Position - ( x + y ) * 0.5f;
				Vector4 color = m_particles[i].Color;

				color.W *= age_to_alpha( m_particles[i].Age * m_particles[i].LifeSpanRcp, Simulation.Fade );

				m_v0.XYUV.Xy = p         ; m_v0.Color = color;
				m_v1.XYUV.Xy = p + x     ; m_v1.Color = color;
				m_v2.XYUV.Xy = p + y     ; m_v2.Color = color;
				m_v3.XYUV.Xy = p + x + y ; m_v3.Color = color;

				m_imm_quads.ImmAddQuad( m_v0, m_v1, m_v2, m_v3 );
			}

			m_imm_quads.ImmEndQuads();

			////Common.Profiler.Pop();

			GL.SetDepthMask( true );

			GL.ModelMatrix.Pop();
		}

		void Dump()
		{
			string prefix = Common.FrameCount + " ";
			System.Console.WriteLine( prefix + "ParticlesCount " + ParticlesCount + "/" + MaxParticles );
			System.Console.WriteLine( prefix + "Emit" + Emit.ToString( prefix ) );
			for ( int i=0; i < m_particles_count; ++i )
				System.Console.WriteLine( prefix + "[" +i +"]"+ " " +m_particles[i] );
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// Particles wraps a single ParticleSystem into a scenegraph node.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>ひとつの ParticleSystem を シーングラフノードにラップしたパーティクルクラス。
	/// </summary>
	/// @endif
	public class Particles : Node, System.IDisposable 
	{
		/// @if LANG_EN
		/// <summary>
		/// The actual ParticleSystem object used by this Particles node.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>この Particles によって使用される、実質的な ParticleSystem オブジェクト。
		/// </summary>
		/// @endif
		public ParticleSystem ParticleSystem;

		/// @if LANG_EN
		/// <summary>
		/// ParticleSystem constructor.
		/// Note that the constructor calls ScheduleUpdate().
		/// </summary>
		/// <param name="max_particles">The maximum number of particles for this particle system.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。コンストラクタが ScheduleUpdate() を呼び出すことに注意してください。
		/// </summary>
		/// <param name="max_particles">このパーティクルシステムの最大パーティクル数。</param>
		/// @endif
		public Particles( int max_particles )
		{
			ParticleSystem = new ParticleSystem( max_particles );
			ScheduleUpdate();
		}

		/// @if LANG_EN
		/// <summary>The update function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Update関数。
		/// </summary>
		/// @endif
		public override void Update( float dt )
		{
			base.Update( dt );
			ParticleSystem.Update( dt );
		}

		/// @if LANG_EN
		/// <summary>The draw function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画関数。
		/// </summary>
		/// @endif
		public override void Draw()
		{
			base.Draw();
			ParticleSystem.Draw();
		}

		/// @if LANG_EN
		/// <summary>
		/// Dispose implementation.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Disposeの実装。
		/// </summary>
		/// @endif
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				Common.DisposeAndNullify< ParticleSystem >( ref ParticleSystem );
			}
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D.Base

