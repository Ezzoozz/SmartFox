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

public class PlayerGold : MonoBehaviour
{
    public int gold = 0;

    public TextMeshProUGUI goldText;
    // Start is called before the first frame update
    SmartFox sfs;

     void Start()
    {
        sfs = FindObjectOfType<SFS2X_Connect>().sfs;

        if (GameObject.FindGameObjectWithTag("GoldCount").GetComponent<TextMeshProUGUI>() != null)
            goldText = GameObject.FindGameObjectWithTag("GoldCount").GetComponent<TextMeshProUGUI>();
        else Debug.Log(" text mesh is null");

        SendGoldVariable();
    }
    public void UpdateGoldCount()
    {
        gold++;

        goldText.text = $"Gold : {gold}";

        SendGoldVariable();
    }

    private void SendGoldVariable()
    {
        List<UserVariable> goldList = new List<UserVariable>();

        goldList.Add(new SFSUserVariable("gold", (int)gold));

        sfs.Send(new SetUserVariablesRequest(goldList));



       /*
        
       var x=  sfs.UserManager.GetUserList();

       var variables = x[0].GetVariable("gold");



        */


        
    }
}
