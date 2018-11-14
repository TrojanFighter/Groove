using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Mirror
{
	public class MirrorWebSocketBehavior : WebSocketBehavior
	{

		protected override void OnMessage(MessageEventArgs e)
		{
			Debug.Log("Got Message from: " + ID);
			GrooveTransport.AddMessage(new WebSocketMessage
			{
				ConnectionId = this.ID,
				Data = e.RawData
			});
		}
	}

	public class WebSocketMessage
	{
		public string ConnectionId;
		public byte[] Data;
	}
}
