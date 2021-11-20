using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{

    [SerializeField] private GameObject _playerPrefab;

    private void Start()
    {
        PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);
    }

}
