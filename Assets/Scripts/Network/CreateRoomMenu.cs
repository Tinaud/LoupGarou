using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomMenu : MonoBehaviour {

	public Text RoomSizeUI;
	public InputField RoomName;
	private int roomSizeNumber = 8;

	public int RoomSizeNumber {
		get { return roomSizeNumber; }
	}

	public GameObject[] HostButtons = new GameObject[2];

	bool isLAN = false;

	public void OnSliderLANOnline(bool lan)
	{
		isLAN = lan;
		if (isLAN) {
			HostButtons [0].SetActive (false);
			HostButtons [1].SetActive (true);
		} else {
			HostButtons [1].SetActive (false);
			HostButtons [0].SetActive (true);
		}
	}

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

	public void OnInputChange() {
		if (!isLAN) {
			if (RoomName.contentType != InputField.ContentType.Alphanumeric)
				RoomName.textComponent.color = Color.red;
			else
				RoomName.textComponent.color = Color.black;
		}
	}

	public void HostGameClick() {
		NetworkManager_Custom.custom_singleton.HostGame (roomSizeNumber);
	}
		
}
