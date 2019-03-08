/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using DemoModel;

namespace ShootingDemo
{

/**
 * StageInfoクラス
 */
public class StageInfo : IDisposable
{
    protected UnitPostSchedule unitPostList = new UnitPostSchedule();

    /// エリアの間隔
    protected int areaWait = 105;

    /// シード
    protected System.Random rand = new System.Random();

    /// 乱数
    int randY, randZ;

    /// 構築
    public void Setup()
    {
        setupModel();
        setupShader();
        setupUnitPost();
    }

    /// ユニット配置
    private void setupUnitPost()
    {
        int score = 200;
        int interval = 0, total = 0;

        /// 1-1
        score = 200;
        interval = 10;
        unitPostList.AddEnemyAStraight( areaWait-(total%areaWait), new Vector3(0,  110, 500), new Vector3(  0, 180, 0), score,  0);
        total += areaWait-(total%areaWait);
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0,  110, 500), new Vector3(  0, 180, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0,  110, 500), new Vector3(  0, 180, 0), score, 20);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0,  110, 500), new Vector3(  0, 180, 0), score, 30);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0,  110, 500), new Vector3(  0, 180, 0), score, 40);
        total += interval;

        unitPostList.AddEnemyAStraight( areaWait-(total%areaWait), new Vector3(0, -160, 500), new Vector3(  0, 180, 0), score,  0);
        total += areaWait-(total%areaWait);
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0, -160, 500), new Vector3(  0, 180, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0, -160, 500), new Vector3(  0, 180, 0), score, 20);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0, -160, 500), new Vector3(  0, 180, 0), score, 30);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0, -160, 500), new Vector3(  0, 180, 0), score, 40);
        total += interval;

        unitPostList.AddEnemyAStraight( areaWait-(total%areaWait), new Vector3(0,  110, 500), new Vector3(  0, 180, 0), score,  0);
        total += areaWait-(total%areaWait);
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0,  110, 500), new Vector3(  0, 180, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0,  110, 500), new Vector3(  0, 180, 0), score, 20);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0,  110, 500), new Vector3(  0, 180, 0), score, 30);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0,  110, 500), new Vector3(  0, 180, 0), score, 40);
        total += interval;

        unitPostList.AddEnemyAStraight( areaWait-(total%areaWait), new Vector3(0, -160, 500), new Vector3(  0, 180, 0), score,  0);
        total += areaWait-(total%areaWait);
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0, -160, 500), new Vector3(  0, 180, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0, -160, 500), new Vector3(  0, 180, 0), score, 20);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0, -160, 500), new Vector3(  0, 180, 0), score, 30);
        total += interval;
        unitPostList.AddEnemyAStraight(                  interval, new Vector3(0, -160, 500), new Vector3(  0, 180, 0), score, 40);
        total += interval;

        /// 1-5
        score = 200;
        interval = 10;
        unitPostList.AddEnemyACurveD( areaWait-(total%areaWait), new Vector3(0,  280, 200), new Vector3( 125, 180, 0), score,  0);
        total += areaWait-(total%areaWait);
        unitPostList.AddEnemyACurveD(                  interval, new Vector3(0,  280, 200), new Vector3( 125, 180, 0), score,  5);
        total += interval;
        unitPostList.AddEnemyACurveD(                  interval, new Vector3(0,  280, 200), new Vector3( 125, 180, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyACurveD(                  interval, new Vector3(0,  280, 200), new Vector3( 125, 180, 0), score, 15);
        total += interval;
        unitPostList.AddEnemyACurveD(                  interval, new Vector3(0,  280, 200), new Vector3( 125, 180, 0), score, 20);
        total += interval;
        score = 400;
        unitPostList.AddEnemyACurveD(                  interval, new Vector3(0,  280, 200), new Vector3( 125, 180, 0), score, 25, true);
        total += interval;

        score = 200;
        unitPostList.AddEnemyACurveU( areaWait-(total%areaWait), new Vector3(0, -280, 200), new Vector3(-125, 180, 0), score,  0);
        total += areaWait-(total%areaWait);
        unitPostList.AddEnemyACurveU(                  interval, new Vector3(0, -280, 200), new Vector3(-125, 180, 0), score,  5);
        total += interval;
        unitPostList.AddEnemyACurveU(                  interval, new Vector3(0, -280, 200), new Vector3(-125, 180, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyACurveU(                  interval, new Vector3(0, -280, 200), new Vector3(-125, 180, 0), score, 15);
        total += interval;
        unitPostList.AddEnemyACurveU(                  interval, new Vector3(0, -280, 200), new Vector3(-125, 180, 0), score, 20);
        total += interval;
        score = 400;
        unitPostList.AddEnemyACurveU(                  interval, new Vector3(0, -280, 200), new Vector3(-125, 180, 0), score, 25, true);
        total += interval;

        /// 1-7
        score = 100000;
        interval = 30;
        unitPostList.AddEnemyBStraight( areaWait-(total%areaWait), new Vector3(-400, -60, -140), new Vector3(  0, 0, 0),true);
        total += areaWait-(total%areaWait);
        unitPostList.AddEnemyBStraight(                  interval, new Vector3(-380, -60, -230), new Vector3(  0, 0, 0));
        total += interval;

        score = 1000;
        interval = 120;
        unitPostList.AddEnemyBQuickMU(                   interval, new Vector3(0,  100, 500), new Vector3( 0, 180, 0), score);
        total += interval;
        interval = 0;
        unitPostList.AddEnemyBQuickMD(                   interval, new Vector3(0, -130, 500), new Vector3( 0, 180, 0), score);
        total += interval;

        /// 2-1
        score = 350;
        interval = 35;
        for (int i = 0; i < 3; i++) {
            randY = rand.Next(-200,150);
            randZ = rand.Next(-120,320);
            unitPostList.AddEnemyCNormal( (i==0?(areaWait-(total%areaWait)+areaWait):interval), new Vector3(-400, randY, randZ), new Vector3( 0, 90, 0), score);
            total += (i==0?(areaWait-(total%areaWait)):interval);
        }

        /// 2-2
        interval = 21;
        for (int i = 0; i < 5; i++) {
            randY = rand.Next(-200,150);
            randZ = rand.Next(-120,320);
            unitPostList.AddEnemyCNormal( (i==0?(areaWait-(total%areaWait)+areaWait):interval), new Vector3(-400, randY, randZ), new Vector3( 0, 90, 0), score);
            total += (i==0?(areaWait-(total%areaWait)):interval);
        }

        /// 2-3
        interval = 8;
        for (int i = 0; i < 14; i++) {
            randY = rand.Next(-200,150);
            randZ = rand.Next(-120,320);
            unitPostList.AddEnemyCNormal( (i==0?(areaWait-(total%areaWait)+areaWait):interval), new Vector3(-400, randY, randZ), new Vector3( 0, 90, 0), score);
            total += (i==0?areaWait-(total%areaWait)+areaWait:interval);
        }

        /// 2-4
        score = 1000;
        interval = 60;
        unitPostList.AddEnemyBQuickMU( areaWait-(total%areaWait)+areaWait, new Vector3(0,  100, 500), new Vector3( 0, 180, 0), score);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyBQuickMD(                           interval, new Vector3(0, -130, 500), new Vector3( 0, 180, 0), score);
        total += interval;

        /// 2-5
        score = 200;
        interval = 12;
        unitPostList.AddEnemyATurnD( areaWait-(total%areaWait), new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score,  0);
        total += areaWait-(total%areaWait);
        unitPostList.AddEnemyATurnD(                  interval, new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyATurnD(                  interval, new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score, 20);
        total += interval;
        unitPostList.AddEnemyATurnD(                  interval, new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score, 30);
        total += interval;
        unitPostList.AddEnemyATurnD(                  interval, new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score, 40);
        total += interval;

        unitPostList.AddEnemyATurnU( areaWait-(total%areaWait), new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score,  0);
        total += areaWait-(total%areaWait);
        unitPostList.AddEnemyATurnU(                  interval, new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyATurnU(                  interval, new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score, 20);
        total += interval;
        unitPostList.AddEnemyATurnU(                  interval, new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score, 30);
        total += interval;
        unitPostList.AddEnemyATurnU(                  interval, new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score, 40);
        total += interval;

        /// 2-7
        score = 1000;
        interval = 10;
        unitPostList.AddEnemyBVerticalD( areaWait-(total%areaWait)+areaWait, new Vector3(0,  250, -260), new Vector3( 0,   0, 0), score);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyBVerticalU(                           interval, new Vector3(0, -250,  240), new Vector3( 0, 180, 0), score);
        total += interval;

        unitPostList.AddEnemyBVerticalU( areaWait-(total%areaWait)+areaWait, new Vector3(0, -250, -200), new Vector3( 0,   0, 0), score);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyBVerticalD(                           interval, new Vector3(0,  250,  300), new Vector3( 0, 180, 0), score);
        total += interval;

        /// 2-9
        score = 200;
        interval = 12;
        unitPostList.AddEnemyATurnD( areaWait-(total%areaWait)+areaWait, new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score,  0);
        unitPostList.AddEnemyATurnU(                                  0, new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score,  0);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyATurnD(                           interval, new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score, 10);
        unitPostList.AddEnemyATurnU(                                  0, new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyATurnD(                           interval, new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score, 20);
        unitPostList.AddEnemyATurnU(                                  0, new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score, 20);
        total += interval;
        unitPostList.AddEnemyATurnD(                           interval, new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score, 30);
        unitPostList.AddEnemyATurnU(                                  0, new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score, 30);
        total += interval;
        unitPostList.AddEnemyATurnD(                           interval, new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score, 40);
        unitPostList.AddEnemyATurnU(                                  0, new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score, 40);
        total += interval;

        /// 3-1
        score = 400;
        interval = 30;
        unitPostList.AddEnemyAArcU( areaWait-(total%areaWait)+areaWait, new Vector3(0, -250, 250), new Vector3( -40, 180, 0), score);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyAArcD(                                  0, new Vector3(0,  250, 500), new Vector3(  40, 180, 0), score);
        unitPostList.AddEnemyAArcU(                           interval, new Vector3(0, -250, 250), new Vector3( -40, 180, 0), score);
        total += interval;
        unitPostList.AddEnemyAArcD(                                  0, new Vector3(0,  250, 500), new Vector3(  40, 180, 0), score);

        /// 3-2
        score = 400;
        interval = 12;
        unitPostList.AddEnemyAZigzagD( areaWait-(total%areaWait)+areaWait, new Vector3(0, -25, 500), new Vector3( 90, 180, 0), score,  0);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyAZigzagD(                           interval, new Vector3(0, -25, 500), new Vector3( 90, 180, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyAZigzagD(                           interval, new Vector3(0, -25, 500), new Vector3( 90, 180, 0), score, 20);
        total += interval;
        unitPostList.AddEnemyAZigzagD(                           interval, new Vector3(0, -25, 500), new Vector3( 90, 180, 0), score, 30);
        total += interval;
        unitPostList.AddEnemyAZigzagD(                           interval, new Vector3(0, -25, 500), new Vector3( 90, 180, 0), score, 40);
        total += interval;

        /// 3-3
        score = 2500;
        interval = 180;
        unitPostList.AddEnemyBRectLD( areaWait-(total%areaWait)+areaWait, new Vector3(0,  130, -500), new Vector3(  0, 0, 0), score);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyBRectLU(                           interval, new Vector3(0, -150, -500), new Vector3(  0, 0, 0), score);
        total += areaWait-(total%areaWait);

        /// 3-4
        score = 2500;
        interval = 0;
        unitPostList.AddEnemyBRectMD( areaWait-(total%areaWait)+areaWait, new Vector3(0,   70, 500), new Vector3(  0, 180, 0), score);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyBRectMU(                           interval, new Vector3(0, -120, 500), new Vector3(  0, 180, 0), score);
        total += interval;

        /// 3-5
        score = 700;
        interval = 60;
        unitPostList.AddEnemyCZigzagD( areaWait-(total%areaWait)+areaWait, new Vector3(0,   0, 500), new Vector3( 0, 180, 0), score);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyCZigzagD(                           interval, new Vector3(0, -50, 500), new Vector3( 0, 180, 0), score);
        total += interval;
        unitPostList.AddEnemyCZigzagU(                                  0, new Vector3(0,  50, 500), new Vector3( 0, 180, 0), score);
        total += interval;

        /// 3-6
        score = 200;
        interval = 12;
        unitPostList.AddEnemyATurnD( areaWait-(total%areaWait), new Vector3(-380,  160, -500), new Vector3(  2, 0, 0), score,  0);
        unitPostList.AddEnemyATurnU(                         0, new Vector3(-380, -200, -500), new Vector3( -2, 0, 0), score,  0);
        total += areaWait-(total%areaWait);
        unitPostList.AddEnemyATurnD(                  interval, new Vector3(-380,  200, -500), new Vector3(  2, 0, 0), score, 10);
        unitPostList.AddEnemyATurnU(                         0, new Vector3(-380, -240, -500), new Vector3( -2, 0, 0), score, 10);
        total += interval;
        unitPostList.AddEnemyATurnD(                  interval, new Vector3(-380,  240, -500), new Vector3(  2, 0, 0), score, 20);
        unitPostList.AddEnemyATurnU(                         0, new Vector3(-380, -280, -500), new Vector3( -2, 0, 0), score, 20);
        total += interval;
        score = 400;
        unitPostList.AddEnemyATurnD(                  interval, new Vector3(-380,  280, -500), new Vector3(  2, 0, 0), score, 30, true);
        unitPostList.AddEnemyATurnU(                         0, new Vector3(-380, -320, -500), new Vector3( -2, 0, 0), score, 30, true);
        total += interval;

        /// 3-7
        score = 2500;
        interval = 0;
        unitPostList.AddEnemyBSerialD( areaWait-(total%areaWait)+areaWait, new Vector3(0,  283, 415), new Vector3(  60, 180, 0), score);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyBSerialU( areaWait-(total%areaWait), new Vector3(0, -313, 415), new Vector3( -60, 180, 0), score);
        total += areaWait-(total%areaWait);

        /// 3-9
        randY = rand.Next( -90, 70);
        randZ = rand.Next(-120,240);

        interval = 0;
        score = 700;
        unitPostList.AddEnemyCSplit( areaWait-(total%areaWait)+areaWait, new Vector3(-400,    randY, randZ+80), new Vector3(0, 90, 0), score,    0);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400, randY-80,    randZ), new Vector3(0, 90, 0), score,   90);
        total += interval;
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400,    randY, randZ-80), new Vector3(0, 90, 0), score,  180);
        total += interval;
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400, randY+80,    randZ), new Vector3(0, 90, 0), score,  -90);
        total += interval;
        unitPostList.AddEnemyCSplit(                        interval+15, new Vector3(-400, randY-56, randZ+56), new Vector3(0, 90, 0), score,   45);
        total += interval+15;
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400, randY-56, randZ-56), new Vector3(0, 90, 0), score,  135);
        total += interval; 
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400, randY+56, randZ-56), new Vector3(0, 90, 0), score, -135);
        total += interval;
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400, randY+56, randZ+56), new Vector3(0, 90, 0), score,  -45);
        total += interval;

        randY = rand.Next( -90, 70);
        randZ = rand.Next(-120,240);

        unitPostList.AddEnemyCSplit( areaWait-(total%areaWait)+areaWait, new Vector3(-400,    randY, randZ+80), new Vector3(0, 90, 0), score,    0);
        total += areaWait-(total%areaWait)+areaWait;
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400, randY-80,    randZ), new Vector3(0, 90, 0), score,   90);
        total += interval;
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400,    randY, randZ-80), new Vector3(0, 90, 0), score,  180);
        total += interval;
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400, randY+80,    randZ), new Vector3(0, 90, 0), score,  -90);
        total += interval;
        unitPostList.AddEnemyCSplit(                        interval+15, new Vector3(-400, randY-56, randZ+56), new Vector3(0, 90, 0), score,   45);
        total += interval+15;
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400, randY-56, randZ-56), new Vector3(0, 90, 0), score,  135);
        total += interval; 
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400, randY+56, randZ-56), new Vector3(0, 90, 0), score, -135);
        total += interval;
        unitPostList.AddEnemyCSplit(                           interval, new Vector3(-400, randY+56, randZ+56), new Vector3(0, 90, 0), score,  -45);
        total += interval;

        /// 4-1
        score = 40000;
        interval = 0;
        unitPostList.AddEnemyBoss( areaWait-(total%areaWait)+areaWait, new Vector3(0, -24, 650), new Vector3(0, 180, 0), score);
        total += areaWait-(total%areaWait)+areaWait;
        score = 1200;
        unitPostList.AddEnemyBit(interval, new Vector3(0, 0, 0), new Vector3(0, 0, 0), score);
    }

    /// モデルデータ
    private void setupModel()
    {
        Model model;

        model = new Model("/sht_player/sht_player.mdx");
        model.LoadTexture("sht_player.png", "/sht_player/sht_player.png");
        ModelManager.Add("Player", model);

        model = new Model("/sht_bullet_player/sht_bullet_player.mdx");
        model.LoadTexture("sht_bullet_player.png", "/sht_bullet_player/sht_bullet_player.png");
        ModelManager.Add("NormalBullet", model);

        model = new Model("/sht_ref_bullet_player/sht_ref_bullet_player.mdx");
        model.LoadTexture("sht_ref_bullet_player.png", "/sht_ref_bullet_player/sht_ref_bullet_player.png");
        ModelManager.Add("RefNormalBullet", model);

        model = new Model("/sht_boost_player/sht_boost_player.mdx");
        model.LoadTexture("sht_boost_player.png", "/sht_boost_player/sht_boost_player.png");
        ModelManager.Add("BoostPlayer", model);

        model = new Model("/sht_bullet_enemy_a/sht_bullet_enemy_a.mdx");
        model.LoadTexture("sht_bullet_enemy_a.png", "/sht_bullet_enemy_a/sht_bullet_enemy_a.png");
        ModelManager.Add("EnemyBullet", model);

        model = new Model("/sht_ref_bullet_enemy_a/sht_ref_bullet_enemy_a.mdx");
        model.LoadTexture("sht_ref_bullet_enemy_a.png", "/sht_ref_bullet_enemy_a/sht_ref_bullet_enemy_a.png");
        ModelManager.Add("RefEnemyBullet", model);

        model = new Model("/sht_bullet_enemy_b/sht_bullet_enemy_b.mdx");
        model.LoadTexture("sht_bullet_enemy_b.png", "/sht_bullet_enemy_b/sht_bullet_enemy_b.png");
        ModelManager.Add("MissileBullet", model);

        model = new Model("/sht_charge/sht_charge_attack.mdx");
        model.LoadTexture("sht_charge_attack.png", "/sht_charge/sht_charge_attack.png");
        ModelManager.Add("ChargeAttack", model);

        model = new Model("/sht_charge/sht_charge_continue.mdx");
        model.LoadTexture("sht_charge_continue.png", "/sht_charge/sht_charge_continue.png");
        ModelManager.Add("ChargeContinue", model);

        model = new Model("/sht_charge/sht_charge_start.mdx");
        model.LoadTexture("sht_charge_start.png", "/sht_charge/sht_charge_start.png");
        ModelManager.Add("ChargeStart", model);

        model = new Model("/sht_charge/sht_charge_complete.mdx");
        model.LoadTexture("sht_charge_complete.png", "/sht_charge/sht_charge_complete.png");
        model.LoadTexture("sht_charge_continue.png", "/sht_charge/sht_charge_continue.png");
        ModelManager.Add("ChargeComplete", model);

        model = new Model("/sht_charge/sht_charge_end.mdx");
        model.LoadTexture("sht_charge_continue.png", "/sht_charge/sht_charge_continue.png");
        ModelManager.Add("ChargeEnd", model);

        model = new Model("/sht_enter/sht_enter.mdx");
        model.LoadTexture("sht_enter.png", "/sht_enter/sht_enter.png");
        ModelManager.Add("Enter", model);

        model = new Model("/sht_splash/sht_splash.mdx");
        model.LoadTexture("sht_splash.png", "/sht_splash/sht_splash.png");
        ModelManager.Add("Reflection", model);

        model = new Model("/sht_explode_player/sht_explode_player.mdx");
        model.LoadTexture("sht_explode_player.png", "/sht_explode_player/sht_explode_player.png");
        ModelManager.Add("PlayerExplode", model);

        model = new Model("/sht_explode_enemy/sht_explode_enemy.mdx");
        model.LoadTexture("sht_explode_enemy.png", "/sht_explode_enemy/sht_explode_enemy.png");
        ModelManager.Add("EnemyExplode", model);

        model = new Model("/sht_bullet_boss/sht_bullet_boss.mdx");
        model.LoadTexture("sht_bullet_boss.png", "/sht_bullet_boss/sht_bullet_boss.png");
        ModelManager.Add("BossBullet", model);

        model = new Model("/sht_laser_boss/sht_laser_boss.mdx");
        model.LoadTexture("sht_laser_boss.png", "/sht_laser_boss/sht_laser_boss.png");
        ModelManager.Add("BossLaser", model);

        model = new Model("/sht_enemy_a/sht_enemy_a.mdx");
        model.LoadTexture("sht_enemy_a.png", "/sht_enemy_a/sht_enemy_a.png");
        ModelManager.Add("EnemyA", model);

        model = new Model("/sht_enemy_a/sht_enemy_a.mdx");
        model.LoadTexture("sht_enemy_a.png", "/sht_enemy_a/sht_enemy_a_r.png");
        ModelManager.Add("EnemyASp", model);

        model = new Model("/sht_enemy_b/sht_enemy_b.mdx");
        model.LoadTexture("sht_enemy_b.png", "/sht_enemy_b/sht_enemy_b.png");
        model.LoadTexture("booster.png", "/sht_enemy_b/booster.png");
        ModelManager.Add("EnemyB", model);

        model = new Model("/sht_enemy_b/sht_enemy_b.mdx");
        model.LoadTexture("sht_enemy_b.png", "/sht_enemy_b/sht_enemy_b_r.png");
        model.LoadTexture("booster.png", "/sht_enemy_b/booster.png");
        ModelManager.Add("EnemyBSp", model);

        model = new Model("/sht_enemy_c/sht_enemy_c.mdx");
        model.LoadTexture("sht_enemy_c.png", "/sht_enemy_c/sht_enemy_c.png");
        ModelManager.Add("EnemyC", model);

        model = new Model("/sht_enemy_c/sht_enemy_c.mdx");
        model.LoadTexture("sht_enemy_c.png", "/sht_enemy_c/sht_enemy_c_r.png");
        ModelManager.Add("EnemyCSp", model);

        model = new Model("/sht_boss/sht_boss.mdx");
        model.LoadTexture("sht_boss.png", "/sht_boss/sht_boss.png");
        model.LoadTexture("sht_boss_b.png", "/sht_boss/sht_boss_b.png");
        model.LoadTexture("sht_boss_c.png", "/sht_boss/sht_boss_c.png");
        ModelManager.Add("EnemyBoss", model);

        model = new Model(null);
        ModelManager.Add("NoneBullet", model);

        model = new Model("/sht_bit/sht_bit.mdx");
        model.LoadTexture("sht_bit.png", "/sht_bit/sht_bit.png");
        ModelManager.Add("EnemyBit", model);
    }

    /// 使用シェーダの事前登録
    private void setupShader()
    {
        ShaderContainer shaderContainer = ShootingData.ShaderContainer;

        shaderContainer.Load("W1C0L0NNA0T1I0MN0NN0NA0", "/Application/shaders/cbasic/W1C0L0NNA0T1.vp.cgx", "/Application/shaders/cbasic/C0L0NNT1I0MN0NN0NA0.fp.cgx");
        shaderContainer.Load("W1C0L0NNA1T1I0MN0NN0NA0", "/Application/shaders/cbasic/W1C0L0NNA1T1.vp.cgx", "/Application/shaders/cbasic/C0L0NNT1I0MN0NN0NA0.fp.cgx");
        shaderContainer.Load("W1C0L0NNA1T2I0MI1MN0NA0", "/Application/shaders/cbasic/W1C0L0NNA1T2.vp.cgx", "/Application/shaders/cbasic/C0L0NNT2I0MI1MN0NA0.fp.cgx");
        shaderContainer.Load("W4C0L0NNA1T1I0MN0NN0NA0", "/Application/shaders/cbasic/W4C0L0NNA1T1.vp.cgx", "/Application/shaders/cbasic/C0L0NNT1I0MN0NN0NA0.fp.cgx");
        shaderContainer.Load("W3C0L0NNA0T1I0MN0NN0NA0", "/Application/shaders/cbasic/W3C0L0NNA0T1.vp.cgx", "/Application/shaders/cbasic/C0L0NNT1I0MN0NN0NA0.fp.cgx");
        shaderContainer.Load("W2C0L0NNA0T1I0MN0NN0NA0", "/Application/shaders/cbasic/W2C0L0NNA0T1.vp.cgx", "/Application/shaders/cbasic/C0L0NNT1I0MN0NN0NA0.fp.cgx");
        shaderContainer.Load("W4C0L0NNA0T1I0MN0NN0NA1", "/Application/shaders/cbasic/W4C0L0NNA0T1.vp.cgx", "/Application/shaders/cbasic/C0L0NNT1I0MN0NN0NA1.fp.cgx");
        shaderContainer.Load("W2C0L0NNA1T2I0MI1MN0NA0", "/Application/shaders/cbasic/W2C0L0NNA1T2.vp.cgx", "/Application/shaders/cbasic/C0L0NNT2I0MI1MN0NA0.fp.cgx");
    }

    /// 解放
    public void Dispose()
    {
        unitPostList.Dispose();
    }

    /// ユニット配置リスト取得
    public UnitPostSchedule GetUnitPostSchedule()
    {
        return unitPostList;
    }

}

} // ShootingDemo
