using UnityEngine;
using System.Collections;

public class WheelSpin : MonoBehaviour
{
	void SpinWheel(float accel)
	{
		transform.Rotate(Vector3.right, accel);
	}
}
