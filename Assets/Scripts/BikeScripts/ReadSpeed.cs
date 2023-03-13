using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadSpeed : MonoBehaviour
{
    [SerializeField] public float speed;

    private Vector3 lastPosition = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        speed = Vector3.Distance(currentPosition, lastPosition) / Time.fixedDeltaTime;
        lastPosition = currentPosition;
    }
}
