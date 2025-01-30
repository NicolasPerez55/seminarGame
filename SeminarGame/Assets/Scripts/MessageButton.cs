using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageButton : MonoBehaviour
{
    [SerializeField] private int buttonNumber = 1;
    public bool mouseOverThis = false; //mouse hovering this button

    private TextMessage message;
    public TextMeshPro text;



    void Start()
    {
        mouseOverThis = false;
        message = GetComponentInParent<TextMessage>();
        text = GetComponentInChildren<TextMeshPro>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (mouseOverThis) //clicked on this button
            {
                message.optionChosen(buttonNumber);
            }
        }

        switch (message.messageType)
        {
            case "phonecall":
                if (buttonNumber == 1)
                {
                    //Adds extra . to the "silence" button
                    if (message.phonecallTimer > 15)
                    {
                        text.text = "........";
                    }
                    else if (message.phonecallTimer > 12)
                    {
                        text.text = ".......";
                    }
                    else if (message.phonecallTimer > 9)
                    {
                        text.text = "......";
                    }
                    else if (message.phonecallTimer > 6)
                    {
                        text.text = ".....";
                    }
                    else if (message.phonecallTimer > 3)
                    {
                        text.text = "....";
                    }
                    else
                    {
                        text.text = "...";
                    }
                }
                break;
            case "voicemail":
                break;
            case "email":
                break;
            case "door":
            default:
                break;
        }

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
