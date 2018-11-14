using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
	class GrooveTransport : TransportLayer
	{
		public WebSocketClient Client;
		public GrooveWebSocketServer Server = new GrooveWebSocketServer();

		static Queue<WebSocketMessage> Messages = new Queue<WebSocketMessage>();

		public static void AddMessage(WebSocketMessage msg)
		{
			Messages.Enqueue(msg);
		}

		public void ClientConnect(string address, int port)
		{
			throw new System.NotImplementedException();
		}

		public bool ClientConnected()
		{
			throw new System.NotImplementedException();
		}

		public void ClientDisconnect()
		{
			throw new System.NotImplementedException();
		}

		public bool ClientGetNextMessage(out TransportEvent transportEvent, out byte[] data)
		{
			throw new System.NotImplementedException();
		}

		public bool ClientSend(int channelId, byte[] data)
		{
			throw new System.NotImplementedException();
		}

		public bool GetConnectionInfo(int connectionId, out string address)
		{
			throw new System.NotImplementedException();
		}

		public bool ServerActive()
		{
			throw new System.NotImplementedException();
		}

		public bool ServerDisconnect(int connectionId)
		{
			throw new System.NotImplementedException();
		}

		public bool ServerGetNextMessage(out int connectionId, out TransportEvent transportEvent, out byte[] data)
		{
			transportEvent = Mirror.TransportEvent.Disconnected;
			data = null;
			connectionId = 0;
			if (Messages.Count == 0) {
				return false;
			}
			else
			{
				var d = Messages.Dequeue();
				Debug.Log(d.ConnectionId);
				connectionId = System.BitConverter.ToInt32(System.Text.Encoding.UTF8.GetBytes(d.ConnectionId), 0);
				var c = System.Text.Encoding.UTF8.GetString(d.Data);
				var Packet = c.Split('|');
				var TransportEvent = Packet[0];
				var PacketData = Packet[1];
				data = System.Text.Encoding.UTF8.GetBytes(PacketData);
				Debug.Log("transport event rcvd: " + TransportEvent);
				Debug.Log("data received: " + PacketData);
				return true;
			}
		}

		public bool ServerSend(int connectionId, int channelId, byte[] data)
		{
			throw new System.NotImplementedException();
		}

		public void ServerStart(string address, int port, int maxConnections)
		{
			Server.StartServer(address, port, maxConnections);
		}

		public void ServerStartWebsockets(string address, int port, int maxConnections)
		{
			throw new System.NotImplementedException();
		}

		public void ServerStop()
		{
			throw new System.NotImplementedException();
		}

		public void Shutdown()
		{
			throw new System.NotImplementedException();
		}
	}
}
