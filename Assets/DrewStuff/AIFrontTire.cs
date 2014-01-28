using UnityEngine;
using System.Collections;

public class AIFrontTire : MonoBehaviour
{
    public Transform bike;
    public Transform backTireTrans;

    private DrewBackTire backTire;
    private BikeAI bikePhysics;

    private int layerMask = 1 << 8;

    void Awake()
    {
        bikePhysics = bike.GetComponent<BikeAI>();
        backTire = backTireTrans.GetComponent<DrewBackTire>();
    }

    void Start()
    {
        layerMask = ~layerMask;
    }

    void Update()
    {
        CrashGroundCheck();
    }

    void CrashGroundCheck()
    {
        if (Physics.Raycast(transform.position, transform.forward, 1, layerMask) && !backTire.isGrounded)
        {
            bikePhysics.hasCrashed = true;
        }
    }
}
