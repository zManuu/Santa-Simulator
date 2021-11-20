using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class WaitingForPlayers : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private float _minY, _maxY, _minX, _maxX;
    [SerializeField] private TextMeshProUGUI _onlinePlayersIndicator;
    [SerializeField] private TextMeshProUGUI _chatBox;
    [SerializeField] private Transform _playerContainer;

    private PhotonView syncView;

    private void Start()
    {
        syncView = GetComponent<PhotonView>();

        // spawn player
        Vector2 spawnPoint = new Vector2(Random.Range(_minY, _maxY), Random.Range(_minX, _maxX));
        PhotonNetwork.Instantiate(_playerPrefab.name, spawnPoint, Quaternion.identity);

        // emit to all clients
        syncView.RPC("OnPlayerJoin", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    public void OnPlayerJoin(string playerName)
    {
        print($"'{playerName}' is joining the game.");

        /* nametag
        GameObject newPlayerObject = GameObject.FindGameObjectsWithTag("PLAYER").ToList().Find(g => g.transform.parent == null);
        newPlayerObject.transform.parent = _playerContainer;
        newPlayerObject.transform.name = playerName;
        newPlayerObject.GetComponentInChildren<TextMeshProUGUI>().SetText(playerName);*/


        UpdateOnlinePlayersIndicator(); // update scoreboard
        _chatBox.text += $"\n[<color=\"green\">+</color>] {playerName}"; // join message
    }
    [PunRPC]
    public void OnPlayerLeave(string playerName)
    {
        print($"'{playerName}' is leaving the game.");

        UpdateOnlinePlayersIndicator(); // update scoreboard
        _chatBox.text += $"\n[<color=\"red\">-</color>] {playerName}";
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        syncView.RPC("OnPlayerLeave", RpcTarget.All, otherPlayer.NickName);
    }
    private void UpdateOnlinePlayersIndicator()
    {
        StringBuilder builder = new StringBuilder($"<size=0.25>Spieler in Lobby: {PhotonNetwork.CurrentRoom.PlayerCount}</size>\n");
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values.ToArray())
        {
            builder.Append($"\n- {player.NickName}");
        }
        _onlinePlayersIndicator.SetText(builder.ToString());
    }

}
