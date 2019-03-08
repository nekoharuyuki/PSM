/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace FlightDemo{

public class EnemyModel
    : FlightUnitModel
{
    private float animTime;
    private BasicModel model;
    private string nameModel;

    /// コンストラクタ
    public EnemyModel( string nameModel )
    {
        this.nameModel = nameModel;
    }

    /// デストラクタ
    ~EnemyModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        model = gameData.ModelContainer.Find( nameModel );
        animTime = 0.0f;
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        // 所有権なし
        model = null;
        return true;
    }

    /// アニメーションの更新処理
    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {
        animTime += delta;
        animTime %= model.GetMotionLength( 0 );

        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();
        EnemyUnit myUnit = unit as EnemyUnit;

        model.WorldMatrix = unit.GetPosture();
        model.Update();
        model.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );

        
        for( int i = 0; i < myUnit.GetCollisionCount(); i++ ){
            DbgSphere.Draw( graphics, gameData.GetViewProj(), 
                            myUnit.GetPosture(),
                            myUnit.GetCollision( i ) );
        }

        return true;
    }

}


} // end ns FlightDemo
//===
// EOF
//===
