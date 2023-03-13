using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour
{
    [SerializeField] public Loose looseScript;
    [HideInInspector] public bool isAlreadyWon = false;

    // Start is called before the first frame update
    void Awake()
    {
        WinTrigger[] winTriggers = (WinTrigger[])FindObjectsOfType(typeof(WinTrigger));
        foreach (var winTrigger in winTriggers)
        {
            winTrigger.winScript = this;
        }
    }

    public void WinLevel()
    {
        if (isAlreadyWon)
            return;

        if (looseScript.isAlreadyLost)
            return;

        isAlreadyWon = true;
        Debug.Log("Level Complete!");
    }
}
