using UnityEngine;
using System.Collections;

public class motorcycle_physics : MonoBehaviour
{
	//added physics variables
	public Transform 	psr;
	public Transform 	handleBars;
	public Transform	frontTire;
	public Transform 	rearTire;
	private Vector3 	velocity;
	private Vector3 	acceleration;
	private Vector3 	rotation;
	private float 		inverseMass;
	//currently not going to use friction till movement is working
	//private float 		kineticFriction;
	//private float 		staticFriction;
	
	//motorcyle specific
	//private int			gear;
	
	//accumulators
	private Vector3 	forceAccum;
	private Vector3 	torqAccum;
	
	// Use this for initialization
	void Start()
	{
		velocity.Set		(0.0f, 0.0f, 0.0f);
		acceleration.Set	(0.0f, 0.0f, 0.0f);
		rotation.Set		(0.0f, 0.0f, 0.0f);
		inverseMass 		= 1.0f;
		forceAccum.Set		(0.0f, 0.0f, 0.0f);
		torqAccum.Set		(0.0f, 0.0f, 0.0f);
		//kineticFriction 	= 0.0f;
		//staticFriction 		= 0.0f;
		//gear 				= 1;
	}
	
	// Update is called once per frame
	void Update()
	{
		InputUpdate();
		PhysicsUpdate();
	}
	
	//getters and setters
	void 		SetVelocity(Vector3 input){velocity = input;}
	Vector3 	GetVelocity(){return velocity;}
	void 		SetAcceleration(Vector3 input){acceleration = input;}
	Vector3 	GetAcceleration(){return acceleration;}
	void 		SetRotation(Vector3 input){rotation = input;}
	Vector3 	GetRotation(){return rotation;}
	void 		SetMass(float input){inverseMass = 1/input;}
	float 		GetInverseMass(){return inverseMass;}
	//void 		SetKineticFriction(float input){kineticFriction = input;}
	//float 		GetKineticFriction(){return kineticFriction;}
	//void 		SetStaticFriction(float input){staticFriction = input;}
	//float 		GetStaticFriction(){return staticFriction;}
	
	//forces are added not set
	void 		Addforce(Vector3 input){forceAccum += input;}
	Vector3 	GetForceAccum(){return forceAccum;}
	void 		AddTorq(Vector3 input){torqAccum += input;}
	Vector3 	GetTorqAccum(){return torqAccum;}
	
	//clear accumulators
	void 		ClearAccum(){forceAccum.Set(0.0f, 0.0f, 0.0f); torqAccum.Set(0.0f, 0.0f, 0.0f);}
	
	//physics update
	void PhysicsUpdate()
	{
		//update linear
		Vector3 lastAccel = GetAcceleration();
		lastAccel += GetForceAccum() * GetInverseMass();
		lastAccel *= Time.deltaTime;
		SetAcceleration(lastAccel);
		SetVelocity(GetVelocity() + lastAccel);
		psr.position = psr.position += GetVelocity() * Time.deltaTime;
		
		//update angular
		Vector3 lastAngular = GetRotation();
		lastAngular += GetTorqAccum() * GetInverseMass();
		lastAngular *= Time.deltaTime;
		SetRotation(lastAngular);
		lastAngular *= Time.deltaTime;
		Quaternion temp = Quaternion.identity;
		temp.Set(lastAngular[0], lastAngular[1], lastAngular[2], 0.0f);
		temp = temp * psr.rotation;
		psr.rotation.Set(psr.rotation.x + temp.x * 0.5f, 
		                 psr.rotation.y + temp.y * 0.5f,
		                 psr.rotation.z + temp.z * 0.5f,
		                 psr.rotation.w + temp.w * 0.5f);
		ClearAccum();
	}
	
	//input update
	void InputUpdate()
	{
		//XBox controller variables
		float steering 		= Input.GetAxis("steering");
		//float throttle 		= Input.GetAxis("throttle");
		bool throttle 		= Input.GetButtonDown("throttle");
		bool  frontBrake 	= Input.GetButtonDown("front brake");
		bool  backBrake 	= Input.GetButtonDown("back brake");
		//below will be used for tricks and stunts, needs renaming and redesigning
		//float riderLean 	= Input.GetAxis("Right Analog X");
		//float riderTilt 	= Input.GetAxis("Right Analog Y");

		//Keyboard controller variables

		
		//twist handlebars to left
		if(steering < 0)
		{
			Vector3 temp = new Vector3(20.0f, 0.0f, 0.0f);
			handleBars.Rotate(temp * steering * Time.deltaTime);
		}
		//twist handlebars to right
		else if(steering > 0)
		{
			Vector3 temp = new Vector3(20.0f, 0.0f, 0.0f);
			handleBars.Rotate(temp * steering * Time.deltaTime);
		}
		//accelerate in direction of back tire
		if(throttle)
		{
			Vector3 temp = frontTire.position - rearTire.position;
			temp = temp * 100;//using 100 for acceleration for now till gearing is calculated and used to calculate forces added
			Addforce(temp);
		}
		//accelerate in direction of back tire
		if(frontBrake)
		{
			Vector3 temp = rearTire.position - frontTire.position;
			temp = temp * -50 * Time.deltaTime;//using 50 for acceleration of braking for now till
			Addforce(temp);
		}
		if(backBrake)
		{
			Vector3 temp = rearTire.position - frontTire.position;
			temp = temp * -15 * Time.deltaTime;//using 15 for acceleration of braking for now till
			Addforce(temp);
		}
	}
}
