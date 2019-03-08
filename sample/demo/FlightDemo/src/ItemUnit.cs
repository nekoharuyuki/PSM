/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core;
using DemoGame;

namespace FlightDemo{

/// アイテムの状態
public enum ItemState{
    Sleep,
    Normal,
    Success,
    Fail,
    Destroy
}


public class ItemUnit
    : FlightUnit
{
    private Matrix4 basePosture;
    private FlightUnitManager unitMng;
    private float baseTime = 0.0f;
    private ItemState state = ItemState.Sleep;
    private GeometrySphere collision;

    public float BaseTime{
        get{ return baseTime; }
    }

    public ItemState State{
        get{ return state; }
    }
    public FlightUnitManager ParentUnitMng{
        get{ return unitMng; }
    }

    public ItemUnit( float baseTime, FlightUnitHandle handle, FlightUnitModel model )
    : base( handle, model )
    {
        this.baseTime = baseTime;
    }

    ~ItemUnit()
    {
    }

    /// UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng )
    {
        this.unitMng = unitMng;
        SetBaseTime( unitMng.GameCommonData.FlightRoute, baseTime );

        collision = new GeometrySphere( new Vector3( 0.0f, 0.0f, 0.0f ), 0.5f );

        return true;
    }

    /// UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( FlightUnitManager unitMng )
    {
        // 所有権なし
        this.unitMng = null;
        return true;
    }


    /// 航路上に配置する
    public void SetBaseTime( FlightRoute route, float time )
    {
        this.baseTime = time;
        this.basePosture = route.BasePosture( time );
        SetTrans( 0.0f, 0.0f, 0.0f );
    }

    /// (Sleep状態のアイテムを)アクティブにする
    public void Active()
    {
        if( state == ItemState.Sleep ){
            state = ItemState.Normal;
            onActive();
        }
    }

    /// スリープ状態にする
    public void Sleep()
    {
        state = ItemState.Sleep;
        onSleep();
    }

    /// 取得できずに通り過ぎてしまった
    public void Fail()
    {
        state = ItemState.Fail;
        onFail();
    }

    /// 
    public void Success()
    {
        state = ItemState.Success;
        onSuccess();
    }

    /// 破棄を行ってよいことを通知
    /**
     * 破棄は、Handleが行う
     */
    public void Destroy()
    {
        state = ItemState.Destroy;
        onDestroy();
    }

    /// 基準位置からの相対的な位置指定
    public void SetTrans( float x, float y, float z )
    {
        this.SetPosture( Matrix4.Translation( new Vector3( x, y, z ) ) * this.basePosture );
    }

    /// 飛行機とのあたり判定を取る(汎用)
    public virtual bool IsHit( PlaneUnit plane )
    {
        Vector3 dir = this.GetPos() - plane.GetPos();
        float len = dir.Length();
        float r = this.GetCollision().R + plane.GetCollision().R;
		if( r > len ){
        	return true;
		}
		return false;
    }

    /// あたり判定の取得
    public GeometrySphere GetCollision()
    {
        return collision;
    }
    /// 
    protected virtual void onActive()
    {
    }

    /// ItemUnit.Sleep() が呼ばれたときに通知される
    protected virtual void onSleep()
    {
    }

    /// ItemUnit.Fail() が呼ばれたときに通知される
    protected virtual void onFail()
    {
        this.Destroy();
    }

    /// ItemUnit.Success() が呼ばれたときに通知される
    protected virtual void onSuccess()
    {
    }

    /// ItemUnit.Destroy() が呼ばれたときに通知される
    protected virtual void onDestroy()
    {
        this.ParentUnitMng.Remove( this );
    }

}

} // end ns FlightDemo
//===
// EOF
//===
