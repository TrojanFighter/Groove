using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
	public class GrooveTransport : TransportLayer
	{
		WebSocket WsClient;

		public bool ClientConnected()
		{
			return WsClient.m_IsConnected;
		}

		public void ClientConnect(string address, int port)
		{
			var d = new System.UriBuilder(address);
			d.Port = port;
			var c = d.Uri;
			WsClient = new WebSocket(c);
			WsClient.Connect();
		}

		public bool ClientSend(int channelId, byte[] data)
		{
			var d = System.Text.Encoding.UTF8.GetString(data);
			var c = "Data|" + d;
			WsClient.SendString(c);
			return true;
		}

		public bool ClientGetNextMessage(out TransportEvent transportEvent, out byte[] data)
		{
			var d = WsClient.RecvString();
			transportEvent = TransportEvent.Disconnected;
			data = null;

			if (d != null)
			{
				var PacketData = d.Split('|');
				var TransportType = PacketData[0];
				var Data = System.Text.Encoding.UTF8.GetBytes(PacketData[0]);
				Debug.Log(TransportType);
				Debug.Log(Data);
				switch (TransportType)
				{
					case "Connected":
						transportEvent = TransportEvent.Connected;
						break;
					case "Data":
						transportEvent = TransportEvent.Data;
						data = Data;
						break;
					default:
						break;
				}
				return true;
			}
			return false;
		}

		public void ClientDisconnect()
		{
			WsClient.Close();
		}

		public bool ServerActive()
		{
			return WsClient.ServerConnected();
		}

		public void ServerStart(string address, int port, int maxConnections)
		{
			var d = new System.UriBuilder(address);
			d.Port = port;
			var c = d.Uri;
			WsClient.StartServer(c);
		}

		public void ServerStartWebsockets(string address, int port, int maxConnections)
		{
			var d = new System.UriBuilder(address);
			d.Port = port;
			var c = d.Uri;
			WsClient.StartServer(c);
		}

		public bool ServerSend(int connectionId, int channelId, byte[] data)
		{
			WsClient.ServerSend(connectionId, data);
			return true;
		}

		public bool ServerGetNextMessage(out int connectionId, out TransportEvent transportEvent, out byte[] data)
		{
			throw new System.NotImplementedException();
		}

		public bool ServerDisconnect(int connectionId)
		{
			throw new System.NotImplementedException();
		}

		public bool GetConnectionInfo(int connectionId, out string address)
		{
			throw new System.NotImplementedException();
		}

		public void ServerStop()
		{
			WsClient.StopServer();
		}

		public void Shutdown()
		{
			if (ServerActive())
			{
				WsClient.StopServer();
			}
			WsClient.Close();
		}
	}
}