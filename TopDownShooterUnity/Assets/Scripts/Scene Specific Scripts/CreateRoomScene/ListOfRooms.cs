using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ListOfRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _roomListing;
    [SerializeField] private Transform _panelTransform;

    private List<GameObject> _roomListings = new List<GameObject>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.IsOpen)
                AddNewRoomListing(roomInfo.Name);
            else
                RemoveRoomListing(roomInfo.Name);
        }
    }

    private void AddNewRoomListing(string roomName)
    {
        GameObject newListing = Instantiate(_roomListing, _panelTransform);
        newListing.GetComponent<RoomListing>().roomName = roomName;
        _roomListings.Add(newListing);
    }
    private void RemoveRoomListing(string roomName)
    {
        foreach (GameObject roomListing in _roomListings)
        {
            if (roomListing.GetComponent<RoomListing>().roomName == roomName)
                Destroy(roomListing);
        }
    }
}
