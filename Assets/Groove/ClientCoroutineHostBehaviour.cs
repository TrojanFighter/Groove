using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
	public class ClientCoroutineHostBehaviour : MonoBehaviour
	{

		public WebSocketClient Client;

		public static ClientCoroutineHostBehaviour Instance { get; set; }

		public bool SocketConnected 
		{
			get 
			{
				return Client != null && Client.Connected;
			}
		}

		[SerializeField]
		private bool UseSecureClient = false;

		private void Start()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

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
			Client = new WebSocketClient(uri);
			Client.Connect();
		}

		public void Disconnect()
		{
			Client.Close();
		}

	}
}