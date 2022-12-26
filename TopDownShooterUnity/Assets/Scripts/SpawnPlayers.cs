using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private PhotonPlayerSyncer PhotonPlayerSyncer;

    private int amountOfSpawnedPlayers = 0;

    private GameObject spawnedPlayer;

    void Start()
    {
        //Spawn player (on the photon server)
        StartCoroutine(spawnPlayerOnPhotonNetwork());
    }

    //Executes if another Player joins room
    public override void OnPlayerEnteredRoom(Player player)
    {
        //Only execute following code if this client is master client of room
        if (!PhotonNetwork.IsMasterClient)
            return;

        //Increase amountOfSpawnedPlayers on all clients
        amountOfSpawnedPlayers++;
        photonView.RPC("setAmountOfSpawnedPlayers", RpcTarget.AllBufferedViaServer, amountOfSpawnedPlayers);
    }

    //Spawn player (on the photon server)
    private IEnumerator spawnPlayerOnPhotonNetwork()
    {
        yield return new WaitForSecondsRealtime(2f); //Wait, so client can receive amountOfSpawnedPlayers from Masterclient (incase)

        spawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedPlayer.GetComponent<PlayerController>().playerIndex = amountOfSpawnedPlayers;

        setUpVirtualCameraToFollowPlayer();
        addPlayerToBeSynchronised();
    }

    private void setUpVirtualCameraToFollowPlayer()
    {
        virtualCamera.Follow = spawnedPlayer.transform;
    }

    private void addPlayerToBeSynchronised()
    {
        PhotonPlayerSyncer.addGameObjectToSync(spawnedPlayer);
    }

    [PunRPC]
    private void setAmountOfSpawnedPlayers(int newAmount)
    {
        Debug.Log("Ding ausgeführtet");
        Debug.Log("newAmount: " + newAmount);
        Debug.Log("amountOfPlayersSpawned: " + amountOfSpawnedPlayers);
        if (newAmount > amountOfSpawnedPlayers)
            amountOfSpawnedPlayers = newAmount;
    }

}
