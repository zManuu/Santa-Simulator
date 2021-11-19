using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GAME : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _notificationText;
    [SerializeField] private Transform _playerTF;
    [SerializeField] private Transform _gridTF;
    [SerializeField] private TextMeshProUGUI _timeText;


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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        LoadLevels();
        LoadLevel();
        StartCoroutine(StartTimeCoroutine());
    }

    #region LOADING

    private void LoadLevels()
    {
        _levels = new List<Transform>();
        for (int i=0; i<_gridTF.childCount; i++)
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
        yield return new WaitForSeconds(3);
        PlayerPrefs.SetInt("CURRENTLEVEL", _level + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private IEnumerator StartTimeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            _timeText.SetText($"{_timeRemaining}s");
            if (_timeRemaining == 0)
            {
                if (!_lost && !_presentDelivered)
                {
                    ShowNotification("Die Zeit ist abgelaufen. Das Level wird gleich neugestartet.");
                    _lost = true;
                    StartCoroutine(__RestartLevel());
                }
            } else
            {
                if (!_presentDelivered && !_lost)
                {
                    _timeRemaining -= 1;
                }
            }
        }
    }
    private IEnumerator __RestartLevel()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

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
