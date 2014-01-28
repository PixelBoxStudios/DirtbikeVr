using UnityEngine;
using System.Collections;

public class flythroughScript : MonoBehaviour {
	
	public bool firstpass = true;
	public float waitTimer = 20.0f;
	public GameObject thisCamera;
	public GameObject theRoot;
	public GameObject theBike;
	// Use this for initialization
	void Start () {
	
		
	}
	
	// Update is called once per frame
	void Update () {
	
	if(!IsInvoking("Flythrough"))
				{
					Invoke("Flythrough", waitTimer);
					
				}
    if (Input.GetButtonDown("Trick2"))
    {
        Flythrough();
    }
	}


void Flythrough()
{
	theBike.gameObject.SetActive(true);
	thisCamera.gameObject.SetActive(false);

}
}