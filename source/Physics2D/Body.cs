/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;

//
// Basic Type Definition for Physics2D
//
namespace Sce.PlayStation.HighLevel.Physics2D
{		
	/// @if LANG_EN
	/// <summary>Defines whether rigid body is dynamic, kinematic, static or trigger</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>剛体がダイナミック、キネマティック、スタティックかトリガーを指定する</summary>
	/// @endif
	public enum BodyType 
	{
		/// @if LANG_EN
		/// <summary>usual dynamic body</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>一般的なダイナミック剛体</summary>
		/// @endif
		Dynamic,
		
		/// @if LANG_EN
		/// <summary>kinematic body</summary>
		/// <remarks>does not move for a while, but can move forcely for some event happening <br />
		/// supposed that its mass is infinite, the is always rebounded at the time of the collision </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>キネマティック剛体</summary>
		/// <remarks>しばらく動かないが、何かのイベントで人為的に動かされるようになる <br/>
		/// 質量は無限大と仮定して計算されるので、衝突時には常に相手を跳ね返す</remarks>
		/// @endif
		Kinematic,
		
		/// @if LANG_EN
		/// <summary>static body</summary>
		/// <remarks>does not move at all from the start to the end</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>スタティック剛体</summary>
		/// <remarks>最初から最後まで動くことがない</remarks>
		/// @endif
		Static,
		
		/// @if LANG_EN
		/// <summary>trigger body</summary>		
		/// <remarks>which does not collision response but has collision detection</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>トリガー剛体</summary>		
		/// <remarks>衝突反発はないが、衝突判定はある</remarks>
		/// @endif
		Trigger,
	}
		
	
	/// @if LANG_EN
    /// <summary> Rigid Body is defined with many attributes </summary>
 	/// @endif
	/// @if LANG_JA
    /// <summary> 多くのプロパティを利用して剛体を定義する </summary>	
 	/// @endif
	public partial class PhysicsBody
	{	
		#region Fields
		/// @if LANG_EN
		/// <summary>position of rigid body</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の位置</summary>
		/// @endif
        public Vector2 position = new Vector2(0, 0);
		
		/// @if LANG_EN
		/// <summary>rotation of rigid body</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の回転角度</summary>
		/// @endif
		public float rotation = 0.0f;
		
		/// @if LANG_EN
		/// <summary>velocity of rigid body</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の並進速度</summary>
		/// @endif
        public Vector2 velocity = new Vector2(0, 0);
 
		/// @if LANG_EN
		/// <summary>angular velocity of rigid body</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の回転速度</summary>
		/// @endif
		public float angularVelocity = 0.0f;
		
		/// @if LANG_EN
		/// <summary> local position of rigid body if it is used as compound shape</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> コンパウンド形状の場合のローカル位置</summary>
		/// @endif
		public Vector2 localPosition = new Vector2(0, 0);
		
		/// @if LANG_EN	
		/// <summary> local rotation of rigid body if it is used as compound shape</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> コンパウンド形状の場合のローカル回転角度</summary>
		/// @endif
        public float localRotation = 0.0f;
		
		/// @if LANG_EN	
		/// <summary>acceleration of rigid body</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の並進加速度</summary>
		/// @endif
        public Vector2 acceleration = new Vector2(0, 0);
		
		/// @if LANG_EN
		/// <summary>external force acted to rigid body except for the gravity</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体にかかる重力を除く外力</summary>
		/// @endif
        public Vector2 force = new Vector2(0, 0);
		
		/// @if LANG_EN
		/// <summary>external torque acted to rigid body except for the gravity</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体にかかる重力を除くトルク</summary>
		/// @endif
		public float torque = 0.0f;
		
		/// @if LANG_EN
		/// <summary>mass of rigid body</summary>
		/// <remarks>mass should be consistent with inverse mass</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の質量</summary>
		/// <remarks>剛体の質量は質量の逆数と同期していないといけない</remarks>
		/// @endif
        public float mass = 0.0f;
		
		/// @if LANG_EN
		/// <summary>inverse of rigid body</summary>
		/// <remarks>inverse should be consistent with mass</remarks> 
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の質量の逆数</summary>
		/// <remarks>剛体の質量の逆数は質量と同期していないといけない</remarks> 
		/// @endif
        public float invMass = 0.0f;
		
		/// @if LANG_EN
		/// <summary>inertia tensor of rigid body</summary>	
		/// <remarks>inertia tensor should be consistent with inverse inertia tensor</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体のテンソル</summary>	
		/// <remarks>剛体のテンソルはテンソルの逆数と同期していないといけない</remarks>
		/// @endif
        public float inertia = 0.0f;
		
		/// @if LANG_EN
		/// <summary>inverse inertia tensor of rigid body</summary>	
		/// <remarks>inverse inertia tensor should be consistent with inertia tensor</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体のテンソルの逆数</summary>	
		/// <remarks>剛体のテンソルの逆数はテンソルと同期していないといけない</remarks>
		/// @endif
        public float invInertia = 0.0f;
		
		/// @if LANG_EN
		/// <summary>AABB bounding for default position and rotation</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の初期位置、回転角度に対するAABBバウンディングボックス</summary>
		/// @endif
        public Vector2 width = new Vector2(0, 0);
		
		/// @if LANG_EN
		/// <summary>left bottom corner of AABB bounding updated for each frame</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>毎フレームアップデートされるAABBバウンディングボックスの左下位置</summary>
		/// @endif
        public Vector2 aabbMin = new Vector2(0, 0);
		
		/// @if LANG_EN
		/// <summary>right top corner of AABB bounding updated for each frame</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>毎フレームアップデートされるAABBバウンディングボックスの右上位置</summary>
		/// @endif
        public Vector2 aabbMax = new Vector2(0, 0); 
		
		/// @if LANG_EN
		/// <summary>check whether there is broad phase collision</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Broad Phase衝突判定の結果</summary>
		/// @endif
        public bool broadCollide = false;
		
		/// @if LANG_EN
		/// <summary>check whether there is narrow phase collision</summary>
		/// @endif
		/// @if LANG_JA 
		/// <summary>Narrow Phase衝突判定の結果</summary>
		/// @endif
        public bool narrowCollide = false;
	
		/// @if LANG_EN
		/// <summary>index to the shape of rigid body</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の形状のインデックス</summary>
		/// @endif
        public uint shapeIndex = 0;
		
		/// @if LANG_EN
		/// <summary>collision filter to rigid body</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体のcollisionフィルター</summary>
		/// @endif
        public uint collisionFilter = 0;
		
