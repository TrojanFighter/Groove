using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
	class GrooveTransport : TransportLayer
	{

		public bool ClientConnectedLastFrame = false;

#if !UNITY_WEBGL || UNITY_EDITOR
		public GrooveWebSocketServer Server = new GrooveWebSocketServer();

		static Queue<WebSocketMessage> ServerMessages = new Queue<WebSocketMessage>();


		public static void AddMessage(WebSocketMessage msg)
		{
			ServerMessages.Enqueue(msg);
		}
#endif

		public void ClientConnect(string address, int port)
		{
			//var d = new System.UriBuilder(address);
			//d.Port = port;
			//d.Scheme = "ws://";
			//d.Path += "game";
			//Debug.Log("attempting to start client on: " + d.ToString());
			//Client = new WebSocketClient(d.Uri);
			//ClientConnectInternal();
			ClientCoroutineHostBehaviour.Instance.Connect();
		}

		//public IEnumerator ClientConnectInternal()
		//{
		//	yield return Client.Connect();
		//	Debug.Log("connected");
		//}

		public bool ClientConnected()
		{
			return ClientCoroutineHostBehaviour.Instance.SocketConnected;
		}

		public void ClientDisconnect()
		{
			throw new System.NotImplementedException();
		}

		public bool ClientGetNextMessage(out TransportEvent transportEvent, out byte[] data)
		{
			transportEvent = TransportEvent.Disconnected;
			data = null;

			if (ClientCoroutineHostBehaviour.Instance.Client != null)
			{
				if (!ClientConnectedLastFrame)
				{
					ClientConnectedLastFrame = ClientCoroutineHostBehaviour.Instance.SocketConnected;
					if (ClientConnectedLastFrame)
					{
						transportEvent = TransportEvent.Connected;
						return true;
					}
				}
				try
				{
					var c = ClientCoroutineHostBehaviour.Instance.Client.Recv();
					if (c != null)
					{
						transportEvent = TransportEvent.Data;
						data = c;
						return true;
					}
					else
					{
						return false;
					}
				}
				catch(System.Exception e)
				{
					Debug.LogError("Client Exception: " + e);
					return false;
				}
			}
			else
			{
				return false;
			}
			
		}

		public bool ClientSend(int channelId, byte[] data)
		{
			ClientCoroutineHostBehaviour.Instance.Client.Send(data);
			return true;
		}

		public bool GetConnectionInfo(int connectionId, out string address)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			return Server.GetConnectionId(connectionId, out address);
#else
			address = "";
			return false;
#endif
		}

		public bool ServerActive()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			return Server.ServerActive;
			#else
			return false;
#endif
		}

		public bool ServerDisconnect(int connectionId)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			return GrooveWebSocketServer.RemoveConnectionId(connectionId);
#else
			return false;
#endif
		}

		public bool ServerGetNextMessage(out int connectionId, out TransportEvent transportEvent, out byte[] data)
		{

			transportEvent = Mirror.TransportEvent.Disconnected;
			data = null;
			connectionId = 0;
#if !UNITY_WEBGL || UNITY_EDITOR
			if (ServerMessages.Count == 0) {
				return false;
			}
			else
			{
				var d = ServerMessages.Dequeue();
				connectionId = GrooveWebSocketServer.GetMirrorConnectionId(d.ConnectionId);
				switch (d.Type)
				{
					case TransportEvent.Connected:
						transportEvent = TransportEvent.Connected;
						break;
					case TransportEvent.Data:
						transportEvent = TransportEvent.Data;
						data = d.Data;
						break;
					case TransportEvent.Disconnected:
						transportEvent = TransportEvent.Disconnected;
						break;
					default:
						break;
				}
				return true;
			}
#else
			Debug.LogError("DoN't StArT tHe SeRvEr On WeBgL");
			return false;
#endif
		}

		public bool ServerSend(int connectionId, int channelId, byte[] data)
		{
#if !UNITY_WEBGL || UNITY_EDITOR

			return Server.Send(connectionId, data);
#else
			return false;
#endif
		}

		public void ServerStart(string address, int port, int maxConnections)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			Server.StartServer(address, port, maxConnections);
#else
			Debug.LogError("DoN't StArT tHe SeRvEr On WeBgL");
#endif
		}

		public void ServerStartWebsockets(string address, int port, int maxConnections)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			Server.StartServer(address, port, maxConnections);
#else
			Debug.LogError("DoN't StArT tHe SeRvEr On WeBgL");
#endif
		}

		public void ServerStop()
		{
			throw new System.NotImplementedException();
		}

		public void Shutdown()
		{
			throw new System.NotImplementedException();
		}
	}
}
