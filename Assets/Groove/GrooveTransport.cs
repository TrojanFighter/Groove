using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
	class GrooveTransport : TransportLayer
	{
		public WebSocketClient Client;

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
				var c = ClientCoroutineHostBehaviour.Instance.Client.Recv();
				if (c != null)
				{	
					var PacketString = System.Text.Encoding.UTF8.GetString(c);
					var Packet = PacketString.Split('|');
					var EventType = Packet[0];
					switch (EventType)
					{
						case "Connected":
							transportEvent = TransportEvent.Connected;
							break;
						case "Data":
							transportEvent = TransportEvent.Data;
							var PacketData = Packet[1];
							data = System.Text.Encoding.UTF8.GetBytes(PacketData);
							break;
						case "Disconnected":
							transportEvent = TransportEvent.Disconnected;
							break;
						default:
							break;
					}
					Debug.Log("received transport event: " + transportEvent);
					Debug.Log("received data: " + data);
					return true;
				}
				else
				{
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
			Client.Send(data);
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
			throw new System.NotImplementedException();
		}

		public bool ServerDisconnect(int connectionId)
		{
			throw new System.NotImplementedException();
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
				Debug.Log(d.ConnectionId);
				connectionId = GrooveWebSocketServer.GetMirrorConnectionId(d.ConnectionId);
				var c = System.Text.Encoding.UTF8.GetString(d.Data);
				var Packet = c.Split('|');
				var EventType = Packet[0];
				switch (EventType)
				{
					case "Connected":
						transportEvent = TransportEvent.Connected;
						Debug.Log("processed connected message");
						break;
					case "Data":
						transportEvent = TransportEvent.Data;
						var PacketData = Packet[1];
						data = System.Text.Encoding.UTF8.GetBytes(PacketData);
						Debug.Log("data received: " + PacketData);
						break;
					case "Disconnected":
						transportEvent = TransportEvent.Disconnected;
						break;
					default:
						break;
				}
				Debug.Log("transport event rcvd: " + transportEvent);
				return true;
			}
#else
			Debug.LogError("bad");
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
			Debug.LogError("can't start server on WebGL");
#endif
		}

		public void ServerStartWebsockets(string address, int port, int maxConnections)
		{
			throw new System.NotImplementedException();
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
