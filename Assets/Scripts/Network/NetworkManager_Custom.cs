using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

using UnityEngine.UI;
using System.Text.RegularExpressions;

public class NetworkManager_Custom : NetworkManager {

	const short CHAT_MESSAGE = 1002;
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

		Player player = playerGO.GetComponent<Player> ();
		NetworkServer.RegisterHandler (MsgType.Highest+2, player.ReadMessage);

		GameManager.instance.AddPlayer (player.gameObject);

		NetworkServer.AddPlayerForConnection (conn, playerGO, playerControllerId);
	}

	public void CreateRoom () {
		string matchName = GameObject.Find("InputFieldRoomName").transform.FindChild("Text").GetComponent<Text>().text;
		matchName = Regex.IsMatch(matchName, @"[a-zA-Z0-9_]{3,16}$") ? matchName : GameObject.Find ("InputFieldRoomName").transform.FindChild ("Placeholder").GetComponent<Text> ().text;

		uint matchSize = (uint)int.Parse(GameObject.Find("RoomSizeNumber").transform.FindChild("Text").GetComponent<Text>().text);
		matchSize = matchSize < 8 ? 8 : matchSize > 20 ? 20 : matchSize;

		bool matchAdvertise = true;

		Debug.Log (matchName + ", " + matchSize + ", " + matchAdvertise);
		NetworkManager.singleton.matchMaker.CreateMatch (matchName, matchSize, matchAdvertise, "", "", "", 0, 0, NetworkManager.singleton.OnMatchCreate);
	}

	public void JoinGame ( ) {
		Debug.Log("Join Game!");
		NetworkManager.singleton.StartClient ();
	}

	public void HostGame ( int maxConn ) {
		maxConnections = maxConn;
		NetworkManager.singleton.StartHost(matchInfo);
	}

}
