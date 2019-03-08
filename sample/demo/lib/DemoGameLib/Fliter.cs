// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

using Sce.PlayStation.Core.Graphics;

namespace DemoGame{

public abstract class Filter
{
    private FrameBuffer target;

    public FrameBuffer Target{
        get{ return target; }
    }

    /// 書き込み先の指定
    /**
     * null が指定されているときには、GraphicsContext に設定されているバッファが出力先
     */
    public virtual void SetTarget( FrameBuffer dstBuffer )
    {
        this.target = dstBuffer;
    }

    public abstract void Dispose();


} 

} // end ns SceneScript
//===
// EOF
//===
