using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedalsFollowing : MonoBehaviour
{
    [SerializeField] private GameObject backWheel;

    private HingeJoint backWheelJoint;
    private HingeJoint pedalsJoint;

    private float pedalsCurrentAngle = 0;
    private float pedalsLastAngle = 0;
    private float pedalsVelocityReal = 0;
    private float velocityDifference = 0;
    private float followCoeficient = 0;

    private const float pedalsFollowForce = 100, backWheelFollowForce = 100;
    
    // Start is called before the first frame update
    void Start()
    {
        backWheelJoint = backWheel.GetComponent<HingeJoint>();
        pedalsJoint = GetComponent<HingeJoint>();
    }

    public void MakePedalsFollow()
    {
        if (backWheelJoint && pedalsJoint)
        {
            JointMotor backWheelMotor = backWheelJoint.motor;
            JointMotor pedalsMotor = pedalsJoint.motor;

            pedalsLastAngle = pedalsCurrentAngle;
            pedalsCurrentAngle = pedalsJoint.angle;
            pedalsVelocityReal = GeometryCalculator.ClosestDifferenceBetweenAnglesOfCircle(pedalsLastAngle, pedalsCurrentAngle, -180, 180) / Time.fixedDeltaTime;

            velocityDifference = backWheelJoint.velocity - pedalsVelocityReal;

            followCoeficient = (float)(1 - Math.Pow(Math.E, (velocityDifference < 0 ? velocityDifference : -velocityDifference) / 1000));

            pedalsMotor.targetVelocity = backWheelJoint.velocity;
            pedalsMotor.force = pedalsFollowForce * followCoeficient;

            backWheelMotor.targetVelocity += (-velocityDifference * followCoeficient);
            backWheelMotor.force += backWheelFollowForce * followCoeficient;

            backWheelJoint.motor = backWheelMotor;
            pedalsJoint.motor = pedalsMotor;

        }
    }
}
