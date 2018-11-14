using Mirror;

public class GrooveEnabledNetworkManager : NetworkManager {
	public static GrooveEnabledNetworkManager Instance { get; private set; }

	private void Start()
	{
		Instance = this;
	}

	public override void InitializeTransport()
	{
		Transport.layer = new GrooveTransport();
	}
}
