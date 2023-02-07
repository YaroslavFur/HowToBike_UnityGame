using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : MonoBehaviour
{

    private WheelCollider wheelCollider;

    private void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        wheelCollider.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
