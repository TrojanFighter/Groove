using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public class WebSocketClient
{
	private Uri mUrl;

	public static event Action OnClientConnect;
	public static event Action<byte[]> OnClientData;
	public static event Action<Exception> OnClientError;
	public static event Action OnClientDisconnect;


	public WebSocketClient(Uri url)
	{
		mUrl = url;
		Debug.Log(mUrl);

		string protocol = mUrl.Scheme;
		if (!protocol.Equals("ws") && !protocol.Equals("wss"))
			throw new ArgumentException("Unsupported protocol: " + protocol);
	}


	public delegate void MessageRecvCallback(System.IntPtr ptr, System.IntPtr Length);

	[MonoPInvokeCallback(typeof(MessageRecvCallback))]
	public static void MsgRcvdCallback(System.IntPtr ptr, System.IntPtr Length)
	{
		int ReadLength = Length.ToInt32();
		byte[] msg = new byte[ReadLength];
		Debug.Log("Got data of: " + ReadLength);
		Marshal.Copy(ptr, msg, 0, ReadLength);
		OnClientData.Invoke(msg);
	}

	public delegate void ConnectedCallback();

	[MonoPInvokeCallback(typeof(ConnectedCallback))]
	public static void CtdCallback()
	{
		OnClientConnect.Invoke();
	}

	public delegate void DisconnectedCallback();

	[MonoPInvokeCallback(typeof(DisconnectedCallback))]
	public static void DisctdCallback()
	{
		OnClientDisconnect.Invoke();
	}

	public delegate void ErrorCallback(System.IntPtr ErrorCode);

	[MonoPInvokeCallback(typeof(ErrorCallback))]
	public static void ErrorCbk(System.IntPtr Code)
	{
		OnClientError.Invoke(new System.Exception("WebSocket Close Code: "+Code.ToString()));
	}


	[DllImport("__Internal")]
	private static extern int SocketCreate (string url, MessageRecvCallback action, ConnectedCallback ConnectedAction, DisconnectedCallback DisconnectedAction, ErrorCallback ErrorAction);

	[DllImport("__Internal")]
	private static extern int SocketState (int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketSend (int socketInstance, byte[] ptr, int length);

	[DllImport("__Internal")]
	private static extern void SocketClose (int socketInstance);

	int m_NativeRef = 0;

	public void Send(byte[] buffer)
	{
		SocketSend (m_NativeRef, buffer, buffer.Length);
	}

	public void Connect()
	{
		m_NativeRef = SocketCreate (mUrl.ToString(), MsgRcvdCallback, CtdCallback, DisctdCallback, ErrorCbk);
	}
 
	public bool Connected 
	{
		get
		{
			return SocketState(m_NativeRef) != 0;
		}
	}

	public void Close()
	{
		SocketClose(m_NativeRef);
	}


//	WebSocketSharp.WebSocket m_Socket;
//	WebSocketSharp.Server.WebSocketServer m_Server;
//	Queue<byte[]> m_Messages = new Queue<byte[]>();
//	string m_Error = null;

//	bool m_IsConnected = false;

//	public bool Connected
//	{
//		get {
//			return m_IsConnected;
//		}
//	}

//	public void Connect()
//	{
//		m_Socket = new WebSocketSharp.WebSocket(mUrl.ToString());
//		m_Socket.OnMessage += (sender, e) => { lock (m_Messages) { m_Messages.Enqueue(e.RawData); } };
//		m_Socket.OnOpen += (sender, e) => m_IsConnected = true;
//		m_Socket.OnError += (sender, e) => m_Error = e.Message;
//		m_Socket.Connect();
//	}

//	public void Send(byte[] buffer)
//	{
//		m_Socket.Send(buffer);
//	}

//	public byte[] Recv()
//	{
//		lock (m_Messages)
//		{
//			if (m_Messages.Count == 0)
//				return null;
//			return m_Messages.Dequeue();
//		}
//	}

//	public void Close()
//	{
//		m_IsConnected = false;
//		m_Socket.Close();
//	}

//	public string error
//	{
//		get {
//			return m_Error;
//		}
//	}

//#endif 
}