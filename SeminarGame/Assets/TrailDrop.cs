using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrailDrop : MonoBehaviour
{
    //God why is the trail renderer from unity so damn bad.

    public AnimationCurve speed;
    public float speedMultiplier;

    Vector3 from;
    Vector3 to;

    float timer;
    float distance;

    LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        timer += (Time.deltaTime * speedMultiplier) / distance;

        line.SetPosition(0, Vector2.Lerp(from, to, speed.Evaluate(timer)));
        line.SetPosition(1, to);

        if (timer > 1) enabled = false;
    }

    public void GoToLine(Vector3 from, Vector3 to)
    {
        this.from = from;
        this.to = to;

        distance = Vector2.Distance(from, to);

        enabled = true;
        timer = 0;
    }

    public void Resize(float size)
    {
        line.startWidth = size;
        line.endWidth = size;
    }

    public void Move(Vector3 translation)
    {
        to += translation;
    }
}
