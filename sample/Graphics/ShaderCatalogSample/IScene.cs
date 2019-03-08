/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

interface IScene
{
    string Name
    {
        get;
    }
    void Setup( GraphicsContext graphics, Model model );
    void Dispose();
    void Update( float delta );
    void Render( GraphicsContext graphics, Camera camera, LightModel light, Model model, BgModel bg );
}

}
