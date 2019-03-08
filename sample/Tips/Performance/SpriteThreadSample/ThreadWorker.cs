/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System ;
using System.Threading ;
using System.Diagnostics ;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;
using Sce.PlayStation.Core.Environment ;
using Sce.PlayStation.Core.Input ;

namespace Sample
{

//----------------------------------------------------------------
//  Thread Worker
//----------------------------------------------------------------

public class ThreadWorker
{
	public ThreadWorker( ThreadPriority priority = ThreadPriority.Normal )
	{
		syncObject = new object() ;
		thread = new Thread( ThreadProc ) ;
		thread.Priority = priority ;
		thread.Start() ;
	}
	~ThreadWorker()
	{
		StopThread() ;
		thread.Join() ;
	}

	//  Functions

	public void Start( Action action, bool now = false )
	{
		if ( now ) {
			if ( action != null ) action() ;
			return ;
		}
		lock ( syncObject ) {
			while ( taskAction != null ) Monitor.Wait( syncObject ) ;
			if ( action == null ) return ;
			taskAction = action ;
			Monitor.PulseAll( syncObject ) ;
		}
	}
	public void Wait()
	{
		Start( null ) ;
	}
	public static void WaitAll( ThreadWorker[] workers )
	{
		for ( int i = 0 ; i < workers.Length ; i ++ ) {
			workers[ i ].Wait() ;
		}
	}

	//  Properties

	public bool IsRunning {
		get { lock ( syncObject ) { return ( taskAction != null ) ; } }
	}

	//  Subroutines

	void ThreadProc()
	{
		for ( ; ; ) {
			lock ( syncObject ) {
				while ( taskAction == null ) {
					if ( stopRequest ) return ;
					Monitor.Wait( syncObject ) ;
				}
				taskAction() ;
				taskAction = null ;
				Monitor.PulseAll( syncObject ) ;
			}
		}
	}
	void StopThread()
	{
		lock ( syncObject ) {
			stopRequest = true ;
			Monitor.PulseAll( syncObject ) ;
		}
	}

	//  Params

	object syncObject ;
	Thread thread ;
	volatile bool stopRequest ;
	volatile Action taskAction ;
}


} // end ns Sample
