using Mirror;

public class GrooveEnabledNetworkManager : NetworkManager {

	public override void InitializeTransport()
	{
		Transport.layer = new GrooveTransport();
	}
}
