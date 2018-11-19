using UnityEngine;

class UIController : MonoBehaviour
{
	[SerializeField]
	private GameObject ConnectionPanelContainer;

	public void StartServer()
	{
		GrooveEnabledNetworkManager.Instance.StartServer();
		ConnectionPanelContainer.SetActive(false);
	}

	public void StartClient()
	{
		GrooveEnabledNetworkManager.Instance.StartClient();
		ConnectionPanelContainer.SetActive(false);
	}
}

