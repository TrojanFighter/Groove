using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace TwitchPlaysParasite
{
	public class ParasitePlayerController : RewiredBase
	{
		[SyncVar] 
		public Vector2 m_targetVector;

		public override void OnStartServer()
		{
			base.OnStartServer();
			//Initialize();
		}

		public override void OnStartClient()
		{
			Initialize();
		}

		protected override void Start()
		{
			//Initialize();
		}


		protected override void GetInput()
		{
			base.GetInput();
			m_targetVector.x = player.GetAxis("Horizontal");
			m_targetVector.y = player.GetAxis("Vertical");
		}
	}
}
