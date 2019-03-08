/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;

//
//  Colllision Solver Phase of Simulation
//
namespace Sce.PlayStation.HighLevel.Physics2D
{
	public partial class PhysicsScene
    {	
		
		/// @if LANG_EN
		/// <summary> Solver phase, this is algorithm class and all functions are defined as static functions </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> Solver phase, アルゴリズムの実現クラスで全ての関数がStatic関数として定義されます </summary>
		/// @endif 
	    private partial class Solver
	    {		
	
			#region Field
			/// @if LANG_EN
			/// <summary>whether use unique value for the scene</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>シーン特性にユニークな値を使うか</summary>
			/// @endif
			public static bool uniqueProperty = true;
			
			/// @if LANG_EN
			/// <summary>repulse accelaration for penetration</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>めり込みに対する反発力の加速係数</summary>
			/// @endif
	        public static float penetrationRepulse = 0.2f;
				
			/// @if LANG_EN
			/// <summary>penetration limitation</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>めり込みに対する許容度</summary>
			/// @endif 
			public static float penetLimit = 0.03f;
			
			/// @if LANG_EN
			/// <summary>tangent friction for repluse</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>反発に対する接線方向の摩擦係数</summary>
			/// @endif
			public static float tangentFriction = 0.3f;
	        
			/// @if LANG_EN
			/// <summary>restitution coefficient for repluse</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>反発に対する反発係数</summary>
			/// @endif
			public static float restitutionCoeff = 0.0f;
	
	        // For slippy scene
		    //public static float penetrationRepulse = 1.0f;
			//public static float penetLimit = 0.0f;
			//public static float tangentFriction = 0.0f;
			//public static float restitutionCoeff = 0.0f;
			#endregion
			
