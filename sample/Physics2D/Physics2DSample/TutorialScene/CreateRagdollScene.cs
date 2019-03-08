/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


//
// Basic ragdoll creation
//


// [How to set up the simulation scene]
//
// (1) Make an simulation scene
// (2) Arrange coefficients for contact solver or gravity etc...
// (3) Prepare collision shapes for rigid sceneBodies
// (4) Make the rigid body instances by specifying collision shapes and masses 
// (5) Set the position, rotation, velocity and angular velocity based on the scene
//
// Recommendation of scales for dynamic rigid body
//	
// mass : 0.2[Kg] - 20.0[Kg] for dynamic rigid body
// length : 0.2[m] - 20.0[m] for dynamic rigid body
//
// 1.0[Kg] x 1[m] is a typical mass & length of rigid body for dynamic rigid body 
//
// Please notice that the object has infinite mass while user is picking it 
//
//


// [Tips and notice]
//
// This sample shows the way to create very simple ragdoll style object.
//
// Ragdoll is composed of many primitives. (boxes and spheres)
//
// You can determine which parts has collision detection inside one ragdoll by AddIgnorePair(...).
// This is more convenient to construct the ragdoll compared with usual collision filter or group filter.
//
// But should be care about the fact that a lot of ignorePair makes the performance of broadphase become worse.
// And once that the interpenetration happens, then it is difficult for the ragdoll to restore the original relative position
// if there are a lot of ignorePair inside the ragdoll.
// (interpenetration free means that it is also easy for the ragdoll to restore the original position
// because there is no collision detection and it is free for any parts of the ragdoll to go forward and go back.)
//

// use direct access to structure of simulation data to set up initial scene
#define USE_OLD_API

using System;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;

// Include 2D Physics Framework
using Sce.PlayStation.HighLevel.Physics2D;

namespace Physics2DSample
{
    // To create a simulation scene, please inherit from PhysicsScene class
    public class CreateRagdollScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // Vertex Buffer for Debug Rendering
        private VertexBuffer colVert = null;
		
		private int[] ragdoll_start_index = new int[3];
		private int[] ragdoll_end_index = new int[3];
		
        public CreateRagdollScene()
        {
            // Simulation Scene Set Up
            InitScene();

            // Setup for Rendering Object
            for (int i = 0; i < numShape; i++)
            {
                if (sceneShapes[i].numVert == 0)
                {
                    vertices[i] = new VertexBuffer(37, VertexFormat.Float3);
                }
                else
                {
                    vertices[i] = new VertexBuffer(sceneShapes[i].numVert + 1, VertexFormat.Float3);
                }

                MakeLineListConvex(sceneShapes[i], vertices[i]);
            }

            // VertexBuffer for contact points debug rendering
            {
                colVert = new VertexBuffer(4, VertexFormat.Float3);

                const float scale = 0.2f;

                float[] vertex = new float[]
                {
                    -1.0f, -1.0f, 0.0f,
                    1.0f, -1.0f, 0.0f,
                    1.0f, 1.0f, 0.0f,
                    -1.0f, 1.0f, 0.0f
                };

                for (int i = 0; i < 12; i++)
                    vertex[i] = vertex[i] * scale;

                colVert.SetVertices(0, vertex);
            }
        }

        ~CreateRagdollScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {
            for (int i = 0; i < numShape; i++)
                if(vertices[i] != null)
                    vertices[i].Dispose();

            if(colVert != null) colVert.Dispose();
        }
		
#if USE_OLD_API
		
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "CreateRagdollScene";
			
			// Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);
			
			Vector2 box_width = new Vector2(3.0f, 3.0f);
            sceneShapes[2] = new PhysicsShape(box_width);
	
			float sphere_width = 3.0f;
            sceneShapes[3] = new PhysicsShape(sphere_width);


			// body center
			Vector2 body_width = new Vector2(2.0f, 2.5f);
			sceneShapes[4] = new PhysicsShape(body_width);
			
			// head
			float head_width = 1.5f;
			sceneShapes[5] = new PhysicsShape(head_width);
			
			// arm 1, 2
			Vector2 arm_width = new Vector2(1.5f, 0.75f);
			sceneShapes[6] = new PhysicsShape(arm_width);

			// hand
			float hand_width = 0.75f;
			sceneShapes[7] = new PhysicsShape(hand_width);

