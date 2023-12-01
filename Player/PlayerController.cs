using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JhutenFPP.Manager;
using JhutenFPP.Inventory;

namespace JhutenFPP.PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;
        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField, Range(10, 1000)] private float JumpFactor = 260f;
        [SerializeField] private float Dis2Ground = 0.8f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] private float AirResistance = 0.8f;
        [SerializeField] private GameObject Weapon;
        [SerializeField] private SwordAttack swordAttack;
        private Rigidbody _playerRigidbody;
        private InputControl _inputManager;
        private Animator _animator;
        private bool _grounded = false;
        private bool _hasAnimator;
        private int _xVelHash;
        private int _yVelHash;
        private int _jumpHash;
        private int _groundHash;
        private int _fallingHash;
        private int _zVelHash;
        private int _combatHash;
        private int _attackHash;
        private int _attackTypeHash;
        private int _blockHash;
        //private int _crouchHash;
        private float _xRotation;

        private const float _walkSpeed = 3f;
        private float _runSpeed = 6f;
        private Vector2 _currentVelocity;

        public IInteractable InteractableObj { get; set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }
        public bool IsDead;
        private void Start() {
            _hasAnimator = TryGetComponent<Animator>(out _animator);
            _playerRigidbody = GetComponent<Rigidbody>();
            _inputManager = GetComponent<InputControl>();

            _xVelHash = Animator.StringToHash("X_Velocity");
            _yVelHash = Animator.StringToHash("Y_Velocity");
            _zVelHash = Animator.StringToHash("Z_Velocity");
            _jumpHash = Animator.StringToHash("Jump");
            _groundHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");
            _combatHash = Animator.StringToHash("Combat");
            _attackHash = Animator.StringToHash("Attack");
            _blockHash = Animator.StringToHash("Block");
            _attackTypeHash = Animator.StringToHash("AttackType");
            //_crouchHash = Animator.StringToHash("Crouch");

            IsDead = false;
        }

        private void FixedUpdate() {
            if (!IsDead)
            {
                SampleGround();
                Move();
                HandleJump();
                HandleCombat();
                HandleInteract();
                //HandleCrouch();
            }
        }

        private void LateUpdate() {
            if (CamConditions()) CamMovements();
        }
        private bool CamConditions()
        {
            if (IsDead || _inputManager.Inventory || InventoryManager.Instance.IsChestOpen) return false;
            else return true;
        }
        private void Move()
        {
            if(!_hasAnimator) return;

            float targetSpeed = _inputManager.Run ? _runSpeed : _walkSpeed;
            //if(_inputManager.Crouch) targetSpeed = 1.5f;
            if(_inputManager.Move ==Vector2.zero) targetSpeed = 0;

            if(_grounded)
            {
                
            _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, _inputManager.Move.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
            _currentVelocity.y =  Mathf.Lerp(_currentVelocity.y, _inputManager.Move.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

            var xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
            var zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

            _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0 , zVelDifference)), ForceMode.VelocityChange);
            }
            else
            {
                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(_currentVelocity.x * AirResistance,0,_currentVelocity.y * AirResistance)), ForceMode.VelocityChange);
            }

            if (targetSpeed == _runSpeed)
                Debug.Log(_playerRigidbody.velocity.magnitude);
            _animator.SetFloat(_xVelHash , _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }

        private void CamMovements()
        {
            if(!_hasAnimator) return;

            var Mouse_X = _inputManager.Look.x;
            var Mouse_Y = _inputManager.Look.y;
            Camera.position = CameraRoot.position;
            
            
            _xRotation -= Mouse_Y * MouseSensitivity * Time.smoothDeltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

            Camera.localRotation = Quaternion.Euler(_xRotation, 0 , 0);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0, Mouse_X * MouseSensitivity * Time.smoothDeltaTime, 0));
        }

        //private void HandleCrouch() => _animator.SetBool(_crouchHash , _inputManager.Crouch);


        private void HandleJump()
        {
            if(!_hasAnimator) return;
            if(!_inputManager.Jump) return;
            if(!_grounded) return;
            _animator.SetTrigger(_jumpHash);

            //Enable this if you want B-Hop
            //_playerRigidbody.AddForce(-_playerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
            //_playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
            //_animator.ResetTrigger(_jumpHash);
        }

        public void JumpAddForce()
        {
            //Comment this out if you want B-Hop, otherwise the player will jump twice in the air
            _playerRigidbody.AddForce(-_playerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
            _playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
            _animator.ResetTrigger(_jumpHash);
        }

        public void HandleCombat()
        {
            if (!_hasAnimator) return;
            if (!_grounded) return;
            if (!_inputManager.Unsheathe)
            {
                _animator.SetFloat(_combatHash, 0f);
                _animator.SetFloat(_attackTypeHash, 0f);
                _runSpeed = 6f;
                Weapon.SetActive(false);
                return;
            }
            if (!Weapon.activeInHierarchy)
            {
                Weapon.SetActive(true);
                _runSpeed = 4f;
                _animator.SetFloat(_attackTypeHash, .25f);
            }
            _animator.SetFloat(_combatHash, .8f, .3f, Time.fixedDeltaTime);
            if (_inputManager.Attack)
            {
                swordAttack.InitAttack(GetComponent<PlayerStats>().Damage);
                _animator.SetTrigger(_attackHash);
            }
            if (_inputManager.Block)
            {
                _animator.SetTrigger(_blockHash);
            } else
            {
                _animator.ResetTrigger(_blockHash);
            }
        }
        public void AttackEvent()
        {
            _animator.ResetTrigger(_attackHash);
        }
        private void HandleInteract()
        {
            if (InteractableObj == null) return;
            if (_inputManager.Interact) InteractableObj.InteractWithObj();
        }

        private void SampleGround()
        {
            if(!_hasAnimator) return;

            if (Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out RaycastHit hitInfo, Dis2Ground + 0.1f, GroundCheck))
            {
                //Grounded
                _grounded = true;
                SetAnimationGrounding();
                return;
            }
            //Falling
            _grounded = false;
            _animator.SetFloat(_zVelHash, _playerRigidbody.velocity.y);
            SetAnimationGrounding();
            return;
        }

        private void SetAnimationGrounding()
        {
            _animator.SetBool(_fallingHash, !_grounded);
            _animator.SetBool(_groundHash, _grounded);
        }

        public void SetWeapon(GameObject weapon)
        {
            swordAttack.isEquipped = false;
            Weapon.SetActive(false);
            Weapon = weapon;
            swordAttack = Weapon.GetComponent<SwordAttack>();
            swordAttack.isEquipped = true;
        }


    }
}
