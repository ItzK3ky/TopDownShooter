using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class RoomListing : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _joinButton;
    [SerializeField] private TMP_Text _roomListingText;

    public string roomName;

    private void Update()
    {
        _roomListingText.text = "\"" + roomName + "\"";
    }

    public void RoomListingPressed()
    {
        if (_joinButton.activeSelf)
            _joinButton.SetActive(false);
        else
            _joinButton.SetActive(true);
    }

    public void JoinButtonPressed()
    { 
        PhotonNetwork.JoinRoom(roomName);
    }
}
