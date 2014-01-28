using UnityEngine;
using System.Collections;

public class DrewBikePhysics : MonoBehaviour
{
    public GUISkin mySkin;

    public float speed = 10.0f;
    public float brakeForce = 40.0f;
    public float maxSpeed = 20.0f;
    public float steerSpeed = 10.0f;
    public float steerAngle = 30.0f;
    public float deccelSpeed = 3.0f;
    public float flipSpeed = 5.0f;

    public float flipAngle = 20.0f;
    //public float wheelieAngle = 100.0f;

    //[HideInInspector]
    public float turboBar = 0.0f;
    public float turboBoostSpeed = 12.0f;
    public float maxTurboBar = 30.0f;
    public float turboSpeed = 40.0f;
    public float turboDepletionSpeed = 5.0f;

    public int curLap = 0;

    public Transform backTire;
	public Transform frontTire;
    public Transform bikeBody;
    public Transform lapController;

    [HideInInspector]
    public bool hasCrashed = false;

    [HideInInspector]
    public float accelFactor = 0.0f;
    private float curMaxSpeed = 0.0f;
    public float distFromGround = 5.0f;

    private Vector3 rotDir;
    private Vector3 moveDir;
    private Vector3 lastPos;

    private Quaternion initRot;

    private DrewBackTire drewBackTire;
    private CheckPoints checkPoints;
    private LapController lapCounter;

    void Awake()
    {
        drewBackTire = backTire.GetComponent<DrewBackTire>();
        checkPoints = GetComponent<CheckPoints>();
        lapCounter = lapController.GetComponent<LapController>();
    }

    void Start()
    {
        initRot = bikeBody.rotation;

        curMaxSpeed = maxSpeed;
    }

	void Update ()
    {
        Movement();
        Turbo();
        HeightControl();
	}

