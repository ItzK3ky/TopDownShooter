using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Only meant to sit on player object
/// </summary>
public class PlayerColor : MonoBehaviour
{
    #region Script Description for inspector

    [Header("   SCRIPT DESCRIPTION:   ")]
    [Header("This script takes care of the player \n" +
            "color and synchronizing it.")]
    [Space(20)]

    [SerializeField] private bool iDoNothingLol;

    [Space(20)]

    #endregion

    private PhotonView _view;

    private Color32 redColor;
    private Color32 greenColor;
    private Color32 cyanColor;
    private Color32 yellowColor;
    private Color32 grayColor;

    private void OnEnable()
    {
        _view = GetComponent<PhotonView>();

        if (!_view.IsMine)
            return;

        redColor = new Color32(255, 0, 0, 255);
        greenColor = new Color32(0, 255, 0, 255);
        cyanColor = new Color32(0, 255, 255, 255);
        yellowColor = new Color32(255, 255, 0, 255);
        grayColor = new Color32(150, 150, 150, 255);

        gameObject.GetComponent<SpriteRenderer>().color = GetRandomPlayerColor();
        Color32 playerColor = gameObject.GetComponent<SpriteRenderer>().color;

        Hashtable playerColorRHash = new Hashtable();
        Hashtable playerColorGHash = new Hashtable();
        Hashtable playerColorBHash = new Hashtable();

        playerColorRHash.Add("PlayerColorR", playerColor.r);
        playerColorGHash.Add("PlayerColorG", playerColor.g);
        playerColorBHash.Add("PlayerColorB", playerColor.b);

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerColorRHash);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerColorGHash);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerColorBHash);
    }

    private Color32 GetRandomPlayerColor()
    {
        switch (Random.Range(0, 5))
        {
            case 0:
                return redColor;
            case 1:
                return greenColor;
            case 2:
                return cyanColor;
            case 3:
                return yellowColor;
            case 4:
                return grayColor;
        }

        return new Color32(0, 0, 0, 0);
    }
}
