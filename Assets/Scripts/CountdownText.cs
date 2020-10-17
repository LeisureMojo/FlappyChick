using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour
{
    public delegate void CountdownFinished();

    public static event CountdownFinished OnCountdownFinished;

    public AudioSource countdownAudio;

    Text countdown;

    void OnEnable()
    {
        // this will get called every time we set to be active
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine("Countdown");
    }

    IEnumerator Countdown()
    {
        int count = 3;
        new WaitForSeconds(1);
        for (int i = 0; i < count; i++)
        {
            countdown.text = (count - i).ToString();
            countdownAudio.Play();
            yield return new WaitForSeconds(1);
        }

        OnCountdownFinished();
    }
}