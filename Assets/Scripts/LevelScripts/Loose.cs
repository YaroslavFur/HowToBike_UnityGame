using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loose : MonoBehaviour
{
    [SerializeField] public Win winScript;
    [SerializeField] public float anlgeOfBikeTiltToLoose;
    [SerializeField] public float minSpeedToLoose;
    [SerializeField] public float secondsOfBikeLayToLoose;
    [HideInInspector] public bool isAlreadyLost = false;

    // Start is called before the first frame update
    void Awake()
    {
        LooseTrigger[] looseTriggers = (LooseTrigger[])FindObjectsOfType(typeof(LooseTrigger));
        foreach (var looseTrigger in looseTriggers)
        {
            looseTrigger.looseScript = this;
        }
    }

    public void LooseLevel()
    {
        if (winScript.isAlreadyWon)
            return;

        if (isAlreadyLost)
            return;

        isAlreadyLost = true;
        Debug.Log("Level Failed!");
    }
}
