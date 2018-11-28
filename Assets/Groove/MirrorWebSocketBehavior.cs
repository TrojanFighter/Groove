#if !UNITY_WEBGL || UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Mirror
{
	public class MirrorWebSocketBehavior : WebSocketBehavior
	{
		internal GrooveWebSocketServer Server;

		internal int connectionId = 0;

		public MirrorWebSocketBehavior()
		{
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			Server.OnConnect(connectionId, this);
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			Server.OnMessage(connectionId, e.RawData);
		}

		protected override void OnClose(CloseEventArgs e)
		{
			Server.OnDisconnect(connectionId);
			base.OnClose(e);
		}

		public void SendData(byte[] data)
		{
			base.Send(data);
		}
	}

}
#endif