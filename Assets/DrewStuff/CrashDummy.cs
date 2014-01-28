using UnityEngine;
using System.Collections;

public class CrashDummy : MonoBehaviour
{
    public Transform bike;

    private DrewBikePhysics bikePhysics;

    void Awake()
    {
        bikePhysics = bike.GetComponent<DrewBikePhysics>();
    }

    void Update()
    {
        CrashGroundCheck();
    }

    void CrashGroundCheck()
    {
        if (Physics.Raycast(transform.position, transform.up, 0.5f))
        {
            bikePhysics.hasCrashed = true;
        }
    }
}
