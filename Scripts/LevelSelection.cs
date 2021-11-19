using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{

    [SerializeField] private Transform _arrowTR;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _playerCenterTR;

    private void Update()
    {
        float camDis = _camera.transform.position.y - _arrowTR.position.y;
        Vector3 mouse = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDis));
        float AngleRad = Mathf.Atan2(mouse.y - _arrowTR.position.y, mouse.x - _arrowTR.position.x);
        float angle = (180 / Mathf.PI) * AngleRad;
        //print(angle);
        _playerCenterTR.rotation = Quaternion.Euler(0, 0, angle);
        _arrowTR.localPosition = new Vector2(_playerCenterTR.forward.x * 100, _playerCenterTR.forward.y * 100);
    }

}
