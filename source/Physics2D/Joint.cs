/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;

//
//  Solver(Joint) Phase of Simulation
//
namespace Sce.PlayStation.HighLevel.Physics2D
{	
	/// @if LANG_EN
    /// <summary> Joint </summary>
    /// @endif
	/// @if LANG_JA
    /// <summary> ジョイント </summary>
    /// @endif
	public partial class PhysicsJoint
    {
		#region Fields
		/// @if LANG_EN
		/// <summary> totalIndex of joint </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ジョイントの合成インデックス</summary>
		/// @endif
        public uint totalIndex;
			
		/// @if LANG_EN
		/// <summary>relative position to anchor point from bodyA </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 接続点のbodyAから見たときの相対位置 </summary>
		/// @endif
		public Vector2 localAnchor1 = new Vector2(0, 0);
		
		/// @if LANG_EN
		/// <summary>relative position to anchor point from bodyB </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 接続点のbodyBから見たときの相対位置 </summary>
		/// @endif
		public Vector2 localAnchor2 = new Vector2(0, 0);

		/// @if LANG_EN
		/// <summary>one axis of the joint</summary>
		/// <remarks>
		/// axis1 direction and axis2 direction should be orthonormal to each other <br/>
		/// axis1Lim = new Vector2(1, 0) means that it lock the x-direction movement<br/>
		/// when it is normalized, it means for joint to lock the direction completely
		/// </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>ジョイントの制御軸１</summary>
		/// <remarks>
		/// ジョイントの制御軸１は制御軸２と直行している必要がある<br/>
		/// axis1Lim = new Vector2(1, 0) はＸ方向の移動をロックする<br/>
		/// 正規化されたベクトルであるとき、方向移動を完全にロックする
		/// </remarks>
		/// @endif
		public Vector2 axis1Lim = new Vector2(1, 0);
		
		/// @if LANG_EN
		/// <summary>the other axis of the joint</summary>
		/// <remarks>
		/// axis2 direction and axis2 direction should be orthonormal to each other <br/>
		/// axis2Lim = new Vector2(0, 1) means that it lock the y-direction movement<br/>
		/// when it is normalized, it means for joint to the direction completely
		/// </remarks> 
		/// @endif
		/// @if LANG_JA
		/// <summary>ジョイントの制御軸２</summary>
		/// <remarks>
		/// ジョイントの制御軸２は制御軸１と直行している必要がある<br/>
		/// axis1Lim = new Vector2(1, 0) はＹ方向の移動をロックする<br/>
		/// 正規化されたベクトルであるとき、方向移動を完全にロックする
		/// </remarks> 
		/// @endif
        public Vector2 axis2Lim = new Vector2(0, 1);
		
		/// @if LANG_EN
		/// <summary>rotation of the joint</summary>
		/// <remarks>
		/// angleLim [0.0f - 1.0f]<br/>
		/// lock of angle angleLim=1.0f<br/>
		/// free of angle angleLim=0.0f<br/>
		/// When you set the value which does not belong to [0.0f - 1.0f], it will be truncated
		/// </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>ジョイントの回転制御</summary>
		/// <remarks>
		/// angleLim [0.0f - 1.0f]<br/>
		/// 回転のロック angleLim=1.0f<br/>
		/// 回転のフリー angleLim=0.0f<br/>
		/// [0.0f - 1.0f]以外の値をセットした場合は自動的に端の値にセットされます
		/// </remarks>
		/// @endif
		public float angleLim = 1.0f;
		
		/// <summary>the other axis of the joint</summary>			
        private float localRotation = 0.0f;
		
		/// <summary> initial angle for bodyA of the joint </summary>	
		private float angle1 = 0.0f;

		/// <summary> initial angle for bodyA of the joint </summary>	
		private float angle2 = 0.0f;
	
		/// @if LANG_EN
		/// <summary> friction of the joint</summary>
		/// <remarks>
		/// friction is necessary to stop joint rotation finally,
		/// ball joint will continue to rotate if this value is not set properly 
		///</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>ジョイントの摩擦</summary>
		/// <remarks>
		/// ジョイントの回転を最終的にストップするには摩擦が必要であり,
		/// 摩擦がないのであれば、ボールジョイントはずっと回転し続けてしまう
		/// </remarks>
		/// @endif
        public float friction = 0.001f;

