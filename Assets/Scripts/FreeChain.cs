using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeChain : MonoBehaviour
{
    [SerializeField] private GameObject backWheel;
    [SerializeField] private GameObject pedals;

    private HingeJoint backWheelJoint;
    private HingeJoint pedalsJoint;
    private FixedJoint chainJoint;

    // Start is called before the first frame update
    void Start()
    {
        backWheelJoint = backWheel.GetComponent<HingeJoint>();
        pedalsJoint = pedals.GetComponent<HingeJoint>();
        chainJoint = GetComponent<FixedJoint>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // free chain if backWheel or pedals are broken
        if (!pedalsJoint || !backWheelJoint)
        {
            if (chainJoint)
            {
                Destroy(chainJoint);
            }
        }
    }
}
