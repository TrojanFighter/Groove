using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCoroutineHostBehaviour : MonoBehaviour {

	public WebSocketClient Client;

	public static ClientCoroutineHostBehaviour Instance { get; set; }

	private void Start()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void Connect()
	{
		Debug.Log("connecting");
		StartCoroutine(ConnectInternal());
		Debug.Log("connected");
	}

	private IEnumerator ConnectInternal()
	{
		Debug.Log("started connectInternal");
		var address = "10.1.10.65";
		var port = 7777;
		var d = new System.UriBuilder(address);
		d.Port = port;
		d.Scheme = "ws://";
		d.Path += "game";
		Debug.Log("attempting to start client on: " + d.Uri.ToString());
		Client = new WebSocketClient(d.Uri);
		yield return StartCoroutine(Client.Connect());
		Client.SendString("Connected|brr");
	}
	
}