        // These are hidden from user
        private const float JointBiasLinear = (0.5f/8.0f);
        private const float JointBiasAngle = (0.5f/8.0f);
	
		/// @if LANG_EN
		/// <summary> distance lower limitation for linear motion </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 併進移動の下限値 </summary>
		/// @endif
        public float axis1Lower = 0.0f;
				
		/// @if LANG_EN
		/// <summary> distance upper limitation for linear motion </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 併進移動の上限値 </summary>
		/// @endif
		public float axis1Upper = 0.0f;
		
		/// @if LANG_EN
		/// <summary> distance lower limitation for linear motion </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 併進移動の下限値 </summary>
		/// @endif
		public float axis2Lower = 0.0f;
		
		/// @if LANG_EN
		/// <summary> distance upper limitation for linear motion </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 併進移動の上限値 </summary>
		/// @endif
		public float axis2Upper = 0.0f;
      
		/// @if LANG_EN
		/// <summary> angular lower limitation for angular motion </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 回転の下限値 </summary>
		/// @endif
		public float angleLower = 0.0f;
		
		/// @if LANG_EN
		/// <summary> angular upper limitation for angular motion </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 回転の上限値 </summary>
		/// @endif
		public float angleUpper = 0.0f;
		
		/// @if LANG_EN
		/// <summary> external impulse for angular motion </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 回転に与えられる外からのインパルス </summary>
		/// @endif
		public float impulseRotMotor = 0.0f;
		
		/// @if LANG_EN
		/// <summary> external impulse for linear motion </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 並進に与えられる外からのインパルス </summary>
		/// @endif
		public Vector2 impulseLinearMotor = new Vector2(0, 0);
		
		
		// Internal Implementation Variable
		private Mat2 mat;
        private Vector2 biasMove;
        private float biasAngle;
        private float mAngle;
        private Vector2 impulse;
		private float impulseq;
		
		private float axis1_flag, axis2_flag;
		private Vector2 dp_save;
		private float dangle_save;
		
        #endregion

		#region Methods
				
		/// @if LANG_EN
		/// <summary> PhysicsJoint Constructor </summary>
        /// <param name="body1"> one of rigid body for joint </param>
        /// <param name="body2"> the other of rigid body for joint </param>
        /// <param name="anchor"> anchor point of joint </param> 
        /// <param name="index1"> index of body1 </param>
        /// <param name="index2"> index of body2 </param> 
 		/// @endif
		/// @if LANG_JA
		/// <summary> ジョイントコンストラクタ </summary>
        /// <param name="body1"> ジョイントに接続された一方の剛体</param>
        /// <param name="body2"> ジョイントに接続されたもう一方の剛体 </param>
        /// <param name="anchor"> ジョイントの接続点</param> 
        /// <param name="index1"> 剛体１のインデックス </param>
        /// <param name="index2"> 剛体２のインデックス </param> 
 		/// @endif
        public PhysicsJoint(PhysicsBody body1, PhysicsBody body2, Vector2 anchor, uint index1, uint index2)
        {
	        totalIndex = PhysicsUtility.PackIdx(index1, index2);
			
			// To remove instability of the calculation
			// should cancel 2*Pi difference			
	        angle1 = PhysicsUtility.GetRadianMod(body1.rotation);
	        angle2 = PhysicsUtility.GetRadianMod(body2.rotation);
					
	        Vector2 r1 = anchor - body1.position;
	        Vector2 r2 = anchor - body2.position;

	        localAnchor1 = new Vector2
                ((float)System.Math.Cos(angle1) * r1.X + (float)System.Math.Sin(angle1) * r1.Y,
                -(float)System.Math.Sin(angle1) * r1.X + (float)System.Math.Cos(angle1) * r1.Y);

	        localAnchor2 = new Vector2
                ((float)System.Math.Cos(angle2) * r2.X + (float)System.Math.Sin(angle2) * r2.Y,
                -(float)System.Math.Sin(angle2) * r2.X + (float)System.Math.Cos(angle2) * r2.Y);
			
	        localRotation = body2.rotation - body1.rotation;
        }
		
		/// @if LANG_EN
		/// <summary> CheckJointParameter </summary> 
		/// <param name="jnt"> joint class </param>
		/// @endif
		/// @if LANG_JA
		/// <summary> CheckJointParameter </summary>
        /// <param name="jnt"> ジョイント </param>
		/// @endif
		internal static void CheckJointParameter(PhysicsJoint jnt)
        {
			if(jnt.angleLim < 0.0f) jnt.angleLim = 0.0f;
			else if(jnt.angleLim > 1.0f) jnt.angleLim = 1.0f;
	
		}
		
