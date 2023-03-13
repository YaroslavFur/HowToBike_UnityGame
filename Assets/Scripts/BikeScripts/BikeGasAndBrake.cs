using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeGasAndBrake : MonoBehaviour
{
    [SerializeField] public float targetMaximumSpeed;
    [SerializeField] public float accelerationMotorForce;
    [SerializeField] public float targetBrakeSpeed;
    [SerializeField] public float brakeMotorForce;

    [SerializeField] public PedalsFollowing PedalsFollowingScript;

    [HideInInspector] public bool acceleration = false, braking = false;
    private HingeJoint backWheelJoint;


    // Start is called before the first frame update
    void Start()
    {
        backWheelJoint = GetComponent<HingeJoint>();
        backWheelJoint.useMotor = true;
    }

    // Update is called once per frame
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
    }

    void FixedUpdate()
    {
        if (backWheelJoint)
        {
            if (acceleration)
            {
                SetHingeJointMotorVelocityAndForce(backWheelJoint, targetMaximumSpeed, accelerationMotorForce);
            }
            else if (braking)
            {
                SetHingeJointMotorVelocityAndForce(backWheelJoint, targetBrakeSpeed, brakeMotorForce);
            }
            else
            {
                SetHingeJointMotorVelocityAndForce(backWheelJoint, 0, 0);
            }
        }

        PedalsFollowingScript.MakePedalsFollow();
    }

    private void SetHingeJointMotorVelocityAndForce(HingeJoint joint, float velocity, float force)
    {
        JointMotor motor = joint.motor;

        motor.targetVelocity = velocity;
        motor.force = force;

        joint.motor = motor;
    }
}
