namespace Mirror
{
	class GrooveTransport : TransportLayer
	{
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
			throw new System.NotImplementedException();
		}

		public bool ServerSend(int connectionId, int channelId, byte[] data)
		{
			throw new System.NotImplementedException();
		}

		public void ServerStart(string address, int port, int maxConnections)
		{
			
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
