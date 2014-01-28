using UnityEngine;

using System.Collections;
/// this is a 2d game

 

public class BikePhysics: MonoBehaviour {

    

    public Rigidbody bikesRigidbody;

    

    public Transform frontWheel;

    public Transform rearWheel;

    

    public bool onGround;

    

    public float speed;

    public float horsePower;

    public float rotationWeightFactor = 50.0f;

    private float currentSpeed;

    

    // Update is called once per frame

    void Update () {

        if(Input.GetKey(KeyCode.Space)) {

            bikesRigidbody.AddForce(Vector3.up * 100);  

        }

        

        if(Input.GetKey(KeyCode.UpArrow)) {

            frontWheel.transform.gameObject.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, -horsePower * 100 * Time.deltaTime), ForceMode.Impulse);

            rearWheel.transform.gameObject.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, -horsePower * 100 * Time.deltaTime), ForceMode.Impulse);

        }

        

        if(Input.GetKey(KeyCode.DownArrow)) {

            frontWheel.transform.gameObject.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, 0, horsePower * 100 * Time.deltaTime), ForceMode.Impulse);

            rearWheel.transform.gameObject.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, 0, horsePower * 100 * Time.deltaTime), ForceMode.Impulse);

        }

        

        if(Input.GetKey(KeyCode.LeftArrow)) {

            if(onGround) {

                bikesRigidbody.AddTorque(new Vector3(0, 0, (rotationWeightFactor * 2) * 10 * Time.deltaTime));

                bikesRigidbody.AddForceAtPosition(Vector3.up * 30, new Vector3(frontWheel.transform.position.x, rearWheel.transform.position.y - 0.5f, transform.position.z));

            } else {

                bikesRigidbody.AddTorque(new Vector3(0, 0, rotationWeightFactor * 10 * Time.deltaTime));    

            }

        } else if(Input.GetKey(KeyCode.RightArrow)) {

            if(onGround) {

                bikesRigidbody.AddTorque(new Vector3(0, 0, -(rotationWeightFactor * 2) * 10 * Time.deltaTime));

                bikesRigidbody.AddForceAtPosition(Vector3.up * 30, new Vector3(rearWheel.transform.position.x, rearWheel.transform.position.y - 0.5f, transform.position.z));

            } else {

                bikesRigidbody.AddTorque(new Vector3(0, 0, -rotationWeightFactor * 10 * Time.deltaTime));   

            }

        }

        

        onGround = isGrounded();

    

        currentSpeed = new Vector3(rearWheel.transform.gameObject.GetComponent<Rigidbody>().velocity.x, 0, rearWheel.transform.gameObject.GetComponent<Rigidbody>().velocity.z).magnitude;

    

        LimitSpeed(15.0f);

        

        RaycastHit hit;

        if(Physics.Raycast(rearWheel.transform.position, -Vector3.up, out hit, 100.0f) || Physics.Raycast(frontWheel.transform.position, -Vector3.up, out hit, 100.0f)) {

            if(hit.transform.tag == "Ground" || hit.transform.name == "Floor") {

                float distanceToGround = hit.distance;

                Vector3 pointOfHit = hit.point;

                //Debug.Log("Ray Hit Distance: " + hit.distance + "Hit Points: " + hit.point);

                onGround = true;

            } else {

                onGround = false;

            }

        }

    }

    

    void LimitVelocity (float limit) {

        Vector3 velocity = bikesRigidbody.velocity;

        if (velocity == Vector3.zero) return;

    

        float magnitude = velocity.magnitude;

        if (magnitude > limit) {

            velocity *= (limit / magnitude);

            bikesRigidbody.velocity = velocity;

        }   

    }

    

    void LimitSpeed (float limit) {

        if (currentSpeed > limit) {

            rearWheel.transform.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(rearWheel.transform.gameObject.GetComponent<Rigidbody>().velocity.x, 0, rearWheel.transform.gameObject.GetComponent<Rigidbody>().velocity.z).normalized * limit + rearWheel.transform.gameObject.GetComponent<Rigidbody>().velocity.y * Vector3.up;

        }   

    }

    

    public bool isGrounded () {

        return Physics.Raycast(transform.position, -Vector3.up, rearWheel.GetComponent<SphereCollider>().bounds.extents.y + 0.1f);

    }
}