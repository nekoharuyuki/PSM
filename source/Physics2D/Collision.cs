/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;


//
//	Collision Phase of Simulation
//
namespace Sce.PlayStation.HighLevel.Physics2D
{
   	public partial class PhysicsScene
    {		
	#region Methods
	/// @if LANG_EN
	/// <summary> Collision phase, this is algorithm class and all functions are defined as static functions </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary> Collision phase, アルゴリズムの実現クラスで全ての関数がStatic関数として定義されます </summary>
	/// @endif 
    private class Collision
    {
		/// @if LANG_EN
        /// <summary> DetectSphereLineClosestPoint </summary>
        /// <remarks> Sphere vs Line-segment closest point check </remarks>
        /// <param name="PosA"> one of points of line segment </param>
        /// <param name="PosA2"> the other of points of line segment </param>
        /// <param name="Center"> center of sphere </param>
        /// <param name="radius"> radius of sphere </param>
       	/// <returns> return test value</returns>
       	/// @endif
		/// @if LANG_JA
        /// <summary> DetectSphereLineClosestPoint </summary>
        /// <remarks> 球と線分の近接点の判定をします </remarks>
        /// <param name="PosA"> 線分の端点 </param>
        /// <param name="PosA2"> 線分のもう一方の端点 </param>
        /// <param name="Center"> 球の中心 </param>
        /// <param name="radius"> 球の半径 </param>
       	/// <returns> 判定の結果を返します </returns>
       	/// @endif
        public static int DetectSphereLineClosestPoint(Vector2 PosA, Vector2 PosA2, Vector2 Center, float radius)
        {
	        float len = radius;
	        int index = -1;
        	
	        if(Math.Sqrt(Vector2.Dot(PosA - Center, PosA - Center)) < len)
	        {
                len = (float)Math.Sqrt(Vector2.Dot(PosA - Center, PosA - Center));
		        index = 0;
	        }

            if (Math.Sqrt(Vector2.Dot(PosA2 - Center, PosA2 - Center)) < len)
	        {
                len = (float)Math.Sqrt(Vector2.Dot(PosA2 - Center, PosA2 - Center));
		        index = 1;
	        }

	        Vector2 Vec = PosA2 - PosA;
            float lenVec = (float)Math.Sqrt(Vector2.Dot(Vec, Vec));
	        Vec /= lenVec;

            float t = Vector2.Dot(Center - PosA, Vec);

	        if((t > 0.0) && (t < lenVec))
	        {
		        Vector2 PosC = PosA + t * Vec;

                if (Math.Sqrt(Vector2.Dot(PosC - Center, PosC - Center)) < len)
		        {
                    len = (float)Math.Sqrt(Vector2.Dot(PosC - Center, PosC - Center));
			        index = 2;
		        }
	        }
			
	        return index;

        }
			
			
		/// @if LANG_EN
        /// <summary> DetectSphereLineCrossPoint </summary>
        /// <remarks> Calculate the cross point of line-segment vs sphere </remarks>
        /// <param name="posA1"> one of points of line segment </param>
        /// <param name="posA2"> the other of points of line segment </param>
        /// <param name="center"> center of sphere </param>
        /// <param name="radius"> radius of sphere </param>
       	/// <returns> return the point of line-segment as ratio </returns>
       	/// @endif
		/// @if LANG_JA
        /// <summary> DetectSphereLineCrossPoint </summary>
        /// <remarks> 球と線分の交差点の計算をします </remarks>
        /// <param name="posA1"> 線分の端点 </param>
        /// <param name="posA2"> 線分のもう一方の端点 </param>
        /// <param name="center"> 球の中心 </param>
        /// <param name="radius"> 球の半径 </param>
       	/// <returns> 交差している線分の位置を割合で返す </returns>
       	/// @endif
        public static float DetectSphereLineCrossPoint(Vector2 posA1, Vector2 posA2, Vector2 center, float radius)
		{				
			Vector2 dir = posA2 - posA1;	
			
			float aArg = Vector2.Dot (dir, dir);
			float bArg = 2.0f * Vector2.Dot (posA1 - center, dir);
			float cArg = Vector2.Dot (posA1 - center, posA1 - center) - radius * radius;
			float dArg = bArg * bArg - 4.0f * aArg * cArg;
				
			if(dArg > 0.0f)
			{
				dArg = (float)Math.Sqrt(dArg);
				float root = (-bArg - dArg)/(2.0f * aArg);
		
				if((root < 0.0f) || (root > 1.0f))
				{
				 	root = (-bArg + dArg)/(2.0f * aArg);
						
					if((root < 0.0f) || (root > 1.0f))
						return -1.0f;
					else
						return root;	
				}
				else 
				{
					return root;
				}
			}
				
			else
				return -1.0f;
        }


