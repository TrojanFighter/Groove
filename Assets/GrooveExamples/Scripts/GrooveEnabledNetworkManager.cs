using Mirror;
using UnityEngine;

public class GrooveEnabledNetworkManager : NetworkManager {
	public static GrooveEnabledNetworkManager Instance { get; private set; }

	private void Start()
	{
		Instance = this;
	}

	public override void InitializeTransport()
	{
		Transport.layer = new GrooveTransport();
		//Transport.layer = new LLAPITransport();
	}

	public override void OnServerAddPlayer(NetworkConnection conn, NetworkReader extraMessageReader)
	{
		base.OnServerAddPlayer(conn, extraMessageReader);
		var c = GameObject.Find("Player(Clone)");
		var d = c.GetComponent<NetworkIdentity>();
		Debug.Log(d.assetId);
	}
}
