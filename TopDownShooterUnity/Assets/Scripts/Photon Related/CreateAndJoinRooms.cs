using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField nameInput;
    [SerializeField] private InputField createInput;
    [SerializeField] private InputField joinInput;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom() 
    {
        PhotonNetwork.LoadLevel("MainScene");
    }

    public void OnNameInputFieldChange()
    {
        PhotonNetwork.LocalPlayer.NickName = nameInput.text;
    }
}
