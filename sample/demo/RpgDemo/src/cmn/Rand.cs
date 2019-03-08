/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;

namespace AppRpg { namespace Common {

///***************************************************************************
/// ランダム処理（軽量版）
///***************************************************************************
static public class Rand
{
    static private    uint    randSeed = 0;


/// public メソッド
///---------------------------------------------------------------------------
    static public void SetSeed( uint seed )
    {
        randSeed = seed;
    }

    static public ushort Get()
    {
        randSeed *= 1597073941;
        randSeed += 2067483159;

        return( (ushort) (randSeed >> 16) );
    }

}

}} // namespace
