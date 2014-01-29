using UnityEngine;
using System.Collections.Generic;

public class BikeAI : MonoBehaviour
{
    private List<GameObject> allTargets;
    private int curTarget;

	public List<GameObject> allAIBikes;

    public float rotSpeed = 20.0f;
    public float forwardSpeed = 22.0f;
    public float maxSpeed = 25.0f;
    public float steerAngle = 10.0f;
    public float deccelSpeed = 40.0f;

	public float distFromBikes = 10.0f;

    public Transform backTireTrans;
    public Transform bikeBody;
    public Transform lapController;
    public Transform bike;

    public bool hasCrashed = false;

    public float accelFactor = 0.0f;
    private float curSpeed = 0.0f;

    public int curLap = 0;

    private Quaternion initRot;

    private Vector3 moveDir;
    private Vector3 rotDir;

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

        curSpeed = maxSpeed;
        initRot = bikeBody.rotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        AIPhysics();
		Spacing();
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
        else if (LevelScripts.isGreen)
        {
            //accelerate
            accelFactor = Mathf.MoveTowards(accelFactor, curSpeed, forwardSpeed * Time.deltaTime);
        }
        
        //go through the waypoints
        if (curTarget <= allTargets.Count - 1)
        {
            Vector3 dir = allTargets[curTarget].transform.position - transform.position;
            float dist = Vector3.Distance(allTargets[curTarget].transform.position, transform.position);

            if (dist < 5)
            {
                curTarget++;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);
        }
        else
        {
            //if (curLap >= LapCounter.lapCount)
            //{
            //    //slow to a stop
            //    accelFactor = Mathf.MoveTowards(accelFactor, 0, deccelSpeed * Time.deltaTime);
            //}
            //else
            //{
                //loop back to first waypoint
                curTarget = 0;
            //}
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
			
			if (sqrLen < distFromBikes * distFromBikes)
			{
				float bikeOnRight = Vector3.Angle(offsetFromOther, side);
				float bikeOnLeft = Vector3.Angle(offsetFromOther, -side);
				
				if (bikeOnRight < 40)
				{
					print(other.transform.name + " bike on right side of " + transform.name);
				}
				if (bikeOnLeft < 40)
				{
					print(other.transform.name + " bike on left side of " + transform.name);
				}
			}
		}
	}

    void Respawn()
    {
        bikeBody.localRotation = initRot;
        transform.rotation = checkPoints.currentCheckpoint.rotation;
        transform.position = checkPoints.currentCheckpoint.position;

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
