using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{

    [SerializeField] private float _characterSpeed;
    [SerializeField] private int _rotateSpeed;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private Vector3 _offsetRotation;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private InputAction _inputActions;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _dashTrailObject;
    [SerializeField] private GameObject _staticRotation;

    private float _groundDistance = 0.4f;
    private bool _isGrounded;
    private int _rotateSpeedHolder;
    private bool _isAimingBow;
    private Vector3 _fixedDirection;
    private Vector3 _offsetDirection;
    private Vector3 _velocity;
    private CharacterCombat _characterCombat;

    private void Start()
    {
        _rotateSpeedHolder = _rotateSpeed;
        _characterCombat = GetComponent<CharacterCombat>();
    }
    private void OnEnable()
    {
        _inputActions.Enable();
    }
    private void OnDisable()
    {
        _inputActions.Disable();
    }
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        
        if(!_isAimingBow)
        {
            Vector2 _direction = _inputActions.ReadValue<Vector2>();
            _fixedDirection = new Vector3(_direction.x, 0f, _direction.y);
            _offsetDirection = new Vector3(0f, 0f, 0f);

            if (_direction.x == 0 && _direction.y < 0)
                _offsetDirection = new Vector3(-1f, 0f, -1f);
            if (_direction.x < 0f && _direction.y < 0f)
                _offsetDirection = new Vector3(-1f, 0f, 0f);
            if (_direction.x < 0 && _direction.y == 0)
                _offsetDirection = new Vector3(-1f, 0f, 1f);
            if (_direction.x < 0f && _direction.y > 0f)
                _offsetDirection = new Vector3(0f, 0f, 1f);
            if (_direction.x == 0 && _direction.y > 0)
                _offsetDirection = new Vector3(1f, 0f, 1f);
            if (_direction.x > 0f && _direction.y > 0f)
                _offsetDirection = new Vector3(1f, 0f, 0f);
            if (_direction.x > 0 && _direction.y == 0)
                _offsetDirection = new Vector3(1f, 0f, -1f);
            if (_direction.x > 0f && _direction.y < 0f)
                _offsetDirection = new Vector3(0f, 0f, -1f);

            if (_characterCombat._isAttacking)
            {
                _offsetDirection = new Vector3(0f, 0f, 0f);
                _rotateSpeed = 0;
            }
            else
            {
                _rotateSpeed = _rotateSpeedHolder;
            }

            if (_fixedDirection.magnitude > 0.2f)
            {
                transform.rotation = Quaternion.Lerp(_staticRotation.transform.rotation, Quaternion.LookRotation(_fixedDirection) * Quaternion.Euler(0f, 45f, 0f), Time.deltaTime * _rotateSpeed);
            }

            _animator.SetFloat("MoveSpeed", _offsetDirection.magnitude);


            if (_offsetDirection.magnitude >= 0.1f)
            {
                _characterController.Move(_offsetDirection * _characterSpeed * Time.deltaTime);
            }

            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

    }


    public Vector3 PFixedDirection { get { return _fixedDirection; } }
    public Vector3 POffsetDirection { get { return _offsetDirection; } }
    public bool PIsAimingBow { get { return _isAimingBow; } set { _isAimingBow = value; } }
}

