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
        if (true) {
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
                if (Pla.id == id)
                {
                    //Resources.Load("enemy")
                    SelectButton = Instantiate((GameObject)Resources.Load("PlayerSelect"), new Vector3(0, 0, 0), Quaternion.identity);
                    SelectButton.transform.SetParent(Camera.main.transform);
                    //UnityEngine.UI.Button button = GameObject.Find("PlayerButton").GetComponent<UnityEngine.UI.Button>();
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
