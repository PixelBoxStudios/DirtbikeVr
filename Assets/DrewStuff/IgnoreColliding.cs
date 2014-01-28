using UnityEngine;
using System.Collections;

public class IgnoreColliding : MonoBehaviour
{
    public Transform finishLine;
    public Transform checkPoint;

	// Use this for initialization
	void Start ()
    {
        Physics.IgnoreCollision(finishLine.collider, collider);
        Physics.IgnoreCollision(checkPoint.collider, collider);
	}
}
