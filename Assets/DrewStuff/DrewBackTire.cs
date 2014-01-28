using UnityEngine;
using System.Collections;

public class DrewBackTire : MonoBehaviour
{
    //[HideInInspector]
    public bool isGrounded = false;

    private int layerMask = 1 << 8;

    void Start()
    {
        layerMask = ~layerMask;
    }

    void LateUpdate()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 2, layerMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
