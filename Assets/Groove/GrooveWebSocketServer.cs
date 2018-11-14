using UnityEngine;
using WebSocketSharp.Server;

namespace Mirror
{
	public class GrooveWebSocketServer
	{
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

	}
}
