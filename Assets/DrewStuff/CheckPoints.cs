using UnityEngine;
using System.Collections.Generic;

public class CheckPoints : MonoBehaviour
{
    public Transform currentCheckpoint;

    public Transform checkpoint;

    public List<GameObject> allCheckpoints;
    public int curCheckpointPos;

    void Start()
    {
        allCheckpoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("CheckpointPos"));

        allCheckpoints.Sort(delegate(GameObject a1, GameObject a2) { return a1.name.CompareTo(a2.name); });
    }

    void Update()
    {
        if (curCheckpointPos == allCheckpoints.Count)
        {
            curCheckpointPos = 0;
        }
        checkpoint.position = allCheckpoints[curCheckpointPos].transform.position;
        checkpoint.rotation = allCheckpoints[curCheckpointPos].transform.rotation;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Checkpoint")
        {
            if (curCheckpointPos <= allCheckpoints.Count - 1)
            {
                curCheckpointPos++;
                
                currentCheckpoint = allCheckpoints[curCheckpointPos - 1].transform;
            }
        }
    }
}
