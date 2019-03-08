/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System ;
using System.IO ;
using System.Threading ;
using System.Diagnostics ;
using System.Collections.Generic ;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Environment ;
using Sce.PlayStation.Core.Graphics ;
using Sce.PlayStation.Core.Imaging ;

namespace Sample
{

//----------------------------------------------------------------
//  Resource loader
//----------------------------------------------------------------

public class ResourceLoader
{
	public ResourceLoader( string path = "" )
	{
		syncObject = new object() ;
		pathPrefix = path ;
		loadAction = (index,filename,category,resource) => {} ;
		maxBufferUsage = 1024 * 1024 * 1 ;
		requestQueue = new Queue<Request>() ;
		resultQueue = new Queue<Result>() ;
		resourceTypes = new Dictionary<string,ResourceType>() ;

		AddDefaultResourceTypes() ;
	}

	//  Loading

	public void Load( params string[] filenames )
	{
		for ( int i = 0 ; i < filenames.Length ; i ++ ){
			var resourceType = GetResourceType( Path.GetExtension( filenames[ i ] ) ) ;
			var request = new Request( filenames[ i ], i, pathPrefix, resourceType ) ;
			object resource = null ;
			try {
				if ( resourceType.QuickLoad != null ) {
					resource = resourceType.QuickLoad( request ) ;
				} else {
					resource = resourceType.PostLoad( resourceType.PreLoad( request ) ) ;
				}
			}
			catch ( Exception e ) {}
			loadAction( request.Index, request.Filename, request.ResourceType.Category, resource ) ;
		}
	}
	public void Load( string[] filenames, ThreadWorker[] workers )
	{
		StartLoading( filenames, workers ) ;
		FinishLoading( true ) ;
	}
	public void StartLoading( string[] filenames, ThreadWorker[] workers )
	{
		for ( int i = 0 ; i < filenames.Length ; i ++ ) {
			var resourceType = GetResourceType( Path.GetExtension( filenames[ i ] ) ) ;
			EnqueueRequest( new Request( filenames[ i ], i, pathPrefix, resourceType ) ) ;
		}
		int workerCount = workers.Length ;
		for ( int N = 0 ; N < workerCount ; N ++ ) {
			workers[ N ].Start( () => {
				Request request ;
				while ( DequeueRequest( out request ) ) {
					Result result = new Result( request, new byte[ 0 ] ) ;
					try {
						result = request.ResourceType.PreLoad( request ) ;
					}
					catch ( Exception e ) {}
					EnqueueResult( result ) ;
				}
			} ) ;
		}
	}
	public bool FinishLoading( bool now = false )
	{
		Result result ;
		while ( DequeueResult( out result, now ) ) {
			var request = result.Request ;
			object resource = null ;
			try {
				resource = request.ResourceType.PostLoad( result ) ;
			}
			catch ( Exception e ) {}
			loadAction( request.Index, request.Filename, request.ResourceType.Category, resource ) ;
		}
		lock ( syncObject ) {
			return ( requestCount == 0 ) ;
		}
	}

	//  Configuration

	public ResourceType GetResourceType( string extension )
	{
		return resourceTypes[ extension.ToLower() ] ;
	}
	public void AddResourceType( string extension, ResourceType resourceType )
	{
		resourceTypes[ extension.ToLower() ] = resourceType ;
	}
	public void AddResourceType( string[] extensions, ResourceType resourceType )
	{
		for ( int i = 0 ; i < extensions.Length ; i ++ ) {
			AddResourceType( extensions[ i ], resourceType ) ;
		}
	}
	public void AddDefaultResourceTypes()
	{
		string[] textureExtensions = { ".png", ".jpg", ".gif", ".bmp" } ;
		var textureResourceType = new ResourceType( "texture",
			(request) => {
				byte[] fileimage ;
				using ( var reader = new BinaryReader( File.OpenRead( request.FullPath ) ) ) {
					fileimage = reader.ReadBytes( (int)reader.BaseStream.Length ) ;
				}
				var image = new Image( fileimage ) ;
				image.Decode() ;
				var buffer = image.ToBuffer() ;
				var size = image.Size ;
				image.Dispose() ;
				return new Result( request, buffer, size.Width, size.Height ) ;
			},
			(result) => {
				int width = result.Param[ 0 ], height = result.Param[ 1 ] ;
				var texture = new Texture2D( width, height, false, PixelFormat.Rgba ) ;
				texture.SetPixels( 0, result.Buffer ) ;
				return texture ;
			},
			(request) => {
				return new Texture2D( request.FullPath, false ) ;
			}
		) ;
		AddResourceType( textureExtensions, textureResourceType ) ;
	}

