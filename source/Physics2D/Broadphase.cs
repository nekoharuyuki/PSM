/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;

//
//  Broad Phase of Simulation
//
namespace Sce.PlayStation.HighLevel.Physics2D
{    

	
   	public partial class PhysicsScene
    {	
		
		/// @if LANG_EN
		/// <summary> Switch the combination of collisionFilter and groupFilter </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> collisionFilterとgroupFilterの組み合わせを変更する </summary>
		/// @endif
		public enum BroadOpType 
		{
			/// @if LANG_EN
			/// <summary> OR operation</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary> OR操作</summary>
			/// @endif
			Or, 
			
			/// @if LANG_EN
			/// <summary> AND operation</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary> AND操作</summary>
			/// @endif 
			And
		}
		
		/// @if LANG_EN
		/// <summary> Broadphase </summary>
		/// <remarks> broadphase phase, this is algorithm class and all functions are defined as static functions </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> Broadphase </summary>
		/// <remarks> broadphase phase, アルゴリズムの実現クラスで全ての関数がStatic関数として定義されます </remarks>
		/// @endif 
	    private class Broadphase
	    {
			#region Fields
			/// @if LANG_EN
			/// <summary>filterOp</summary>
	        /// <remarks>OR / AND switch for collilsion and group filter combination</remarks>
	        /// @endif
			/// @if LANG_JA
			/// <summary>filterOp</summary>
	        /// <remarks>ORとANDのcollilsion and group filter の組み合わせを変更します</remarks>
	        /// @endif
			public static BroadOpType filterOp;
			#endregion
			
			#region Methods
			/// @if LANG_EN
			/// <summary> AABB_BroadPhase </summary>
	        /// <param name="ob1"> one of rigid body pair</param>
	        /// <param name="ob2"> the other of rigid body pair </param>
	        /// <returns> return the result whether there is broadphase collision by Axis Aligned Bounding Box check </returns>
	        /// @endif
			/// @if LANG_JA
			/// <summary> AABB_BroadPhase </summary>
	        /// <param name="ob1"> 剛体ペアの一方</param>
	        /// <param name="ob2"> 剛体ペアのもう一方 </param>
	        /// <returns> Axis Aligned Bounding BoxチェックによってBroad Phaseの衝突があったか結果を返します </returns>
	        /// @endif
	        internal static bool AABB_BroadPhase(PhysicsBody ob1, PhysicsBody ob2)
	        {
				// both objects are slepping
				// or sleeping dynamic object vs kinematic which also can sleep if it is not set as non-sleep
				if((ob1.sleep == true)&&(ob2.sleep == true)) return false;
				
				// one of objects is sleeping and the other is static object
				if((ob1.sleep == true)&&(ob2.invMass == 0)&&(ob2.IsStatic())) return false;
				
				// one of objects is sleeping and the other is static object
				if((ob1.invMass == 0)&&(ob1.IsStatic())&&(ob2.sleep == true)) return false;

	        	// both objects are static objects
				if ((ob1.invMass == 0) && (ob2.invMass == 0)) return false;
				
				// same collision filter for cancel of collision detection
				// same group filter can be considered
				if(filterOp == BroadOpType.Or)
				{
	            	if ((ob1.collisionFilter & ob2.collisionFilter) != 0 ||
						(ob1.groupFilter & ob2.groupFilter) == 0 )
						return false;
				}
				else
				{
	            	if ((ob1.collisionFilter & ob2.collisionFilter) != 0 &&
						(ob1.groupFilter & ob2.groupFilter) == 0 )
						return false;			
				}
				
				// belong to same compound body ?
				if (ob1.compound == ob2.compound) return false;
	      
				// checking bounding box test
	            if (ob1.aabbMin.X > ob2.aabbMax.X) return false;
	            if (ob2.aabbMin.X > ob1.aabbMax.X) return false;
	            if (ob1.aabbMin.Y > ob2.aabbMax.Y) return false;
	            if (ob2.aabbMin.Y > ob1.aabbMax.Y) return false;
	
				// check the flag for AABB true
	            ob1.broadCollide |= true;
	            ob2.broadCollide |= true;
	            return true;
	        }
			
			
			/// @if LANG_EN
			/// /// <summary> AABB_SimpleBroadPhase </summary>
	        /// <param name="ob1"> one of rigid body pair</param>
	        /// <param name="ob2"> the other of rigid body pair </param>
	        /// <returns> return the result whether there is broadphase collision by Axis Aligned Bounding Box check </returns>
	        /// @endif
			/// @if LANG_JA
			/// <summary> AABB_SimpleBroadPhase </summary>
	        /// <param name="ob1"> 剛体ペアの一方</param>
	        /// <param name="ob2"> 剛体ペアのもう一方 </param>
	        /// <returns> Axis Aligned Bounding BoxチェックによってBroad Phaseの衝突があったか結果を返します </returns>
	        /// @endif
	        internal static bool AABB_SimpleBroadPhase(PhysicsBody ob1, PhysicsBody ob2)
	        {								
				// checking bounding box test
	            if (ob1.aabbMin.X > ob2.aabbMax.X) return false;
	            if (ob2.aabbMin.X > ob1.aabbMax.X) return false;
	            if (ob1.aabbMin.Y > ob2.aabbMax.Y) return false;
	            if (ob2.aabbMin.Y > ob1.aabbMax.Y) return false;
				
	            return true;
	        }
			
			
			/// @if LANG_EN
			/// <summary> AABB_Point_BroadPhase </summary>
	        /// <param name="ob"> rigid body </param>
	        /// <param name="pos"> point </param>
	        /// <returns> return the result whether point is inside AABB of rigid body </returns>
	       	/// @endif
			/// @if LANG_JA
			/// <summary> AABB_Point_BroadPhase </summary>
	        /// <param name="ob"> 剛体 </param>
	        /// <param name="pos"> 点 </param>
	        /// <returns> 点が剛体の領域に含まれるかの結果を返します </returns>
	       	/// @endif
	        internal static bool AABB_Point_BroadPhase(PhysicsBody ob, Vector2 pos)
	        {
		        if(ob.aabbMin.X > pos.X) return false;
		        if(ob.aabbMax.X < pos.X) return false;
		        if(ob.aabbMin.Y > pos.Y) return false;
		        if(ob.aabbMax.Y < pos.Y) return false;
		        return true;
	        }
			
			/// @if LANG_EN		
			/// <summary> setter and getter of filterOp </summary>
	       	/// @endif
			/// @if LANG_JA	
			/// <summary> setter and getter of filterOp </summary>
	       	/// @endif
			public BroadOpType FilterOp
			{
				set { filterOp = value; }
				get { return filterOp; }
			}
			#endregion
	    }
		
	}
}
