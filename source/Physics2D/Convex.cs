/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;

//
//	Collision Shape Definition
//
namespace Sce.PlayStation.HighLevel.Physics2D
{	
	/// @if LANG_EN
	/// <summary> All shapes are defined as convex type </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary> 全ての形状は凸形状としてセットされます </summary>
	/// @endif
	public partial class PhysicsShape
    {
		#region Fields
		/// @if LANG_EN
		/// <summary> number of points </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> 頂点数 </summary>
		/// @endif
        public int numVert;

		/// @if LANG_EN
		/// <summary> list of points </summary>
		/// <remarks> assume maximum number of vertex  = 30,
		/// numVert = 0 means sphere, this is the special case 
		/// and the first x-coordinate of the point holds radius</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> 頂点リスト </summary>
		/// <remarks>  頂点リスト、最大で３０点まで、numVert = 0 は
		/// 球を表す特別なケースで最初の点のＸ座標に半径が記録されている</remarks>
		/// @endif
        public Vector2[] vertList = new Vector2[30];
		
		/// @if LANG_EN
		/// <summary> hint for PhysicsShape </summary>
		/// <remarks> hint defines symmetry of PhysicsShape
		/// hint=1 means non symmetry, hint=2 means symmetry
		/// except for sphere and box, convex shape should define this property to accelarate the  
		/// simulation when usr uses convex shape </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> PhysicsShapeに対するヒント </summary>
		/// <remarks> PhysicsShapeの対称性を判定している
		/// hint=1は形状が対称性を有していないケースで, hint=2は形状が対称性を有している,
		/// 任意に凸形状を指定した場合、球とボックスの場合を除いて、シミュレーション速度を上げるため、
		/// 適切にこの変数を指定することが望ましい </remarks>
		/// @endif
		public uint hint;
		#endregion
		
		#region Methods
		/// @if LANG_EN
		/// <summary> Default Constructor </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> デフォルトコンストラクタ </summary>
		/// @endif
		public PhysicsShape()
		{
			numVert = 30;
			hint = 1;
			for(int i=0; i<30; i++)
			{
				vertList[i] = new Vector2(0.0f, 0.0f);
			}
		}
		
		/// @if LANG_EN
		/// <summary> Sphere Constructor </summary>
        /// <param name="radius"> radius of sphere </param>
		/// @endif
		/// @if LANG_JA
		/// <summary> 球生成 </summary>
        /// <param name="radius"> 球の半径 </param>
		/// @endif
        public PhysicsShape(float radius)
        {
            numVert = 0;
			hint = 1;
            vertList[0] = new Vector2(radius, radius);
        }
		
		/// @if LANG_EN
		/// <summary> Box Constructor </summary>
        /// <param name="width"> half width of box </param>
        /// <remarks> width param is half width of box</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> ボックス生成 </summary>
        /// <param name="width"> ボックスの半分の縦横長さ </param>
        /// <remarks> 縦横長さではなく、縦横半分の長さである </remarks>
		/// @endif
		public PhysicsShape(Vector2 width)
        {
            numVert = 4;
			hint = 2;
	        vertList[0] = new Vector2(-width.X, -width.Y);
            vertList[1] = new Vector2(width.X, -width.Y);
            vertList[2] = new Vector2(width.X, width.Y);
            vertList[3] = new Vector2(-width.X, width.Y);
        }
			
		/// @if LANG_EN
		/// <summary> Convex Constructor </summary>
        /// <param name="pos"> point list for convex shape </param>
        /// <param name="num"> num of point list for convex shape </param>
        /// <remarks> the limitation of number of point list containd by convex shape is 30</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> 凸形状生成 </summary>
        /// <param name="pos"> 凸形状の頂点リスト </param>
        /// <param name="num"> 頂点リストの数 </param>
        /// <remarks> 頂点リストの数の最大は３０個 </remarks>
		/// @endif
		public PhysicsShape(Vector2[] pos, int num)
		{
			// truncate the number of points if it is over 30
			if(num > 30)
			{
				num = 30;
				Console.WriteLine("Physics2D warning: num of vertex is over 30 and truncated to 30");
			}
			
			numVert = num;
			hint = 1;
			for(int i=0; i<numVert; i++)
			{
				vertList[i] = new Vector2(pos[i].X, pos[i].Y);
			}
		}
		
