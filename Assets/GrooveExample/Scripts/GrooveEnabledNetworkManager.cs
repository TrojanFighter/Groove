using Mirror;
using Mirror.Groove;
using UnityEngine;
using UnityEngine.Rendering;

namespace GrooveExample
{
	public class GrooveEnabledNetworkManager : NetworkManager
	{
		private void Start()
		{
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
			{
				// headless mode.   Just start the server
				StartServer();
			}
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