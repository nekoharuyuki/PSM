/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


//
// Small Scale Scene --- Small World Creation
//
// How to set up the simulation scene
// (1) Make an simulation scene
// (2) Arrange coefficients for contact solver or gravity etc...
// (3) Prepare collision shapes for rigid sceneBodies
// (4) Make the rigid body instances by specifying collision shapes and masses 
// (5) Set the position, rotation, velocity and angular velocity
//




//
// Tips and notice
//
// To create a simulation scene, please create a class that inherits from PhysicsScene.
//
// InitScene() sets up the initial state of the simulation scene.
//
// You can create a static 2d rigid body by setting its mass to PhysicsUtility.FltMax.
//
// this.restitutionCoeff = 0.8f; makes things bounce easily in the simulation
// To make stacks relatively stable, please set this.restitutionCoeff a bit lower (<= 0.3)
//
// The simulation's main loop is the parent class's PhysicsScene::Simulate()
// By calling this, the simulation is processed using the default time step of 1/60s
//
// DrawAdditionalInfo() does debug rendering of collision points(RigidBodyA<->RigidBodyB)
// and AABB(axis-aligned bounding box) for rigid sceneBodies
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
    public class SmallScaleScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // Vertex Buffer for Debug Rendering
        private VertexBuffer aabbVert = null;
        private VertexBuffer colVert = null;

        private System.Random rand_gen = new System.Random(10);
        private int index0, index1, index2;
		
		//
		// The scene here is basically similar to what is done in PrimitiveScene,
		// but since the scale is smaller objects appear to move faster.
		// The perceived speed and the scale of objects are related.
		//
		private const float scene_scale = 0.2f; 
        
        public SmallScaleScene()
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

            // VertexBuffer for AABB debug rendering
            {
                aabbVert = new VertexBuffer(5, VertexFormat.Float3);

                float[] vertex = new float[]
                {
                    0.0f, 0.0f, 0.0f,
                    1.0f, 0.0f, 0.0f,
                    1.0f, 1.0f, 0.0f,
                    0.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 0.0f,
                };

                aabbVert.SetVertices(0, vertex);
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

        ~SmallScaleScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {
            for (int i = 0; i < numShape; i++)
                if(vertices[i] != null)
                    vertices[i].Dispose();

            if(aabbVert != null) aabbVert.Dispose();
            if(colVert != null) colVert.Dispose();
        }
		
#if USE_OLD_API

		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "SmallScaleScene";
			
            // Set the restitution coefficient a bit stronger
            this.restitutionCoeff = 0.8f;

            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width * scene_scale);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height * scene_scale);

            Vector2 box_width = new Vector2(2.0f, 2.0f);
            sceneShapes[2] = new PhysicsShape(box_width * scene_scale);

            // Sphere Shape Setting PhysicsShape( "radius" )
            float sphere_width = 2.0f;
            sceneShapes[3] = new PhysicsShape(sphere_width * scene_scale);


            Vector2[] test_point = new Vector2[10];

            for (int i = 0; i < 10; i++)
            {
                test_point[i] = new Vector2(rand_gen.Next(-1000, 1000), rand_gen.Next(-1000, 1000)) * 2.0f / 1000.0f;
                test_point[i] = test_point[i] * scene_scale;
            }

            // Convex Shape Setting (by using random points)
            sceneShapes[4] = PhysicsShape.CreateConvexHull(test_point, 10);

            numShape = 5;
     

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

            // Create dynamic rigid sceneBodies
            {
                // Create a box-shaped dynamic rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                    sceneBodies[numBody].position = new Vector2(-10.0f, -5.0f);
                    sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f);
                    sceneBodies[numBody].shapeIndex = 2;
                    numBody++;
                }

                // Create a sphere-shaped dynamic rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
                    sceneBodies[numBody].position = new Vector2(0.0f, -5.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 3;
                    sceneBodies[numBody].colFriction = 0.01f;
                    numBody++;
                }

                // Create a convex-shaped dynamic rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 1.0f);
                    sceneBodies[numBody].position = new Vector2(10.0f, -5.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 4;
                    numBody++;
                }
            }

            // Create static rigid sceneBodies
            {
                // Create a box-shaped static rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(-10.0f, 5.0f);
                    sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f);
                    sceneBodies[numBody].shapeIndex = 2;
                    numBody++;
                }

                // Create a sphere-shaped static rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(0.0f, 5.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 3;
                    sceneBodies[numBody].colFriction = 0.01f;
                    numBody++;
                }

                // Create a convex-shaped static rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(10.0f, 5.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 4;
                    numBody++;
                }
            }


            // キネマティック剛体（生成時に動的な剛体として生成され、一時的に静的な剛体になっている）を作成する場合には、
            // 普通の動的な剛体として生成した後、setKinematic()を呼び、動的な剛体に戻すにはbackToDynamic()を呼ぶ
            //
            //
            // E: Create kinematic rigid sceneBodies
            //
            // Kinematic rigid sceneBodies should be defined as dynamic objects at first,
            // and then those should be switched to the kinematic object by calling setKinematic().
            // Kinematic rigid sceneBodies can be changed to dynamic rigid sceneBodies by calling backToDynamic() again.
            //
            {
                // Create a kinematic rigid body whose shape is box
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                    sceneBodies[numBody].position = new Vector2(-10.0f, 15.0f);
                    sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f);
                    sceneBodies[numBody].shapeIndex = 2;
                    sceneBodies[numBody].SetBodyKinematic();
                    index0 = numBody++;
                }

                // Create a sphere-shaped kinematic rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
                    sceneBodies[numBody].position = new Vector2(0.0f, 15.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 3;
                    sceneBodies[numBody].colFriction = 0.01f;
                    sceneBodies[numBody].SetBodyKinematic();
                    index1 = numBody++;
                }

                // Create a convex-shaped kinematic rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 1.0f);
                    sceneBodies[numBody].position = new Vector2(10.0f, 15.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 4;
                    sceneBodies[numBody].SetBodyKinematic();
                    index2 = numBody++;
                }
            }

            for(int i=0; i<numBody; i++)
            {
            	sceneBodies[i].position = sceneBodies[i].position * scene_scale;
            }

        }