		/// @if LANG_EN
		/// <summary>group filter to rigid body</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>剛体のgroupフィルター</summary>
		/// @endif
		public uint groupFilter = 0xFFFFFFFF;
		
		/// @if LANG_EN
		/// <summary>air friction is treated inside integrate phase</summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary>Integrate Phaseで考慮される空気摩擦係数</summary>
 		/// @endif
        public float airFriction = 0.0f;
	
		/// @if LANG_EN
		/// <summary>collision friction is treated inside solver phase</summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary>Solver Phaseで考慮される衝突摩擦係数</summary>
 		/// @endif
		public float colFriction = 0.0f; 
		
		/// @if LANG_EN
		/// <summary>sleep status, if it is true, it will be ignored for simulation</summary>
 		/// @endif
		/// @if LANG_JA 
		/// <summary>スリープ状態、真ならシミュレーションで考慮されない</summary>
 		/// @endif
        public bool sleep = false;
		
		/// @if LANG_EN
		/// <summary>counter to check whether rigid body falls into sleep status</summary>
 		/// @endif
		/// @if LANG_JA 
		/// <summary>スリープ状態に入るかをチェックするカウンター</summary>
 		/// @endif
		public uint sleepCount = 0;
		
		/// @if LANG_EN
		/// <summary>define the type of rigid boy</summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary>剛体のタイプを指定する</summary>
 		/// @endif
		internal BodyType type = BodyType.Dynamic;
		
		/// @if LANG_EN
		/// <summary>picking enabled or disabled for default simulation loop implementation</summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary>シミュレーション中にピッキングを可能にするかどうか</summary>
 		/// @endif
		public bool picking = true;
		
		/// @if LANG_EN
		/// <summary>old mass of rigid body</summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の前の質量</summary>
 		/// @endif
        public float oldMass = 0.0f;
		
		/// @if LANG_EN
		/// <summary>old inverse mass of rigid body</summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の前の質量の逆数</summary>
 		/// @endif
		public float oldInvMass = 0.0f;
		
		/// @if LANG_EN
		/// <summary>old inertia tensor of rigid body</summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の前のテンソル</summary>
 		/// @endif
		public float oldInertia = 0.0f;
		
		/// @if LANG_EN
		/// <summary>old inverse inertia tensor of rigid body</summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary>剛体の前のテンソルの逆数</summary>
 		/// @endif
        public float oldInvInertia = 0.0f;
		
		/// @if LANG_EN
		/// <summary>compound map for compound shape</summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary>コンパウンド形状のためのマッピング情報</summary>
 		/// @endif
		internal uint compound = 0;
		
		/// @if LANG_EN
		/// <summary>repulse accelaration for penetration</summary>
		/// <remarks>To enable this value, it is required to set uniqueProperty of PhysicsScene as false</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>めり込みに対する反発力の加速係数</summary>
		/// <remarks>この値を有効にするにはPhysicsSceneのuniquePropertyをfalseにする必要あり</remarks>
		/// @endif
		public float penetrationRepulse = 0.2f;
			
		/// @if LANG_EN
		/// <summary>penetration limitation</summary>
		/// <remarks>To enable this value, it is required to set uniqueProperty of PhysicsScene as false</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>めり込みに対する許容度</summary>
		/// <remarks>この値を有効にするにはPhysicsSceneのuniquePropertyをfalseにする必要あり</remarks>
		/// @endif 
		public float penetLimit = 0.03f;
		
		/// @if LANG_EN
		/// <summary>tangent friction for repluse</summary>
		/// <remarks>To enable this value, it is required to set uniqueProperty of PhysicsScene as false</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>反発に対する接線方向の摩擦係数</summary>
		/// <remarks>この値を有効にするにはPhysicsSceneのuniquePropertyをfalseにする必要あり</remarks>
		/// @endif 
		public float tangentFriction = 0.3f;
		        
		/// @if LANG_EN
		/// <summary>restitution coefficient for repluse</summary>
		/// <remarks>To enable this value, it is required to set uniqueProperty of PhysicsScene as false</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>反発に対する反発係数</summary>
		/// <remarks>この値を有効にするにはPhysicsSceneのuniquePropertyをfalseにする必要あり</remarks>
		/// @endif 
		public float restitutionCoeff = 0.0f;
		
		/// @if LANG_EN
		/// <summary> reference to user data </summary>
		/// <remarks> user data should be managed by user properly</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary>ユーザーデータへの参照</summary>
		/// <remarks> 参照先データはユーザーによって適切に管理される必要があります </remarks>
		/// @endif 
		public object userData = null;
		#endregion
		
		#region Fields
		/// @if LANG_EN
		/// <summary> Default Constructor </summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary> デフォルトコンスタラクタ </summary>
 		/// @endif
		public PhysicsBody()
		{
			// nothing done here for this version
			Clear ();
		}
			
		/// @if LANG_EN    
		/// <summary> Copy Constructor </summary>
        /// <param name="body"> source rigid body </param>
 		/// @endif
		/// @if LANG_JA
		/// <summary> コピーコンストラクタ </summary>
        /// <param name="body"> コピー元の剛体 </param>
 		/// @endif	
		public PhysicsBody(PhysicsBody body)
		{
			position = body.position;
			rotation = body.rotation;
			velocity = body.velocity;
			angularVelocity = body.angularVelocity;
			localPosition = body.localPosition;
			localRotation = body.LocalRotation;
			acceleration = body.acceleration;
			force = body.force;
			torque = body.torque;
			mass = body.mass;
			invMass = body.invMass;
			inertia = body.inertia;
			invInertia = body.invInertia;
			width = body.width;
			aabbMin = body.aabbMin;
			aabbMax = body.aabbMax;
			broadCollide = body.broadCollide;
			narrowCollide = body.narrowCollide;
			shapeIndex = body.shapeIndex;
			collisionFilter = body.collisionFilter;
			groupFilter = body.groupFilter;
			airFriction = body.airFriction;
			colFriction = body.colFriction;
			sleep = body.sleep;
			sleepCount = body.sleepCount;
			type = body.type;
			picking = body.picking;
			oldMass = body.oldMass;
			oldInvMass = body.oldInvMass;
			oldInertia = body.oldInertia;
			oldInvInertia = body.oldInvInertia;
			compound = body.compound;
			userData = body.userData;
		}
		
