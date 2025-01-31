using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private float musicStartLoweringTimer = 0;
    [SerializeField] private float secondTimer = 0;
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
                music.volume -= 0.003f;
            }
        }
        else
        {
            musicStartLoweringTimer += Time.deltaTime;
        }
    }
}