		/// @if LANG_EN
		/// <summary> Chain of Line Segments Constructor </summary>
        /// <param name="pos"> point list for line shape </param>
        /// <param name="num"> num of point list for convex shape </param>
        /// <param name="flag"> true should be given </param>
        /// <remarks> the limitation of number of point list containd by convex shape is 30, and this is only for static rigid body</remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> 接続したライン形状生成 </summary>
        /// <param name="pos"> ライン形状の頂点リスト </param>
        /// <param name="num"> 頂点リストの数 </param>
        /// <param name="flag"> trueを常に与える </param>
        /// <remarks> 頂点リストの数の最大は３０個であり、また、これによって作れるのは静的剛体のみです</remarks>
		/// @endif
		public PhysicsShape(Vector2[] pos, int num, bool flag)
		{			
			if(flag != true)
			{
				Console.WriteLine("flag should be set as true");
				Console.WriteLine("Creation of PhysicsShape error");
				return;
			}
			
			// truncate the number of points if it is over 30
			if(num > 30)
			{
				num = 30;
				Console.WriteLine("Physics2D warning: num of vertex is over 30 and truncated to 30");
			}
			
			numVert = num;
			// hint = 0 means that it does not represent convex shape snd represents line
			hint = 0;
			for(int i=0; i<numVert; i++)
			{
				vertList[i] = new Vector2(pos[i].X, pos[i].Y);
			}
		}
		
		/// @if LANG_EN
		/// <summary> PhysicsShape Copy Constructor </summary>
        /// <param name="con"> shape of rigid body </param>
		/// @endif
		/// @if LANG_JA
		/// <summary> コピー形状生成 </summary>
        /// <param name="con"> 剛体の形状 </param>
		/// @endif
        public PhysicsShape(PhysicsShape con)
		{
            numVert = con.numVert;
			hint = con.hint;
		
			if(numVert != 0)
			{
	            for (int i = 0; i < numVert; i++)
	            {
					vertList[i] = con.vertList[i];
	            }
			}
			else
			{
				vertList[0] = con.vertList[0];	
			}
        }
		
		/// @if LANG_EN
		/// <summary> CopyFromConvex </summary>
		/// <remarks> copy function of PhysicsShape </remarks>
        /// <param name="con"> reference to shape of rigid body </param>
		/// @endif
		/// @if LANG_JA
		/// <summary> 形状コピー </summary>
		/// <remarks> PhysicsShapeのコピー関数 </remarks>
        /// <param name="con"> 剛体の形状への参照 </param>
		/// @endif
		internal void CopyFromConvex(ref PhysicsShape con)
		{
            numVert = con.numVert;
			hint = con.hint;
		
			if(numVert != 0)
			{
	            for (int i = 0; i < numVert; i++)
	            {
					vertList[i] = con.vertList[i];
	            }
			}
			else
			{
				vertList[0] = con.vertList[0];	
			}
		}
		