		/// @if LANG_EN    
		/// <summary> Constructor </summary>
        /// <param name="conv"> shape of rigid body </param>
        /// <param name="m"> mass of rigid body </param>
 		/// @endif
		/// @if LANG_JA
		/// <summary> コンストラクタ </summary>
        /// <param name="conv"> 剛体の形状 </param>
        /// <param name="m"> 剛体の質量 </param>
 		/// @endif
        public PhysicsBody(PhysicsShape conv, float m)
        {
            mass = m;
			
	        float xmax = -PhysicsUtility.FltMax;
	        float ymax = -PhysicsUtility.FltMax;

            // conv.numVert == 0 means sphere object
            // (and conv.vertList[0].x should contains radius of the sphere)
            if (conv.numVert != 0)
            {
                for (int i = 0; i < conv.numVert; i++)
                {
                    Vector2 v = conv.vertList[i];
                    xmax = Math.Max(xmax, Math.Abs(v.X));
                    ymax = Math.Max(ymax, Math.Abs(v.Y));
                }
            }
            else
	        {
		        xmax = conv.vertList[0].X;
		        ymax = conv.vertList[0].X;
	        }

			width = new Vector2(xmax, ymax);
			
            if (mass < PhysicsUtility.FltMax)
	        {
                invMass = 1.0f / mass;
                inertia = mass * (width.X * width.X + width.Y * width.Y) / 3.0f;
                invInertia = 1.0f / inertia;
				type = BodyType.Dynamic;
	        }
	        else
	        {
                invMass = 0.0f;
                inertia = PhysicsUtility.FltMax;
                invInertia = 0.0f;
				type = BodyType.Static;
	        }
			
            picking = true;
        }

		/// @if LANG_EN    
		/// <summary> Clear all members </summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary> 全てのメンバをクリアする </summary>
 		/// @endif	
		public void Clear()
		{
			position = new Vector2(0, 0);
			rotation = 0.0f;
			velocity = new Vector2(0, 0);
			angularVelocity = 0.0f;
			localPosition = new Vector2(0, 0);
			localRotation = 0.0f;
			acceleration = new Vector2(0, 0);
			force = new Vector2(0, 0);
			torque = 0.0f;
			mass = 0.0f;
			invMass = 0.0f;
			inertia = 0.0f;
			invInertia = 0.0f;
			width = new Vector2(0, 0);
			aabbMin = new Vector2(0, 0);
			aabbMax = new Vector2(0, 0);
			broadCollide = false;
			narrowCollide = false;
			shapeIndex =  0;
			collisionFilter = 0;
			groupFilter = 0xFFFFFFFF;
			airFriction = 0.0f;
			colFriction = 0.0f;
			sleep = false;
			sleepCount = 0;
			type = BodyType.Dynamic;
			picking = true;
			oldMass = 0.0f;
			oldInvMass = 0.0f;
			oldInertia = 0.0f;
			oldInvInertia = 0.0f;
			compound = 0;
			userData = null;
		}
		
		/// @if LANG_EN    
		/// <summary> Copy all members of rigid body</summary>
 		/// @endif
		/// @if LANG_JA
		/// <summary> 全てのメンバをコピーする </summary>
 		/// @endif		
		public void CopyBody(PhysicsBody body)
		{
			position = body.position;
			rotation = body.rotation;
			velocity = body.velocity;
			angularVelocity = body.angularVelocity;
			localPosition = body.localPosition;
			localRotation = body.localRotation;
			acceleration = body.acceleration;
			force = body.force;
			torque = body.torque;
			mass = body.mass;
			invMass = body.invMass;
			inertia = body.inertia;
			invInertia = body.invInertia;
			width = body.width;
			aabbMin = body.aabbMin;
			aabbMax = body.aabbMax;
			broadCollide = body.broadCollide;
			narrowCollide = body.NarrowCollide;
			shapeIndex = body.shapeIndex;
			collisionFilter = body.collisionFilter;
			groupFilter = body.groupFilter;
			airFriction = body.airFriction;
			colFriction = body.colFriction;
			sleep = body.sleep;
			sleepCount = body.sleepCount;
			type = body.type;
			picking = body.picking;
			oldMass = body.oldMass;
			oldInvMass = body.oldInvMass;
			oldInertia = body.oldInertia;
			oldInvInertia = body.oldInvInertia;
			compound = body.compound;
			penetrationRepulse = body.penetrationRepulse;
			penetLimit = body.penetLimit;
			tangentFriction = body.tangentFriction;
			restitutionCoeff = body.restitutionCoeff;
			userData = body.userData;
		}
		
			
		/// @if LANG_EN    
		/// <summary> Update position, rotation, velocity, angularVelocity</summary>
        /// <param name="pt"> new position of rigid body </param>
        /// <param name="rt"> new rotation of rigid body </param>
        /// <param name="dt"> timestep </param>
        /// <remarks> Set velocity and angularVelocity automatically by giving new position and rotation<br />
        /// position, rotation, velocity, angularVelocity are set by this function
        /// This is mainly for kinematic rigid body, and cannot be used for static rigid body
        /// </remarks>
 		/// @endif
		/// @if LANG_JA
		/// <summary> 位置、回転、並進速度、回転速度をアップデートする </summary>
        /// <param name="pt"> 剛体の新しい位置 </param>
        /// <param name="rt"> 剛体の新しい回転 </param>
        /// <param name="dt"> タイムステップ </param>
        /// <remarks> 剛体の新しい位置と回転を与えて、自動的に並進速度、回転速度を計算して設定します<br />
        /// 位置、回転、並進速度、回転速度がこの関数によってセットされます
        /// 主にキネマティック剛体に対して使用し、スタティック剛体には使用できません </remarks>
 		/// @endif		
		public void MoveTo(Vector2 pt, float rt, float dt)
		{
			if(IsStatic())
				return;
			
			sleep = false;
			sleepCount = 0;
			
			Vector2 oldPos = position;
			float oldRot = rotation;
			position = pt;
			rotation = rt;
			velocity = (position - oldPos)/dt;
			angularVelocity = (rotation - oldRot)/dt;
		}