			// leg 1, 2
			Vector2 leg_width = new Vector2(0.75f, 2.0f);
			sceneShapes[8] = new PhysicsShape(leg_width);		
			
			numShape = 9;
			
		
            // Create static walls to limit the range of action of active rigid sceneBodies
            {
                // new PhysicsBody( "shape of the body",  "mass of the body(kg)" ) 
                sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);

                // Set the position & the rotation
                sceneBodies[numBody].position = new Vector2(0, -wall_height.Y);
                sceneBodies[numBody].rotation = 0;

                // Make shapeIndex consistent with what we set as convex shape
                sceneBodies[numBody].shapeIndex = 0;
                numBody++;

                sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(wall_width.X, 0);
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 1;
                numBody++;

                sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(-wall_width.X, 0);
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 1;
                numBody++;

                sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(0, wall_height.Y);
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 0;
                numBody++;
            }
			
            // Create the ragdoll
			// Ragdoll is composed with many primitives and joints
			for(int i=0; i<3; i++)
            {
				int body_index, head_index;
				int larm1_index, larm2_index, lhand_index;
				int rarm1_index, rarm2_index, rhand_index;
				int lleg1_index, lleg2_index;
				int rleg1_index, rleg2_index;
				PhysicsBody bodyA, bodyB;
				
				ragdoll_start_index[i] = numBody;
				
				// create body
				Vector2 center_pos = new Vector2(-10.0f + 20.0f * i, 5.0f);
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 5.0f);
                sceneBodies[numBody].position = center_pos;
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 4;
                body_index = numBody++;
				
				// create head
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 1.0f);
                sceneBodies[numBody].position = center_pos + new Vector2(0.0f, body_width.Y) + new Vector2(0, head_width);
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 5;				
                head_index = numBody++;				
				
				// creat joint between body and head
				bodyA = sceneBodies[body_index];
				bodyB = sceneBodies[head_index];
				sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, bodyB.position + new Vector2(0.0f, -head_width), (uint)body_index, (uint)head_index);
				sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
				sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
				sceneJoints[numJoint].angleLim = 1;
				sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-10.0f);
				sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(10.0f);
				numJoint++;
				
				
				// Left Upper Part
				{
					// create left arm1					
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 1.0f);
	                sceneBodies[numBody].position = center_pos + new Vector2(-body_width.X, 0.5f*body_width.Y) + new Vector2(-arm_width.X, 0.0f);
	                sceneBodies[numBody].rotation = 0;
	                sceneBodies[numBody].shapeIndex = 6;
	                larm1_index = numBody++;		
				
					// creat joint between body and left arm1				
					bodyA = sceneBodies[body_index];
					bodyB = sceneBodies[larm1_index];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, bodyB.position+ new Vector2(+arm_width.X, 0.0f), (uint)body_index, (uint)larm1_index);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-45.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(45.0f);
					numJoint++;
					
					// create left arm2	
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 1.0f);
	                sceneBodies[numBody].position = center_pos + new Vector2(-body_width.X, 0.5f*body_width.Y) + new Vector2(-arm_width.X, 0.0f) + new Vector2(-2.0f*arm_width.X, 0.0f);
	                sceneBodies[numBody].rotation = 0;
	                sceneBodies[numBody].shapeIndex = 6;
	                larm2_index = numBody++;	
			
					// creat joint between left arm1 and left arm2				
					bodyA = sceneBodies[larm1_index];
					bodyB = sceneBodies[larm2_index];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, 0.5f*(bodyA.position + bodyB.position), (uint)larm1_index, (uint)larm2_index);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-45.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(45.0f);
					numJoint++;
					
					// create left hand
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], 1.0f);
	                sceneBodies[numBody].position = center_pos + new Vector2(-body_width.X, 0.5f*body_width.Y) + new Vector2(-4.0f*arm_width.X, 0.0f) + new Vector2(-hand_width, 0.0f);
	                sceneBodies[numBody].rotation = 0;
	                sceneBodies[numBody].shapeIndex = 7;					
	                lhand_index = numBody++;
					
					// creat joint between left arm2 and left hand				
					bodyA = sceneBodies[larm2_index];
					bodyB = sceneBodies[lhand_index];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, bodyB.position + new Vector2(hand_width, 0), (uint)larm2_index, (uint)lhand_index);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-10.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(10.0f);
					numJoint++;		
				}
				
			
				// Right Upper Part				
				{
					// create right arm1
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 1.0f);				
	                sceneBodies[numBody].position = center_pos + new Vector2(+body_width.X, 0.5f*body_width.Y) + new Vector2(+arm_width.X, 0.0f);
	                sceneBodies[numBody].rotation = 0;
	                sceneBodies[numBody].shapeIndex = 6;					
					rarm1_index = numBody++;		
				
					// creat joint between body and right arm1				
					bodyA = sceneBodies[body_index];
					bodyB = sceneBodies[rarm1_index];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, bodyB.position+ new Vector2(-arm_width.X, 0.0f), (uint)body_index, (uint)rarm1_index);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-45.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(45.0f);
					numJoint++;
						
					// create right arm2	
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 1.0f);
	                sceneBodies[numBody].position = center_pos + new Vector2(+body_width.X, 0.5f*body_width.Y) + new Vector2(+arm_width.X, 0.0f) + new Vector2(+2.0f*arm_width.X, 0.0f);
	                sceneBodies[numBody].rotation = 0;
	                sceneBodies[numBody].shapeIndex = 6;					
	                rarm2_index = numBody++;	
			
					// creat joint between right arm1 and right arm2				
					bodyA = sceneBodies[rarm1_index];
					bodyB = sceneBodies[rarm2_index];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, 0.5f*(bodyA.position + bodyB.position), (uint)rarm1_index, (uint)rarm2_index);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-45.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(45.0f);
					numJoint++;
					
					// create right hand	
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], 1.0f);
	                sceneBodies[numBody].position = center_pos + new Vector2(+body_width.X, 0.5f*body_width.Y) + new Vector2(+4.0f*arm_width.X, 0.0f) + new Vector2(+hand_width, 0.0f);
	                sceneBodies[numBody].rotation = 0;
	                sceneBodies[numBody].shapeIndex = 7;					
	                rhand_index = numBody++;
					
					// creat joint between right arm2 and right hand				
					bodyA = sceneBodies[rarm2_index];
					bodyB = sceneBodies[rhand_index];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, bodyB.position + new Vector2(hand_width, 0), (uint)rarm2_index, (uint)rhand_index);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-10.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(10.0f);
					numJoint++;		
				}	
				
				
				// Left Lower Part
				{
					// create left leg1					
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[8], 1.0f);
	                sceneBodies[numBody].position = center_pos + new Vector2(-0.5f*body_width.X, -body_width.Y) + new Vector2(0.0f, -leg_width.Y);
	                sceneBodies[numBody].rotation = 0;
	                sceneBodies[numBody].shapeIndex = 8;						
	                lleg1_index = numBody++;		
				
					// creat joint between body and left leg1				
					bodyA = sceneBodies[body_index];
					bodyB = sceneBodies[lleg1_index];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, bodyB.position + new Vector2(0.0f, +leg_width.Y), (uint)body_index, (uint)lleg1_index);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-45.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(10.0f);
					numJoint++;
						
					// create left leg2	
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[8], 1.0f);
	                sceneBodies[numBody].position = center_pos + new Vector2(-0.5f*body_width.X, -body_width.Y) + new Vector2(0.0f, -3.0f*leg_width.Y);
	                sceneBodies[numBody].rotation = 0;
	                sceneBodies[numBody].shapeIndex = 8;					
	                lleg2_index = numBody++;	
			
					// creat joint between left leg1 and left leg2				
					bodyA = sceneBodies[lleg1_index];
					bodyB = sceneBodies[lleg2_index];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, 0.5f*(bodyA.position + bodyB.position), (uint)lleg1_index, (uint)lleg2_index);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-45.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(10.0f);
					numJoint++;
				}
				
				
				// Right Lower Part
				{
					// create left leg1					
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[8], 1.0f);
	                sceneBodies[numBody].position = center_pos + new Vector2(+0.5f*body_width.X, -body_width.Y) + new Vector2(0.0f, -leg_width.Y);
	                sceneBodies[numBody].rotation = 0;
	                sceneBodies[numBody].shapeIndex = 8;			
	                rleg1_index = numBody++;		
				
					// creat joint between body and left leg1				
					bodyA = sceneBodies[body_index];
					bodyB = sceneBodies[rleg1_index];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, bodyB.position + new Vector2(0.0f, +leg_width.Y), (uint)body_index, (uint)rleg1_index);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-45.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(10.0f);
					numJoint++;
						
					// create left leg2	
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[8], 1.0f);
	                sceneBodies[numBody].position = center_pos + new Vector2(+0.5f*body_width.X, -body_width.Y) + new Vector2(0.0f, -3.0f*leg_width.Y);
	                sceneBodies[numBody].rotation = 0;
	                sceneBodies[numBody].shapeIndex = 8;					
	                rleg2_index = numBody++;	
			
					// creat joint between left leg1 and left leg2				
					bodyA = sceneBodies[rleg1_index];
					bodyB = sceneBodies[rleg2_index];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, 0.5f*(bodyA.position + bodyB.position), (uint)rleg1_index, (uint)rleg2_index);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-45.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(10.0f);
					numJoint++;
				}				
				
				
				// Collision model are different among three ragdolls
				if(i==0)
				{
					// Ignore only those collisions
					// In the case that very strong force is applied
					// (touch pad drag is one of strong forces becase making it move forcely) to the leg, 
					// then it can cause the interpenetration between left leg & right leg.
					// Once the interpenetration happens, 
					// it is difficult for those parts to restore original relative position.
					
					AddIgnorePair((uint)body_index, (uint)head_index);
					
					AddIgnorePair((uint)body_index, (uint)rarm1_index);
					AddIgnorePair((uint)rarm1_index, (uint)rarm2_index);
					AddIgnorePair((uint)rarm2_index, (uint)rhand_index);
					
					AddIgnorePair((uint)body_index, (uint)larm1_index);
					AddIgnorePair((uint)larm1_index, (uint)larm2_index);
					AddIgnorePair((uint)larm2_index, (uint)lhand_index);
					
					AddIgnorePair((uint)body_index, (uint)rleg1_index);
					AddIgnorePair((uint)rleg1_index, (uint)rleg2_index);
					
					AddIgnorePair((uint)body_index, (uint)lleg1_index);
					AddIgnorePair((uint)lleg1_index, (uint)lleg2_index);
				}
				else if(i==1) // This is a recommendation for computational cost and stability
				{
					// Cancel all collisions inside the ragdoll by using same collision filter.
					// Compared with the above case(i==0),
					// this case is relatively stable because collision free model is applied to this ragdoll.
					sceneBodies[body_index].collisionFilter = 1;
					sceneBodies[head_index].collisionFilter = 1;
					sceneBodies[larm1_index].collisionFilter = 1;
					sceneBodies[larm2_index].collisionFilter = 1;
					sceneBodies[lhand_index].collisionFilter = 1;
					sceneBodies[rarm1_index].collisionFilter = 1;
					sceneBodies[rarm2_index].collisionFilter = 1;
					sceneBodies[rhand_index].collisionFilter = 1;
					sceneBodies[lleg1_index].collisionFilter = 1;
					sceneBodies[lleg2_index].collisionFilter = 1;
					sceneBodies[rleg1_index].collisionFilter = 1;
					sceneBodies[rleg2_index].collisionFilter = 1;
				}
				else // i==2
				{
					// For this case, there are no setting for collision free parts.
					// therefore there are a lot of collision happens among any parts of the ragdoll.
					// Not unstable too much, just for the test
					// For the test, the head part is locked by SetBodyKinematic(...) 
					sceneBodies[head_index].SetBodyKinematic();
				}
				
				ragdoll_end_index[i] = numBody;
			
			}
			
			// Create test box				
			// Create a dynamic rigid body whose shape is box (1.0 Kg)
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
			sceneBodies[numBody].position = new Vector2(-30.0f, 0.0f);
			sceneBodies[numBody].shapeIndex = 2;
			numBody++;
			
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
			sceneBodies[numBody].position = new Vector2(-30.0f, 10.0f);
			sceneBodies[numBody].shapeIndex = 3;
			numBody++;		
			
        }

