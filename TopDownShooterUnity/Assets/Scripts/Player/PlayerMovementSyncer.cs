using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovementSyncer : MonoBehaviour, IPunObservable
{
    private PhotonView _view;

    private Vector3 _receivedPosition;
    private Quaternion _receivedRotation;

    // Start is called before the first frame update
    void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!_view.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, _receivedPosition, Time.deltaTime * PhotonNetwork.SerializationRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, _receivedRotation, Time.deltaTime * PhotonNetwork.SerializationRate);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            _receivedPosition = (Vector3)stream.ReceiveNext();
            _receivedRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