		/// @if LANG_EN
		/// <summary> Merge of rigid body </summary>
		/// <remarks>
        /// To merge multiple objects (compound object), index_list should be given in manner of the ascending order
        /// </remarks>
        /// <param name="bodyList"> given rigid body list of the scene</param>
        /// <param name="indexList"> given index list of rigid body list to be attached </param>
       	/// <param name="num"> given number of rigid sceneBodies to be attached </param>
        /// <param name="sceneShapes"> given convex list of the scene </param>
        /// <param name="map"> Compound map of the scene </param>
        /// <param name="flag"> whether rearrange the position of merged rigid body </param>
  		/// @endif
		/// @if LANG_JA
		/// <summary> 剛体のマージ </summary>
		/// <remarks>
        /// 複数の剛体 (コンパウンド形状)をマージするために、インデックスが昇順で与えられている
        /// </remarks>
        /// <param name="bodyList"> シーンの剛体リスト</param>
        /// <param name="indexList"> マージするための剛体のインデックスリスト </param>
       	/// <param name="num"> マージするための剛体の数 </param>
        /// <param name="sceneShapes"> シーンの形状リスト </param>
        /// <param name="map"> コンパウンド形状対応マップ </param>
        /// <param name="flag"> 剛体の位置をマージ後に再調整するかどうか </param>
  		/// @endif
		public static Vector2 MergeBody(PhysicsBody[] bodyList, uint[] indexList, uint num, PhysicsShape[] sceneShapes, uint[] map, bool flag=true)
        {
			// you can switch whether you use rough tensor calculation
			// rough tensor calculation is relatively stable for compound case
			bool useRoughTensor = true;
			
            PhysicsBody body = new PhysicsBody();

            Vector2 bd_max = new Vector2(-PhysicsUtility.FltMax, -PhysicsUtility.FltMax);
            Vector2 bd_min = new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);

            // determine width of compound object
            for (int i = 0; i < num; i++)
            {
                uint index = indexList[i];
                PhysicsShape conv = sceneShapes[bodyList[index].shapeIndex];
                float ca = (float)Math.Cos(bodyList[index].rotation);
                float sa = (float)Math.Sin(bodyList[index].rotation);

                if (conv.numVert == 0)
                {
                    bd_max = Vector2.Max(bd_max, new Vector2(conv.vertList[0].X, conv.vertList[0].X) + bodyList[index].position);
                    bd_min = Vector2.Min(bd_min, new Vector2(-conv.vertList[0].X, -conv.vertList[0].X) + bodyList[index].position);
                }
                else
                {
                    for (int j = 0; j < conv.numVert; j++)
                    {
                        Vector2 v = conv.vertList[j];

                        v = new Vector2(
                                ca * v.X - sa * v.Y,
                                sa * v.X + ca * v.Y
                                );

                        v = v + bodyList[index].position;

                        bd_max = Vector2.Max(bd_max, v);
                        bd_min = Vector2.Min(bd_min, v);
                    }
                }
            }

            body.width = 0.5f * (bd_max - bd_min);
			
			// Inertia tensor is calculated based on the center of bounding box of compound object at first.
			// This is a very simple way.
            body.position = (bd_max + bd_min) * 0.5f;

			// Then it is re-calculated based on the difference of distance (center of bounding box <-> center of mass)
			Vector2 tran = new Vector2(0, 0);
			if(flag){
				Vector2 mass_center = new Vector2(0, 0);
				float mass_total = 0.0f;
				for (int i = 0; i < num; i++)
				{
					mass_center += bodyList[indexList[i]].mass * bodyList[indexList[i]].position;
					mass_total += bodyList[indexList[i]].mass;
				}
		
				mass_center *= 1.0f/mass_total;
				tran = body.position - mass_center;
				body.position = mass_center;
			}
			
            body.rotation = 0.0f;
            body.mass = 0.0f;
			body.inertia = 0.0f;
		
            // When one of object is static object
			// compound shape itself also be considered as a static object
            bool static_flag = false;

            for (int i = 0; i < num; i++)
            {
                uint index = indexList[i];
                body.mass += bodyList[index].mass;
				if(!useRoughTensor)
					body.inertia += bodyList[index].inertia + bodyList[index].mass * Vector2.Dot (bodyList[index].position - body.position, bodyList[index].position - body.position);

                if (bodyList[index].mass < PhysicsUtility.FltMax)
                {
                    // do nothing
                }
                else
                {
                    static_flag = true;
                }
            }


            if (static_flag)
            {
                body.invMass = 0.0f;
                body.inertia = PhysicsUtility.FltMax;
                body.invInertia = 0.0f;
				body.type = BodyType.Static;

                for (int i = 0; i < num; i++)
                {
                    uint index = indexList[i];

                    bodyList[index].mass = body.mass;
                    bodyList[index].invMass = body.invMass;

                    bodyList[index].inertia = body.inertia;
                    bodyList[index].invInertia = body.invInertia;

                    bodyList[index].localPosition = bodyList[index].position - body.position;
                    bodyList[index].localRotation = bodyList[index].rotation;

                    bodyList[index].position = body.position;
                    bodyList[index].rotation = 0.0f;
					
					bodyList[index].type = body.type;
					
                }
            }
            else
            {
                body.invMass = 1.0f / body.mass;
				if(useRoughTensor)	
					body.inertia = body.mass * (body.width.X * body.width.X + body.width.Y * body.width.Y) / 3.0f; 
				body.invInertia = 1.0f / body.inertia;
				body.type = BodyType.Dynamic;
				
                for (int i = 0; i < num; i++)
                {
                    uint index = indexList[i];

                    bodyList[index].mass = body.mass;
                    bodyList[index].invMass = body.invMass;

                    bodyList[index].inertia = body.inertia;
                    bodyList[index].invInertia = body.invInertia;

                    bodyList[index].localPosition = bodyList[index].position - body.position;
                    bodyList[index].localRotation = bodyList[index].rotation;

                    bodyList[index].position = body.position;
                    bodyList[index].rotation = 0.0f;
					
					bodyList[index].type = body.type;
                }
            }

            // Set compound map
            for (int i = 1; i < num; i++)
            {
                map[indexList[i]] = indexList[0];
                bodyList[indexList[i]].compound = indexList[0];
            }
			
			return body.position;
        }
		
		/// @if LANG_EN
		/// <summary> Merge of rigid body with gravity center </summary>
		/// <remarks>
        /// To merge multiple objects (compound object), index_list should be given in manner of the ascending order
        /// </remarks>
        /// <param name="bodyList"> given rigid body list of the scene</param>
        /// <param name="indexList"> given index list of rigid body list to be attached </param>
       	/// <param name="num"> given number of rigid sceneBodies to be attached </param>
        /// <param name="sceneShapes"> given convex list of the scene </param>
        /// <param name="map"> Compound map of the scene </param>
        /// <param name="pos"> gravity center of merged rigid body </param>
  		/// @endif
		/// @if LANG_JA
		/// <summary> 重心の設定を行う剛体のマージ </summary>
		/// <remarks>
        /// 複数の剛体 (コンパウンド形状)をマージするために、インデックスが昇順で与えられている
        /// </remarks>
        /// <param name="bodyList"> シーンの剛体リスト</param>
        /// <param name="indexList"> マージするための剛体のインデックスリスト </param>
       	/// <param name="num"> マージするための剛体の数 </param>
        /// <param name="sceneShapes"> シーンの形状リスト </param>
        /// <param name="map"> コンパウンド形状対応マップ </param>
        /// <param name="pos"> マージした剛体の重心 </param>
  		/// @endif
		public static Vector2 MergeBodyWithPos(PhysicsBody[] bodyList, uint[] indexList, uint num, PhysicsShape[] sceneShapes, uint[] map, Vector2 pos)
        {
			// you can switch whether you use rough tensor calculation
			// rough tensor calculation is relatively stable for compound case
			bool useRoughTensor = true;
			
            PhysicsBody body = new PhysicsBody();

            Vector2 bd_max = new Vector2(-PhysicsUtility.FltMax, -PhysicsUtility.FltMax);
            Vector2 bd_min = new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);

