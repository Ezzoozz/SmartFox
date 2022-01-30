using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities;
using TMPro;
using System;

public class SFS2X_Connect : MonoBehaviour
{
    public string ServerIP = "127.0.0.1";

    public int ServerPort = 9933;

    public string ZoneName = "SmartFox";

    public string UserName = "Ezz";

    public string RoomName = "Lobby";

   public SmartFox sfs;
    // public SmartFox Sfs { get; }


     void Awake()
    {
        DontDestroyOnLoad(gameObject);   
    }
    void Start()
    {
        sfs = new SmartFox();

        sfs.ThreadSafeMode = true;

        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
        sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);

        sfs.Connect(ServerIP, ServerPort);
        
    }

    

    void Update()
    {
        sfs.ProcessEvents();
    }

    void OnConnection(BaseEvent e)
    {
        if ((bool)e.Params["success"])
        {
            Debug.Log("connected");
            

        }
        else
        {
            Debug.Log("connection failed");
        }
    }

    public void LoginToServer(string name)
    {
        sfs.Send(new LoginRequest(name, "hello", ZoneName));
    }
    void OnLogin(BaseEvent e)
    {
        User user = (User)e.Params["user"];
        
        Debug.Log("Logged in " + user.Name);
        sfs.Send(new JoinRoomRequest(RoomName));
    }

   

    void OnJoinRoom(BaseEvent e)
    {
        Debug.Log("Joined Room: " + e.Params["room"]);
        

    }

    void OnApplicationQuit()
    {
        if (sfs.IsConnected)
            sfs.Disconnect();

    }
    
    
    void OnPublicMessage(BaseEvent e)
    {
        Room room = (Room)e.Params["room"];

        User sender = (User)e.Params["sender"];

        Debug.Log($"( {room.Name} ) {sender.Name} : {e.Params["message"]}");

    }
    void OnJoinRoomError(BaseEvent e)
    {
        Debug.Log("JoinRoom Error ("+ e.Params["errorCode"] +")" + e.Params["errorMessage"]);

    }

    void OnLoginError(BaseEvent e)
    {

        Debug.Log("Login error " + e.Params["errorCode"] +" error message " + e.Params["errorMessage"]);
    }

}