    void HeightControl()
    {
        RaycastHit hitInfo;

        //apply slow mo
        if (!Physics.Raycast(transform.position, Vector3.down, out hitInfo, distFromGround) && LevelScripts.isGreen)
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0.3f;
            }
        }
            //reset to normal speed
        else
        {
            if (Time.timeScale != 1)
            {
                Time.timeScale = 1;
            }
        }
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

    }

    void Movement()
    {
        //get inputs
        float forwardInput = Input.GetAxis("Vertical");
        float gasInput = Input.GetAxis("RightTrigger");
        float steer = Input.GetAxis("Horizontal");
        float flipInput = Input.GetAxis("RightAnalog");
        float brakeInput = Input.GetAxis("LeftTrigger");

        GearShifts(gasInput, forwardInput);

        if (drewBackTire.isGrounded)
        {
            //reset gravity
            Physics.gravity = new Vector3(0, -9.81f, 0);

            if (gasInput == 0 && forwardInput == 0)
            {
                //slow to a stop with no input
                accelFactor = Mathf.MoveTowards(accelFactor, 0, deccelSpeed * Time.deltaTime);
            }

            if (!hasCrashed && LevelScripts.isGreen)
            {
                //turn the bike
                rotDir.y += steer * steerSpeed * Time.deltaTime;

                if (gasInput != 0)
                {
                    //accelerate
                    accelFactor = Mathf.MoveTowards(accelFactor, curMaxSpeed, gasInput * speed * Time.deltaTime);
                }
                if (brakeInput != 0)
                {
                    //brake
                    accelFactor = Mathf.MoveTowards(accelFactor, 0, brakeInput * brakeForce * Time.deltaTime);   
                }
                if (forwardInput > 0)
                {
                    //accelerate
                    accelFactor = Mathf.MoveTowards(accelFactor, curMaxSpeed, forwardInput * speed * Time.deltaTime);
                }
                if (forwardInput < 0)
                {
                    //brake
                    accelFactor = Mathf.MoveTowards(accelFactor, 0, forwardInput * -brakeForce * Time.deltaTime);   
                }
            }
        }
        else
        {
            if (!hasCrashed && flipInput > 0)
            {
                //rotate bike for a flip
                bikeBody.Rotate(Vector3.right * -flipSpeed);
            }
            if (Input.GetKey(KeyCode.DownArrow) && !hasCrashed)
            {
                bikeBody.Rotate(Vector3.right * -flipSpeed);
            }
            //make gravity stronger
            Physics.gravity = new Vector3(0, -24, 0);
        }

        //if crashed stop moving and reset rotation and reposition at the last checkpoint
        if (hasCrashed)
        {
            accelFactor = 0;

            if (!IsInvoking("Respawn"))
            {
                Invoke("Respawn", 2.0f);
            }
        }
        else
        {
            if ((flipInput == 0 || Input.GetKeyUp(KeyCode.DownArrow)))
            {
                //reset bike's rotation
                bikeBody.localRotation = Quaternion.RotateTowards(bikeBody.localRotation, initRot, flipSpeed);
            }
        }
        //set max and min speeds
        moveDir.z = Mathf.Clamp(moveDir.z, 0, accelFactor);
        //apply speed values
        moveDir = new Vector3(0, rigidbody.velocity.y, accelFactor);
        //bank the bike
        rotDir = new Vector3(rotDir.x, rotDir.y, Mathf.LerpAngle(transform.eulerAngles.z, -steerAngle * steer, 0.3f));
        //limit the x angle
        rotDir.x = ClampAngle(transform.eulerAngles.x, flipAngle);

        //apply the rotation values
        transform.eulerAngles = rotDir;

        //move
        //if (!LevelScripts.isGreen)
        //{
            transform.Translate(moveDir * Time.deltaTime);
        //}
    }

    void Respawn()
    {
        rotDir = checkPoints.currentCheckpoint.eulerAngles;
        transform.position = checkPoints.currentCheckpoint.position;

        bikeBody.localRotation = initRot;
        
        hasCrashed = false;
    }

    void Turbo()
    {
        //activate turbo
        if ((Input.GetButton("Boost") || Input.GetButton("Jump")) && turboBar > 0)
        {
            //deplete turbo
            turboBar -= turboDepletionSpeed * Time.deltaTime;
            //set turbo speed
            curMaxSpeed = turboSpeed;
            audio.pitch = 3;
        }
        else
        {
            //reset speed
            curMaxSpeed = maxSpeed;
        }
        if (turboBar < 0)
        {
            turboBar = 0;
        }
        if (turboBar > maxTurboBar)
        {
            turboBar = maxTurboBar;
        }
    }

    void GearShifts(float padInput, float keyboardInput)
    {
        if (!Input.GetButton("Boost") || !Input.GetButton("Jump"))
        {
            //rev engine sound
            if (padInput != 0)
            {
                audio.pitch += padInput * 3 * Time.deltaTime;
            }
            else if (keyboardInput != 0)
            {
                audio.pitch += keyboardInput * 3 * Time.deltaTime;
            }
            //cap off the pitch
            if (audio.pitch < 0.5f)
            {
                audio.pitch = 0.5f;
            }
            if (audio.pitch > 1.5f)
            {
                audio.pitch = 1.5f;
            }
        }
        //reset the pitch
        if (padInput == 0 && keyboardInput == 0)
        {
            audio.pitch = 0.5f;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Barrier")
        {
            accelFactor = 0;
            Respawn();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Checkpoint" && checkPoints.curCheckpointPos == checkPoints.allCheckpoints.Count - 1)
        {
            //count for next lap
            curLap++;
            //record ranking position
            if (curLap == LapController.lapCount)
            {
                lapCounter.RecordRank(transform.gameObject);
                LapController.isRaceFinished = true;
            }
        }
    }

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (!LapController.isRaceFinished)
        {
            GUI.Label(new Rect(120, 100, 500, 100), "Lap " + (curLap + 1) + "/" + LapController.lapCount);
            GUI.Label(new Rect(250, 100, 500, 100), "Turbo: " + turboBar.ToString("f0") + "/" + maxTurboBar);
        }
    }

    //limit the angle
    float ClampAngle(float angle, float limit)
    {
        if (angle > 180)
        {
            float angleB = 360 - angle;

            if (angleB > limit)
            {
                angle = 360 - limit;
            }
            else
            {
                return angle;
            }
        }
        if (angle < 180)
        {
            if (angle > limit && angle < 360 - limit)
            {
                angle = limit;
            }
            else
            {
                return angle;
            }
        }

        return angle;
    }
}