            // determine width of compound object
            for (int i = 0; i < num; i++)
            {
                uint index = indexList[i];
                PhysicsShape conv = sceneShapes[bodyList[index].shapeIndex];
                float ca = (float)Math.Cos(bodyList[index].rotation);
                float sa = (float)Math.Sin(bodyList[index].rotation);

                if (conv.numVert == 0)
                {
                    bd_max = Vector2.Max(bd_max, new Vector2(conv.vertList[0].X, conv.vertList[0].X) + bodyList[index].position);
                    bd_min = Vector2.Min(bd_min, new Vector2(-conv.vertList[0].X, -conv.vertList[0].X) + bodyList[index].position);
                }
                else
                {
                    for (int j = 0; j < conv.numVert; j++)
                    {
                        Vector2 v = conv.vertList[j];

                        v = new Vector2(
                                ca * v.X - sa * v.Y,
                                sa * v.X + ca * v.Y
                                );

                        v = v + bodyList[index].position;

                        bd_max = Vector2.Max(bd_max, v);
                        bd_min = Vector2.Min(bd_min, v);
                    }
                }
            }
			
			// This assumes that there is mass density whose of center is at pos
            body.width = Vector2.Max(bd_max - pos, pos - bd_min); 
			
            body.rotation = 0.0f;
            body.mass = 0.0f;
			body.inertia = 0.0f;
			body.position = pos;
			
            // When one of object is static object
			// compound shape itself also be considered as a static object
            bool static_flag = false;

            for (int i = 0; i < num; i++)
            {
                uint index = indexList[i];
                body.mass += bodyList[index].mass;
				if(!useRoughTensor)
					body.inertia += bodyList[index].inertia + bodyList[index].mass * Vector2.Dot (bodyList[index].position - body.position, bodyList[index].position - body.position);

                if (bodyList[index].mass < PhysicsUtility.FltMax)
                {
                    // do nothing
                }
                else
                {
                    static_flag = true;
                }
            }


            if (static_flag)
            {
                body.invMass = 0.0f;
                body.inertia = PhysicsUtility.FltMax;
                body.invInertia = 0.0f;
				body.type = BodyType.Static;

                for (int i = 0; i < num; i++)
                {
                    uint index = indexList[i];

                    bodyList[index].mass = body.mass;
                    bodyList[index].invMass = body.invMass;

                    bodyList[index].inertia = body.inertia;
                    bodyList[index].invInertia = body.invInertia;

                    bodyList[index].localPosition = bodyList[index].position - body.position;
                    bodyList[index].localRotation = bodyList[index].rotation;

                    bodyList[index].position = body.position;
                    bodyList[index].rotation = 0.0f;
					
					bodyList[index].type = body.type;
					
                }
            }
            else
            {
                body.invMass = 1.0f / body.mass;
				if(useRoughTensor)	
					body.inertia = body.mass * (body.width.X * body.width.X + body.width.Y * body.width.Y) / 3.0f; 
				body.invInertia = 1.0f / body.inertia;
				body.type = BodyType.Dynamic;
				
                for (int i = 0; i < num; i++)
                {
                    uint index = indexList[i];

                    bodyList[index].mass = body.mass;
                    bodyList[index].invMass = body.invMass;

                    bodyList[index].inertia = body.inertia;
                    bodyList[index].invInertia = body.invInertia;

                    bodyList[index].localPosition = bodyList[index].position - body.position;
                    bodyList[index].localRotation = bodyList[index].rotation;

                    bodyList[index].position = body.position;
                    bodyList[index].rotation = 0.0f;
					
					bodyList[index].type = body.type;
                }
            }

            // Set compound map
            for (int i = 1; i < num; i++)
            {
                map[indexList[i]] = indexList[0];
                bodyList[indexList[i]].compound = indexList[0];
            }
			
