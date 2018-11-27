using UnityEngine;
using System.Collections;
using System;

public class EchoTest : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		WebSocketClient w = new WebSocketClient(new Uri("ws://echo.websocket.org"));
		w.Connect();

		while (!w.Connected)
			yield return 0;

		w.SendString("Hi there");
		int i=0;
		while (true)
		{
			string reply = w.RecvString();
			if (reply != null)
			{
				Debug.Log ("Received: "+reply);
				w.SendString("Hi there"+i++);
			}
			if (w.error != null)
			{
				Debug.LogError ("Error: "+w.error);
				break;
			}
			yield return 0;
		}
		w.Close();
	}
}
