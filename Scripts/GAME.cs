using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GAME : MonoBehaviourPunCallbacks
{

    [SerializeField] private TextMeshProUGUI _notificationText;
    [SerializeField] private Transform _playerTF;
    [SerializeField] private Transform _gridTF;
    [SerializeField] private TextMeshProUGUI _timeText;

    private int _level;
    private List<Transform> _levels;
    private Transform _levelTR;
    private Transform _treeTF;
    private GameObject _presentContainer;
    private Canvas _treeTooltip;
    private int _timeRemaining;
    private bool _presentDelivered = false;
    private bool _lost = false;
    private PhotonView _syncView;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        LoadLevels();
        StartupMultiplayer();
        StartCoroutine(StartTimeCoroutine());
    }

    #region LOADING

    private void LoadLevels()
    {
        _levels = new List<Transform>();
        for (int i = 0; i < _gridTF.childCount; i++)
        {
            _levels.Add(_gridTF.GetChild(i));
        }
    }
    private void StartupMultiplayer()
    {
        _syncView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            _syncView.RPC("RPC_Loadlevel", RpcTarget.All, Random.Range(0, _levels.Count) + 1);
        }
        _syncView.RPC("RPC_PlayerJoin", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId);
    }

    #endregion

    public void OnEPressed()
    {
        if (_presentDelivered || _lost) return;
        if (Vector2.Distance(_treeTF.position, _playerTF.position) < 1.5f)
        {
            _treeTooltip.gameObject.SetActive(false);
            _presentContainer.SetActive(true);
            ShowNotification("Du hast die Geschenke für diese Familie abgeliefert. Begib dich schnell zum nächsten Haus!");
            _presentDelivered = true;
            StartCoroutine(__OnEPressedCoroutine());
        }
    }
    private IEnumerator __OnEPressedCoroutine()
    {
        yield return new WaitForSeconds(2);
        PlayerPrefs.SetInt("CURRENTLEVEL", _level + 1);
        PhotonNetwork.LoadLevel("Ingame");
    }
    private IEnumerator StartTimeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (PhotonNetwork.IsMasterClient)
            {
                _timeRemaining -= 1;
                //print("This client is the masterclient. Syncing time now.");
                _syncView.RPC("RPC_Synctime", RpcTarget.All, _timeRemaining);
            }
        }
    }
    private IEnumerator __RestartLevel()
    {
        yield return new WaitForSeconds(2);
        PhotonNetwork.LoadLevel("Ingame");
    }

    #region PUN_RPCs


    [PunRPC]
    public void RPC_Synctime(int t)
    {
        print("Incoming time sync request from masterclient: " + t);
        _timeText.SetText($"{t}s");
        if (t == 0)
        {
            /*if (!_lost && !_presentDelivered)
            {
                ShowNotification("Die Zeit ist abgelaufen. Das Level wird gleich neugestartet.");
                _lost = true;
                StartCoroutine(__RestartLevel());
            }*/
        }
        else
        {
            if (!_presentDelivered && !_lost)
            {
                _timeRemaining = t;
            }
        }
    }

    [PunRPC]
    public void RPC_Loadlevel(int level)
    {
        print("Loading the level: " + level);
        _level = level; // '_level' is assigned to the value the master client calculated.
        _levelTR = _levels[_level - 1]; // the level-Transform var beeing assigned (level - 1 !!)
        _levelTR.gameObject.SetActive(true); // the level-Transform is beeing activated
        LevelDataHolder levelData = _levelTR.GetComponent<LevelDataHolder>(); // the levelData of the level-Transform
        Vector2[] possibleTreePoints = levelData.PossibleTreePoints; // the possible spawnpoints for the tree for the specific level
        _treeTF = _levelTR.GetChild(0); // the tree-Transform is beeing assigned (it has to be the first child of the level-Transform!)
        print(possibleTreePoints.Length);
        _treeTF.position = possibleTreePoints[Random.Range(0, possibleTreePoints.Length - 1)]; // the tree is 'teleported' to its random spawnpoint
        _presentContainer = _treeTF.GetChild(1).gameObject; // the present-Container is beeing assigned (it has to be the second child of the tree-Transform!)
        _treeTooltip = _treeTF.GetChild(0).GetComponent<Canvas>(); // the tooltip-Canvas is beeing assigned (it has to be the first child of the tree-Transform!)
        _timeRemaining = levelData.StartTime; // the timeRemaining integer is beeing assigned by the levels custom time limit
        PhotonNetwork.Instantiate(_playerTF.name, levelData.SpawnPoint, Quaternion.identity); // the player is beeing instanciated!
    }

    [PunRPC]
    public void RPC_PlayerJoin(string playerID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            
        }
    }


    #endregion


    #region UTIL FUNCS

    public void ShowNotification(string text) => StartCoroutine(__ShowNotification(text));
    public IEnumerator __ShowNotification(string text)
    {
        _notificationText.SetText(text);
        _notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);
        _notificationText.gameObject.SetActive(false);
    }

    #endregion

}