		/// @if LANG_EN
		/// <summary> JointSolver </summary>
        /// <param name="jnt"> joint class </param>
        /// <param name="body1"> the other of rigid body for joint </param>
        /// <param name="body2"> anchor point of joint </param> 
        /// <param name="inv_dt"> inverse of simulation time step </param>
        /// <param name="first_flag"> first iteration of each frame? </param>
        /// <param name="iteration_num"> total iteration num for joint solver for each frame </param> 
		/// @endif
		/// @if LANG_JA
		/// <summary> JointSolver </summary>
        /// <param name="jnt"> ジョイント </para></para></param>
        /// <param name="body1"> ジョイントの一方の剛体 </param>
        /// <param name="body2"> ジョイントのもう一方の剛体 </param> 
        /// <param name="inv_dt"> シミュレーションステップの逆数 </param>
        /// <param name="first_flag">　各フレーム最初の繰り返しか </param>
        /// <param name="iteration_num"> ジョイント解決ソルバーの合計の繰り返し回数</param> 
		/// @endif
        internal static void JointSolver(PhysicsJoint jnt, PhysicsBody body1, PhysicsBody body2, float inv_dt, bool first_flag, int iteration_num)
        {
			float ang1 = body1.rotation;
			float ang2 = body2.rotation;
			
			float cs1 = (float)System.Math.Cos(ang1);
			float ss1 = (float)System.Math.Sin(ang1);
			
			Vector2 r1 = new Vector2(
				cs1*jnt.localAnchor1.X - ss1*jnt.localAnchor1.Y,
				ss1*jnt.localAnchor1.X + cs1*jnt.localAnchor1.Y );
			
			float cs2 = (float)System.Math.Cos(ang2);
			float ss2 = (float)System.Math.Sin(ang2);
			
			Vector2 r2 = new Vector2(
				cs2*jnt.localAnchor2.X - ss2*jnt.localAnchor2.Y,
				ss2*jnt.localAnchor2.X + cs2*jnt.localAnchor2.Y );
					
			float delta_angle1 = ang1 - jnt.angle1;
			float delta_angle2 = ang2 - jnt.angle2;
			float average_delta_angle;

			if(body1.invMass == 0.0f)
				average_delta_angle = delta_angle1;
			else if(body2.invMass == 0.0f)
				average_delta_angle = delta_angle2;
			else
				average_delta_angle = (delta_angle1 * body1.mass + delta_angle2 * body2.mass) / (body1.mass + body2.mass);

			Vector2 axis1 = jnt.axis1Lim;
			Vector2 axis2 = jnt.axis2Lim;
			
			float cs = (float)System.Math.Cos(average_delta_angle);
			float ss = (float)System.Math.Sin(average_delta_angle);
			
			axis1 = new Vector2(
						cs*axis1.X - ss*axis1.Y,
						ss*axis1.X + cs*axis1.Y );
	
			axis2 = new Vector2(
						cs*axis2.X - ss*axis2.Y,
						ss*axis2.X + cs*axis2.Y );
			
            Vector2 normalized_axis1Lim;
            Vector2 normalized_axis2Lim;
			
			float len1 = axis1.Length();
			if(len1 > 0.00001f)
				normalized_axis1Lim = axis1/len1;
			else
				normalized_axis1Lim = new Vector2(0, 0);
		
			float len2 = axis2.Length();
			if(len2 > 0.00001f)
				normalized_axis2Lim = axis2/len2;
			else
				normalized_axis2Lim = new Vector2(0, 0);
		
		
			if(first_flag == true)
			{
				CheckJointParameter(jnt);
				
				Mat2 kmat1;
				kmat1.col1.X = body1.invMass + body2.invMass;
				kmat1.col1.Y = 0;
				kmat1.col2.X = 0;
				kmat1.col2.Y = body1.invMass + body2.invMass;
		
				Mat2 kmat2;
				kmat2.col1.X =  body1.invInertia * r1.Y * r1.Y;		
				kmat2.col2.X = -body1.invInertia * r1.X * r1.Y;		
		
				kmat2.col1.Y = -body1.invInertia * r1.X * r1.Y;
				kmat2.col2.Y =  body1.invInertia * r1.X * r1.X;
		
				Mat2 kmat3;
				kmat3.col1.X =  body2.invInertia * r2.Y * r2.Y;
				kmat3.col2.X = -body2.invInertia * r2.X * r2.Y;		
		
				kmat3.col1.Y = -body2.invInertia * r2.X * r2.Y;
				kmat3.col2.Y =  body2.invInertia * r2.X * r2.X;
		
				Mat2 kmat_total;
				kmat_total.col1 = kmat1.col1 + kmat2.col1 + kmat3.col1;
				kmat_total.col2 = kmat1.col2 + kmat2.col2 + kmat3.col2;
		
				jnt.mat = Mat2.MakeInvert(kmat_total);
		
				Vector2 p1 = body1.position + r1;
				Vector2 p2 = body2.position + r2;
				Vector2 dp = p2 - p1;
		
				jnt.dp_save = dp;

                jnt.axis1_flag = jnt.axis2_flag = 1.0f;
		
				if(Vector2.Dot(jnt.dp_save, normalized_axis1Lim) <= jnt.axis1Lower)
				{
					jnt.axis1_flag = 1.0f;
					dp = dp - jnt.axis1Lower * normalized_axis1Lim;
				}
				else if(Vector2.Dot(jnt.dp_save, normalized_axis1Lim) >= jnt.axis1Upper)
				{
					jnt.axis1_flag = 1.0f;
					dp = dp - jnt.axis1Upper * normalized_axis1Lim;
				}
				else
				{
					jnt.axis1_flag = 0;
				}
		
				if(Vector2.Dot(jnt.dp_save, normalized_axis2Lim) <= jnt.axis2Lower)
				{
					jnt.axis2_flag = 1.0f;
					dp = dp - jnt.axis2Lower * normalized_axis2Lim;
				}
				else if(Vector2.Dot(jnt.dp_save, normalized_axis2Lim) >= jnt.axis2Upper)
				{
					jnt.axis2_flag = 1.0f;
					dp = dp - jnt.axis2Upper * normalized_axis2Lim;
				}
				else
				{
					jnt.axis2_flag = 0;
				}
		
				float dp_angle;
				
				jnt.biasMove = -(JointBiasLinear/(float)iteration_num*8) * inv_dt * dp;
				jnt.mAngle = 1.0f/(body1.invInertia + body2.invInertia);
				
				float current_lotation = body2.rotation - body1.rotation;
					
				dp_angle = current_lotation - jnt.localRotation;
				
				// To remove instability of the calculation
				// should cancel 2*Pi difference
				dp_angle = PhysicsUtility.GetRadianMod(dp_angle);
				
				jnt.dangle_save = dp_angle;
				
				if(jnt.angleLower != jnt.angleUpper)
				{
					if(dp_angle < jnt.angleLower)
					{
						dp_angle = dp_angle - jnt.angleLower;
					}
					else if(dp_angle > jnt.angleUpper)
					{
						dp_angle = dp_angle - jnt.angleUpper;
					}
					else
					{
					}
				}
		
				jnt.biasAngle = -(JointBiasAngle/(float)iteration_num*8) * inv_dt * (dp_angle);
			}

			
			Vector2 dv = body2.velocity + Vector2Util.Cross(body2.angularVelocity, r2)
                - body1.velocity - Vector2Util.Cross(body1.angularVelocity, r1);
		
			Vector2 impulse = jnt.mat * (-dv + jnt.biasMove);
		
			{					
				float check_axis1 = Vector2.Dot(jnt.dp_save, normalized_axis1Lim);
					
				if(jnt.axis1Upper == 0 && jnt.axis1Lower == 0)
				{
					jnt.axis1_flag = 1;
				}
				else if(check_axis1 <= jnt.axis1Lower)
				{
					if((check_axis1 > jnt.axis1Lower - 1.0f) && (Vector2.Dot(dv, normalized_axis1Lim) >= 0.0))
					{
						jnt.axis1_flag = 0;
					}
				}
				else if(check_axis1 >= jnt.axis1Upper)
				{
					if((check_axis1 < jnt.axis1Upper + 1.0f) && (Vector2.Dot(dv, normalized_axis1Lim) <= 0.0))
					{
						jnt.axis1_flag = 0;
					}
				}
				else
				{
					// do nothing
				}
			}
		
			{
				float check_axis2 = Vector2.Dot(jnt.dp_save, normalized_axis2Lim);
				
				if((jnt.axis2Upper == 0) && (jnt.axis2Lower == 0))
				{
					jnt.axis2_flag = 1;
				}
				else if(check_axis2 <= jnt.axis2Lower)
				{
					if((check_axis2 > jnt.axis2Lower - 1.0f) && (Vector2.Dot(dv, normalized_axis2Lim) >= 0))
					{
						jnt.axis2_flag = 0;
					}
				}
				else if(check_axis2 >= jnt.axis2Upper)
				{
					if((check_axis2 < jnt.axis2Upper + 1.0f) && (Vector2.Dot(dv, normalized_axis2Lim) <= 0))
					{
						jnt.axis2_flag = 0;
					}
				}
				else
				{
					// do nothing
				}
			}
		
		
			impulse = jnt.axis1_flag*Vector2.Dot(impulse, normalized_axis1Lim)*axis1 + jnt.axis2_flag*Vector2.Dot(impulse, normalized_axis2Lim)*axis2;

			if(first_flag == true)
				impulse += jnt.impulseLinearMotor;
			
			jnt.impulse = impulse;

			body1.velocity -= body1.invMass * jnt.impulse;
			body1.angularVelocity -= body1.invInertia * Vector2Util.Cross(r1, jnt.impulse);
			
			body2.velocity += body2.invMass * jnt.impulse;
            body2.angularVelocity += body2.invInertia * Vector2Util.Cross(r2, jnt.impulse);
		
			float dvq = body2.angularVelocity - body1.angularVelocity;
			float impulseq = jnt.mAngle * (-dvq * System.Math.Max(jnt.friction, jnt.angleLim) + jnt.biasAngle * jnt.angleLim);
			
			if((jnt.angleUpper == 0) && (jnt.angleLower == 0))
			{
				// do nothing
			}
			else if(jnt.dangle_save < jnt.angleLower)
			{
				if(dvq > 0)
				{
					impulseq = jnt.mAngle * (-dvq * System.Math.Max(jnt.friction, 0.0f) + jnt.biasAngle * jnt.angleLim);
				}
				else
				{
					// do nothing
				}
			}
			else if(jnt.dangle_save > jnt.angleUpper)
			{
				if(dvq < 0)
				{
					impulseq = jnt.mAngle * (-dvq * System.Math.Max(jnt.friction, 0.0f) + jnt.biasAngle * jnt.angleLim);
				}
				else
				{
					// do nothing
				}
			}
			else
			{
				impulseq = jnt.mAngle * (-dvq * System.Math.Max(jnt.friction, 0.0f));
			}
	
			
			if(first_flag == true)
				impulseq += jnt.impulseRotMotor;
			
			body1.angularVelocity -= body1.invInertia * impulseq;
			body2.angularVelocity += body2.invInertia * impulseq;
			
			jnt.impulseq = impulseq;
        }
		
