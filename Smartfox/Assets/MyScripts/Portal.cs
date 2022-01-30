using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Logging;

using System;
using TMPro;

public class Portal : MonoBehaviour
{
    Gamemanager manager;

    [SerializeField] int readyCount = 0;
     
    SFS2X_Connect connection;

    [SerializeField] int teamSize = 2;
    // Start is called before the first frame update
    void Start()
    {
        connection = FindObjectOfType<SFS2X_Connect>();

   
        connection.sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        connection.sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        connection.sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnRoomJoin);
        manager = FindObjectOfType<Gamemanager>();

       
    }

    void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.transform.position = new Vector3(0, 0, 1381);



        connection.sfs.Send(new JoinRoomRequest("Game"));


    }

   
    private void OnRoomJoin(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];

        if (room.UserList.Count >= teamSize && room.Name=="Game")
        {
            manager.LoadLava();

        }
    }

    private void OnRoomJoinError(BaseEvent evt)
    {
        string error = (string)evt.Params["errorMessage"];

        Debug.Log("cant join room, error message: " + error);
    }


}
