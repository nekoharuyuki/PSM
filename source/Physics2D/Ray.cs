/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;

//
//  Total Phase of Simulation
//
namespace Sce.PlayStation.HighLevel.Physics2D
{
	/// @if LANG_EN
	/// <summary> Ray information for raycast </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary> レイキャスト用のレイ情報 </summary>
	/// @endif
	public struct RayHit
	{
		#region Fields
		/// @if LANG_EN
		/// <summary>start position of the ray</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>レイのスタート地点</summary>
		/// @endif
		public Vector2 start;
		
		/// @if LANG_EN
		/// <summary>start position of the ray</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>レイの終了地点</summary>
		/// @endif
		public Vector2 end;

		/// @if LANG_EN
		/// <summary>contact position of the ray</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>レイの衝突地点</summary>
		/// @endif
		public Vector2 hit;
		
		/// @if LANG_EN
		/// <summary>contact index(rigid body) of the ray</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>レイの衝突する剛体のインデックス</summary>
		/// @endif
		public int hitIndex;
		
		/// @if LANG_EN
		/// <summary>contact depth of the ray</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>レイの衝突深さ</summary>
		/// @endif
		public float depth;
		
		/// @if LANG_EN
		/// <summary>contact ratio of the ray</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>レイの衝突割合</summary>
		/// @endif
		public float ratio;
		#endregion
		
		
		#region Methods
		/// @if LANG_EN
		/// <summary> setter and getter of start </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of start </summary>
		/// @endif
		public Vector2 Start
		{
			set { this.start = value; }
			get { return this.start; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of end </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of end </summary>
		/// @endif
		public Vector2 End
		{
			set { this.end = value; }
			get { return this.end; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of hit </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of hit </summary>
		/// @endif
		public Vector2 Hit
		{
			set { this.hit = value; }
			get { return this.hit; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of hitIndex </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of hitIndex </summary>
		/// @endif
		public int HitIndex
		{
			set { this.hitIndex = value; }
			get { return this.hitIndex; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of depth </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of depth </summary>
		/// @endif
		public float Depth
		{
			set { this.depth = value; }
			get { return this.depth; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of ratio </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of ratio </summary>
		/// @endif
		public float Ratio
		{
			set { this.ratio = value; }
			get { return this.ratio; }
		}
		#endregion
		
	}

    public partial class PhysicsScene
    {


		private PhysicsStopwatch swRay = new PhysicsStopwatch();

		/// @if LANG_EN
		/// <summary> Raycast check </summary>
        /// <param name="ray"> Ray information for raycast　</param>
       	/// <returns> return wheter ray hit exsists </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> レイキャストチェック </summary>
        /// <param name="ray"> レイキャスト用のレイ情報 </param>
       	/// <returns> レイの交差があるかないかを返す </returns>
        /// @endif
		public bool RaycastHit(ref RayHit ray)
        {
			return RaycastHit(ref ray, 0, 0xFFFFFFFF);
		}
		
		/// @if LANG_EN
		/// <summary> Raycast check </summary>
        /// <param name="ray"> Ray information for raycast　</param>
        /// <param name="collisionFilter"> collision filter as usual collision　</param>
        /// <param name="groupFilter"> group filter as usual collision </param>
       	/// <returns> return wheter ray hit exsists </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> レイキャストチェック </summary>
        /// <param name="ray"> レイキャスト用のレイ情報 </param>
        /// <param name="collisionFilter"> 通常の衝突判定と同様のコリジョンフィルター </param>
        /// <param name="groupFilter"> 通常の衝突判定と同様のグループフィルター </param>
       	/// <returns> レイの交差があるかないかを返す </returns>
        /// @endif
		public bool RaycastHit(ref RayHit ray, uint collisionFilter, uint groupFilter)
        {
	   
			swRay.printPerf = false;
			swRay.Reset();
            swRay.Start();
			
			Vector2 tempMin = Vector2.Min(ray.start, ray.end);
			Vector2 tempMax = Vector2.Max(ray.start, ray.end);
			
			// clear the ray information
			ray.hitIndex = -1;
			ray.ratio = PhysicsUtility.FltMax;
			
			for(int i=0; i<numBody; i++)
			{							
				PhysicsBody ob;
				ob = sceneBodies[i];
				
				// Filtering for Raycast
				if(filterOp == BroadOpType.Or)
				{
	            	if ((ob.collisionFilter & collisionFilter) != 0 ||
						(ob.groupFilter & groupFilter) == 0 )
						continue;
				}
				else
				{
	            	if ((ob.collisionFilter & collisionFilter) != 0 &&
						(ob.groupFilter & groupFilter) == 0 )
						continue;			
				}
				
				// AABB check at first
				if (ob.aabbMin.X > tempMax.X) continue;
	       	    if (tempMin.X > ob.aabbMax.X) continue;
	            if (ob.aabbMin.Y > tempMax.Y) continue;
	            if (tempMin.Y > ob.aabbMax.Y) continue;

				// find the point which ray touches
				float ratio = Collision.TestAxisCheckRaycast(ob, sceneShapes[sceneBodies[i].shapeIndex], ray.start, ray.end);
				
				if((ratio < 0.0f) || (ratio > 1.0f))
				{
					// this is out of range of line-segment
					continue;
				}
				else
				{
					if(ratio < ray.ratio)
					{
						ray.ratio = ratio;
						ray.hitIndex = i;
					}
				}
			}
			
	        swRay.Stop();
			swRay.Print("Raycast");
			
			if(ray.hitIndex != -1)
			{
				ray.hit = ray.start + ray.ratio * (ray.end - ray.start);
				ray.depth = (ray.hit - ray.start).Length();
				return true;
			}
			else 
				return false;

		}
	}
}
