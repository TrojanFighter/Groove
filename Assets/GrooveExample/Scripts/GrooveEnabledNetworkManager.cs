using Mirror;

namespace GrooveExample
{
	public class GrooveEnabledNetworkManager : NetworkManager
	{
		public static GrooveEnabledNetworkManager Instance { get; private set; }

		private void Start()
		{
			Instance = this;
		}

		public override void InitializeTransport()
		{
			// Use this if you are smart:
			Transport.layer = new GrooveTransport();
			// Use this if you want to go insane:
			//Transport.layer = new LLAPITransport();
		}
	}
}