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
#else
		public void StartServer(string address, int port, int maxConnections){
			Debug.LogError("can't start server in WebGL");
		}
#endif


	}
}
