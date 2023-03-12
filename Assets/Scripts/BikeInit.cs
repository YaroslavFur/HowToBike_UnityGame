using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BikeInit : MonoBehaviour
{
    [SerializeField] private GameObject frontWheel;
    [SerializeField] private GameObject backWheel;
    [SerializeField] private GameObject fork;
    [SerializeField] private GameObject frame;
    [SerializeField] private GameObject pedals;
    [SerializeField] private GameObject leftPedal;
    [SerializeField] private GameObject rightPedal;

    void Awake()
    {
        HingeJoint backWheelJoint = backWheel.GetComponent<HingeJoint>();
        backWheelJoint.useMotor = true;

        HingeJoint forkJoint = fork.GetComponent<HingeJoint>();
        forkJoint.useMotor = true;

        Rigidbody forkRB = fork.GetComponent<Rigidbody>();
        forkRB.centerOfMass = new Vector3(0, 0, 0);
        forkRB.maxAngularVelocity = Mathf.Infinity;

        Rigidbody backWheelRB = backWheel.GetComponent<Rigidbody>();
        backWheelRB.maxAngularVelocity = Mathf.Infinity;

        Rigidbody frontWheelRB = frontWheel.GetComponent<Rigidbody>();
        frontWheelRB.maxAngularVelocity = Mathf.Infinity;

        HingeJoint pedalsJoint = pedals.GetComponent<HingeJoint>();
        pedalsJoint.useMotor = true;

        Rigidbody pedalsRB = pedals.GetComponent<Rigidbody>();
        pedalsRB.centerOfMass = new Vector3(0, 0, 0);
        pedalsRB.maxAngularVelocity = Mathf.Infinity;

        Rigidbody leftPedalRB = leftPedal.GetComponent<Rigidbody>();
        leftPedalRB.maxAngularVelocity = Mathf.Infinity;

        Rigidbody rightPedalRB = rightPedal.GetComponent<Rigidbody>();
        rightPedalRB.maxAngularVelocity = Mathf.Infinity;
    }
}