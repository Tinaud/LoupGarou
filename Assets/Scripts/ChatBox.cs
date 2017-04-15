using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChatBox : MonoBehaviour {

    //public ScrollRect ChatZone;
    public InputField TextZone;
    public GameObject ChatZone;
    public GameObject TextPrefab;

    private List<GameObject> MessagesBox = new List<GameObject>();

    // Use this for initialization
    void Start () {
        //TextZone.text;
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && TextZone.text != "")
        {
            ChatUpdate(TextZone.text);
            TextZone.text = "";
        }
    }

    void ChatUpdate (string newMsg)
    {
        RectTransform ContentTransform;
        Text MsgText;

        GameObject Msg = Instantiate(TextPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Msg.transform.SetParent(ChatZone.transform);
        MessagesBox.Add(Msg);
        Msg.transform.localPosition = new Vector3(105.0f, -14.0f* MessagesBox.Count, 0);
        MsgText = Msg.GetComponent<Text>();

        MsgText.text = newMsg;
        ContentTransform = ChatZone.GetComponent<RectTransform>();
        ContentTransform.sizeDelta = new Vector2(0, 17.10001f * MessagesBox.Count);
    }
}
