using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamLogic : MonoBehaviour
{
	public AudioClip cheer;
	
	public List<GameObject> allCrowds;
	public float maxDistToStands = 20.0f;
	
	public Transform bike;
	public Transform backTire;

	private DrewBikePhysics bikePhysics;
	private DrewBackTire drewBackTire;

	private bool isLookingSide = false;

	// Use this for initialization
	void Start ()
	{
		bikePhysics = bike.GetComponent<DrewBikePhysics>();
		allCrowds = new List<GameObject>(GameObject.FindGameObjectsWithTag("Crowd"));
		drewBackTire = backTire.GetComponent<DrewBackTire>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckViewAngle();
		TurboControl();
	}
	
	void CheckViewAngle()
	{
		float angleToBike = Vector3.Angle(transform.right, bike.transform.right);

		//looking mostly to the side
		if (angleToBike > 55)
		{
			if (drewBackTire.isGrounded)
			{
				bikePhysics.hasCrashed = true;
			}
			isLookingSide = true;
		}
		//looking mostly straight
		else
		{
			isLookingSide = false;
		}
	}
	
	void TurboControl()
	{
		//sort by distance
		allCrowds.Sort(delegate(GameObject a1, GameObject a2) { return Vector3.Distance(a1.transform.position, transform.position).CompareTo(
				Vector3.Distance(a2.transform.position, transform.position)); });

		//find relative position
		Vector3 dirToCrowd = allCrowds[0].transform.position - transform.position;
		float sqrDist = dirToCrowd.sqrMagnitude;

		float angle = Vector3.Angle(dirToCrowd, transform.forward);

		//looking in general direction but cap how far away a stand can be
//		if (angle < 20 && !drewBackTire.isGrounded && sqrDist < maxDistToStands * maxDistToStands && isLookingSide)
		if (angle < 20 && !drewBackTire.isGrounded && isLookingSide)
		{
			//call cheering crowd animation and cheer sound FX
			allCrowds[0].BroadcastMessage("Cheer", 0, SendMessageOptions.DontRequireReceiver);
			allCrowds[0].audio.PlayOneShot(cheer, 0.6f);
			
			//apply turbo
			if (bikePhysics.turboBar < bikePhysics.maxTurboBar)
			{
				bikePhysics.turboBar += bikePhysics.turboBoostSpeed;
			}
		}
		if (angle > 20)
		{
			//call idle crowd animation
			allCrowds[0].BroadcastMessage("Idle", 0, SendMessageOptions.DontRequireReceiver);
		}
	}
}
