using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class Player : NetworkBehaviour {
	
	[SerializeField]
	GameObject ChatBoxPrefab = null;

	[SerializeField]
	GameObject Capsule = null;

	[SerializeField]
	Camera PlayerCamera = null;

    static int nextId = 0;

	[SyncVar]
    public int id;

	[SyncVar]
    public string pseudo;

	GameManager gameManager;

    Quaternion targetRotation;
   	
    GameObject SelectButton;
	ChatBox CurrentChat;

    Player Pla;

    [SyncVar]
    public bool yourTurn = false;

	public override void OnStartLocalPlayer() {

		changeColor (Color.red);
		CmdSetup ();

		GameObject ChatB = Instantiate (ChatBoxPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
		ChatB.transform.SetParent (PlayerCamera.transform);
		CurrentChat = ChatB.GetComponent<ChatBox> ();
		CurrentChat.setPlayer (this);

		Cursor.lockState = CursorLockMode.Locked;
	}

	public override void OnStartClient() {
		
	}

	[Command]
	void CmdSetup() {
		id = nextId++;
		gameManager = GameManager.instance;

		int x = Random.Range (0, gameManager.nom.Count);
		pseudo = gameManager.nom [x];
		gameManager.nom.RemoveAt (x);

		gameManager.AddPlayer (gameObject);
	}

    [ClientRpc]
    public void RpcUpdateChatB(string role)
    {
        if (!isLocalPlayer)
            return;
        Debug.Log("Nouveau : " + role);
        GameObject.Find("Role").GetComponent<Text>().text = role;
    }

    void Start() {
		if (!isLocalPlayer) {
			PlayerCamera.enabled = false;
			Capsule.GetComponent<MouseLook> ().enabled = false;
		}
    }

    [ClientRpc]
    public void RpcChangeColor(Color c)
    {
        /*if (!isLocalPlayer)
            return;*/
        GetComponent<MeshRenderer> ().material.color = c;
	}

    [Command]
    public void CmdVictims(GameObject selectedPlayer)
    {
        if (!isLocalPlayer)
            return;
        //Appel server pour l'envoie de la victime
        gameManager.AddVictim(selectedPlayer);
    }



	[Command]
    public void CmdSendMsg(string Message)
    {
        //Appel server pour l'envoie du message
		gameManager.MessageToPlayers(Message);
    }
		
	[ClientRpc]
    public void RpcAddMsg(string Message)
    {
        //Appel su serveur pour l'ajout du message
		if (isLocalPlayer)
            CurrentChat.ChatUpdate(Message);
    }

    void Update() {

		if (!isLocalPlayer)
			return;

		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 2.0f);
        //Raycast pour savoir si on a toucher un joueur bon joueur
		if (Input.GetMouseButtonDown(0))//&& yourTurn)
        {
            RaycastHit hit;
			//Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit))
            {
                Destroy(SelectButton);
                SelectButton = null;
				Pla = hit.transform.gameObject.GetComponentInParent<Player>();
				if (Pla != null) {
					if (Pla.id != id) {
						SelectButton = Instantiate ((GameObject)Resources.Load ("PlayerSelect"), new Vector3 (0, 0, 0), Quaternion.identity);
						SelectButton.transform.SetParent (PlayerCamera.transform);
						SelectButton.GetComponentInChildren<Text> ().text = "Player " + Pla.pseudo;
						SelectButton.GetComponentInChildren<Button> ().onClick.AddListener (selectionPlayer);
					}
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
				playerToVote = gameManager.GetPlayers()[Random.Range(0, gameManager.GetPlayers().Count)];
			while (id == playerToVote.GetComponent<Player>().ID());

            Vector3 relativePos = playerToVote.transform.position - transform.position;
            targetRotation = Quaternion.LookRotation(relativePos);
        }
    }

    public int ID() {
        return id;
    }
}
