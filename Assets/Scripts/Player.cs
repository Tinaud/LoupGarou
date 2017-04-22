using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

	const short CHAT_MESSAGE = 1002;

	class ChatMessage : MessageBase {
		public string message;
	}

	public void SendChatMessage(string _message) {
		ChatMessage msg = new ChatMessage ();
		msg.message = _message;
		Debug.Log ("Sending... " + msg.message);
		NetworkServer.SendToAll (MsgType.Highest+2, msg);
	}

	public void ReadMessage(NetworkMessage netMsg) {
		ChatMessage msg = netMsg.ReadMessage<ChatMessage>();
		Debug.Log ("Receiving... " + msg.message);
		CurrentChat.ChatUpdate(msg.message);
	}

    static int nextId = 0;
    public int id;
    public string pseudo;
    GameManager gm;
    Quaternion targetRotation;
    public GameObject ChatPrefab;
    GameObject SelectButton;
    GameObject ChatB;
	ChatBox CurrentChat;

    Player Pla;
    bool yourTurn = true;


    void Start() {
		if (isLocalPlayer) {
			id = nextId++;
			gm = Camera.main.GetComponent<GameManager> ();
			ChatB = Instantiate (ChatPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
			ChatB.transform.SetParent (Camera.main.transform);
			CurrentChat = ChatB.GetComponent<ChatBox> ();
			CurrentChat.setPlayer (this);
			int x = Random.Range (0, GameManager.instance.nom.Count);
			pseudo = GameManager.instance.nom [x];
			GameManager.instance.nom.RemoveAt (x);
		} else
			changeColor (Color.blue);
    }

	public override void OnStartLocalPlayer() {
		changeColor (Color.red);
	}
		
	public void changeColor(Color c) {
		GetComponent<MeshRenderer> ().material.color = c;
	}

	[Command]
    public void CmdSenMsg(string Message)
    {
        //Appel server pour l'envoie du message
        RpcAddMsg(Message);
    }

	[ClientRpc]
    public void RpcAddMsg(string Message)
    {
        //Appel su serveur pour l'ajout du message
        CurrentChat.ChatUpdate(Message);
    }

    void Update() {

		if (!isLocalPlayer)
			return;

		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 2.0f);
        //Raycast pour savoir si on a toucher un joueur bon joueur
        if (Input.GetMouseButtonDown(0) && yourTurn)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Destroy(SelectButton);
                SelectButton = null;
                Pla = hit.transform.gameObject.GetComponent<Player>();
                if (Pla.id == id && Pla != null && isLocalPlayer)
                {
                    SelectButton = Instantiate((GameObject)Resources.Load("PlayerSelect"), new Vector3(0, 0, 0), Quaternion.identity);
                    SelectButton.transform.SetParent(Camera.main.transform);
                    SelectButton.GetComponentInChildren<Text>().text = "Player " + Pla.pseudo;
                    SelectButton.GetComponentInChildren<Button>().onClick.AddListener(selectionPlayer);
                }
            }
        }
    }

    void selectionPlayer()
    {
        //envoie de la sélection au serveur
        Debug.Log("Player " + pseudo + " : is choosen");
        Destroy(SelectButton);
        SelectButton = null;
    }

    public IEnumerator Vote() {
        while(true) {
            yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));

            GameObject playerToVote;

            do
                playerToVote = gm.GetPlayers()[Random.Range(0, gm.GetPlayers().Count)];
            while (id == playerToVote.GetComponent<Player>().ID());

            Vector3 relativePos = playerToVote.transform.position - transform.position;
            targetRotation = Quaternion.LookRotation(relativePos);
        }
    }

    public int ID() {
        return id;
    }
}
