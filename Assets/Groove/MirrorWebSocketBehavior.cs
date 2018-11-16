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
			var connId = GrooveWebSocketServer.AddConnectionId(ID);
			Debug.Log("conn opened by: " + connId);
			GrooveTransport.AddMessage(new WebSocketMessage { ConnectionId = ID, Type = TransportEvent.Connected, Data = null });
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