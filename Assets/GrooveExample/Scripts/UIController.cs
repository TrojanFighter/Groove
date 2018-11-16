using UnityEngine;

class UIController : MonoBehaviour
{

	public void StartServer()
	{
		GrooveEnabledNetworkManager.Instance.StartServer();
	}

	public void StartClient()
	{
		GrooveEnabledNetworkManager.Instance.StartClient();
	}
}

