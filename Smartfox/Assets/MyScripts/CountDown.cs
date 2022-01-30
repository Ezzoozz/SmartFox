using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CountDown : MonoBehaviour
{
    int count = 5;
   public TextMeshProUGUI countDownText;

    // Start is called before the first frame update
    void Start()
    {
        countDownText = GetComponent<TextMeshProUGUI>();

        StartCoroutine(CountDownTimer());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

     IEnumerator CountDownTimer()
    {
        Debug.Log("counting down");

        for (int i=0; i < 5; i++){

            count--;
            yield return new WaitForSeconds(1f);

            countDownText.text = $"Starting in {count}";

        }

        countDownText.enabled = false;
    }
}
