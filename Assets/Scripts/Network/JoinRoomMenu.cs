using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinRoomMenu : MonoBehaviour {

	List<GameObject> roomList = new List<GameObject> ();
	NetworkManager networkManager;

	[SerializeField]
	private Text status = null;

	[SerializeField]
	private GameObject roomListItemPrefab = null;

	[SerializeField]
	private Transform roomListParent = null;

	// Use this for initialization
	public void Setup () {
		networkManager = NetworkManager.singleton;
		if (networkManager.matchMaker == null) {
			networkManager.StartMatchMaker ();
		}

		RefreshRoomList ();
	}

	public void RefreshRoomList(){
		networkManager.matchMaker.ListMatches (0, 20, "", true, 0, 0, OnMatchList); 
		status.text = "Refreshing...";
	}

	public void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> matchList) {
		status.text = "";

		if (!success || matchList == null) {
			status.text = "Couldn't get room list.";
			return;
		}

		ClearRoomList ();
		foreach (MatchInfoSnapshot match in matchList) {
			GameObject _roomListItemGO = Instantiate (roomListItemPrefab);
			_roomListItemGO.transform.SetParent (roomListParent, false);

			RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem> ();
			if (_roomListItem != null)
				_roomListItem.Setup (match, JoinRoom);

			roomList.Add (_roomListItemGO);
		}

		if (roomList.Count == 0) {
			status.text = "No rooms at the moment";
		}

	}

	void ClearRoomList() {
		foreach (GameObject room in roomList)
			Destroy (room);

		roomList.Clear ();
	}
			
	
	public void JoinRoom(MatchInfoSnapshot match) {
		Debug.Log ("joining " + match.name);
		networkManager.matchMaker.JoinMatch (match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
		ClearRoomList ();
		status.text = "Joining...";

	}
}
