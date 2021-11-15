using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectNadir
{
    public class PlayerMovement : StateMachine
    {
        #region poperties, fields, etc
        [SerializeField] private float _walkSpeed = 15f;
        [SerializeField] private float _dashSpeed = 50f;
        [SerializeField] private float _jumpHeight = 1f;
        [SerializeField] private float _doubleJumpModifier = 1.1f;
        [SerializeField] private float _gravity = 4f;
        [SerializeField] private float _jumpTime = .1f;
        [SerializeField] private float _dashTime = .1f;
        [SerializeField] private float _dashJumpSpeedModifier = .6f;
        private bool _isGrounded;
        private bool _doubleJumpPossible;
        private bool _airDashPossible;

        //animator bools
        private bool _isWalking = false;


        private PlayerInput _playerInput;
        private CharacterController _characterController;

        public Vector2 inputDirection = Vector2.zero;
        public Vector3 moveDirection = Vector3.zero;
        public Vector3 neutralDirection = Vector3.zero;

        #endregion

        #region get only properties
        public float WalkSpeed { get { return _walkSpeed; } }
        public float DashSpeed { get { return _dashSpeed; } }
        public float JumpHeight { get { return _jumpHeight; } }
        public float DoubleJumpModifier { get { return _doubleJumpModifier; } }
        public float Gravity { get { return _gravity; } }
        public bool IsGrounded { get { return _isGrounded; } }
        public State CurrentState { get { return _currentState; } }
        public float JumpTime { get { return _jumpTime; } }
        public float DashTime { get { return _dashTime; } }
        public float DashJumpSpeedModifier { get { return _dashJumpSpeedModifier; } }
        public bool DoubleJumpPossible { get { return _doubleJumpPossible; } }
        public bool AirDashPossible { get { return _airDashPossible; } }
        public bool IsWalking { get { return _isWalking; } }
        public Vector3 Velocity { get { return _characterController.velocity; } }

        #endregion

        #region set methods
        public void SetDoubleJump(bool inBool)
        {
            _doubleJumpPossible = inBool;
        }
        public void SetAirDash(bool inBool)
        {
            _airDashPossible = inBool;
        }
        #endregion

        #region unity event methods
        private void Awake()
        {
            _playerInput = new PlayerInput();
            _characterController = this.gameObject.GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            _playerInput.PlayerMovement.Move.started += MoveHandler;
            _playerInput.PlayerMovement.Move.performed += MoveHandler;
            _playerInput.PlayerMovement.Move.canceled += MoveHandler;

            _playerInput.PlayerMovement.Jump.started += JumpHandler;

            _playerInput.PlayerMovement.Dash.started += DashHandler;

            _playerInput.PlayerMovement.Melee.started += MeleeHandler;


            _playerInput.PlayerMovement.Enable();
        }
        private void OnDisable()
        {
            _playerInput.PlayerMovement.Move.started -= MoveHandler;
            _playerInput.PlayerMovement.Move.performed -= MoveHandler;
            _playerInput.PlayerMovement.Move.canceled -= MoveHandler;

            _playerInput.PlayerMovement.Jump.started -= JumpHandler;

            _playerInput.PlayerMovement.Dash.started -= DashHandler;

            _playerInput.PlayerMovement.Disable();

            StopAllCoroutines();
        }

        private void Start()
        {
            SetState(new Standard(this));
        }
        private void FixedUpdate()
        {
            _isGrounded = _characterController.isGrounded;

            _currentState.ApplyGravity();
            _currentState.StateManager();

            ProccessNeutralDirection();
            ProccessAnimationParameters();

            _characterController.Move(moveDirection);

        }

        private void ProccessAnimationParameters()
        {
            if (_isGrounded && _characterController.velocity != Vector3.zero)
            {
                _isWalking = true;
            }
            else
            {
                _isWalking = false;
            }
        }
        #endregion

        #region input handlers
        private void MoveHandler(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            inputDirection = context.ReadValue<Vector2>();

            StartCoroutine(_currentState.Walk());
        }
        private void JumpHandler(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            StartCoroutine(_currentState.Jump());
        }
        private void DashHandler(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            StartCoroutine(_currentState.Dash());
        }
        private void MeleeHandler(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            StartCoroutine(_currentState.Melee());
        }
        #endregion

        #region private methods
        private void ProccessNeutralDirection()
        {
            if (inputDirection != Vector2.zero)
            {
                neutralDirection.x = inputDirection.x;
                neutralDirection.z = inputDirection.y;
            }
        }

        #endregion
    }



    #region state classes
    public class Standard : State
    {
        public override IEnumerator Start()
        {
            playerMovement.SetDoubleJump(true);
            playerMovement.SetAirDash(true);
            UpdateMovedirection(playerMovement.WalkSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Walk()
        {
            UpdateMovedirection(playerMovement.WalkSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Jump()
        {
            if (playerMovement.IsGrounded)
            {
                ApplyJumpForce(playerMovement.JumpHeight);
                playerMovement.SetState(new Jumping(playerMovement));
            }

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Dash()
        {
            StartDash();
            playerMovement.SetState(new Dashing(playerMovement));

            yield return new WaitForEndOfFrame();
        }
        public override IEnumerator Melee()
        {
            playerMovement.SetState(new Attack001(playerMovement));
            yield return new WaitForEndOfFrame();
        }

        public override void ApplyGravity()
        {
            YepGravity();
        }

        public Standard(PlayerMovement playerMovement) : base(playerMovement) { }
    }
    public class Jumping : State
    {
        private float _currentJumpTime = 0f;
        public override IEnumerator Start()
        {
            _currentJumpTime = 0;

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Walk()
        {
            UpdateMovedirection(playerMovement.WalkSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Dash()
        {
            if (playerMovement.AirDashPossible)
            {
                playerMovement.SetAirDash(false);
                StartDash();
                playerMovement.SetState(new Dashing(playerMovement));
            }
            yield return new WaitForFixedUpdate();
        }

        public override void StateManager()
        {
            if (_currentJumpTime >= playerMovement.JumpTime)
            {
                playerMovement.SetState(new Airborne(playerMovement));
            }
            _currentJumpTime += Time.fixedDeltaTime;
        }
        public override void ApplyGravity()
        {
            YepGravity();
        }

        public Jumping(PlayerMovement playerMovement) : base(playerMovement) { }
    }
    public class Airborne : State
    {
        public override IEnumerator Start()
        {
            UpdateMovedirection(playerMovement.WalkSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Walk()
        {
            UpdateMovedirection(playerMovement.WalkSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Jump()
        {
            if (playerMovement.DoubleJumpPossible)
            {
                UpdateMovedirection(playerMovement.WalkSpeed);
                playerMovement.SetDoubleJump(false);
                ApplyJumpForce(playerMovement.JumpHeight * playerMovement.DoubleJumpModifier);
                playerMovement.SetState(new Jumping(playerMovement));
            }
            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Dash()
        {
            if (playerMovement.AirDashPossible)
            {
                playerMovement.SetAirDash(false);
                StartDash();
                playerMovement.SetState(new Dashing(playerMovement));
            }
            yield return new WaitForFixedUpdate();
        }

        public override void StateManager()
        {
            if (playerMovement.IsGrounded)
            {
                playerMovement.SetState(new Standard(playerMovement));
            }
        }
        public override void ApplyGravity()
        {
            YepGravity();
        }

        public Airborne(PlayerMovement playerMovement) : base(playerMovement) { }
    }
    public class Dashing : State
    {
        private float _currentDashTime = 0f;

        public override IEnumerator Jump()
        {
            if (playerMovement.IsGrounded)
            {
                ApplyJumpForce(playerMovement.JumpHeight * playerMovement.DashJumpSpeedModifier);
                playerMovement.SetState(new DashJumping(playerMovement));
            }
            else if (playerMovement.DoubleJumpPossible)
            {
                playerMovement.SetDoubleJump(false);
                ApplyJumpForce(playerMovement.JumpHeight);
                playerMovement.SetState(new Jumping(playerMovement));
            }

            yield return new WaitForFixedUpdate();
        }

        public override void StateManager()
        {
            if (_currentDashTime >= playerMovement.DashTime)
            {
                if (playerMovement.IsGrounded)
                {
                    playerMovement.SetState(new Standard(playerMovement));
                }
                else if (playerMovement.IsGrounded == false)
                {
                    playerMovement.SetState(new Airborne(playerMovement));
                }
            }
            _currentDashTime += Time.fixedDeltaTime;
        }

        public Dashing(PlayerMovement playerMovement) : base(playerMovement) { }
    }
    public class DashJumping : State
    {
        private float _currentJumpTime = 0f;
        public override IEnumerator Start()
        {
            _currentJumpTime = 0;

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Walk()
        {
            UpdateMovedirection(playerMovement.DashSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Dash()
        {
            if (playerMovement.AirDashPossible)
            {
                playerMovement.SetAirDash(false);
                StartDash();
                playerMovement.SetState(new Dashing(playerMovement));
            }
            yield return new WaitForFixedUpdate();
        }

        public override void StateManager()
        {
            if (_currentJumpTime >= playerMovement.JumpTime)
            {
                playerMovement.SetState(new Airborne(playerMovement));
            }
            _currentJumpTime += Time.fixedDeltaTime;
        }
        public override void ApplyGravity()
        {
            YepGravity();
        }
        public DashJumping(PlayerMovement playerMovement) : base(playerMovement) { }
    }

    public class Attack001 : State
    {
        public override void ApplyGravity()
        {
            YepGravity();
        }
        public Attack001(PlayerMovement playerMovement) : base(playerMovement) { }
    }
    #endregion
}