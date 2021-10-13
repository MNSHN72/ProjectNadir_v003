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
            if (playerMovement.inputDirection != 0f && Mathf.Abs(playerMovement.inputDirection) > 0.2f)
            {
                _lookDirection.x = playerMovement.inputDirection;
            }
            else
            {
                _lookDirection.x = playerMovement.neutralDirection;
            }
            ProccessOffset();
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
