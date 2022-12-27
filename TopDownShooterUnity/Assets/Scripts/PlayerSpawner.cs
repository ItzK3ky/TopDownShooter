using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;

    private CinemachineVirtualCamera virtualCamera;
    private PhotonPlayerSyncer photonPlayerSyncer;

    private PhotonView photonView; //(Of this GameObject)

    private GameObject spawnedPlayer; //Player, that this script spawns (also player of this client)

    private int amountOfPlayersInRoom = 0;

    void Start()
    {
        //Get Objects From Scene
        virtualCamera = GameObject.FindGameObjectWithTag("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        photonPlayerSyncer = GameObject.FindGameObjectWithTag("Photon Player Syncer").GetComponent<PhotonPlayerSyncer>();
        photonView = GetComponent<PhotonView>();

        StartCoroutine(spawnPlayer());
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
    }

    //Spawn player on Photon Server
    private IEnumerator spawnPlayer()
    {
        yield return new WaitForSecondsRealtime(2f);

        spawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);

        //If masterclient (first to join the room) first assigning playerIndex then increase amountOfPlayersInRoom
        //If not masterclient (first to join the room) first increase amontOfPlayersInRoom then assigning PlayerIndex
        if (PhotonNetwork.IsMasterClient)
        {
            spawnedPlayer.GetComponent<PlayerController>().playerIndex = amountOfPlayersInRoom;
            photonView.RPC("increaseAmountOfPlayersInRoomByOne", RpcTarget.AllBufferedViaServer);
        }
        else { 
            while (true)
            {
                yield return new WaitForSecondsRealtime(2f);
                if (amountOfPlayersInRoom !=0)
                    Debug.Log("rpc arrived");
                break;
            }
            spawnedPlayer.GetComponent<PlayerController>().playerIndex = amountOfPlayersInRoom;
            photonView.RPC("increaseAmountOfPlayersInRoomByOne", RpcTarget.AllBufferedViaServer);
        }

        setupVirtualCameraToFollowPlayer();
        addPlayerToBeSynced();
    }
    
    [PunRPC]
    private void increaseAmountOfPlayersInRoomByOne()
    {
        amountOfPlayersInRoom++;
    }

    private void setupVirtualCameraToFollowPlayer()
    {
        virtualCamera.Follow = spawnedPlayer.transform;
    }

    private void addPlayerToBeSynced()
    {
        photonPlayerSyncer.addGameObjectToSync(spawnedPlayer);
    }
}