		/// @if LANG_EN
        /// <summary> DetectLineLineCrossPoint </summary>
        /// <remarks> Calculate the cross point of line-segment vs line-segment </remarks>
        /// <param name="PosA"> one of points of line segment </param>
        /// <param name="PosA2"> the other of points of line segment </param>
        /// <param name="PosB"> one of points of line segment </param>
        /// <param name="PosB"> the other of points of line segment </param>
       	/// <returns> return the point of line-segment as ratio </returns>
       	/// @endif
		/// @if LANG_JA
        /// <summary> DetectLineLineCrossPoint </summary>
        /// <remarks> 線分と線分の交差点の計算をします </remarks>
        /// <param name="PosA"> 線分の端点 </param>
        /// <param name="PosA2"> 線分のもう一方の端点 </param>
        /// <param name="PosB"> 球の中心 </param>
        /// <param name="PosB2"> 球の半径 </param>
       	/// <returns> 交差している線分の位置を割合で返す </returns>
       	/// @endif	
        public static float DetectLineLineCrossPoint(Vector2 PosA, Vector2 PosA2, Vector2 PosB, Vector2 PosB2)
        {

			Vector2 dirA = PosA2 - PosA;
			Vector2 dirB = PosB2 - PosB;
			Vector2 normalB = new Vector2(-dirB.Y, dirB.X);
		
			float dot = Vector2.Dot (dirA, normalB);
				
			if(dot == 0.0f)
					return -1.0f;
				
			float ratioA = Vector2.Dot(PosB - PosA, normalB)/dot;

			if((ratioA < 0.0f) || (ratioA > 1.0f))
				return -1.0f;
			
			Vector2 crossPos = PosA + ratioA * dirA;
				
			float ratioB = Vector2.Dot (crossPos - PosB, dirB)/Vector2.Dot (dirB, dirB);
				
			if((ratioB < 0.0f) || (ratioB > 1.0f))
				return -1.0f;
			else
				return ratioA;
				
		}
			
			
		/// @if LANG_EN
	    /// <summary> DetectLineLineClosestPoint </summary>
	    /// <remarks> Line-segment vs Line-segment closest point check </remarks>
        /// <param name="resultPos"> given result by Separate Axis Theorem </param>
        /// <param name="resultPair"> contact point1 which will be given to Solver Phase </param>
        /// <param name="resultPair2"> contact point2 which will be given to Solver Phase </param>
        /// <returns>return number of contact points (1 or 2) for the contact pair </returns>
        /// @endif
		/// @if LANG_JA
	    /// <summary> DetectLineLineClosestPoint </summary>
	    /// <remarks> 線分と線分の近接点を判定します </remarks>
        /// <param name="resultPos"> Separate Axis Theorem判定による結果 </param>
        /// <param name="resultPair"> Solver Phaseに与えられる接触点１</param>
        /// <param name="resultPair2"> Solver Phaseに与えられる接触点２</param>
        /// <returns> 衝突ペアに対する接触点の数（１個または２個）を返します </returns>
        /// @endif
        public static int DetectLineLineClosestPoint(ContactPoint resultPos, ref PhysicsSolverPair resultPair, ref PhysicsSolverPair resultPair2)
        {
            int i;
			
            // This pair is removed by Axis separate theorem because of flag setting
            if (resultPos.totalIndex == 0xFFFFFFFF)
            {
                return 0;
            }

            Vector2 PosA, PosA2, PosB, PosB2;
            Vector2 resA, resA2, resB, resB2;
            Vector2 axis_vec;
            float penetrationDepth, penetrationDepth2;
            Vector2 NewPosA, NewPosB, OldPosA, OldPosB;

            PosA = resultPos.PosA;
            PosA2 = resultPos.PosA2;

            PosB = resultPos.PosB;
            PosB2 = resultPos.PosB2;

            NewPosA = resultPos.NewPosA;
            NewPosB = resultPos.NewPosB;
            OldPosA = resultPos.OldPosA;
            OldPosB = resultPos.OldPosB;

            axis_vec = resultPos.normal;
            penetrationDepth = resultPos.penetrationDepth;

            Vector2 vecDirA = PosA2 - PosA;
            Vector2 vecDirB = PosB2 - PosB;

            float len_vecDirA = (float)Math.Sqrt(Vector2.Dot(vecDirA, vecDirA));
            float len_vecDirB = (float)Math.Sqrt(Vector2.Dot(vecDirB, vecDirB));
            vecDirA = vecDirA / len_vecDirA;
            vecDirB = vecDirB / len_vecDirB;

            PosA += NewPosA;
            PosA2 += NewPosA;
            PosB += NewPosB;
            PosB2 += NewPosB;

            float[] len = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
            Vector2[] tempA = new Vector2[4] { new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0) };
            Vector2[] tempB = new Vector2[4] { new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0) };
            uint[] type = new uint[4] { 0, 0, 0, 0 };

            // projection from PosA
            {
                float t = Vector2.Dot(PosA - PosB, vecDirB);

                if (t <= 0)
                {
                    len[0] = Vector2.Dot(PosA - PosB, PosA - PosB);
                    tempA[0] = PosA;
                    tempB[0] = PosB;
                    type[0] = 0x11;
                }
                else if (t >= len_vecDirB)
                {
                    len[0] = Vector2.Dot(PosA - PosB2, PosA - PosB2);
                    tempA[0] = PosA;
                    tempB[0] = PosB2;
                    type[0] = 0x12;
                }
                else
                {
                    Vector2 localB = PosB + t * vecDirB;
                    len[0] = Vector2.Dot(PosA - localB, PosA - localB);
                    tempA[0] = PosA;
                    tempB[0] = localB;
                    type[0] = 0x10;
                }
            }

            // projection from PosA2 
            {
                float t = Vector2.Dot(PosA2 - PosB, vecDirB);

                if (t <= 0)
                {
                    len[1] = Vector2.Dot(PosA2 - PosB, PosA2 - PosB);
                    tempA[1] = PosA2;
                    tempB[1] = PosB;
                    type[1] = 0x21;
                }
                else if (t >= len_vecDirB)
                {
                    len[1] = Vector2.Dot(PosA2 - PosB2, PosA2 - PosB2);
                    tempA[1] = PosA2;
                    tempB[1] = PosB2;
                    type[1] = 0x22;
                }
                else
                {
                    Vector2 localB = PosB + t * vecDirB;
                    len[1] = Vector2.Dot(PosA2 - localB, PosA2 - localB);
                    tempA[1] = PosA2;
                    tempB[1] = localB;
                    type[1] = 0x20;
                }
            }

            // projection from PosB
            {
                float t = Vector2.Dot(PosB - PosA, vecDirA);

                if (t <= 0)
                {
                    len[2] = Vector2.Dot(PosB - PosA, PosB - PosA);
                    tempA[2] = PosA;
                    tempB[2] = PosB;
                    type[2] = 0x11;
                }
                else if (t >= len_vecDirA)
                {
                    len[2] = Vector2.Dot(PosB - PosA2, PosB - PosA2);
                    tempA[2] = PosA2;
                    tempB[2] = PosB;
                    type[2] = 0x21;
                }
                else
                {
                    Vector2 localA = PosA + t * vecDirA;
                    len[2] = Vector2.Dot(localA - PosB, localA - PosB);
                    tempA[2] = localA;
                    tempB[2] = PosB;
                    type[2] = 0x01;
                }
            }


            // projection from PosB2
            {
                float t = Vector2.Dot(PosB2 - PosA, vecDirA);

                if (t <= 0)
                {
                    len[3] = Vector2.Dot(PosB2 - PosA, PosB2 - PosA);
                    tempA[3] = PosA;
                    tempB[3] = PosB2;
                    type[3] = 0x12;
                }
                else if (t >= len_vecDirA)
                {
                    len[3] = Vector2.Dot(PosB2 - PosA2, PosB2 - PosA2);
                    tempA[3] = PosA2;
                    tempB[3] = PosB2;
                    type[3] = 0x22;
                }
                else
                {
                    Vector2 localA = PosA + t * vecDirA;
                    len[3] = Vector2.Dot(localA - PosB2, localA - PosB2);
                    tempA[3] = localA;
                    tempB[3] = PosB2;
                    type[3] = 0x02;
                }
            }

            // detect closest point
            int index1 = -1;

            float len1 = PhysicsUtility.FltMax;
            float len2 = PhysicsUtility.FltMax;

            resA.X = 0.0f; resA.Y = 0.0f;
            resB.X = 0.0f; resB.Y = 0.0f;

			// sort four choices (which is the closest9
			for (i = 0; i < 4; i++)
            {
                if (len[i] < len1)
                {
                    len1 = len[i];
                    resA = tempA[i];
                    resB = tempB[i];
                    index1 = i;
                }
            }
			
            resA2 = resA;
            resB2 = resB;

