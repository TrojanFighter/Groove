using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTClientBehaviour : MonoBehaviour {

	WebSocketClient Client;

	// Use this for initialization
	IEnumerator Start () {
		var address = "10.1.10.65";
		var port = 7777;
		var d = new System.UriBuilder(address);
		d.Port = port;
		d.Scheme = "ws://";
		d.Path += "game";
		Debug.Log("attempting to start client on: " + d.Uri.ToString());
		Client = new WebSocketClient(d.Uri);
		yield return StartCoroutine(Client.Connect());
		Debug.Log("connected");
	}
	
}
