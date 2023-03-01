using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BikeScript : MonoBehaviour
{
    [SerializeField] private GameObject frontWheel;
    [SerializeField] private GameObject backWheel;
    [SerializeField] private GameObject fork;
    [SerializeField] private GameObject frame;

    [SerializeField] private float targetMaximumSpeed;
    [SerializeField] private float accelerationMotorForce;
    [SerializeField] private float targetBrakeSpeed;
    [SerializeField] private float brakeMotorForce;
    [SerializeField, Range(0, 5)] private float steeringSensitivity;
    [SerializeField] private float steeringTargetSpeed;
    [SerializeField] private float steeringForce;

    private bool acceleration = false, braking = false, steering = false;
    private float mouseInitialSteeringPosition = 0, forkInitialSteeringAngle = 0;

    [DebugGUIGraph(group: 0, min: 0, max: 3, r: 0, g: 1, b: 0, autoScale: false)]
    private float wheelSlipGraph;

    [DebugGUIGraph(group: 1, min: 0, max: 10f, r: 0, g: 1, b: 0, autoScale: true)]
    private float bikeSpeedGraph;

    [DebugGUIGraph(group: 2, min: 0, max: 0f, r: 0, g: 1, b: 0, autoScale: true)]
    private float forkCurrentAngle;

    [DebugGUIGraph(group: 2, min: 0, max: 0f, r: 1, g: 0, b: 0, autoScale: true)]
    private float forkTargetAngle;

    void Start()
    {
        HingeJoint backWheelJoint = backWheel.GetComponent<HingeJoint>();
        backWheelJoint.useMotor = true;
        HingeJoint forkJoint = fork.GetComponent<HingeJoint>();
        forkJoint.useMotor = true;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            acceleration = true;
        else
            acceleration = false;

        if (Input.GetKey(KeyCode.DownArrow))
            braking = true;
        else
            braking = false;

        // true in only the frame when LMB *starts* being pressed
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (steering == false)
            {
                steering = true;

                mouseInitialSteeringPosition = Input.mousePosition.x / Screen.width;
                forkInitialSteeringAngle = Math.Clamp(fork.GetComponent<HingeJoint>().angle, -90, 90);

                DebugGUI.LogPersistent("mousePivotSteeringPosition", "mousePivotSteeringPosition " + (mouseInitialSteeringPosition).ToString("F3"));
                DebugGUI.LogPersistent("forkPivotSteeringPosition", "forkPivotSteeringPosition " + (forkInitialSteeringAngle).ToString("F3"));
            }
            else
            {
                steering = false;
            }
        }
    }

    public void FixedUpdate()
    {
        HingeJoint backWheelJoint = backWheel.GetComponent<HingeJoint>();
        JointMotor backWheelMotor = backWheelJoint.motor;

        if (acceleration)
        {
            backWheelMotor.targetVelocity = targetMaximumSpeed;
            backWheelMotor.force = accelerationMotorForce;
        }
        else if (braking)
        {
            backWheelMotor.targetVelocity = targetBrakeSpeed;
            backWheelMotor.force = brakeMotorForce;
        }
        else
        {
            backWheelMotor.targetVelocity = 0;
            backWheelMotor.force = 0;
        }

        backWheelJoint.motor = backWheelMotor;

        HingeJoint forkJoint = fork.GetComponent<HingeJoint>();
        JointMotor forkMotor = forkJoint.motor;

        forkMotor.targetVelocity = 0;
        forkMotor.force = 0;
        if (steering)
        {

            float mouseXposition = Input.mousePosition.x / Screen.width;
            float mousePositionDifference = mouseXposition - mouseInitialSteeringPosition;
            float targetForkAngle = Math.Clamp(forkInitialSteeringAngle + (mousePositionDifference * 180 * steeringSensitivity), -90, 90);
            float angleDifference = targetForkAngle - forkJoint.angle;

            Debug.Log("1 targetAngle: " + targetForkAngle + " CurrentAngle: " + forkJoint.angle + " AngleDifference: " + angleDifference + " ForkCurrentVelocity: " + forkJoint.velocity + " ForkTargetVelocity: " + forkMotor.targetVelocity + " ForkForce: " + forkMotor.force);

            if (angleDifference > 0)
            {
                // formula (y=1-e^-x) - the closer x to 0, the less y is
                forkMotor.targetVelocity = (float)(steeringTargetSpeed * (1 - Math.Pow(Math.E, -angleDifference / 5)));
                forkMotor.force = (float)(steeringForce * (1 - Math.Pow(Math.E, -angleDifference / 5)));
            }
            else if (angleDifference < 0)
            {
                forkMotor.targetVelocity = (float)(-steeringTargetSpeed * (1 - Math.Pow(Math.E, angleDifference / 5)));
                forkMotor.force = (float)(steeringForce * (1 - Math.Pow(Math.E, angleDifference / 5)));
            }
            Debug.Log("2 targetAngle: " + targetForkAngle + " CurrentAngle: " + forkJoint.angle + " AngleDifference: " + angleDifference + " ForkCurrentVelocity: " + forkJoint.velocity + " ForkTargetVelocity: " + forkMotor.targetVelocity + " ForkForce: " + forkMotor.force);

            forkTargetAngle = targetForkAngle;
            forkCurrentAngle = forkJoint.angle;

        }
        forkJoint.motor = forkMotor;

        DebugGUI.LogPersistent("wheelTorqueTarget", "wheelTorqueTarget " + (backWheelMotor.force).ToString("F3"));
        DebugGUI.LogPersistent("wheelTorqueTarget", "wheelTorqueTarget " + (backWheelMotor.force).ToString("F3"));
        DebugGUI.LogPersistent("wheelTorque", "wheelTorque " + (backWheel.GetComponentInChildren<HingeJoint>().motor.force).ToString("F3"));

        double wheelSpeed = /*radius*/ 1 * 2 * Math.PI * (backWheelJoint.velocity / 360.0f);
        bikeSpeedGraph = (float)frame.transform.InverseTransformDirection(frame.GetComponent<Rigidbody>().velocity).z;
        wheelSlipGraph = (float)Math.Round(wheelSpeed / bikeSpeedGraph, 2);

        DebugGUI.LogPersistent("wheelSpeed", "wheelSpeed " + (wheelSpeed).ToString("F3"));
        DebugGUI.LogPersistent("speed", "speed " + (bikeSpeedGraph).ToString("F3"));
        DebugGUI.LogPersistent("wheelSlip", "wheelSlip " + (wheelSlipGraph).ToString("F3"));
    }
}