		/// @if LANG_EN
		/// <summary> Copy all members of joint </summary>
        /// <param name="jnt"> joint </param>
 		/// @endif
		/// @if LANG_JA
		/// <summary> ジョイントの値をコピーする </summary>
        /// <param name="jnt"> ジョイント </param> 
 		/// @endif
		public void CopyJoint(PhysicsJoint jnt)
		{
			totalIndex = jnt.totalIndex;
			localAnchor1 = jnt.localAnchor1;
			localAnchor2 = jnt.localAnchor2;
			axis1Lim = jnt.axis1Lim;
			axis2Lim = jnt.axis2Lim;
			angleLim = jnt.angleLim;
			localRotation = jnt.localRotation;
			angle1 = jnt.angle1;
			angle2 = jnt.angle2;
			friction = jnt.friction;
			axis1Lower = jnt.axis1Lower;
			axis1Upper = jnt.axis1Upper;
			axis2Lower = jnt.axis2Lower;
			axis2Upper = jnt.axis2Upper;
			angleLower = jnt.angleLower;
			angleUpper = jnt.angleUpper;
			impulseRotMotor = jnt.impulseRotMotor;
			impulseLinearMotor = jnt.impulseLinearMotor;
			mat = jnt.mat;
			biasMove = jnt.biasMove;
			biasAngle = jnt.biasAngle;
			mAngle = jnt.mAngle;
			impulse = jnt.impulse;
			impulseq = jnt.impulseq;
			axis1_flag = jnt.axis1_flag;
			axis2_flag = jnt.axis2_flag;
			dp_save = jnt.dp_save;
			dangle_save = jnt.dangle_save;
		}
		
