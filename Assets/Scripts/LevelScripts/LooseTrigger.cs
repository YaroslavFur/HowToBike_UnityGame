using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseTrigger : MonoBehaviour
{
    [SerializeField] public GameObject frame;
    [SerializeField] public ReadSpeed readSpeedScript;

    [HideInInspector] public Loose looseScript;
    [HideInInspector] public bool isLayingDown;
    private float timeTillLoose = 0;

    // Start is called before the first frame update
    void Start()
    {
        timeTillLoose = looseScript.secondsOfBikeLayToLoose;
    }

    private void FixedUpdate()
    {
        //Debug.Log("Time Till loose: " + timeTillLoose);
        //Debug.Log("Rotation Z: " + transform.rotation.eulerAngles.z);
        //Debug.Log("Speed: " + readSpeedScript.speed);

        if ((transform.rotation.eulerAngles.z > looseScript.anlgeOfBikeTiltToLoose &&
            transform.rotation.eulerAngles.z < 360 - looseScript.anlgeOfBikeTiltToLoose) &&
            readSpeedScript.speed < looseScript.minSpeedToLoose)
            isLayingDown = true;
        else
            isLayingDown = false;

        if (isLayingDown)
        {
            timeTillLoose -= Time.fixedDeltaTime;
        }
        else
        {
            timeTillLoose = looseScript.secondsOfBikeLayToLoose;
        }

        if (timeTillLoose <= 0)
        {
            looseScript.LooseLevel();
        }
    }
}
