using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip nutDelivered;
    [SerializeField] private AudioClip lose;
    [SerializeField] private AudioClip windingNut;
    private AudioSource player;

    private void Awake()
    {
        EventManager.OnGameOver.AddListener(PlayGameOver);
        EventManager.OnNutDelivered.AddListener(PlayNutDelivered);
        EventManager.OnBlockSpinner.AddListener(PlayNutWinding);
    }

    void Start()
    {
        player = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayGameOver()
    {
        player.clip = lose;
        player.Play();
    }
    void PlayNutDelivered()
    {
        player.clip = nutDelivered;
        player.Play();
    }
    void PlayNutWinding()
    {
        player.clip = windingNut;
        player.Play();
    }

    public void SetSound(float vol)
    {
        player.volume = vol;
        PlayGameOver();
    }

}