		/// @if LANG_EN
		/// <summary> Check whether a given point is inside this convex </summary>
        /// <param name="pos"> position, should be defined as relative position to the center of convex</param>
        /// <returns> whether a given point is inside this convex </returns>
		/// @endif
		/// @if LANG_JA
		/// <summary> 与えられた点が凸形状の中に入っているかどうか判定 </summary>
        /// <param name="pos"> 点は凸形状に対する相対位置で指定する必要あり </param>
        /// <returns> 与えられた点が凸形状の中に入っているかどうか </returns>
		/// @endif
		internal bool InsideConvex(Vector2 pos)
		{
			if(numVert != 0)
			{
				for(int i=0; i<numVert; i++)
				{
					Vector2 u = vertList[(i+1)%numVert] - vertList[i];
					Vector2 v = vertList[(i+2)%numVert] - vertList[(i+1)%numVert];
					Vector2 t = pos - vertList[i];
					
                    if (Vector2Util.Cross(u, v) * Vector2Util.Cross(u, t) < 0)
						return false;
				}
				return true;
			}
			else
			{
				if(Vector2.Dot(pos, pos) <= vertList[0].X * vertList[0].X)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// @if LANG_EN
		/// <summary> rough angle calculation by cos -> [0, 4] mapping  </summary>
        /// <param name="vec1"> one of 2d vector </param>
		/// <param name="vec2"> the other of 2d vector </param>
		/// @endif
		/// @if LANG_JA
		/// <summary> cos -> [0, 4] マッピングを使用した簡易な角度判定  </summary>
        /// <param name="vec1"> 一方の２次元ベクトル </param>
		/// <param name="vec2"> もう一方の２次元ベクトル </param>
		/// @endif
        internal static float GetTheta(Vector2 vec1, Vector2 vec2)
        {
            float dx = vec2.X - vec1.X;
            float dy = vec2.Y - vec1.Y;
            float len = (float)System.Math.Sqrt(dx * dx + dy * dy);

            if (len == 0)
                return 4.0f;

            float ccos = dx / len;

            if (dy >= 0)
            {
                return 1.0f - ccos;
            }
            else
            {
                return 3.0f + ccos;
            }
        }
		
		/// @if LANG_EN
		/// <summary> create convex shape from random point </summary>
        /// <param name="vertList"> random point set </param>
        /// <param name="num"> number of random point set </param> 
        /// <returns> convex shape </returns>
		/// @endif
		/// @if LANG_JA
		/// <summary> ランダム頂点から凸包を生成します </summary>
        /// <param name="vertList"> ランダムな頂点列 </param>
        /// <param name="num"> 頂点数 </param> 
        /// <returns> 凸形状 </returns>
		/// @endif
        public static PhysicsShape CreateConvexHull(Vector2[] vertList, int num)
        {
			// to ignore and remove very closest vertices
            const float convex_creation_dis = 0.03f;

            PhysicsShape result = new PhysicsShape();

            result.vertList[0] = vertList[0];

            for (int i = 0; i < num; i++)
            {
                if (vertList[i].Y < result.vertList[0].Y)
                {
                    result.vertList[0] = vertList[i];
                }
                else if (vertList[i].Y == result.vertList[0].Y)
                {
                    if (vertList[i].X > result.vertList[0].X)
                    {
                        result.vertList[0] = vertList[i];
                    }
                }
                else
                {
                    continue;
                }
            }

            float current_rad = 0.0f;
            float max_rad = 4.0f;

            for (int i = 0; i < num; i++)
            {
                max_rad = 4.0f;

                for (int j = 0; j < num; j++)
                {
                    float rad = GetTheta(result.vertList[i], vertList[j]);
                    //Console.WriteLine("rad = " + rad );

                    if ((rad <= max_rad) && (rad > current_rad))
                    {
                        result.vertList[i + 1] = vertList[j];
                        max_rad = rad;
                    }
                    else if ((rad <= max_rad) && (rad == current_rad))
                    {
                        Vector2 vec1, vec2;
                        vec1 = result.vertList[i] - vertList[j];
                        vec2 = result.vertList[i] - result.vertList[i + 1];

                        if (vec1.Length() > vec2.Length())
                        {
                            result.vertList[i + 1] = vertList[j];
                            max_rad = rad;
                        }
                    }
                    else
                    {
                        // do nothing
                    }
                }

                current_rad = max_rad;
				
				if(current_rad == 4.0)
				{
					result.numVert = i+1;
					break;
				}

                Vector2 vec_temp = result.vertList[i + 1] - result.vertList[0];
                if (Vector2.Dot(vec_temp, vec_temp) < 0.0001f)
                {
                    result.numVert = i + 1;
                    break;
                }
            }

            //
            // remove very close pair of points
            // to avoid simulation error
            //

            bool check_flag = true;

            while (check_flag)
            {
                check_flag = false;

                for (int i = 0; i < result.numVert - 1; i++)
                {

                    if (Vector2.Dot(result.vertList[i + 1] - result.vertList[i],
                        result.vertList[i + 1] - result.vertList[i]) < convex_creation_dis)
                    {
                        Console.WriteLine("Physics2D warning: there is a closest point inside Convex Shape");
                        Console.WriteLine("Physics2D warning: Removing this point ...");

                        check_flag = true;

                        for (int j = i + 1; j < result.numVert - 1; j++)
                        {
                            result.vertList[j] = result.vertList[j + 1];
                        }

                        i = i - 1;
                        result.numVert--;
                    }
                }
            }

            //
            // move all points based on the center of gravity
            //

            Vector2 center = new Vector2(0, 0);

            for (int i = 0; i < result.numVert; i++)
            {
                center += result.vertList[i];
            }

            center = (1.0f / result.numVert) * center;

            for (int i = 0; i < result.numVert; i++)
            {
                result.vertList[i] -= center;
            }

            return result;
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
		/// <summary> setter and getter of numVert </summary>
       	/// @endif
		/// @if LANG_JA
		/// <summary> setter and getter of numVert </summary>
       	/// @endif
		public int NumVert
		{
			set { this.numVert = value; }
			get { return this.numVert; }
		}
				
		/// @if LANG_EN
		/// <summary> setter and getter of vertList </summary>
	    /// @endif
		/// @if LANG_JA 
		/// <summary> setter and getter of vertList </summary>
       	/// @endif
		public Vector2[] VertList 
		{
			set { this.vertList = value; }
			get { return this.vertList; }
		}
		
		/// @if LANG_EN
		/// <summary> setter and getter of hint </summary>
	    /// @endif
		/// @if LANG_JA 
		/// <summary> setter and getter of hint </summary>
       	/// @endif
		public uint Hint
		{
			set { this.hint = value; }
			get { return this.hint; }
		}
		
		#endregion
    }
}
