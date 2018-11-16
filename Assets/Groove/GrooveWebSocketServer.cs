using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if !UNITY_WEBGL || UNITY_EDITOR
using WebSocketSharp.Server;
#endif

namespace Mirror
{
	public class GrooveWebSocketServer
	{
#if !UNITY_WEBGL || UNITY_EDITOR
		WebSocketServer Server;

		public static Dictionary<int, string> ConnectionIdToWebSocketId = new Dictionary<int, string>();

		public bool ServerActive { get
			{
				return Server.IsListening;
			} }

		public static int GetMirrorConnectionId(string IdToGet)
		{
			return ConnectionIdToWebSocketId.FirstOrDefault(x => x.Value == IdToGet).Key;
		}

		public static bool RemoveConnectionId(int IdToRemove)
		{
			string WebSocketId;
			if(ConnectionIdToWebSocketId.TryGetValue(IdToRemove, out WebSocketId))
			{
				ConnectionIdToWebSocketId.Remove(IdToRemove);
				return true;
			}
			else
			{
				return false;
			}
		}

		public void StopServer()
		{
			Server.Stop();
			ConnectionIdToWebSocketId = new Dictionary<int, string>();
		}


		public void StartServer(string address, int port, int maxConnections)
		{
			var d = new System.UriBuilder(address);
			d.Port = port;
			d.Scheme = "ws://";
			if (Mirror.LogFilter.Debug)
			{
				Debug.Log("attempting to start WebSocket server on: " + d.Uri.ToString());
			}
			Server = new WebSocketServer(d.Uri.ToString());
			Server.AddWebSocketService<MirrorWebSocketBehavior>("/game");
			Server.Start();
		}

		public bool GetConnectionId(int idToGet, out string address)
		{
			address = "";
			var c = ConnectionIdToWebSocketId[idToGet];
			var results = Server.WebSocketServices["/game"].Sessions.ActiveIDs.Where(x => x == c);
			if (results.Any())
			{
				address = Server.WebSocketServices["/game"].Sessions.Sessions.Where(x => x.ID == results.First()).First().Context.UserEndPoint.Address.ToString();
				return true;
			}
			else
			{
				return false;
			}
		}

		internal bool Send(int connectionId, byte[] data)
		{
			var d = ConnectionIdToWebSocketId[connectionId];
			Server.WebSocketServices["/game"].Sessions.SendTo(data, d);
			return true;
		}

		internal static int AddConnectionId(string Id)
		{
			var d = System.BitConverter.ToInt32(System.Text.Encoding.UTF8.GetBytes(Id), 0);
			ConnectionIdToWebSocketId.Add(d, Id);
			return d;
		}
#else
		public void StartServer(string address, int port, int maxConnections){
			Debug.LogError("can't start server in WebGL");
		}
#endif


	}
}
