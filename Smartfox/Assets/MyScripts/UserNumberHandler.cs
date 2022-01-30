using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UserNumberHandler : MonoBehaviour
{
    TextMeshProUGUI display;
    SFS2X_Connect connection;
    [SerializeField] int numOfPlayers = 2;

    // Start is called before the first frame update
    void Start()
    {
        display = GetComponent<TextMeshProUGUI>();

        connection = FindObjectOfType<SFS2X_Connect>();

        connection.sfs.AddEventListener(SFSEvent.ROOM_JOIN, UpdateDisplay);

        connection.sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, UpdateDisplay);
    }

    private void UpdateDisplay(BaseEvent evt)
    {
        Room room = (Room) evt.Params["room"];

       
      
            display.text = "Starting game in 5 seconds";
            Invoke("LoadScene", 2f);

        
    }

     void LoadScene()
    {
        
        connection.sfs.RemoveEventListener(SFSEvent.ROOM_JOIN, UpdateDisplay);
        connection.sfs.RemoveEventListener(SFSEvent.USER_ENTER_ROOM, UpdateDisplay);
        SceneManager.LoadScene("Game");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
