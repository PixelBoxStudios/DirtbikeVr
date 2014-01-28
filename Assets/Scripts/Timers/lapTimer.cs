using UnityEngine;
using System.Collections;

public class lapTimer : MonoBehaviour
{
	public static lapTimer Instance;
    //Lap Timer Variables
    float fastestTime = 0.0f;
    float totalTime = 0.0f;
    public float currentLapTime = 0.0f;
    float lastLapTime = 0.0f;

    //Lap variables
    int currentLap = 0;
    int totalLap = 0;

    //start variable
    bool raceGoing = false;
    bool raceStart = false;

    // Use this for initialization
    void Start()
    {
		Instance = this;

        //read total laps
        //read fastest lap
    }

    // Update is called once per frame
    void Update()
    {
        //check if race started
        if (raceStart == true)
        {
            currentLapTime = 0.0f;
            raceGoing = true;
        }
        //if race is going time it
        if (raceGoing == true)
        {
			currentLapTime += Time.deltaTime;
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        //if the player collides with the finish line trigger
        if (other.gameObject.tag == "player")
        {
                if (currentLap == totalLap && raceGoing == true)
                {
                    totalTime += currentLapTime;
                    if (totalTime > fastestTime)
                    {
                        //replace the fastest time with current  
                   
                        //stop the race
                        raceGoing = false;
                    }
                    else
                    {
                        //stop the race
                        raceGoing = false;
                    }
                }
                if (currentLap != totalLap && raceGoing == true)
                {
                    //add the time to total and save the lap time
                    totalTime += currentLapTime;
                    lastLapTime = currentLapTime;
                    //reset the lap time
                    currentLapTime = 0.0f;
					currentLap += 1;
                }
        }
    }
}