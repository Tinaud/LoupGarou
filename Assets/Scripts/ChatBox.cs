using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ChatBox : NetworkBehaviour
{
    //public ScrollRect ChatZone;
    public InputField TextZone;
    public Text WelcomeText;
    public Text RoleText;
    public GameObject ChatZone;
    public GameObject Timer;
    public GameObject TextPrefab;

    private List<GameObject> MessagesBox;
    private float MaxLength = 0.0f;

    private RectTransform ContentTransform;
    private Text MsgText;
    private GameObject Msg;

    private Player P1;

    private float voteTime;

    public void setPlayer(Player j)
    {
        P1 = j;
    }

    // Use this for initialization
    void Start()
    {
        //TextZone.text;
        MessagesBox = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && TextZone.text != "")
        {
            if (P1 != null)
                TextZone.text = P1.pseudo + " : " + TextZone.text;
            else
                TextZone.text = "NoName : " + TextZone.text;
            P1.CmdSendMsg(TextZone.text);
            TextZone.text = "";
        }
        UpdateTextPos();
        if (P1.timer)
        {
            voteTime -= Time.deltaTime;
            Timer.GetComponentInChildren<Text>().text = "Temps de vote : " + (int)voteTime + " secondes.";
            if (voteTime <= 0)
                P1.timer = false;
        }
        else
        { 
            voteTime = 30f;
            Timer.GetComponentInChildren<Text>().text = "Ce n'est pas le vote";
        }
    }

    public void ChatUpdate(string newMsg)
    {
        Msg = Instantiate(TextPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        Msg.transform.SetParent(ChatZone.transform);
        MsgText = Msg.GetComponent<Text>();
        Msg.transform.rotation = new Quaternion(0, 0, 0, 0);
        MsgText.text = newMsg;
        MessagesBox.Add(Msg);
        Msg.transform.localPosition = new Vector3(0, -14.0f * MessagesBox.Count, 0);
    }

    void UpdateTextPos()
    {
        if (MessagesBox != null)
        {
            for (int i = 0; i < MessagesBox.Count; i++)
            {
                ContentTransform = MessagesBox[i].GetComponent<RectTransform>();
                if (ContentTransform.sizeDelta.x > (MaxLength + 355.0f))
                    MaxLength = ContentTransform.sizeDelta.x - 355.0f;
                MessagesBox[i].transform.localPosition = new Vector3(ContentTransform.sizeDelta.x / 2 + 2.0f, -14.0f * (i + 1), 0);
            }
            ContentTransform = ChatZone.GetComponent<RectTransform>();
            ContentTransform.sizeDelta = new Vector2(MaxLength, 17.10001f * MessagesBox.Count);
        }
    }
}
