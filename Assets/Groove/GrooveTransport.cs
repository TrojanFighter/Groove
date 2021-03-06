﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Groove
{
	public class GrooveTransport : MonoBehaviour, ITransport
	{
		public int Port = 7777;

		[SerializeField]
		private bool SecureServer = false;

		[SerializeField]
		private string PathToCertificate = "/../certificate.pfx";

		[SerializeField]
		private string CertificatePassword = "FillMeOut";

		public WebSocketClientContainer Client = new WebSocketClientContainer();

#if !UNITY_WEBGL || UNITY_EDITOR
		public WebSocketServerContainer Server = new WebSocketServerContainer();

#endif

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
			WebSocketMessage msg;
			bool GotMessage = Client.GetNextMessage(out msg);
			if (GotMessage)
			{
				transportEvent = msg.Type;
				data = msg.Data;
			}
			else
			{
				transportEvent = TransportEvent.Disconnected;
				data = null;
			}
			return GotMessage;
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

		public int GetMaxPacketSize(int channelId = 0)
		{
			return int.MaxValue;
		}

		public void ClientConnect(string address)
		{
			Client.Connect(address, Port, SecureServer);
		}

		public void ServerStart()
		{
#if !UNITY_WEBGL || UNITY_EDITOR
			if (SecureServer)
			{
				Server.StartServer(Port, PathToCertificate, CertificatePassword);
			}
			else
			{
				Server.StartServer(Port);
			}
			
#else
			Debug.LogError("DoN't StArT tHe SeRvEr On WeBgL");
#endif
		}
	}
}
