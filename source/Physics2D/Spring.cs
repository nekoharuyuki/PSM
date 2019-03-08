/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;

//
//  Solver(Spring) Phase of Simulation
//
namespace Sce.PlayStation.HighLevel.Physics2D
{	
	/// @if LANG_EN
    /// <summary> Spring </summary>
    /// @endif
	/// @if LANG_JA
    /// <summary> スプリング </summary>
    /// @endif
	public partial class PhysicsSpring
    {		
		#region Fields
		/// @if LANG_EN
		/// <summary> totalIndex of spring </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプリングの合成インデックス</summary>
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
		/// <summary> original distance between two anchor points </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 元の接続点間の距離 </summary>
		/// @endif
		public float distance = 0.0f;
		
		/// @if LANG_EN
		/// <summary> current distance between two anchor points </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 現在の接続点間の距離 </summary>
		/// @endif
		public float currentDistance = 0.0f;
		
		
		/// @if LANG_EN
		/// <summary> spring parameter </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> スプリング係数 </summary>
		/// @endif		
		public float elastic = 10.0f;
	
		/// @if LANG_EN
		/// <summary> velocity dumping parameter </summary>
		/// <remarks> To remove unstability of frequent oscillation </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> 速度減衰係数 </summary>
		/// <remarks> 振動による計算不安定性を取り除くため </remarks>
		/// @endif
        public float dumping = 0.4f;
		
        #endregion

		#region Methods
				
		/// @if LANG_EN
		/// <summary> PhysicsSpring Constructor </summary>
        /// <param name="body1"> one of rigid body for spring </param>
        /// <param name="body2"> the other of rigid body for spring </param>
        /// <param name="anchor1"> anchor point1 of spring </param> 
        /// <param name="anchor2"> anchor point2 of spring </param> 
        /// <param name="index1"> index of body1 </param>
        /// <param name="index2"> index of body2 </param> 
 		/// @endif
		/// @if LANG_JA
		/// <summary> スプリングコンストラクタ </summary>
        /// <param name="body1"> スプリングに接続された一方の剛体</param>
        /// <param name="body2"> スプリングに接続されたもう一方の剛体 </param>
        /// <param name="anchor1"> スプリングの接続点１</param>
        /// <param name="anchor2"> スプリングの接続点２</param>  
        /// <param name="index1"> 剛体１のインデックス </param>
        /// <param name="index2"> 剛体２のインデックス </param> 
 		/// @endif
        public PhysicsSpring(PhysicsBody body1, PhysicsBody body2, Vector2 anchor1, Vector2 anchor2, uint index1, uint index2)
        {
	        totalIndex = PhysicsUtility.PackIdx(index1, index2);
			
			// To remove instability of the calculation
			// should cancel 2*Pi difference			
	        float angle1 = PhysicsUtility.GetRadianMod(body1.rotation);
	        float angle2 = PhysicsUtility.GetRadianMod(body2.rotation);
					
	        Vector2 r1 = anchor1 - body1.position;
	        Vector2 r2 = anchor2 - body2.position;

	        localAnchor1 = new Vector2
                ((float)System.Math.Cos(angle1) * r1.X + (float)System.Math.Sin(angle1) * r1.Y,
                -(float)System.Math.Sin(angle1) * r1.X + (float)System.Math.Cos(angle1) * r1.Y);

	        localAnchor2 = new Vector2
                ((float)System.Math.Cos(angle2) * r2.X + (float)System.Math.Sin(angle2) * r2.Y,
                -(float)System.Math.Sin(angle2) * r2.X + (float)System.Math.Cos(angle2) * r2.Y);
			
			distance = (anchor2 - anchor1).Length();
			
			currentDistance = distance;
        }
		

        internal static void SpringSolver(PhysicsSpring jnt, PhysicsBody body1, PhysicsBody body2, float dt)
        {
			float ang1 = body1.rotation;
			float ang2 = body2.rotation;

			Vector2 r1 = new Vector2(
				(float)System.Math.Cos(ang1)*jnt.localAnchor1.X - (float)System.Math.Sin(ang1)*jnt.localAnchor1.Y,
				(float)System.Math.Sin(ang1)*jnt.localAnchor1.X + (float)System.Math.Cos(ang1)*jnt.localAnchor1.Y );

			Vector2 r2 = new Vector2(
				(float)System.Math.Cos(ang2)*jnt.localAnchor2.X - (float)System.Math.Sin(ang2)*jnt.localAnchor2.Y,
				(float)System.Math.Sin(ang2)*jnt.localAnchor2.X + (float)System.Math.Cos(ang2)*jnt.localAnchor2.Y );
			
			Vector2 p1 = body1.position + r1;
			Vector2 p2 = body2.position + r2;
			
			Vector2 dp = p2 - p1;
			
			Vector2 dv = body2.velocity + Vector2Util.Cross(body2.angularVelocity, r2)
                - body1.velocity - Vector2Util.Cross(body1.angularVelocity, r1);
			
	
			Vector2 normalVec;
			
			if((p2-p1).Length() < 0.001f)
				normalVec = new Vector2(1.0f, 0.0f);
			else
				normalVec = (p2 - p1).Normalize();
			
			jnt.currentDistance = (p2 - p1).Length();
				
			Vector2 impulse;

			impulse = -jnt.dumping * dv + -jnt.elastic * (jnt.currentDistance - jnt.distance) * normalVec;

			body1.velocity -= body1.invMass * impulse * dt;
			body1.angularVelocity -= body1.invInertia * Vector2Util.Cross(r1, impulse) * dt;
			
			body2.velocity += body2.invMass * impulse * dt;
            body2.angularVelocity += body2.invInertia * Vector2Util.Cross(r2, impulse) * dt;

		}
		
