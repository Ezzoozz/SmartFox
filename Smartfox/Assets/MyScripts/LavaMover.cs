using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaMover : MonoBehaviour
{
    [SerializeField] Vector3 graveYardPosition;

    SFS2X_Connect connection;

    GameObject LavaPrefab;

    GameObject localLava;

    int deathCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        connection = FindObjectOfType<SFS2X_Connect>();
        SpawnLava();

        List<RoomVariable> listOfVars = new List<RoomVariable>();


        listOfVars.Add(new SFSRoomVariable("deathCount", 0));

        listOfVars.Add(new SFSRoomVariable("lavaPos",(double) -100));

        connection.sfs.Send(new Sfs2X.Requests.SetRoomVariablesRequest(listOfVars));
    }

    private void UpdateDeathCount()
    {
        List<RoomVariable> listOfVars = new List<RoomVariable>();

        int incremented = connection.sfs.LastJoinedRoom.GetVariable("deathCount").GetIntValue();

        double lavapos = connection.sfs.LastJoinedRoom.GetVariable("lavaPos").GetDoubleValue();

        listOfVars.Add(new SFSRoomVariable("lavaPos", lavapos + 0.1));

        listOfVars.Add(new SFSRoomVariable("deathCount", incremented+1));

        connection.sfs.Send(new Sfs2X.Requests.SetRoomVariablesRequest(listOfVars));
    }

    private void SpawnLava()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
      //  transform.Translate(0, 0.01f, 0);

        List<RoomVariable> listOfVars = new List<RoomVariable>();

        double lavapos = connection.sfs.LastJoinedRoom.GetVariable("lavaPos").GetDoubleValue();

        listOfVars.Add(new SFSRoomVariable("lavaPos", lavapos + 0.1));

        connection.sfs.Send(new Sfs2X.Requests.SetRoomVariablesRequest(listOfVars));

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision");
        if (other.TryGetComponent<PlayerMover>(out PlayerMover mover) )
        {
            Debug.Log("trigger with player mover");
            deathCount++;

            UpdateDeathCount();

            UpdatePosition(other);
            

        }
    }

    private void UpdatePosition(Collider other)
    {
        other.GetComponent<Transform>().position = graveYardPosition;

        List<UserVariable> userVariables = new List<UserVariable>();

        userVariables.Add(new SFSUserVariable("x", (double)other.transform.position.x));
        userVariables.Add(new SFSUserVariable("y", (double)other.transform.position.y));
        userVariables.Add(new SFSUserVariable("z", (double)other.transform.position.z));
        userVariables.Add(new SFSUserVariable("rot", (double)other.transform.rotation.y));

        connection.sfs.Send(new SetUserVariablesRequest(userVariables));

        other.GetComponent<PlayerMover>().MovementDirty = true;
    }

}
