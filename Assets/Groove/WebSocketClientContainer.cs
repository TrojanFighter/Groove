using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Groove
{
	public class WebSocketClientContainer
	{
		public WebSocketClient ClientInterface;

		public bool SocketConnected { get { return ClientInterface != null && ClientInterface.Connected; } }

		[SerializeField]
		private bool UseSecureClient = false;

		private bool ClientConnectedLastFrame = false;

		public event Action OnClientConnect;
		public event Action<byte[]> OnClientData;
		public event Action<Exception> OnClientError;
		public event Action OnClientDisconnect;

		public void Connect(string address, int port)
		{
			WebSocketClient.OnClientData += WebSocketClient_OnClientData;
			WebSocketClient.OnClientConnect += WebSocketClient_OnClientConnect;
			WebSocketClient.OnClientDisconnect += WebSocketClient_OnClientDisconnect;
			WebSocketClient.OnClientError += WebSocketClient_OnClientError;
			ConnectInternal(address, port);
			
			if (LogFilter.Debug)
			{
				Debug.Log("WebSocket client connected");
			}
		}

		private void WebSocketClient_OnClientError(Exception obj)
		{
			OnClientError.Invoke(obj);
		}

		private void WebSocketClient_OnClientDisconnect()
		{
			OnClientDisconnect.Invoke();
		}

		private void WebSocketClient_OnClientConnect()
		{
			OnClientConnect.Invoke();
		}

		private void WebSocketClient_OnClientData(byte[] obj)
		{
			OnClientData.Invoke(obj);
		}

		private void ConnectInternal(string address, int port)
		{
			string scheme = UseSecureClient ? "wss://" : "ws://";

			Uri uri = new System.UriBuilder(scheme, address, port, "game").Uri;
			if (Mirror.LogFilter.Debug)
			{
				Debug.Log("attempting to start client on: " + uri.ToString());
			}
			ClientInterface = new WebSocketClient(uri);
			ClientInterface.Connect();
		}

		public void Disconnect()
		{
			ClientInterface.Close();
		}
	}
}