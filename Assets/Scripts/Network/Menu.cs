using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Menu : MonoBehaviour {

	[SerializeField]
	SkyManager skyManager = null;

	public GameObject mainMenu, createRoomMenu, joinRoomMenu;
	bool isLAN = false;

	public void OnSliderLANOnline() {
		isLAN = !isLAN;
		createRoomMenu.GetComponent<CreateRoomMenu> ().OnSliderLANOnline (isLAN);
		if (isLAN)
			NetworkManager.singleton.StopMatchMaker ();
		else
			NetworkManager.singleton.StartMatchMaker ();
	}

	void Start () {
		StartCoroutine (skyManager.SwitchTime ());
		mainMenu.SetActive(true);
		createRoomMenu.SetActive(false);
		joinRoomMenu.SetActive(false);
	}


	public void CreateGameClick () {
		mainMenu.SetActive (false);
		createRoomMenu.SetActive (true);
		joinRoomMenu.SetActive (false);
	}


	public void JoinGameClick () {
		if (!isLAN) {
			mainMenu.SetActive (false);
			createRoomMenu.SetActive (false);
			joinRoomMenu.SetActive (true);
			joinRoomMenu.GetComponent<JoinRoomMenu> ().Setup ();
		} else {
			NetworkManager_Custom.custom_singleton.JoinGame ();
		}
	}

	public void BackToMenuClick () {
		mainMenu.SetActive(true);
		createRoomMenu.SetActive(false);
		joinRoomMenu.SetActive(false);
	}
		
	public void QuitGameClick () {
		Application.Quit ();
	}
		
}
