/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

// 2D Physicsフレームワークをインクルード
using Sce.PlayStation.HighLevel.Physics2D;
namespace Physics2dDemo
{
public class Stage : PhysicsScene
{
	public float scene_scale = 1.0f;
	public TargetManager objectManager;
    // デバッグ表示用の頂点バッファ
    public VertexBuffer colVert = null;

	// ジョイントレンダリング用の頂点バッファ
    public VertexBuffer jointVert = null;

	// オブジェクトレンダリング用の頂点バッファ
    public VertexBuffer[] vertices = new VertexBuffer[50];

	// 作業員（投げる物体）のsceneBodiesインデックス
	public int throwObjIdx = 0;
		
	// ステージ2のドラム缶sceneBodiesインデックス
	public int stageToDrumIdx = 0;

    // stage1:クレーン箱に衝突したかどうか
	public bool isCrane_st1 = false;
    // stage3:クレーンドラム缶に衝突したかどうか
	public bool isCrane_st3 = false;
		
	/// 作業員が投げられたフラグ
	public bool throwFlag = false;
	public bool ThrowFrag
	{
		get{return throwFlag;}
		set{throwFlag = value;}
	}
	
	/// 作業員が投げられた回数
	public int throwCnt = 0;
	public int ThrowCnt
	{
		get{return throwCnt;}
		set{throwCnt = value;}
	}
	
	/// フリックのベース位置
	public static Vector2 flickBasePos = new Vector2(0,0);
	public static Vector2 FlickBasePos
	{
		get{return flickBasePos;}
		set{flickBasePos = value;}
	}
	
	/// 現在のフリック位置
	public static Vector2 flickCurrPos = new Vector2(0,0);
	public static Vector2 FlickCurrPos
	{
		get{return flickCurrPos;}
		set{flickCurrPos = value;}
	}

	public Stage ()
	{
	}

	~Stage ()
	{
		ReleaseScene();
	}

	/// シーンの解放
    public override void ReleaseScene()
    {
		for(int i = 0; i < numShape; i++)
            if (vertices[i] != null)
                vertices[i].Dispose();
					
		if(colVert != null) colVert.Dispose();
        if(jointVert != null) jointVert.Dispose();

		if(objectManager != null){
			objectManager.Dispose();
			objectManager = null;
		}
	}

	/// 2D素材の生成
	/// 
	public virtual void CreateObject()
	{
	}

	/// 剛体との位置を合わせる
	public virtual void FittingPosition()
	{
	}
		
	/// 2Dレイアウトをスクロールに合わせる
	public void Scroll(int x)
	{
		if(objectManager != null)
			objectManager.Scroll(x);
	}
	
    /// オブジェクト表示のためのライン(デバッグ用)
	///
	/// @param [in] con       PhysicsShape
	/// @param [in] vertices  VertexBuffer
	///
    public void makeLineList_Convex(PhysicsShape con, VertexBuffer vertices)
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

	/// シミュレーション前のアクション定義
	/// 位置に関わることはここで
    public override void UpdateFuncBeforeSim()
	{
		if(throwFlag){
			//物体を飛ばす	
			sceneBodies[throwObjIdx].BackToDynamic();
			sceneBodies[throwObjIdx].velocity += new Vector2((flickCurrPos.X - flickBasePos.X)*8.0f*scene_scale, (flickCurrPos.Y - flickBasePos.Y)*9.0f*scene_scale);
			throwFlag = false;
		}
	}
	
	/// オブジェクトのレンダリング（デバッグ用）
	///
	/// @param [in] graphics       GraphicsContext
	/// @param [in] program        ShaderProgram
	/// @param [in] renderMatrix   Matrix4
	/// @param [in] click_index    int
	///
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
                    if (sceneBodies[i].sleep == true)
                    {
                        Vector3 color = new Vector3(1.0f, 1.0f, 0.0f);
                        program.SetUniformValue(1, ref color);
                    }
                    else
                    {
                        Vector3 color = new Vector3(0.0f, 1.0f, 1.0f);
                        program.SetUniformValue(1, ref color);
                    }
                }

                if (sceneShapes[index].numVert == 0)
                    graphics.DrawArrays(DrawMode.TriangleStrip, 0, 37);
                else
                    graphics.DrawArrays(DrawMode.TriangleStrip, 0, sceneShapes[index].numVert + 1);

                {
                    Vector3 color = new Vector3(0.0f, 0.0f, 1.0f);
                    program.SetUniformValue(1, ref color);
                }

                if (sceneShapes[index].numVert == 0)
                    graphics.DrawArrays(DrawMode.LineStrip, 0, 37);
                else
                    graphics.DrawArrays(DrawMode.LineStrip, 0, sceneShapes[index].numVert + 1);
            }

        }
	}
		
    /// 衝突点(RigidBody A <=> RigidBody B)とAABB(Axis Aligned Bounding Box)のデバッグ表示
	///
	/// @param [in] graphics       GraphicsContext
	/// @param [in] program        ShaderProgram
	/// @param [in] renderMatrix   Matrix4
	///
    public override void DrawAdditionalInfo(ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 renderMatrix)
    {
	}
	
	public void ObjectDraw()
	{
		if(objectManager != null)
			objectManager.Render();
	}		
		
	/// 指定のsceneBodiesのPositionを取得
	public Vector2 GetsceneBodiesPosition(int index)
	{	
		return sceneBodies[index].Position;
	}

	/// 指定のsceneBodiesのInertiaを取得
	public float GetsceneBodiesInertia(int index)
	{	
		return sceneBodies[index].Inertia;
	}

	/// 指定のsceneBodiesのVelocityを取得
	public Vector2 GetsceneBodiesVelocity(int index)
	{	
		return sceneBodies[index].Velocity;
	}

	/// 指定のsceneBodiesのRotateを取得
	public float GetsceneBodiesRotation(int index)
	{	
		return sceneBodies[index].Rotation;
	}

	/// 全てのsceneBodiesを静態にする
	public void SetsceneBodiesKinematic()
	{	
		for(int i=0; i<numBody; i++)
			sceneBodies[i].SetBodyKinematic();
	}
		
	/// 指定のsceneBodiesを静態にする（ジョイントしている剛体は避ける）
	public virtual void SetsceneBodiesKinematic(int idx)
	{	
		sceneBodies[idx].SetBodyKinematic();
	}
	
}
}

