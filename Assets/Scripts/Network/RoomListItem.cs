using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class RoomListItem : MonoBehaviour {

	public delegate void JoinRoomDelegate(MatchInfoSnapshot match);
	private JoinRoomDelegate joinRoomCallback;

	[SerializeField]
	private Text roomNameText;

	[SerializeField]
	private Text roomSizeText;

	[SerializeField]
	private Button joinButton;

	private MatchInfoSnapshot match;

	public void Setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallback) {
		match = _match;
		joinRoomCallback = _joinRoomCallback;

		roomNameText.text = match.name;
		roomSizeText.text = match.currentSize + "/" + match.maxSize;
		joinButton.onClick.AddListener (JoinRoom);
	}

	public void JoinRoom() {
		Debug.Log ("calling something");

		joinRoomCallback.Invoke (match);
	}
}
