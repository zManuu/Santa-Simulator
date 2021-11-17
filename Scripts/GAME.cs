using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GAME : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _notificationText;
    [SerializeField] private Canvas _treeTooltip;
    [SerializeField] private GameObject _presentContainer;

    public bool _gamePaused = false;
    public Transform _treeTF;
    public Transform _playerTF;
    public bool _presentDelivered = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnEPressed()
    {
        if (_presentDelivered) return;
        if (Vector2.Distance(_treeTF.position, _playerTF.position) < 1.5f)
        {
            _treeTooltip.gameObject.SetActive(false);
            _presentContainer.SetActive(true);
            ShowNotification("Du hast die Geschenke für diese Familie abgeliefert. Begib dich schnell zum nächsten Haus!");
            _presentDelivered = true;
        }
    }


    public void ShowNotification(string text) => StartCoroutine(__ShowNotification(text));
    public IEnumerator __ShowNotification(string text)
    {
        _notificationText.SetText(text);
        _notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);
        _notificationText.gameObject.SetActive(false);
    }

}
