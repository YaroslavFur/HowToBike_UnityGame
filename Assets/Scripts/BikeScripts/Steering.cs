using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{
    [SerializeField, Range(0, 5)] public float steeringSensitivity;
    [SerializeField] public float steeringTargetSpeed;
    [SerializeField] public float steeringForce;

    HingeJoint forkJoint;

    private bool steering = false;
    private float mouseInitialSteeringPosition = 0, forkInitialSteeringAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        forkJoint = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        // true in only the frame when LMB *starts* being pressed
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (steering == false)
            {
                steering = true;

                mouseInitialSteeringPosition = Input.mousePosition.x / Screen.width;

                if (forkJoint)
                {
                    forkInitialSteeringAngle = Math.Clamp(forkJoint.angle, -90, 90);
                }
            }
            else
            {
                steering = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (forkJoint)
        {
            JointMotor forkMotor = forkJoint.motor;

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
    }
}
