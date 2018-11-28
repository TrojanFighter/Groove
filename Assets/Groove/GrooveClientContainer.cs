using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
	public class GrooveClientContainer
	{
		public WebSocketClient ClientInterface;

		public bool SocketConnected { get { return ClientInterface != null && ClientInterface.Connected; } }

		[SerializeField]
		private bool UseSecureClient = false;

		public void Connect(string address, int port)
		{
			ConnectInternal(address, port);
			if (LogFilter.Debug)
			{
				Debug.Log("WebSocket client connected");
			}
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