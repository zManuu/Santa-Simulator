using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rigidbody;

    private Vector2 moveDir;

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0)
        {
            moveDir = new Vector2(moveX * _speed, 0);
        }
        else
        {
            moveDir = new Vector2(0, moveY * _speed);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = moveDir;
    }

}
