using System;
using System.Json;
using System.Collections.Generic;
using Sce.PlayStation.Core.Services;

namespace Sce.PlayStation.HighLevel.JsonHelper
{
    /// <summary>Scoreboard types</summary>
    public enum NetworkScoreboardType : uint
    {
        Daily = 1,
        Weekly,
        Monthly,
        AllTime
    };
	
    /// <summary>Service types</summary>
    public enum NetworkServiceType : uint
    {	
		invalid,
        score_board,
        friend_score_board,
        friend_list
    };
	
	/// <summary>data used for set scoreboard requests</summary>
    public class NetworkSetRequestData
    {
        public string scoreboardToken;					// required
        public string score;							// required
        public string metadata;							// optional
    };

    /// <summary>data used for get scoreboard requests</summary>
    public class NetworkGetRequestData
    {
		public NetworkServiceType serviceType;			// required
        public string[] scoreboardTokens;				// required for NetworkServiceType.score_board & NetworkServiceType.friend_score_board
        public NetworkScoreboardType[] scoreboardTypes;	// required for NetworkServiceType.score_board
        public Int32 limit;								// optional for NetworkServiceType.score_board & NetworkServiceType.friend_score_board
        public Int32 offset;							// optional for NetworkServiceType.score_board & NetworkServiceType.friend_score_board
    };
	
    /// <summary>scoreboard data parsed from response</summary>
    public class NetworkResponseData
    {
        public struct ScoreData
        {
            public string user;
            public string score;
            public string date;
            public ScoreData(string u, string s, string d) { user = u; score = s; date = d; }
        };

        public string username = "";
        // [name(LB1-token) : [type(alltime) : {user, score, date} ] ]
        public Dictionary<string, Dictionary<string, List<ScoreData>>> scoreboard = null;
        public List<string> friends = null;
        public string error = "";
		
		/// <summary>Gets the number of scores in the list.</summary>
		/// <returns>The score data.</returns>
		/// <param name='scoreboardToken'>Scoreboard token.</param>
		/// <param name='scoreboardType'>Scoreboard type.</param>
		public int GetScoreCount(string scoreboardToken, string scoreboardType)
		{
			if (scoreboard != null &&
				scoreboard.ContainsKey(scoreboardToken) &&
				scoreboard[scoreboardToken].ContainsKey(scoreboardType))
					return scoreboard[scoreboardToken][scoreboardType].Count;
			return 0;
		}
		
		/// <summary>Gets the score data.</summary>
		/// <returns>The score data.</returns>
		/// <param name='scoreboardToken'>Scoreboard token.</param>
		/// <param name='scoreboardType'>Scoreboard type.</param>
		/// <param name='index'>Index.</param>
		public ScoreData GetScoreData(string scoreboardToken, string scoreboardType, int index)
		{
			if (scoreboard != null &&
				scoreboard.ContainsKey(scoreboardToken) &&
				scoreboard[scoreboardToken].ContainsKey(scoreboardType) &&
				scoreboard[scoreboardToken][scoreboardType].Count > index)
					return scoreboard[scoreboardToken][scoreboardType][index];
			
			return new NetworkResponseData.ScoreData();
		}		
    }	
	
	/// <summary>Helper class to encode and decode Json</summary>
	public class JsonHelper
	{
		const string NETWORK_KEY_HEADER 	= "h";
        const string NETWORK_KEY_TOKEN 		= "a";
        const string NETWORK_KEY_SB_TOKENS 	= "b";
        const string NETWORK_KEY_SCORE 		= "s";
        const string NETWORK_KEY_METADATA 	= "m";
        const string NETWORK_KEY_PAYLOAD 	= "p";
        const string NETWORK_KEY_BODY 		= "b";		
        const string NETWORK_KEY_USER 		= "u";
        const string NETWORK_KEY_TICKET 	= "w";
        const string NETWORK_KEY_RANDOM		= "r";
        const string NETWORK_KEY_TIME		= "t";
        const string NETWORK_KEY_SB_TYPES   = "t";
        const string NETWORK_KEY_LIMIT      = "i";
        const string NETWORK_KEY_OFFSET     = "o";		
		const string NETWORK_KEY_ERROR		= "e";
		const string NETWORK_KEY_ERROR_TYPE	= "t";
		const string NETWORK_KEY_RANK		= "r";
		const string NETWORK_KEY_TOTAL		= "t";
		const string NETWORK_KEY_FRIENDS	= "f";
		
		static string applicationToken;
		
