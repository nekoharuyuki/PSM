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

    public partial class PhysicsScene
    {	
	
		// Island Generation Map
		private uint[] IslandMap = new uint[maxBody];
		private uint[] IslandMap_Rank = new uint[maxBody];

		/// <summary> CheckIslandSleep </summary>
		/// <remarks> internal use only, if outside of the main area, 
		/// object will not be considered to take care </remarks>
        private void CheckIslandSleep()
        {

            // check the sleep of Island
            for (uint i = 0; i < numBody; i++)
            {
                // non-dynamic actor
				// if it is trigger type, it is set as sleep true for temporal
                if ((sceneBodies[i].invMass == 0) || (sceneBodies[i].type == BodyType.Trigger))
                {
                    sceneBodies[i].sleep = true;
                }
                // scene bound check
                else if (CheckOutOfBound(sceneBodies[i]))
                {
                    //
                    // no way to activate this again
                    //					
                    sceneBodies[i].sleep = true;
                }
                else
                {
                    if ((sceneBodies[i].velocity.Length() <= LinearSleepVelocity)
                    && (System.Math.Abs(sceneBodies[i].angularVelocity) <= AngularSleepVelocity))
                    {
                        sceneBodies[i].sleepCount++;
                        if (sceneBodies[i].sleepCount >= 180)
                            sceneBodies[i].sleep = true;
                    }
                    else
                    {
                        sceneBodies[i].sleepCount = 0;
                        sceneBodies[i].sleep = false;
                    }
                }
            }

            // sleep status -> root node
            for (int i = 0; i < numBody; i++)
            {
                sceneBodies[IslandMap[i]].sleep &= sceneBodies[i].sleep;
            }

            // root node -> sleep status
            for (int i = 0; i < numBody; i++)
            {
                sceneBodies[i].sleep = sceneBodies[IslandMap[i]].sleep;
            }

            for (int i = 0; i < numBody; i++)
            {
                if (sceneBodies[i].sleep == true)
                {
                    sceneBodies[i].velocity = new Vector2(0, 0);
                    sceneBodies[i].angularVelocity = 0.0f;
                    sceneBodies[i].sleepCount = 180;
                }
                else
                {
                    // do nothing here
                }
            }
			
			// re-wakeup trigger object for collision detection
			// and cancel all speed.
			for (int i=0; i < numBody; i++)
			{
				if(sceneBodies[i].type == BodyType.Trigger)
				{
					sceneBodies[i].sleep = false;
					sceneBodies[i].velocity = new Vector2(0.0f);
					sceneBodies[i].angularVelocity = 0.0f;
				}
			}
        }
	

		private uint Find(uint x)
        {
            if (IslandMap[x] == x)
            {
                return x;
            }
            else
            {
                IslandMap[x] = Find(IslandMap[x]);
                return IslandMap[x];
            }
        }

        private bool IsSame(uint x, uint y)
        {
            return Find(x) == Find(y);
        }

        private void Union(uint x, uint y)
        {
            x = Find(x);
            y = Find(y);

            if (IslandMap_Rank[x] > IslandMap_Rank[y])
            {
                IslandMap[y] = IslandMap[x];
            }
            else if (IslandMap_Rank[x] < IslandMap_Rank[y])
            {
                IslandMap[x] = IslandMap[y];
            }
            else if (IslandMap[x] != IslandMap[y])
            {
                IslandMap[y] = IslandMap[x];
                IslandMap_Rank[x]++;
            }
        }

        private void GenerateIsland()
        {
            // re-initialize Island
            for (uint i = 0; i < numBody; i++)
            {
                IslandMap[i] = i;
                IslandMap_Rank[i] = 0;
            }
			
			// Island for Compound
			for(uint i=0; i<numBody; i++)
			{
				if(compoundMap[i] != i)
					Union(compoundMap[i], i);
			}
			
            // Island for Spring
            for (uint i = 0; i < numSpring; i++)
            {
                uint index1 = PhysicsUtility.UnpackIdx1(sceneSprings[i].totalIndex);
                uint index2 = PhysicsUtility.UnpackIdx2(sceneSprings[i].totalIndex);

                if ((sceneBodies[index1].invMass == 0) || (sceneBodies[index2].invMass == 0))
                    continue;
				
				if (CheckOutOfBound(sceneBodies[index1])||CheckOutOfBound(sceneBodies[index2]))
				    continue;
				
                Union(index1, index2);
            }
			
            // Island for Joint
            for (uint i = 0; i < numJoint; i++)
            {
                uint index1 = PhysicsUtility.UnpackIdx1(sceneJoints[i].totalIndex);
                uint index2 = PhysicsUtility.UnpackIdx2(sceneJoints[i].totalIndex);

                if ((sceneBodies[index1].invMass == 0) || (sceneBodies[index2].invMass == 0))
                    continue;
				
				if (CheckOutOfBound(sceneBodies[index1])||CheckOutOfBound(sceneBodies[index2]))
				    continue;
				
                Union(index1, index2);
            }

            // Island for Contact
			
			if(sleepAlgorithm == SleepAlgorithmType.BroadphaseBase)		
			{
	            for (uint i = 0; i < numBroadPair; i++)
	            {
	                uint index1 = PhysicsUtility.UnpackIdx1(broadPair[i]);
	                uint index2 = PhysicsUtility.UnpackIdx2(broadPair[i]);
	
	                if ((sceneBodies[index1].invMass == 0) || (sceneBodies[index2].invMass == 0))
	                    continue;
					
					if (CheckOutOfBound(sceneBodies[index1])||CheckOutOfBound(sceneBodies[index2]))
					    continue;
					
	                Union(index1, index2);
	            }
			}
			else
			{
	            for (uint i = 0; i < numPhysicsSolverPair; i++)
	            {
	                uint index1 = PhysicsUtility.UnpackIdx1(solverPair[i].totalIndex);
	                uint index2 = PhysicsUtility.UnpackIdx2(solverPair[i].totalIndex);
	
	                if ((sceneBodies[index1].invMass == 0) || (sceneBodies[index2].invMass == 0))
	                    continue;
					
					if (CheckOutOfBound(sceneBodies[index1])||CheckOutOfBound(sceneBodies[index2]))
					    continue;
					
	                Union(index1, index2);
	            }	
			}
			
			for(uint i=0; i<numBody; i++)
			{
				IslandMap[i] = Find (i);
			}
		}
    }
	
	
}
