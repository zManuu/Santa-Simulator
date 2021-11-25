using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System.Linq;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{

    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _createInput;
    [SerializeField] private TMP_InputField _enterInput;

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player-{Random.Range(0, 100)}";
    }

    public void SetName()
    {
        PhotonNetwork.LocalPlayer.NickName = _nameInput.text;
    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(_createInput.text);
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_enterInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Ingame");
    }

}
