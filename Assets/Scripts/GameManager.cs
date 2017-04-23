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



	public class SyncListPlayer : SyncListStruct<PlayerInfo> {} 

	void PlayerChanged(SyncListPlayer.Operation op, int itemIndex)
	{
		Debug.Log("player changed: " + op);
	}


	[SyncVar]
	int nbrPlayersMax;

	[SyncVar]
    int nbrPlayers;

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
		nbrPlayersMax = 3;

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

		playersList.Callback = PlayerChanged;
		wolvesList.Callback = PlayerChanged;
		victimsList.Callback = PlayerChanged;
    }

    void Update () {
        /*if (Input.GetKeyDown(KeyCode.Space) && nbrPlayers < 20 && !gameStarted)
            NewPlayer();*/

		if(nbrPlayers >= nbrPlayersMax && !gameStarted)
			CmdStartGame();
	}

	[Command]
    void CmdStartGame() {
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
                    g.playerRef.RpcUpdateChatB("Villageois");
                    break;
                case "Loup-Garou":
					g.playerRef.gameObject.AddComponent<Loup>();
                    g.playerRef.RpcUpdateChatB("Loup");
                    wolvesList.Add(g);
                    break;
                case "Sorcière":
					g.playerRef.gameObject.AddComponent<Sorciere>();
                    g.playerRef.RpcUpdateChatB("Sorciere");
                    refSorciere = g;
                    break;
                case "Cupidon":
					g.playerRef.gameObject.AddComponent<Cupidon>();
                    g.playerRef.RpcUpdateChatB("Cupidon");
                    refCupidon = g;
                    break;
                case "Chasseur":
					g.playerRef.gameObject.AddComponent<Chasseur>();
                    g.playerRef.RpcUpdateChatB("Chasseur");
                    refChasseur = g;
                    break;
                case "Voyante":
					g.playerRef.gameObject.AddComponent<Voyante>();
                    g.playerRef.RpcUpdateChatB("Voyante");
                    refVoyante = g;
                    break;
                default:
                    Debug.Log("Unknown role :(");
                    break;
            }

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

		playersList.Add(pInfo);
		nbrPlayers++;
		//nbrPlayersMax++;

		RearrangePlayers();

		if (playersList.Count > 1)
			MessageToPlayers (pInfo.ToString() + "is connected.");
	}

    void RearrangePlayers() {
        float angle = 0.0f;

		foreach(PlayerInfo g in playersList) {
            Vector3 pos = new Vector3(10 * Mathf.Cos(angle), 0, 10 * Mathf.Sin(angle));
            Vector3 relativePos = Vector3.zero - pos;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            
            g.playerRef.transform.position = pos;
			g.playerRef.transform.rotation = rotation;

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

	[Command]
	public void CmdRemoveWolf(GameObject _w) {
		PlayerInfo pInfo = new PlayerInfo ();
		pInfo.playerRef = _w.GetComponent<Player> ();
		pInfo.netId = _w.GetComponent<NetworkIdentity> ().netId;

		wolvesList.Remove(pInfo);
    }

    public void AddVictim(GameObject _v) {
		PlayerInfo pInfo = new PlayerInfo ();
		pInfo.playerRef = _v.GetComponent<Player> ();
		pInfo.netId = _v.GetComponent<NetworkIdentity> ().netId;

		victimsList.Add(pInfo);
    }

	[Command]
    public void CmdSaveVictim() {
		victimsList.Clear();
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
                MessageToPlayers("MJ : La sorcière choisi de sauver ou de tuer une personne");
				BaseRole _refSorciere = refSorciere.playerRef.GetComponent<BaseRole>();
				_refSorciere.PlayTurn();
				yield return new WaitUntil(() => _refSorciere.IsReady());
            }

            //FIN DE LA NUIT
			if(victimsList.Count > 0) {
				foreach(PlayerInfo v in victimsList)
                { 
					BaseRole _refVictim = v.playerRef.GetComponent<BaseRole>();
					_refVictim.Die();
					MessageToPlayers("MJ : " + v.playerRef.pseudo + " est retrouvé mort. C'était : " + _refVictim.GetType());
                }
                if (victimsList != null)
				    victimsList.Clear();
            }
            else
            {
                MessageToPlayers("MJ :  Il n'y a aucun mort cette nuit! gg wp.");
                Debug.Log("{MORT} Il n'y a aucun mort cette nuit! gg wp");
            }
                
            Debug.Log(playersList.Count + " " + wolvesList.Count);

            yield return new WaitForSeconds(4f);
            
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
        }
    }
		
    public void MessageToPlayers(string Msg)
	{
		foreach (PlayerInfo p in playersList)
        {
			p.playerRef.RpcAddMsg(Msg);
        }
    }
}