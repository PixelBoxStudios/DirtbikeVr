using UnityEngine;
using System.Collections.Generic;

public class LevelScripts : MonoBehaviour 
{

	public AudioClip[] clipHolder;
	public GameObject[] lightHolder;
	public GameObject[] gateHolder;
	
	public int currentPosition;
	public int totalLaps = 5;
	public int lapsLeft = 0;
	public int currentLap = 0;
	public static bool  isGreen = false;
	
	private float lightTimer;
	private int lightCount = 0;
	private float waitTimer = 0.0f;
	
	public int clipCount = 0;
	private bool  playOnce = false;
	private float timeLength = 3.0f;
	
	private float currTimer = 0.0f;
	private float prevTimer = 0.0f;
	public float showTimer = 0.0f;

	void Start () 
	{
	  timeLength += Time.time;
	}
	
	// Update is called once per frame
	void Update () 
	{
	 
		if(Time.time > timeLength)
		{			
			if(clipCount == 0)
			{
				timeLength = Time.time + clipHolder[clipCount].length;
				audio.PlayOneShot(clipHolder[clipCount]);
				clipCount++;
				timeLength = Time.time + clipHolder[clipCount].length;
			}
		
		
			else if(clipCount == 1)
			{
				if(!playOnce){
				audio.PlayOneShot(clipHolder[clipCount]);
				playOnce = true;
				}
				if(!IsInvoking("LightActivation"))
				{
					Invoke("LightActivation", waitTimer);
					
				}	
				
		
			}
			else
			{
				StartTimer();
                //print (showTimer);
			//	StartLapCounter();
			}
		}
	}

	
	void LightActivation()
	{
			waitTimer = 0.72f;
			if(lightCount <= 3)
			{
			lightHolder[lightCount].SetActive(true);
			
			}
		else
		{
			clipCount++;
			audio.PlayOneShot(clipHolder[clipCount]);
			for( int i = 0; i < 1; i++)
			{
				gateHolder[i].transform.localRotation = new Quaternion(90.0f,3.0f,0.0f,0.0f);
			}
			isGreen = true;
		}
			lightCount++;
	}
	
	void StartTimer()
	{
		if (currTimer == 0.0f && prevTimer == 0)
		{
			currTimer = Time.time;
			prevTimer = Time.time;
		}
		else
		{
			currTimer = Time.time;	
		}
		showTimer += currTimer - prevTimer;
		prevTimer = currTimer;
	}
	
}