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
    [Range(0, 10)]
    public float RotateForceMultiplier;
    [Range(0, 10)]
    public float StraighteningForceMultiplier;
    [Range(0, 30)]
    public float DriverForceMultiplier;
    [Range(0, 15)]
    public float keepSpeed;

    // degrees per second
    float wheelRotationSpeed = 0;

    [DebugGUIGraph(group: 0, min: 0, max: 3, r: 0, g: 1, b: 0, autoScale: false)]
    float wheelSlipGraph;

    [DebugGUIGraph(group: 1, min: 0, max: 10f, r: 0, g: 0, b: 1, autoScale: true)]
    float bikeSpeedGraph;

    [DebugGUIGraph(group: 2, min: 0, max: 0f, r: 0, g: 1, b: 0, autoScale: true)]
    float tiltGraph;

    [DebugGUIGraph(group: 3, min: 0, max: 0f, r: 0, g: 1, b: 0, autoScale: true)]
    float wheelRotationGraph;

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
            if (bikeSpeedGraph < keepSpeed)
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
        bikeSpeedGraph = (float)bike.transform.InverseTransformDirection(bike.GetComponent<Rigidbody>().velocity).z;
        wheelSlipGraph = (float)Math.Round(wheelSpeed / bikeSpeedGraph, 2);

        Debug.Log("BackWheel:" +
            " Wheel speed: " + Math.Round(wheelSpeed, 2) + 
            " Bike speed: " + Math.Round(bikeSpeedGraph, 2) + 
            " Wheel Slip: " + wheelSlipGraph);

        DebugGUI.LogPersistent("wheelSlip", "wheelSlip " + (wheelSlipGraph).ToString("F3"));
        DebugGUI.LogPersistent("speed", "speed " + (bikeSpeedGraph).ToString("F3"));

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
        Rigidbody bikeRB = bike.GetComponent<Rigidbody>();
        float bikeVelocity = (float)bikeRB.velocity.magnitude;

        DebugGUI.LogPersistent("tilt", "tilt " + (wheelCollider.transform.eulerAngles.z).ToString("F3"));

        // force that turns wheel aside when a bike is tilted
        // depends on: weight of wheel, bike tilt angle, wheel rotation angle
        // formula: F = m*g*sin(bike tilt angle - wheel rotation angle)
        { 
            rotationForce += (float)(
                wheelMass * 
                -Physics.gravity.y * 
                -Math.Sin((wheelCollider.transform.rotation.eulerAngles.z * 2 + wheelRotation) * (Math.PI / 180)) *
                RotateForceMultiplier);

            DebugGUI.LogPersistent("rotationForce", "rotationForce " + (rotationForce).ToString("F3"));
        }

        // force that turns wheel to straight position when bike is moving forward
        // depends on: rotation angle, bike velocity
        // formula: F = m*g*sin(wheel rotation angle)*speed
        {
            float straighteningForce = (float)(
                wheelMass *
                -Physics.gravity.y *
                -Math.Sin((wheelRotation) * (Math.PI / 180)) *
                bikeVelocity *
                StraighteningForceMultiplier);

            DebugGUI.LogPersistent("SinOfWheelRotation", "SinOfWheelRotation " + (Math.Sin((wheelRotation) * (Math.PI / 180))).ToString("F3"));
            DebugGUI.LogPersistent("bikeVelocity", "bikeVelocity " + (bikeVelocity).ToString("F3"));

            DebugGUI.LogPersistent("straighteningForce", "straighteningForce " + (straighteningForce).ToString("F3"));

            rotationForce += straighteningForce;
        }

        // force produced by the driver
        {
            float driverForce = 0;
            if (Input.GetKey(KeyCode.LeftArrow))
                driverForce = -1;
            if (Input.GetKey(KeyCode.RightArrow))
                driverForce = 1;
            driverForce *= DriverForceMultiplier;

            DebugGUI.LogPersistent("driverForce", "driverForce " + (driverForce).ToString("F3"));
            rotationForce += driverForce;
        }
        
        DebugGUI.LogPersistent("finalRotationForce", "finalRotationForce " + (rotationForce).ToString("F3"));
        float rotationAcceleration = rotationForce / wheelMass;
        DebugGUI.LogPersistent("rotationAcceleration", "rotationAcceleration " + (rotationAcceleration).ToString("F3"));
        wheelRotationSpeed += rotationAcceleration;
        wheelRotationSpeed *= (1 - wheelRotationDamper);
        
        wheelCollider.steerAngle = wheelRotation + wheelRotationSpeed * Time.deltaTime;
        DebugGUI.LogPersistent("wheelRotation", "wheelRotation " + (wheelRotation).ToString("F3"));
        DebugGUI.LogPersistent("steerAngle", "steerAngle " + (wheelCollider.steerAngle).ToString("F3"));

        if (wheelCollider.transform.eulerAngles.z > 180)
            tiltGraph = wheelCollider.transform.eulerAngles.z - 360;
        else
            tiltGraph = wheelCollider.transform.eulerAngles.z;

        wheelRotationGraph = wheelCollider.steerAngle;
    }
}