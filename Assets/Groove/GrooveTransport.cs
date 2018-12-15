using System;

namespace Mirror.Groove
{
	public class GrooveTransport : TransportLayer
	{
		#region Client Events
		public event Action OnClientConnect;
		public event Action<byte[]> OnClientData;
		public event Action<Exception> OnClientError;
		public event Action OnClientDisconnect;
		#endregion

		#region Server Events
		public event Action<int> OnServerConnect;
		public event Action<int, byte[]> OnServerData;
		public event Action<int, Exception> OnServerError;
		public event Action<int> OnServerDisconnect;
		#endregion

		protected WebSocketServerContainer Server = new WebSocketServerContainer();
		protected WebSocketClientContainer Client = new WebSocketClientContainer();

		public void ClientConnect(string address, int port)
		{
			BindClientEvents();
			var Address = new System.UriBuilder(address)
			{
				Port = port
			};
			Client.Connect(Address.Uri);
		}

		private void BindClientEvents()
		{
			Client.OnClientConnect += Client_OnClientConnect;
			Client.OnClientData += Client_OnClientData;
			Client.OnClientDisconnect += Client_OnClientDisconnect;
			Client.OnClientError += Client_OnClientError;
		}

		private void Client_OnClientError(Exception obj)
		{
			OnClientError.Invoke(obj);
		}

		private void Client_OnClientDisconnect()
		{
			OnClientDisconnect.Invoke();
		}

		private void Client_OnClientData(byte[] obj)
		{
			OnClientData.Invoke(obj);
		}

		private void Client_OnClientConnect()
		{
			OnClientConnect.Invoke();
		}

		public bool ClientConnected()
		{
			return Client.Connected;
		}

		public void ClientDisconnect()
		{
			Client.Disconnect();
		}

		public void ClientSend(int channelId, byte[] data)
		{
			Client.ClientSend(data);
		}

		public bool GetConnectionInfo(int connectionId, out string address)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			return Server.GetConnectionInfo(connectionId, out address);
#endif
			address = "";
			return false;
		}

		public int GetMaxPacketSize(int channelId = 0)
		{
			return int.MaxValue;
		}

		public bool ServerActive()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			return Server.ServerActive;
#endif
			return false;
		}

		public bool ServerDisconnect(int connectionId)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			return Server.Disconnect(connectionId);
#endif
			return false;
		}

		public void ServerSend(int connectionId, int channelId, byte[] data)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			Server.Send(connectionId, data);
#endif
		}

		public void ServerStart(string address, int port, int maxConnections)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			BindServerEvents();
			Server.StartServer(address, port, maxConnections);
#endif
		}

		public void ServerStartWebsockets(string address, int port, int maxConnections)
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			BindServerEvents();
			Server.StartServer(address, port, maxConnections);
#endif
		}

		public void ServerStop()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			if (Server.ServerActive)
			{
				Server.StopServer();
			}
#endif
		}

		public void Shutdown()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			if (Server.ServerActive)
			{
				Server.StopServer();
			}
#endif
			if (Client.Connected)
			{
				Client.Disconnect();
			}
		}

		#region Standalone Only Code (Server Only)
#if !UNITY_WEBGL || UNITY_EDITOR
		private void BindServerEvents()
		{
			Server.OnServerConnect += Server_OnServerConnect;
			Server.OnServerData += Server_OnServerData;
			Server.OnServerDisconnect += Server_OnServerDisconnect;
			Server.OnServerError += Server_OnServerError;
		}

		private void Server_OnServerConnect(int obj)
		{
			OnServerConnect.Invoke(obj);
		}

		private void Server_OnServerData(int arg1, byte[] arg2)
		{
			OnServerData.Invoke(arg1, arg2);
		}

		private void Server_OnServerDisconnect(int obj)
		{
			OnServerDisconnect.Invoke(obj);
		}

		private void Server_OnServerError(int arg1, Exception arg2)
		{
			OnServerError.Invoke(arg1, arg2);
		}
#endif
		#endregion
	}
}
