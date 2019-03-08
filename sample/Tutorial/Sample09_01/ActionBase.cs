using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Framework;
using Tutorial.Utility;


namespace Sample
{
	public class ActionBase
	{
		GameActor gameActor=null;
		bool done = false;
		public bool Done { get {return done;}}
		Int32 cnt=0;
		
		public ActionBase ()
		{
		}
		
		public ActionBase(GameActor gameActor)
		{
			this.gameActor = gameActor;	
		}
		
		
		virtual public void Update()
		{
			if(done==false)
			{
				this.gameActor.spriteB.Rotation += FMath.PI/60.0f;
				
				if(++cnt >= 60)
				{
					done=true;	
				}
			}
		}
	}
}

