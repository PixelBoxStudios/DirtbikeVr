using UnityEngine;
using System.Collections;

public class DrewFrontTire : MonoBehaviour
{
    public Transform bike;
    public Transform backTireTrans;

    private DrewBackTire backTire;
    private DrewBikePhysics bikePhysics;

    private int layerMask = 1 << 8;

	void Awake ()
    {
        bikePhysics = bike.GetComponent<DrewBikePhysics>();
	}

    void Start()
    {
        layerMask = ~layerMask;
    }
}