            // remove double count contact point
            // check whether the same point is checked two times and remove it
            for (i = 0; i < 4; i++)
            {
                if (i == index1)
                    continue;

                if (len[i] < len2)
                {
                    // Both of points are PosA or PosA2 for bodyA ,and remove this choice
                    if (((type[index1] & type[i]) & 0xf0) != 0) continue;
					// Both of points are PosB or PosB2 for bodyB ,and remove this choice
                    if (((type[index1] & type[i]) & 0x0f) != 0) continue;
					
                    len2 = len[i];
                    resA2 = tempA[i];
                    resB2 = tempB[i];
                }
            }

            len1 = (float)Math.Sqrt(len1);
            len2 = (float)Math.Sqrt(len2);

            resA = resA - NewPosA + OldPosA;
            resA2 = resA2 - NewPosA + OldPosA;

            resB = resB - NewPosB + OldPosB;
            resB2 = resB2 - NewPosB + OldPosB;
			
			// all results are saved to resultPair (and resultPair2 if it exsists) 
			// Those will be sent to Solver Phase
            resultPair.resA = resA;
            resultPair.resB = resB;
            resultPair.Pt = resultPair.Pn = 0;
            resultPair.massNormal = resultPair.massTangent = 0;
            resultPair.normal = axis_vec;
            resultPair.totalIndex = resultPos.totalIndex;
            resultPair.penetrationDepth = penetrationDepth;

            // Compare the length of closest points pair vs the length of second closest points pair
			// In the case that there is no big difference, the number of contact points are considered as two points
            if (len2 < (len1 + penetrationDepth))
            {

                penetrationDepth2 = penetrationDepth + (len1 - len2);

                resultPair2.resA = resA2;
                resultPair2.resB = resB2;
                resultPair2.Pt = resultPair2.Pn = 0;
                resultPair2.massNormal = resultPair2.massTangent = 0;
                resultPair2.normal = axis_vec;
                resultPair2.totalIndex = resultPos.totalIndex;
                resultPair2.penetrationDepth = penetrationDepth2;

                return 2;
            }
            else
            {
                return 1;
            }
        }




		private static PhysicsShape aCon = new PhysicsShape();
		private static PhysicsShape bCon = new PhysicsShape();
	
