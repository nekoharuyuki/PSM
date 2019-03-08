/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace FlightDemo
{

public enum RepeatMode
{
    Constant,
    Loop
}

/**
 * LayoutActionクラス
 */
public class LayoutAction : IDisposable
{
    private string currentKey = null;
    private Dictionary<string, LayoutAnimationList> animTable = new Dictionary<string, LayoutAnimationList>();
    private long animTimeMillis;

    public int RenderOffsetX {get; set;}
    public int RenderOffsetY {get; set;}
    public RepeatMode RepeatMode {get; private set;}

    ///
    public LayoutAction(int renderOffsetX = 0, int renderOffsetY = 0)
    {
        RenderOffsetX = renderOffsetX;
        RenderOffsetY = renderOffsetY;

        RepeatMode = RepeatMode.Constant;
    }

    ///
    public void Dispose()
    {
        foreach (string key in animTable.Keys) {
            animTable[key].Dispose();
        }

        animTable.Clear();
    }

    ///
    public void Add(string key, LayoutAnimationList animList)
    {
        if (Find(key) == null) {
            animTable[key] = animList;
        }
    }

    ///
    public LayoutAnimationList Find(string key)
    {
        if (key != null && animTable.ContainsKey(key)) {
            return animTable[key];
        }

        return null;
    }

    ///
    public LayoutAnimationList Current()
    {
        return Find(currentKey);
    }

    ///
    public string CurrentKey
    {
        get {return currentKey;}
    }

    ///
    public void ChangeCurrent(string key, RepeatMode repeatMode = RepeatMode.Constant)
    {
        if (currentKey != key) {
            SetCurrent(key, repeatMode);
        }
    }

    ///
    public void SetCurrent(string key, RepeatMode repeatMode = RepeatMode.Constant)
    {
        currentKey = key;
        animTimeMillis = 0;
        RepeatMode = repeatMode;
    }

    ///
    public void Update(long addTimeMillis)
    {
        var anim = Current();

        if (anim != null) {
            animTimeMillis += addTimeMillis;

            long endTimeMillis = anim.EndTimeMillis();

            if (animTimeMillis > endTimeMillis) {
                switch (RepeatMode) {
                case RepeatMode.Constant:
                    animTimeMillis = endTimeMillis;
                    break;
                case RepeatMode.Loop:
                    if (endTimeMillis > 0) {
                        animTimeMillis %= endTimeMillis;
                    }
                    break;
                }
            }
        }
    }

    ///
    public void Render()
    {
        var anim = Current();

        if (anim != null) {
            anim.Render(animTimeMillis, RenderOffsetX, RenderOffsetY);
        }
    }

    ///
    public bool IsPlayEnd()
    {
        var anim = Current();
        if (anim != null && animTimeMillis >= anim.EndTimeMillis()) {
            return true;
        }

        return false;
    }
}

} // ShootingDemo
