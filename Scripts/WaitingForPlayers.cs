using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaitingForPlayers : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private float _minY, _maxY, _minX, _maxX;
    [SerializeField] private TextMeshProUGUI _onlinePlayersIndicator;
    [SerializeField] private TextMeshProUGUI _chatBox;

    private PhotonView syncView;
    private bool quitAllowed;

    private void Start()
    {
        syncView = GetComponent<PhotonView>();
        Vector2 spawnPoint = new Vector2(Random.Range(_minY, _maxY), Random.Range(_minX, _maxX));
        PhotonNetwork.Instantiate(_playerPrefab.name, spawnPoint, Quaternion.identity);
        _onlinePlayersIndicator.SetText($"Spieler: {PhotonNetwork.CurrentRoom.PlayerCount}");
        syncView.RPC("OnPlayerJoin", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    public void OnPlayerJoin(string playerName)
    {
        print($"'{playerName}' is joining the game.");
        _onlinePlayersIndicator.SetText($"Spieler: {PhotonNetwork.CurrentRoom.PlayerCount}");
        _chatBox.text += $"\n[<color=\"green\">+</color>] {playerName}";
    }
    [PunRPC]
    public void OnPlayerLeave(string playerName)
    {
        print($"'{playerName}' is leaving the game.");
        _onlinePlayersIndicator.SetText($"Spieler: {PhotonNetwork.CurrentRoom.PlayerCount}");
        _chatBox.text += $"\n[<color=\"red\">-</color>] {playerName}";
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        syncView.RPC("OnPlayerLeave", RpcTarget.All, otherPlayer.NickName);
    }

}
