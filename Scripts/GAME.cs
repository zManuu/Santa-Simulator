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
    [SerializeField] private Texture2D _cursorTexture;


    public bool _gamePaused = false;

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
        SetCustomCursor();
        LoadLevels();
        LoadLevel();
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
    private void LoadLevel()
    {
        _level = PlayerPrefs.GetInt("CURRENTLEVEL", 1);
        if (_level >= _levels.Count) _level = _levels.Count;
        _levelTR = _levels[_level - 1];
        _levelTR.gameObject.SetActive(true);
        _treeTF = _levelTR.GetChild(0);
        _presentContainer = _treeTF.GetChild(1).gameObject;
        _treeTooltip = _treeTF.GetChild(0).GetComponent<Canvas>();
        _timeRemaining = _levelTR.GetComponent<LevelDataHolder>().StartTime;
    }
    private void SetCustomCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Vector2 cursorOffset = new Vector2(_cursorTexture.width / 2, _cursorTexture.height / 2);
        Cursor.SetCursor(_cursorTexture, cursorOffset, CursorMode.Auto);
    }
    private void StartupMultiplayer()
    {
        _syncView = GetComponent<PhotonView>();
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
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                _timeRemaining -= 1;
                print("This client is the masterclient. Syncing time now.");
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
        print("Time sync was requested by the masterclient: " + t);
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
