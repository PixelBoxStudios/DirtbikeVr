var frontWheelCollider : WheelCollider;
var backWheelCollider : WheelCollider;
var level: Terrain;
var frontWheelTransform : Transform;
var backWheelTransform : Transform;
 
var motorPower : float = 50;
var maxSteerAngle : float = 10;
var centerOfMass : Vector3;
var bike : DirtBike; 
private var backWheelRotation : float = 0;
private var frontWheelRotation : float = 0;
private var terrainNormal: Vector3;
 
function Start() {
	bike = this;
    rigidbody.centerOfMass = centerOfMass;  
}
 
function UpdateWheelHeight(wheelTransform : Transform, collider : WheelCollider) {
    var localPosition : Vector3 = wheelTransform.localPosition;
    var hit : WheelHit;
 	
    // see if we have contact with ground   
    if (collider.GetGroundHit(hit)) {
        // calculate the distance between current wheel position and hit point, correct
        // wheel position
        localPosition.y -= Vector3.Dot(wheelTransform.position - hit.point, transform.up) - (collider.radius/2);
    	 }
    else {
        // no contact with ground, just extend wheel position with suspension distance
        localPosition = -Vector3.up * (collider.suspensionDistance + (collider.radius/2));
    }
    // actually update the position

    wheelTransform.localPosition.y = localPosition.y;
}
 
function Update() {
	
    var deltaTime : float = Time.deltaTime;
    var accel = Input.GetAxis("Vertical");
    var steer = Input.GetAxis("Horizontal") * maxSteerAngle;
 
    // set power
    backWheelCollider.motorTorque = accel * motorPower;
 
    // set steering
    frontWheelCollider.steerAngle = steer;
 
    // calculate the rotation of the wheels
    //frontWheelRotation = Mathf.Repeat(frontWheelRotation + deltaTime * frontWheelCollider.rpm * 360.0 / 60.0, 360.0);
    backWheelRotation = Mathf.Repeat(backWheelRotation + deltaTime * backWheelCollider.rpm * 360.0 / 60.0, 360.0);
 
    // set the rotation of the wheels
    frontWheelTransform.localRotation = Quaternion.Euler(frontWheelRotation, steer, 0.0);
    backWheelTransform.localRotation = Quaternion.Euler(backWheelRotation, 0, 0.0);
 
    // now some more difficult stuff: suspension, we have to manually move the wheels up and down
    // depending on the point of impact. As we have to do it twice (two wheels) we put it in a separate function
    UpdateWheelHeight(frontWheelTransform, frontWheelCollider);
    UpdateWheelHeight(backWheelTransform, backWheelCollider);
	var cross = Vector3.Cross(bike.frontWheelCollider.transform.position.normalized, level.transform.position.normalized);
        bike.transform.Rotate(cross);

	 
   
}