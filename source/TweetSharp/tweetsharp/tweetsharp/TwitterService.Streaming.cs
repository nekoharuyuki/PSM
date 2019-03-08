using System;
using System.Linq;
using Hammock;
using Hammock.Streaming;
using Hammock.Web;

namespace TweetSharp
{
    partial class TwitterService
    {
        private readonly RestClient _userStreamsClient;
        private readonly RestClient _publicStreamsClient;

        /// <summary>
        /// Cancels pending streaming actions from this service.
        /// </summary>
        public virtual void CancelStreaming()
        {
            if(_userStreamsClient != null)
            {
                _userStreamsClient.CancelStreaming();
            }
            if (_publicStreamsClient != null)
            {
                _publicStreamsClient.CancelStreaming();
            }
        }

        /// <summary>
        /// Accesses an asynchronous Twitter filter stream indefinitely, until terminated.
        /// </summary>
        /// <seealso href="http://dev.twitter.com/pages/streaming_api_methods#statuses-filter" />
        /// <param name="action"></param>
        /// <returns></returns>
#if !WINDOWS_PHONE && !PSM
        public virtual IAsyncResult StreamFilter(Action<TwitterStreamArtifact, TwitterResponse> action, string[] segment)
#else
        public virtual void StreamFilter(Action<TwitterStreamArtifact, TwitterResponse> action, string[] segment)
#endif
        {
            var options = new StreamOptions { ResultsPerCallback = 1 };
#if !WINDOWS_PHONE && !PSM
            return 
#endif
            WithHammockPublicStreaming(options, action, "statuses/filter", segment);
        }
		
		public virtual void StreamSample(Action<TwitterStreamArtifact, TwitterResponse> action, string[] segment)
        {
			var options = new StreamOptions { ResultsPerCallback = 1};
            WithHammockPublicStreaming(options, action, "statuses/sample", segment);
        }
		
        /// <summary>
        /// Accesses an asynchronous Twitter user stream indefinitely, until terminated.
        /// </summary>
        /// <seealso href="http://dev.twitter.com/pages/user_streams" />
        /// <param name="action"></param>
        /// <returns></returns>
#if !WINDOWS_PHONE && !PSM
        public virtual IAsyncResult StreamUser(Action<TwitterStreamArtifact, TwitterResponse> action, string[] segment)
#else
        public virtual void StreamUser(Action<TwitterStreamArtifact, TwitterResponse> action, string[] segment)
#endif
        {
            var options = new StreamOptions { ResultsPerCallback = 1};
#if !WINDOWS_PHONE && !PSM
            return 
#endif
            WithHammockUserStreaming(options, action, "user", segment);

        }

#if !WINDOWS_PHONE && !PSM
        private IAsyncResult WithHammockUserStreaming<T>(StreamOptions options, Action<T, TwitterResponse> action, string path) where T : class
#else
        private void WithHammockUserStreaming<T>(StreamOptions options, Action<T, TwitterResponse> action, string path, params object[] segments) where T : class
#endif
        {
            var request = PrepareHammockQuery(path);
			request.Path = ResolveUrlSegments(path, segments.ToList());
#if !WINDOWS_PHONE  && !PSM
            return 
#endif 
            WithHammockStreamingImpl(_userStreamsClient, request, options, action);
        }

#if !WINDOWS_PHONE && !PSM
        private IAsyncResult WithHammockPublicStreaming<T>(StreamOptions options, Action<T, TwitterResponse> action, string path) where T : class
#else
        private void WithHammockPublicStreaming<T>(StreamOptions options, Action<T, TwitterResponse> action, string path, params object[] segments) where T : class
#endif
        {
            var request = PrepareHammockQuery(path);
			request.Path = ResolveUrlSegments(path, segments.ToList());
#if !WINDOWS_PHONE && !PSM
            return 
#endif 
            WithHammockStreamingImpl(_publicStreamsClient, request, options, action);
        }

#if !WINDOWS_PHONE && !PSM
        private IAsyncResult WithHammockStreamingImpl<T>(RestClient client, RestRequest request, StreamOptions options, Action<T, TwitterResponse> action)
#else
        private static void WithHammockStreamingImpl<T>(RestClient client, RestRequest request, StreamOptions options, Action<T, TwitterResponse> action)
#endif
        {
            request.StreamOptions = options;
			if(request.Path.Contains("statuses/filter")){
            	request.Method = WebMethod.Post;
			}
			else{
            	request.Method = WebMethod.Get;
			}
			request.QueryHandling = QueryHandling.AppendToParameters;
#if SILVERLIGHT
            request.AddHeader("X-User-Agent", client.UserAgent); 
#endif
            
#if !WINDOWS_PHONE && !PSM
            return
#endif
            client.BeginRequest(request, new RestCallback<T>((req, resp, state) =>
            {
                Exception exception;
                var entity = TryAsyncResponse(() => 
                        {
#if !SILVERLIGHT && !PSM
                            SetResponse(resp);
#endif
                            return resp.ContentEntity;
                        },
                        out exception);
                action(entity, new TwitterResponse(resp, exception));
            }));
        }
    }
}
