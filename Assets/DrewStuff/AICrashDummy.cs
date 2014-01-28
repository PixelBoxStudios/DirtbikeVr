using UnityEngine;
using System.Collections;

public class AICrashDummy : MonoBehaviour
{
    public Transform bike;

    private BikeAI bikePhysics;

    void Awake()
    {
        bikePhysics = bike.GetComponent<BikeAI>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.name == "LevelTerrain")
        {
            bikePhysics.hasCrashed = true;
        }
    }
}
