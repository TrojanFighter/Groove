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

		public bool SocketConnected = false;

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
			StartCoroutine(ConnectInternal(address, port));
			if (LogFilter.Debug)
			{
				Debug.Log("WebSocket client connected");
			}
		}

		private IEnumerator ConnectInternal(string address, int port)
		{
			string scheme = UseSecureClient ? "wss://" : "ws://";

			Uri uri = new System.UriBuilder(scheme, address, port, "game").Uri;
			if (Mirror.LogFilter.Debug)
			{
				Debug.Log("attempting to start client on: " + uri.ToString());
			}
			Client = new WebSocketClient(uri);
			yield return StartCoroutine(Client.Connect());
			SocketConnected = true;
		}

		public void Disconnect()
		{
			Client.Close();
			SocketConnected = false;
		}

	}
}