		/// @if LANG_EN
		/// <summary> TestAxisCheck </summary>
		/// <remarks> Separate Axis Theorem </remarks>
        /// <param name="bodyA"> one of rigid body pair</param>
        /// <param name="bodyB"> the other of rigid body pair </param>
        /// <param name="sceneShapes"> convex shape list of the scene </param>
        /// <param name="aabbPairIndex"> collision check result </param>
        /// <param name="resultPos"> collision check result </param> 
        /// <param name="mapIndex"> this is an unused parameter for this release version </param> 
        /// <returns> return the result whether there is collision by Separate Axis Theorem </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> TestAxisCheck </summary>
		/// <remarks> Separate Axis Theorem判定</remarks>
        /// <param name="bodyA"> 剛体ペアの一方</param>
        /// <param name="bodyB"> 剛体ペアのもう一方 </param>
        /// <param name="sceneShapes"> シーンの形状リスト </param>
        /// <param name="aabbPairIndex"> Broadphase衝突判定の結果 </param>
        /// <param name="resultPos"> 衝突判定の結果 </param> 
        /// <param name="mapIndex"> 本バージョンでは使用しないパラメータ </param> 
        /// <returns> 衝突があったのかSeparate Axis Theorem判定による結果を返します </returns>
        /// @endif
		public static bool TestAxisCheck(PhysicsBody bodyA, PhysicsBody bodyB, PhysicsShape[] sceneShapes, ref uint aabbPairIndex, ref ContactPoint resultPos, int mapIndex)
        {
	        int i, j;
			
			//  Convex aCon, bCon;
       	 	//  aCon = new Convex(sceneShapes[a.shapeIndex]);
         	//  bCon = new Convex(sceneShapes[b.shapeIndex]);

			aCon.CopyFromConvex(ref sceneShapes[bodyA.shapeIndex]);
			bCon.CopyFromConvex(ref sceneShapes[bodyB.shapeIndex]);

	        float angle;
	        float ca, sa;

	        Vector2 localPosition;
	        float ca_local;
	        float sa_local;

	        angle = bodyA.rotation;
	        ca = (float)Math.Cos(angle);
            sa = (float)Math.Sin(angle);

	        localPosition = bodyA.localPosition;
            ca_local = (float)Math.Cos(bodyA.localRotation);
            sa_local = (float)Math.Sin(bodyA.localRotation);
        	
	        for(i=0; i<aCon.numVert; i++)
	        {
		        Vector2 v = aCon.vertList[i];
		        v = new Vector2(ca_local*v.X - sa_local*v.Y, sa_local*v.X + ca_local*v.Y);	
		        v += localPosition;
                v = new Vector2(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
		        v -= new Vector2(ca * bodyA.localPosition.X - sa * bodyA.localPosition.Y, sa * bodyA.localPosition.X + ca * bodyA.localPosition.Y);
		        aCon.vertList[i] = v;
	        }

	        Vector2 a_position; 
	        Vector2 b_position; 

	        a_position = bodyA.position + new Vector2(ca * bodyA.localPosition.X - sa * bodyA.localPosition.Y, sa * bodyA.localPosition.X + ca * bodyA.localPosition.Y);

	        angle = bodyB.rotation;
	        ca = (float)Math.Cos(angle);
	        sa = (float)Math.Sin(angle);

	        localPosition = bodyB.localPosition;
            ca_local = (float)Math.Cos(bodyB.localRotation);
            sa_local = (float)Math.Sin(bodyB.localRotation);
        	
	        for(i=0; i<bCon.numVert; i++)
	        {
		        Vector2 v = bCon.vertList[i];
		        v = new Vector2(ca_local*v.X - sa_local*v.Y, sa_local*v.X + ca_local*v.Y);	
		        v += localPosition;
		        v = new Vector2(ca*v.X - sa*v.Y, sa*v.X + ca*v.Y);	
		        v -= new Vector2(ca * bodyB.localPosition.X - sa * bodyB.localPosition.Y, sa * bodyB.localPosition.X + ca * bodyB.localPosition.Y);
		        bCon.vertList[i] = v;
	        }

	        b_position = bodyB.position + new Vector2(ca * bodyB.localPosition.X - sa * bodyB.localPosition.Y, sa * bodyB.localPosition.X + ca * bodyB.localPosition.Y);

			// Direction of Separate Axis
	        Vector2 testDir = new Vector2(0, 0);
            float penetrationDepth = PhysicsUtility.FltMax;
            float extraction_depth = -PhysicsUtility.FltMax;         
            float ra, rb, interval;
			
			// if result = true, it means that there is collision by Separate Axis Theorem
	        bool result = true;
			Vector2 axisVec = new Vector2(0, 0);
			
			// For bodyA
	        for(i=0; i<(aCon.numVert/aCon.hint); i++)
	        {
		        testDir = aCon.vertList[(i+1)%aCon.numVert] - aCon.vertList[i];
		        testDir /= (float)Math.Sqrt(Vector2.Dot(testDir, testDir));

				// there is difference of 90.0f degree between direction of edge and separate axis
		        if((aCon.numVert == 2) && (i==1))
		        {
			        continue;
		        }
		        else
		        {
                    testDir = Vector2Util.Cross(testDir, 1.0f);
		        }

		        // testDir should have the similar direction as well as b_position - a_position
		        interval = Vector2.Dot(b_position  - a_position, testDir);

		        if( interval < 0 )
		        {
			        testDir = -testDir;
			        interval = -interval;
		        }
		        else
		        {
			        // do nothing
		        }

 
		        ra = 0;
		        rb = 0;
				
				// biggest distance for bodyA
		        for(j=0; j<aCon.numVert; j++)
		        {
			        float len = Vector2.Dot(aCon.vertList[j], testDir);
			        if(len > ra)
			        {
				        ra = len;
			        }
		        }

		        // smallest (negative biggest) distance for bodyB
		        if(bCon.numVert != 0)
				{
					for(j=0; j<bCon.numVert; j++)
		        	{
			        	float len = Vector2.Dot(bCon.vertList[j], testDir);
			        	if(len < rb)
			        	{
				       	 rb = len;
			        	}
		        	}
				}
				else
				{
					rb = -bCon.vertList[0].X;
				}

		        // direction is returned to oposite direction
		        rb = -rb;

		        if(interval > ra + rb) {
			        result = false;
			        if( interval - (ra + rb) > extraction_depth)
			        {
				        extraction_depth = interval - (ra + rb);
				       	axisVec = testDir;;
			        }
	
				}
		        else if(result) {
			        if( (ra + rb - interval) < penetrationDepth)
			        {
				        penetrationDepth = ra + rb - interval;
				        axisVec = testDir;
			        }
		        }
	        }


			// For body B
	        for(i=0; i<(bCon.numVert/bCon.hint); i++)
	        {
		        testDir = bCon.vertList[(i+1)%bCon.numVert] - bCon.vertList[i];
		        testDir /= (float)Math.Sqrt(Vector2.Dot(testDir, testDir));

				// there is difference of 90 degree between direction and separate axis
		        if((bCon.numVert == 2) && (i==1))
		        {
			        continue;
		        }
		        else
		        {
                    testDir = Vector2Util.Cross(testDir, 1.0f);
		        }
        			
		        interval = Vector2.Dot(b_position - a_position, testDir);

		        if( interval < 0 )
		        {
			        testDir = -testDir;
			        interval = -interval;
		        }
		        else
		        {
			        // do nothing
		        }

		       
		        ra = 0;
		        rb = 0;
				
				// biggest distance for bodyA
				if(aCon.numVert != 0)
				{
			        for(j=0; j<aCon.numVert; j++)
			        {
				        float len = Vector2.Dot(aCon.vertList[j], testDir);
				        if(len > ra)
				        {
					        ra = len;
				        }
			        }
				}
				else
				{
					ra = aCon.vertList[0].X;	
				}

		        // lonest distance
		        for(j=0; j<bCon.numVert; j++)
		        {
			        float len = Vector2.Dot(bCon.vertList[j], testDir);
			        if(len < rb)
			        {
				        rb = len;
			        }
		        }

		        // opposite direction
		        rb = -rb;

		        if(interval > ra + rb) {
					
			        result = false;
			        if( interval - (ra + rb) > extraction_depth)
			        {
				        extraction_depth = interval - (ra + rb);
				        axisVec = testDir;
			        }
		        }
		        else if(result) {
			        if( (ra + rb - interval) < penetrationDepth)
			        {
				        penetrationDepth = ra + rb - interval;
				        axisVec = testDir;
			        }
		        }
	        }
			
			// Exceptional case for Sphere
	        if((aCon.numVert == 0)&&(bCon.numVert == 0))
	        {
		        ra = aCon.vertList[0].X;
		        rb = bCon.vertList[0].X;
		        interval = (float)Math.Sqrt(Vector2.Dot(b_position - a_position, b_position - a_position));
		        if( interval > (ra + rb))
		        {
			        result = false;
			        extraction_depth = ra + rb - interval;
		        }
		        else
		        {
			        axisVec = (b_position - a_position)/interval;
			        penetrationDepth = ra + rb - interval;
		        }
	        }

	        // there is collision because of separate axis theorem
	        if(result == true)
	        {
		        bodyA.narrowCollide |= true;
		        bodyB.narrowCollide |= true;
	        }
	        else
	        {
		        bodyA.narrowCollide |= false;
		        bodyB.narrowCollide |= false;
		        aabbPairIndex = 0xFFFFFFFF;
		        penetrationDepth = extraction_depth;
		        resultPos.totalIndex = 0xFFFFFFFF;
		        resultPos.penetrationDepth = extraction_depth;
		        return false;
	        }

	        // two points of closest points between Body A and Body B
	        // normal direction should be Body A -> Body B

	        float lenA, lenA2;
	        lenA = lenA2 = -PhysicsUtility.FltMax;
        	
	        float lenB, lenB2;
            lenB = lenB2 = PhysicsUtility.FltMax;

	        int indexA1, indexA2;
	        indexA1 = indexA2 = 0;

	        int indexB1, indexB2;
	        indexB1 = indexB2 = 0;

	        for(i=0; i<aCon.numVert; i++)
	        {
		        Vector2 v = aCon.vertList[i];
		        float len = Vector2.Dot(v, axisVec);

		        if(len > lenA)
		        {
			        indexA2 = indexA1;
			        lenA2 = lenA;

			        indexA1 = i;
			        lenA = len;
		        }
		        else if(len > lenA2)
		        {
			        indexA2 = i;
			        lenA2 = len;
		        }
		        else
		        {
			        // do nothing
		        }
	        }


	        for(i=0; i<bCon.numVert; i++)
	        {
		        Vector2 v = bCon.vertList[i];
		        float len = Vector2.Dot(v, axisVec);

		        if(len < lenB)
		        {
			        indexB2 = indexB1;
			        lenB2 = lenB;

			        indexB1 = i;
			        lenB = len;
		        }
		        else if(len < lenB2)
		        {
			        indexB2 = i;
			        lenB2 = len;
		        }
		        else
		        {
			        // do nothing
		        }
	        }

	        resultPos.totalIndex = aabbPairIndex;
	        resultPos.normal = axisVec;
	        resultPos.penetrationDepth = penetrationDepth;
	        resultPos.PosA = aCon.vertList[indexA1];
	        resultPos.PosA2 = aCon.vertList[indexA2];
	        resultPos.PosB = bCon.vertList[indexB1];
	        resultPos.PosB2 = bCon.vertList[indexB2];


	        // This is for sphere
	        if((aCon.numVert == 0) && (bCon.numVert == 0))
	        {
		        resultPos.PosA =  aCon.vertList[0].X * resultPos.normal;
		        resultPos.PosA2 = -resultPos.PosA;

		        resultPos.PosB =  -bCon.vertList[0].X * resultPos.normal;
		        resultPos.PosB2 = -resultPos.PosB;
	        }
	        else if((aCon.numVert == 0) && (bCon.numVert !=0))
	        {

		        Vector2 temp_PosB = resultPos.PosB + b_position; 
		        Vector2 temp_PosB2 = resultPos.PosB2 + b_position;
		        Vector2 temp_Center = a_position;

		        int result2;

		        if((result2 = DetectSphereLineClosestPoint(temp_PosB, temp_PosB2, temp_Center, Math.Abs(aCon.vertList[0].X))) < 0)
		        {
					// should check internal case
					Vector2 u = temp_PosB2 - temp_PosB;
					Vector2 v = a_position - temp_PosB;
					Vector2 t = b_position - temp_PosB;
					
					// if the direction is same for both
					if (Vector2Util.Cross(u, v) * Vector2Util.Cross(u, t) > 0)
					{
						resultPos.PosA = aCon.vertList[0].X * resultPos.normal;
				        resultPos.PosA2 = -resultPos.PosA;	
					}
					else
					{
				        bodyA.narrowCollide |= false;
				        bodyB.narrowCollide |= false;
				        aabbPairIndex = 0xFFFFFFFF;
				        penetrationDepth = extraction_depth;
				        resultPos.totalIndex = 0xFFFFFFFF;
				        resultPos.penetrationDepth = extraction_depth;
				        return false;
					}
		        }
		        else
		        {

			        if(result2 == 0)
			        {
				        testDir = temp_PosB - a_position;
						float len_testDir = (float)Math.Sqrt(Vector2.Dot(testDir, testDir));
                        testDir /= len_testDir;
				        resultPos.PosA = aCon.vertList[0].X * testDir;
				        resultPos.PosA2 = -resultPos.PosA;
				        resultPos.normal = testDir;
                        resultPos.penetrationDepth = (float)Math.Abs(aCon.vertList[0].X) - len_testDir;
			        }
			        else if(result2 == 1)
			        {
				        testDir = temp_PosB2 - a_position;
						float len_testDir = (float)Math.Sqrt(Vector2.Dot(testDir, testDir));
                        testDir /= len_testDir;
				        resultPos.PosA = aCon.vertList[0].X * testDir;
				        resultPos.PosA2 = -resultPos.PosA;
				        resultPos.normal = testDir;
                        resultPos.penetrationDepth = (float)Math.Abs(aCon.vertList[0].X) - len_testDir;
			        }
			        else
			        {
				        resultPos.PosA = aCon.vertList[0].X * resultPos.normal;
				        resultPos.PosA2 = -resultPos.PosA;
			        }
		        }
	        }

	        else if((bCon.numVert == 0) && (aCon.numVert != 0))
	        {

		        Vector2 temp_PosA = resultPos.PosA + a_position; 
		        Vector2 temp_PosA2 = resultPos.PosA2 + a_position;
		        Vector2 temp_Center = b_position;

		        int result2;

                if ((result2 = DetectSphereLineClosestPoint(temp_PosA, temp_PosA2, temp_Center, Math.Abs(bCon.vertList[0].X))) < 0)
		        {
					// should check internal case
					Vector2 u = temp_PosA2 - temp_PosA;
					Vector2 v = a_position - temp_PosA;
					Vector2 t = b_position - temp_PosA;
					
					// if the direction is same for both
					if (Vector2Util.Cross(u, v) * Vector2Util.Cross(u, t) > 0)
					{
				        resultPos.PosB = -bCon.vertList[0].X * resultPos.normal;
				        resultPos.PosB2 = -resultPos.PosB;	
					}
					else
					{
					    bodyA.narrowCollide |= false;
			        	bodyB.narrowCollide |= false;
			        	aabbPairIndex = 0xFFFFFFFF;
			        	penetrationDepth = extraction_depth;
			        	resultPos.totalIndex = 0xFFFFFFFF;
			        	resultPos.penetrationDepth = extraction_depth;
			        	return false;;
					}
					
	
		        }
		        else
		        {
			        if(result2 == 0)
			        {
				        testDir = temp_PosA - b_position;
						float len_testDir = (float)Math.Sqrt(Vector2.Dot(testDir, testDir));
                        testDir /= len_testDir;
				        resultPos.PosB = bCon.vertList[0].X * testDir;
				        resultPos.PosB2 = -resultPos.PosB;
				        resultPos.normal = -testDir;
                        resultPos.penetrationDepth = (float)Math.Abs(bCon.vertList[0].X) - len_testDir;
			        }
			        else if(result2 == 1)
			        {
				        testDir = temp_PosA2 - b_position;
						float len_testDir = (float)Math.Sqrt(Vector2.Dot(testDir, testDir));
                        testDir /= len_testDir;
				        resultPos.PosB = bCon.vertList[0].X * testDir;
				        resultPos.PosB2 = -resultPos.PosB;
				        resultPos.normal = -testDir;
                        resultPos.penetrationDepth = (float)Math.Abs(bCon.vertList[0].X) - len_testDir;
			        }
			        else
			        {
				        resultPos.PosB = -bCon.vertList[0].X * resultPos.normal;
				        resultPos.PosB2 = -resultPos.PosB;
			        }
		        }
	        }

	        // move position of object along separate axis to avoid penetration
	        resultPos.OldPosA = resultPos.NewPosA = a_position;
	        resultPos.OldPosB = resultPos.NewPosB = b_position;

	        const float epsilon = 0.05f; 

	        if(bodyA.invMass == 0)
	        {
		        resultPos.NewPosB += 2*(0.5f + epsilon) * penetrationDepth * axisVec;
	        }
	        else if(bodyB.invMass == 0)
	        {
		        resultPos.NewPosA -= 2*(0.5f + epsilon) * penetrationDepth * axisVec;
	        }
	        else
	        {
		        resultPos.NewPosA -= (0.5f + epsilon) * penetrationDepth * axisVec;
		        resultPos.NewPosB += (0.5f + epsilon) * penetrationDepth * axisVec;
	        }

	        return true;

        }
		
			
	
		/// @if LANG_EN
		/// <summary> TestAxisCheckOffset </summary>
		/// <remarks> Separate Axis Theorem </remarks>
        /// <param name="bodyA"> one of rigid body pair</param>
        /// <param name="bodyB"> the other of rigid body pair </param>
        /// <param name="shapeA"> shape of one of rigid body pair</param>
        /// <param name="shapeB"> shape of the other of rigid body pair </param>
        /// <param name="sceneShapes"> convex shape list of the scene </param>
        /// <param name="aabbPairIndex"> collision check result </param>
        /// <param name="resultPos"> collision check result </param> 
        /// <returns> return the result whether there is collision by Separate Axis Theorem </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> TestAxisCheckOffset </summary>
		/// <remarks> Separate Axis Theorem判定</remarks>
        /// <param name="bodyA"> 剛体ペアの一方</param>
        /// <param name="bodyB"> 剛体ペアのもう一方 </param>
        /// <param name="shapeA"> 剛体ペアの一方の形状</param>
        /// <param name="shapeB"> 剛体ペアのもう一方の形状 </param>
        /// <param name="sceneShapes"> シーンの形状リスト </param>
        /// <param name="aabbPairIndex"> Broadphase衝突判定の結果 </param>
        /// <param name="resultPos"> 衝突判定の結果 </param> 
        /// <returns> 衝突があったのかSeparate Axis Theorem判定による結果を返します </returns>
        /// @endif
		public static bool TestAxisCheckOffset(PhysicsBody bodyA, PhysicsBody bodyB, PhysicsShape shapeA, PhysicsShape shapeB, PhysicsShape[] sceneShapes, ref uint aabbPairIndex, ref ContactPoint resultPos)
        {
	        int i, j;
			
			//  Convex aCon, bCon;
       	 	//  aCon = new Convex(sceneShapes[a.shapeIndex]);
         	//  bCon = new Convex(sceneShapes[b.shapeIndex]);

			aCon.CopyFromConvex(ref shapeA);
			bCon.CopyFromConvex(ref shapeB);

	        float angle;
	        float ca, sa;

	        Vector2 localPosition;
	        float ca_local;
	        float sa_local;

	        angle = bodyA.rotation;
	        ca = (float)Math.Cos(angle);
            sa = (float)Math.Sin(angle);

	        localPosition = bodyA.localPosition;
            ca_local = (float)Math.Cos(bodyA.localRotation);
            sa_local = (float)Math.Sin(bodyA.localRotation);
        	
	        for(i=0; i<aCon.numVert; i++)
	        {
		        Vector2 v = aCon.vertList[i];
		        v = new Vector2(ca_local*v.X - sa_local*v.Y, sa_local*v.X + ca_local*v.Y);	
		        v += localPosition;
                v = new Vector2(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
		        v -= new Vector2(ca * bodyA.localPosition.X - sa * bodyA.localPosition.Y, sa * bodyA.localPosition.X + ca * bodyA.localPosition.Y);
		        aCon.vertList[i] = v;
	        }

	        Vector2 a_position; 
	        Vector2 b_position; 

	        a_position = bodyA.position + new Vector2(ca * bodyA.localPosition.X - sa * bodyA.localPosition.Y, sa * bodyA.localPosition.X + ca * bodyA.localPosition.Y);

	        angle = bodyB.rotation;
	        ca = (float)Math.Cos(angle);
	        sa = (float)Math.Sin(angle);

	        localPosition = bodyB.localPosition;
            ca_local = (float)Math.Cos(bodyB.localRotation);
            sa_local = (float)Math.Sin(bodyB.localRotation);
        	
	        for(i=0; i<bCon.numVert; i++)
	        {
		        Vector2 v = bCon.vertList[i];
		        v = new Vector2(ca_local*v.X - sa_local*v.Y, sa_local*v.X + ca_local*v.Y);	
		        v += localPosition;
		        v = new Vector2(ca*v.X - sa*v.Y, sa*v.X + ca*v.Y);	
		        v -= new Vector2(ca * bodyB.localPosition.X - sa * bodyB.localPosition.Y, sa * bodyB.localPosition.X + ca * bodyB.localPosition.Y);
		        bCon.vertList[i] = v;
	        }

	        b_position = bodyB.position + new Vector2(ca * bodyB.localPosition.X - sa * bodyB.localPosition.Y, sa * bodyB.localPosition.X + ca * bodyB.localPosition.Y);

			// Direction of Separate Axis
	        Vector2 testDir = new Vector2(0, 0);
            float penetrationDepth = PhysicsUtility.FltMax;
            float extraction_depth = -PhysicsUtility.FltMax;         
            float ra, rb, interval;
			
			// if result = true, it means that there is collision by Separate Axis Theorem
	        bool result = true;
			Vector2 axisVec = new Vector2(0, 0);
			
			// For bodyA
	        for(i=0; i<(aCon.numVert/aCon.hint); i++)
	        {
		        testDir = aCon.vertList[(i+1)%aCon.numVert] - aCon.vertList[i];
		        testDir /= (float)Math.Sqrt(Vector2.Dot(testDir, testDir));

				// there is difference of 90.0f degree between direction of edge and separate axis
		        if((aCon.numVert == 2) && (i==1))
		        {
			        continue;
		        }
		        else
		        {
                    testDir = Vector2Util.Cross(testDir, 1.0f);
		        }

		        // testDir should have the similar direction as well as b_position - a_position
		        interval = Vector2.Dot(b_position  - a_position, testDir);

		        if( interval < 0 )
		        {
			        testDir = -testDir;
			        interval = -interval;
		        }
		        else
		        {
			        // do nothing
		        }

 
		        ra = 0;
		        rb = 0;
				
				// biggest distance for bodyA
		        for(j=0; j<aCon.numVert; j++)
		        {
			        float len = Vector2.Dot(aCon.vertList[j], testDir);
			        if(len > ra)
			        {
				        ra = len;
			        }
		        }

		        // smallest (negative biggest) distance for bodyB
		        if(bCon.numVert != 0)
				{
					for(j=0; j<bCon.numVert; j++)
		        	{
			        	float len = Vector2.Dot(bCon.vertList[j], testDir);
			        	if(len < rb)
			        	{
				       	 rb = len;
			        	}
		        	}
				}
				else
				{
					rb = -bCon.vertList[0].X;
				}

		        // direction is returned to oposite direction
		        rb = -rb;

		        if(interval > ra + rb) {
			        result = false;
			        if( interval - (ra + rb) > extraction_depth)
			        {
				        extraction_depth = interval - (ra + rb);
				       	axisVec = testDir;;
			        }
	
				}
		        else if(result) {
			        if( (ra + rb - interval) < penetrationDepth)
			        {
				        penetrationDepth = ra + rb - interval;
				        axisVec = testDir;
			        }
		        }
	        }


			// For body B
	        for(i=0; i<(bCon.numVert/bCon.hint); i++)
	        {
		        testDir = bCon.vertList[(i+1)%bCon.numVert] - bCon.vertList[i];
		        testDir /= (float)Math.Sqrt(Vector2.Dot(testDir, testDir));

				// there is difference of 90 degree between direction and separate axis
		        if((bCon.numVert == 2) && (i==1))
		        {
			        continue;
		        }
		        else
		        {
                    testDir = Vector2Util.Cross(testDir, 1.0f);
		        }
        			
		        interval = Vector2.Dot(b_position - a_position, testDir);

		        if( interval < 0 )
		        {
			        testDir = -testDir;
			        interval = -interval;
		        }
		        else
		        {
			        // do nothing
		        }

		       
		        ra = 0;
		        rb = 0;
				
				// biggest distance for bodyA
				if(aCon.numVert != 0)
				{
			        for(j=0; j<aCon.numVert; j++)
			        {
				        float len = Vector2.Dot(aCon.vertList[j], testDir);
				        if(len > ra)
				        {
					        ra = len;
				        }
			        }
				}
				else
				{
					ra = aCon.vertList[0].X;	
				}

		        // lonest distance
		        for(j=0; j<bCon.numVert; j++)
		        {
			        float len = Vector2.Dot(bCon.vertList[j], testDir);
			        if(len < rb)
			        {
				        rb = len;
			        }
		        }

		        // opposite direction
		        rb = -rb;

		        if(interval > ra + rb) {
					
			        result = false;
			        if( interval - (ra + rb) > extraction_depth)
			        {
				        extraction_depth = interval - (ra + rb);
				        axisVec = testDir;
			        }
		        }
		        else if(result) {
			        if( (ra + rb - interval) < penetrationDepth)
			        {
				        penetrationDepth = ra + rb - interval;
				        axisVec = testDir;
			        }
		        }
	        }
			
			// Exceptional case for Sphere
	        if((aCon.numVert == 0)&&(bCon.numVert == 0))
	        {
		        ra = aCon.vertList[0].X;
		        rb = bCon.vertList[0].X;
		        interval = (float)Math.Sqrt(Vector2.Dot(b_position - a_position, b_position - a_position));
		        if( interval > (ra + rb))
		        {
			        result = false;
			        extraction_depth = ra + rb - interval;
		        }
		        else
		        {
			        axisVec = (b_position - a_position)/interval;
			        penetrationDepth = ra + rb - interval;
		        }
	        }

	        // there is collision because of separate axis theorem
	        if(result == true)
	        {
		        bodyA.narrowCollide |= true;
		        bodyB.narrowCollide |= true;
	        }
	        else
	        {
		        bodyA.narrowCollide |= false;
		        bodyB.narrowCollide |= false;
		        //aabbPairIndex = 0xFFFFFFFF;
		        penetrationDepth = extraction_depth;
		        resultPos.totalIndex = 0xFFFFFFFF;
		        resultPos.penetrationDepth = extraction_depth;
		        return false;
	        }

	        // two points of closest points between Body A and Body B
	        // normal direction should be Body A -> Body B

	        float lenA, lenA2;
	        lenA = lenA2 = -PhysicsUtility.FltMax;
        	
	        float lenB, lenB2;
            lenB = lenB2 = PhysicsUtility.FltMax;

	        int indexA1, indexA2;
	        indexA1 = indexA2 = 0;

	        int indexB1, indexB2;
	        indexB1 = indexB2 = 0;

	        for(i=0; i<aCon.numVert; i++)
	        {
		        Vector2 v = aCon.vertList[i];
		        float len = Vector2.Dot(v, axisVec);

		        if(len > lenA)
		        {
			        indexA2 = indexA1;
			        lenA2 = lenA;

			        indexA1 = i;
			        lenA = len;
		        }
		        else if(len > lenA2)
		        {
			        indexA2 = i;
			        lenA2 = len;
		        }
		        else
		        {
			        // do nothing
		        }
	        }


	        for(i=0; i<bCon.numVert; i++)
	        {
		        Vector2 v = bCon.vertList[i];
		        float len = Vector2.Dot(v, axisVec);

		        if(len < lenB)
		        {
			        indexB2 = indexB1;
			        lenB2 = lenB;

			        indexB1 = i;
			        lenB = len;
		        }
		        else if(len < lenB2)
		        {
			        indexB2 = i;
			        lenB2 = len;
		        }
		        else
		        {
			        // do nothing
		        }
	        }

	        resultPos.totalIndex = aabbPairIndex;
	        resultPos.normal = axisVec;
	        resultPos.penetrationDepth = penetrationDepth;
	        resultPos.PosA = aCon.vertList[indexA1];
	        resultPos.PosA2 = aCon.vertList[indexA2];
	        resultPos.PosB = bCon.vertList[indexB1];
	        resultPos.PosB2 = bCon.vertList[indexB2];


	        // This is for sphere
	        if((aCon.numVert == 0) && (bCon.numVert == 0))
	        {
		        resultPos.PosA =  aCon.vertList[0].X * resultPos.normal;
		        resultPos.PosA2 = -resultPos.PosA;

		        resultPos.PosB =  -bCon.vertList[0].X * resultPos.normal;
		        resultPos.PosB2 = -resultPos.PosB;
	        }
	        else if((aCon.numVert == 0) && (bCon.numVert !=0))
	        {

		        Vector2 temp_PosB = resultPos.PosB + b_position; 
		        Vector2 temp_PosB2 = resultPos.PosB2 + b_position;
		        Vector2 temp_Center = a_position;

		        int result2;

		        if((result2 = DetectSphereLineClosestPoint(temp_PosB, temp_PosB2, temp_Center, Math.Abs(aCon.vertList[0].X))) < 0)
		        {
					// should check internal case
					Vector2 u = temp_PosB2 - temp_PosB;
					Vector2 v = a_position - temp_PosB;
					Vector2 t = b_position - temp_PosB;
					
					// if the direction is same for both
					if (Vector2Util.Cross(u, v) * Vector2Util.Cross(u, t) > 0.05f)
					{
						resultPos.PosA = aCon.vertList[0].X * resultPos.normal;
				        resultPos.PosA2 = -resultPos.PosA;	
					}
					else
					{
				        bodyA.narrowCollide |= false;
				        bodyB.narrowCollide |= false;
				        //aabbPairIndex = 0xFFFFFFFF;
				        penetrationDepth = extraction_depth;
				        resultPos.totalIndex = 0xFFFFFFFF;
				        resultPos.penetrationDepth = extraction_depth;
				        return false;
					}
		        }
		        else
		        {

			        if(result2 == 0)
			        {
				        testDir = temp_PosB - a_position;
						float len_testDir = (float)Math.Sqrt(Vector2.Dot(testDir, testDir));
                        testDir /= len_testDir;
				        resultPos.PosA = aCon.vertList[0].X * testDir;
				        resultPos.PosA2 = -resultPos.PosA;
				        resultPos.normal = testDir;
                        resultPos.penetrationDepth = (float)Math.Abs(aCon.vertList[0].X) - len_testDir;
			        }
			        else if(result2 == 1)
			        {
				        testDir = temp_PosB2 - a_position;
						float len_testDir = (float)Math.Sqrt(Vector2.Dot(testDir, testDir));
                        testDir /= len_testDir;
				        resultPos.PosA = aCon.vertList[0].X * testDir;
				        resultPos.PosA2 = -resultPos.PosA;
				        resultPos.normal = testDir;
                        resultPos.penetrationDepth = (float)Math.Abs(aCon.vertList[0].X) - len_testDir;
			        }
			        else
			        {
				        resultPos.PosA = aCon.vertList[0].X * resultPos.normal;
				        resultPos.PosA2 = -resultPos.PosA;
			        }
		        }
	        }

	        else if((bCon.numVert == 0) && (aCon.numVert != 0))
	        {

		        Vector2 temp_PosA = resultPos.PosA + a_position; 
		        Vector2 temp_PosA2 = resultPos.PosA2 + a_position;
		        Vector2 temp_Center = b_position;

		        int result2;

                if ((result2 = DetectSphereLineClosestPoint(temp_PosA, temp_PosA2, temp_Center, Math.Abs(bCon.vertList[0].X))) < 0)
		        {
					// should check internal case
					Vector2 u = temp_PosA2 - temp_PosA;
					Vector2 v = a_position - temp_PosA;
					Vector2 t = b_position - temp_PosA;
					
					// if the direction is same for both
					if (Vector2Util.Cross(u, v) * Vector2Util.Cross(u, t) > 0.05f)
					{
				        resultPos.PosB = -bCon.vertList[0].X * resultPos.normal;
				        resultPos.PosB2 = -resultPos.PosB;	
					}
					else
					{
					    bodyA.narrowCollide |= false;
			        	bodyB.narrowCollide |= false;
			        	//aabbPairIndex = 0xFFFFFFFF;
			        	penetrationDepth = extraction_depth;
			        	resultPos.totalIndex = 0xFFFFFFFF;
			        	resultPos.penetrationDepth = extraction_depth;
			        	return false;;
					}
					
	
		        }
		        else
		        {
			        if(result2 == 0)
			        {
				        testDir = temp_PosA - b_position;
						float len_testDir = (float)Math.Sqrt(Vector2.Dot(testDir, testDir));
                        testDir /= len_testDir;
				        resultPos.PosB = bCon.vertList[0].X * testDir;
				        resultPos.PosB2 = -resultPos.PosB;
				        resultPos.normal = -testDir;
                        resultPos.penetrationDepth = (float)Math.Abs(bCon.vertList[0].X) - len_testDir;
			        }
			        else if(result2 == 1)
			        {
				        testDir = temp_PosA2 - b_position;
						float len_testDir = (float)Math.Sqrt(Vector2.Dot(testDir, testDir));
                        testDir /= len_testDir;
				        resultPos.PosB = bCon.vertList[0].X * testDir;
				        resultPos.PosB2 = -resultPos.PosB;
				        resultPos.normal = -testDir;
                        resultPos.penetrationDepth = (float)Math.Abs(bCon.vertList[0].X) - len_testDir;
			        }
			        else
			        {
				        resultPos.PosB = -bCon.vertList[0].X * resultPos.normal;
				        resultPos.PosB2 = -resultPos.PosB;
			        }
		        }
	        }

	        // move position of object along separate axis to avoid penetration
	        resultPos.OldPosA = resultPos.NewPosA = a_position;
	        resultPos.OldPosB = resultPos.NewPosB = b_position;

	        const float epsilon = 0.05f; 

	        if(bodyA.invMass == 0)
	        {
		        resultPos.NewPosB += 2*(0.5f + epsilon) * penetrationDepth * axisVec;
	        }
	        else if(bodyB.invMass == 0)
	        {
		        resultPos.NewPosA -= 2*(0.5f + epsilon) * penetrationDepth * axisVec;
	        }
	        else
	        {
		        resultPos.NewPosA -= (0.5f + epsilon) * penetrationDepth * axisVec;
		        resultPos.NewPosB += (0.5f + epsilon) * penetrationDepth * axisVec;
	        }

	        return true;

        }

			
		/// @if LANG_EN
		/// <summary> TestAxisCheckRaycast </summary>
        /// <remarks> Raycast Check </remarks>
        /// <param name="bodyA"> one of rigid body pair</param>
        /// <param name="shapeA"> shape of one of rigid body pair</param>
        /// <param name="PosB"> convex shape list of the scene </param>
        /// <param name="PosB2"> collision check result </param>
       	/// <returns> return the point of line-segment as ratio </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> TestAxisCheckRaycast </summary>
        /// <remarks> レイキャストチェック </remarks>
        /// <param name="bodyA"> 剛体ペアの一方</param>
        /// <param name="shapeA"> 剛体ペアのもう一方 </param>
        /// <param name="PosB"> 剛体ペアの一方の形状</param>
        /// <param name="PosB2"> 剛体ペアのもう一方の形状 </param>
       	/// <returns> 交差している線分の位置を割合で返す </returns>
        /// @endif
		public static float TestAxisCheckRaycast(PhysicsBody bodyA, PhysicsShape shapeA, Vector2 PosB, Vector2 PosB2)
        {
	        int i;

			aCon.CopyFromConvex(ref shapeA);

	        float angle;
	        float ca, sa;

	        Vector2 localPosition;
	        float ca_local;
	        float sa_local;

	        angle = bodyA.rotation;
	        ca = (float)Math.Cos(angle);
            sa = (float)Math.Sin(angle);

	        localPosition = bodyA.localPosition;
            ca_local = (float)Math.Cos(bodyA.localRotation);
            sa_local = (float)Math.Sin(bodyA.localRotation);
        	
	        for(i=0; i<aCon.numVert; i++)
	        {
		        Vector2 v = aCon.vertList[i];
		        v = new Vector2(ca_local*v.X - sa_local*v.Y, sa_local*v.X + ca_local*v.Y);	
		        v += localPosition;
                v = new Vector2(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
		        v -= new Vector2(ca * bodyA.localPosition.X - sa * bodyA.localPosition.Y, sa * bodyA.localPosition.X + ca * bodyA.localPosition.Y);
		        aCon.vertList[i] = v;
	        }

	        Vector2 a_position; 

	        a_position = bodyA.position + new Vector2(ca * bodyA.localPosition.X - sa * bodyA.localPosition.Y, sa * bodyA.localPosition.X + ca * bodyA.localPosition.Y);

			if(aCon.numVert == 0)
			{				
		        float ratio = DetectSphereLineCrossPoint(PosB, PosB2, a_position, Math.Abs(aCon.vertList[0].X));
				return ratio;
			}
			else
			{
				float ratio = PhysicsUtility.FltMax;
					
				for(i=0; i<aCon.numVert; i++)
				{
					if((shapeA.hint == 0) && (i == (aCon.numVert -1)))
						   break;
						   
					float ratio2 = DetectLineLineCrossPoint(PosB, PosB2, aCon.vertList[i] + a_position, aCon.vertList[(i+1)%aCon.numVert] + a_position);                     	
						
					if((ratio2 < 0.0f)||(ratio2 > 1.0f))
							continue;
						
					ratio = Math.Min (ratio, ratio2);
						
				}
					
				return ratio;   	
			}
				
				

        }	
		#endregion
    }	
	}
}
