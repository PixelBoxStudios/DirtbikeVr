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
	private bool isOnFinishLine = false;
	
	private float accelFactor = 0.0f;
	
	public int curLap = 0;
	public int curTarget = 0;
	
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
		else if (LevelScripts.isGreen)  //@@ - set to check for true not false
		{
			//accelerate
			accelFactor = Mathf.MoveTowards(accelFactor, maxSpeed, forwardSpeed * Time.deltaTime);
		}
		
		//go through the waypoints
		float dist = 10.0f;

		//sort by distance
//		allTargets.Sort(delegate(GameObject a1, GameObject a2) { return Vector3.Distance(a1.transform.position, transform.position).CompareTo(
//				Vector3.Distance(a2.transform.position, transform.position)); });

//		foreach(GameObject waypoint in allTargets)
//		{
			
		if (curTarget < allTargets.Count)
		{
			Vector3 toWaypoint = allTargets[curTarget].transform.position - transform.position;
			float sqrMag = toWaypoint.sqrMagnitude;

//			//in range of waypoint
			if (sqrMag < distFromWaypoint * distFromWaypoint)
			{
		
				//not on a waypoint so asign current
				if (!isOnWaypoint)
				{
					curTarget++;
					curWaypoint = allTargets[curTarget].transform;
//					dist = sqrMag;
				}
				isOnWaypoint = true;
			}
				
			else
			{
				//out of range so reset some values
//				dist = distFromWaypoint;
				isOnWaypoint = false;
			}
		}
		else
		{
			curTarget = 0;
		}

		//straighten out the bike if grounded
		if (backTire.isGrounded)
		{
			Stabilize();
		}

		//align to current waypoint
//		Vector3 lookDir = curWaypoint.eulerAngles;
		Vector3 lookDir = allTargets[curTarget].transform.position - transform.position;
		Quaternion rot = transform.rotation;
//		rot.y = Mathf.MoveTowardsAngle(rot.y, lookDir.y, rotSpeed * Time.deltaTime);
		rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), rotSpeed * Time.deltaTime);
//		rot.x = ClampAngle(transform.eulerAngles.x, 60.0f);
		rot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
		transform.rotation = rot;

		if (curLap >= LapController.lapCount)
		{
		    //slow to a stop
		    accelFactor = Mathf.MoveTowards(accelFactor, 0, deccelSpeed * Time.deltaTime);
		}

		//move
		moveDir = new Vector3(0, rigidbody.velocity.y, accelFactor);
		transform.Translate(moveDir * Time.deltaTime);

		if (accelFactor != 0)
		{
			BroadcastMessage("SpinWheel", accelFactor, SendMessageOptions.DontRequireReceiver);
		}
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
	
	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Finish Line")
		{
			if (!isOnFinishLine)
			{
				//count next lap
				curLap++;
				//record rank position
				if (curLap == LapController.lapCount)
				{
					lapCounter.RecordRank(transform.gameObject);
				}
			}
			isOnFinishLine = true;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Finish Line")
		{
			isOnFinishLine = false;
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
