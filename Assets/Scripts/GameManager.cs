using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	public struct PlayerInfo {
		public string pseudo;
		public NetworkInstanceId netId;

		public PlayerInfo(NetworkInstanceId _netId,  string _p) {
			netId = _netId;
			pseudo = _p;
		}

		public Player playerRef() {
			return NetworkServer.FindLocalObject (netId).GetComponent<Player>();
		}

		public void setPlayerRef(Player p) {
			Player _p = NetworkServer.FindLocalObject (netId).GetComponent<Player> ();
			_p = p;
		}

		public override string ToString ()
		{
			return pseudo + " (" + netId + ") ";
		}

		public bool Equals (PlayerInfo p)
		{
			return (netId == p.netId) && (pseudo == p.pseudo);
		}
	}

    public enum TurnIssue { NO_VICTIMS, VICTIMS, WITCH, DEAD };

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
    GameObject fireCamp = null;

    [SyncVar]
    public TurnIssue turnIssue = TurnIssue.NO_VICTIMS;

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
	public SyncListPlayer ghostsList = new SyncListPlayer();

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
                    g.playerRef().gameObject.AddComponent<Villageois>();
                    break;
                case "Loup-Garou":
					g.playerRef().gameObject.AddComponent<Loup>();
                    wolvesList.Add(g);
                    break;
                case "Sorcière":
					g.playerRef().gameObject.AddComponent<Sorciere>();
                    refSorciere = g;
                    break;
                case "Cupidon":
					g.playerRef().gameObject.AddComponent<Cupidon>();
                    refCupidon = g;
                    break;
                case "Chasseur":
					g.playerRef().gameObject.AddComponent<Chasseur>();
                    refChasseur = g;
                    break;
                case "Voyante":
					g.playerRef().gameObject.AddComponent<Voyante>();
                    refVoyante = g;
                    break;
                default:
                    Debug.Log("Unknown role :(");
                    break;
            }
			g.playerRef().RpcUpdateChatBRole(g.pseudo + " [" + roles[r] +"]");
            g.playerRef().yourTurn = false;
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


		PlayerInfo pInfo = new PlayerInfo (_p.GetComponent<NetworkIdentity> ().netId, _p.GetComponent<Player> ().pseudo);
		pInfo.playerRef().vote = 0;


        playersList.Add(pInfo);
		nbrPlayers = playersList.Count;

		RearrangePlayers();

		if (nbrPlayers > 1)
			MessageToPlayers (pInfo.ToString() + "is connected.", true);

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

            g.playerRef().RpcUpdatePosition(pos, rotation);

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
			_playerList.Add (_p.playerRef().gameObject);

		return _playerList;
    }

    public bool IsStarted() {
        return gameStarted;
    }

	public void RemovePlayer(GameObject _p) {
		PlayerInfo pInfo = new PlayerInfo (_p.GetComponent<NetworkIdentity> ().netId, _p.GetComponent<Player> ().pseudo);

		playersList.Remove(pInfo);
	}
		
	public void RemoveWolf(GameObject _p) {
		PlayerInfo pInfo = new PlayerInfo (_p.GetComponent<NetworkIdentity> ().netId, _p.GetComponent<Player> ().pseudo);

		wolvesList.Remove(pInfo);
    }
    
    public void RemoveVictims(GameObject _p)
    {
		PlayerInfo pInfo = new PlayerInfo (_p.GetComponent<NetworkIdentity> ().netId, _p.GetComponent<Player> ().pseudo);

        victimsList.Remove(pInfo);
    }

    public void AddVictim(GameObject _p) {
		PlayerInfo pInfo = new PlayerInfo (_p.GetComponent<NetworkIdentity> ().netId, _p.GetComponent<Player> ().pseudo);

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
				if (playersList[i].playerRef().id == pId)
                {
					playersList[i].playerRef().vote--;
                }
        }
        for (int i = 0; i < playersList.Count; i++)
			if (playersList[i].playerRef().id == id) { 
				playersList[i].playerRef().vote++;
            }
    }

    IEnumerator GameTurn() {
        //First Turn only
        yield return new WaitForSeconds(2f);

        //CUPIDON
		if(refCupidon.pseudo != null) {
            MessageToPlayers("MJ : Cupidon choisi deux personnes qui tomberont amoureuse", true);
			BaseRole _refCupidon = refCupidon.playerRef().GetComponent<BaseRole>();
            refCupidon.playerRef().yourTurn = true;
            _refCupidon.PlayTurn();
			yield return new WaitUntil(() => _refCupidon.IsReady());
            refCupidon.playerRef().yourTurn = false;
        }

        bool gameRun = true;

        while(gameRun) {

            //VOYANTE
			if(refVoyante.pseudo != null)
            {
                MessageToPlayers("MJ : La voyante choisi une personne pour connaitre son rôle", true);
				BaseRole _refVoyante = refVoyante.playerRef().GetComponent<BaseRole>();
                refVoyante.playerRef().yourTurn = true;
                _refVoyante.PlayTurn();
				yield return new WaitUntil(() => _refVoyante.IsReady());
                refVoyante.playerRef().yourTurn = false;
            }

            //LOUPS
			if(wolvesList.Count > 0)
            {
                MessageToPlayers("MJ : Les loups choissisent une personnes à tuer", true);
				BaseRole _refWolf = wolvesList[0].playerRef().GetComponent<BaseRole>();
				_refWolf.PlayTurn();
				yield return new WaitUntil(() => _refWolf.IsReady());
            }

            //SORCIÈRE
			if(refSorciere.pseudo != null)
            {
                turnIssue = TurnIssue.WITCH;
				FireForPlayers ();

                MessageToPlayers("MJ : La sorcière choisi de sauver ou de tuer une personne", true);
				BaseRole _refSorciere = refSorciere.playerRef().GetComponent<BaseRole>();
				_refSorciere.PlayTurn();

				yield return new WaitUntil(() => _refSorciere.IsReady());
				refSorciere.playerRef().yourTurn = false;
            }

			yield return new WaitForSeconds(4f);

            //FIN DE LA NUIT
			if(victimsList.Count > 0) {
                turnIssue = TurnIssue.VICTIMS;
				FireForPlayers ();

				foreach (PlayerInfo v in victimsList)
                {
                    BaseRole _refVictim = v.playerRef().GetComponent<BaseRole>();

					if (v.Equals(refSorciere))
						refSorciere.setPlayerRef(null);

					if (v.Equals(refChasseur))
						refChasseur.setPlayerRef(null);

					if (v.Equals(refCupidon))
						refCupidon.setPlayerRef(null);

					if (v.Equals(refVoyante))
						refVoyante.setPlayerRef(null);

					_refVictim.Die();
					v.playerRef ().RpcUpdateChatBRole (v.pseudo + " [Ghost - " + _refVictim.GetType () + "]");
					v.playerRef().RpcChangeFireColor (TurnIssue.DEAD);

					ghostsList.Add (v);

					MessageToPlayers("MJ : " + v.playerRef().pseudo + " est retrouvé mort. C'était : " + _refVictim.GetType(), true);

					if(_refVictim.lover != null) {
						BaseRole _refLover = _refVictim.lover.GetComponent<BaseRole> ();
						Player _refLoverP = _refLover.GetComponent<Player> ();
						Debug.Log("Son amour apporta quelqu'un dans la mort.");

						MessageToPlayers ("MJ : Son amour apporta quelqu'un dans la mort.", true);
						MessageToPlayers("MJ : " + _refLoverP.pseudo + " est retrouvé mort. C'était : " + _refLover.GetType(), true);

						_refVictim.SetLover(null);
						_refLover.SetLover (null);

						_refLover.Die();
						_refLoverP.RpcUpdateChatBRole (_refLoverP.pseudo + " [Ghost - " + _refLover.GetType () + "]");
						_refLoverP.RpcChangeFireColor (TurnIssue.DEAD);

						ghostsList.Add (new PlayerInfo(_refLoverP.GetComponent<NetworkIdentity>().netId, _refLoverP.pseudo));

					}
                }

				victimsList.Clear ();
            }
            else
            {
                MessageToPlayers("MJ :  Il n'y a aucun mort cette nuit! gg wp.", true);
                Debug.Log("{MORT} Il n'y a aucun mort cette nuit! gg wp");
            }

            foreach (PlayerInfo g in playersList)
            {
					g.playerRef().yourTurn = true;
            }

            Debug.Log(playersList.Count + " " + wolvesList.Count);
            Debug.Log("Players: " + playersList.Count + ", Wolves: " + wolvesList.Count);
            
            yield return new WaitForSeconds(30f);

            for (int i = 0; i < playersList.Count; i++)
            {
				Debug.Log(playersList[i].pseudo + " vote: " + playersList[i].playerRef().vote);
            }

            if (wolvesList.Count <= 0) {
                MessageToPlayers("VILLAGEOIS GAGNENT!", true);
                Debug.Log("VILLAGEOIS GAGNENT!");
                gameRun = false;
            }
			else if(playersList.Count == wolvesList.Count) {
                MessageToPlayers("MJ :  LOUPS GAGNENT!", true);
                Debug.Log("LOUPS GAGNENT!");
                gameRun = false;
            }
            
            foreach (PlayerInfo g in playersList)
            {
				g.playerRef().yourTurn = false;
				g.playerRef().prevVote = -1;
            }

            if (turnIssue != TurnIssue.NO_VICTIMS)
            {
                turnIssue = TurnIssue.NO_VICTIMS;
				FireForPlayers ();
            }
        }
    }
		
    public void MessageToPlayers(string Msg, bool massif)
	{
		foreach (PlayerInfo p in playersList)
        {
			//p.playerRef().RpcAddMsg(Msg);

			if(p.playerRef().yourTurn || massif)
				p.playerRef().RpcAddMsg(Msg);
        }

		foreach (PlayerInfo p in ghostsList)
		{
			p.playerRef().RpcAddMsg(Msg);
		}
    }

	public void FireForPlayers()
	{
		foreach (PlayerInfo p in playersList)
		{
			p.playerRef().RpcChangeFireColor(turnIssue);
		}
	}
}