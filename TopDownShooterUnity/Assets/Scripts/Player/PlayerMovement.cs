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

    private Joystick _joystick;

    private Vector2 movementVector;

    void Start()
    {
        Application.targetFrameRate = 60;

        _view = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody2D>();
        _joystick = FindObjectOfType<Joystick>();
    }

    private void Update()
    {
        //Player Input
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            movementVector = _joystick.Direction * _playerSpeed * Time.fixedDeltaTime;
        else
            movementVector = new Vector2(Input.GetAxisRaw("Horizontal") * _playerSpeed * Time.fixedDeltaTime, Input.GetAxisRaw("Vertical") * _playerSpeed * Time.fixedDeltaTime);

    }

    void FixedUpdate()
    {
        if (_view.IsMine)
            _rb.velocity = movementVector;
    }
}
