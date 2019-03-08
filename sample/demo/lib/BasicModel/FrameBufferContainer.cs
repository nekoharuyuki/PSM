// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;

namespace DemoModel{

public class FrameBufferContainer
{
    Dictionary< string, FrameBuffer > frameBufferTable = new Dictionary< string, FrameBuffer>();

    public FrameBufferContainer()
    {
    }

	public void Dispose()
    {
        foreach( var entry in frameBufferTable ){
            entry.Value.Dispose();
        }
        frameBufferTable.Clear();
    }
    
    public FrameBuffer Regist( string key, FrameBuffer frameBuffer )
    {
        if( Find( key ) != null ){
            return null;
        }
        frameBufferTable[ key ] = frameBuffer;
        return frameBuffer;
    }

    public PixelBuffer GetPixelBuffer( string key )
    {
        FrameBuffer fb = this.Find( key );
        if( fb != null ){
            return fb.GetColorTarget().Buffer;
        }
        return null;
    }

    public FrameBuffer Find( string key )
    {
        if( frameBufferTable.ContainsKey( key ) ){
            return frameBufferTable[ key ];
        }
        return null;
    }
}

} // end ns DemoModel
//===
// EOF
//===
