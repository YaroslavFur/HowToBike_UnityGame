using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] public GameObject objectFollowed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = objectFollowed.transform.position;
        transform.rotation = objectFollowed.transform.rotation;
    }
}
