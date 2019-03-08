/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


namespace UnitSys{

    //public enum UnitState{
    //    Sleep,
    //    Active,
    //}

public abstract class Unit
{
	private bool isEnd = false;
    protected UnitHandle handle = null;
    protected UnitModel model = null;


    /// コンストラクタ
    public Unit( UnitHandle handle,
                 UnitModel model )
    {
        this.handle = handle;
        this.model = model;
    }

    /// デストラクタ
    ~Unit()
    {
    }

    /// UnitManager に登録されたときに呼び出されるハンドラ
    public abstract bool OnStart( UnitManager unitMng );

    /// UnitManager の登録から削除されたときに呼び出されるハンドラ
    public abstract bool OnEnd( UnitManager unitMng );

    /// UnitManager に対する Regist時呼び出しのインターフェース(friend)
    public bool Start( UnitManager unitMng )
    {
        if( this.OnStart( unitMng ) == false ){
            return false;
        }
        if( handle != null && (isEnd == false)){
            if( handle.OnStart( unitMng, this ) == false ){
                return false;
            }
        }
        if( model != null && (isEnd == false)){
            if( model.OnStart( unitMng.CommonData, this ) == false ){
                return false;
            }
        }
        return true;
    }

    /// UnitManager に対する Remove 時呼び出しのインターフェース(friend)
    /**
     * Model -> Handle -> Unit
     */
    public bool End( UnitManager unitMng )
    {
        bool ret = true;

        if( model != null ){
            if( model.OnEnd( unitMng.CommonData, this ) == false ){
                ret = false;
            }
        }

        if( handle != null ){
            if( handle.OnEnd( unitMng, this ) == false ){
                ret = false;
            }
        }

        if( this.OnEnd( unitMng ) == false ){
            ret = false;
        }
		isEnd = true;
        return ret;
    }


    /// 時間差分更新処理
    public bool Update( UnitManager unitMng, float delta )
    {
        if( handle != null && (isEnd == false)){
            if( handle.OnUpdate( unitMng, this, delta  ) == false ){
                return false;
            }
        }
        if( model != null && (isEnd == false)){
            if( model.OnUpdate( unitMng.CommonData, this, delta ) == false ){
                return false;
            }
        }
        return true;
    }

    /// 描画処理
    public bool Render( UnitManager unitMng )
    {
        if( model != null && (isEnd == false)){
            return model.OnRender( unitMng.CommonData, this );
        }
        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
