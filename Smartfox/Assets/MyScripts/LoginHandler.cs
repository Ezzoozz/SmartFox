using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginHandler : MonoBehaviour
{
    InputField enterName;
    SFS2X_Connect connection;
    // Start is called before the first frame update
    void Start()
    {
        enterName = GetComponent<InputField>();
        connection = FindObjectOfType<SFS2X_Connect>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessEnter();

    }

     void ProcessEnter()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        connection.LoginToServer(enterName.text);
    }
}