#else
			
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "SmallScaleScene";
			
            // Set the restitution coefficient a bit stronger
            this.RestitutionCoeff = 0.8f;

            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width * scene_scale);

            Vector2 wall_height = new Vector2(8, 30);
            AddBoxShape(wall_height * scene_scale);

            Vector2 box_width = new Vector2(2.0f, 2.0f);
            AddBoxShape(box_width * scene_scale);

            // Sphere Shape Setting PhysicsShape( "radius" )
            float sphere_width = 2.0f;
            AddSphereShape(sphere_width * scene_scale);


            Vector2[] test_point = new Vector2[10];

            for (int i = 0; i < 10; i++)
            {
                test_point[i] = new Vector2(rand_gen.Next(-1000, 1000), rand_gen.Next(-1000, 1000)) * 2.0f / 1000.0f;
                test_point[i] = test_point[i] * scene_scale;
            }

            // Convex Shape Setting (by using random points)
            AddConvexHullShape(test_point, 10);
				

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

            // Create dynamic rigid sceneBodies
            {
                // Create a box-shaped dynamic rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
                    body.Position = new Vector2(-10.0f, -5.0f);
                    body.Rotation = PhysicsUtility.GetRadian(30.0f);
                }

                // Create a sphere-shaped dynamic rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[3], 1.0f);
                    body.Position = new Vector2(0.0f, -5.0f);
                    body.Rotation = 0;
                    body.ColFriction = 0.01f;
                }

                // Create a convex-shaped dynamic rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[4], 1.0f);
                    body.Position = new Vector2(10.0f, -5.0f);
                    body.Rotation = 0;
                }
            }

            // Create static rigid sceneBodies
            {
                // Create a box-shaped static rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[2], PhysicsUtility.FltMax);
                    body.Position = new Vector2(-10.0f, 5.0f);
                    body.Rotation = PhysicsUtility.GetRadian(30.0f);
                }

                // Create a sphere-shaped static rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[3], PhysicsUtility.FltMax);
                    body.Position = new Vector2(0.0f, 5.0f);
                    body.Rotation = 0;
                    body.ColFriction = 0.01f;
                }

                // Create a convex-shaped static rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[4], PhysicsUtility.FltMax);
                    body.Position = new Vector2(10.0f, 5.0f);
                    body.Rotation = 0;
                }
            }
            // Create static rigid sceneBodies
            {
                // Create a box-shaped static rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[2], PhysicsUtility.FltMax);
                    body.Position = new Vector2(-10.0f, 5.0f);
                    body.Rotation = PhysicsUtility.GetRadian(30.0f);
                }

                // Create a sphere-shaped static rigid body
                {
                    PhysicsBody body = AddBody(sceneShapes[3], PhysicsUtility.FltMax);
                    body.Position = new Vector2(0.0f, 5.0f);
                    body.Rotation = 0;
                    body.ColFriction = 0.01f;
                }

                // Create a convex-shaped static rigid body
                {
                    PhysicsBody body = AddBody(sceneShapes[4], PhysicsUtility.FltMax);
                    body.Position = new Vector2(10.0f, 5.0f);
                    body.Rotation = 0;
                }
            }
			// キネマティック剛体（生成時に動的な剛体として生成され、一時的に静的な剛体になっている）を作成する場合には、
            // 普通の動的な剛体として生成した後、setKinematic()を呼び、動的な剛体に戻すにはbackToDynamic()を呼ぶ
            //
            //
            // E: Create kinematic rigid sceneBodies
            //
            // Kinematic rigid sceneBodies should be defined as dynamic objects at first,
            // and then those should be switched to the kinematic object by calling setKinematic().
            // Kinematic rigid sceneBodies can be changed to dynamic rigid sceneBodies by calling backToDynamic() again.
            //
            {
                // Create a kinematic rigid body whose shape is box
                {
                    PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
                    body.Position = new Vector2(-10.0f, 15.0f);
                    body.Rotation = PhysicsUtility.GetRadian(30.0f);
                    body.SetBodyKinematic();
                    index0 = NumBody-1;
                }

                // Create a sphere-shaped kinematic rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[3], 1.0f);
                    body.Position = new Vector2(0.0f, 15.0f);
                    body.Rotation = 0;
                    body.ColFriction = 0.01f;
                    body.SetBodyKinematic();
                    index1 = NumBody-1;
                }

                // Create a convex-shaped kinematic rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[4], 1.0f);
                    body.Position = new Vector2(10.0f, 15.0f);
                    body.Rotation = 0;
                    body.SetBodyKinematic();
                    index2 = NumBody-1;
                }
            }

            for(int i=0; i<NumBody; i++)
            {
            	SceneBodies[i].Position = SceneBodies[i].Position * scene_scale;
            }

        }
