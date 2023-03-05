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
    [SerializeField] private GameObject pedals;

    [SerializeField] private float targetMaximumSpeed;
    [SerializeField] private float accelerationMotorForce;
    [SerializeField] private float targetBrakeSpeed;
    [SerializeField] private float brakeMotorForce;
    [SerializeField, Range(0, 5)] private float steeringSensitivity;
    [SerializeField] private float steeringTargetSpeed;
    [SerializeField] private float steeringForce;
    private const float pedalsTargetSpeed = 10000, pedalsForce = 100000;

    private bool acceleration = false, braking = false, steering = false;
    private float mouseInitialSteeringPosition = 0, forkInitialSteeringAngle = 0;

    [DebugGUIGraph(group: 1, min: 0, max: 10f, r: 0, g: 1, b: 0, autoScale: true)]
    private float bikeSpeedGraph;

    void Start()
    {
        HingeJoint backWheelJoint = backWheel.GetComponent<HingeJoint>();
        backWheelJoint.useMotor = true;
        HingeJoint forkJoint = fork.GetComponent<HingeJoint>();
        forkJoint.useMotor = true;
        HingeJoint pedalJoint = pedals.GetComponent<HingeJoint>();
        pedalJoint.useMotor = true;

        Rigidbody frameRB = frame.GetComponent<Rigidbody>();
        frameRB.centerOfMass = new Vector3(0, 0, 0);
        Rigidbody forkRB = frame.GetComponent<Rigidbody>();
        forkRB.centerOfMass = new Vector3(0, 0, 0);
        Rigidbody pedalsRB = pedals.GetComponent<Rigidbody>();
        pedalsRB.centerOfMass = new Vector3(0, 0, 0);
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
        // backWheel control
        float targetPedalsAngle;
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
            targetPedalsAngle = backWheelJoint.angle;
        }

        // pedals control
        {
            HingeJoint pedalsJoint = pedals.GetComponent<HingeJoint>();
            JointMotor pedalsMotor = pedalsJoint.motor;

            float angleDifference = ClosestDifferenceBetweenAnglesOfCircle(pedalsJoint.angle, targetPedalsAngle, -180, 180);

            if (angleDifference > 0)
            {
                // formula (y=1-e^-x) - the closer x to 0, the less y is
                pedalsMotor.targetVelocity = (float)(pedalsTargetSpeed * (1 - Math.Pow(Math.E, -angleDifference / 180)));
                pedalsMotor.force = (float)(pedalsForce * (1 - Math.Pow(Math.E, -angleDifference / 180)));
            }
            else if (angleDifference < 0)
            {
                pedalsMotor.targetVelocity = (float)(-pedalsTargetSpeed * (1 - Math.Pow(Math.E, angleDifference / 180)));
                pedalsMotor.force = (float)(pedalsForce * (1 - Math.Pow(Math.E, angleDifference / 180)));
            }

            pedalsJoint.motor = pedalsMotor;
        }

        // fork control
        {
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
            }

            forkJoint.motor = forkMotor;
        }

        bikeSpeedGraph = (float)frame.transform.InverseTransformDirection(frame.GetComponent<Rigidbody>().velocity).z;
    }

    public float ClosestDifferenceBetweenAnglesOfCircle(float currentAngle, float targetAngle, float minAngleLimit, float maxAngleLimit)
    {
        float distanceToTargetAngleUp, distanceToTargetAngleDown;

        if (targetAngle > currentAngle)
        {
            distanceToTargetAngleUp = targetAngle - currentAngle;
            distanceToTargetAngleDown = (maxAngleLimit - targetAngle) + (currentAngle - minAngleLimit);
        }
        else if (targetAngle < currentAngle)
        {
            distanceToTargetAngleUp = (targetAngle - minAngleLimit) + (maxAngleLimit - currentAngle);
            distanceToTargetAngleDown = currentAngle - targetAngle;
        }
        else
        {
            distanceToTargetAngleUp = 0;
            distanceToTargetAngleDown = 0;
        }

        if (distanceToTargetAngleUp < distanceToTargetAngleDown)
            return distanceToTargetAngleUp;
        else
            return -distanceToTargetAngleDown;
    }
}