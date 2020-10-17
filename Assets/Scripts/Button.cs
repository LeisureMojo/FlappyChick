using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public AudioSource myButton;
    public AudioClip hoverSoundEffect;

    public AudioClip clickSoundEffect;

    public void PointerEnter() 
    {
        transform.localScale = new Vector2(1.2f, 1.2f);
        myButton.PlayOneShot(hoverSoundEffect);
    }

    public void PointerDown()
    {
        transform.localScale = new Vector2(1.2f, 1.2f);
        myButton.PlayOneShot(clickSoundEffect);
    }

    public void PointerExit()
    {
        transform.localScale = new Vector2(1f, 1f);
    }
}
