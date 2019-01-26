using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace TwitchPlaysParasite
{

	public class ParasitePlayer : NetworkBehaviour
	{

		[SyncVar] public string Id;

		[SyncVar(hook = "HealthChanged")] public int Health;

		public List<Behaviour> ComponentsToDisableOnRemote;

		private void Start()
		{
			if (!isLocalPlayer)
			{
				foreach (var item in ComponentsToDisableOnRemote)
				{
					item.enabled = false;
				}
			}
		}

		public override void OnStartServer()
		{
			base.OnStartServer();
			Id = System.Guid.NewGuid().ToString();
		}

		public void HealthChanged(int Health)
		{
			Debug.Log("new health: " + Health);
		}
	}
}