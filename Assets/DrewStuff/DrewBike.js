    var frame : Transform;
    var frontFork : Transform;
    var rearCarriage : Transform;
     
     
    var frontWheel : Transform;
    var rearWheel : Transform;
     
    var col : Transform;
     
    private var maxSteer=45;
    private var minSteer=5;
    private var rake = 30.0;
    private var motorPower=150;
    private var mass=500.0;
    var centerOfMass=Vector3(0,-0.25,1);
     
     
    private var frontJoint;
    private var rearJoint;
     
    private var frontWheelSize : Vector3;
    private var rearWheelSize : Vector3;
    private var stopTilt = 5.0;
    private var doScript=false;
     
    private var steerAngle=0.0;
    private var motorAmount=0.0;
    private var motorTorque=0.0;
    private var handBrake=0.0;
    private var engineBrake=0.0;
    private var engineRPM=0.0;
    private var motorMod=22.0 / 50;
    private var gear=0;
    private var relativeVelocity=Vector3.zero;
    private var veloSteer=maxSteer;
    private var lastX =0.0;
    private var lastY=0.0;
    private var lastZ=0.0;
    private var steerLean = 0.0;
    private var steerAmount=0.0;
    private var defaultForkAngle;
    private var forkSteeringPoint;
     
    function Start(){
        if(!col)return;
        if(!frontWheel)return;
        if(!rearWheel)return;
        if(!frontFork)return;
        if(!rearCarriage)return;
        if(!frontWheel.gameObject.GetComponent(MeshFilter))return;
        if(!rearWheel.gameObject.GetComponent(MeshFilter))return;
       
       
        gameObject.AddComponent(Rigidbody);
       
        var MC = col.gameObject.AddComponent(MeshCollider);
        MC.convex=true;
        MC.smoothSphereCollisions=true;
        col.renderer.enabled=false;
       
        frontWheelSize=frontWheel.gameObject.GetComponent(MeshFilter).mesh.bounds.size;
        rearWheelSize=rearWheel.gameObject.GetComponent(MeshFilter).mesh.bounds.size;
       
        forkSteeringPoint=GameObject().transform;
        forkSteeringPoint.parent=frontFork;
        forkSteeringPoint.localPosition=Vector3.zero;
        forkSteeringPoint.Rotate(Vector3(-rake, 0, 0 ));//self space
        var axis = forkSteeringPoint.TransformDirection(Vector3.up);
       
        defaultForkAngle=frontFork.localRotation;
        if(false){
            frontFork.gameObject.AddComponent(Rigidbody);
            //frontFork.rigidbody.isKinematic=true;
           
            var joint=frontFork.gameObject.AddComponent(HingeJoint);
            joint.axis=axis;
            joint.anchor=Vector3.zero;
            joint.useSpring = false;
            joint.useLimits=true;
            joint.limits.min = -5;
            joint.limits.minBounce = 0;
            joint.limits.max = 5;
            joint.limits.maxBounce = 0;
            joint.connectedBody=rigidbody;//1.740522
        }
       
        frontWheel.parent=frontFork;
        frontJoint = frontWheel.gameObject.AddComponent(WheelCollider);
        frontJoint.radius=frontWheelSize.y * transform.localScale.y / 2;
        frontJoint.suspensionDistance = 0.0;
        frontJoint.forwardFriction.stiffness = 0.02;
        frontJoint.sidewaysFriction.stiffness = 0.02;
       
        rearJoint = rearWheel.gameObject.AddComponent(WheelCollider);
        rearJoint.radius=rearWheelSize.y * transform.localScale.y / 2;
        rearJoint.suspensionDistance = 0.0;
        rearJoint.forwardFriction.stiffness = 0.02;
        rearJoint.sidewaysFriction.stiffness = 0.02;
       
        resetCenterOfMass();
       
        doScript=true;
    }
     
     
    function resetCenterOfMass(){
        rigidbody.mass=mass;
        rigidbody.drag=0.175;
        rigidbody.centerOfMass+=centerOfMass;
    }
     
    function Update () {
        if(!doScript)return;
       
        relativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        veloSteer=Mathf.Clamp(maxSteer-Mathf.Abs(relativeVelocity.z) * 0.5, minSteer, maxSteer);
       
        motorAmount = Input.GetAxis("Vertical") * motorPower * Time.deltaTime;
        steerAngle = Input.GetAxis("Horizontal") * veloSteer;
        handBrake = (Input.GetButton("Jump")?1.0:0.0) * mass;
       
        if(handBrake>0){
            rearJoint.motorTorque = 0.0;
            rearJoint.brakeTorque = handBrake;
            frontJoint.brakeTorque = handBrake * 0.25;
        }else{
            rearJoint.motorTorque = motorAmount * 100;
            rearJoint.brakeTorque = 0.0;
            frontJoint.brakeTorque = 0.0;
        }
       
        var x=transform.localEulerAngles.x;
        var y=transform.localEulerAngles.y;
        var z=transform.localEulerAngles.z;
       
        if(x>180)x=x-360;
        if(y>180)y=y-360;
        if(z>180)z=z-360;
       
       
        //frontFork.Rotate(Vector3.right);
        // steering lean
        // slow movement gets a less lean
        var rpm=(Mathf.Abs(frontJoint.rpm) + Mathf.Abs(rearJoint.rpm))/2;
        var gyroscope=Mathf.Clamp01((relativeVelocity.z) / 20);
        var gyrosteer =(1-gyroscope) * 0.9;
        steerLean=steerAngle * gyroscope;
        steerAmount=steerAngle - steerLean * gyrosteer;
       
        //print(Array(steerLean, steerAmount, relativeVelocity.z, rpm));
       
        if(!frontJoint.isGrounded && !rearJoint.isGrounded && relativeVelocity.z > 5){
            //steerLean=lastLean;
            transform.localEulerAngles.y=lastY;
            if(Input.GetKey("space"))
                rigidbody.AddRelativeTorque (Vector3.right * mass * 2);
            //else
            //  if(Input.GetAxis("Vertical")>0)
            //      rigidbody.AddRelativeTorque (-Vector3.right * mass * 2);
            //transform.localEulerAngles.x=lastX;
        }
       
        transform.localEulerAngles.z=-steerLean;// Mathf.Lerp(lastLean, -steerLean, Time.deltaTime * 8);
       
        frontFork.localRotation=defaultForkAngle;
        var axis = forkSteeringPoint.TransformDirection(Vector3.up);
        frontFork.RotateAround (frontFork.position, axis, steerAmount);
       
        x=transform.localEulerAngles.x;
        y=transform.localEulerAngles.y;
        z=transform.localEulerAngles.z;
       
        if(x>180)x=x-360;
        if(y>180)y=y-360;
        if(z>180)z=z-360;
       
        lastX=x;
        lastY=y;
        lastZ=-z;
       
        if(Input.GetKeyDown("r"))doReset();
    }
     
    function LateUpdate(){
       
    }
     
    function doReset(){
        transform.position+=Vector3(0,0.5,0);
        transform.rotation=Quaternion.Euler( 0.0, transform.localEulerAngles.y, 0.0 );
        rigidbody.velocity=Vector3.zero;
        rigidbody.angularVelocity=Vector3.zero;
    }
     
    function OnCollisionEnter(collision : Collision) {
        //print(Array("Enter Collision",collision.relativeVelocity.magnitude, collision.contacts[0].thisCollider.transform.name));
        for (var contact : ContactPoint in collision.contacts) {
            //print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            // Visualize the contact point
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }