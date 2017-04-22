using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    static int nextId = 0;
    public int id;
    GameManager gm;
    Quaternion targetRotation;
    public GameObject ChatPrefab;
    GameObject SelectButton;
    GameObject ChatB;
    ChatBox CurrentChat;

    void Start() {
        if (isLocalPlayer) {
			id = nextId++;
			gm = Camera.main.GetComponent<GameManager> ();
            ChatB = Instantiate(ChatPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            ChatB.transform.SetParent(Camera.main.transform);
            CurrentChat = ChatB.GetComponent<ChatBox>();
            CurrentChat.setPlayer(this);
        }
    }

	public override void OnStartLocalPlayer() {
		changeColor (Color.red);
	}
		
	public void changeColor(Color c) {
		GetComponent<MeshRenderer> ().material.color = c;
	}

    public void SenMsg(string Message)
    {
        //Appel server pour l'envoie du message
        AddMsg(Message);
    }

    public void AddMsg(string Message)
    {
        //Appel su serveur pour l'ajout du message
        CurrentChat.ChatUpdate(Message);
    }

    void Update() {
		if (isLocalPlayer) {
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 2.0f);
		}
        //Raycast pour savoir si on a toucher un joueur bon joueur
        if (Input.GetMouseButtonDown(0))
        {
            if (SelectButton != null)
            {
                Destroy(SelectButton);
                SelectButton = null;
            }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Player Pla = hit.transform.gameObject.GetComponent<Player>();
                if (Pla.id == id && Pla != null)
                {
                    SelectButton = Instantiate((GameObject)Resources.Load("PlayerSelect"), new Vector3(0, 0, 0), Quaternion.identity);
                    SelectButton.transform.SetParent(Camera.main.transform);
                    SelectButton.GetComponentInChildren<Text>().text = "Player " + Pla.id;
                    Debug.Log("Player " + id);
                }
            }
        }
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