#else
		
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "CreateRagdollScene";
			
			// Before making the rigid sceneBodies, setup collision shapes data

            // Box Shape Setting AddBoxShape(new Vector2("width", "height"))
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
			AddBoxShape(wall_height);
			
			Vector2 box_width = new Vector2(3.0f, 3.0f);
            AddBoxShape(box_width);
	
			float sphere_width = 3.0f;
			AddSphereShape(sphere_width);

			// body center
			Vector2 body_width = new Vector2(2.0f, 2.5f);
			AddBoxShape(body_width);
			
			// head
			float head_width = 1.5f;
			AddSphereShape(head_width);
			
			// arm 1, 2
			Vector2 arm_width = new Vector2(1.5f, 0.75f);
			AddBoxShape(arm_width);

			// hand
			float hand_width = 0.75f;
			AddSphereShape(hand_width);

			// leg 1, 2
			Vector2 leg_width = new Vector2(0.75f, 2.0f);
			AddBoxShape(leg_width);
		
             // Create static walls to limit the range of action of active rigid sceneBodies
            {
				PhysicsBody body = null;
                // new PhysicsBody( "shape of the body",  "mass of the body(kg)" ) 
                body = AddBody(SceneShapes[0], PhysicsUtility.FltMax);

                // Set the position & the rotation
                body.Position = new Vector2(0, -wall_height.Y);
                body.Rotation = 0;

                body = AddBody(SceneShapes[1], PhysicsUtility.FltMax);
                body.Position = new Vector2(wall_width.X, 0);
                body.Rotation = 0;

                body = AddBody(SceneShapes[1], PhysicsUtility.FltMax);
                body.Position = new Vector2(-wall_width.X, 0);
                body.Rotation = 0;

                body = AddBody(SceneShapes[0], PhysicsUtility.FltMax);
                body.Position = new Vector2(0, wall_height.Y);
                body.Rotation = 0;
            }
			
			
            // Create the ragdoll
			// Ragdoll is composed with many primitives and joints
			for(int i=0; i<3; i++)
            {
				PhysicsBody center = null, head = null;
				PhysicsBody larm1 = null, larm2 = null, lhand = null;
				PhysicsBody rarm1 = null, rarm2 = null, rhand = null;
				PhysicsBody lleg1 = null, lleg2 = null;
				PhysicsBody rleg1 = null, rleg2 = null;

				ragdoll_start_index[i] = numBody;
				
				// create body
				Vector2 center_pos = new Vector2(-10.0f + 20.0f * i, 5.0f);
				
				center = AddBody(SceneShapes[4], 5.0f);
                center.Position = center_pos;
                center.Rotation = 0;
 
				// create head
				head = AddBody(SceneShapes[5], 1.0f);
                head.Position = center_pos + new Vector2(0.0f, body_width.Y) + new Vector2(0, head_width);
                head.Rotation = 0;			
				
				// creat joint between body and head
				PhysicsJoint joint = AddJoint(center, head, head.Position + new Vector2(0.0f, -head_width));
				joint.Axis1Lim = new Vector2(1.0f, 0.0f);
				joint.Axis2Lim = new Vector2(0.0f, 1.0f);
				joint.AngleLim = 1;
				joint.AngleLower = PhysicsUtility.GetRadian(-10.0f);
				joint.AngleUpper = PhysicsUtility.GetRadian(10.0f);
				
				// Left Upper Part
				{
					// create left arm1					
					larm1 = AddBody(SceneShapes[6], 1.0f);
	                larm1.Position = center_pos + new Vector2(-body_width.X, 0.5f*body_width.Y) + new Vector2(-arm_width.X, 0.0f);
	                larm1.Rotation = 0;	
				
					// creat joint between body and left arm1				
					joint = AddJoint(center, larm1, larm1.Position+ new Vector2(+arm_width.X, 0.0f));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 1;
					joint.AngleLower = PhysicsUtility.GetRadian(-45.0f);
					joint.AngleUpper = PhysicsUtility.GetRadian(45.0f);
					
					// create left arm2	
					larm2 = AddBody(SceneShapes[6], 1.0f);
	                larm2.Position = center_pos + new Vector2(-body_width.X, 0.5f*body_width.Y) + new Vector2(-arm_width.X, 0.0f) + new Vector2(-2.0f*arm_width.X, 0.0f);
	                larm2.Rotation = 0;
			
					// creat joint between left arm1 and left arm2			
					joint = AddJoint(larm1, larm2, 0.5f*(larm1.Position + larm2.Position));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 1;
					joint.AngleLower = PhysicsUtility.GetRadian(-45.0f);
					joint.AngleUpper = PhysicsUtility.GetRadian(45.0f);
					
					// create left hand
					lhand = AddBody(SceneShapes[7], 1.0f);
	                lhand.Position = center_pos + new Vector2(-body_width.X, 0.5f*body_width.Y) + new Vector2(-4.0f*arm_width.X, 0.0f) + new Vector2(-hand_width, 0.0f);
	                lhand.Rotation = 0;
					
					// creat joint between left arm2 and left hand				
					joint = AddJoint(larm2, lhand, lhand.Position + new Vector2(hand_width, 0));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 1;
					joint.AngleLower = PhysicsUtility.GetRadian(-10.0f);
					joint.AngleUpper = PhysicsUtility.GetRadian(10.0f);
				}
				
			
				// Right Upper Part				
				{
					// create right arm1
					rarm1 = AddBody(SceneShapes[6], 1.0f);				
	                rarm1.Position = center_pos + new Vector2(+body_width.X, 0.5f*body_width.Y) + new Vector2(+arm_width.X, 0.0f);
	                rarm1.Rotation = 0;

					// creat joint between body and right arm1				
					joint = AddJoint(center, rarm1, rarm1.Position+ new Vector2(-arm_width.X, 0.0f));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 1;
					joint.AngleLower = PhysicsUtility.GetRadian(-45.0f);
					joint.AngleUpper = PhysicsUtility.GetRadian(45.0f);

					// create right arm2	
					rarm2 = AddBody(SceneShapes[6], 1.0f);
	                rarm2.Position = center_pos + new Vector2(+body_width.X, 0.5f*body_width.Y) + new Vector2(+arm_width.X, 0.0f) + new Vector2(+2.0f*arm_width.X, 0.0f);
	                rarm2.Rotation = 0;
					
					// creat joint between right arm1 and right arm2				
					joint = AddJoint(rarm1, rarm2, 0.5f*(rarm1.Position + rarm2.Position));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 1;
					joint.AngleLower = PhysicsUtility.GetRadian(-45.0f);
					joint.AngleUpper = PhysicsUtility.GetRadian(45.0f);
					
					// create right hand	
					rhand = AddBody(SceneShapes[7], 1.0f);
	                rhand.Position = center_pos + new Vector2(+body_width.X, 0.5f*body_width.Y) + new Vector2(+4.0f*arm_width.X, 0.0f) + new Vector2(+hand_width, 0.0f);
	                rhand.Rotation = 0;
					
					// creat joint between right arm2 and right hand				
					joint = AddJoint(rarm2, rhand, rhand.Position + new Vector2(hand_width, 0));
					joint.axis1Lim = new Vector2(1.0f, 0.0f);
					joint.axis2Lim = new Vector2(0.0f, 1.0f);
					joint.angleLim = 1;
					joint.angleLower = PhysicsUtility.GetRadian(-10.0f);
					joint.angleUpper = PhysicsUtility.GetRadian(10.0f);
				}	
				
				
				// Left Lower Part
				{
					// create left leg1					
					lleg1 = AddBody(SceneShapes[8], 1.0f);
	                lleg1.Position = center_pos + new Vector2(-0.5f*body_width.X, -body_width.Y) + new Vector2(0.0f, -leg_width.Y);
	                lleg1.Rotation = 0;	
				
					// creat joint between body and left leg1				
					joint = AddJoint(center, lleg1, lleg1.Position + new Vector2(0.0f, +leg_width.Y));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 1;
					joint.AngleLower = PhysicsUtility.GetRadian(-45.0f);
					joint.AngleUpper = PhysicsUtility.GetRadian(10.0f);
						
					// create left leg2	
					lleg2 = AddBody(SceneShapes[8], 1.0f);
	                lleg2.Position = center_pos + new Vector2(-0.5f*body_width.X, -body_width.Y) + new Vector2(0.0f, -3.0f*leg_width.Y);
	                lleg2.Rotation = 0;
					
					// creat joint between left leg1 and left leg2				
					joint = AddJoint(lleg1, lleg2, 0.5f*(lleg1.Position + lleg2.Position));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 1;
					joint.AngleLower = PhysicsUtility.GetRadian(-45.0f);
					joint.AngleUpper = PhysicsUtility.GetRadian(10.0f);
				}
				
				
				// Right Lower Part
				{
					// create left leg1					
					rleg1 = AddBody(SceneShapes[8], 1.0f);
	                rleg1.Position = center_pos + new Vector2(+0.5f*body_width.X, -body_width.Y) + new Vector2(0.0f, -leg_width.Y);
	                rleg1.Rotation = 0;	
				
					// creat joint between body and left leg1				
					joint = AddJoint(center, rleg1, rleg1.Position + new Vector2(0.0f, +leg_width.Y));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 1;
					joint.AngleLower = PhysicsUtility.GetRadian(-45.0f);
					joint.AngleUpper = PhysicsUtility.GetRadian(10.0f);

					// create left leg2	
					rleg2 = AddBody(SceneShapes[8], 1.0f);
	                rleg2.Position = center_pos + new Vector2(+0.5f*body_width.X, -body_width.Y) + new Vector2(0.0f, -3.0f*leg_width.Y);
	                rleg2.Rotation = 0;
					
					// creat joint between left leg1 and left leg2				
					joint = AddJoint(rleg1, rleg2, 0.5f*(rleg1.Position + rleg2.Position));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 1;
					joint.AngleLower = PhysicsUtility.GetRadian(-45.0f);
					joint.AngleUpper = PhysicsUtility.GetRadian(10.0f);
				}				
				
				
				// Collision model are different among three ragdolls
				if(i==0)
				{
					// Ignore only those collisions
					// In the case that very strong force is applied
					// (touch pad drag is one of strong forces becase making it move forcely) to the leg, 
					// then it can cause the interpenetration between left leg & right leg.
					// Once the interpenetration happens, 
					// it is difficult for those parts to restore original relative position.
					
					AddIgnorePair(center, head);
					
					AddIgnorePair(center, rarm1);
					AddIgnorePair(rarm1, rarm2);
					AddIgnorePair(rarm2, rhand);
					
					AddIgnorePair(center, larm1);
					AddIgnorePair(larm1, larm2);
					AddIgnorePair(larm2, lhand);
					
					AddIgnorePair(center, rleg1);
					AddIgnorePair(rleg1, rleg2);
					
					AddIgnorePair(center, lleg1);
					AddIgnorePair(lleg1, lleg2);
				}
				else if(i==1) // This is a recommendation for computational cost and stability
				{
					// Cancel all collisions inside the ragdoll by using same collision filter.
					// Compared with the above case(i==0),
					// this case is relatively stable because collision free model is applied to this ragdoll.
					center.CollisionFilter = 1;
					head.CollisionFilter = 1;
					larm1.CollisionFilter = 1;
					larm2.CollisionFilter = 1;
					lhand.CollisionFilter = 1;
					rarm1.CollisionFilter = 1;
					rarm2.CollisionFilter = 1;
					rhand.CollisionFilter = 1;
					lleg1.CollisionFilter = 1;
					lleg2.CollisionFilter = 1;
					rleg1.CollisionFilter = 1;
					rleg2.CollisionFilter = 1;
				}
				else // i==2
				{
					// For this case, there are no setting for collision free parts.
					// therefore there are a lot of collision happens among any parts of the ragdoll.
					// Not unstable too much, just for the test
					// For the test, the head part is locked by SetBodyKinematic(...) 
					head.SetBodyKinematic();
				}
				
				ragdoll_end_index[i] = NumBody;

			}
			
			// Create test box				
			// Create a dynamic rigid body whose shape is box (1.0 Kg)
			{
				PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
				body.Position = new Vector2(-30.0f, 0.0f);
			}
			
			{
				PhysicsBody body = AddBody(SceneShapes[3], 1.0f);
				body.Position = new Vector2(-30.0f, 10.0f);
			}
			
        }

