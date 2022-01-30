using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class GoldDisplay : MonoBehaviour
{

      public  PlayerGold player;
     [SerializeField] public TextMeshProUGUI texts;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerGold>();
        if (player == null)
        {
            Debug.Log("playergold componenet not found");

        }
        
        texts = GetComponent<TextMeshProUGUI>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (texts == null) Debug.Log("text is null");
        if (player == null) Debug.Log("playergold componenet no found");
        texts.text = "gold " + player.gold;
    }
}
