using UnityEngine;
using System.Collections;

public class checkpointTimer : MonoBehaviour
{
    private lapTimer timer;

    //checkpoint Timer Variables
    float currentTime = 0.0f;
    float lastTime = 0.0f;
    float diffrenceTime = 0.0f;
    float currentLapTime = 0.0f;


    // Use this for initialization
    void Start()
    {
        //timer = lapTimer.GetComponent<lapTimer>();
    }

    // Update is called once per frame
    void Update()
    {
        //update the lap timers time with this one
        currentLapTime = timer.currentLapTime;
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "player")
        {
            //replace the lap before with the last time
            currentTime = lastTime;
            //save current time over last lap
            currentTime = currentLapTime;
            //calculate the diffrence
            diffrenceTime = currentTime - lastTime;
        }
    }
}