using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectNadir
{
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private float _lookOffset = 1f;
        private Vector3 _lookDirection = Vector3.zero;

        private Animator _animator;


        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
        }
        private void FixedUpdate()
        {
            ProccessLookDirection();
            gameObject.transform.rotation = Quaternion.LookRotation(-_lookDirection, Vector3.up);

            _animator.SetBool("IsWalking", playerMovement.IsWalking);
            _animator.SetFloat("Speed", Mathf.Abs(playerMovement.Velocity.x));
        }
        private void ProccessLookDirection()
        {
            if (playerMovement.inputDirection != Vector2.zero)
            {
                _lookDirection.x = playerMovement.inputDirection.x;
                _lookDirection.z = playerMovement.inputDirection.y;
            }
        }
        private void ProccessOffset()
        {
            if (_lookDirection.x > 0)
            {
                _lookDirection.z = -_lookDirection.x * _lookOffset;
            }
            else if (_lookDirection.x < 0)
            {
                _lookDirection.z = _lookDirection.x * _lookOffset;
            }
        }
    }
}
