using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextMessage : MonoBehaviour
{
    private AudioSource audio;
    [SerializeField] private AudioClip notificationSound;
    [SerializeField] private AudioClip newMessageSound;

    [SerializeField] private TextMeshPro receivedText;

    //The two option buttons
    [SerializeField] private MessageButton option1;
    [SerializeField] private MessageButton option2;

    public float phonecallTimer = 0; //How long the phonecall has been going for
    public string messageType = "phonecall"; //phonecall, voicemail, email, door. (Yes I am making this a string whatchu gonna do about it)

    void Start()
    {
        audio = GetComponent<AudioSource>();
        switch (messageType)
        {
            case "phonecall":
                receivedText.text = "Hey, it's me, Sam. We haven't talked in a long while, have we? I'm in the neighbourhood, and just thought maybe we could hang out again. What do you say?";

                option1.text.text = "...";
                option2.text.text = "Maybe.";

                //PLAY VOICECLIP OF PERSON TALKING IN PHONE HERE
                option2.gameObject.SetActive(false);
                break;
            case "voicemail":
                receivedText.text = "-VOICE MESSAGE TRANSCRIPT- \nHey, it's me, Sam. We haven't talked in a long while, have we? I'm in the neighbourhood, and just thought maybe we could hang out again. Call me back when you can, if you are interested...";

                option1.text.text = "EXIT";

                audio.clip = newMessageSound;
                audio.Play();
                option2.gameObject.SetActive(false);
                break;
            case "email":
                receivedText.text = "NEW EMAIL RECEIVED\nHello, John. \n We've been missing you at the office. I hope you're doing well, the news about Eve was a big shock to all of us. How much longer are you going to be on leave?\n\nWith many wishes,\nJake";

                option1.text.text = "EXIT";
                option2.text.text = "REPLY";

                audio.clip = notificationSound;
                audio.Play();
                break;
            case "door":
            default:
                receivedText.text = "Howdy Neighbour! Barely seen your face in like, two months! I noticed you've left a garbage bag here in the hall for a week. Are you going to take it out soon?";

                option1.text.text = "...";
                option2.text.text = "...";

                //PLAY VOICECLIP OF PERSON TALKING AT THE DOOR HERE
                break;
        }
    }

    void Update()
    {
        switch (messageType)
        {
            case "phonecall":
                phonecallTimer += Time.deltaTime;

                //If the player waits 30 seconds the call hangs up automatically as if they chose silence
                if (phonecallTimer > 30)
                {
                    optionChosen(1);
                }
                //Enables the second dialogue option after 20 seconds
                else if (phonecallTimer > 20)
                {
                    option2.gameObject.SetActive(true);
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

    //When a button is pressed. 1 = Left button, 2 = Right button
    public void optionChosen(int buttonNumber)
    {
        //Yeah this code is bad but it's faster for me to do it this way than a better way
        switch (messageType)
        {
            case "phonecall":
                if (buttonNumber == 1)
                {
                    //INSERT CALL TO A CENTRAL MANAGER OF PLAYER CHOICES HERE
                    closeMessage();
                }
                else
                {
                    //INSERT CALL TO A CENTRAL MANAGER OF PLAYER CHOICES HERE
                    closeMessage();
                }
                break;
            case "voicemail":
                //INSERT CALL TO A CENTRAL MANAGER OF PLAYER CHOICES HERE
                closeMessage();
                break;
            case "email":
                if (buttonNumber == 1)
                {
                    //INSERT CALL TO A CENTRAL MANAGER OF PLAYER CHOICES HERE
                    closeMessage();
                }
                else
                {
                    //INSERT CALL TO A CENTRAL MANAGER OF PLAYER CHOICES HERE
                    closeMessage();
                }
                break;
            case "door":
            default:
                if (buttonNumber == 1)
                {
                    //INSERT CALL TO A CENTRAL MANAGER OF PLAYER CHOICES HERE
                    closeMessage();
                }
                else
                {
                    //INSERT CALL TO A CENTRAL MANAGER OF PLAYER CHOICES HERE
                    closeMessage();
                }
                break;
        }
    }

    private void closeMessage()
    {
        Destroy(gameObject);
    }
}
