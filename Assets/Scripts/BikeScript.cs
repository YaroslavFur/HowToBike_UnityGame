using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeScript : MonoBehaviour
{
    public GameObject frontWheel;
    public GameObject backWheel;
    public GameObject bike;
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    [DebugGUIGraph(group: 1, min: 0, max: 3, r: 0, g: 1, b: 0, autoScale: false)]
    float wheelSlip;

    [DebugGUIGraph(group: 2, min: 1, max: 1.01f, r: 1, g: 0, b: 0, autoScale: true)]
    float wheelPositionY;

    [DebugGUIGraph(group: 3, min: 0, max: 10f, r: 0, g: 0, b: 1, autoScale: true)]
    float bikeSpeedZ;

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

        double wheelSpeed = backWheel.GetComponentInChildren<WheelCollider>().radius * 2 * Math.PI * backWheel.GetComponentInChildren<WheelCollider>().rpm / 60;
        bikeSpeedZ = (float)bike.GetComponentInChildren<Rigidbody>().velocity.z;
        wheelSlip = (float)Math.Round(wheelSpeed / bikeSpeedZ, 2);
        wheelPositionY = backWheel.transform.position.y;

        Debug.Log("BackWheel:" +
            " Wheel speed: " + Math.Round(wheelSpeed, 2) + 
            " Bike speed: " + Math.Round(bikeSpeedZ, 2) + 
            " Wheel Slip: " + wheelSlip);

        DebugGUI.LogPersistent("wheelSlip", (wheelSlip).ToString("F3"));
        DebugGUI.LogPersistent("wheelPositionY", (wheelPositionY).ToString("F3"));
        DebugGUI.LogPersistent("speed", (bikeSpeedZ).ToString("F3"));
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