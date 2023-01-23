using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInfoSyncer : MonoBehaviour, IPunObservable
{
    private PlayerController _playerController;

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_playerController.health);
        }
        else
        {
            _playerController.health = (int)stream.ReceiveNext();
        }
    }
}
