using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDisappear : MonoBehaviour
{
    public SpriteRenderer sprite;

    public AnimationCurve speed;
    public float speedMultiplier = 1;
    float timer = 1;

    void Update()
    {
        timer -= Time.deltaTime * speedMultiplier;

        sprite.color = new Color(1, 1, 1, speed.Evaluate(timer));

        Debug.Log(timer + "  " + speed.Evaluate(timer));

        if (timer <= 0) enabled = false;
    }
}
