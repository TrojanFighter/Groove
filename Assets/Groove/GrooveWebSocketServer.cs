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


		public void StartServer(string address, int port, int maxConnections)
		{
			var d = new System.UriBuilder(address);
			d.Port = port;
			d.Scheme = "ws://";
			Debug.Log("attempting to start server on: " + d.Uri.ToString());
			Server = new WebSocketServer(d.Uri.ToString());
			Server.AddWebSocketService<MirrorWebSocketBehavior>("/game");
			Server.Start();
		}

		public bool GetConnectionId(int idToGet, out string address)
		{
			address = "";
			var results = Server.WebSocketServices["/game"].Sessions.ActiveIDs.Where(x => System.BitConverter.ToInt32(System.Text.Encoding.UTF8.GetBytes(x), 0) == idToGet);
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
			
		}
#else
		public void StartServer(string address, int port, int maxConnections){
			Debug.LogError("can't start server in WebGL");
		}
#endif


	}
}
