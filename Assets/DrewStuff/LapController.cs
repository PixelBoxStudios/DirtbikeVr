using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LapController : MonoBehaviour
{
    public static int lapCount = 3;
    public static bool isRaceFinished = false;

    public GUISkin mySkin;

    public AudioClip youWin;
    public AudioClip youLose;

    public int firstPlace = 0;
    public bool playedOnce = true;

    public List<GameObject> racers;

    public int rank = 0;

    void OnGUI()
    {
        GUI.skin = mySkin;

        //display ranks after player has finished the race
        if (isRaceFinished)
        {
            for (int i = 0; i < racers.Count; i++)
            {
                if (rank == 1 && racers[i].name == "moto")
                {
                    if (playedOnce)
                    {
                        audio.PlayOneShot(youWin);
                        playedOnce = false;
                    }
                }
                else if (rank > 1 && racers[i].name == "moto")
                {
                    if (playedOnce)
                    {
                        audio.PlayOneShot(youLose);
                        playedOnce = false;
                    }
                }
                GUI.Label(new Rect(250, 100 * i, 500, 500), "Racer: " + racers[i].name + " placed " + (i + 1));
            }
            if (!IsInvoking("EndRace"))
            {
                Invoke("EndRace", 5);
            }
        }
    }

    void EndRace()
    {
        Application.LoadLevel(0);
    }

    public void RecordRank(GameObject racer)
    {
        racers.Add(racer);
        rank++;
        print(racer.name + " is " + rank + " place.");
    }
}