		public JsonHelper()
		{
		}
		
		/// <summary>
		/// Initialize the JsonHelper library
		/// </summary>
		/// <param name='applicationToken'>
		/// Application token.
		/// </param>
		public static void Initialize(string applicationToken)
		{
            JsonHelper.applicationToken = applicationToken;
			Network.Initialize(applicationToken);
		}
		
		/// <summary>
		/// Creates a "set" request.  This is used to set a score on a scoreboard.
		/// </summary>
		/// <returns>
		/// The request.
		/// </returns>
		/// <param name='data'>
		/// Data.
		/// </param>
		public static NetworkRequest CreateRequest(NetworkSetRequestData data)
		{
			string json = JsonHelper.ConstructJson(data);
			return Network.CreateRequest(NetworkRequestType.Set, Enum.GetName(typeof(NetworkServiceType), NetworkServiceType.score_board), json);
		}
		
		/// <summary>
		/// Creates a "get" request.  This is used to retrieve scoreboards, the friend scoreboard, and friend list.
		/// </summary>
		/// <returns>
		/// The request.
		/// </returns>
		/// <param name='data'>
		/// Data.
		/// </param>
		public static NetworkRequest CreateRequest(NetworkGetRequestData data)
		{
			string json = JsonHelper.ConstructJson(data);
			return Network.CreateRequest(NetworkRequestType.Get, Enum.GetName(typeof(NetworkServiceType), data.serviceType), json);			
		}
		
		static void ConstructHeader(ref JsonObject o)
		{
			JsonObject h = new JsonObject();
			
			//h.Add(NETWORK_KEY_RANDOM, 12345);
			h.Add(NETWORK_KEY_TIME, DateTime.UtcNow);
			o.Add(NETWORK_KEY_HEADER, h);
		}
		
		static void ConstructAppData(ref JsonObject p)
		{
            if (applicationToken == null || applicationToken == "")
            {
                throw new ArgumentException("NetworkServices server must be connected.");
            }			
				
			p.Add(NETWORK_KEY_TOKEN, applicationToken);
		}
		
		/// <summary>
		/// Constructs a Json string for a "get" data requrest.
		/// </summary>
		/// <returns>
		/// The Json string, or null if input parameters are invalid.
		/// </returns>
		/// <param name='data'>
		/// Data.
		/// </param>
        public static string ConstructJson(NetworkGetRequestData data)
        {
			JsonObject b = new JsonObject();
			
			if (data.serviceType == NetworkServiceType.invalid)
			{
            	throw new ArgumentException("parameter must be set.", "serviceType");
			}
			
            if (data.serviceType != NetworkServiceType.friend_list)
            {
				JsonObject p = new JsonObject();
				ConstructAppData(ref p);
				
                if (data.scoreboardTokens == null || data.scoreboardTokens.Length == 0)
                {
                    throw new ArgumentException("parameter must be set.", "scoreboardTokens");
                }

                if (data.serviceType == NetworkServiceType.friend_score_board)
                {
                    if (data.scoreboardTokens.Length != 1)
                    {
                        throw new ArgumentException("parameter must be of length 1.", "scoreboardTokens");
                    }
					p.Add(NETWORK_KEY_SB_TOKENS, data.scoreboardTokens[0]);
                }
                else
                {
					JsonArray a = new JsonArray();
					foreach(string s in data.scoreboardTokens)
					{
						a.Add(s);
					}
					p.Add(NETWORK_KEY_SB_TOKENS, a);
                }
				
                if (data.scoreboardTypes != null && data.scoreboardTypes.Length > 0)
                {
					JsonArray a = new JsonArray();
					foreach(uint i in data.scoreboardTypes)
					{
						a.Add(i);
					}					
					p.Add(NETWORK_KEY_SB_TYPES, a);
                }

                if (data.limit > 0)
                {
					p.Add(NETWORK_KEY_LIMIT, data.limit);
                }

                if (data.offset > 0)
                {
					p.Add(NETWORK_KEY_OFFSET, data.offset);
                }
				
				b.Add(NETWORK_KEY_PAYLOAD, p);
            }
			
			JsonObject u = new JsonObject();
			u.Add(NETWORK_KEY_TICKET, Network.ServerTicket);
			b.Add(NETWORK_KEY_USER, u);
			
			JsonObject o = new JsonObject();
			ConstructHeader(ref o);
			o.Add(NETWORK_KEY_BODY, b);
			
			return o.ToString();
        }
		
