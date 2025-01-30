using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageButton : MonoBehaviour
{
    public bool mouseOverThis = false;
    private TextMessage message;

    void Start()
    {
        mouseOverThis = false;
        message = GetComponentInParent<TextMessage>();
    }

    void Update()
    {
        
    }

    public void OnMouseOver()
    {
        mouseOverThis = true;
    }

    public void OnMouseExit()
    {
        mouseOverThis = false;
    }
}
