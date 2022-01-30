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
public class Gamemanager : MonoBehaviour
{
    LavaMover lava = null;

    SFS2X_Connect connection;
    SmartFox sfs;

  //  [SerializeField] GameObject text; 

    Dictionary<SFSUser, GameObject> remotePlayers = new Dictionary<SFSUser, GameObject>();

    [SerializeField] GameObject prefab;

    [SerializeField] GameObject remotePrefab;

    GameObject LocalPlayer;

   [SerializeField] TextMeshProUGUI wintext;

    private void Awake()
    {
        if (FindObjectsOfType<Gamemanager>().Length > 1)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        

        Application.runInBackground = true;
        connection = FindObjectOfType<SFS2X_Connect>();

      
        CheckForConnection();

        
        
    }

     void CheckForConnection()
    {
        
        if (connection.sfs == null)
        {
            SceneManager.LoadScene("Login");
        }
        else
        {
            Debug.Log(" Connection maintained");
        }
        
        sfs = connection.sfs;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        SetEventListeners();



        SpawnLocalPlayer(prefab);
    }

     void SpawnLocalPlayer(GameObject prefab)
    {
        if (LocalPlayer != null) return;

        LocalPlayer = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);

        LocalPlayer.GetComponentInChildren<TextMesh>().text = sfs.MySelf.Name;
    }

    private void SetEventListeners()
    {
        sfs.AddEventListener(SFSEvent.OBJECT_MESSAGE, OnObjectMessage);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.AddEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, OndeathCountUpdate);
        sfs.AddEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, OnLavaPosUpdate);
    }

    private void OnLavaPosUpdate(BaseEvent evt)
    {
        if (lava == null) lava = FindObjectOfType<LavaMover>();

        List<string> changedVars = (List<string>)evt.Params["changedVars"];

        if (!changedVars.Contains("lavaPos")) { return; }

        double lavaPos = sfs.LastJoinedRoom.GetVariable("lavaPos").GetDoubleValue();

        float lavax = lava.transform.position.x;

        float lavaz = lava.transform.position.z;

        float lavay = (float) lavaPos;

        lava.transform.position = new Vector3(lavax, lavay, lavaz);

    }

    private void OndeathCountUpdate(BaseEvent evt)
    {
        List<string> changedVars = (List<string>)evt.Params["changedVars"];

        if (!changedVars.Contains("deathCount")) {  return; }

        int deathCount = sfs.LastJoinedRoom.GetVariable("deathCount").GetIntValue();

        Debug.Log("death count is now " + deathCount);

        Debug.Log("users in room" + connection.sfs.UserManager.UserCount);

        if (deathCount != connection.sfs.UserManager.UserCount) return;//if deathcount is equal to number of players end game

       
        User user = GetGameWinner();

       

        Debug.Log(user.Name + " has won");

         

        // wintext.text = user.Name+" is the winner!";

        //  Invoke("endGame", 5f);

        sfs.Send(new JoinRoomRequest("Lobby"));

        SceneManager.LoadScene("Game");


    }
      
     User GetGameWinner()
    {

        List<User> users = connection.sfs.LastJoinedRoom.PlayerList;

        int numOfUsers = users.Count;

        User winner = users[0];

        int max = 0;

        for (int i = 0; i < numOfUsers; i++)
        {
            if (users[i].GetVariable("gold").GetIntValue() > max)
            {
                winner = users[i];

                max = users[i].GetVariable("gold").GetIntValue();

            }

        } //now we have the winner

        return winner;
    }

    void endGame()
    {
       

        


    }




    void FixedUpdate()
    {
        sfs.ProcessEvents();

        SendMoveVariables();



    }

    private void SendMoveVariables()
    {
        if (!LocalPlayer.GetComponent<PlayerMover>().MovementDirty) return;

        List<UserVariable> userVariables = new List<UserVariable>();

        userVariables.Add(new SFSUserVariable("x",   (double) LocalPlayer.transform.position.x));
        userVariables.Add(new SFSUserVariable("y",   (double) LocalPlayer.transform.position.y));
        userVariables.Add(new SFSUserVariable("z",   (double) LocalPlayer.transform.position.z));
        userVariables.Add(new SFSUserVariable("rot", (double) LocalPlayer.transform.rotation.y));

        sfs.Send(new SetUserVariablesRequest(userVariables));

        LocalPlayer.GetComponent<PlayerMover>().MovementDirty = true;
    }

    private void OnApplicationQuit()
    {
        RemoveLocalPlayer();
    }

    private void RemoveLocalPlayer()
    {
        SFSObject obj = new SFSObject();

        obj.PutUtfString("cmd", "rm");

        sfs.Send(new ObjectMessageRequest(obj, sfs.LastJoinedRoom));
    }



    private void OnUserEnterRoom(BaseEvent evt)
    {
        if(LocalPlayer != null)
        {
            List<UserVariable> userVariables = new List<UserVariable>();
            userVariables.Add(new SFSUserVariable("x", (double)LocalPlayer.transform.position.x));
            userVariables.Add(new SFSUserVariable("y", (double)LocalPlayer.transform.position.y));
            userVariables.Add(new SFSUserVariable("z", (double)LocalPlayer.transform.position.z));
            userVariables.Add(new SFSUserVariable("rot", (double)LocalPlayer.transform.rotation.eulerAngles.y));
            userVariables.Add(new SFSUserVariable("x", (double)LocalPlayer.transform.position.x));

            sfs.Send(new SetUserVariablesRequest(userVariables));
        }
    }

   
  public void OnConnectionLost(BaseEvent e)
    {
        sfs.RemoveAllEventListeners();

        SceneManager.LoadScene("login");
    }

    public void OnObjectMessage(BaseEvent evt)
    {
        ISFSObject dataObj = (SFSObject)evt.Params["message"];
        SFSUser sender = (SFSUser)evt.Params["Sender"];

        if(dataObj.ContainsKey("cmd"))
        {
            if(dataObj.GetUtfString("cmd") == "rm")
            {
                RemoveRemotePlayer(sender);
            }
        }


    }
    private void OnUserExitRoom(BaseEvent evt)
    {
        SFSUser user = (SFSUser) evt.Params["user"];

        RemoveRemotePlayer(user);
    }

    private void RemoveRemotePlayer(SFSUser sFSUser)
    {
        if (sFSUser == sfs.MySelf) return;

        if (remotePlayers.ContainsKey(sFSUser))
        {
            Destroy(remotePlayers[sFSUser]);
            remotePlayers.Remove(sFSUser);

            
        }
    }

    private void OnUserVariableUpdate(BaseEvent evt)
    {
       List<string> changedVars = (List<string>) evt.Params["changedVars"];

        SFSUser user = (SFSUser)evt.Params["user"];

        if (user == sfs.MySelf) return;

        if (!remotePlayers.ContainsKey(user))
        {
            Vector3 pos = new Vector3(0, 0, 0);

            if(user.ContainsVariable("x") && user.ContainsVariable("y") && user.ContainsVariable("z"))
            {
                pos.x = (float)user.GetVariable("x").GetDoubleValue();
                pos.y = (float)user.GetVariable("y").GetDoubleValue();
                pos.z = (float)user.GetVariable("z").GetDoubleValue();
            }
            float rotAngle = 0;
            if (user.ContainsVariable("rot"))
            {
                rotAngle = (float)user.GetVariable("rot").GetDoubleValue();

            }

            SpawnRemotePlayer(user, pos, Quaternion.Euler(0,rotAngle,0));
        }

        if(changedVars.Contains("x") && changedVars.Contains("y") && changedVars.Contains("z") && changedVars.Contains("rot"))
        {
                remotePlayers[user].GetComponent<SimpleInterpolation>().SetTransform(
                new Vector3((float) user.GetVariable("x").GetDoubleValue(), (float)user.GetVariable("y").GetDoubleValue(), (float)(user.GetVariable("z").GetDoubleValue())),
                Quaternion.Euler(0, (float)user.GetVariable("rot").GetDoubleValue(), 0), true);

        }

      
    }

    private void SpawnRemotePlayer(SFSUser user, Vector3 pos, Quaternion quaternion)
    {
        if(remotePlayers.ContainsKey(user) && remotePlayers[user] != null)
        {
            Destroy(remotePlayers[user]);
            remotePlayers.Remove(user);
        }

        GameObject remotePlayer = GameObject.Instantiate(remotePrefab, new Vector3(0, 0, 0), Quaternion.identity);

        remotePlayer.GetComponentInChildren<TextMesh>().text = user.Name;

        remotePlayers.Add(user, remotePlayer);


    }

    public void LoadLava()
    {
       // Reset();


       // text.gameObject.SetActive(true);

        Debug.Log("loading scene");
        Invoke("LoadLavaScene", 1f);

        
    }

    void LoadLavaScene()
    {

        LocalPlayer.transform.position = new Vector3(0, 0, 0);

        SceneManager.LoadScene("Lava");

        LocalPlayer.GetComponent<PlayerGold>().goldText = GameObject.FindGameObjectWithTag("GoldCount").GetComponent<TextMeshProUGUI>() ;



    }
    
       

        
    

    private void Reset()
    {
        sfs.RemoveEventListener(SFSEvent.OBJECT_MESSAGE, OnObjectMessage);
        sfs.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.RemoveEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
        sfs.RemoveEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.RemoveEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
    }


    // Update is called once per frame

}
