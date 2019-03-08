/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using DemoGame;

namespace ShootingDemo
{

/**
 * ChargeAttackクラス
 */
public class ChargeAttack : Mono
{
    private enum ChargeStep
    {
        Wait,
        Start,
        Complete,
        Continue,
        End
    }

    private const long waitTime = 500;
    private const long chargeTime = 2000;

    private ModelAction action = null;
    private ChargeStep chargeStep;
    private long timer;

    public override float ZParam {
        get {return WorldPosition.X - 2;}
    }

    /// コンストラクタ
    public ChargeAttack() : base("ChargeAttack")
    {
    }

    /// 開始処理
    public override bool Start(MonoManager monoManager)
    {
        action = new ModelAction();
        action.Add("Start",    new List<ModelAnim>() {MonoModel.createAnim3("ChargeStart")});
        action.Add("Complete", new List<ModelAnim>() {MonoModel.createAnim3("ChargeComplete", 0, RepeatMode.Constant)});
        action.Add("Continue", new List<ModelAnim>() {MonoModel.createAnim3("ChargeContinue")});
        action.Add("End",      new List<ModelAnim>() {MonoModel.createAnim3("ChargeEnd", 0, RepeatMode.Constant)});

        action.SetCurrent(null);

        chargeStep = ChargeStep.Wait;
        timer = 0;

        return true;
    }

    /// 終了処理
    public override bool End(MonoManager monoManager)
    {
        action.Dispose();
        action = null;

        AudioManager.StopSound("ChargeStart");
        AudioManager.StopSound("ChargeEnd");

        return true;
    }

    /// 更新処理
    public override bool Update(MonoManager monoManager)
    {
        if (chargeAttackHandle(monoManager)) {
            return true;
        }

        if (Parent == null || Parent.MonoLifeState == MonoLifeState.Explode) {
            monoManager.Remove(this);
            return true;
        }

        action.Update(GameData.FrameTimeMillis);

        if (Parent != null) {
            WorldMatrix = Parent.WorldMatrix;
        }

        return true;
    }

    /// 描画処理
    public override bool Render(MonoManager monoManager)
    {
        Matrix4 matrix = WorldMatrix;
        action.Render(ref matrix);

        return true;
    }


    /// 溜め攻撃制御
    private bool chargeAttackHandle(MonoManager monoManager)
    {
        var pad = InputManager.InputGamePad;

        if (((pad.Scan & InputGamePadState.Cross) != 0)||((pad.Scan & InputGamePadState.Circle) != 0)) {
            switch (chargeStep) {
            case ChargeStep.Wait:
                timer += GameData.FrameTimeMillis;
                if (timer > waitTime) {
                    timer = 0;
                    chargeStep = ChargeStep.Start;
                    action.ChangeCurrent("Start");
                    AudioManager.PlaySound("ChargeStart", false);
                }
                break;
            case ChargeStep.Start:
                timer += GameData.FrameTimeMillis;
                if (timer > chargeTime) {
                    chargeStep = ChargeStep.Complete;
                    action.ChangeCurrent("Complete");
                    AudioManager.StopSound("ChargeStart");
                    AudioManager.PlaySound("ChargeEnd", false);
                }
                break;
            case ChargeStep.Complete:
                if (action.IsEndAction()) {
                    chargeStep = ChargeStep.Continue;
                    action.ChangeCurrent("Continue");
                }
                break;
            }
        }

        if (((pad.Free & InputGamePadState.Cross) != 0)||((pad.Free & InputGamePadState.Circle) != 0)) {
            switch (chargeStep) {
            case ChargeStep.Wait:
            case ChargeStep.Start:
                ShootBullet1(monoManager);
                monoManager.Remove(this);
                return true;
            case ChargeStep.Complete:
            case ChargeStep.Continue:
                chargeStep = ChargeStep.End;
                action.ChangeCurrent("End");

                monoManager.Regist(new ChargeBullet(WorldMatrix, GroupId, "ChargeBullet"));

                /// 照り返し
                Stage stage = (Stage)monoManager.FindMono("Stage");

                if ( stage != null ) {
                    stage.setLightStatus();
                }

                break;
            }
        }

        if (chargeStep == ChargeStep.End && action.IsEndAction()) {
            monoManager.Remove(this);
            return true;
        }

        return false;
    }
}

} // ShootingDemo
