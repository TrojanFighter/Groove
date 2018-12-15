using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;

public class WebSocket
{
	private System.Uri mUrl;

	public WebSocket(System.Uri url)
	{
		mUrl = url;

		string protocol = mUrl.Scheme;
		if (!protocol.Equals("ws") && !protocol.Equals("wss"))
			throw new System.ArgumentException("Unsupported protocol: " + protocol);
	}

	public static bool SocketConnected = false;

	#region Client Events
	public static event System.Action OnClientConnect;
	public static event System.Action<byte[]> OnClientData;
	public static event System.Action<System.Exception> OnClientError;
	public static event System.Action OnClientDisconnect;
	#endregion

	#region Unity Callback Types
	public delegate void SingleIntCallback(System.IntPtr ptr);

	public delegate void TwoIntCallback(System.IntPtr ptr, System.IntPtr Length);

	public delegate void SimpleCallback();
	#endregion

	#region JSLIB Imports
	[DllImport("__Internal")]
	private static extern int SocketCreate (string url, SimpleCallback Connected, TwoIntCallback DataReceived, SingleIntCallback Error, SimpleCallback Disconnected);

	[DllImport("__Internal")]
	private static extern int SocketState (int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketSend (int socketInstance, byte[] ptr, int length);

	[DllImport("__Internal")]
	private static extern void SocketClose (int socketInstance);

	[DllImport("__Internal")]
	private static extern int SocketError (int socketInstance, byte[] ptr, int length);
	#endregion

	#region Unity Callbacks
	[AOT.MonoPInvokeCallback(typeof(SimpleCallback))]
	public static void Connected()
	{
		SocketConnected = true;
		Debug.Log("connected callback received by Unity");
		OnClientConnect.Invoke();
	}

	[AOT.MonoPInvokeCallback(typeof(TwoIntCallback))]
	public static void Data(System.IntPtr ptr, System.IntPtr Length)
	{
		Debug.Log("Data callback received by Unity");
		var L = Length.ToInt32();
		byte[] msg = new byte[L];
		Marshal.Copy(ptr, msg, 0, L);
		OnClientData.Invoke(msg);
	}

	[AOT.MonoPInvokeCallback(typeof(SingleIntCallback))]
	public static void ErrorCallback(System.IntPtr Err)
	{
		OnClientError.Invoke(new System.Exception("WebSocket Error Code: " + Err));
	}

	[AOT.MonoPInvokeCallback(typeof(SimpleCallback))]
	public static void DisconnectedCallback()
	{
		SocketConnected = false;
		OnClientDisconnect.Invoke();
	}
	#endregion

	int m_NativeRef = 0;

	public void Send(byte[] buffer)
	{
		SocketSend (m_NativeRef, buffer, buffer.Length);
	}

	public void Connect()
	{
		Debug.Log("Initiating JS Layer connecting to: " + mUrl.ToString());
		m_NativeRef = SocketCreate(mUrl.ToString(), Connected, Data, ErrorCallback, DisconnectedCallback);
	}
 
	public void Close()
	{
		SocketClose(m_NativeRef);
		SocketConnected = false;
	}

	public string error
	{
		get {
			const int bufsize = 1024;
			byte[] buffer = new byte[bufsize];
			int result = SocketError (m_NativeRef, buffer, bufsize);

			if (result == 0)
				return null;

			return Encoding.UTF8.GetString (buffer);				
		}
	}

	//WebSocketSharp.WebSocket m_Socket;
	//bool m_IsConnected = false;
	//string m_Error = null;

	//public IEnumerator Connect()
	//{
	//	m_Socket = new WebSocketSharp.WebSocket(mUrl.ToString());
	//	m_Socket.OnMessage += (sender, e) => OnClientData(e.RawData);
	//	m_Socket.OnOpen += (sender, e) => OnConnect();
	//	m_Socket.OnError += (sender, e) => OnClientError(e.Exception);
	//	m_Socket.OnClose += (sender, e) => OnClientDisconnect.Invoke();
	//	m_Socket.ConnectAsync();
	//	while (!m_IsConnected && m_Error == null)
	//		yield return 0;
	//}

	//private void OnConnect()
	//{
	//	m_IsConnected = true;
	//	OnClientConnect.Invoke();
	//}

	//public void Send(byte[] buffer)
	//{
	//	m_Socket.Send(buffer);
	//}

	//public void Close()
	//{
	//	m_Socket.Close();
	//}

	//public string error
	//{
	//	get {
	//		return m_Error;
	//	}
	//}

}