using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadSpeed : MonoBehaviour
{
    private Rigidbody objectAtSpeedRB;
    [SerializeField] public float speed;

    // Start is called before the first frame update
    void Start()
    {
        objectAtSpeedRB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        speed = (float)transform.InverseTransformDirection(objectAtSpeedRB.velocity).magnitude;
    }
}
