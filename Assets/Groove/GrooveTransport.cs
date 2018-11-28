using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
	public class GrooveTransport : TransportLayer
	{

		public bool ClientConnectedLastFrame = false;

		public GrooveClientContainer Client = new GrooveClientContainer();

#if !UNITY_WEBGL || UNITY_EDITOR
		public GrooveWebSocketServer Server = new GrooveWebSocketServer();

#endif

		public void ClientConnect(string address, int port)
		{
			Client.Connect(address, port);
		}

		public bool ClientConnected()
		{
			return Client.SocketConnected;
		}

		public void ClientDisconnect()
		{
			Client.Disconnect();
		}

		public bool ClientGetNextMessage(out TransportEvent transportEvent, out byte[] data)
		{
			transportEvent = TransportEvent.Disconnected;
			data = null;

			if (Client.ClientInterface != null)
			{
				if (!ClientConnectedLastFrame)
				{
					ClientConnectedLastFrame = Client.SocketConnected;
					if (ClientConnectedLastFrame)
					{
						transportEvent = TransportEvent.Connected;
						return true;
					}
				}
				try
				{
					var Rcvd = Client.ClientInterface.Recv();
					if (Rcvd != null)
					{
						transportEvent = TransportEvent.Data;
						data = Rcvd;
						return true;
					}
					else
					{
						return false;
					}
				}
				catch (System.Exception e)
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
			Client.ClientInterface.Send(data);
			return true;
		}

		public bool GetConnectionInfo(int connectionId, out string address)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			return Server.GetConnectionInfo(connectionId, out address);
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
			return Server.RemoveConnectionId(connectionId);
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

			WebSocketMessage message = Server.GetNextMessage();
			if (message == null)
				return false;
			connectionId = message.connectionId;

			switch (message.Type)
			{
				case TransportEvent.Connected:
					transportEvent = TransportEvent.Connected;
					break;
				case TransportEvent.Data:
					transportEvent = TransportEvent.Data;
					data = message.Data;
					break;
				case TransportEvent.Disconnected:
					transportEvent = TransportEvent.Disconnected;
					break;
				default:
					break;
			}
			return true;

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
#if !UNITY_WEBGL || UNITY_EDITOR
			Server.StopServer();
#endif
		}

		public void Shutdown()
		{
			if (Client != null)
			{
				if (Client.SocketConnected)
				{
					Client.Disconnect();
				}
			}
#if !UNITY_WEBGL || UNITY_EDITOR
			if (Server != null)
			{
				if (Server.ServerActive)
				{
					Server.StopServer();
				}
			}
#endif
		}

		public int GetMaxPacketSize()
		{
			return int.MaxValue;
		}
	}
}