		/// @if LANG_EN
		/// <summary> Copy all members of spring </summary>
        /// <param name="jnt"> spring </param>
 		/// @endif
		/// @if LANG_JA
		/// <summary> スプリングの値をコピー </summary>
        /// <param name="jnt"> スプリング</param>
 		/// @endif
		public void CopySpring(PhysicsSpring jnt)
		{
			totalIndex = jnt.totalIndex;
			localAnchor1 = jnt.localAnchor1;
			localAnchor2 = jnt.localAnchor2;
			distance = jnt.distance;
			elastic = jnt.elastic;
			dumping = jnt.dumping;
		}
		
		// @if LANG_EN
		/// <summary> GetIndex1 </summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary> GetIndex1 </summary>
 		/// @endif
		public uint GetIndex1()
		{
			return PhysicsUtility.UnpackIdx1(totalIndex);
		}
		
		// @if LANG_EN
		/// <summary> GetIndex2 </summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary> GetIndex2 </summary>
 		/// @endif		
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
		/// <summary> setter and getter of localAnchor2 </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of localAnchor2 </summary>	
		/// @endif
		public Vector2 LocalAnchor2
		{
			set { this.localAnchor2 = value; }
			get { return this.localAnchor2; }
		}
		
		
		/// @if LANG_EN
		/// <summary> setter and getter of distance </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of distance </summary>	
		/// @endif
		public float Distance
		{
			set { this.distance = value; }
			get { return this.distance; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of currentDistance </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of currentDistance </summary>	
		/// @endif
		public float CurrentDistance
		{
			set { this.currentDistance = value; }
			get { return this.currentDistance; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of elastic </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of elastic </summary>	
		/// @endif
		public float Elastic
		{
			set { this.elastic = value; }
			get { return this.elastic; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of dumping </summary>	
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of dumping </summary>	
		/// @endif
		public float Dumping
		{
			set { this.dumping = value; }
			get { return this.dumping; }
		}		
		
		#endregion
		/// @if LANG_EN		
		/// <summary> 2d vector internal math function </summary>
		/// @endif
		/// @if LANG_JA	
		/// <summary> 2d vector 計算内部用関数 </summary>
		/// @endif
		private class Vector2Util
		{ 
			#region Methods
			/// @if LANG_EN
			/// <summary> Cross Product </summary>
		    /// <param name="vec1"> 2d vector </param>
		    /// <param name="vec2"> 2d vector </param>
		    /// <returns> cross product of 2d vector and 2d vector </returns>
			/// @endif
			/// @if LANG_JA
			/// <summary> 外積計算 </summary>
		    /// <param name="vec1"> ２次元ベクトル </param>
		    /// <param name="vec2"> ２次元ベクトル </param>
		    /// <returns> ２次元ベクトルと２次元ベクトルの外積 </returns>
			/// @endif
		    public static float Cross(Vector2 vec1, Vector2 vec2)
		    {
				return vec1.X * vec2.Y - vec1.Y * vec2.X;
			}	
				
			/// @if LANG_EN
			/// <summary> Cross Product </summary>
			/// <param name="val"> scalar </param>
		    /// <param name="vec"> 2d vector </param>
		    /// <returns> cross product of scalar and 2d vector </returns>
			/// @endif
			/// @if LANG_JA
			/// <summary> 外積計算 </summary>
		    /// <param name="val"> スカラー </param>
		    /// <param name="vec"> ２次元ベクトル </param>
		    /// <returns> スカラーと２次元ベクトルの外積 </returns>
			/// @endif
		    public static Vector2 Cross(float val, Vector2 vec)
			{
				Vector2 result = new Vector2(-val * vec.Y, val * vec.X);
				return result;
			}
				
			/// @if LANG_EN
			/// <summary> Cross Product </summary>
			/// <param name="vec"> 2d vector </param>
			/// <param name="val"> scalar </param>
			/// <returns> cross product of 2d vector and scalar </returns>
			/// @endif
			/// @if LANG_JA
			/// <summary> 外積計算 </summary>
			/// <param name="vec"> ２次元ベクトル </param>
			/// <param name="val"> スカラー </param>
			/// <returns> ２次元ベクトルとスカラーの外積 </returns>
			/// @endif
			public static Vector2 Cross(Vector2 vec, float val)
			{
				Vector2 result = new Vector2(val * vec.Y, -val * vec.X);
				return result;
			}
			#endregion
		}	

    }
}
