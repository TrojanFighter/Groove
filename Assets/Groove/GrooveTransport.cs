using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Groove
{
	public class GrooveTransport : TransportLayer
	{

		public WebSocketClientContainer Client = new WebSocketClientContainer();

		public WebSocketServerContainer Server = new WebSocketServerContainer();

		// events for the client
		public event Action OnClientConnect;
		public event Action<byte[]> OnClientData;
		public event Action<Exception> OnClientError;
		public event Action OnClientDisconnect;

		// events for the server
		public event Action<int> OnServerConnect;
		public event Action<int, byte[]> OnServerData;
		public event Action<int, Exception> OnServerError;
		public event Action<int> OnServerDisconnect;

		public GrooveTransport()
		{

		}

		public void ClientConnect(string address, int port)
		{
			Client.OnClientConnect += Client_OnClientConnect;
			Client.OnClientData += Client_OnClientData;
			Client.OnClientDisconnect += Client_OnClientDisconnect;
			Client.OnClientError += Client_OnClientError;
			Client.Connect(address, port);
		}

		private void Client_OnClientError(Exception obj)
		{
			OnClientError.Invoke(obj);
		}

		private void Client_OnClientDisconnect()
		{
			OnClientDisconnect.Invoke();
		}

		private void Client_OnClientConnect()
		{
			OnClientConnect.Invoke();
		}

		private void Client_OnClientData(byte[] obj)
		{
			OnClientData(obj);
		}

		public bool ClientConnected()
		{
			return Client.SocketConnected;
		}

		public virtual void ClientDisconnect()
		{
			Client.Disconnect();
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

		public virtual bool ServerActive()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			return Server.ServerActive;
#else
			return false;
#endif
		}

		public virtual bool ServerDisconnect(int connectionId)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			return Server.RemoveConnectionId(connectionId);
#else
			return false;
#endif
		}

		public void ServerStart(string address, int port, int maxConnections)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			BindServerEvents();
			Server.StartServer(address, port, maxConnections);
#else
			Debug.LogError("DoN't StArT tHe SeRvEr On WeBgL");
#endif
		}

		public void ServerStartWebsockets(string address, int port, int maxConnections)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			BindServerEvents();
			Server.StartServer(address, port, maxConnections);
#else
			Debug.LogError("DoN't StArT tHe SeRvEr On WeBgL");
#endif
		}

#if !UNITY_WEBGL || UNITY_EDITOR
		private void BindServerEvents()
		{
			Server.OnServerConnect += Server_OnServerConnect;
			Server.OnServerData += Server_OnServerData;
			Server.OnServerDisconnect += Server_OnServerDisconnect;
			Server.OnServerError += Server_OnServerError;
		}
#endif

		private void Server_OnServerError(int arg1, Exception arg2)
		{
			OnServerError.Invoke(arg1, arg2);
		}

		private void Server_OnServerDisconnect(int obj)
		{
			OnServerDisconnect.Invoke(obj);
		}

		private void Server_OnServerData(int arg1, byte[] arg2)
		{
			OnServerData.Invoke(arg1, arg2);
		}

		private void Server_OnServerConnect(int obj)
		{
			OnServerConnect.Invoke(obj);
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

		public int GetMaxPacketSize(int channelId = 0)
		{
			return int.MaxValue;
		}

		public virtual void ClientSend(int channelId, byte[] data)
		{
			Client.ClientInterface.Send(data);
		}

		public virtual void ServerSend(int connectionId, int channelId, byte[] data)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			Debug.Log("Sending data");
			Server.Send(connectionId, data);
#else
#endif
		}
	}
}
