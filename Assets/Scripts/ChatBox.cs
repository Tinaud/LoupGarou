using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class ChatBox : NetworkBehaviour
{
    //public ScrollRect ChatZone;
    public InputField TextZone;
    public GameObject ChatZone;
    public GameObject TextPrefab;

    private List<GameObject> MessagesBox;
    private float MaxLength = 0.0f;

    private RectTransform ContentTransform;
    private Text MsgText;
    private GameObject Msg;

    private Player P1;

    public void setPlayer(Player j)
    {
        P1 = j;
    }

    // Use this for initialization
    void Start () {
        //TextZone.text;
        MessagesBox = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && TextZone.text != "")
        {
            if (P1 != null)
                TextZone.text = "Player " + P1.id + " : " + TextZone.text;
            else
                TextZone.text = "NoName : " + TextZone.text;
            P1.SenMsg(TextZone.text);
            TextZone.text = "";
        }
        UpdateTextPos();
    }

    public void ChatUpdate (string newMsg)
    {
        Msg = Instantiate(TextPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Msg.transform.SetParent(ChatZone.transform);
        MsgText = Msg.GetComponent<Text>();
        MsgText.text = newMsg;
        MessagesBox.Add(Msg);
        Msg.transform.localPosition = new Vector3(0, -14.0f* MessagesBox.Count, 0);
    }

    void UpdateTextPos()
    {
        if(MessagesBox != null)
        { 
            for (int i = 0; i < MessagesBox.Count; i++)
            {
                ContentTransform = MessagesBox[i].GetComponent<RectTransform>();
                if (ContentTransform.sizeDelta.x > MaxLength)
                    MaxLength = ContentTransform.sizeDelta.x-355.0f;
                MessagesBox[i].transform.localPosition = new Vector3(ContentTransform.sizeDelta.x/2 + 2.0f, -14.0f * (i+1), 0);
            }
            ContentTransform = ChatZone.GetComponent<RectTransform>();
            ContentTransform.sizeDelta = new Vector2(MaxLength, 17.10001f * MessagesBox.Count);
        }
    }
}