#endif
			


        // Game Controller Handling
        public override void KeyboardFunc(GamePadButtons ePad)
        {
            switch (ePad)
            {
                case GamePadButtons.Square:
                    
                    //
                    // isKinematicOrStatic() checks the mass of the rigid body and determines whether a body is kinematic or static
                    // backToDynamic() cannot be called for rigid sceneBodies not set up with setKinematic()
                    //
                    if (sceneBodies[index0].IsKinematicOrStatic())
                        sceneBodies[index0].BackToDynamic();
                    else
                        sceneBodies[index0].SetBodyKinematic();

                    if (sceneBodies[index1].IsKinematicOrStatic())
                        sceneBodies[index1].BackToDynamic();
                    else
                        sceneBodies[index1].SetBodyKinematic();

                    if (sceneBodies[index2].IsKinematicOrStatic())
                        sceneBodies[index2].BackToDynamic();
                    else
                        sceneBodies[index2].SetBodyKinematic();

                    break;
                default:
                    break;
            }
        }


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
                        Vector3 color = new Vector3(0.0f, 1.0f, 1.0f);
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

            // Draw AABB Bounding Box
            graphics.SetVertexBuffer(0, aabbVert);

            for (uint i = 0; i < numBody; i++)
            {

                Matrix4 scaleMatrix = new Matrix4(
                     sceneBodies[i].aabbMax.X - sceneBodies[i].aabbMin.X, 0.0f, 0.0f, 0.0f,
                     0.0f, sceneBodies[i].aabbMax.Y - sceneBodies[i].aabbMin.Y, 0.0f, 0.0f,
                     0.0f, 0.0f, 1.0f, 0.0f,
                     0.0f, 0.0f, 0.0f, 1.0f
                 );

                Matrix4 transMatrix = Matrix4.Translation(
                    new Vector3(sceneBodies[i].aabbMin.X, sceneBodies[i].aabbMin.Y, 0.0f));

                Matrix4 WorldMatrix = renderMatrix * transMatrix * scaleMatrix;
                program.SetUniformValue(0, ref WorldMatrix);

                Vector3 color = new Vector3(1.0f, 1.0f, 0.0f);
                program.SetUniformValue(1, ref color);

                graphics.DrawArrays(DrawMode.LineStrip, 0, 5);
            }
        }
	}
}


		
		

		

		
