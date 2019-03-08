/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.Framework;

namespace Sample
{

public class AppMain
{
    public static void Main(string[] args)
    {
		using( GameFrameworkSample game = new GameFrameworkSample())
		{
			game.Run(args);
		}
    }
}

}