			#region Methods
			/// @if LANG_EN
			/// <summary> PreStepOfSolver </summary>
	        /// <param name="body1"> one of rigid body pair </param>
	        /// <param name="body2"> the other of rigid body pair </param> 
	        /// <param name="pairSol"> PhysicsSolverPair for contact response </param>
			/// @endif
			/// @if LANG_JA
			/// <summary> PreStepOfSolver </summary>
	        /// <param name="body1"> 剛体ペアの一方</param>
	        /// <param name="body2"> 剛体ペアのもう一方 </param> 
	        /// <param name="pairSol"> この衝突に対するPhysicsSolverPair </param>
			/// @endif 
	        internal static void PreStepOfSolver(PhysicsBody body1, PhysicsBody body2, ref PhysicsSolverPair pairSol)
	        {
				if((body1.type == BodyType.Trigger) || (body2.type == BodyType.Trigger))
				{
	        		pairSol.Pn = 0.0f;
	        		pairSol.Pt = 0.0f;
	        		pairSol.massNormal = 0.0f;
	        		pairSol.massTangent = 0.0f;
					return;
				}
				
				// This is required when kinematic object is moving
				if(body1.IsKinematic() && (body1.sleep == false))
				{
					body2.sleep = false; 
					body2.sleepCount = 0;
				}
				
				if(body2.IsKinematic() && (body2.sleep == false))
				{
					body1.sleep = false; 
					body1.sleepCount = 0;
				}
				
		        Vector2 r1 = pairSol.resA - body1.position;
		        Vector2 r2 = pairSol.resB - body2.position;
				
		        Vector2 v1 = body1.velocity;
		        Vector2 v2 = body2.velocity;
		        
				float vr1 = body1.angularVelocity;
		        float vr2 = body2.angularVelocity;
				
		        float inv1 = body1.invMass;
		        float inv2 = body2.invMass;
		        
				float invI1 = body1.invInertia;
		        float invI2 = body2.invInertia;
				
		        Vector2 normal = pairSol.normal;
		        
				float massNormal = pairSol.massNormal;
		        float massTangent = pairSol.massTangent;
		        
				float cPt = pairSol.Pt;
		        float cPn = pairSol.Pn;
	
		        float rn1 = Vector2.Dot(r1, normal);
		        float rn2 = Vector2.Dot(r2, normal);
		        float kNormal = inv1 + inv2;
	
		        kNormal += invI1 * (Vector2.Dot(r1, r1) - rn1 * rn1) + invI2 * (Vector2.Dot(r2, r2) - rn2 * rn2);
		        massNormal = 1.0f/kNormal;
	
	            Vector2 tangent = Vector2Util.Cross(normal, 1.0f);
		        float rt1 = Vector2.Dot(r1, tangent);
		        float rt2 = Vector2.Dot(r2, tangent);
		        float kTangent = inv1 + inv2;
	            kTangent += invI1 * (Vector2.Dot(r1, r1) - rt1 * rt1) + invI2 * (Vector2.Dot(r2, r2) - rt2 * rt2);
		        massTangent = 1.0f /  kTangent;
				
		        Vector2 Pn = cPn * normal;
	
		        // colllision friction for stability
		      	if(true)
				{
			        v1 -= body1.colFriction*v1;
			        vr1  -= body1.colFriction * vr1;
			        v2 -= body2.colFriction*v2;
			        vr2 -= body2.colFriction * vr2;
		        }
	
		        v1 -= inv1 * Pn;
	            vr1 -= invI1 * Vector2Util.Cross(r1, Pn);
	        	
		        v2 += inv2 * Pn;
	            vr2 += invI2 * Vector2Util.Cross(r2, Pn);
	
		        Vector2 Pt = cPt * tangent;
	
		        v1 -= inv1 * Pt;
	            vr1 -= invI1 * Vector2Util.Cross(r1, Pt);
	        	
		        v2 += inv2 * Pt;
	            vr2 += invI2 * Vector2Util.Cross(r2, Pt);
	            
		        pairSol.massNormal = massNormal;
		        pairSol.massTangent = massTangent;
	
		        body1.velocity = v1;
		        body1.angularVelocity = vr1;
		        body2.velocity = v2;
		        body2.angularVelocity = vr2;
	        }
			/// @if LANG_EN
	  		/// <summary> AfterStepOfSolver </summary>
	        /// <param name="body1"> one of rigid body pair </param>
	        /// <param name="body2"> the other of rigid body pair </param> 
	        /// <param name="pairSol"> PhysicsSolverPair for contact response </param>
			/// <param name="inv_dt"> inverse of time step </param>
	        /// <param name="flag"> first iteration of each simulation </param> 
	        /// <param name="iteration_num"> total iteration num for solver </param>
			/// @endif
			/// @if LANG_JA
	  		/// <summary> AfterStepOfSolver </summary>
	        /// <param name="body1"> 剛体ペアの一方 </param>
	        /// <param name="body2"> 剛体ペアのもう一方 </param> 
	        /// <param name="pairSol"> この衝突に対するpairSol </param>
			/// <param name="inv_dt"> シミュレーションタイムステップの逆数 </param>
	        /// <param name="flag"> 最初のSolver繰り返しかどうか </param> 
	        /// <param name="iteration_num"> Solverの合計繰り返し回数 </param>
			/// @endif
	        internal static void AfterStepOfSolver(PhysicsBody body1, PhysicsBody body2, ref PhysicsSolverPair pairSol, float inv_dt, bool flag, int iteration_num)
	        {		
				if((body1.type == BodyType.Trigger) || (body2.type == BodyType.Trigger))
				{
					return;
				}
					
		        Vector2 r1 = pairSol.resA - body1.position;
		        Vector2 r2 = pairSol.resB - body2.position;
	
		        Vector2 v1 = body1.velocity;
		        Vector2 v2 = body2.velocity;
		        float vr1 = body1.angularVelocity;
		        float vr2 = body2.angularVelocity;
		        float inv1 = body1.invMass;
		        float inv2 = body2.invMass;
		        float invI1 = body1.invInertia;
		        float invI2 = body2.invInertia;
	
		        float penetrationDepth = pairSol.penetrationDepth;
		        Vector2 normal = pairSol.normal;
		        float massNormal = pairSol.massNormal;
		        float massTangent = pairSol.massTangent;
		        float cPt = pairSol.Pt;
		        float cPn = pairSol.Pn;
	
	            Vector2 dv = v2 + Vector2Util.Cross(vr2, r2) - v1 - Vector2Util.Cross(vr1, r1);
	
	            normal.Normalize();
	
	            float vn = Vector2.Dot(dv, normal);
	
		        float dPn;
				
				if(uniqueProperty)		
		            dPn = massNormal * (-(1.0f + restitutionCoeff) * vn 
						+ Math.Abs(Math.Max(0, penetrationDepth - penetLimit) * penetrationRepulse * (5.0f/(float)iteration_num) * inv_dt));
		        else
					dPn = massNormal * (-(1.0f + Math.Max(body1.restitutionCoeff, body2.restitutionCoeff)) * vn 
					    + Math.Abs(Math.Max(0, penetrationDepth - Math.Max(body1.penetLimit, body2.penetLimit)) * Math.Max(body1.penetrationRepulse, body2.penetrationRepulse) * (5.0f/(float)iteration_num) * inv_dt));
				
				float Pn0 = cPn;
		        cPn = (float)Math.Min(Math.Max(Pn0 + dPn, 0.0), PhysicsUtility.FltMax);
		        dPn = cPn - Pn0;
	
		        Vector2 Pn = dPn * normal;
	
		        v1 -= inv1 * Pn;
	            vr1 -= invI1 * Vector2Util.Cross(r1, Pn);
	        	
		        v2 += inv2 * Pn;
	            vr2 += invI2 * Vector2Util.Cross(r2, Pn);
	
	            dv = v2 + Vector2Util.Cross(vr2, r2) - v1 - Vector2Util.Cross(vr1, r1);
	
	            Vector2 tangent = Vector2Util.Cross(normal, 1.0f);
	
		        float vt = Vector2.Dot(dv, tangent);
		        float dPt = massTangent * (-vt);
	
				float maxPt;
				
				if(uniqueProperty)
					maxPt = tangentFriction * cPn;
				else
					maxPt = (float)Math.Sqrt((double)body1.tangentFriction * (double)body2.tangentFriction) * cPn;
				
		        float oldTangentImpulse = cPt;
		        cPt = Math.Max(-maxPt, Math.Min(maxPt, oldTangentImpulse + dPt));
		        dPt = cPt - oldTangentImpulse;
	
		        Vector2 Pt = dPt * tangent;
	
		        v1 -= inv1 * Pt;
	            vr1 -= invI1 * Vector2Util.Cross(r1, Pt);
	        	
		        v2 += inv2 * Pt;
	            vr2 += invI2 * Vector2Util.Cross(r2, Pt);
	
		        pairSol.Pn = cPn;
		        pairSol.Pt = cPt;
	
		        body1.velocity = v1;
		        body1.angularVelocity = vr1;
		        body2.velocity = v2;
		        body2.angularVelocity = vr2;
			}
			
			#endregion		
	    }
	}
}
