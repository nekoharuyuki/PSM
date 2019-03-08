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
	//
	// This is algorithm class
	// All functions are defined as static functions.
	//
	
	/// @if LANG_EN
    /// <summary> Physics2D simulation base scene </summary>
	/// @endif
	/// @if LANG_JA
    /// <summary> ２Ｄ物理シミュレーションのシーン </summary>
	/// @endif
    public partial class PhysicsScene
    {	
				
		/// @if LANG_EN
		/// <summary> Switch the algorithm of checking sleep status</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> スリープ状態をチェックする際のアルゴリズムをスイッチする</summary>
		/// @endif
		public enum SleepAlgorithmType
		{
			/// @if LANG_EN
			/// <summary> Based on broadphase collision </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary> broadphase collisionに基づいて </summary>
			/// @endif 
			BroadphaseBase, 
			
			/// @if LANG_EN
			/// <summary> Based on narrowphase collision </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary> narrowphase collisionに基づいて </summary>
			/// @endif 
			NarrowphaseBase
		}
		
		
		#region Field
		// Following variables defines the limitation of the scene
	    private const int maxBody = 250;
	    private const int maxContacts = 5000;
		private const int maxShape = 100;
        private const int maxJoint = 100;
        private const int maxSpring = 100;
		private const int maxIgnore = 250;
		
		/// @if LANG_EN
		/// <summary> number of rigid sceneBodies in the scene</summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中の剛体数</summary>
	 	/// @endif
	    public int numBody = 0;
		
		/// @if LANG_EN
		/// <summary> number of joints in the scene</summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中のジョイント数</summary>
	 	/// @endif
		public int numJoint = 0;
		
		/// @if LANG_EN
		/// <summary> number of springs in the scene</summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中のスプリング数</summary>
	 	/// @endif
		public int numSpring = 0;
		
		/// @if LANG_EN
		/// <summary> number of shapes in the scene</summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中の形状数</summary>
	 	/// @endif
		public int numShape = 0;
		
		/// @if LANG_EN	
		/// <summary> number of broad phase collision pairs </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> broad phase衝突判定により衝突可能性のあるペア数 </summary>
	 	/// @endif
		public int numBroadPair = 0;
		
		/// @if LANG_EN	
		/// <summary> number of narrow phase collision pairs </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> narrow phase衝突判定により衝突するペア数</summary>
	 	/// @endif
		public int numPhysicsSolverPair = 0;
		
		/// @if LANG_EN	
		/// <summary> number of old narrow phase collision pairs </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> 前のnarrow phase衝突判定により衝突するペア数</summary>
	 	/// @endif
	    public int numOldPhysicsSolverPair = 0;
		
		/// @if LANG_EN	
		/// <summary> array of rigid sceneBodies in the scene </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中の剛体リスト</summary>
	 	/// @endif
	    public PhysicsBody[] sceneBodies = new PhysicsBody[maxBody];
	 
		/// @if LANG_EN	
		/// <summary> array of shapes in the scene </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中の形状リスト </summary>
	 	/// @endif
		public PhysicsShape[] sceneShapes = new PhysicsShape[maxShape];
		
		/// @if LANG_EN    
		/// <summary> array of broad phase collision pairs in the scene </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中のbroad phase衝突ペアリスト </summary>
	 	/// @endif
		public uint[] broadPair = new uint[maxContacts];
	
		/// @if LANG_EN    
		/// <summary> array of separate axis theorem pairs in the scene </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中のSeparate Axis Theorem判定で衝突可能性ありペアリスト </summary>
	 	/// @endif
		private ContactPoint[] satPair = new ContactPoint[maxContacts];
	    
		/// @if LANG_EN 
		/// <summary> array of old solver pairs in the scene </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中の過去の衝突ペアリスト </summary>
	 	/// @endif
		private SolverPairCache[] oldPhysicsSolverPair = new SolverPairCache[maxContacts];
	    
		/// @if LANG_EN 	
		/// <summary> array of solver pairs in the scene </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中の衝突ペアリスト</summary>
	 	/// @endif
		public PhysicsSolverPair[] solverPair = new PhysicsSolverPair[maxContacts];
	    
		/// @if LANG_EN 	
		/// <summary> array of joints in the scene </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中のジョイントリスト </summary>
	 	/// @endif
		public PhysicsJoint[] sceneJoints = new PhysicsJoint[maxJoint];
 
		/// @if LANG_EN 	
		/// <summary> array of springs in the scene </summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シーン中のスプリングリスト </summary>
	 	/// @endif
		public PhysicsSpring[] sceneSprings = new PhysicsSpring[maxSpring];
		
		/// @if LANG_EN 	
		/// <summary> compound map in the scene </summary>
		/// <remarks> internal use only </remarks>
	 	/// @endif
		/// @if LANG_JA
		/// <summary>　コンパウンド形状対応マップ </summary>
		/// <remarks> internal use only </remarks>
	 	/// @endif
        public uint[] compoundMap = new uint[maxBody];

		
		/// @if LANG_EN 
		/// <summary> simulation time step in the scene </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> シミュレーションのタイムステップ </summary>
		/// @endif
		public float simDt = 1.0f/60.0f;
        
		/// @if LANG_EN 
		/// <summary> gravity in the scene </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> シーン中の重力加速度 </summary>
		/// @endif
		public Vector2 gravity = new Vector2(0, -9.8f);
		
		/// @if LANG_EN 
		/// <summary> minimum value of bounding box of simulation area </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> シミュレーションエリアの境界 </summary>
		/// @endif
		public Vector2 sceneMin = new Vector2(-1000.0f, -1000.0f);
		
		/// @if LANG_EN 
		/// <summary> maximum value of bounding box of simulation area </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> シミュレーションエリアの境界 </summary>
		/// @endif
		public Vector2 sceneMax = new Vector2(1000.0f, 1000.0f);
		
		// For general scene, these parameters are suitable
		private const float LinearSleepVelocity = 0.1f;
		private const float AngularSleepVelocity = 0.01f;
		
		/// @if LANG_EN 
		/// <summary> scene frame number of this simulation </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> このシーンのシミュレーションが始まってからのフレーム数 </summary>
		/// @endif
		public ulong sceneFrame = 0;
	    		
		/// <summary> stop watch time of this simulation </summary>
		private PhysicsStopwatch sw;
			
		/// @if LANG_EN 
		/// <summary> scene name of this simulation </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> シミュレーションのシーン名 </summary>
		/// @endif
		public string sceneName = "Scene made by Physics2DEngine";
			
		/// @if LANG_EN 
		/// <summary> number of ignore pairs in the scene </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> シーン中の衝突を無視するペアの数 </summary>
		/// @endif
		public int numIgnorePair = 0;
		
		/// @if LANG_EN
		/// <summary> array of ignore pairs in the scene </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> シーン中の衝突を無視するペアリスト </summary>
		/// @endif
		public uint[] ignorePair = new uint[maxIgnore];
		
		/// <summary> ContactCacheDist </summary>
		/// <remarks> internal use only inside the algorithm </remarks>
        private const float ContactCacheDist = 0.05f;
		
		/// @if LANG_EN
		/// <summary> print out performance of each simulation phase</summary>
	 	/// @endif
		/// @if LANG_JA
		/// <summary> シミュレーション中の各計算フェーズのパフォーマンスを出力する</summary>
	 	/// @endif		
		public bool printPerf = false;
		
		/// @if LANG_EN
		/// <summary>whether use unique value for the scene</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>シーン特性にユニークな値を使うか</summary>
		/// @endif
		public bool uniqueProperty = true;
		
		/// @if LANG_EN
		/// <summary>repulse acceleration for penetration</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>めり込みに対する反発力の加速係数</summary>
		/// @endif
		public float penetrationRepulse = 0.2f;
				
		/// @if LANG_EN
		/// <summary>penetration limitation</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>めり込みに対する許容度</summary>
		/// @endif 
		public float penetLimit = 0.03f;
			
		/// @if LANG_EN
		/// <summary>tangent friction for repulse</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>反発に対する接線方向の摩擦係数</summary>
		/// @endif
		public float tangentFriction = 0.3f;
	        
		/// @if LANG_EN
		/// <summary>restitution coefficient for repulse</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>反発に対する反発係数</summary>
		/// @endif
		public float restitutionCoeff = 0.0f;
	        
		/// @if LANG_EN
		/// <summary>sleep algorithm based on broadphase or narrow phase</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スリープアルゴリズムがブロードフェーズによるか、ナローフェーズによるか</summary>
		/// @endif
		public SleepAlgorithmType sleepAlgorithm = SleepAlgorithmType.BroadphaseBase;
		
	    // For slippy scene
		//public static float penetrationRepulse = 1.0f;
		//public static float penetLimit = 0.0f;
		//public static float tangentFriction = 0.0f;
		//public static float restitutionCoeff = 0.0f;
	
			
		/// @if LANG_EN
		/// <summary>filterOp</summary>
		/// <remarks>OR / AND switch for collision and group filter combination</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>filterOp</summary>
		/// <remarks>ORとANDのcollision and group filter の組み合わせを変更する</remarks>
		/// @endif
		public BroadOpType filterOp;		
		
		private PhysicsShape aLine = new PhysicsShape();	
		private PhysicsBody staticDummy = new PhysicsBody();
	
		private int numBroadPairSum = 0;
		
		#endregion		

		
		
        #region Methods
		/// @if LANG_EN
		/// <summary> Default Constructor </summary>
  		/// @endif
		/// @if LANG_JA
		/// <summary>　シーンコンストラクタ</summary>
  		/// @endif
		public PhysicsScene()
		{
			sw = new PhysicsStopwatch();
			
			numBody = 0;
			numJoint = 0;
			numSpring = 0;
			numShape = 0;
			numIgnorePair = 0;
			
			numBroadPair = 0;
			numPhysicsSolverPair = 0;
			numOldPhysicsSolverPair = 0;
		
			// Initialization of compound map
			for(uint i=0; i<maxBody; i++)
				compoundMap[i] = i;			
		}
		
		/// @if LANG_EN
		/// <summary> PhysicsScene Destructor </summary>
        /// <remarks> clear numbers of properties of scene </remarks>
   		/// @endif
		/// @if LANG_JA
		/// <summary> PhysicsScene Destructor </summary>
        /// <remarks> プロパティをクリアする </remarks>
   		/// @endif
		~PhysicsScene()
		{
			numBody = 0;
			numJoint = 0;
			numSpring = 0;
			numShape = 0;
			numIgnorePair = 0;
			
			numBroadPair = 0;
			numPhysicsSolverPair = 0;
			numOldPhysicsSolverPair = 0;

            // Initialization of compound map
            for (uint i = 0; i < maxBody; i++)
                compoundMap[i] = i;
		}
		
		/// @if LANG_EN	
		/// <summary> Simulate </summary>
   		/// @endif
		/// @if LANG_JA
		/// <summary> シミュレートする </summary>
   		/// @endif
		public void Simulate()
		{
			Vector2 zero1 = new Vector2(0, 0);
			Vector2 zero2 = new Vector2(0, 0);
			Simulate(-1, zero1, zero2);
		}
		
		/// @if LANG_EN	
		/// <summary> Simulate </summary>
        /// <param name="click_index"> one of rigid body pair </param>
        /// <param name="click_pos"> the other of rigid body pair </param> 
        /// <param name="diff_pos"> sceneShapes of the scene </param>
     	/// <remarks> Simulate function including user-click event manipulation</remarks>
   		/// @endif
		/// @if LANG_JA
		/// <summary> シミュレートする </summary>
        /// <param name="click_index"> クリックした剛体のインデックス </param>
        /// <param name="click_pos"> クリックした位置 </param> 
        /// <param name="diff_pos"> クリックした位置と剛体の中心の差 </param>
     	/// <remarks> クリックによる影響を剛体に与えたい場合のシミュレート関数</remarks>
   		/// @endif
	    public void Simulate(int click_index, Vector2 click_pos, Vector2 diff_pos)
		{
			// Set performance counter
			if((printPerf == true)&&(sceneFrame%10 == 0))
				sw.printPerf = true;
			else
				sw.printPerf = false;
			
            //
			// To check whether settings are set properly
            //
			{
				if((numBody > maxBody) || (numJoint > maxJoint) || (numSpring > maxSpring) || (numShape > maxShape))
                {
					Console.WriteLine("Out of range of maximum of maxBody, maxJoint, maxSpring or maxShape");
                    numBody = numJoint = numSpring = numShape = 0;
                }
			}
			
			// User Update
			UpdateFuncBeforeSim();
			
			// Mouse Controll Phase----------------------------------------------------------------------------
			if((click_index != -1)&&(click_index < numBody)&&(sceneBodies[click_index].picking))
	        {
				click_index = (int)compoundMap[click_index];
	            sceneBodies[click_index].velocity =  ((click_pos - diff_pos) - sceneBodies[click_index].position) * (1.0f/simDt);
				sceneBodies[click_index].sleep = false;
				sceneBodies[click_index].sleepCount = 0;
	        }
			
			// Pre Integrate Phase----------------------------------------------------------------------------
            sw.Reset();
            sw.Start();
	        for (int i = 0; i < numBody; i++)
	        {
				Integrate.PreIntegratePhase(sceneBodies[i], sceneShapes, simDt, gravity);
	        }
	        sw.Stop();
			sw.Print("Pre Integrate");
			
			// Compound Shape support----------------------------------------------------------------------------
			for(int i=0; i<numBody; i++)
			{
				sceneBodies[i].compound = compoundMap[i];

				if(compoundMap[i] != i)
				{
					uint index = compoundMap[i];
					sceneBodies[i].position = sceneBodies[index].position;
					sceneBodies[i].rotation = sceneBodies[index].rotation;
					sceneBodies[i].velocity = sceneBodies[index].velocity;
					sceneBodies[i].angularVelocity = sceneBodies[index].angularVelocity;
					sceneBodies[i].force = sceneBodies[index].force;
					sceneBodies[i].torque = sceneBodies[index].torque;
				}
			}
			
			
	        // Broadphase Phase----------------------------------------------------------------------------
            sw.Reset();
			sw.Start();
			
			Broadphase.filterOp = filterOp;
			
	        {
	            numBroadPair = 0;
	
	            for (uint i = 0; i < numBody; i++)
	            {
	                for (uint j = i + 1; j < numBody; j++)
	                {
	                    if (Broadphase.AABB_BroadPhase(sceneBodies[i], sceneBodies[j]))
	                    {
							if(CheckOutOfBound(sceneBodies[i])) break;
							if(CheckOutOfBound(sceneBodies[j])) break;
							   
							uint index = PhysicsUtility.PackIdx(i, j);
							
							bool find_flag = false;
							// Check the ignore pair
							for(uint k = 0; k < numIgnorePair; k++)
							{
								if(index == ignorePair[k] ) {find_flag = true; break;}
								else if(index < ignorePair[k]) break;
							}
							
							if(find_flag) continue;
							
	                        broadPair[numBroadPair] = PhysicsUtility.PackIdx(i, j);
	                        numBroadPair++;
	                    }
	                }
				}	
	        }
			
	        sw.Stop();
			sw.Print("Broadphase");
			
			if(sleepAlgorithm == SleepAlgorithmType.BroadphaseBase)
			{
				
		        // IslandGeneration Phase----------------------------------------------------------------------------
	            sw.Reset();
				sw.Start();
		        {
					GenerateIsland();
		        }
		        sw.Stop();
				sw.Print("IslandGeneration");
			
			}
			
			
	        // Collision Phase----------------------------------------------------------------------------
            sw.Reset();
			sw.Start();
			
			numBroadPairSum = 0;
			
			{
	            for (int i = 0; i < numBroadPair; i++)
	            {
					uint index1, index2;
					index1 = sceneBodies[PhysicsUtility.UnpackIdx1(broadPair[i])].shapeIndex;
					index2 = sceneBodies[PhysicsUtility.UnpackIdx2(broadPair[i])].shapeIndex;
					
					PhysicsBody ob1, ob2;
					ob1 = sceneBodies[PhysicsUtility.UnpackIdx1(broadPair[i])];
					ob2 = sceneBodies[PhysicsUtility.UnpackIdx2(broadPair[i])];
						
					if(sceneShapes[index1].hint == 0)
					{
						float ca, sa;
						ca = (float)Math.Cos(ob1.rotation);
						sa = (float)Math.Sin(ob1.rotation);
						
						for(int j=0; j<sceneShapes[index1].numVert-1; j++)
						{
							
							staticDummy.Clear();
							staticDummy.SetBodyKinematic();
							staticDummy.position = ob1.position;
							staticDummy.rotation = ob1.rotation;
							
							aLine.numVert = 2;
							aLine.hint = 1;
							aLine.vertList[0] = sceneShapes[index1].vertList[j];
							aLine.vertList[1] = sceneShapes[index1].vertList[j+1];
								
							Vector2 tempMin = new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
							Vector2 tempMax = -new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
							
							for(int k=0; k<2; k++)
							{
								Vector2 v = aLine.vertList[k];
								v = new Vector2(ca*v.X - sa*v.Y, sa*v.X + ca*v.Y);	
								v += staticDummy.position;
								aLine.vertList[k] = v;
								tempMin = Vector2.Min(tempMin, v);
								tempMax = Vector2.Max(tempMax, v);
							}
							                       
							if (ob2.aabbMin.X > tempMax.X) continue;
	            			if (tempMin.X > ob2.aabbMax.X) continue;
	            			if (ob2.aabbMin.Y > tempMax.Y) continue;
	            			if (tempMin.Y > ob2.aabbMax.Y) continue;
							
							staticDummy.position = 0.5f * (aLine.vertList[0] + aLine.vertList[1]);
							staticDummy.rotation = 0.0f;
		
							aLine.vertList[0] = aLine.vertList[0] - staticDummy.position;
							aLine.vertList[1] = aLine.vertList[1] - staticDummy.position;
				
							Collision.TestAxisCheckOffset(staticDummy, ob2, aLine, sceneShapes[index2], sceneShapes, ref broadPair[i], ref satPair[numBroadPairSum]);
							
							ob1.narrowCollide |= staticDummy.narrowCollide;
							satPair[numBroadPairSum].centerA = staticDummy.position;
							numBroadPairSum++;
						}
					}
					else if(sceneShapes[index2].hint == 0)
					{
						float ca, sa;
						ca = (float)Math.Cos(ob2.rotation);
						sa = (float)Math.Sin(ob2.rotation);
						
						for(int j=0; j<sceneShapes[index2].numVert-1; j++)
						{	
							staticDummy.Clear();
							staticDummy.SetBodyKinematic();
							staticDummy.position = sceneBodies[PhysicsUtility.UnpackIdx2(broadPair[i])].position;
							staticDummy.rotation = sceneBodies[PhysicsUtility.UnpackIdx2(broadPair[i])].rotation;
								
							aLine.numVert = 2;
							aLine.hint = 1;
							aLine.vertList[0] = sceneShapes[index2].vertList[j];
							aLine.vertList[1] = sceneShapes[index2].vertList[j+1];
			
							Vector2 tempMin = new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
							Vector2 tempMax = -new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
							
							for(int k=0; k<2; k++)
							{
								Vector2 v = aLine.vertList[k];
								v = new Vector2(ca*v.X - sa*v.Y, sa*v.X + ca*v.Y);	
								v += staticDummy.position;
								aLine.vertList[k] = v;
								tempMin = Vector2.Min(tempMin, v);
								tempMax = Vector2.Max(tempMax, v);
							}
                      
							if (ob1.aabbMin.X > tempMax.X) continue;
	            			if (tempMin.X > ob1.aabbMax.X) continue;
	            			if (ob1.aabbMin.Y > tempMax.Y) continue;
	            			if (tempMin.Y > ob1.aabbMax.Y) continue;
							
							staticDummy.position = 0.5f * (aLine.vertList[0] + aLine.vertList[1]);
							staticDummy.rotation = 0.0f;
		
							aLine.vertList[0] = aLine.vertList[0] - staticDummy.position;
							aLine.vertList[1] = aLine.vertList[1] - staticDummy.position;
				
							Collision.TestAxisCheckOffset(ob1, staticDummy, sceneShapes[index1], aLine, sceneShapes, ref broadPair[i], ref satPair[numBroadPairSum]);
							
							ob2.narrowCollide |= staticDummy.narrowCollide;
							satPair[numBroadPairSum].centerB = staticDummy.position;
							numBroadPairSum++;
						}
					}
					else
					{					      
						Collision.TestAxisCheckOffset(sceneBodies[PhysicsUtility.UnpackIdx1(broadPair[i])], sceneBodies[PhysicsUtility.UnpackIdx2(broadPair[i])],
						                              sceneShapes[index1], sceneShapes[index2],
						                              sceneShapes, ref broadPair[i], ref satPair[numBroadPairSum]);
						
						numBroadPairSum++;
					}
	            }	
	        }
	        sw.Stop();
			sw.Print("Separate Axis");

            sw.Reset();
			sw.Start();
	        {
	            numPhysicsSolverPair = 0;
	
	            for (int i = 0; i < numBroadPairSum; i++)
	            {
	                int contact_num;
	                contact_num = Collision.DetectLineLineClosestPoint(satPair[i], ref solverPair[numPhysicsSolverPair], ref solverPair[numPhysicsSolverPair + 1]);
	                
					solverPair[numPhysicsSolverPair].centerA = satPair[i].centerA;
					solverPair[numPhysicsSolverPair].centerB = satPair[i].centerB;
					solverPair[numPhysicsSolverPair+1].centerA = satPair[i].centerA;
					solverPair[numPhysicsSolverPair+1].centerB = satPair[i].centerB;
					
					numPhysicsSolverPair += contact_num;
	            }
	        }
	        sw.Stop();
			sw.Print("Collision");

			// change pair for compound shape
			for(int i=0; i<numPhysicsSolverPair; i++)
			{
				uint index1 = PhysicsUtility.UnpackIdx1(solverPair[i].totalIndex);
				uint index2 = PhysicsUtility.UnpackIdx2(solverPair[i].totalIndex);
				solverPair[i].totalIndex = PhysicsUtility.PackIdx(compoundMap[index1], compoundMap[index2]);
			}
			
			
	        // CacheCheck Phase----------------------------------------------------------------------------
            sw.Reset();
			sw.Start();
	        {
				int last_key = 0;
				
	            for (int i = 0; i < numPhysicsSolverPair; i++)
	            {
	                PhysicsSolverPair ob = solverPair[i];
	                int totalIndex = (int)ob.totalIndex;
	
	                int j;
	                for (j=last_key; j < numOldPhysicsSolverPair; j++)
	                {
	                    if (oldPhysicsSolverPair[j].totalIndex == totalIndex)
	                    {
	
	                        if ((Vector2.Dot(ob.resA - oldPhysicsSolverPair[j].resA,
	                            ob.resA - oldPhysicsSolverPair[j].resA) < ContactCacheDist)
	                            &&
	                           (Vector2.Dot(ob.resB - oldPhysicsSolverPair[j].resB,
	                            ob.resB - oldPhysicsSolverPair[j].resB) < ContactCacheDist))
	                        {
	                            solverPair[i].Pn = oldPhysicsSolverPair[j].Pn;
	                            solverPair[i].Pt = oldPhysicsSolverPair[j].Pt;
								last_key = System.Math.Max(0, j-1);
	                            break;
	                        }
	                    }
	                    else
	                    {
	
	                    }
	                }

	                if (j == numOldPhysicsSolverPair)
	                {
	                    solverPair[i].Pn = 0.0f;
	                    solverPair[i].Pt = 0.0f;
	                }
	            }
	        }
	        sw.Stop();
			sw.Print("CacheCheck");
	

			if(sleepAlgorithm == SleepAlgorithmType.NarrowphaseBase)
			{
				
		        // IslandGeneration Phase----------------------------------------------------------------------------
	            sw.Reset();
				sw.Start();
		        {
					GenerateIsland();
		        }
		        sw.Stop();
				sw.Print("IslandGeneration");
			
			}
			
			
	        // Solver Phase----------------------------------------------------------------------------
            sw.Reset();
	        sw.Start();
			
			Solver.uniqueProperty = this.uniqueProperty;
			Solver.penetrationRepulse  = this.penetrationRepulse;
			Solver.penetLimit  = this.penetLimit;
			Solver.tangentFriction = this.tangentFriction;
			Solver.restitutionCoeff = this.restitutionCoeff;
			
			// At first calc spring before solver
			for(int i=0; i<numSpring; i++)
			{	   
				PhysicsSpring.SpringSolver(sceneSprings[i],                  
					sceneBodies[PhysicsUtility.UnpackIdx1(sceneSprings[i].totalIndex)],
	                sceneBodies[PhysicsUtility.UnpackIdx2(sceneSprings[i].totalIndex)],
	                simDt);
			}
			
	        {
	            const int iteration_num = 5;
	
	            // One time for contact pair
	            for (int i = 0; i < numPhysicsSolverPair; i++)
	            {
	
					uint index1, index2;
					index1 = PhysicsUtility.UnpackIdx1(solverPair[i].totalIndex);
					index2 = PhysicsUtility.UnpackIdx2(solverPair[i].totalIndex);
					
					if(sceneShapes[sceneBodies[index1].shapeIndex].hint == 0)
					{
						staticDummy.Clear();
						staticDummy.SetBodyKinematic();
						staticDummy.position = solverPair[i].centerA;
						
						Solver.PreStepOfSolver(
							staticDummy,
							sceneBodies[index2],
							ref solverPair[i]);
					}
					else if(sceneShapes[sceneBodies[index2].shapeIndex].hint == 0)
					{
						staticDummy.Clear();
						staticDummy.SetBodyKinematic();
						staticDummy.position = solverPair[i].centerB;
						
						Solver.PreStepOfSolver(
							sceneBodies[index1],
							staticDummy,
							ref solverPair[i]);
					}
					else
					{
		                Solver.PreStepOfSolver(
		                    sceneBodies[PhysicsUtility.UnpackIdx1(solverPair[i].totalIndex)],
		                    sceneBodies[PhysicsUtility.UnpackIdx2(solverPair[i].totalIndex)],
		                    ref solverPair[i]);
					}
	            }
	
	
				// Joint solver at first
	            for (int i = 0; i < 3; i++)
				{
	                for (int j = 0; j < numJoint; j++)
	                {
	                    bool flag;
	                    if (i == 0) flag = true;
	                    else flag = false;
						
						// check both actor is sleeping ?
						if((sceneBodies[PhysicsUtility.UnpackIdx1(sceneJoints[j].totalIndex)].sleep)
							&& (sceneBodies[PhysicsUtility.UnpackIdx2(sceneJoints[j].totalIndex)].sleep))
							continue;
						
	                    PhysicsJoint.JointSolver(sceneJoints[j],
	                        sceneBodies[PhysicsUtility.UnpackIdx1(sceneJoints[j].totalIndex)],
	                        sceneBodies[PhysicsUtility.UnpackIdx2(sceneJoints[j].totalIndex)],
	                        1.0f/simDt, flag, (iteration_num + 3));
	                }
				}

	            for (int i = 0; i < iteration_num; i++)
	            {
					// Contact Solver
	                for (int j = 0; j < numPhysicsSolverPair; j++)
	                {
						
						uint index1, index2;
						index1 = PhysicsUtility.UnpackIdx1(solverPair[j].totalIndex);
						index2 = PhysicsUtility.UnpackIdx2(solverPair[j].totalIndex);
						
						if(sceneShapes[sceneBodies[index1].shapeIndex].hint == 0)
						{
							staticDummy.Clear();
							staticDummy.SetBodyKinematic();
							staticDummy.position = solverPair[i].centerA;
							
							Solver.AfterStepOfSolver(
								staticDummy,
								sceneBodies[index2],
		                        ref solverPair[j],
		                        1.0f/simDt, true, iteration_num);	
						}
						else if(sceneShapes[sceneBodies[index2].shapeIndex].hint == 0)
						{
							staticDummy.Clear();
							staticDummy.SetBodyKinematic();
							staticDummy.position = solverPair[i].centerB;
							
							Solver.AfterStepOfSolver(
								sceneBodies[index1],
								staticDummy,
		                        ref solverPair[j],
		                        1.0f/simDt, true, iteration_num);	
						}
						else
						{
		                    Solver.AfterStepOfSolver(
		                        sceneBodies[PhysicsUtility.UnpackIdx1(solverPair[j].totalIndex)],
		                        sceneBodies[PhysicsUtility.UnpackIdx2(solverPair[j].totalIndex)],
		                        ref solverPair[j],
		                        1.0f/simDt, true, iteration_num);		
						}
	                }

					// Joint Solver
	                for (int j = 0; j < numJoint; j++)
	                {
						// check both actor is sleeping ?
						if((sceneBodies[PhysicsUtility.UnpackIdx1(sceneJoints[j].totalIndex)].sleep)
							&& (sceneBodies[PhysicsUtility.UnpackIdx2(sceneJoints[j].totalIndex)].sleep))
							continue;
						
	                    PhysicsJoint.JointSolver(sceneJoints[j],
	                        sceneBodies[PhysicsUtility.UnpackIdx1(sceneJoints[j].totalIndex)],
	                        sceneBodies[PhysicsUtility.UnpackIdx2(sceneJoints[j].totalIndex)],
	                        1.0f/simDt, false, (iteration_num + 3));
	                }
	            }
	        }
	        sw.Stop();
			sw.Print("Solver");

	        // Save Previous Phase ----------------------------------------------------------------------------
	        for (int i = 0; i < numPhysicsSolverPair; i++)
	        {
	            SolverPairCache temp;
	            temp.totalIndex = solverPair[i].totalIndex;
	            temp.resA = solverPair[i].resA;
	            temp.resB = solverPair[i].resB;
	            temp.Pn = solverPair[i].Pn;
	            temp.Pt = solverPair[i].Pt;
	            oldPhysicsSolverPair[i] = temp;
	        }
	
	        numOldPhysicsSolverPair = numPhysicsSolverPair;

			
			// This is good before check sleep
			// Compound Shape support----------------------------------------------------------------------------
			for(int i=0; i<numBody; i++)
			{
				sceneBodies[i].compound = compoundMap[i];

				if(compoundMap[i] != i)
				{
					uint index = compoundMap[i];
					sceneBodies[i].position = sceneBodies[index].position;
					sceneBodies[i].rotation = sceneBodies[index].rotation;
					sceneBodies[i].velocity = sceneBodies[index].velocity;
					sceneBodies[i].angularVelocity = sceneBodies[index].angularVelocity;
					sceneBodies[i].force = sceneBodies[index].force;
					sceneBodies[i].torque = sceneBodies[index].torque;
				}
			}
			
			
			// Check Sleep Status ----------------------------------------------------------------------------
			CheckIslandSleep();	
			
			// Integrate Phase----------------------------------------------------------------------------
            sw.Reset();
            sw.Start();
	        for (int i = 0; i < numBody; i++)
	        {
	            Integrate.IntegratePhase(sceneBodies[i], sceneShapes, simDt, gravity);
	        }
	        sw.Stop();
			sw.Print("Integrate");			

			
			UpdateFuncAfterSim();
			// Increment frame counter
			sceneFrame++;
	

	    }
		
		/// @if LANG_EN
		/// <summary> Add sphere shape to the scene </summary>
        /// <param name="radius"> radius of sphere </param>
        /// <remarks> increment numShape automatically by this function </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> シーンに球形状を追加する </summary>
        /// <param name="radius"> 球の半径 </param>
        /// <remarks> シーンの衝突形状数が自動的にインクリメントされる </remarks>
		/// @endif
       public PhysicsShape AddSphereShape(float radius)
        {
			sceneShapes[numShape] = new PhysicsShape(radius);
			numShape++;
			return sceneShapes[numShape-1];
        }
		
		/// @if LANG_EN
		/// <summary> Add box shape to the scene </summary>
        /// <param name="width"> half width of box </param>
        /// <remarks> width param is half width of box <br />
        /// increment numShape automatically by this function 
        /// </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> シーンにボックス形状を追加する  </summary>
        /// <param name="width"> ボックスの半分の縦横長さ </param>
        /// <remarks> 縦横長さではなく、縦横半分の長さである <br />
        /// シーンの衝突形状数が自動的にインクリメントされる
        /// </remarks>
		/// @endif
		public PhysicsShape AddBoxShape(Vector2 width)
        {
			sceneShapes[numShape] = new PhysicsShape(width);
			numShape++;
			return sceneShapes[numShape-1];
        }
			
		/// @if LANG_EN
		/// <summary> Add convex shape to the scene </summary>
        /// <param name="pos"> point list for convex shape </param>
        /// <param name="num"> num of point list for convex shape </param>
        /// <remarks> the limitation of number of point list containd by convex shape is 30 <br />
        /// increment numShape automatically by this function 
        /// </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> シーンに凸形状を追加する </summary>
        /// <param name="pos"> 凸形状の頂点リスト </param>
        /// <param name="num"> 頂点リストの数 </param>
        /// <remarks> 頂点リストの数の最大は３０個 <br />
        /// シーンの衝突形状数が自動的にインクリメントされる
        /// </remarks></remarks>
		/// @endif
		public PhysicsShape AddConvexShape(Vector2[] pos, int num)
		{
			sceneShapes[numShape] = new PhysicsShape(pos, num);
			numShape++;
			return sceneShapes[numShape-1];
		}
		
		/// @if LANG_EN
		/// <summary> Add convex hull shape comoposed of random points to the scene </summary>
        /// <param name="num"> random point set </param>
        /// <param name="num"> number of random point set </param>
        /// <remarks> the limitation of number of point list containd by convex shape is 30 <br />
        /// increment numShape automatically by this function 
        /// </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> シーンにランダム頂点列による凸包形状を追加する </summary>
        /// <param name="pos"> ランダムな頂点列 </param>
        /// <param name="num"> 頂点数 </param> 
        /// <remarks> 頂点リストの数の最大は３０個 <br />
        /// シーンの衝突形状数が自動的にインクリメントされる
        /// </remarks></remarks>
		/// @endif
		public PhysicsShape AddConvexHullShape(Vector2[] pos, int num)
		{
			sceneShapes[numShape] = PhysicsShape.CreateConvexHull(pos, num);
			numShape++;
			return sceneShapes[numShape-1];
		}
		
		/// @if LANG_EN
		/// <summary> Add chain of line segments to the scene </summary>
        /// <param name="pos"> point list for line shape </param>
        /// <param name="num"> num of point list for convex shape </param>
        /// <remarks> the limitation of number of point list containd by convex shape is 30, and this is only for static rigid body <br />
        /// increment numShape automatically by this function 
        /// </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> シーンに接続したライン形状生成を追加する </summary>
        /// <param name="pos"> ライン形状の頂点リスト </param>
        /// <param name="num"> 頂点リストの数 </param>
        /// <remarks> 頂点リストの数の最大は３０個であり、また、これによって作れるのは静的剛体のみです <br />
        /// シーンの衝突形状数が自動的にインクリメントされる 
        /// </remarks>
		/// @endif
		public PhysicsShape AddLineChainShape(Vector2[] pos, int num)
		{
			sceneShapes[numShape] = new PhysicsShape(pos, num, true);
			numShape++;
			return sceneShapes[numShape-1];
		}
		
		/// @if LANG_EN	
		/// <summary> Add Rigid Body </summary>
        /// <param name="shapeIndex"> index of collision shape </param>
        /// <param name="mass"> mass of rigid body </param>
        /// <returns> reference to this rigid body </returns>
        /// <remarks> increment numBody automatically by this function </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> 剛体をシーンに追加する </summary>
        /// <param name="shapeIndex"> 衝突形状のインデックス </param>
        /// <param name="mass"> 質量 </param>
        /// <returns>　剛体への参照 </returns>
        /// <remarks> シーンの剛体数が自動的にインクリメントされる </remarks>
    	/// @endif
		public PhysicsBody AddBody(int shapeIndex, float mass)
		{
			if((shapeIndex < 0) || (shapeIndex >= numShape))
			{
				Console.WriteLine ("index is out of range");
				return null;
			}
			
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[shapeIndex], mass);
			sceneBodies[numBody].shapeIndex = (uint)shapeIndex;
			numBody++;
			return sceneBodies[numBody-1];
		}
		
		/// @if LANG_EN	
		/// <summary> Add Rigid Body </summary>
        /// <param name="shape"> reference to collision shape </param>
        /// <param name="mass"> mass of rigid body </param>
        /// <returns> reference to this rigid body </returns>
        /// <remarks> increment numBody automatically by this function </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> 剛体をシーンに追加する </summary>
        /// <param name="shape"> 衝突形状へのリファレンス </param>
        /// <param name="mass"> 質量 </param>
        /// <returns>　剛体への参照 </returns>
        /// <remarks> シーンの剛体数が自動的にインクリメントされる </remarks>
    	/// @endif
		public PhysicsBody AddBody(PhysicsShape shape, float mass)
		{
			if(shape == null)
				return null;
			
			int shapeIndex = 0;
			
			// To check the index by using reference pointer
			if((shapeIndex = GetShapeIndex(shape)) < 0)
				return null;
			else
				return AddBody(shapeIndex, mass);	
		}
		
		/// @if LANG_EN
		/// <summary> Merge of rigid body </summary>
		/// <remarks>
        /// To merge multiple objects (compound object), bodyList should be given in manner of the ascending order
        /// </remarks>
        /// <param name="bodyList"> given rigid body list of the scene</param>
       	/// <param name="num"> given number of rigid sceneBodies to be attached </param>
        /// <param name="flag"> whether rearrange the position of merged rigid body </param>
  		/// @endif
		/// @if LANG_JA
		/// <summary> 剛体のマージ </summary>
		/// <remarks>
        /// 複数の剛体 (コンパウンド形状)をマージするために、インデックスが昇順で与えられている
        /// </remarks>
        /// <param name="bodyList"> シーンの剛体リスト</param>
       	/// <param name="num"> マージするための剛体の数 </param>
        /// <param name="flag"> 剛体の位置をマージ後に再調整するかどうか </param>
  		/// @endif
		public Vector2 MergeCompound(PhysicsBody[] bodyList, int num, bool flag = true)
		{
			uint[] indexList = new uint[num];
			
			for(int i=0; i<num; i++)
			{
				for(int j=0; j<numBody; j++)
				{
					if(bodyList[i] == sceneBodies[j])
					{
						indexList[i] = (uint)j;
						break;
					}
					
					// cannot find this body
					if(j == (numBody-1))
					{
						Console.WriteLine ("One of elements cannot be found");
						return new Vector2(0, 0);
					}
				}
			}
			
			return PhysicsBody.MergeBody(sceneBodies, indexList, (uint)num, sceneShapes, compoundMap, flag);      
			
		}
		
		/// @if LANG_EN	
		/// <summary> Add joint </summary>
        /// <param name="bodyIndex1"> index of one of rigid body pair </param>
        /// <param name="bodyIndex2"> index of the other of rigid body pair </param>
        /// <param name="anchorPos"> anchor position of joint </param>
        /// <returns> reference to this joint </returns>
        /// <remarks> increment numJoint automatically by this function </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> ジョイントをシーンに追加する </summary>
        /// <param name="bodyIndex1"> 剛体ペアの一方のインデックス </param>
        /// <param name="bodyIndex2"> 剛体ペアのもう一方のインデックス</param>
        /// <param name="anchorPos"> ジョイントのアンカーポイント </param>
        /// <returns>　ジョイントへの参照 </returns>
        /// <remarks> シーンのジョイント数が自動的にインクリメントされる </remarks>
    	/// @endif
		public PhysicsJoint AddJoint(int bodyIndex1, int bodyIndex2, Vector2 anchorPos)
		{
			if((bodyIndex1 < 0) || (bodyIndex1 >= numBody))
			{
				Console.WriteLine ("index is out of range");
				return null;
			}
			if((bodyIndex2 < 0) || (bodyIndex2 >= numBody))
			{
				Console.WriteLine ("index is out of range");
				return null;
			}
			
			PhysicsBody b1 = sceneBodies[bodyIndex1];
			PhysicsBody b2 = sceneBodies[bodyIndex2];
			sceneJoints[numJoint] = new PhysicsJoint(b1, b2, anchorPos, (uint)bodyIndex1, (uint)bodyIndex2); 
			numJoint++;
			return sceneJoints[numJoint-1];
		}
		
		/// @if LANG_EN	
		/// <summary> Add joint </summary>
        /// <param name="body1"> reference to one of rigid body pair </param>
        /// <param name="body2"> reference to the other of rigid body pair </param>
        /// <param name="anchorPos"> anchor position of joint </param>
        /// <returns> reference to this joint </returns>
        /// <remarks> increment numJoint automatically by this function </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> ジョイントをシーンに追加する </summary>
        /// <param name="body1"> 剛体ペアの一方のインデックス </param>
        /// <param name="body2"> 剛体ペアのもう一方のインデックス</param>
        /// <param name="anchorPos"> ジョイントのアンカーポイント </param>
        /// <returns>　ジョイントへの参照 </returns>
        /// <remarks> シーンのジョイント数が自動的にインクリメントされる </remarks>
    	/// @endif
		public PhysicsJoint AddJoint(PhysicsBody body1, PhysicsBody body2, Vector2 anchorPos)
		{
			if((body1 == null) || (body2 == null))
				return null;
			
			int bodyIndex1 = 0, bodyIndex2 = 0;
			
			// To check the index by using reference pointer
			if((bodyIndex1 = GetBodyIndex(body1)) < 0)
				return null;

			if((bodyIndex2 = GetBodyIndex(body2)) < 0)
				return null;
			
			return AddJoint(bodyIndex1, bodyIndex2, anchorPos);
		}
		
		/// @if LANG_EN	
		/// <summary> Add joint </summary>
        /// <param name="bodyIndex1"> index of one of rigid body pair </param>
        /// <param name="bodyIndex2"> index of the other of rigid body pair </param>
        /// <param name="anchorPos"> anchor position of joint </param>
        /// <returns> reference to this joint </returns>
        /// <remarks> increment numJoint automatically by this function </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> ジョイントをシーンに追加する </summary>
        /// <param name="bodyIndex1"> 剛体ペアの一方のインデックス </param>
        /// <param name="bodyIndex2"> 剛体ペアのもう一方のインデックス</param>
        /// <param name="anchorPos"> ジョイントのアンカーポイント </param>
        /// <returns>　ジョイントへの参照 </returns>
        /// <remarks> シーンのジョイント数が自動的にインクリメントされる </remarks>
    	/// @endif
		public PhysicsSpring AddSpring(int bodyIndex1, int bodyIndex2, Vector2 anchorPos1, Vector2 anchorPos2)
		{
			if((bodyIndex1 < 0) || (bodyIndex1 >= numBody))
			{
				Console.WriteLine ("index is out of range");
				return null;
			}
			if((bodyIndex2 < 0) || (bodyIndex2 >= numBody))
			{
				Console.WriteLine ("index is out of range");
				return null;
			}
			
			PhysicsBody b1 = sceneBodies[bodyIndex1];
			PhysicsBody b2 = sceneBodies[bodyIndex2];
			sceneSprings[numSpring] = new PhysicsSpring(b1, b2, anchorPos1, anchorPos2, (uint)bodyIndex1, (uint)bodyIndex2); 
			numSpring++;
			return sceneSprings[numSpring-1];
		}
		
		/// @if LANG_EN	
		/// <summary> Add spring </summary>
        /// <param name="body1"> reference to one of rigid body pair </param>
        /// <param name="body2"> reference to the other of rigid body pair </param>
        /// <param name="anchorPos1"> one of anchor position of joint </param>
        /// <param name="anchorPos2"> the other of anchor positions of spring </param>
        /// <returns> reference to this spring </returns>
        /// <remarks> increment numSpring automatically by this function </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> スプリングをシーンに追加する </summary>
        /// <param name="body1"> 剛体ペアの一方のインデックス </param>
        /// <param name="body2"> 剛体ペアのもう一方のインデックス</param>
        /// <param name="anchorPos1"> スプリングのアンカーポイントの１つ </param>
        /// <param name="anchorPos2"> スプリングのアンカーポイントのもう１つ </param>
        /// <returns>　スプリングへの参照 </returns>
        /// <remarks> シーンのスプリング数が自動的にインクリメントされる </remarks>
    	/// @endif
		public PhysicsSpring AddSpring(PhysicsBody body1, PhysicsBody body2, Vector2 anchorPos1, Vector2 anchorPos2)
		{
			if((body1 == null) || (body2 == null))
				return null;
			
			int bodyIndex1 = 0, bodyIndex2 = 0;

			// To check the index by using reference pointer
			if((bodyIndex1 = GetBodyIndex(body1)) < 0)
				return null;

			if((bodyIndex2 = GetBodyIndex(body2)) < 0)
				return null;
			
			return AddSpring(bodyIndex1, bodyIndex2, anchorPos1, anchorPos2); 

		}
		
		
		/// @if LANG_EN	
		/// <summary> DeleteJoint </summary>
        /// <param name="index1"> index of joint </param>
        /// <returns> total number of joints of scene after removing the joint </returns>
    	/// @endif
		/// @if LANG_JA
		/// <summary> ジョイント削除 </summary>
        /// <param name="index"> ジョイントのインデックス </param>
        /// <returns>　削除後のジョイントの総数 </returns>
    	/// @endif
		public int DeleteJoint(int index)
		{
			if((numJoint > 0) && (index < numJoint))
			{
				// wake up if dynamic object is sleeping
				PhysicsBody body1 = sceneBodies[PhysicsUtility.UnpackIdx1(sceneJoints[index].totalIndex)];
				PhysicsBody body2 = sceneBodies[PhysicsUtility.UnpackIdx2(sceneJoints[index].totalIndex)];
				
				body1.WakeUpKinematic();
				body2.WakeUpKinematic();
				
				for(int i=index; i<numJoint-1; i++)
				{
					//sceneJoints[i].CopyJoint(sceneJoints[i+1]); 
					sceneJoints[i] = sceneJoints[i+1];
				}
				
				sceneJoints[numJoint-1] = null;
				numJoint = numJoint-1;
			}
			return numJoint;
		}
		
		/// @if LANG_EN	
		/// <summary> DeleteJoint </summary>
        /// <param name="joint"> reference to joint </param>
        /// <returns> total number of joints of scene after removing the joint </returns>
    	/// @endif
		/// @if LANG_JA
		/// <summary> ジョイント削除 </summary>
        /// <param name="joint"> ジョイントへの参照 </param>
        /// <returns>　削除後のジョイントの総数 </returns>
    	/// @endif
		public int DeleteJoint(PhysicsJoint joint)
		{
			int jointIndex = 0;
			
			// To check the index of rigid body by using reference pointer
			if((jointIndex = GetJointIndex(joint)) < 0)
				return numJoint;
			else
				return DeleteJoint(jointIndex);	
		}
		
		/// @if LANG_EN	
		/// <summary> DeleteSpring </summary>
        /// <param name="index"> index of spring </param>
        /// <returns> total number of springs of scene after removing the spring </returns>
    	/// @endif
		/// @if LANG_JA
		/// <summary> スプリング削除 </summary>
        /// <param name="index"> スプリングのインデックス </param>
        /// <returns>　削除後のスプリングの総数 </returns>
    	/// @endif
		public int DeleteSpring(int index)
		{
			if((numSpring > 0) && (index < numSpring))
			{
				// wake up if dynamic object is sleeping
				PhysicsBody body1 = sceneBodies[PhysicsUtility.UnpackIdx1(sceneSprings[index].totalIndex)];
				PhysicsBody body2 = sceneBodies[PhysicsUtility.UnpackIdx2(sceneSprings[index].totalIndex)];
				
				body1.WakeUpKinematic();			
				body2.WakeUpKinematic();
			
				for(int i=index; i<numSpring-1; i++)
				{
					//sceneSprings[i].CopySpring(sceneSprings[i+1]); 
					sceneSprings[i] = sceneSprings[i+1];
				}
				
				sceneSprings[numSpring-1] = null;
				numSpring = numSpring-1;
			}
			return numSpring;
		}
		
		/// @if LANG_EN	
		/// <summary> DeleteSpring </summary>
        /// <param name="spring"> reference to spring </param>
        /// <returns> total number of springs of scene after removing the spring </returns>
    	/// @endif
		/// @if LANG_JA
		/// <summary> スプリング削除 </summary>
        /// <param name="spring"> スプリングへの参照 </param>
        /// <returns>　削除後のスプリングの総数 </returns>
    	/// @endif
		public int DeleteSpring(PhysicsSpring spring)
		{
			int springIndex = 0;
			
			// To check the index of rigid body by using reference pointer
			if((springIndex = GetSpringIndex(spring)) < 0)
				return numSpring;
			else
				return DeleteSpring(springIndex);	
		}
		
		/// @if LANG_EN	
		/// <summary> DeleteBody </summary>
        /// <param name="index"> index of rigid body </param>
        /// <returns> total number of joints of scene </returns>
        /// <remarks> This will requires rearrangment of internal cache data used by simulation. <br />
        /// When you want to remove many objects within one frame, you should be careful about the performance,
        ///  and it is recommended not to remove many objects at once as much as possible and try to consider other ways such that
        /// unused objects are put outside the area of simulation <br />
        /// When compound rigid body is given, all rigid bodies related to it are removed. </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> 剛体削除 </summary>
        /// <param name="index"> 剛体インデックス </param>
        /// <returns>　剛体の総数 </returns>
        /// <remarks> シミュレーション内で使用しているキャッシュデータの再構築が必要になるために計算負荷がかかります<br />
        /// 同フレームで多くの剛体を削除する場合は注意が必要で、削除でなくシミュレーション領域外へ置くなどの方法も検討してください<br />
        /// Compound剛体を削除する場合は、compound剛体に関わる剛体が全て削除されます </remarks>
    	/// @endif
		public int DeleteBody(int index)
		{			
			// Index is out of range
			if((index < 0) || (index >= numBody))
			{
				return numBody;
			}
			
			// At first, should check this is compound rigid body ?
			// If it is compound rigid body, it is necessary to delete all bodies which are related to the compound
			bool isCompound = false;
			
			for(int i=0; i<numBody; i++)
			{
				// Other object's target is index
				if((compoundMap[i] == index) && (i != index))
				{
					isCompound = true;
					break;
				}
			}
			
			if(compoundMap[index] != index)
				isCompound = true;
			   	
			// For the safety userData reference should be null
			sceneBodies[index].userData = null;
			
			// This is not comound rigid body
			if(isCompound == false)
			{
				
				for(int i=0; i<numBody; i++)
				{
					sceneBodies[i].compound = compoundMap[i];
					//Console.WriteLine ("compound " + i + "->" + sceneBodies[i].compound);
				}
				
				DeleteOneBody(index);
				
				for(int i=0; i<numBody; i++)
				{	
					compoundMap[i] = sceneBodies[i].compound;
					//Console.WriteLine ("compound " + i + "->" + sceneBodies[i].compound);
				}
				
				return numBody;
				
			}
			else
			{
				
				// Retarget
				uint target_index = compoundMap[index];
				
				int oldNumBody = numBody;
				int removedBody = 0;
			
				for(int i=0; i<numBody; i++)
				{
					sceneBodies[i].compound = compoundMap[i];
					//Console.WriteLine ("compound " + i + "->" + sceneBodies[i].compound);
				}
					
				for(int i=0; i<oldNumBody; i++)
				{
					if(compoundMap[i] == target_index)
					{
						DeleteOneBody(i-removedBody);
						removedBody++;
					}	
				}
				
				numBody = oldNumBody - removedBody;
				
				for(int i=0; i<numBody; i++)
				{	
					compoundMap[i] = sceneBodies[i].compound;
					//Console.WriteLine ("compound " + i + "->" + sceneBodies[i].compound);
				}
				
				return numBody;
			}
			
		}
		
		/// @if LANG_EN	
		/// <summary> DeleteBody </summary>
        /// <param name="body"> reference to rigid body </param>
        /// <returns> total number of joints of scene </returns>
        /// <remarks> This will requires rearrangment of internal cache data used by simulation. <br />
        /// When you want to remove many objects within one frame, you should be careful about the performance,
        ///  and it is recommended not to remove many objects at once as much as possible and try to consider other ways such that
        /// unused objects are put outside the area of simulation <br />
        /// When compound rigid body is given, all rigid bodies related to it are removed. </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> 剛体削除 </summary>
        /// <param name="body"> 剛体への参照 </param>
        /// <returns>　剛体の総数 </returns>
        /// <remarks> シミュレーション内で使用しているキャッシュデータの再構築が必要になるために計算負荷がかかります<br />
        /// 同フレームで多くの剛体を削除する場合は注意が必要で、削除でなくシミュレーション領域外へ置くなどの方法も検討してください<br />
        /// Compound剛体を削除する場合は、compound剛体に関わる剛体が全て削除されます </remarks>
    	/// @endif
		public int DeleteBody(PhysicsBody body)
		{			
			int bodyIndex = 0;
			
			// To check the index of rigid body by using reference pointer
			if((bodyIndex = GetBodyIndex(body)) < 0)
				return numBody;
			else
				return DeleteBody(bodyIndex);
		}
		
		/// @if LANG_EN	
		/// <summary> DeleteBody </summary>
        /// <param name="index1"> index of rigid body </param>
        /// <returns> total number of joints of scene </returns>
        /// <remarks> This will requires rearrangment of internal cache data used by simulation.
        /// It is recommended not to use as much as possible, and try to figure out other ways such that
        /// unused objects are put outside the area of simulation </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> 剛体削除 </summary>
        /// <param name="index"> 剛体インデックス </param>
        /// <returns>　剛体の総数 </returns>
        /// <remarks> シミュレーション内で使用しているキャッシュデータの再構築が必要になるため、
        /// 計算負荷がかかります。可能な限り剛体を削除せず、シミュレーション領域外へ置くなどの回避策を検討してください。</remarks>
    	/// @endif
		private int DeleteOneBody(int index)
		{
			// Wake up near objects
			{	 
				for(int i=0; i<numBody; i++)
				{
					if(Broadphase.AABB_SimpleBroadPhase(sceneBodies[index], sceneBodies[i]) && sceneBodies[i].IsDynamic())
					{
						sceneBodies[i].WakeUpDynamic();
					}
				}
			}
				
			// At first check springs which may contain sceneBodies[index] and delete those
			for(int i=0; i<numSpring; i++)
			{
				// wake up if dynamic object is sleeping
				uint index1 = PhysicsUtility.UnpackIdx1(sceneSprings[i].totalIndex);
				uint index2 = PhysicsUtility.UnpackIdx2(sceneSprings[i].totalIndex);
				                                     
				if(index1 == index)
				{
					// Wakup body
					sceneBodies[index2].WakeUpDynamic();
					
					// Delete this spring
					DeleteSpring (i);
					i--;
				}
				else if(index2 == index)
				{
					// Wakup body
					sceneBodies[index1].WakeUpDynamic();
					
					// Delete this spring
					DeleteSpring (i);
					i--;
				}
			}
			
			// At first check joints which may contain sceneBodies[index] and delete those
			for(int i=0; i<numJoint; i++)
			{
				// wake up if dynamic object is sleeping
				uint index1 = PhysicsUtility.UnpackIdx1(sceneJoints[i].totalIndex);
				uint index2 = PhysicsUtility.UnpackIdx2(sceneJoints[i].totalIndex);
				                                     
				if(index1 == index)
				{
					// Wakup body
					sceneBodies[index2].WakeUpDynamic();
					
					// Delete this joint
					DeleteJoint (i);
					i--;
				}
				else if(index2 == index)
				{
					// Wakup body
					sceneBodies[index1].WakeUpDynamic();
					
					// Delete this joint
					DeleteJoint (i);
					i--;
				}
			}
			
			// Then delete this rigid body
			{
				// Update compound map
				for(int i=0; i<numBody; i++)
				{
					if(sceneBodies[i].compound > index)
						sceneBodies[i].compound--;
				}
				
				for(int i=index; i<numBody-1; i++)
				{
					//sceneBodies[i].CopyBody(sceneBodies[i+1]);
					sceneBodies[i] = sceneBodies[i+1];
				}
				
				sceneBodies[numBody-1] = null;
			}
	
			// If there is no rigid body anymore
			if(numBody == 1)
			{
				numBody = 0;
				return numBody;
			}
	
		
			// Update all information related cache data
			uint[] indexMap = new uint[maxBody];
			
			for(uint i=0; i<numBody; i++)
			{
				if(i < index)
					indexMap[i] = i;
				else
					indexMap[i] = i-1;
			}
	
			// Update sceneSprings
			for(int i=0; i<numSpring; i++)
			{
				uint index1 = PhysicsUtility.UnpackIdx1(sceneSprings[i].totalIndex);
				uint index2 = PhysicsUtility.UnpackIdx2(sceneSprings[i].totalIndex);
				sceneSprings[i].totalIndex = PhysicsUtility.PackIdx(indexMap[index1], indexMap[index2]);
			}
			
			// Update sceneJoints
			for(int i=0; i<numJoint; i++)
			{
				uint index1 = PhysicsUtility.UnpackIdx1(sceneJoints[i].totalIndex);
				uint index2 = PhysicsUtility.UnpackIdx2(sceneJoints[i].totalIndex);
				sceneJoints[i].totalIndex = PhysicsUtility.PackIdx(indexMap[index1], indexMap[index2]);
			}

			// Update broadPair ?
			if(true){
				int newBroadPair = 0;
				
				for(int i=0; i<numBroadPair; i++)
				{
					uint index1 = PhysicsUtility.UnpackIdx1(broadPair[i]);
					uint index2 = PhysicsUtility.UnpackIdx2(broadPair[i]);
	
					// delete these pair
					if((index1 == index) || (index2 == index))
						continue;
					
					broadPair[newBroadPair++] = PhysicsUtility.PackIdx(indexMap[index1], indexMap[index2]);
				}
				
				numBroadPair = newBroadPair;
			}

			// Update satPair  ?
			bool switchFlag = false;
			
			if(switchFlag){
				
				int newBroadPairSum = 0;
				
				for(int i=0; i<numBroadPairSum; i++)
				{
					if(satPair[i].totalIndex == 0xFFFFFFFF)
						continue;
					
					uint index1 = PhysicsUtility.UnpackIdx1(satPair[i].totalIndex);
					uint index2 = PhysicsUtility.UnpackIdx2(satPair[i].totalIndex);
					
					// delete these pair
					if((index1 == index) || (index2 == index))
						continue;
					
					satPair[newBroadPairSum] = satPair[i];
					satPair[newBroadPairSum].totalIndex = PhysicsUtility.PackIdx(indexMap[index1], indexMap[index2]);
					newBroadPairSum++;
					
				}
				
				numBroadPairSum = newBroadPairSum;
				
			}
	

			// Update oldPhysicsSolverPair ?
			if(true){
				int newOldPhysicsSolverPair = 0;
				
				for(int i=0; i<numOldPhysicsSolverPair; i++)
				{
					uint index1 = PhysicsUtility.UnpackIdx1(oldPhysicsSolverPair[i].totalIndex);
					uint index2 = PhysicsUtility.UnpackIdx2(oldPhysicsSolverPair[i].totalIndex);
					
					// delete these pair
					if((index1 == index) || (index2 == index))
						continue;
					
					oldPhysicsSolverPair[newOldPhysicsSolverPair] = oldPhysicsSolverPair[i];
					oldPhysicsSolverPair[newOldPhysicsSolverPair].totalIndex = PhysicsUtility.PackIdx(indexMap[index1], indexMap[index2]);
					newOldPhysicsSolverPair++;
				}
				
				numOldPhysicsSolverPair = newOldPhysicsSolverPair;
			}
			
			// Update solverPair ?
			if(true){
				int newPhysicsSolverPair = 0;
				
				for(int i=0; i<numPhysicsSolverPair; i++)
				{
					uint index1 = PhysicsUtility.UnpackIdx1(solverPair[i].totalIndex);
					uint index2 = PhysicsUtility.UnpackIdx2(solverPair[i].totalIndex);
					
					// delete these pair
					if((index1 == index) || (index2 == index))
						continue;
					
					solverPair[newPhysicsSolverPair] = solverPair[i];
					solverPair[newPhysicsSolverPair].totalIndex = PhysicsUtility.PackIdx(indexMap[index1], indexMap[index2]);
					newPhysicsSolverPair++;
				}
				
				numPhysicsSolverPair = newPhysicsSolverPair;
			}
			
			// Update ignorePair ?
			if(true){
				int newIgnorePair = 0;
				
				for(int i=0; i<numIgnorePair; i++)
				{
					uint index1 = PhysicsUtility.UnpackIdx1(ignorePair[i]);
					uint index2 = PhysicsUtility.UnpackIdx2(ignorePair[i]);
	
					// delete these pair
					if((index1 == index) || (index2 == index))
						continue;
					
					ignorePair[newIgnorePair++] = PhysicsUtility.PackIdx(indexMap[index1], indexMap[index2]);
				}
				
				numIgnorePair = newIgnorePair;
			}
		
			numBody = numBody-1;
			
			return numBody;
		}

		
		/// @if LANG_EN	
		/// <summary> Check whether there is a collision between index1 and index2 </summary>
        /// <param name="index1"> one of rigid body pair </param>
        /// <param name="index2"> the other of rigid body pair </param>
        /// <returns> the result is true if there is a collision </returns>
    	/// @endif
		/// @if LANG_JA
		/// <summary> ２つの剛体の間に衝突があったかどうかの記録を調べる </summary>
        /// <param name="index1"> 剛体ペアの一方 </param>
        /// <param name="index2"> 剛体ペアのもう一方 </param>
        /// <returns>　剛体ペアに衝突があった記録があれば真を返す </returns>
    	/// @endif
        public bool QueryContact(uint index1, uint index2)
        {
            uint index;

            if (index1 <= index2)
                index = PhysicsUtility.PackIdx(index1, index2);
            else
                index = PhysicsUtility.PackIdx(index2, index1);

            for (int i = 0; i < numPhysicsSolverPair; i++)
            {
                PhysicsSolverPair ob = solverPair[i];
				
                if (index == (int)ob.totalIndex)
                    return true;
				else if(ob.totalIndex > index)
					return false;
            }

            return false;
        }
		
		/// @if LANG_EN	
		/// <summary> Check whether there is a collision between index1 and index2 </summary>
        /// <param name="body1"> one of rigid body pair </param>
        /// <param name="body2"> the other of rigid body pair </param>
        /// <returns> the result is true if there is a collision </returns>
    	/// @endif
		/// @if LANG_JA
		/// <summary> ２つの剛体の間に衝突があったかどうかの記録を調べる </summary>
        /// <param name="body1"> 剛体ペアの一方 </param>
        /// <param name="body2"> 剛体ペアのもう一方 </param>
        /// <returns>　剛体ペアに衝突があった記録があれば真を返す </returns>
    	/// @endif
        public bool QueryContact(PhysicsBody body1, PhysicsBody body2)
        {
		
			if((body1 == null) || (body2 == null))
				return false;
			
			int bodyIndex1 = 0, bodyIndex2 = 0;
			
			// To check the index of rigid body by using reference pointer
			if((bodyIndex1 = GetBodyIndex(body1)) < 0)
				return false;
			
			if((bodyIndex2 = GetBodyIndex(body2)) < 0)
				return false;	
			
			return QueryContact((uint)bodyIndex1, (uint)bodyIndex2);
        }
				
		
		/// @if LANG_EN	
		/// <summary> Check whether there is a collision between trigger rigid body and others </summary>
        /// <param name="index"> index of trigger rigid body</param>
        /// <param name="indexlist"> index list which contact with the trigger will be saved </param>
        /// <returns> number of rigid bodies which contact with the trigger </returns>
    	/// @endif
		/// @if LANG_JA
		/// <summary> トリガー剛体と他の剛体の間に衝突があったかチェックする </summary>
        /// <param name="index"> トリガー剛体のインデックス </param>
        /// <param name="indexlist"> トリガー剛体との衝突を検出された剛体インデックスが格納される </param>
        /// <returns>　トリガー剛体との衝突を検出された剛体の数 </returns>
    	/// @endif
		public int QueryContactTrigger(uint index, uint[] indexlist)
        {
            if(sceneBodies[index].type != BodyType.Trigger)
				return 0;
			
			int result_num = 0;
			
            for (int i = 0; i < numPhysicsSolverPair; i++)
            {
                PhysicsSolverPair ob = solverPair[i];
				uint index1 = PhysicsUtility.UnpackIdx1(ob.totalIndex);
				uint index2 = PhysicsUtility.UnpackIdx2(ob.totalIndex);
				
				if(index1 == index)
				{
					if(sceneBodies[index2].type != BodyType.Static)
						indexlist[result_num++] = index2;
				}
				else if(index2 == index)
				{
					if(sceneBodies[index1].type != BodyType.Static)
						indexlist[result_num++] = index1;
				}
            }

            return result_num;
        }
		
		/// @if LANG_EN
		/// <summary> Get index of collision shape </summary>  
        /// <param name="shape"> collision shape </param>
        /// <returns> if it is not found, -1 will be returned </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> 衝突形状のインデックスを調べる </summary> 
        /// <param name="shape"> 衝突形状</param>
        /// <returns> 見つからない時は-1が返される </returns>
        /// @endif
		public int GetShapeIndex(PhysicsShape shape)
		{
			for(int i=0; i<numShape; i++)
			{
				if(sceneShapes[i] == shape)
				{
					return i;
				}
			}
			
			// cannot find this index
			return -1;
		}
		
		/// @if LANG_EN
		/// <summary> Get index of rigid body </summary>  
        /// <param name="body"> rigid body</param>
        /// <returns> if it is not found, -1 will be returned </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> 剛体のインデックスを調べる </summary> 
        /// <param name="body"> 剛体</param>
        /// <returns> 見つからない時は-1が返される </returns>
        /// @endif
		public int GetBodyIndex(PhysicsBody body)
		{
			for(int i=0; i<numBody; i++)
			{
				if(sceneBodies[i] == body)
				{
					return i;
				}
			}
			
			// cannot find this index
			return -1;
		}
		
		/// @if LANG_EN
		/// <summary> Get index of joint </summary>  
        /// <param name="joint"> joint </param>
        /// <returns> if it is not found, -1 will be returned </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> ジョイントのインデックスを調べる </summary>
		/// <param name="joint"> ジョイント </param>
        /// <returns> 見つからない時は-1が返される </returns> 
        /// @endif
		public int GetJointIndex(PhysicsJoint joint)
		{
			for(int i=0; i<numJoint; i++)
			{
				if(sceneJoints[i] == joint)
				{
					return i;
				}
			}
			
			// cannot find this index
			return -1;
		}
		
		/// @if LANG_EN
		/// <summary> Get index of spring </summary>  
        /// <param name="spring"> spring </param>
        /// <returns> if it is not found, -1 will be returned </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> スプリングのインデックスを調べる </summary> 
        /// <param name="spring"> スプリング </param>
        /// <returns> 見つからない時は-1が返される </returns>
        /// @endif
		public int GetSpringIndex(PhysicsSpring spring)
		{
			for(int i=0; i<numSpring; i++)
			{
				if(sceneSprings[i] == spring)
				{
					return i;
				}
			}
			
			// cannot find this index
			return -1;
		}
		
		/// @if LANG_EN
		/// <summary> Get collilsion shape </summary>  
        /// <param name="index"> index of shape </param>
        /// <returns> if it is not found, null will be returned </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> 衝突形状を得る </summary> 
        /// <param name="index"> 衝突形状のインデックス</param>
        /// <returns> 見つからない時はnullが返される </returns>
        /// @endif
		public PhysicsShape GetShape(int index)
		{
			if((index < 0) || (index >= numShape))
				return null;
			else
				return sceneShapes[index];
		}
		
		/// @if LANG_EN
		/// <summary> Get rigid body </summary>  
        /// <param name="index"> index of rigid body </param>
        /// <returns> if it is not found, null will be returned </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> 剛体を得る </summary> 
        /// <param name="index"> 剛体のインデックス</param>
        /// <returns> 見つからない時はnullが返される </returns>
        /// @endif
		public PhysicsBody GetBody(int index)
		{
			if((index < 0) || (index >= numBody))
				return null;
			else
				return sceneBodies[index];
		}
		
		/// @if LANG_EN
		/// <summary> Get joint </summary>  
        /// <param name="index"> index of joint </param>
        /// <returns> if it is not found, null will be returned </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> ジョイントを得る </summary> 
        /// <param name="index"> ジョイントのインデックス</param>
        /// <returns> 見つからない時はnullが返される </returns>
        /// @endif
		public PhysicsJoint GetJoint(int index)
		{
			if((index < 0) || (index >= numJoint))
				return null;
			else
				return sceneJoints[index];
		}
		
		/// @if LANG_EN
		/// <summary> Get spring </summary>  
        /// <param name="index"> index of spring </param>
        /// <returns> if it is not found, null will be returned </returns>
        /// @endif
		/// @if LANG_JA
		/// <summary> スプリングを得る </summary> 
        /// <param name="index"> スプリングのインデックス</param>
        /// <returns> 見つからない時はnullが返される </returns>
        /// @endif
		public PhysicsSpring GetSpring(int index)
		{
			if((index < 0) || (index >= numSpring))
				return null;
			else
				return sceneSprings[index];
		}	
			
		/// @if LANG_EN
		/// <summary> Add the pair to ignore list </summary>
        /// <param name="index1"> one of the pair </param>   
        /// <param name="index2"> the other of the pair</param>
        /// @endif
		/// @if LANG_JA
		/// <summary> 衝突を無視するペアをリストに加える </summary>
        /// <param name="index1"> 剛体ペアの一方 </param>   
        /// <param name="index2"> 剛体ペアのもう一方</param>
        /// @endif
		public void AddIgnorePair(uint index1, uint index2)
		{
			uint index;
			
			if(index1 < index2)
				index = PhysicsUtility.PackIdx(index1, index2);
			else
				index = PhysicsUtility.PackIdx(index2, index1);
			
			int insert_index = 0;
			
			for(insert_index=0; insert_index<numIgnorePair; insert_index++)
			{
				if(index < ignorePair[insert_index])
					break;
			}

			numIgnorePair++;
			
			for(int i=(numIgnorePair-2); i>=insert_index; i--)
				ignorePair[i+1] = ignorePair[i];
			
			ignorePair[insert_index] = index;
        }
		
		/// @if LANG_EN
		/// <summary> Add the pair to ignore list </summary>
        /// <param name="body1"> one of the pair </param>   
        /// <param name="body2"> the other of the pair</param>
        /// @endif
		/// @if LANG_JA
		/// <summary> 衝突を無視するペアをリストに加える </summary>
        /// <param name="body1"> 剛体ペアの一方 </param>   
        /// <param name="body2"> 剛体ペアのもう一方</param>
        /// @endif
		public void AddIgnorePair(PhysicsBody body1, PhysicsBody body2)
		{
			
			if((body1 == null) || (body2 == null))
				return;
			
			int bodyIndex1 = 0, bodyIndex2 = 0;
			
			// To check the index of rigid body by using reference pointer
			if((bodyIndex1 = GetBodyIndex(body1)) < 0)
				return;
			
			if((bodyIndex2 = GetBodyIndex(body2)) < 0)
				return;	
			
			AddIgnorePair((uint)bodyIndex1, (uint)bodyIndex2);
        }
				
		/// @if LANG_EN
		/// <summary> Remove the pair from ignore list </summary>
        /// <param name="index1"> one of the pair </param>   
        /// <param name="index2"> the other of the pair</param>
        /// @endif
		/// @if LANG_JA
		/// <summary> 衝突を無視するペアをリストから削除する </summary>
        /// <param name="index1"> 剛体ペアの一方  </param>   
        /// <param name="index2"> 剛体ペアのもう一方</param>
        /// @endif
		public void RemoveIgnorePair(uint index1, uint index2)
		{
			uint index;
						
			// cannot find the pair
			if(index1 >= numBody)
				return;
			else if(index2 >= numBody)
				return;
			
			if(index1 < index2)
				index = PhysicsUtility.PackIdx(index1, index2);
			else
				index = PhysicsUtility.PackIdx(index2, index1);
			
			uint insert_index = 0;
			
			for(insert_index=0; insert_index<numIgnorePair; insert_index++)
			{
				if(ignorePair[insert_index] == index)
					break;
			}
			
			// cannot find the pair
			if(insert_index == numIgnorePair)
				return;

			for(uint i=insert_index; i<numIgnorePair-1; i++)
				ignorePair[i] = ignorePair[i+1];
					
			numIgnorePair--;
        }
		
		/// @if LANG_EN
		/// <summary> Remove the pair from ignore list </summary>
        /// <param name="body1"> one of the pair </param>   
        /// <param name="body2"> the other of the pair</param>
        /// @endif
		/// @if LANG_JA
		/// <summary> 衝突を無視するペアをリストから削除する </summary>
        /// <param name="body1"> 剛体ペアの一方  </param>   
        /// <param name="body2"> 剛体ペアのもう一方</param>
        /// @endif
		public void RemoveIgnorePair(PhysicsBody body1, PhysicsBody body2)
		{
			if((body1 == null) || (body2 == null))
				return;
			
			int bodyIndex1 = 0, bodyIndex2 = 0;
			
			// To check the index of rigid body by using reference pointer
			if((bodyIndex1 = GetBodyIndex(body1)) < 0)
				return;
			
			if((bodyIndex2 = GetBodyIndex(body2)) < 0)
				return;	
			
			RemoveIgnorePair((uint)bodyIndex1, (uint)bodyIndex2);
        }
		
		/// @if LANG_EN
		/// <summary> if outside of the main area, 
		/// object will not be considered to take care </summary>
        /// <param name="body"> give rigid body </param>
        /// <returns> whether it exists out side of the main area </returns> 
        /// @endif
		/// @if LANG_JA
		/// <summary>　シミュレーションエリアを出たかどうかをチェック、出ていたらシミュレーションに考慮されない </summary>
        /// <param name="body">　剛体 </param>
        /// <returns> シミュレーションエリア外なら真を返す </returns> 
        /// @endif
        public bool CheckOutOfBound(PhysicsBody body)
        {
            if (body.position.X < sceneMin.X) return true;
            if (body.position.X > sceneMax.X) return true;
            if (body.position.Y < sceneMin.Y) return true;
            if (body.position.Y > sceneMax.Y) return true;

            return false;
        }
		
		/// @if LANG_EN
		/// <summary> check whether any rigid body is under touch point </summary>
        /// <param name="click_pos"> given touch point </param>
        /// <param name="diff_pos"> difference between touch point and center of rigid body </param>
        /// <param name="rigorous_check"> whether rigorous check is done </param>
        /// <returns> return index of rigid body which is under touch point,
        /// if there are no any rigid sceneBodies, then return -1 </returns> 
        /// @endif
		/// @if LANG_JA
		/// <summary> タッチポイントの下にある剛体を探す </summary>
        /// <param name="click_pos"> タッチポイント </param>
        /// <param name="diff_pos"> 剛体中心とタッチポイントの差 </param>
        /// <param name="rigorous_check"> 厳密なチェックを行うかどうか </param>
        /// <returns> タッチされている剛体があれば、そのインデックスをなければ-1を返す </returns> 
        /// @endif
        public int CheckPicking(ref Vector2 click_pos, ref Vector2 diff_pos, bool rigorous_check)
        {
			// Update AABB
			for(int i=0; i<numBody; i++)
				Integrate.UpdateBoundingBox(sceneBodies[i], sceneShapes);
			
            for (int i = 0; i < numBody; i++)
            {
                if ((sceneBodies[i].invMass != 0) && (sceneBodies[i].picking) && (sceneBodies[i].shapeIndex != uint.MaxValue))
                {
                    if (rigorous_check &&
                         (sceneBodies[i].InsideBody(click_pos, sceneShapes[sceneBodies[i].shapeIndex])))
                    {
                        diff_pos = click_pos - sceneBodies[i].position;
                        return i;
                    }
                    else if (Broadphase.AABB_Point_BroadPhase(sceneBodies[i], click_pos))
                    {
                        diff_pos = click_pos - sceneBodies[i].position;
                        return i;
                    }
                }
            }
            return -1;
        }
		
		/// @if LANG_EN
		/// <summary> check whether any rigid body is under touch point </summary>
        /// <param name="click_pos"> given touch point </param>
        /// <param name="diff_pos"> difference between touch point and center of rigid body </param>
        /// <param name="rigorous_check"> whether rigorous check is done </param>
        /// <returns> return index of rigid body which is under touch point,
        /// if there are no any rigid sceneBodies, then return -1  <br />
        /// This function returns index even if its index is belong to static rigid body </returns> 
        /// @endif
		/// @if LANG_JA
		/// <summary> タッチポイントの下にある剛体を探す </summary>
        /// <param name="click_pos"> タッチポイント </param>
        /// <param name="diff_pos"> 剛体中心とタッチポイントの差 </param>
        /// <param name="rigorous_check"> 厳密なチェックを行うかどうか </param>
        /// <returns> タッチされている剛体があれば、そのインデックスをなければ-1を返す <br />
        /// 剛体がStaticであってもピッキングデータを返すことに注意 </returns> 
        /// @endif
        public int CheckForcePicking(ref Vector2 click_pos, ref Vector2 diff_pos, bool rigorous_check)
        {
			// Update AABB
			for(int i=0; i<numBody; i++)
				Integrate.UpdateBoundingBox(sceneBodies[i], sceneShapes);
			
            for (int i = 0; i < numBody; i++)
            {
                if (true)
                {
                    if (rigorous_check &&
                         (sceneBodies[i].InsideBody(click_pos, sceneShapes[sceneBodies[i].shapeIndex])))
                    {
                        diff_pos = click_pos - sceneBodies[i].position;
                        return i;
                    }
                    else if (Broadphase.AABB_Point_BroadPhase(sceneBodies[i], click_pos))
                    {
                        diff_pos = click_pos - sceneBodies[i].position;
                        return i;
                    }
                }
            }
            return -1;
        }
					
		/// @if LANG_EN		
		/// <summary> Clear numbers of properties of scene </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> シーンの初期化を定義する </summary>
    	/// @endif
		public virtual void InitScene()
		{
			numBody = 0;
			numJoint = 0;
			numSpring = 0;
			numShape = 0;
			numIgnorePair = 0;
			
			numBroadPair = 0;
			numPhysicsSolverPair = 0;
			numOldPhysicsSolverPair = 0;

			simDt = 1.0f/60.0f;
			gravity = new Vector2(0, -9.8f);
			
            Solver.penetrationRepulse = 0.2f;
			Solver.penetLimit = 0.03f;
            Solver.tangentFriction = 0.3f;
			Solver.restitutionCoeff = 0.0f;
			
		}
		
		/// @if LANG_EN
		/// <summary> user should define that all resources
		/// are released including debug rendering objects  </summary>
   		/// @endif
		/// @if LANG_JA
		/// <summary> デバッグレンダリングリソースを含む、リソースの解放をここで定義する </summary>
   		/// @endif
        public virtual void ReleaseScene() {}
		
		/// @if LANG_EN
		/// <summary> user should define Game pad handling </summary>
		/// <param name="ePad"> game pad input </param>
   		/// @endif
		/// @if LANG_JA
		/// <summary> ゲームパッドイベントの扱いを定義 </summary>
		/// <param name="ePad"> ゲームパッド入力 </param>
   		/// @endif
        public virtual void KeyboardFunc(GamePadButtons ePad) {}
		
		/// @if LANG_EN
		/// <summary> Post simulation, user update based on contact points... etc  </summary>
   		/// @endif
		/// @if LANG_JA
		/// <summary> ポストシミュレーション処理, 衝突情報に基づく処理などを定義する </summary>
   		/// @endif
        public virtual void UpdateFuncAfterSim() {}
		
		/// @if LANG_EN  
		/// <summary> Pre simulation, user update to force positions changes... etc </summary> 
   		/// @endif
		/// @if LANG_JA
		/// <summary> プレシミュレーション処理, 位置情報に基づき、外力を加えるなどの処理を定義する </summary> 
   		/// @endif
        public virtual void UpdateFuncBeforeSim() {}
		
		/// @if LANG_EN
		/// <summary> User can define how to render rigid body </summary>
		/// <param name="graphics"> given graphics context </param>
		/// <param name="program"> given shader program </param>
		/// <param name="rendering_scale"> given rendering scale </param>
		/// <param name="click_index"> given touch point index </param>
    	/// @endif
		/// @if LANG_JA
		/// <summary> レンダリングをユーザーが定義する </summary>
		/// <param name="graphics"> グラフィックスコンテキスト </param>
		/// <param name="program"> シェーダープログラム </param>
		/// <param name="rendering_scale"> レンダリングスケール </param>
		/// <param name="click_index"> クリックした剛体インデックス </param>
    	/// @endif
        public virtual void DrawAllBody
            (ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 rendering_scale, int click_index) { }
		
		/// @if LANG_EN	
		/// <summary> User can define additional debug rendering </summary>
		/// <param name="graphics"> given graphics context </param>
		/// <param name="program"> given shader program </param>
		/// <param name="rendering_scale"> given rendering scale </param>
    	/// @endif
		/// @if LANG_JA
		/// <summary> デバッグレンダリングをユーザーが定義する </summary>
		/// <param name="graphics"> グラフィックスコンテキスト </param>
		/// <param name="program"> シェーダープログラム </param>
		/// <param name="rendering_scale"> レンダリングスケール </param>
    	/// @endif
        public virtual void DrawAdditionalInfo
            (ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 rendering_scale) { }

		//
        // Get and Set Methods
		//
	

		
		/// @if LANG_EN
		/// <summary> setter and getter of numBody </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of numBody </summary>
    	/// @endif
        public int NumBody
        {
            set { this.numBody = value; }
            get { return numBody; }
        }
			
		/// @if LANG_EN
		/// <summary> setter and getter of numJoint  </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of numJoint  </summary>
    	/// @endif
        public int NumJoint 
        {
            set { this.numJoint = value; }
            get { return numJoint; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of numSpring  </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of numSpring </summary>
    	/// @endif
        public int NumSpring 
        {
            set { this.numSpring = value; }
            get { return numSpring; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of numShape </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of numShape </summary>
    	/// @endif
        public int NumShape
        {
            set { this.numShape = value; }
            get { return numShape; }
        }	
		
		/// @if LANG_EN
		/// <summary> setter and getter of numBroadPair  </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of numBroadPair  </summary>
    	/// @endif
        public int NumBroadPair  
        {
            set { this.numBroadPair  = value; }
            get { return numBroadPair ; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of numPhysicsSolverPair </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of numPhysicsSolverPair </summary>
    	/// @endif
        public int NumPhysicsSolverPair   
        {
            set { this.numPhysicsSolverPair   = value; }
            get { return numPhysicsSolverPair  ; }
        }	
	
		/// @if LANG_EN
		/// <summary> setter and getter of numOldPhysicsSolverPair </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of numOldPhysicsSolverPair </summary>
    	/// @endif
        public int NumOldPhysicsSolverPair   
        {
            set { this.numOldPhysicsSolverPair    = value; }
            get { return numOldPhysicsSolverPair   ; }
        }	
		
		/// @if LANG_EN	
		/// <summary> setter and getter of sceneBodies </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sceneBodies </summary>
    	/// @endif
        public PhysicsBody[] SceneBodies
        {
            set { this.sceneBodies = value; }
            get { return sceneBodies; }
        }
		
		/// @if LANG_EN	
		/// <summary> setter and getter of sceneShapes </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sceneShapes </summary>
    	/// @endif
        public PhysicsShape[] SceneShapes
        {
            set { this.sceneShapes = value; }
            get { return sceneShapes; }
        }
		
		/// @if LANG_EN	
		/// <summary> setter and getter of sceneJoints </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sceneJoints </summary>
    	/// @endif
        public PhysicsJoint[] SceneJoints
        {
            set { this.sceneJoints = value; }
            get { return sceneJoints; }
        }
		
		/// @if LANG_EN	
		/// <summary> setter and getter of sceneSprings </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sceneSprings </summary>
    	/// @endif
        public PhysicsSpring[] SceneSprings
        {
            set { this.sceneSprings = value; }
            get { return sceneSprings; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of simDt </summary>	
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of simDt </summary>	
    	/// @endif
		public float SimDt
        {
            set { this.simDt = value; }
            get { return simDt; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of gravity </summary>	
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of gravity </summary>	
    	/// @endif
		public Vector2 Gravity
        {
            set { this.gravity = value; }
            get { return gravity; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of sceneMin </summary>	
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sceneMin </summary>	
    	/// @endif
		public Vector2 SceneMin
        {
            set { this.sceneMin = value; }
            get { return sceneMin; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of sceneMax </summary>	
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sceneMax </summary>	
    	/// @endif
		public Vector2 SceneMax
        {
            set { this.sceneMax = value; }
            get { return sceneMax; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of sceneFrame </summary>	
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sceneFrame </summary>	
    	/// @endif
		public ulong SceneFrame
        {
            set { this.sceneFrame = value; }
            get { return sceneFrame; }
        }
		
		
		/// @if LANG_EN
		/// <summary> setter and getter of sceneName </summary>	
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sceneName </summary>	
    	/// @endif
		public string SceneName
        {
            set { this.sceneName = value; }
            get { return sceneName; }
        }
			
		/// @if LANG_EN
		/// <summary> setter and getter of numIgnorePair </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of numIgnorePair </summary>
    	/// @endif
        public int NumIgnorePair   
        {
            set { this.numIgnorePair = value; }
            get { return numIgnorePair; }
        }		
			
		/// @if LANG_EN
		/// <summary> setter and getter of ignorePair </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of ignorePair </summary>
    	/// @endif
        public uint[] IgnorePair    
        {
            set { this.ignorePair = value; }
            get { return ignorePair; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of printPerf </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of printPerf </summary>
    	/// @endif
        public bool PrintPerf     
        {
            set { this.printPerf = value; }
            get { return printPerf  ; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of uniqueProperty </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of uniqueProperty </summary>
    	/// @endif
        public bool UniqueProperty      
        {
            set { this.uniqueProperty = value; }
            get { return uniqueProperty   ; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of penetrationRepulse </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of penetrationRepulse </summary>
    	/// @endif
        public float PenetrationRepulse       
        {
            set { this.penetrationRepulse = value; }
            get { return penetrationRepulse   ; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of penetLimit </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of penetLimit </summary>
    	/// @endif
        public float PenetLimit       
        {
            set { this.penetLimit = value; }
            get { return penetLimit; }
        }
		
		/// @if LANG_EN
		/// <summary> setter and getter of tangentFriction </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of tangentFriction </summary>
    	/// @endif
        public float TangentFriction        
        {
            set { this.tangentFriction = value; }
            get { return tangentFriction; }
        }	

		/// @if LANG_EN
		/// <summary> setter and getter of restitutionCoeff </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of restitutionCoeff </summary>
    	/// @endif
        public float RestitutionCoeff         
        {
            set { this.restitutionCoeff = value; }
            get { return restitutionCoeff; }
        }	

		/// @if LANG_EN
		/// <summary> setter and getter of filterOp </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of filterOp </summary>
    	/// @endif
        public BroadOpType FilterOp         
        {
            set { this.filterOp = value; }
            get { return filterOp; }
        }	
		
		/// @if LANG_EN
		/// <summary> setter and getter of compoundMap </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of compoundMap </summary>
    	/// @endif
		public uint[] CompoundMap         
        {
            set { this.compoundMap = value; }
            get { return compoundMap; }
        }	
		
		/// @if LANG_EN
		/// <summary> getter of solverPair </summary>
		/// <remarks> setter is not provided </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> getter of solverPair </summary>
		/// <remarks> setter is not provided </remarks>
    	/// @endif
		public PhysicsSolverPair[] SolverPair
		{
            get { return solverPair; }	
		}
		
		/// @if LANG_EN
		/// <summary> getter of broadPair </summary>
		/// <remarks> setter is not provided </remarks>
    	/// @endif
		/// @if LANG_JA
		/// <summary> getter of broadPair </summary>
		/// <remarks> setter is not provided </remarks>
    	/// @endif
		public uint[] BroadPair
		{
            get { return broadPair; }		
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of sleepAlgorithm </summary>
    	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sleepAlgorithm </summary>
    	/// @endif
		public SleepAlgorithmType SleepAlgorithm         
        {
            set { this.sleepAlgorithm = value; }
            get { return sleepAlgorithm; }
        }
		
        #endregion
		
		/// @if LANG_EN
		/// <summary> SolverPairCache class </summary>
		/// <remarks> this contains cache data for PhysicsSolverPair,
		/// user don't need to use this structure generally</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> SolverPairCache class </summary>
		/// <remarks> PhysicsSolverPairのキャッシュ情報、一般的には使用する必要はない </remarks>
		/// @endif
	    private struct SolverPairCache
	    {
			#region Fields
			/// @if LANG_EN
			/// <summary>total of ContactPointCache</summary>
			/// <remarks>shift and sum of index of bodyA and bodyB </remarks>
			/// @endif
			/// @if LANG_JA
			/// <summary> ContactPointCacheの衝突ペアの合成インデックス</summary>
			/// <remarks>bodyA and bodyBのインデックスをシフト演算と和を用いて合成したインデックス</remarks>
			/// @endif
		    internal uint totalIndex;
				
			/// @if LANG_EN
			/// <summary>contact point cache of ContactPointCache</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>ContactPointCacheの衝突点のキャッシュ情報</summary>
			/// @endif
		    internal Vector2 resA;
			
			/// @if LANG_EN
			/// <summary>contact point cache of ContactPointCache</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>ContactPointCacheの衝突点のキャッシュ情報</summary>
			/// @endif
		    internal Vector2 resB;
		   
			/// @if LANG_EN
			/// <summary> impulese cache for normal direction of ContactPointCache</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary> ContactPointCacheの反発法線方向の撃力のキャッシュ</summary>
			/// @endif
			internal float Pn;
		    
			/// @if LANG_EN
			/// <summary> impulse cache for tangent direction of ContactPointCache</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary> ContactPointCacheの反接戦方向の撃力のキャッシュ</summary>
			/// @endif
			internal float Pt;	
	        #endregion		
	    }	
		
		/// @if LANG_EN
		/// <summary> ContactPoint class </summary>
		/// <remarks> this structure does not contains actual
		/// contact point for collision pair bodyA and bodyB,
		/// and it just contain the result for Separate Axis Theorem check,
		/// and user don't need to use this information generally.
		/// Please see PhysicsSolverPair</remarks>
		/// @endif
		/// @if LANG_JA 
		/// <summary> ContactPoint class </summary>
		/// <remarks> this structure does not contains actual
		/// contact point for collision pair bodyA and bodyB,
		/// and it just contain the result for Separate Axis Theorem check,
		/// and user don't need to use this information generally.
		/// Please see PhysicsSolverPair</remarks>
		/// @endif
		private struct ContactPoint
	    {
			#region Fields
			/// @if LANG_EN
			/// <summary>contact point for BodyA of ContactPoint</summary>
			/// <remarks>when there is jut one collision point, PosA2 is meaningless </remarks>
			/// @endif
			/// @if LANG_JA
			/// <summary>contact point for BodyA of ContactPoint</summary>
			/// <remarks>when there is jut one collision point, PosA2 is meaningless </remarks>
			/// @endif
		    internal Vector2 PosA,PosA2;
			
			/// @if LANG_EN
			/// <summary>contact point for BodyB of ContactPoint</summary>
			/// <remarks>when there is only one collision point, PosB2 is meaningless </remarks>
			/// @endif
			/// @if LANG_JA
			/// <summary>contact point for BodyB of ContactPoint</summary>
			/// <remarks>when there is only one collision point, PosB2 is meaningless </remarks>
			/// @endif
		    internal Vector2 PosB, PosB2;
			
			/// @if LANG_EN
			/// <summary>total of ContactPoint</summary>
			/// <remarks>shift and sum of index of bodyA and bodyB </remarks>
			/// @endif
			/// @if LANG_JA
			/// <summary>total of ContactPoint</summary>
			/// <remarks>shift and sum of index of bodyA and bodyB </remarks>
			/// @endif
	        internal uint totalIndex;
			
			/// @if LANG_EN
			/// <summary>normal direction to Separate Axis</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>normal direction to Separate Axis</summary>
			/// @endif
	        internal Vector2 normal;
			
			/// @if LANG_EN		
			/// <summary>penetration depth of ContactPoint</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>penetration depth of ContactPoint</summary>
			/// @endif
	        internal float penetrationDepth;
			
			/// @if LANG_EN	
			/// <summary>fixed poisition to avoid penetration before solver phase calculation</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>fixed poisition to avoid penetration before solver phase calculation</summary>
			/// @endif
			internal Vector2 NewPosA, NewPosB;
			
			/// @if LANG_EN	
			/// <summary>old poisition to avoid penetration before solver phase</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>old poisition to avoid penetration before solver phase</summary>
			/// @endif
	        internal Vector2 OldPosA, OldPosB;
			
			internal Vector2 centerA, centerB;
	        #endregion		
	    }
	    

    }
	
	
}