		/// <summary>
		/// Constructs a Json string for a "set" data requrest.
		/// </summary>
		/// <returns>
		/// The Json string, or null if input parameters are invalid.
		/// </returns>
		/// <param name='data'>
		/// Data.
		/// </param>		
        public static string ConstructJson(NetworkSetRequestData data)
        {	
            if (data.scoreboardToken == null || data.scoreboardToken == "")
            {
                throw new ArgumentException("parameter must be set.", "scoreboardToken");
            }			
			
            if (data.score == null || data.score == "")
            {
                throw new ArgumentException("parameter must be set.", "score");
            }			
			
			JsonObject o = new JsonObject();
			JsonObject b = new JsonObject();
			JsonObject p = new JsonObject();

			ConstructAppData(ref p);
			p.Add(NETWORK_KEY_SB_TOKENS, data.scoreboardToken);
			p.Add(NETWORK_KEY_SCORE, data.score);

            if (data.metadata != null && data.metadata != "")
            {
				p.Add(NETWORK_KEY_METADATA, data.metadata);
            }
			
			JsonObject u = new JsonObject();
			u.Add(NETWORK_KEY_TICKET, Network.ServerTicket);
			b.Add(NETWORK_KEY_USER, u);			
			
			b.Add(NETWORK_KEY_PAYLOAD, p);
			ConstructHeader(ref o);
			o.Add(NETWORK_KEY_BODY, b);
				
			return o.ToString();
        }		
		
		/// <summary>
		/// Parse the specified jsonResponse.
		/// </summary>
		/// <param name='jsonResponse'>
		/// Json response.
		/// </param>
       	public static NetworkResponseData Parse(string jsonResponse)
        {
            NetworkResponseData response = new NetworkResponseData();

            try
            {
                JsonValue val = JsonObject.Parse(jsonResponse);

            	if (val.ContainsKey(NETWORK_KEY_ERROR))
			    {
				    response.error = (string)val.GetValue(NETWORK_KEY_ERROR).GetValue(NETWORK_KEY_ERROR_TYPE);
			    }
                else if (val.ContainsKey(NETWORK_KEY_BODY))
                {
					JsonValue body = val.GetValue(NETWORK_KEY_BODY);

                    response.username = (string)val.GetValue(NETWORK_KEY_BODY).GetValue(NETWORK_KEY_USER);

 		            if (body.ContainsKey(NETWORK_KEY_SB_TOKENS))
		            {
			            JsonValue boards = body.GetValue(NETWORK_KEY_SB_TOKENS);
						if (boards.Count == 0)
						{
							return response;
						}

                        response.scoreboard = new Dictionary<string, Dictionary<string, List<NetworkResponseData.ScoreData>>>();
						IEnumerator<KeyValuePair<string, JsonValue>> board = boards.GetEnumerator();
			            while(board.MoveNext())
			            {
			                KeyValuePair<string, JsonValue> boardVal = board.Current;
				            response.scoreboard.Add(boardVal.Key, new Dictionary<string, List<NetworkResponseData.ScoreData>>());   
				            IEnumerator<KeyValuePair<string, JsonValue>> type = boardVal.Value.GetEnumerator();
			                while(type.MoveNext())
				            {
					            KeyValuePair<string, JsonValue> typeVal = type.Current;
								// TODO implement
					            if (typeVal.Key == NETWORK_KEY_RANK)
						            continue;
        	
					            if (typeVal.Key == NETWORK_KEY_TOTAL)
						            continue;

					            response.scoreboard[boardVal.Key].Add(typeVal.Key, new List<NetworkResponseData.ScoreData>());
        						
					            JsonArray userArray = (JsonArray)typeVal.Value;
        						
					            for (int i=0; i<userArray.Count; ++i)
					            {
						            response.scoreboard[boardVal.Key][typeVal.Key].Add(new NetworkResponseData.ScoreData((string)userArray[i][0], userArray[i][1][0].ToString(), (string)userArray[i][1][1]));
					            }
				            } 
			            } 
		            }
		            else if (body.ContainsKey(NETWORK_KEY_FRIENDS))
		            {
                        response.friends = new List<string>();
			            JsonArray friendsArray = (JsonArray)val.GetValue(NETWORK_KEY_BODY).GetValue(NETWORK_KEY_FRIENDS);
			            for (int i=0; i<friendsArray.Count; ++i)
			            {
				            response.friends.Add((string)friendsArray[i]);
			            }
        				
		            }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Json parsing failed!");
				Console.WriteLine(e);
            }

            return response;
        }
		
		
	}
}

