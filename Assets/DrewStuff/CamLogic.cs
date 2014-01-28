using UnityEngine;
using System.Collections.Generic;

public class CamLogic : MonoBehaviour
{
    public AudioClip cheer;

    public GameObject[] allCrowds;
    public GameObject curCrowd;
    public List<GameObject> allFans;
    public float angle;
    //private GameObject[] allFans;

    public Transform bike;
    public Transform backTire;

    private CrowdAnimation[] crowdAnimation;
    private DrewBikePhysics bikePhysics;
    private DrewBackTire drewBackTire;

	// Use this for initialization
	void Start ()
    {
        bikePhysics = bike.GetComponent<DrewBikePhysics>();
        allCrowds = GameObject.FindGameObjectsWithTag("Crowd");
        drewBackTire = backTire.GetComponent<DrewBackTire>();

        //curCrowd = allCrowds[0];
	}
	
	// Update is called once per frame
	void Update ()
    {
        TurboControl();
	}

    void TurboControl()
    {
        //curCrowd = allCrowds[0];

        if (!curCrowd)
        {
            foreach (GameObject fanStand in allCrowds)
            {
                float dist = Vector3.Distance(bike.transform.position, fanStand.transform.position);

                if (dist < 30)
                {
                    curCrowd = fanStand;
                }
            }
        }
        else
        {
            //for (int i = 0; i < allCrowds.Length; i++)
            foreach (GameObject fanStand in allCrowds)
            {
                float dist = Vector3.Distance(bike.transform.position, fanStand.transform.position);
                //Vector3 dist = bike.transform.position - fanStand.transform.position;
                //get direction to current crowd
                //Vector3 dirToCrowd = allCrowds[i].transform.position - transform.position;
                Vector3 dirToCrowd = fanStand.transform.position - transform.position;
                //closest distance to crowd
                //crowdDist = Vector3.Distance(bike.transform.position, allCrowds[i].transform.position);
                //crowdDist = bike.transform.position - allCrowds[i].transform.position;
                //crowdDist = bike.transform.position - fanStand.transform.position;
                //the angle
                angle = Vector3.Angle(dirToCrowd, transform.forward);
                //float dotProduct = Vector3.Dot(transform.forward, dirToCrowd.normalized);

                //float distX = Mathf.Abs(dist.x);
                //float distZ = Mathf.Abs(dist.z);
                //crowdDistX = Mathf.Abs(crowdDist.x);

                if (dist < 60)
                {
                    //set current stand we are looking at
                    //curCrowd = fanStand;
                    //looking in general direction
                    if (angle < 40 && !drewBackTire.isGrounded)
                    {
                        //set current stand we are looking at
                        curCrowd = fanStand;
                        //call cheering crowd animation
                        curCrowd.BroadcastMessage("Cheer", 0, SendMessageOptions.DontRequireReceiver);

                        fanStand.audio.PlayOneShot(cheer, 0.6f);

                        //apply turbo
                        if (bikePhysics.turboBar < bikePhysics.maxTurboBar)
                        {
                            bikePhysics.turboBar += bikePhysics.turboBoostSpeed;
                        }
                    }
                }
                Vector3 oldCrowdPos = fanStand.transform.position - transform.position;

                //looking away from stands
                //if (Vector3.Dot(transform.forward, oldCrowdPos.normalized) < 0.2f)
                if (angle > 40)
                {
                    //call idle crowd animation
                    curCrowd.BroadcastMessage("Idle", 0, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
