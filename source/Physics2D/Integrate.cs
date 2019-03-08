/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;

//
// Simulation integration
//
namespace Sce.PlayStation.HighLevel.Physics2D
{
   	public partial class PhysicsScene
    {	
		/// @if LANG_EN
		/// <summary> Integrate phase, this is algorithm class and all functions are defined as static functions </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> Integrate phase, アルゴリズムの実現クラスで全ての関数がStatic関数として定義されます</summary>
		/// @endif
	    private class Integrate
	    {
			
			internal static Vector2 aabbBias = new Vector2(0.0f, 0.0f);
			#region Methods
			/// @if LANG_EN
			/// <summary> IntegratePhase </summary>
	        /// <param name="ob"> rigid body </param>
	        /// <param name="sceneShapes"> sceneShapes of the scene </param>
	  	    /// <param name="dt"> time step of simulation </param>
	  	    /// <param name="gravity"> gravity </param>
			/// @endif
			/// @if LANG_JA
			/// <summary> IntegratePhase </summary>
	        /// <param name="ob"> 剛体 </param>
	        /// <param name="sceneShapes"> シーンの形状リスト </param>
	  	    /// <param name="dt"> シミュレーションのタイムステップ </param>
	  	    /// <param name="gravity"> 重力 </param>
			/// @endif
	        internal static void IntegratePhase(PhysicsBody ob, PhysicsShape[] sceneShapes, float dt, Vector2 gravity)
	        {
	            if ((ob.invMass != 0.0f) && (ob.sleep != true))
	            {
	                ob.position += dt * ob.velocity;
	                ob.rotation += dt * ob.angularVelocity;
	
					// To make the integration phase stable, 
					// rotation should be between -Pi <-> Pi
					ob.rotation = PhysicsUtility.GetRadianMod(ob.rotation);
			
					Vector2 old_vel = ob.velocity;
	
	                ob.velocity += dt * (gravity + ob.invMass * ob.force);
	                ob.angularVelocity += dt * ob.invInertia * ob.torque;
	
	                ob.acceleration = (ob.velocity - old_vel) / dt;
	
	                // Air friction (ON or OFF)
	                if (true)
	                {
	                    ob.velocity -= ob.airFriction * ob.velocity;
	                    ob.angularVelocity -= ob.airFriction * ob.angularVelocity;
	                }
	            }
	            //　Cancel all velocity and angular velocity of static object for safety
	            else
	            {
	                ob.velocity = new Vector2(0, 0);
	                ob.angularVelocity = 0.0f;
	            }
				
			    if (ob.shapeIndex != uint.MaxValue)
	            {
	                ob.aabbMin = new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
	                ob.aabbMax = -new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
	
	                PhysicsShape cc = sceneShapes[ob.shapeIndex];
	
	                float localangle = ob.localRotation;
	                Vector2 localPosition = ob.localPosition;
	                float ca_local = (float)Math.Cos(localangle);
	                float sa_local = (float)Math.Sin(localangle);
	
	                float angle = ob.rotation;
	
	                float ca = (float)Math.Cos(angle);
	                float sa = (float)Math.Sin(angle);
	
	                if (cc.numVert == 0)
	                {
	                    Vector2 v = new Vector2(
	                        ca * localPosition.X - sa * localPosition.Y,
	                        sa * localPosition.X + ca * localPosition.Y
	                        );
	
	                    ob.aabbMin = v - new Vector2(cc.vertList[0].X, cc.vertList[0].X);
	                    ob.aabbMax = v + new Vector2(cc.vertList[0].X, cc.vertList[0].X);
	                }
	                else
	                {
	                    for (int i = 0; i < cc.numVert; i++)
	                    {
	                        Vector2 v = cc.vertList[i];
	
	                        v = new Vector2(
	                                ca_local * v.X - sa_local * v.Y,
	                                sa_local * v.X + ca_local * v.Y
	                                );
	
	                        v += localPosition;
	
	                        v = new Vector2(
	                                ca * v.X - sa * v.Y,
	                                sa * v.X + ca * v.Y
	                                );
	
	                        ob.aabbMin = Vector2.Min(ob.aabbMin, v);
	                        ob.aabbMax = Vector2.Max(ob.aabbMax, v);
	                    }
	                }
	
	                ob.aabbMin = ob.aabbMin + ob.position;
	                ob.aabbMax = ob.aabbMax + ob.position;
	
	            }		
				
	            ob.force = new Vector2(0.0f, 0.0f);
	            ob.torque = 0.0f;
	
	        }
			