		public uint GetIndex1()
		{
			return PhysicsUtility.UnpackIdx1(totalIndex);
		}
		
		public uint GetIndex2()
		{
			return PhysicsUtility.UnpackIdx2(totalIndex);
		}
		//
		// Set & Get methods
		//
		
		/// @if LANG_EN
		/// <summary> setter and getter of totalIndex </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of totalIndex </summary>	
		/// @endif
		public uint TotalIndex
		{
			set { this.totalIndex = value; }
			get { return this.totalIndex; }
		}
			
		/// @if LANG_EN
		/// <summary> setter and getter of localAnchor1 </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of localAnchor1 </summary>	
		/// @endif
		public Vector2 LocalAnchor1
		{
			set { this.localAnchor1 = value; }
			get { return this.localAnchor1; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of LocalAnchor2 </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of LocalAnchor2 </summary>	
		/// @endif
		public Vector2 LocalAnchor2
		{
			set { this.localAnchor2 = value; }
			get { return this.localAnchor2; }
		}
		
		
		/// @if LANG_EN
		/// <summary> setter and getter of axis1Lim </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of axis1Lim </summary>	
		/// @endif
		public Vector2 Axis1Lim
		{
			set { this.axis1Lim = value; }
			get { return this.axis1Lim; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of axis2Lim </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of axis2Lim </summary>
		/// @endif
		public Vector2 Axis2Lim
		{
			set { this.axis2Lim = value; }
			get { return this.axis2Lim; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of angleLim </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of angleLim </summary>	
		/// @endif
		public float AngleLim
		{
			set { this.angleLim = value; }
			get { return this.angleLim; }
		}
				
		/// @if LANG_EN
		/// <summary> setter and getter of friction </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of friction </summary>	
		/// @endif
		public float Friction
		{
			set { this.friction = value; }
			get { return this.friction; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of axis1Lower </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of axis1Lower </summary>
		/// @endif
		public float Axis1Lower
		{
			set { this.axis1Lower = value; }
			get { return this.axis1Lower; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of axis1Upper </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of axis1Upper </summary>
		/// @endif
		public float Axis1Upper
		{
			set { this.axis1Upper = value; }
			get { return this.axis1Upper; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of axis2Lower </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of axis2Lower </summary>
		/// @endif
		public float Axis2Lower
		{
			set { this.axis2Lower = value; }
			get { return this.axis2Lower; }
		}					
				
		/// @if LANG_EN
		/// <summary> setter and getter of axis2Upper </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of axis2Upper </summary>
		/// @endif
		public float Axis2Upper
		{
			set { this.axis2Upper = value; }
			get { return this.axis2Upper; }
		}
		/// @if LANG_EN
		/// <summary> setter and getter of angleLower </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of angleLower </summary>
		/// @endif
		public float AngleLower
		{
			set { this.angleLower = value; }
			get { return this.angleLower; }
		}
		/// @if LANG_EN
		/// <summary> setter and getter of angleUpper </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of angleUpper </summary>
		/// @endif	
		public float AngleUpper
		{
			set { this.angleUpper = value; }
			get { return this.angleUpper; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of linear impulse of joint </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of linear impulse of joint </summary>
		/// @endif	
		public Vector2 ImpulseLinearMotor
		{
			set { this.impulseLinearMotor = value; }
			get { return this.impulseLinearMotor; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of rotational impulse of joint </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of rotational impulse of joint </summary>
		/// @endif	
		public float ImpulseRotMotor
		{	
			set { this.impulseRotMotor = value; }
			get { return this.impulseRotMotor; }
		}
		
		/// @if LANG_EN
		/// <summary> getter of linear impulse of joint </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> ジョイントの撃力並進成分 </summary>
		/// @endif	
		public Vector2 ImpulseLinear
		{
			get { return this.impulse; }
		}
		
		/// @if LANG_EN
		/// <summary> getter of rotational impulse of joint </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> ジョイントの撃力回転成分 </summary>
		/// @endif	
		public float ImpulseRot
		{	
			get { return this.impulseq; }
		}
		#endregion
		

    }
}
