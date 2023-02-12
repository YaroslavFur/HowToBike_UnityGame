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
    [Range(0, 1)]
    public float wheelRotationDamper = 0.05f;
    [Range(0, 5)]
    public float RotateForceMultiplier;

    // degrees per second
    float wheelRotationSpeed = 0;

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

        if (Input.GetKey(KeyCode.DownArrow))
            motor -= maxMotorTorque;
        if (Input.GetKey(KeyCode.UpArrow))
            motor += maxMotorTorque;

        /*
        if (frontWheel.transform.rotation.z > direction)
            steering -= maxSteeringAngle;
        if (frontWheel.transform.rotation.z < direction)
            steering += maxSteeringAngle;

        if (Input.GetKey(KeyCode.LeftArrow))
            direction += 0.001f;
        if (Input.GetKey(KeyCode.RightArrow))
            direction -= 0.001f;
        */

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

        DebugGUI.LogPersistent("wheelSlip", "wheelSlip " + (wheelSlip).ToString("F3"));
        DebugGUI.LogPersistent("wheelPositionY", "wheelPositionY " + (wheelPositionY).ToString("F3"));
        DebugGUI.LogPersistent("speed", "speed " + (bikeSpeedZ).ToString("F3"));

        CalculateFrontWheelForces(frontWheel.GetComponentInChildren<WheelCollider>(), bike);
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

    public void CalculateFrontWheelForces(WheelCollider wheelCollider, GameObject bike)
    {
        float rotationForce = 0;
        float wheelRotation = wheelCollider.steerAngle;
        float wheelMass = wheelCollider.mass;
        float bikeMass = bike.GetComponent<Rigidbody>().mass;
        // float bikeVelocity = (float)bike.GetComponentInChildren<Rigidbody>().velocity.magnitude;

        // force that turns wheel aside when a bike is tilted
        // depends on: weight of wheel, bike tilt angle, wheel rotation angle
        // formula: F = m*g*cos(bike tilt angle - wheel rotation angle)
        { 
            rotationForce = (float)(
                wheelMass * 
                -Physics.gravity.y * 
                -Math.Sin((wheelCollider.transform.rotation.eulerAngles.z + wheelRotation) * (Math.PI / 180)) *
                RotateForceMultiplier);

            DebugGUI.LogPersistent("Gravity", "Gravity " + (-Physics.gravity.y).ToString("F3"));
            DebugGUI.LogPersistent("tilt", "tilt " + (wheelCollider.transform.eulerAngles.z).ToString("F3"));
            DebugGUI.LogPersistent("tilt cos", "tilt cos " + (Math.Sin(wheelCollider.transform.rotation.eulerAngles.z * (Math.PI / 180))).ToString("F3"));
            DebugGUI.LogPersistent("rotationForce1", "rotationForce1 " + (rotationForce).ToString("F3"));
        }

        // force that turns wheel to straight position when bike is moving forward
        // depends on: tilt angle, bike velocity
        // formula: 
        {

        }

        // force produced by the driver
        {

        }

        float rotationAcceleration = rotationForce / wheelMass;
        DebugGUI.LogPersistent("rotationAcceleration", "rotationAcceleration " + (rotationAcceleration).ToString("F3"));
        wheelRotationSpeed += rotationAcceleration;
        wheelRotationSpeed *= (1 - wheelRotationDamper);
        wheelCollider.steerAngle = wheelRotation + wheelRotationSpeed * Time.deltaTime;
        DebugGUI.LogPersistent("wheelRotation", "wheelRotation " + (wheelRotation).ToString("F3"));
        DebugGUI.LogPersistent("steerAngle", "steerAngle " + (wheelCollider.steerAngle).ToString("F3"));
    }
}