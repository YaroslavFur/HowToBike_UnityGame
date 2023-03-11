using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedalsFollowing : MonoBehaviour
{
    [SerializeField] private GameObject backWheel;

    private HingeJoint backWheelJoint;
    private HingeJoint pedalsJoint;
    private float angleDifference;

    private const float pedalsFollowTargetSpeed = 100000, pedalsFollowForce = 100000, backWheelFollowTargetSpeed = 1000, backWheelFollowForce = 10;

    // Start is called before the first frame update
    void Start()
    {
        backWheelJoint = backWheel.GetComponent<HingeJoint>();
        pedalsJoint = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (backWheelJoint && pedalsJoint)
        {
            JointMotor backWheelMotor = backWheelJoint.motor;
            JointMotor pedalsMotor = pedalsJoint.motor;

            angleDifference = GeometryCalculator.ClosestDifferenceBetweenAnglesOfCircle(pedalsJoint.angle, backWheelJoint.angle, -180, 180);

            if (angleDifference > 0)
            {
                // formula (y=1-e^-x) - the closer x to 0, the less y is
                float followCoeficient = (float)(1 - Math.Pow(Math.E, -angleDifference / 180));

                pedalsMotor.targetVelocity = pedalsFollowTargetSpeed * followCoeficient;
                pedalsMotor.force = pedalsFollowForce * followCoeficient;

                backWheelMotor.targetVelocity = backWheelJoint.velocity + (-backWheelFollowTargetSpeed * followCoeficient);
                if (backWheelMotor.force == 0)
                    backWheelMotor.force = backWheelFollowForce * followCoeficient;
            }
            else if (angleDifference < 0)
            {
                float followCoeficient = (float)(1 - Math.Pow(Math.E, angleDifference / 180));

                pedalsMotor.targetVelocity = -pedalsFollowTargetSpeed * followCoeficient;
                pedalsMotor.force = pedalsFollowForce * followCoeficient;

                backWheelMotor.targetVelocity = backWheelJoint.velocity + (backWheelFollowTargetSpeed * followCoeficient);
                if (backWheelMotor.force == 0)
                    backWheelMotor.force = backWheelFollowForce * followCoeficient;
            }

            backWheelJoint.motor = backWheelMotor;
            pedalsJoint.motor = pedalsMotor;
        }
    }
}
