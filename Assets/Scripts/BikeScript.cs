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
    [SerializeField] private GameObject chain;

    [SerializeField] private float targetMaximumSpeed;
    [SerializeField] private float accelerationMotorForce;
    [SerializeField] private float targetBrakeSpeed;
    [SerializeField] private float brakeMotorForce;
    [SerializeField, Range(0, 5)] private float steeringSensitivity;
    [SerializeField] private float steeringTargetSpeed;
    [SerializeField] private float steeringForce;
    private const float pedalsFollowTargetSpeed = 100000, pedalsFollowForce = 100000, backWheelFollowTargetSpeed = 1000, backWheelFollowForce = 10;

    private bool acceleration = false, braking = false, steering = false;
    private float mouseInitialSteeringPosition = 0, forkInitialSteeringAngle = 0;

    [DebugGUIGraph(group: 1, min: 0, max: 10f, r: 0, g: 1, b: 0, autoScale: true)]
    private float bikeSpeedGraph;

    [DebugGUIGraph(group: 1, min: 0, max: 10f, r: 1, g: 0, b: 0, autoScale: true)]
    private float wheelSpeedGraph;

    private Vector3 com = new Vector3(0, 0.4f, 0);

    void Start()
    {
        HingeJoint backWheelJoint = backWheel.GetComponent<HingeJoint>();
        backWheelJoint.useMotor = true;
        HingeJoint forkJoint = fork.GetComponent<HingeJoint>();
        forkJoint.useMotor = true;
        HingeJoint pedalJoint = pedals.GetComponent<HingeJoint>();
        pedalJoint.useMotor = true;

        Rigidbody forkRB = fork.GetComponent<Rigidbody>();
        forkRB.centerOfMass = new Vector3(0, 0, 0);
        Rigidbody pedalsRB = pedals.GetComponent<Rigidbody>();
        pedalsRB.centerOfMass = new Vector3(0, 0, 0);

        Rigidbody backWheelRB = backWheel.GetComponent<Rigidbody>();
        backWheelRB.maxAngularVelocity = Mathf.Infinity;
        Rigidbody frontWheelRB = frontWheel.GetComponent<Rigidbody>();
        frontWheelRB.maxAngularVelocity = Mathf.Infinity;
        forkRB.maxAngularVelocity = Mathf.Infinity;
        pedalsRB.maxAngularVelocity = Mathf.Infinity;
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

                HingeJoint forkJoint = fork.GetComponent<HingeJoint>();
                if (forkJoint)
                {
                    forkInitialSteeringAngle = Math.Clamp(fork.GetComponent<HingeJoint>().angle, -90, 90);
                }
            }
            else
            {
                steering = false;
            }
        }
    }

    public void FixedUpdate()
    {
        float targetPedalsAngle = 0;

        HingeJoint backWheelJoint = backWheel.GetComponent<HingeJoint>();

        // backWheel control    
        if (backWheelJoint)
        {
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

        HingeJoint pedalsJoint = pedals.GetComponent<HingeJoint>();

        // pedals control
        if (pedalsJoint)
        {
            JointMotor backWheelMotor = backWheelJoint.motor;

            JointMotor pedalsMotor = pedalsJoint.motor;

            float angleDifference = ClosestDifferenceBetweenAnglesOfCircle(pedalsJoint.angle, targetPedalsAngle, -180, 180);

            if (angleDifference > 0)
            {
                // formula (y=1-e^-x) - the closer x to 0, the less y is
                float followCoeficient = (float)(1 - Math.Pow(Math.E, -angleDifference / 180));

                pedalsMotor.targetVelocity = pedalsFollowTargetSpeed * followCoeficient;
                pedalsMotor.force = pedalsFollowForce * followCoeficient;

                backWheelMotor.targetVelocity += -backWheelFollowTargetSpeed * followCoeficient;
                backWheelMotor.force += backWheelFollowForce * followCoeficient;
            }
            else if (angleDifference < 0)
            {
                float followCoeficient = (float)(1 - Math.Pow(Math.E, angleDifference / 180));

                pedalsMotor.targetVelocity = -pedalsFollowTargetSpeed * followCoeficient;
                pedalsMotor.force = pedalsFollowForce * followCoeficient;

                backWheelMotor.targetVelocity += backWheelFollowTargetSpeed * followCoeficient;
                backWheelMotor.force += backWheelFollowForce * followCoeficient;
            }

            backWheelJoint.motor = backWheelMotor;

            pedalsJoint.motor = pedalsMotor;
        }

        HingeJoint forkJoint = fork.GetComponent<HingeJoint>();

        // fork control
        if (forkJoint)
        {
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

        // free chain if backWheel or pedals are broken
        if (!pedalsJoint || !backWheelJoint)
        {
            FixedJoint chainJoint = chain.GetComponent<FixedJoint>();
            if (chainJoint)
            {
                Destroy(chainJoint);
            }
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