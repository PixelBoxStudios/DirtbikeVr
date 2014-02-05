using UnityEngine;
using System.Collections;

public class AIWheelSpin : MonoBehaviour
{
	void SpinWheel(float accel)
	{
		transform.Rotate(Vector3.right, accel);
	}
}
