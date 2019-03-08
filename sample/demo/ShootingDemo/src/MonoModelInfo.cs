/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace ShootingDemo
{

/**
 * MonoModelInfoクラス
 */
public class MonoModelInfo
{
    public float CollisionRadius {get; protected set;}
    public float CollisionRadius2 {get; protected set;}
    public CollisionLevel CollisionLevel {get; protected set;}
    public int Hitpoint {get; protected set;}
    public int Attack {get; protected set;}
    public int Defense {get; protected set;}

    /// コンストラクタ
    public MonoModelInfo(float collisionRadius,
                     CollisionLevel collisionLevel = CollisionLevel.None,
                     int hitpoint = 1,
                     int attack = 1,
                     int defense = 0,
                     float collisionRadius2 = 0)
    {
        CollisionRadius = collisionRadius;
        CollisionRadius2 = collisionRadius2;
        CollisionLevel = collisionLevel;
        Hitpoint = hitpoint;
        Attack = attack;
        Defense = defense;
    }
}

} // ShootingDemo