	//  Properties

	public string PathPrefix {
		get { return pathPrefix ; }
		set { pathPrefix = value ; }
	}
	public Action<int,string,string,object> LoadAction {
		get { return loadAction ; }
		set { loadAction = value ; }
	}
	public int MaxBufferUsage {
		get { return maxBufferUsage ; }
		set { maxBufferUsage = value ; }
	}

	//  Subroutines

	void EnqueueRequest( Request request )
	{
		lock ( syncObject ) {
			requestQueue.Enqueue( request ) ;
			requestCount ++ ;
			Monitor.PulseAll( syncObject ) ;
		}
	}
	bool DequeueRequest( out Request request, bool now = false )
	{
		request = new Request() ;
		lock ( syncObject ) {
			if ( now && requestQueue.Count == 0 && requestCount > 0 ) Monitor.Wait( syncObject ) ;
			if ( requestQueue.Count == 0 ) return false ;
			request = requestQueue.Dequeue() ;
			Monitor.PulseAll( syncObject ) ;
			return true ;
		}
	}
	void EnqueueResult( Result result )
	{
		lock ( syncObject ) {
			while ( usedBufferSize > 0 && usedBufferSize + result.Buffer.Length > maxBufferUsage ) {
				Monitor.Wait( syncObject ) ;
			}
			usedBufferSize += result.Buffer.Length ;
			resultQueue.Enqueue( result ) ;
			Monitor.PulseAll( syncObject ) ;
		}
	}
	bool DequeueResult( out Result result, bool now = false )
	{
		result = new Result() ;
		lock ( syncObject ) {
			while ( now && resultQueue.Count == 0 && requestCount > 0 ) Monitor.Wait( syncObject ) ;
			if ( resultQueue.Count == 0 ) return false ;
			result = resultQueue.Dequeue() ;
			usedBufferSize -= result.Buffer.Length ;
			-- requestCount ;
			Monitor.PulseAll( syncObject ) ;
			return true ;
		}
	}

	//  Types

	public struct Request {
		public Request( string filename, int index, string pathPrefix, ResourceType resourceType )
		{
			Filename = filename ;
			Index = index ;
			FullPath = pathPrefix + filename ;
			ResourceType = resourceType ;
		}
		public string Filename ;
		public int Index ;
		public string FullPath ;
		public ResourceType ResourceType ;
	} ;
	public struct Result {
		public Result( Request request, byte[] buffer, params int[] param )
		{
			Request = request ;
			Buffer = buffer ;
			Param = param ;
		}
		public Request Request ;
		public byte[] Buffer ;
		public int[] Param ;
	} ;
	public class ResourceType {
		public ResourceType( string category, Func<Request,Result> preLoad, Func<Result,object> postLoad, Func<Request,object> quickLoad = null )
		{
			Category = category ;
			PreLoad = preLoad ;
			PostLoad = postLoad ;
			QuickLoad = quickLoad ;
		}
		public string Category ;
		public Func<Request,Result> PreLoad ;
		public Func<Result,object> PostLoad ;
		public Func<Request,object> QuickLoad ;
	} ;

	//  Params

	object syncObject ;
	string pathPrefix ;
	Action<int,string,string,object> loadAction ;

	int requestCount ;
	int maxBufferUsage ;
	int usedBufferSize ;
	Queue<Request> requestQueue ;
	Queue<Result> resultQueue ;

	Dictionary<string,ResourceType> resourceTypes ;
}


} // end ns Sample