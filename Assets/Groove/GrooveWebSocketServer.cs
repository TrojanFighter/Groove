using System;
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

		public static int MaxConnections { get; private set; }

		public bool ServerActive { get
			{
				return Server.IsListening;
			} }

		private readonly bool UseSecureServer = false;
		private string PathToCertificate;
		private readonly string CertificatePassword = "FillMeOutPlease";

		public GrooveWebSocketServer()
		{
			PathToCertificate = Application.dataPath + "/../certificate.pfx";
		}

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
# if !UNITY_WEBGL || UNITY_EDITOR
			string scheme = UseSecureServer ? "wss://" : "ws://";
			if (string.IsNullOrEmpty(address))
			{
				address = "0.0.0.0";
			}

			Uri uri = new System.UriBuilder(scheme, address, port).Uri;
			if (Mirror.LogFilter.Debug)
			{
				Debug.Log("attempting to start WebSocket server on: " + uri.ToString() + " host: " + address);
			}
			MaxConnections = maxConnections;
			Server = new WebSocketServer(uri.ToString());
			Server.AddWebSocketService<MirrorWebSocketBehavior>("/game");
			if (UseSecureServer)
			{
				Server.SslConfiguration.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToCertificate, CertificatePassword);
			}
			Server.Start();
#else
			Debug.Log("don't start the server on webgl please");
#endif
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

		internal static bool AddConnectionId(string Id, out int ConnectionId)
		{
			if (ConnectionIdToWebSocketId.Count < MaxConnections)
			{
				ConnectionId = System.BitConverter.ToInt32(System.Text.Encoding.UTF8.GetBytes(Id), 0);
				ConnectionIdToWebSocketId.Add(ConnectionId, Id);
				return true;
			}
			else
			{
				ConnectionId = 0;
				return false;
			}
		}
#else
		public void StartServer(string address, int port, int maxConnections){
			Debug.LogError("can't start server in WebGL");
		}
#endif


		}
}
