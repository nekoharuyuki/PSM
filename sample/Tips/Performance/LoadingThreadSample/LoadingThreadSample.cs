/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System ;
using System.Threading ;
using System.Diagnostics ;
using System.Collections.Generic ;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;
using Sce.PlayStation.Core.Environment ;
using Sce.PlayStation.Core.Input ;
using Sce.PlayStation.HighLevel.UI ;

namespace Sample
{

/**
 * LoadingThreadSample
 */
class LoadingThreadSample
{
	static GraphicsContext graphics ;
	static Stopwatch stopwatch ;
	static MainScene mainScene ;

	static ResourceLoader loader ;
	static ThreadWorker[] workers ;
	static string[] filenames ;
	static Texture2D[] textures ;

	static int loadMode ;
	static int loadStage ;
	static int loadCount ;
	static int loadError ;
	static long startTime ;
	static long finishTime ;

	static bool loop = true ;

	static void Main( string[] args )
	{
		Init() ;
		while ( loop ) {
			SystemEvents.CheckEvents() ;
			Update() ;
			Render() ;
		}
		Term() ;
	}

	static void Init()
	{
		graphics = new GraphicsContext() ;
		stopwatch = new Stopwatch() ;
		stopwatch.Start() ;

		UISystem.Initialize( graphics ) ;
		mainScene = new MainScene() ;
		UISystem.SetScene( mainScene, null ) ;
		mainScene.OnButton = OnButton ;

		InitResourceLoader( 100 ) ;
		InitThreadWorker( 3 ) ;
	}

	static void Term()
	{
		graphics.Dispose() ;
	}

	static bool Update()
	{
		List<TouchData> touchDataList = Touch.GetData( 0 ) ;
		UISystem.Update( touchDataList ) ;

		if ( loadMode == 1 ) BatchLoading() ;
		if ( loadMode == 2 ) SequentialLoading() ;
		if ( loadMode == 3 ) MultithreadedLoading() ;
		return true ;
	}

	static bool Render()
	{
		graphics.SetClearColor( 0.0f, 0.5f, 1.0f, 0.0f ) ;
		graphics.Clear() ;
		UISystem.Render() ;
		graphics.SwapBuffers() ;
		return true ;
	}

	//------------------------------------------------------------
	//  Button functions
	//------------------------------------------------------------

	static void OnButton( int num )
	{
		if ( loadMode != 0 ) return ;
		loadMode = num ;
		loadStage = 0 ;
		loadCount = 0 ;
		loadError = 0 ;

		GC.Collect() ;	//  clean garbages
	}

	//------------------------------------------------------------
	//  Thread functions
	//------------------------------------------------------------

	static void InitThreadWorker( int maxWorkerCount )
	{
		workers = new ThreadWorker[ maxWorkerCount ] ;
		for ( int i = 0 ; i < maxWorkerCount ; i ++ ) {
			workers[ i ] = new ThreadWorker() ;
		}
	}

	//------------------------------------------------------------
	//  Loader functions
	//------------------------------------------------------------

	static void InitResourceLoader( int maxTextureCount )
	{
		loader = new ResourceLoader( "/Application/resources/" ) ;
		loader.LoadAction = AddResource ;		//  call this function

		filenames = new string[ maxTextureCount ] ;
		textures = new Texture2D[ maxTextureCount ] ;

		//  initialize filenames

		var random = new Random( 12345 ) ;
		var list = new int[ maxTextureCount ] ;
		int left = maxTextureCount ;
		for ( int i = 0 ; i < maxTextureCount ; i ++ ) list[ i ] = i % 10 ;
		for ( int i = 0 ; i < maxTextureCount ; i ++ ) {
			int index = random.Next( left ) ;
			filenames[ i ] = string.Format( "test{0}.png", list[ index ] ) ;
			list[ index ] = list[ -- left ] ;
		}
	}

	static void BatchLoading()
	{
		if ( loadStage == 0 ) {
			startTime = stopwatch.ElapsedMilliseconds ;
			mainScene.Message = "LOADING" ;
			loadStage ++ ;
		} else if ( loadStage == 1 ) {
			loader.Load( filenames ) ;		//  load all files
			mainScene.Progress = 1.0f ;
			loadStage ++ ;
		} else {
			finishTime = stopwatch.ElapsedMilliseconds ;
			mainScene.Result1 = string.Format( "{0} ms", finishTime - startTime ) ;
			mainScene.Message = ( loadError == 0 ) ? "READY" : "ERROR" ;
			mainScene.Progress = 0.0f ;
			loadMode = 0 ;
		}
	}

	static void SequentialLoading()
	{
		if ( loadStage == 0 ) {
			startTime = stopwatch.ElapsedMilliseconds ;
			mainScene.Message = "LOADING" ;
			loadStage ++ ;
		} else if ( loadStage == 1 ) {
			loader.Load( filenames[ loadCount ] ) ;		//  load one file
			mainScene.Progress = (float)loadCount / filenames.Length ;
			if ( loadCount == filenames.Length ) {
				mainScene.Progress = 1.0f ;
				loadStage ++ ;
			}
		} else {
			finishTime = stopwatch.ElapsedMilliseconds ;
			mainScene.Result2 = string.Format( "{0} ms", finishTime - startTime ) ;
			mainScene.Message = ( loadError == 0 ) ? "READY" : "ERROR" ;
			mainScene.Progress = 0.0f ;
			loadMode = 0 ;
		}
	}

	static void MultithreadedLoading()
	{
		if ( loadStage == 0 ) {
			startTime = stopwatch.ElapsedMilliseconds ;
			loader.StartLoading( filenames, workers ) ;		//  start loading
			mainScene.Message = "LOADING" ;
			loadStage ++ ;
		} else if ( loadStage == 1 ) {
			if ( !loader.FinishLoading( false ) ) {			//  finish loading
				mainScene.Progress = (float)loadCount / filenames.Length ;
			} else {
				mainScene.Progress = 1.0f ;
				loadStage ++ ;
			}
		} else {
			finishTime = stopwatch.ElapsedMilliseconds ;
			mainScene.Result3 = string.Format( "{0} ms", finishTime - startTime ) ;
			mainScene.Message = ( loadError == 0 ) ? "READY" : "ERROR" ;
			mainScene.Progress = 0.0f ;
			loadMode = 0 ;
		}
	}

	static void AddResource( int index, string filename, string category, object resource )
	{
		string status = ( resource != null ) ? "ok" : "error" ;
		Console.Write( "load #{0} {1} ( {2} {3} )\n", index, filename, category, status ) ;

		if ( category == "texture" ) textures[ loadCount ] = resource as Texture2D ;
		if ( resource == null ) loadError ++ ;
		loadCount ++ ;
	}
}


} // end ns Sample