			return body.position;
        }
		
		/// @if LANG_EN
		/// <summary> CalcBoundingBox </summary>
  		/// @endif
		/// @if LANG_JA
		/// <summary> バウンディングボックス計算 </summary>
        /// </remarks>
  		/// @endif
		private void CalcBoundingBox(PhysicsShape cc)
		{
			if (shapeIndex != uint.MaxValue)
			{
				aabbMin = new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
				aabbMax = -new Vector2(PhysicsUtility.FltMax, PhysicsUtility.FltMax);
           
				float ca_local = (float)Math.Cos(localRotation);            
				float sa_local = (float)Math.Sin(localRotation);       

				float ca = (float)Math.Cos(rotation);            
				float sa = (float)Math.Sin(rotation);
            
				if (cc.numVert == 0)	            
				{            
					Vector2 v = new Vector2(	                
						ca * localPosition.X - sa * localPosition.Y,                 
						sa * localPosition.X + ca * localPosition.Y                    
						);
	
					aabbMin = v - new Vector2(cc.vertList[0].X, cc.vertList[0].X);	                
					aabbMax = v + new Vector2(cc.vertList[0].X, cc.vertList[0].X);	                
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
		                        
						aabbMin = Vector2.Min(aabbMin, v);
						aabbMax = Vector2.Max(aabbMax, v);	                    
					}	                
				}
	            
				aabbMin = aabbMin + position;	            
				aabbMax = aabbMax + position;
       
			}
		}
		
		
		/// @if LANG_EN
		/// <summary> Check whether the point is inside rigid body </summary>
        /// <param name="pos"> given point </param>
        /// <param name="con"> shape of rigid body </param>
        /// <returns> if it is inside, true is reaturne </returns>
  		/// @endif
		/// @if LANG_JA
		/// <summary> 頂点が剛体の領域に含まれるかどうか </summary>
        /// <param name="pos"> 与えられた頂点 </param>
        /// <param name="con"> 剛体の形状</para> </param>
        /// <returns> 頂点が剛体の領域に入って入れば、真を返す</returns>
  		/// @endif
		public bool InsideBody(Vector2 pos, PhysicsShape con)
		{
			// Check for AABB bounding box at first
			if(pos.X < aabbMin.X) return false;
			if(pos.X > aabbMax.X) return false;
			if(pos.Y < aabbMin.Y) return false;
			if(pos.Y > aabbMax.Y) return false;
	
			float ca = (float)Math.Cos(-rotation);
			float sa = (float)Math.Sin(-rotation);

			Vector2 test_vec = new Vector2(0, 0);
			test_vec = pos - position;

			test_vec = new Vector2(
						ca*test_vec.X - sa*test_vec.Y,
						sa*test_vec.X + ca*test_vec.Y 
						);

			float ca_local = (float)Math.Cos(-localRotation);
			float sa_local = (float)Math.Sin(-localRotation);
			
			test_vec = test_vec - localPosition;

			test_vec = new Vector2(
						ca_local*test_vec.X - sa_local*test_vec.Y,
						sa_local*test_vec.X + ca_local*test_vec.Y 
						);
			
			// Position should be changed to local coordinate system
			return con.InsideConvex(test_vec);
			
		}
		
		/// @if LANG_EN
		/// <summary> Reset the mass of rigid body </summary>
        /// <param name="m">  given mass of rigid body to reset </param>	
  		/// @endif
		/// @if LANG_JA
		/// <summary> 剛体の質量をリセットする </summary>
        /// <param name="m"> リセットするための質量 </param>	
  		/// @endif
		void ResetBodyMass(float m)
		{
			if(type != BodyType.Dynamic)
				return;
			
			// Resetting mass of object causes the change of the tensor too
			float oldMass_save = mass;	
			mass = m;
			invMass = invMass * (oldMass_save/m);
			inertia = inertia * (m/oldMass_save);
			invInertia = invInertia * (oldMass_save/m);
		}
		
		/// @if LANG_EN  
		/// <summary> Set rigid body as Static </summary>
		/// <remarks> Once rigid body is set as static, it cannot be changed to dynamic rigid body any more</remarks>
  		/// @endif
		/// @if LANG_JA
		/// <summary> 剛体をスタティック剛体にセット </summary>
		/// <remarks> 剛体は一度スタティック剛体にセットされるとダイナミック剛体には戻せない </remarks>
  		/// @endif
		public void SetBodyStatic()
		{
			mass = PhysicsUtility.FltMax;
			inertia = PhysicsUtility.FltMax;
			invMass = 0;
			invInertia = 0;
			type = BodyType.Static;
		}
	
		/// @if LANG_EN 
		/// <summary> Set rigid body as Kinematic </summary>
		/// <remarks> make rigid body set as kinematic,
		/// and necessary properties are saved to old variables
        /// </remarks>	
   		/// @endif
		/// @if LANG_JA
		/// <summary> 剛体をキネマティック剛体にセット </summary>
		/// <remarks> 剛体をキネマティック剛体に設定,必要な情報を変数に保存する
        /// </remarks>
  		/// @endif
        public void SetBodyKinematic()
        {			
			if(type == BodyType.Dynamic)
			{
	            oldMass = mass;
	            oldInvMass = invMass;
	            oldInertia = inertia;
	            oldInvInertia = invInertia;
				
	            mass = PhysicsUtility.FltMax;
	            invMass = 0;
	            inertia = PhysicsUtility.FltMax;
	            invInertia = 0;
				
	            velocity = new Vector2(0, 0);
	            angularVelocity = 0.0f;
	
	            sleep = true;
				type = BodyType.Kinematic;
			}
        }
		
		/// @if LANG_EN 
		/// <summary> Set rigid body as Trigger </summary>
   		/// @endif
		/// @if LANG_JA
		/// <summary> 剛体をトリガー剛体に設定する </summary>
   		/// @endif
        public void SetBodyTrigger()
        {
			if(type == BodyType.Dynamic)
			{
				sleepCount = 0;
				sleep = false;
				type = BodyType.Trigger;
			}
        }
    
		/// @if LANG_EN 	
        /// <summary> Make kinematic rigid body go back to dynamic rigid body </summary>
        /// <remarks>
		/// make kinamatic rigid body go back to dynamic rigid body,
		/// and necessary properties are restored to each variables
        /// </remarks>
   		/// @endif
		/// @if LANG_JA
        /// <summary> キネマティック剛体をダイナミック剛体に戻す </summary>
        /// <remarks>
		/// キネマティック剛体をダイナミック剛体に戻し、必要な情報を元の変数にセットする
        /// </remarks>
   		/// @endif
        public void BackToDynamic()
        {
			if(type == BodyType.Kinematic)
			{
				mass = oldMass;
	            invMass = oldInvMass;
	            inertia = oldInertia;
	            invInertia = oldInvInertia;
	            sleep = false;
				sleepCount = 0;
				type = BodyType.Dynamic;
			}
			else if(type == BodyType.Trigger)
			{
				type = BodyType.Dynamic;	
			}
			else
			{
				// do nothing here
			}
        }
		
		/// @if LANG_EN
		/// <summary> Wake up dynamic object which is sleeping </summary>
		/// <remarks> same as sleep = false; sleepCount = 0;</remarks>
   		/// @endif
		/// @if LANG_JA
		/// <summary> スリープ状態のダイナミック剛体を起こす </summary>
		/// <remarks> sleep = false; sleepCount = 0;　と同じ</remarks>
   		/// @endif
        public void WakeUpDynamic()
		{
			if(IsDynamic())
			{
				sleep = false;
				sleepCount = 0;
			}
		}
	
		/// @if LANG_EN
		/// <summary> Wake up kinematic object which is sleeping </summary>
		/// <remarks> When kinematic object is moving, it is required for kinematic object not to be in sleep for correct collision <br />
		/// same as sleep = false; sleepCount = 0;</remarks>
   		/// @endif
		/// @if LANG_JA
		/// <summary> スリープ状態のキネマティック剛体を起こす </summary>
		/// <remarks> キネマティック剛体を動かす時にはスリープ状態から起こさないと正しく衝突判定が行われません <br />
		/// sleep = false; sleepCount = 0;　と同じ</remarks>
   		/// @endif
        public void WakeUpKinematic()
		{
			if(IsKinematic())
			{
				sleep = false;
				sleepCount = 0;
			}
		}
		
		/// @if LANG_EN
		/// <summary> Check whether object is kinematic or static </summary>
   		/// @endif
		/// @if LANG_JA
		/// <summary> 剛体がキネマティックまたはスタティック剛体であるかを判定する </summary>
   		/// @endif
        public bool IsKinematicOrStatic()
        {
			return ((type == BodyType.Kinematic) || (type == BodyType.Static));
		}
		
		/// @if LANG_EN
        /// <summary> Check whether object is kinematic </summary>
   		/// @endif
		/// @if LANG_JA
        /// <summary> 剛体がキネマティックであるかを判定する </summary>
   		/// @endif
		public bool IsKinematic()
		{
			return (type == BodyType.Kinematic);
		}
		
		/// @if LANG_EN
        /// <summary> Check whether object is static </summary>
   		/// @endif
		/// @if LANG_JA
        /// <summary> 剛体がスタティックであるかを判定する </summary>
   		/// @endif
		public bool IsStatic()
		{
			return (type == BodyType.Static);
		}
		
		/// @if LANG_EN
        /// <summary> Check whether object is dynamic </summary>
   		/// @endif
		/// @if LANG_JA
        /// <summary> 剛体がダイナミックであるかを判定する </summary>
   		/// @endif
		public bool IsDynamic()
		{
			return (type == BodyType.Dynamic);
		}
		
		/// @if LANG_EN
     	/// <summary> Check whether object is trigger </summary>
   		/// @endif
		/// @if LANG_JA
		/// <summary> 剛体がトリガーであるかを判定する </summary>
   		/// @endif
		public bool IsTrigger()
		{
			return (type == BodyType.Trigger);
		}
		
		/// @if LANG_EN
     	/// <summary> Apply force to the center of mass </summary>
        /// <param name="power"> given force </param>
        /// <remarks> available only for dynamic rigid body </remarks> 
		/// /// @endif
		/// @if LANG_JA
		/// <summary> 重心に力を加える </summary>
        /// <param name="power"> 与えられる力 </param>
        /// <remarks> ダイナミック剛体のみに有効 </remarks>
   		/// @endif
		public void ApplyForce(Vector2 power)
		{
			if(IsDynamic() == false)
				return;
			
			force = power;
		}
		
		/// @if LANG_EN
     	/// <summary> Apply force to any point </summary>
        /// <param name="power"> given force </param>
        /// <param name="pos"> given force point </param>
        /// <param name="isGlobal"> pos is represented by global axis ? or local axis ? global axis at default </param>
        /// <remarks> available only for dynamic rigid body </remarks>
   		/// @endif
		/// @if LANG_JA
		/// <summary> 任意の点に力を加える </summary>
        /// <param name="power"> 与えられる力 </param>
        /// <param name="pos"> 力の位置 </param>
        /// <param name="isGlobal"> 力の位置がグローバル座標かローカル座標かを示す。デフォルトはグローバル座標 </param>
        /// <remarks> ダイナミック剛体のみに有効 </remarks>
   		/// @endif
		public void ApplyForce(Vector2 power, Vector2 pos, bool isGlobal = true)
		{
			if(IsDynamic() == false)
				return;
			
			Vector2 localVector;
			
			if(isGlobal == false)
			{
				float ang = rotation;
				
				localVector = new Vector2(
					(float)System.Math.Cos(ang)*pos.X - (float)System.Math.Sin(ang)*pos.Y,
					(float)System.Math.Sin(ang)*pos.X + (float)System.Math.Cos(ang)*pos.Y );

			}
			else
			{
				localVector = pos - this.position;
			}
			
			force = power;
			torque = Vector2Util.Cross(localVector, power);
		}
		
		
		/// @if LANG_EN
     	/// <summary> Apply torque to the rigid body </summary>
        /// <param name="rotPower"> given torque </param>
        /// <remarks> available only for dynamic rigid body </remarks>
   		/// @endif
		/// @if LANG_JA
		/// <summary> 剛体にトルクを加える </summary>
        /// <param name="rotPower"> 与えられるトルク </param>
        /// <remarks> ダイナミック剛体のみに有効 </remarks>
   		/// @endif
		public void ApplyTorque(float rotPower)
		{
			if(IsDynamic() == false)
				return;
			
			torque = rotPower;
		}
		
		//
		// Set & Get methods
		//
		// For this version, most of variables are defined as public 
		// and you can access variables directly too
		// 
		// But should be care about mass, invMass, inertia, invInertia
		// because there are relations each other
		//
		
		/// @if LANG_EN
		/// <summary> setter and getter of position </summary>
       	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of position </summary>
       	/// @endif
		public Vector2 Position
		{
			set { this.position = value; }
			get { return this.position; }
		}
				
		/// @if LANG_EN
		/// <summary> setter and getter of rotation </summary>
	    /// @endif
		/// @if LANG_JA 
		/// <summary> setter and getter of position </summary>
       	/// @endif
		public float Rotation
		{
			set { this.rotation = value; }
			get { return this.rotation; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of velocity </summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of velocity </summary>
	    /// @endif
		public Vector2 Velocity
		{
			set { this.velocity = value; }
			get { return this.velocity; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of angular velocity </summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of angular velocity </summary>
	    /// @endif
		public float AngularVelocity
		{
			set { this.angularVelocity = value; }
			get { return this.angularVelocity; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of local position </summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of local position </summary>
	    /// @endif
		public Vector2 LocalPosition
		{
			set { this.localPosition = value; }
			get { return this.localPosition; }
		}

		/// @if LANG_EN
		/// <summary> setter and getter of local rotation </summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of local rotation </summary>
	    /// @endif
		public float LocalRotation
		{
			set { this.localRotation = value; }
			get { return this.localRotation; }
		}
		
		/// @if LANG_EN	
		/// <summary> setter and getter of acceleration </summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of acceleration </summary>
	    /// @endif
		public Vector2 Acceleration
		{
			// no set method, read only
			set { this.acceleration = value; }
			get { return this.acceleration; }
		}
		
		/// @if LANG_EN	
		/// <summary> setter and getter of force </summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of force </summary>
	    /// @endif
		public Vector2 Force
		{
			set { this.force = value; }
			get { return this.force; }
		}
		
		/// @if LANG_EN	
		/// <summary> setter and getter of torque </summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of torque </summary>
	    /// @endif
		public float Torque
		{
			set { this.torque = value; }
			get { return this.torque; }
		}
		
		/// @if LANG_EN	
		/// <summary> setter and getter of mass </summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of mass </summary>
	    /// @endif
		public float Mass
		{
			set { this.mass = value; this.invMass = 1.0f/value; }
			get { return this.mass; }
		}
		
		/// @if LANG_EN	
		/// <summary> setter and getter of inverse mass </summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of inverse mass </summary>
	    /// @endif 
		public float InvMass
		{
			set { this.invMass = value; this.mass = 1.0f/value; }
			get { return this.invMass; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of inertia tensor</summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of inertia tensor</summary>
	    /// @endif
		public float Inertia
		{
			set { this.inertia = value; this.invInertia = 1.0f/value;}
			get { return this.inertia; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of inverse inertia tensor</summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of inverse inertia tensor</summary>
	    /// @endif
		public float InvInertia
		{
			set { this.invInertia = value; this.inertia = 1.0f/value; }
			get { return this.invInertia; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of Width</summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of Width</summary>
	    /// @endif
		public Vector2 Width
		{
			set { this.width = value; }
			get { return this.width; } 
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of AaabbMin</summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of AaabbMin</summary>
	    /// @endif
		public Vector2 AabbMin
		{
			set { this.aabbMin = value; }
			get { return this.aabbMin; } 
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of aabbMax</summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of aabbMax</summary>
	    /// @endif
		public Vector2 AabbMax
		{
			set { this.aabbMax = value; }
			get { return this.aabbMax; } 
		}
		
		/// @if LANG_EN
		/// <summary>  getter of broadCollide</summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary>  getter of broadCollide</summary>
	    /// @endif
		public bool BroadCollide
		{
			get { return this.broadCollide; }
		}
		
		/// @if LANG_EN
		/// <summary> getter of narrowCollide</summary>
	    /// @endif
		/// @if LANG_JA
		/// <summary> getter of narrowCollide</summary>
	    /// @endif
		public bool NarrowCollide
		{
			get { return this.narrowCollide; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of shape index</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of shape index</summary>
		/// @endif
		public uint ShapeIndex
		{
			set { this.shapeIndex = value; }
			get { return this.shapeIndex; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of collisionFilter</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of collisionFilter</summary>
		/// @endif
		public uint CollisionFilter
		{
			set { this.collisionFilter = value; }
			get { return this.collisionFilter; }
		}

		/// @if LANG_EN
		/// <summary> setter and getter of groupFilter</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of groupFilter</summary>
		/// @endif
		public uint GroupFilter
		{
			set { this.groupFilter = value; }
			get { return this.groupFilter; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of airFriction</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of airFriction</summary>
		/// @endif
		public float AirFriction
		{
			set { this.airFriction = value; }
			get { return this.airFriction; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of colFriction</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of airFriction</summary>
		/// @endif
		public float ColFriction
		{
			set { this.colFriction = value; }
			get { return this.colFriction; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of sleep</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sleep</summary>
		/// @endif
		public bool Sleep
		{
			set { this.sleep = value; }
			get { return this.sleep; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of sleepCount</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of sleepCount</summary>
		/// @endif
		public uint SleepCount
		{
			set { this.sleepCount = value; }
			get { return this.sleepCount; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of picking</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of picking</summary>
		/// @endif
		public bool Picking
		{
			set { this.picking = value; }
			get { return this.picking; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of type</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of type</summary>
		/// @endif
		public BodyType Type
		{
			set { this.type = value; }
			get { return this.type; }
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
			get { return this.penetrationRepulse; }
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
			get { return this.penetLimit; }
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
			get { return this.tangentFriction; }
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
			get { return this.restitutionCoeff; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of userData </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of userData </summary>
		/// @endif
		public object UserData  
		{
			set { this.userData = value; }
			get { return this.userData; }
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
			
	/// @if LANG_EN
	/// <summary> Contains actual contact point for collision pair </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary> 衝突した剛体ペアの衝突情報を保持している </summary>
	/// @endif
	public struct PhysicsSolverPair
	{
	    #region Fields
		/// @if LANG_EN
		/// <summary> index created by shifting and summing to represent both index of the pair</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>シフト演算と和によって合成されたペアのインデックス</summary>
		/// @endif
	    public uint totalIndex;
		
		/// @if LANG_EN
		/// <summary>normal direction of collision response</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>反発法線方向</summary>
		/// @endif
        public Vector2 normal;
		
		/// @if LANG_EN	
		/// <summary>contact point for bodyA of PhysicsSolverPair</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>bodyAに対して見た時の衝突点</summary>
		/// @endif
        public Vector2 resA;
		
		/// @if LANG_EN	
		/// <summary>contact point bodyB of PhysicsSolverPair</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>bodyBに対して見た時の衝突点</summary>
		/// @endif
		public Vector2 resB;		

        internal float Pn;
		
        internal float Pt;
	
        internal float penetrationDepth;
		
		internal float massNormal;
	
		internal float massTangent;
		
		internal Vector2 centerA, centerB;
        #endregion

		#region Methods
		/// @if LANG_EN	
		/// <summary> return index1 of PhysicsSolverPair </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> ペアの剛体Ａのインデックスを返す </summary>
		/// @endif
		public uint GetIndexA()
		{
			return PhysicsUtility.UnpackIdx1(totalIndex);
		}
		/// @if LANG_EN
		/// <summary> return index2 of PhysicsSolverPair </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> ペアの剛体Ｂのインデックスを返す </summary>
		/// @endif
		public uint GetIndexB()
		{
			return PhysicsUtility.UnpackIdx2(totalIndex);	
		}
		
		/// @if LANG_EN	
		/// <summary> get impluse for normal direction </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 法線方向の撃力を返す </summary>
		/// @endif
		public float GetImpulseNormal()
		{
			return Pn;
		}
		
		/// @if LANG_EN
		/// <summary> get impluse for tangent direction </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 接線方向の撃力を返す </summary>
		/// @endif
		public float GetImpulseTangent()
		{
			return Pt;	
		}
		
		//
		// Set & Get methods
		//
		
		/// @if LANG_EN
		/// <summary> setter and getter of totalIndex</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of totalIndex</summary>
		/// @endif
		public uint TotalIndex
		{
			set { this.totalIndex = value; }
			get { return this.totalIndex; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of normal</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of normal</summary>
		/// @endif
		public Vector2 Normal
		{
			set { this.normal = value; }
			get { return this.normal; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of resA</summary>
		/// @endif
		/// @if LANG_JA 
		/// <summary> setter and getter of resA</summary>
		/// @endif
		public Vector2 ResA
		{
			set { this.resA = value; }
			get { return this.resA; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of resB</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of resB</summary>
		/// @endif
		public Vector2 ResB
		{
			set { this.resB = value; }
			get { return this.resB; }
		}
        #endregion
	}

}