			/// @if LANG_EN
			/// <summary> PreIntegratePhase </summary>
	        /// <param name="ob"> rigid body </param>
	        /// <param name="sceneShapes"> sceneShapes of the scene </param>
	  	    /// <param name="dt"> time step of simulation </param>
	  	    /// <param name="gravity"> gravity </param>
			/// @endif
			/// @if LANG_JA
			/// <summary> PreIntegratePhase </summary>
	        /// <param name="ob"> 剛体 </param>
	        /// <param name="sceneShapes"> シーンの形状リスト </param>
	  	    /// <param name="dt"> シミュレーションのタイムステップ </param>
	  	    /// <param name="gravity"> 重力 </param>
			/// @endif
	        internal static void PreIntegratePhase(PhysicsBody ob, PhysicsShape[] sceneShapes, float dt, Vector2 gravity)
	        {
				
	            //　Cancel all velocity and angular velocity of static object for safety
				if (ob.IsStatic() || (ob.sleep == true))
				{
					ob.velocity = new Vector2(0, 0);
					ob.angularVelocity = 0.0f;
				}
					
	            if (ob.shapeIndex != uint.MaxValue)
	            {
	                ob.aabbMin = new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
	                ob.aabbMax = -new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
	
	                PhysicsShape cc = sceneShapes[ob.shapeIndex];
	
	                float localangle = ob.localRotation;
	                Vector2 localPosition = ob.localPosition;
	                float ca_local = (float)Math.Cos(localangle);
	                float sa_local = (float)Math.Sin(localangle);
	
	                float angle = ob.rotation;
	
	                float ca = (float)Math.Cos(angle);
	                float sa = (float)Math.Sin(angle);
	
	                if (cc.numVert == 0)
	                {
	                    Vector2 v = new Vector2(
	                        ca * localPosition.X - sa * localPosition.Y,
	                        sa * localPosition.X + ca * localPosition.Y
	                        );
	
	                    ob.aabbMin = v - new Vector2(cc.vertList[0].X, cc.vertList[0].X);
	                    ob.aabbMax = v + new Vector2(cc.vertList[0].X, cc.vertList[0].X);
	                }
	                else
	                {
	                    for (int i = 0; i < cc.numVert; i++)
	                    {
	                        Vector2 v = cc.vertList[i];
	
	                        v = new Vector2(
	                                ca_local * v.X - sa_local * v.Y,
	                                sa_local * v.X + ca_local * v.Y
	                                );
	
	                        v += localPosition;
	
	                        v = new Vector2(
	                                ca * v.X - sa * v.Y,
	                                sa * v.X + ca * v.Y
	                                );
	
	                        ob.aabbMin = Vector2.Min(ob.aabbMin, v);
	                        ob.aabbMax = Vector2.Max(ob.aabbMax, v);
	                    }
	                }
	
	                ob.aabbMin = ob.aabbMin + ob.position + aabbBias;
	                ob.aabbMax = ob.aabbMax + ob.position - aabbBias;
	
	            }
	
	            ob.broadCollide = false;
	            ob.narrowCollide = false;
	        }
	        
						
			/// @if LANG_EN
			/// <summary> UpdateBoundingBox </summary>
	        /// <param name="ob"> rigid body </param>
	        /// <param name="sceneShapes"> sceneShapes of the scene </param>
			/// @endif
			/// @if LANG_JA
			/// <summary> UpdateBoundingBox </summary>
	        /// <param name="ob"> 剛体 </param>
	        /// <param name="sceneShapes"> シーンの形状リスト </param>
			/// @endif
	        internal static void UpdateBoundingBox(PhysicsBody ob, PhysicsShape[] sceneShapes)
	        {
	            if (ob.shapeIndex != uint.MaxValue)
	            {
	                ob.aabbMin = new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
	                ob.aabbMax = -new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
	
	                PhysicsShape cc = sceneShapes[ob.shapeIndex];
	
	                float localangle = ob.localRotation;
	                Vector2 localPosition = ob.localPosition;
	                float ca_local = (float)Math.Cos(localangle);
	                float sa_local = (float)Math.Sin(localangle);
	
	                float angle = ob.rotation;
	
	                float ca = (float)Math.Cos(angle);
	                float sa = (float)Math.Sin(angle);
	
	                if (cc.numVert == 0)
	                {
	                    Vector2 v = new Vector2(
	                        ca * localPosition.X - sa * localPosition.Y,
	                        sa * localPosition.X + ca * localPosition.Y
	                        );
	
	                    ob.aabbMin = v - new Vector2(cc.vertList[0].X, cc.vertList[0].X);
	                    ob.aabbMax = v + new Vector2(cc.vertList[0].X, cc.vertList[0].X);
	                }
	                else
	                {
	                    for (int i = 0; i < cc.numVert; i++)
	                    {
	                        Vector2 v = cc.vertList[i];
	
	                        v = new Vector2(
	                                ca_local * v.X - sa_local * v.Y,
	                                sa_local * v.X + ca_local * v.Y
	                                );
	
	                        v += localPosition;
	
	                        v = new Vector2(
	                                ca * v.X - sa * v.Y,
	                                sa * v.X + ca * v.Y
	                                );
	
	                        ob.aabbMin = Vector2.Min(ob.aabbMin, v);
	                        ob.aabbMax = Vector2.Max(ob.aabbMax, v);
	                    }
	                }
	
	                ob.aabbMin = ob.aabbMin + ob.position + aabbBias;
	                ob.aabbMax = ob.aabbMax + ob.position - aabbBias;
	
	            }
	        }
	        
			#endregion
		}
	}
}
