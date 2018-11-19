#if !UNITY_WEBGL || UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Mirror
{
	public class MirrorWebSocketBehavior : WebSocketBehavior
	{
		protected override void OnOpen()
		{
			base.OnOpen();
			int connId;
			var AddPlayer = GrooveWebSocketServer.AddConnectionId(ID, out connId);
			if (AddPlayer)
			{
				GrooveTransport.AddMessage(new WebSocketMessage { ConnectionId = ID, Type = TransportEvent.Connected, Data = null });
			}
			else
			{
				GrooveTransport.AddMessage(new WebSocketMessage { ConnectionId = ID, Type = TransportEvent.Disconnected });
			}
		}
		protected override void OnMessage(MessageEventArgs e)
		{
			Debug.Log("Got Message from: " + ID);
			GrooveTransport.AddMessage(new WebSocketMessage
			{
				ConnectionId = this.ID,
				Type = TransportEvent.Data,
				Data = e.RawData
			});
		}

		protected override void OnClose(CloseEventArgs e)
		{
			base.OnClose(e);
			GrooveTransport.AddMessage(new WebSocketMessage { ConnectionId = ID, Type = TransportEvent.Disconnected, Data = null });
		}
	}

	public class WebSocketMessage
	{
		public string ConnectionId;
		public TransportEvent Type;
		public byte[] Data;
	}
}
#endif