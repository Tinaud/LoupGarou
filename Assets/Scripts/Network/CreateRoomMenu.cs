using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class CreateRoomMenu : MonoBehaviour {

	public Text RoomSizeUI;
	public InputField RoomName;
	public InputField IpAddress;
	private int roomSizeNumber = 2;

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
			IpAddress.transform.parent.gameObject.SetActive (true);
		} else {
			IpAddress.transform.parent.gameObject.SetActive (false);
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
		if (roomSizeNumber <= 2)
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
		string matchName = RoomName.text;
		matchName = Regex.IsMatch (matchName, @"[a-zA-Z0-9_]{3,16}$") ? matchName : RoomName.placeholder.GetComponent<Text>().text;

		int matchSize = roomSizeNumber;
		matchSize = matchSize < 2 ? 2 : matchSize > 20 ? 20 : matchSize;

		string ipAddress = IpAddress.text;
		ipAddress = Regex.IsMatch (ipAddress, @"localhost$|^127(?:\.[0-9]+){0,2}\.[0-9]+$|^(?:0*\:)*?:?0*1$") ? ipAddress : IpAddress.placeholder.GetComponent<Text>().text;

		NetworkManager_Custom.custom_singleton.HostGame (matchName, matchSize, ipAddress);
	}
		
	public void CreateRoomClick() {
		string matchName = RoomName.text;
		matchName = Regex.IsMatch (matchName, @"[a-zA-Z0-9_]{3,16}$") ? matchName : RoomName.placeholder.GetComponent<Text>().text;

		uint matchSize = (uint) roomSizeNumber;
		matchSize = matchSize < 2 ? 2 : matchSize > 20 ? 20 : matchSize;

		NetworkManager_Custom.custom_singleton.CreateRoom (matchName, matchSize);
	}

}
