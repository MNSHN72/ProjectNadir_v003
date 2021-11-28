using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectNadir
{
    public class PlayerMovement : StateMachine
    {
        #region poperties, fields, etc

        //basic movement
        [SerializeField] private Transform _playerModel;
        [SerializeField] private float _walkSpeed = 15f;
        [SerializeField] private float _dashSpeed = 50f;
        [SerializeField] private float _jumpHeight = 1f;
        [SerializeField] private float _doubleJumpModifier = 1f;
        [SerializeField] private float _gravity = 3f;
        [SerializeField] private float _jumpTime = .1f;
        [SerializeField] private float _dashTime = .1f;
        [SerializeField] private float _dashJumpSpeedModifier = 1f;

        private bool _isGrounded;
        private bool _doubleJumpPossible;
        private bool _airDashPossible;
        private bool _updateLookDirection = true;

        //animator parameters
        private bool _isWalking = false;


        private Animator _animator;
        private PlayerInput _playerInput;
        private CharacterController _characterController;


        [SerializeField]private Vector2 _inputDirection = Vector2.zero;
        [SerializeField]private Vector3 _neutralDirection = Vector3.zero;

        //needs to be public because it's being modifed by state classes
        public Vector3 moveDirection = Vector3.zero;

        //ledge detection
        [SerializeField] private Transform _ledgeDetector;

        private Ray _ledgeDetectionRay = new Ray();
        private Vector3 _ledgeDetectionRayDirection = new Vector3(0, 0, 0);
        [SerializeField] private float _rayLength = 1.5f;
        [SerializeField] private float _rayAngle = -4f;
        [SerializeField] private float _ledgeDetectionRadius = .5f;

        private Ray _dashLedgeDetectionRay = new Ray();
        private Vector3 _dashLedgeDetectionRayDirection = new Vector3(0, 0, 0);

        [SerializeField] private float _dashRayLength = 1.5f;
        [SerializeField] private float _dashRayAngle = -4f;
        [SerializeField] private float _dashLedgeDetectionRadius = .5f;








        #endregion

        #region C# events
        public delegate void AnimationExitHandler();
        public event AnimationExitHandler OnAnimationEnd;
        #endregion

        #region get only properties
        public Vector3 NeutralDirection { get { return _neutralDirection; } }
        public Vector2 InputDirection { get { return _inputDirection; } }
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
        public bool UpdateLookDirection { get { return _updateLookDirection; } }
        public Vector3 Velocity { get { return _characterController.velocity; } }



        public Vector3 LedgeDetectorP { get { return _ledgeDetector.transform.position; } }



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
        public void SetLookUpdate(bool inBool) 
        {
            _updateLookDirection = inBool;
        }
        #endregion

        #region unity event methods
        private void Awake()
        {
            _playerInput = new PlayerInput();

            _characterController = this.gameObject.GetComponent<CharacterController>();
            _animator = this.GetComponent<Animator>();
        }
        private void OnEnable()
        {
            _playerInput.PlayerMovement.Move.started += MoveHandler;
            _playerInput.PlayerMovement.Move.performed += MoveHandler;
            _playerInput.PlayerMovement.Move.canceled += MoveHandler;

            _playerInput.PlayerMovement.Jump.started += JumpHandler;

            _playerInput.PlayerMovement.Dash.started += DashHandler;

            _playerInput.PlayerMovement.Attack.started += AttackHandler;


            _playerInput.PlayerMovement.Enable();

            OnAnimationEnd += () => { SetState(new Standard(this)); };
        }
        private void OnDisable()
        {
            _playerInput.PlayerMovement.Move.started -= MoveHandler;
            _playerInput.PlayerMovement.Move.performed -= MoveHandler;
            _playerInput.PlayerMovement.Move.canceled -= MoveHandler;

            _playerInput.PlayerMovement.Jump.started -= JumpHandler;

            _playerInput.PlayerMovement.Dash.started -= DashHandler;

            _playerInput.PlayerMovement.Disable();

            OnAnimationEnd = null;
            StopAllCoroutines();
        }
        private void Start()
        {
            SetState(new Standard(this));
        }
        private void FixedUpdate()
        {
            ProccessNeutralDirection();
            ProccessLedgeDetection();

            if (_updateLookDirection == true)
            {
                _playerModel.rotation = Quaternion.LookRotation(-1f * _neutralDirection, Vector3.up); 
            }


            _currentState.ApplyGravity();
            _currentState.StateManager();

            _characterController.Move(moveDirection);

            //placeholder as af


            _isGrounded = _characterController.isGrounded;
            ProccessAnimationParameters();
            _animator.SetBool("IsWalking", IsWalking);
            _animator.SetFloat("Speed", Mathf.Abs(Velocity.x));
            Debug.DrawRay(_ledgeDetector.transform.position, _ledgeDetectionRayDirection, Color.red);
            Debug.DrawRay
                (_ledgeDetector.transform.position, new Vector3(NeutralDirection.normalized.x, -4f, NeutralDirection.normalized.z), Color.red);
        }


        #endregion

        #region input handlers
        private void MoveHandler(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _inputDirection = context.ReadValue<Vector2>();
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
        private void AttackHandler(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _animator.SetTrigger("Attack");
            StartCoroutine(_currentState.Attack());
        }
        #endregion

        #region private methods
        private void ProccessLedgeDetection()
        {
            _ledgeDetectionRayDirection = new Vector3(_neutralDirection.normalized.x, _rayAngle, _neutralDirection.normalized.z);
            _ledgeDetectionRay = new Ray(_ledgeDetector.transform.position, _ledgeDetectionRayDirection);
        }
        private void ProccessNeutralDirection()
        {
            if (_inputDirection != Vector2.zero)
            {
                _neutralDirection.x = _inputDirection.x;
                _neutralDirection.y = 0f;
                _neutralDirection.z = _inputDirection.y;
            }
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

        #region public methods

        public void ExitAnimation()
        {
            OnAnimationEnd?.Invoke();
        }
        public bool LedgeDetection() 
        {
            return Physics.SphereCast(_ledgeDetectionRay, _ledgeDetectionRadius, _rayLength);
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
            UpdateMoveDirection(playerMovement.WalkSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Walk()
        {
            UpdateMoveDirection(playerMovement.WalkSpeed);

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
            playerMovement.SetLookUpdate(false);
            playerMovement.SetState(new Dashing(playerMovement));

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Attack()
        {
            playerMovement.SetState(new Attack001(playerMovement));
            yield return new WaitForEndOfFrame();
        }

        public override void StateManager()
        {
            if (playerMovement.IsGrounded && playerMovement.LedgeDetection() == false
                &&playerMovement.Velocity.magnitude>6.5f)
            {
                if (playerMovement.InputDirection != Vector2.zero)
                {
                    Debug.Log("ledgejump");
                    ApplyJumpForce(.3f);
                    playerMovement.SetState(new Airborne(playerMovement)); 
                }
            }
            if (playerMovement.IsGrounded == false)
            {
                playerMovement.SetState(new Airborne(playerMovement));
            }
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
            UpdateMoveDirection(playerMovement.WalkSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Dash()
        {
            if (playerMovement.AirDashPossible)
            {
                playerMovement.SetAirDash(false);
                playerMovement.SetLookUpdate(false);
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
            UpdateMoveDirection(playerMovement.WalkSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Walk()
        {
            UpdateMoveDirection(playerMovement.WalkSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Jump()
        {
            if (playerMovement.DoubleJumpPossible)
            {
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
                Debug.Log("airdash");
                playerMovement.SetAirDash(false);
                playerMovement.SetLookUpdate(false);
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

        public override IEnumerator Start()
        {
            StartDash();
            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Jump()
        {
            if (playerMovement.IsGrounded)
            {
                ApplyJumpForce(playerMovement.JumpHeight * playerMovement.DashJumpSpeedModifier);
                playerMovement.SetLookUpdate(true);
                playerMovement.SetState(new Jumping(playerMovement));
            }
            else if (playerMovement.DoubleJumpPossible)
            {
                playerMovement.SetDoubleJump(false);
                ApplyJumpForce(playerMovement.JumpHeight);
                playerMovement.SetLookUpdate(true);
                playerMovement.SetState(new Jumping(playerMovement));
            }

            yield return new WaitForFixedUpdate();
        }

        public override void StateManager()
        {
            //handles ledge jump
            if (playerMovement.IsGrounded && 
                Physics.SphereCast
                (new Ray(playerMovement.LedgeDetectorP,new Vector3(playerMovement.NeutralDirection.normalized.x,-4f,playerMovement.NeutralDirection.normalized.z)),
                .43f,
                5f)== false)
            {
                Debug.Log("Dash ledgejump");
                ApplyJumpForce(playerMovement.JumpHeight * playerMovement.DashJumpSpeedModifier);
                playerMovement.SetLookUpdate(true);
                playerMovement.SetState(new Jumping(playerMovement));
            }

            //exits state when dash time is over
            if (_currentDashTime >= playerMovement.DashTime)
            {
                if (playerMovement.IsGrounded)
                {
                    playerMovement.SetLookUpdate(true);
                    playerMovement.SetState(new Standard(playerMovement));
                }
                else if (playerMovement.IsGrounded == false)
                {
                    playerMovement.SetLookUpdate(true);
                    playerMovement.SetState(new Airborne(playerMovement));
                }
            }
            _currentDashTime += Time.fixedDeltaTime;
        }

        public override void ApplyGravity()
        {
            if (playerMovement.IsGrounded == true)
            {
                YepGravity();
            }
        }

        public Dashing(PlayerMovement playerMovement) : base(playerMovement) { }
    }

    public class Attack001 : State
    {
        public override IEnumerator Start()
        {
            yield return new WaitForFixedUpdate();
        }
        public override IEnumerator Walk()
        {
            UpdateMoveDirection(playerMovement.WalkSpeed);

            yield return new WaitForFixedUpdate();
        }
        public override void ApplyGravity()
        {
            YepGravity();
        }
        public Attack001(PlayerMovement playerMovement) : base(playerMovement) { }
    }
    #endregion
}