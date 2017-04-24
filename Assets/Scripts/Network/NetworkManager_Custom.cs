using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

using UnityEngine.UI;


public class NetworkManager_Custom : NetworkManager {
		// Singleton 
	public static NetworkManager_Custom custom_singleton = null;
	void Start() {
		if (custom_singleton == null) {
			custom_singleton = this;
		} else if (custom_singleton != this) {
			Destroy (gameObject); 
		}

		if (NetworkManager.singleton.matchMaker == null)
			NetworkManager.singleton.StartMatchMaker ();
	}


	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
		GameObject playerGO = Instantiate (playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;

		NetworkServer.AddPlayerForConnection (conn, playerGO, playerControllerId);
	}

	public void CreateRoom (string matchName, uint matchSize) {

		bool matchAdvertise = true;

		singleton.matchName = matchName;
		singleton.matchSize = matchSize;

		Debug.Log (matchName + ", " + matchSize + ", " + matchAdvertise);
		NetworkManager.singleton.matchMaker.CreateMatch (matchName, matchSize, matchAdvertise, "", "", "", 0, 0, NetworkManager.singleton.OnMatchCreate);
	}

	public void JoinGame ( ) {
		Debug.Log("Join Game!");
		NetworkManager.singleton.StartClient ();
	}

	public void HostGame ( string name, int maxConn, string ipAdd ) {
		singleton.matchName = name;
		singleton.matchSize = (uint) maxConn;
		singleton.maxConnections = maxConn;
		singleton.networkAddress = ipAdd;

		NetworkManager.singleton.StartHost(matchInfo);
	}

}
