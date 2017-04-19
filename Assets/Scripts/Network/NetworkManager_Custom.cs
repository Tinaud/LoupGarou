using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

using UnityEngine.UI;
using System.Text.RegularExpressions;

public class NetworkManager_Custom : NetworkManager {

	void Start() {
		if (matchMaker == null)
			StartMatchMaker ();
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
		GameObject player = Instantiate (playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		player.GetComponent<Player> ().changeColor (Color.blue);
		NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
	}

	public void CreateRoom () {
		string matchName = GameObject.Find("InputFieldRoomName").transform.FindChild("Text").GetComponent<Text>().text;
		matchName = Regex.IsMatch(matchName, @"[a-zA-Z0-9_]{3,16}$") ? matchName : GameObject.Find ("InputFieldRoomName").transform.FindChild ("Placeholder").GetComponent<Text> ().text;

		uint matchSize = (uint)int.Parse(GameObject.Find("RoomSizeNumber").transform.FindChild("Text").GetComponent<Text>().text);
		matchSize = matchSize < 8 ? 8 : matchSize > 20 ? 20 : matchSize;

		bool matchAdvertise = true;

		Debug.Log (matchName + ", " + matchSize + ", " + matchAdvertise);
		matchMaker.CreateMatch (matchName, matchSize, matchAdvertise, "", "", "", 0, 0, OnMatchCreate);
	}

	public void GetMatchList() {
		matchMaker.ListMatches (0, 20, "", true, 0, 0, OnMatchList); 
	}

	public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
	{
		if (success && matches != null && matches.Count > 0)
		{
			GameObject roomListContainer = GameObject.Find ("RoomListContainer");
			roomListContainer.transform.DetachChildren();
			GameObject roomListObjT = Instantiate (Resources.Load ("RoomListObj")) as GameObject;

			foreach (MatchInfoSnapshot match in matches) {
				GameObject roomObj = Instantiate (roomListObjT);
				roomObj.transform.SetParent (roomListContainer.transform, false);

				Text[] texts = roomObj.GetComponentsInChildren<Text> ();
				texts [0].text = match.name;
				texts [1].text = match.currentSize + "/" + match.maxSize;

				roomObj.GetComponentInChildren<Button> ().onClick.AddListener (() => matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, OnMatchJoined));
			};
		}
		else if (!success)
		{
			Debug.LogError("List match failed: " + extendedInfo);
		}
	}

	public void JoinGame ( ) {
		NetworkManager.singleton.StartClient ();
	}

	public void HostGame ( ) {
		NetworkManager.singleton.StartHost (connectionConfig, 8);
	}

	public void OnConnected(NetworkMessage msg)
	{
		Debug.Log("Connected!");
	}

}
