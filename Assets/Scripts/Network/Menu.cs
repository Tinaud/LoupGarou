using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	public GameObject mainMenu, createRoomMenu, joinRoomMenu;
	bool isLAN = false;

	public void OnSliderLANOnline() {
		isLAN = !isLAN;
		Debug.Log (isLAN);
	}

	void Start () {
		mainMenu.SetActive(true);
		createRoomMenu.SetActive(false);
		joinRoomMenu.SetActive(false);
	}


	public void CreateGameClick () {
		if (!isLAN) {
			mainMenu.SetActive (false);
			createRoomMenu.SetActive (true);
			joinRoomMenu.SetActive (false);
		} else {
			NetworkManager_Custom.singleton.StartHost ();
		}
	}

	public void JoinGameClick () {
		if (!isLAN) {
			mainMenu.SetActive (false);
			createRoomMenu.SetActive (false);
			joinRoomMenu.SetActive (true);
		} else {
			NetworkManager_Custom.singleton.StartClient ();
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
