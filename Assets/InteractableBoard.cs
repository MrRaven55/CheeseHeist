using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBoard : MonoBehaviour, ISign
{
    public GameObject UInote;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SignState(bool state)
    {
        UInote.SetActive(state);
    }
}
