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

public class CollectCoin : MonoBehaviour
{
    TextMeshProUGUI goldCount;
    // [SerializeField] TextMeshPro goldCount;

     void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");

        if (other.TryGetComponent (out PlayerGold gold) )
        {
            gold.UpdateGoldCount();
        }

        Destroy(gameObject);

    }
}