#endif
		// Line Rendering for Object
        private void MakeLineListConvex(PhysicsShape con, VertexBuffer vertices)
        {

            if (con.numVert == 0)
            {
                float[] vertex = new float[3 * 37];

                int i = 0;
                float rad = con.vertList[0].X;

                for (float th1 = 0.0f; th1 < 360.0f; th1 = th1 + 10.0f)
                {
                    float th1_rad = th1 / 180.0f * PhysicsUtility.Pi;

                    float x1 = rad * (float)Math.Cos(th1_rad);
                    float y1 = rad * (float)Math.Sin(th1_rad);

                    vertex[3 * i + 0] = x1;
                    vertex[3 * i + 1] = y1;
                    vertex[3 * i + 2] = 0.0f;
                    i++;
                }

                vertex[3 * i + 0] = vertex[3 * 0 + 0];
                vertex[3 * i + 1] = vertex[3 * 0 + 1];
                vertex[3 * i + 2] = vertex[3 * 0 + 2];

                vertices.SetVertices(0, vertex);

            }
            else
            {
                float[] vertex = new float[3 * (con.numVert + 1)];

                int i;

                for (i = 0; i < con.numVert; i++)
                {
                    Vector2 v1;
                    v1 = con.vertList[i];

                    vertex[3 * i + 0] = v1.X;
                    vertex[3 * i + 1] = v1.Y;
                    vertex[3 * i + 2] = 0.0f;
                }

                vertex[3 * i + 0] = vertex[3 * 0 + 0];
                vertex[3 * i + 1] = vertex[3 * 0 + 1];
                vertex[3 * i + 2] = vertex[3 * 0 + 2];

                vertices.SetVertices(0, vertex);
            }
        }


        // Draw objects
        public override void DrawAllBody(ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 renderMatrix, int click_index)
        {
            for (int j = 0; j < numShape; j++)
            {
                graphics.SetVertexBuffer(0, vertices[j]);

                for (int i = 0; i < numBody; i++)
                {
                    uint index = sceneBodies[i].shapeIndex;

                    if (j != index) continue;

                    Matrix4 rotationMatrix = Matrix4.RotationZ(sceneBodies[i].rotation);

                    Matrix4 transMatrix = Matrix4.Translation(
                        new Vector3(sceneBodies[i].position.X, sceneBodies[i].position.Y, 0.0f));

                    Matrix4 local_rotationMatrix = Matrix4.RotationZ(sceneBodies[i].localRotation);

                    Matrix4 local_transMatrix = Matrix4.Translation(
                        new Vector3(sceneBodies[i].localPosition.X, sceneBodies[i].localPosition.Y, 0.0f));

                    Matrix4 WorldMatrix = renderMatrix * transMatrix * rotationMatrix * local_transMatrix * local_rotationMatrix;

                    program.SetUniformValue(0, ref WorldMatrix);

                    if (i == click_index)
                    {
                        Vector3 color = new Vector3(1.0f, 0.0f, 0.0f);
                        program.SetUniformValue(1, ref color);
                    }
                    else
                    {
						Vector3 color;
						
						if((i>=ragdoll_start_index[0])&&(i<ragdoll_end_index[0]))
							color = new Vector3(0.0f, 1.0f, 0.0f);	
						else if((i>=ragdoll_start_index[1])&&(i<ragdoll_end_index[1]))
							color = new Vector3(1.0f, 0.0f, 1.0f);
						else if((i>=ragdoll_start_index[2])&&(i<ragdoll_end_index[2]))
							color = new Vector3(1.0f, 1.0f, 0.0f);
						else
							color = new Vector3(0.0f, 1.0f, 1.0f);
						
						program.SetUniformValue(1, ref color);
                    }

                    if (sceneShapes[index].numVert == 0)
                        graphics.DrawArrays(DrawMode.LineStrip, 0, 37);
                    else
                        graphics.DrawArrays(DrawMode.LineStrip, 0, sceneShapes[index].numVert + 1);

                }

            }
        }

        // Debug rendering for contact points(RigidBody A <=> RigidBody B) and AABB(Axis Aligned Bounding Box)
        public override void DrawAdditionalInfo(ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 renderMatrix)
        {
            // Draw contact points
            graphics.SetVertexBuffer(0, colVert);

            for (uint i = 0; i < numPhysicsSolverPair; i++)
            {
                // Collision point for RigidBody A 
                {
                    Matrix4 transMatrix = Matrix4.Translation(
                        new Vector3(solverPair[i].resA.X, solverPair[i].resA.Y, 0.0f));

                    Matrix4 WorldMatrix = renderMatrix * transMatrix;
                    program.SetUniformValue(0, ref WorldMatrix);

                    Vector3 color = new Vector3(1.0f, 0.0f, 0.0f);
                    program.SetUniformValue(1, ref color);

                    graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
                }

                // Collision point for RigidBody B 
                {
                    Matrix4 transMatrix = Matrix4.Translation(
                        new Vector3(solverPair[i].resB.X, solverPair[i].resB.Y, 0.0f));

                    Matrix4 WorldMatrix = renderMatrix * transMatrix;
                    program.SetUniformValue(0, ref WorldMatrix);

                    Vector3 color = new Vector3(1.0f, 0.0f, 0.0f);
                    program.SetUniformValue(1, ref color);

                    graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
                }
            }
        }
	}
}


		
		

		

		
