using UnityEngine;

namespace Mirror.Groove
{
	public class WebSocketClientContainer
	{
		protected WebSocket ClientInterface;

		public bool UseSecureClient = false;

		public bool Connected
		{
			get
			{
				return WebSocket.SocketConnected;
			}
		}

		#region Client Events
		public event System.Action OnClientConnect;
		public event System.Action<byte[]> OnClientData;
		public event System.Action<System.Exception> OnClientError;
		public event System.Action OnClientDisconnect;
		#endregion

		public void ClientSend(byte[] data)
		{
			ClientInterface.Send(data);
		}

		public void Disconnect()
		{
			ClientInterface.Close();
			ClientInterface = null;
		}

		public void Connect(System.Uri address)
		{
			var Adr = new System.UriBuilder(address);
			Adr.Path += "game";
			Adr.Scheme = UseSecureClient ? "wss://" : "ws://";
			ClientInterface = new WebSocket(Adr.Uri);
			BindClientEvents();
			Debug.Log("connecting to: " + Adr.Uri.ToString());
			ClientInterface.Connect();
		}

		private void BindClientEvents()
		{
			WebSocket.OnClientConnect += ClientInterface_OnClientConnect;
			WebSocket.OnClientData += ClientInterface_OnClientData;
			WebSocket.OnClientDisconnect += ClientInterface_OnClientDisconnect;
			WebSocket.OnClientError += ClientInterface_OnClientError;
		}

		private void ClientInterface_OnClientError(System.Exception obj)
		{
			OnClientError.Invoke(obj);
		}

		private void ClientInterface_OnClientDisconnect()
		{
			OnClientDisconnect.Invoke();
		}

		private void ClientInterface_OnClientData(byte[] obj)
		{
			OnClientData.Invoke(obj);
		}

		private void ClientInterface_OnClientConnect()
		{
			OnClientConnect.Invoke();
		}
	}
}