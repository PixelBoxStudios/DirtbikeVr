using UnityEngine;
using System.Collections.Generic;

public class BikeAI : MonoBehaviour
{
	public List<GameObject> allTargets;
	public int curTarget;
	
	public List<GameObject> allAIBikes;
	
	public float rotSpeed = 20.0f;
	public float forwardSpeed = 22.0f;
	public float maxSpeed = 25.0f;
	public float steerAngle = 10.0f;
	public float deccelSpeed = 40.0f;
	
	public float distFromBikes = 10.0f;
	public float distFromWaypoint = 5.0f;
	
	public Transform backTireTrans;
	public Transform bikeBody;
	public Transform lapController;
	public Transform bike;
	
	public bool hasCrashed = false;
	public bool isOnWaypoint = false;
	public bool isJumping = false;
	
	public float accelFactor = 0.0f;
	private float curSpeed = 0.0f;
	
	public int curLap = 0;
	
	private Quaternion initRot;
	
	private Vector3 moveDir;
	private Vector3 rotDir;
	private Vector3 initAngle;
	
	private CheckPoints checkPoints;
	private DrewBackTire backTire;
	private LapController lapCounter;
	
	private NavMeshAgent agent;
	
	void Awake()
	{
		backTire = backTireTrans.GetComponent<DrewBackTire>();
		checkPoints = bike.GetComponent<CheckPoints>();
		lapCounter = lapController.GetComponent<LapController>();
	}
	
	// Use this for initialization
	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
		//add all waypoints
		allTargets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
		//add all AI bikes
		allAIBikes = new List<GameObject>(GameObject.FindGameObjectsWithTag("AI"));
		
		//put all the waypoints in order by name
		allTargets.Sort(delegate(GameObject a1, GameObject a2) { return a1.name.CompareTo(a2.name); });
		
		curSpeed = maxSpeed;
		initRot = bikeBody.rotation;
		initAngle = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		AIPhysics();
		//		Jump();
		ApplyGravity();
		//		Spacing();
	}
	
	void Jump()
	{
		RaycastHit hitInfo;
		
		if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 5))
		{
			if (hitInfo.transform.tag == "Jump" && backTire.isGrounded)
			{
				isJumping = true;
			}
		}
		if (isJumping)
		{
			agent.Stop(true);
			
			if (backTire.isGrounded)
			{
				//				rigidbody.AddRelativeForce((Vector3.forward * 20 + Vector3.up * 10) * Time.deltaTime, ForceMode.Impulse);
				rigidbody.velocity = (transform.forward * 15 + Vector3.up * 20);
			}
			//			rigidbody.AddForce((transform.forward * 10 + Vector3.up * 10) * Time.deltaTime, ForceMode.Impulse);
		}
		else
		{
			agent.Resume ();
		}
	}
	
	void Stabilize()
	{
		/*	if out of range of waypoint, straighten out bike.
		 *	else when in range allow for leaning
		 */
		Vector3 rot = transform.eulerAngles;
		rot.z = Mathf.MoveTowardsAngle(rot.z, initAngle.z, 20 * Time.deltaTime);
		transform.eulerAngles = rot;
	}
	
	void ApplyGravity()
	{
		Vector3 vel = rigidbody.velocity;
		vel.y -= 20 * Time.deltaTime;
		rigidbody.velocity = vel;
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
		else if (!LevelScripts.isGreen)
		{
			//accelerate
			accelFactor = Mathf.MoveTowards(accelFactor, curSpeed, forwardSpeed * Time.deltaTime);
		}
		
		//go through the waypoints
		if (curTarget < allTargets.Count - 1)
		{
			Vector3 dir = allTargets[curTarget].transform.position - transform.position;
			float sqrMag = dir.sqrMagnitude;
			
			//			RaycastHit hitInfo;
			
			//            float dist = Vector3.Distance(allTargets[curTarget].transform.position, transform.position);
			
			//in range of waypoint
			if (sqrMag < distFromWaypoint * distFromWaypoint)
				//			if (Physics.Raycast(transform.position, transform.forward, out hitInfo, distFromWaypoint))
				//            {
				//				if (hitInfo.collider.tag == "Waypoint")
			{
				//not on a waypoint so count to next
				if (!isOnWaypoint)
				{
					curTarget++;
				}
				isOnWaypoint = true;
			}
			else
			{
				isOnWaypoint = false;
				//when not in range of a waypoint stabilize the bike
				if (backTire.isGrounded)
				{
					Stabilize();
				}
			}
			//            }
			//			Debug.DrawRay(transform.position, transform.forward * distFromWaypoint, Color.blue);
			
			if (curTarget != 0)
			{
				//				agent.SetDestination(allTargets[curTarget].transform.position);
				//align to previous waypoint
				Vector3 lookDir = allTargets[curTarget - 1].transform.eulerAngles;
				Vector3 rot = transform.eulerAngles;
				rot.y = Mathf.MoveTowardsAngle(rot.y, lookDir.y, rotSpeed * Time.deltaTime);
				transform.eulerAngles = rot;
				//			transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up);
				//            	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), rotSpeed * Time.deltaTime);
			}
		}
		else
		{
			if (curLap >= LapController.lapCount)
			{
			    //slow to a stop
			    accelFactor = Mathf.MoveTowards(accelFactor, 0, deccelSpeed * Time.deltaTime);
			}
			else
			{
			//loop back to first waypoint
				curTarget = 0;
			}
		}
		
		moveDir = new Vector3(0, rigidbody.velocity.y, accelFactor);
		
		//move
		
		//if (LevelScripts.isGreen)
		//{
		transform.Translate(moveDir * Time.deltaTime);
		//}
	}
	
	void Spacing()
	{
		/*
		 * option 1:  keep distance from each AI with vector3.distance and the dot product
		 * then move opposite direction of the dot.
		 * option 2:  use raycasts left right or forward and if intersecting a bike with the AI tag
		 * move opposite direction of the intersecting ray
		 * */
		foreach(GameObject other in allAIBikes)
		{
			Vector3 side = transform.TransformDirection(Vector3.right);
			
			Vector3 offsetFromOther = other.transform.position - transform.position;
			float sqrLen = offsetFromOther.sqrMagnitude;
			
			//in range of a bike
			if (sqrLen < distFromBikes * distFromBikes)
			{
				float bikeOnRight = Vector3.Angle(offsetFromOther, side);
				float bikeOnLeft = Vector3.Angle(offsetFromOther, -side);
				
				if (bikeOnRight < 40)
				{
					print(other.transform.name + " bike on right side of " + transform.name);
					
					rotDir = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.LerpAngle(transform.eulerAngles.z, 10 * rotSpeed, 0.2f));
					moveDir.x = -5;
				}
				if (bikeOnLeft < 40)
				{
					print(other.transform.name + " bike on left side of " + transform.name);
					
					rotDir = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.LerpAngle(transform.eulerAngles.z, -10 * rotSpeed, 0.2f));
					moveDir.x = 5;
				}
			}
			else
			{
				rotDir = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.LerpAngle(transform.eulerAngles.z, 0, 0.2f));
				moveDir.x = 0;
			}
			//apply the rotation values
			//			transform.eulerAngles = rotDir;
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
	
	void OnTriggerExit(Collider col)
	{
		if (col.name == "finish line")
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
	
	//	void OnTriggerStay(Collider col)
	//	{
	//		if (col.tag == "Land")
	//		{
	//			if (backTire.isGrounded && isJumping)
	//			{
	//				isJumping = false;
	//			}
	//		}
	//	}
	
	//limit the angle
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
}
