using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    #region Script Description for inspector

    [Header("   SCRIPT DESCRIPTION:   ")]
    [Header("This script takes care of player \n" +
            "movement")]
    [Space(20)]

    [SerializeField] private bool iDoNothingLol;

    [Space(20)]

    #endregion

    [SerializeField] private float _playerSpeed;

    private PhotonView _view;

    private Rigidbody2D _rb;
    private Joystick joystick;

    void Start()
    {
        _view = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody2D>();
        joystick = FindObjectOfType<Joystick>();
    }

    void FixedUpdate()
    {
        //Player Movement
        if (_view.IsMine)
        {
            Vector2 movementVelocity;
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
                movementVelocity = joystick.Direction * _playerSpeed * Time.deltaTime;
            else
                movementVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * _playerSpeed * Time.fixedDeltaTime, Input.GetAxisRaw("Vertical") * _playerSpeed * Time.deltaTime);

            _rb.velocity = movementVelocity;
        }
    }
}
