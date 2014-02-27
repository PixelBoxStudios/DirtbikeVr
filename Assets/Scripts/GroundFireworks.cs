using UnityEngine;
using System.Collections;

public class GroundFireworks : MonoBehaviour {
	public Transform player;
	public GameObject [] fireworksChilds;
	// Use this for initialization
	void Awake()
	{
		for (int i = 0; i < fireworksChilds.Length; i++) 
		{
			fireworksChilds[i].SetActive(false);
		}
	}
	void FireFireworks(int i)
	{
		fireworksChilds[i].SetActive(true);
	}
	
	// Update is called once per frame
	void Update () 
	{
		for (int i = 0; i < fireworksChilds.Length; i++) 
		{
			float distance = Vector3.Distance(fireworksChilds[i].transform.position, player.position);
			if(distance < 18)
			{
				FireFireworks(i);
			}
			else if(distance > 30)
			{

			}
		}
	}
}
