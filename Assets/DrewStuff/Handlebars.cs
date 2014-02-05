using UnityEngine;
using System.Collections;

public class Handlebars : MonoBehaviour
{
	public static Handlebars instance;

	public float turnRadius = 20.0f;

	// Use this for initialization
	void Awake ()
	{
		instance = this;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void TurnWheel(float turnDir)
	{
		Vector3 rotDir = transform.localEulerAngles;
		rotDir.z = Mathf.LerpAngle(rotDir.z, turnRadius * turnDir, 0.3f);
		transform.localEulerAngles = rotDir;
	}
}
