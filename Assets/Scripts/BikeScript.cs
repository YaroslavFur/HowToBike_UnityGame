using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeScript : MonoBehaviour
{
    public GameObject frontWheel;
    public GameObject backWheel;
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FixedUpdate()
    {
        float motor = 0;
        float steering = 0;

        if (Input.GetKey(KeyCode.DownArrow))
            motor -= maxMotorTorque;
        if (Input.GetKey(KeyCode.UpArrow))
            motor += maxMotorTorque;

        if (frontWheel.transform.rotation.z > 0)
            steering -= maxSteeringAngle;
        if (frontWheel.transform.rotation.z < 0)
            steering += maxSteeringAngle;

        if (Input.GetKey(KeyCode.LeftArrow))
            steering -= maxSteeringAngle;
        if (Input.GetKey(KeyCode.RightArrow))
            steering += maxSteeringAngle;

        Debug.Log("FrontWheel Roatation: Z = " + frontWheel.transform.rotation.z + " Steering angle = " + steering);

        frontWheel.GetComponentInChildren<WheelCollider>().steerAngle = steering;
        backWheel.GetComponentInChildren<WheelCollider>().motorTorque = motor;

        ApplyLocalPositionToVisuals(frontWheel.GetComponentInChildren<WheelCollider>(), frontWheel.transform.Find("Wheel Mesh"));
        ApplyLocalPositionToVisuals(backWheel.GetComponentInChildren<WheelCollider>(), backWheel.transform.Find("Wheel Mesh"));
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider wheelCollider, Transform wheelMesh)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);

        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = rotation;
    }
}