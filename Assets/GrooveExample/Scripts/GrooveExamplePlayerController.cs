using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrooveExamplePlayerController : MonoBehaviour {

	[SerializeField]
	private CharacterController Controller;

	[SerializeField]
	private float Speed;

	private void Update()
	{
		if (Input.GetKey(KeyCode.W))
		{
			Controller.Move(Vector3.forward*Time.deltaTime*Speed);
		}
		if (Input.GetKey(KeyCode.A))
		{
			Controller.Move(Vector3.left * Time.deltaTime * Speed);
		}
		if (Input.GetKey(KeyCode.S))
		{
			Controller.Move(Vector3.back * Time.deltaTime * Speed);
		}
		if (Input.GetKey(KeyCode.D))
		{
			Controller.Move(Vector3.right * Time.deltaTime * Speed);
		}
	}
}
