using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyHandler : MonoBehaviour
{

    [SerializeField] private GAME _gameClass;

    private void Update()
    {

        if (Down(KeyCode.E))
        {
            _gameClass.OnEPressed();
        }
        
    }


    #region UTIL FUNCS

    bool Down(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }
    bool Pressed(KeyCode key)
    {
        return Input.GetKey(key);
    }
    bool Up(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    #endregion

}
