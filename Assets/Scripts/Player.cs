﻿using System.Collections;
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
    public int vote;
    [SyncVar]
    public int prevVote = -1;

    [SyncVar]
    public string pseudo;

	GameManager gameManager;

    Quaternion targetRotation;
   	
    GameObject SelectButton;
	ChatBox CurrentChat;

    Player Pla;

	FireLightScript fireCamp;

    [SyncVar]
    public bool yourTurn = false;
    [SyncVar]
    public bool death = false;

    void Start()
    {
        if (!isLocalPlayer)
        {
            PlayerCamera.gameObject.SetActive(false);
            Capsule.GetComponent<MouseLook>().enabled = false;
        }
    }

    public override void OnStartLocalPlayer() {
        PlayerCamera.enabled = true;
        PlayerCamera.tag = "MainCamera";

		CmdSetup ();
		fireCamp = GameObject.Find ("Campfire").GetComponent<FireLightScript> ();

        GameObject ChatB = Instantiate (ChatBoxPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
		ChatB.transform.SetParent (PlayerCamera.transform);
		CurrentChat = ChatB.GetComponent<ChatBox> ();
		CurrentChat.setPlayer (this);
		CurrentChat.WelcomeText.text = "Bienvenue à " + NetworkManager.singleton.matchName;

		Cursor.lockState = CursorLockMode.Locked;
	}
		
	[Command]
	void CmdSetup() {
		id = nextId++;
		gameManager = GameManager.instance;

        RpcChangeColor(Color.red);

        int x = Random.Range (0, gameManager.nom.Count);
		pseudo = gameManager.nom [x];
		gameManager.nom.RemoveAt (x);

		gameManager.AddPlayer (gameObject);
	}

	[ClientRpc]
	public void RpcChangeFireColor(GameManager.TurnIssue issue) 
	{
		if (!isLocalPlayer)
			return;
		
		fireCamp.ChangeColor (issue);
	}

    [ClientRpc]
    public void RpcUpdateChatBRole(string role)
    {
        if (!isLocalPlayer)
            return;
		
        Debug.Log("Nouveau : " + role);
		CurrentChat.RoleText.GetComponent<Text>().text = role;
    }

    [ClientRpc]
    public void RpcChangeColor(Color c)
    {
        if (!isLocalPlayer)
            return;
		
        Capsule.GetComponent<MeshRenderer> ().material.color = c;
		Capsule.transform.Find("Arm").GetComponent<MeshRenderer>().material.color = c;
	}


    [ClientRpc]
    public void RpcUpdatePosition(Vector3 pos, Quaternion rot)
    {
        if (!isLocalPlayer)
            return;

        transform.position = pos;
        transform.rotation = rot;
    }


    [Command]
    public void CmdSendMsg(string Message)
    {
		gameManager.MessageToPlayers(Message);
    }
		
	[ClientRpc]
    public void RpcAddMsg(string Message)
    {
		if (!isLocalPlayer)
			return;

		CurrentChat.ChatUpdate(Message);
    }

    void Update() {

		if (!isLocalPlayer || death)
			return;

		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 2.0f);
        //Raycast pour savoir si on a toucher un joueur bon joueur
		if (Input.GetMouseButtonDown(0))//&& yourTurn)
        {
            RaycastHit hit;
			//Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit) && yourTurn)
            {
                Destroy(SelectButton);
                SelectButton = null;
				Pla = hit.transform.gameObject.GetComponentInParent<Player>();
				if (Pla != null) {
					if (Pla.id != id) {
                        /*SelectButton = Instantiate ((GameObject)Resources.Load ("PlayerSelect"), new Vector3 (0, 0, 0), Quaternion.identity);
						SelectButton.transform.SetParent (PlayerCamera.transform);
						SelectButton.GetComponentInChildren<Text> ().text = "Player " + Pla.pseudo;
						SelectButton.GetComponentInChildren<Button> ().onClick.AddListener (selectionPlayer);*/
                        CmdVote(Pla.id, prevVote);
                        prevVote = Pla.id;
					}
				}
            }
        }
    }

    [Command]
    public void CmdVote(int id, int pV)
    {
        GameManager.instance.Vote(id, pV);
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
