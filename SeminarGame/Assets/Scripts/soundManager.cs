using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private float musicStartLoweringTimer = 0;
    [SerializeField] private float secondTimer = 0;

    [SerializeField] private AudioSource pieceSettleSound;
    [SerializeField] private AudioSource instaDropSound;
    [SerializeField] private AudioSource lineClearSound;

    [SerializeField] private AudioSource computerStatic;
    void Start()
    {
        
    }

    void Update()
    {
        
        if (musicStartLoweringTimer >= 300)
        {
            secondTimer += Time.deltaTime;
            if (secondTimer >= 1)
            {
                secondTimer = 0;
                if (music.volume > 0)
                {
                    music.volume -= 0.003f;
                    if (music.volume <= 0.001f)
                    {
                        music.volume = 0;
                    }
                }
                computerStatic.volume += 0.003f;
                if (computerStatic.volume >= 0.250f)
                {
                    computerStatic.volume = 0.25f;
                }
            }
        }
        else
        {
            musicStartLoweringTimer += Time.deltaTime;
        }
    }

    public void pieceSettles()
    {
        pieceSettleSound.Play();
    }

    public void instaDrop()
    {
        instaDropSound.Play();
    }

    public void lineClear()
    {
        lineClearSound.Play();
    }
}
