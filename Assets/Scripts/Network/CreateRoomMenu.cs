using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomMenu : MonoBehaviour {

	public Text RoomSizeUI;
	private int roomSizeNumber = 8;

	public void Start() {
		RoomSizeUI.text = roomSizeNumber.ToString ();
	}

	public void IncreaseRoomSize() {
		if (roomSizeNumber >= 20)
			return;

		roomSizeNumber++;
		RoomSizeUI.text = roomSizeNumber.ToString ();
	}

	public void DecreaseRoomSize() {
		if (roomSizeNumber <= 8)
			return;

		roomSizeNumber--;
		RoomSizeUI.text = roomSizeNumber.ToString ();
	}

	public void OnMatchNameChange() {
		
	}
		
}
