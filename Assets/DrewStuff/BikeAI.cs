using UnityEngine;
using System.Collections.Generic;

//PLEASE REMEMBER TO CHANGE LINES WITH THIS SYMBOL (@@) ON IT BACK TO THEIR CORRECT STATES!!!

public class BikeAI : MonoBehaviour
{
	public List<GameObject> allTargets;
	public Transform curWaypoint;

	public List<GameObject> allAIBikes;
	
	public float rotSpeed = 20.0f;
	public float stabilizeSpeed = 15.0f;
	public float forwardSpeed = 22.0f;
	public float maxSpeed = 25.0f;
	public float deccelSpeed = 40.0f;
	
	public float distFromBikes = 10.0f;
	public float distFromWaypoint = 5.0f;
	
	public Transform backTireTrans;
	public Transform bikeBody;
	public Transform lapController;
	public Transform bike;

	[HideInInspector]
	public bool hasCrashed = false;
	private bool isOnWaypoint = false;
	
	private float accelFactor = 0.0f;
	
	private int curLap = 0;
	
	private Quaternion initRot;
	
	private Vector3 moveDir;
	private Vector3 rotDir;
	private Vector3 initAngle;
	
	private CheckPoints checkPoints;
	private DrewBackTire backTire;
	private LapController lapCounter;
	
	void Awake()
	{
		backTire = backTireTrans.GetComponent<DrewBackTire>();
		checkPoints = bike.GetComponent<CheckPoints>();
		lapCounter = lapController.GetComponent<LapController>();
	}
	
	// Use this for initialization
	void Start ()
	{
		//add all waypoints
		allTargets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
		//add all AI bikes
		allAIBikes = new List<GameObject>(GameObject.FindGameObjectsWithTag("AI"));
		
		//put all the waypoints in order by name
		allTargets.Sort(delegate(GameObject a1, GameObject a2) { return a1.name.CompareTo(a2.name); });

		//set first waypoint
		curWaypoint = allTargets[0].transform;

		initRot = bikeBody.rotation;
		initAngle = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		AIPhysics();
	}

	void Stabilize()
	{
		/*	if out of range of waypoint, straighten out bike.
		 *	else when in range allow for leaning
		 */
		Vector3 rot = transform.eulerAngles;
		rot.z = Mathf.MoveTowardsAngle(rot.z, initAngle.z, stabilizeSpeed * Time.deltaTime);
		transform.eulerAngles = rot;
	}
	
	void AIPhysics()
	{
		if (hasCrashed)
		{
			accelFactor = 0;
			
			//we crashed so respawn
			if (!IsInvoking("Respawn"))
			{
				Invoke("Respawn", 0);
			}
		}
		else if (!LevelScripts.isGreen)  //@@ - set to check for true not false
		{
			//accelerate
			accelFactor = Mathf.MoveTowards(accelFactor, maxSpeed, forwardSpeed * Time.deltaTime);
		}
		
		//go through the waypoints
		float dist = 0.0f;

		foreach(GameObject waypoint in allTargets)
		{
			Vector3 toWaypoint = waypoint.transform.position - transform.position;
			float sqrMag = toWaypoint.sqrMagnitude;
			
			//in range of waypoint
			if (sqrMag < dist * dist)
			{
				//not on a waypoint so asign current
				if (!isOnWaypoint)
				{
					curWaypoint = waypoint.transform;
					dist = sqrMag;
				}
				isOnWaypoint = true;
			}
			else
			{
				//out of range so reset some values
				dist = distFromWaypoint;
				isOnWaypoint = false;

				//straighten out the bike if grounded
				if (backTire.isGrounded)
				{
					Stabilize();
				}
			}
		}

		//align to current waypoint
		Vector3 lookDir = curWaypoint.eulerAngles;
		Vector3 rot = transform.eulerAngles;
		rot.y = Mathf.MoveTowardsAngle(rot.y, lookDir.y, rotSpeed * Time.deltaTime);
		transform.eulerAngles = rot;

		if (curLap >= LapController.lapCount)
		{
		    //slow to a stop
		    accelFactor = Mathf.MoveTowards(accelFactor, 0, deccelSpeed * Time.deltaTime);
		}

		//move
		moveDir = new Vector3(0, rigidbody.velocity.y, accelFactor);
		transform.Translate(moveDir * Time.deltaTime);
	}

	void Respawn()
	{
		rigidbody.freezeRotation = true;

		bikeBody.localRotation = initRot;
		transform.rotation = checkPoints.currentCheckpoint.rotation;
		transform.position = checkPoints.currentCheckpoint.position;

		rigidbody.freezeRotation = false;
		hasCrashed = false;
	}
	
	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Finish Line")
		{
			//count next lap
			curLap++;
			//record rank position
			if (curLap == LapController.lapCount)
			{
				lapCounter.RecordRank(transform.gameObject);
			}
		}
	}

	#region I don't know if I'll need this later so for now it stays - Drew
	float ClampAngle(float angle, float limit)
	{
		if (angle > 180)
		{
			float angleB = 360 - angle;
			
			if (angleB > limit)
			{
				angle = 360 - limit;
			}
			else
			{
				return angle;
			}
		}
		if (angle < 180)
		{
			if (angle > limit && angle < 360 - limit)
			{
				angle = limit;
			}
			else
			{
				return angle;
			}
		}
		
		return angle;
	}
	#endregion
}
