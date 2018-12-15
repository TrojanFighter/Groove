using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
#if !UNITY_WEBGL || UNITY_EDITOR
using WebSocketSharp.Server;
#endif

namespace Mirror.Groove
{
	public class WebSocketMessage
	{
		public int connectionId;
		public TransportEvent Type;
		public byte[] Data;
	}


	public class WebSocketServerContainer
	{
#if !UNITY_WEBGL || UNITY_EDITOR
		WebSocketServer WebsocketServer;

		private readonly bool UseSecureServer = false;
		private string PathToCertificate;
		private readonly string CertificatePassword = "FillMeOutPlease";

		public WebSocketServerContainer()
		{
			PathToCertificate = Application.dataPath + "/../certificate.pfx";
		}

		readonly Dictionary<int, IWebSocketSession> WebsocketSessions = new Dictionary<int, IWebSocketSession>();
		public int MaxConnections { get; private set; }

		// events for the server
		public event Action<int> OnServerConnect;
		public event Action<int, byte[]> OnServerData;
		public event Action<int, Exception> OnServerError;
		public event Action<int> OnServerDisconnect;

		int connectionIdCounter = 1;

		internal int NextId()
		{
			return Interlocked.Increment(ref connectionIdCounter);
		}

		internal void OnConnect(int connectionId, IWebSocketSession socketBehavior)
		{
			OnServerConnect.Invoke(connectionId);
		}

		internal void OnMessage(int connectionId, byte[] data)
		{
			OnServerData.Invoke(connectionId, data);
		}

		internal void OnDisconnect(int connectionId)
		{
			OnServerDisconnect.Invoke(connectionId);
		}

		public bool ServerActive { get { return WebsocketServer != null && WebsocketServer.IsListening; } }

		public bool RemoveConnectionId(int connectionId)
		{
			lock (WebsocketSessions)
			{
				IWebSocketSession session;

				if (WebsocketSessions.TryGetValue(connectionId, out session))
				{
					session.Context.WebSocket.Close();
					WebsocketSessions.Remove(connectionId);
					return true;
				}
			}
			return false;
		}

		public void StopServer()
		{
			WebsocketServer.Stop();
			lock (WebsocketSessions)
			{
				WebsocketSessions.Clear();
			}
		}


		public void StartServer(string address, int port, int maxConnections)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			//string scheme = UseSecureServer ? "wss://" : "ws://";
			//if (string.IsNullOrEmpty(address))
			//{
			//	address = "0.0.0.0";
			//}

			//Uri uri = new System.UriBuilder(scheme, address, port).Uri;
			//if (Mirror.LogFilter.Debug)
			//{
			//	Debug.Log("attempting to start WebSocket server on: " + uri.ToString());
			//}
			MaxConnections = maxConnections;
			//WebsocketServer = new WebSocketServer(uri.ToString());
			//WebsocketServer = new WebSocketServer(port);
			WebsocketServer = new WebSocketServer(port, UseSecureServer);

			WebsocketServer.AddWebSocketService<MirrorWebSocketBehavior>("/game", (behaviour) =>
			{
				behaviour.Server = this;
				behaviour.connectionId = NextId();
			});

			if (UseSecureServer)
			{
				WebsocketServer.SslConfiguration.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToCertificate, CertificatePassword);
			}
			WebsocketServer.Start();
#else
			Debug.Log("don't start the server on webgl please");
#endif
		}

		public bool GetConnectionInfo(int connectionId, out string address)
		{
			lock (WebsocketSessions)
			{
				IWebSocketSession session;

				if (WebsocketSessions.TryGetValue(connectionId, out session))
				{
					address = session.Context.UserEndPoint.Address.ToString();
					return true;
				}
			}
			address = null;
			return false;
		}

		public bool Send(int connectionId, byte[] data)
		{
			lock (WebsocketSessions)
			{
				IWebSocketSession session;

				if (WebsocketSessions.TryGetValue(connectionId, out session))
				{
					session.Context.WebSocket.Send(data);
					return true;
				}
			}
			return false;
		}

		public bool Disconnect(int connectionId)
		{
			lock (WebsocketSessions)
			{
				IWebSocketSession session;
				if(WebsocketSessions.TryGetValue(connectionId, out session))
				{
					WebsocketServer.WebSocketServices["/game"].Sessions.CloseSession(session.ID);
					return true;
				}
				else
				{
					return false;
				}
			}
		}

#else
		public void StartServer(string address, int port, int maxConnections){
			Debug.LogError("can't start server in WebGL");
		}
#endif


	}
}
