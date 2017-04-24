using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	public struct PlayerInfo {
		public Player playerRef;
		public NetworkInstanceId netId;

		public PlayerInfo(NetworkInstanceId _netId,  Player _p) {
			netId = _netId;
			playerRef = _p;
        }

		public override string ToString ()
		{
			return playerRef.pseudo + " (" + netId + ") ";
		}
	}

    public enum TurnIssue { NO_DEATH, DEATH, WITCH };

	[SyncVar]
    bool gameStarted;

	[SyncVar]
	PlayerInfo refVoyante;
	[SyncVar]
	PlayerInfo refChasseur;
	[SyncVar]
	PlayerInfo refCupidon;
	[SyncVar]
	PlayerInfo refSorciere;

    [SerializeField]
    GameObject fireCamp;

    [SyncVar]
    public TurnIssue turnIssue = TurnIssue.NO_DEATH;

    public class SyncListPlayer : SyncListStruct<PlayerInfo> {} 

	void PlayersListChanged(SyncListPlayer.Operation op, int itemIndex) { Debug.Log("Players List changed: " + op); }
    void WolvesListChanged(SyncListPlayer.Operation op, int itemIndex) { Debug.Log("Wolves List changed: " + op); }
    void VictimsListChanged(SyncListPlayer.Operation op, int itemIndex) { Debug.Log("Victims List changed: " + op); }

    [SyncVar]
	uint nbrPlayersMax;

	[SyncVar]
    uint nbrPlayers;

	public SyncListPlayer playersList = new SyncListPlayer();
	public SyncListPlayer wolvesList = new SyncListPlayer();
	public SyncListPlayer victimsList = new SyncListPlayer();

	public SyncListString nom = new SyncListString();
	SyncListString roles = new SyncListString();

    public static GameManager instance = null;
    void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
    }

	public override void OnStartServer () {
		
		gameStarted = false;
		nbrPlayers = 0;
		nbrPlayersMax = NetworkManager.singleton.matchSize;

		refVoyante = new PlayerInfo ();
		refChasseur = new PlayerInfo ();
		refSorciere = new PlayerInfo ();
		refCupidon = new PlayerInfo ();

		AddRoles();
		nom.Add("Gillian");
		nom.Add("Samuel");
		nom.Add("Mathieu");
		nom.Add("Sharky");
		nom.Add("Facyl");
		nom.Add("Antonio");
		nom.Add("Olimar");
		nom.Add("Lynk");
		nom.Add("Jean-Charles");
		nom.Add("Fran");
		nom.Add("Simon");
		nom.Add("Mage");
		nom.Add("MegaMan");
		nom.Add("Catmeoutsy");
		nom.Add("Howbowdat");

		playersList.Callback = PlayersListChanged;
		wolvesList.Callback = WolvesListChanged;
		victimsList.Callback = VictimsListChanged;
    }

    void Update () {
        /*if (Input.GetKeyDown(KeyCode.Space) && nbrPlayers < 20 && !gameStarted)
            NewPlayer();*/

		/*if(nbrPlayers >= nbrPlayersMax && !gameStarted)
			CmdStartGame();*/
	}
		
    void StartGame() {
		Debug.Log ("Game started");
        gameStarted = true;

        float wolfNumber = Mathf.Floor(nbrPlayers / 3.0f);

        for (int i = 0; i < wolfNumber; i++)
            roles.Add("Loup-Garou");
        for (int i = roles.Count; i < nbrPlayers; i++)
            roles.Add("Villageois");

		foreach (PlayerInfo g in playersList) {
            int r = Random.Range(0, roles.Count);

            switch (roles[r]) {
                case "Villageois":
                    g.playerRef.gameObject.AddComponent<Villageois>();
                    break;
                case "Loup-Garou":
					g.playerRef.gameObject.AddComponent<Loup>();
                    wolvesList.Add(g);
                    break;
                case "Sorcière":
					g.playerRef.gameObject.AddComponent<Sorciere>();
                    refSorciere = g;
                    break;
                case "Cupidon":
					g.playerRef.gameObject.AddComponent<Cupidon>();
                    refCupidon = g;
                    break;
                case "Chasseur":
					g.playerRef.gameObject.AddComponent<Chasseur>();
                    refChasseur = g;
                    break;
                case "Voyante":
					g.playerRef.gameObject.AddComponent<Voyante>();
                    refVoyante = g;
                    break;
                default:
                    Debug.Log("Unknown role :(");
                    break;
            }
			g.playerRef.RpcUpdateChatBRole("[" + roles[r] +"]");
            roles.RemoveAt(r);
        }

        StartCoroutine(GameTurn());
    }

    /*public void NewPlayer() {
        nbrPlayers++;
        GameObject g = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity);
        playerList.Add(g);
        RearrangePlayers();
    }*/

	public void AddPlayer(GameObject _p) {

		PlayerInfo pInfo = new PlayerInfo ();
		pInfo.playerRef = _p.GetComponent<Player> ();
		pInfo.netId = _p.GetComponent<NetworkIdentity> ().netId;
        pInfo.playerRef.vote = 0;

        playersList.Add(pInfo);
		nbrPlayers = playersList.Count;

		RearrangePlayers();

		if (nbrPlayers > 1)
			MessageToPlayers (pInfo.ToString() + "is connected.");

		Debug.Log ("players connected " + nbrPlayers + "/" + nbrPlayersMax);

		if (nbrPlayers == nbrPlayersMax && !gameStarted)
			StartGame ();
	}

    void RearrangePlayers() {
        float angle = 0.0f;
        Vector3 fireCampPos = fireCamp.transform.position;

        foreach (PlayerInfo g in playersList) {
    
            Vector3 pos = new Vector3(fireCampPos.x + 10f * Mathf.Cos(angle), 0, fireCampPos.z + 10f * Mathf.Sin(angle));
            Quaternion rotation = Quaternion.LookRotation(fireCampPos - pos);

            g.playerRef.RpcUpdatePosition(pos, rotation);

            angle += (2 * Mathf.PI) / playersList.Count;
        }

    }

    void AddRoles() {
        roles.Add("Sorcière");
        roles.Add("Voyante");
        roles.Add("Cupidon");
        roles.Add("Chasseur");
    }

	public List<GameObject> GetPlayers() {
		List<GameObject> _playerList = new List<GameObject> ();

		foreach (PlayerInfo _p in playersList)
			_playerList.Add (_p.playerRef.gameObject);

		return _playerList;
    }

    public bool IsStarted() {
        return gameStarted;
    }

	public void RemovePlayer(GameObject _w) {
		PlayerInfo pInfo = new PlayerInfo ();
		pInfo.playerRef = _w.GetComponent<Player> ();
		pInfo.netId = _w.GetComponent<NetworkIdentity> ().netId;

		playersList.Remove(pInfo);
	}
		
	public void RemoveWolf(GameObject _w) {
		PlayerInfo pInfo = new PlayerInfo ();
		pInfo.playerRef = _w.GetComponent<Player> ();
		pInfo.netId = _w.GetComponent<NetworkIdentity> ().netId;

		wolvesList.Remove(pInfo);
    }
    
    public void RemoveVictims(GameObject _v)
    {
        PlayerInfo pInfo = new PlayerInfo();
        pInfo.playerRef = _v.GetComponent<Player>();
        pInfo.netId = _v.GetComponent<NetworkIdentity>().netId;

        victimsList.Remove(pInfo);
    }

    public void AddVictim(GameObject _v) {
		PlayerInfo pInfo = new PlayerInfo ();
		pInfo.playerRef = _v.GetComponent<Player> ();
		pInfo.netId = _v.GetComponent<NetworkIdentity> ().netId;

		victimsList.Add(pInfo);
    }

    public void SaveVictim() {
        victimsList.Clear();
    }

    public void Vote(int id, int pId)
    {
       if (pId != -1)
        {
            for (int i = 0; i < playersList.Count; i++)
                if (playersList[i].playerRef.id == pId)
                {
                    playersList[i].playerRef.vote--;
                    MessageToPlayers(playersList[i].playerRef.pseudo + "Est choisi");
                }
        }
        for (int i = 0; i < playersList.Count; i++)
            if (playersList[i].playerRef.id == id) { 
                playersList[i].playerRef.vote++;
                MessageToPlayers(playersList[i].playerRef.pseudo + "Est choisi");
            }
    }

    IEnumerator GameTurn() {
        //First Turn only
        yield return new WaitForSeconds(2f);

        //CUPIDON
		if(refCupidon.playerRef != null) {
            MessageToPlayers("MJ : Cupidon choisi deux personnes qui tomberont amoureuse");
			BaseRole _refCupidon = refCupidon.playerRef.GetComponent<BaseRole>();
            refCupidon.playerRef.yourTurn = true;
            _refCupidon.PlayTurn();
			yield return new WaitUntil(() => _refCupidon.IsReady());
            refCupidon.playerRef.yourTurn = false;
        }

        bool gameRun = true;

        while(gameRun) {

            //VOYANTE
			if(refVoyante.playerRef != null)
            {
                MessageToPlayers("MJ : La voyante choisi une personne pour connaitre son rôle");
				BaseRole _refVoyante = refVoyante.playerRef.GetComponent<BaseRole>();
                refVoyante.playerRef.yourTurn = true;
                _refVoyante.PlayTurn();
				yield return new WaitUntil(() => _refVoyante.IsReady());
                refVoyante.playerRef.yourTurn = false;
            }

            //LOUPS
			if(wolvesList.Count > 0)
            {
                MessageToPlayers("MJ : Les loups choissisent une personnes à tuer");
				BaseRole _refWolf = wolvesList[0].playerRef.GetComponent<BaseRole>();
				_refWolf.PlayTurn();
				yield return new WaitUntil(() => _refWolf.IsReady());
            }

            //SORCIÈRE
			if(refSorciere.playerRef != null)
            {
                turnIssue = TurnIssue.WITCH;
                fireCamp.GetComponent<FireLightScript>().RpcChangeColor();
                MessageToPlayers("MJ : La sorcière choisi de sauver ou de tuer une personne");
				BaseRole _refSorciere = refSorciere.playerRef.GetComponent<BaseRole>();
                refSorciere.playerRef.yourTurn = true;
                _refSorciere.PlayTurn();
				yield return new WaitUntil(() => _refSorciere.IsReady());
                refSorciere.playerRef.yourTurn = false;
            }

            //FIN DE LA NUIT
			if(victimsList.Count > 0) {
                turnIssue = TurnIssue.DEATH;
                fireCamp.GetComponent<FireLightScript>().RpcChangeColor();
                int i;
                foreach (PlayerInfo v in victimsList)
                {
                    BaseRole _refVictim = v.playerRef.GetComponent<BaseRole>();
					_refVictim.Die();
					MessageToPlayers("MJ : " + v.playerRef.pseudo + " est retrouvé mort. C'était : " + _refVictim.GetType());
                }

                SaveVictim();
            }
            else
            foreach (PlayerInfo g in playersList)
            {
                g.playerRef.yourTurn = true;
            }

            {
                MessageToPlayers("MJ :  Il n'y a aucun mort cette nuit! gg wp.");
                Debug.Log("{MORT} Il n'y a aucun mort cette nuit! gg wp");
            }

            Debug.Log(playersList.Count + " " + wolvesList.Count);
            Debug.Log("Players: " + playersList.Count + ", Wolves: " + wolvesList.Count);
            
            yield return new WaitForSeconds(30f);

            for (int i = 0; i < playersList.Count; i++)
            {
                Debug.Log(playersList[i].playerRef.pseudo + " vote: " + playersList[i].playerRef.vote);
            }

            if (wolvesList.Count <= 0) {
                MessageToPlayers("VILLAGEOIS GAGNENT!");
                Debug.Log("VILLAGEOIS GAGNENT!");
                gameRun = false;
            }
			else if(playersList.Count == wolvesList.Count) {
                MessageToPlayers("MJ :  LOUPS GAGNENT!");
                Debug.Log("LOUPS GAGNENT!");
                gameRun = false;
            }
            
            foreach (PlayerInfo g in playersList)
            {
                g.playerRef.yourTurn = false;
                g.playerRef.prevVote = -1;
            }

            if (turnIssue != TurnIssue.NO_DEATH)
            {
                turnIssue = TurnIssue.NO_DEATH;
                fireCamp.GetComponent<FireLightScript>().RpcChangeColor();
            }
        }
    }
		
    public void MessageToPlayers(string Msg)
	{
		foreach (PlayerInfo p in playersList)
        {
            if(p.playerRef.yourTurn)
                p.playerRef.RpcAddMsg(Msg);
        }
    }
}