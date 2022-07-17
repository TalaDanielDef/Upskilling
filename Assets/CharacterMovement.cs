using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _characterSpeed;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private InputAction _inputActions;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _rotateSpeed;
    [SerializeField] private GameObject _staticRotation;
    [SerializeField] private Vector3 _offsetRotation;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private Transform _groundCheck;
    private float _groundDistance = 0.4f;
    [SerializeField] private LayerMask _groundMask;
    private Vector3 _velocity;
    private bool _isGrounded;
    public bool _animationFlag = false;
    public bool _animationTrigger = false;
    public bool _isAttacking = false;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime;
    [SerializeField] private GameObject _dashTrailObject;
    private int _rotateSpeedHolder;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private WeaponTypes _currentWeaponType;
    [SerializeField] private WeaponSO _currentWeapon;
    public int _weaponDashCounter = 0;
    private Vector3 _clampedDash;

    public enum WeaponTypes { Sword, Bow}
    private void Start()
    {
        _rotateSpeedHolder = _rotateSpeed;
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
        if(_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        //float _horizontal = Input.GetAxisRaw("Horizontal");
        //float _vertical = Input.GetAxisRaw("Vertical");
        Vector2 _direction = _inputActions.ReadValue<Vector2>();
        Vector3 _fixedDirection = new Vector3(_direction.x, 0f, _direction.y);
        Vector3 _offsetDirection = new Vector3(0f, 0f, 0f);

        if (_direction.x == 0 && _direction.y < 0)
            _offsetDirection = new Vector3(-1f, 0f, -1f);
        if (_direction.x < 0f && _direction.y < 0f)
            _offsetDirection = new Vector3(-1f, 0f, 0f);
        if(_direction.x < 0 && _direction.y == 0)
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

        if(_isAttacking)
        {
            _offsetDirection = new Vector3(0f, 0f, 0f);
            _rotateSpeed = 0;
        }
        else
        {
            _rotateSpeed = _rotateSpeedHolder;
        }

        if(_fixedDirection.magnitude > 0.2f)
        {
            transform.rotation = Quaternion.Lerp(_staticRotation.transform.rotation, Quaternion.LookRotation(_fixedDirection) * Quaternion.Euler(0f, 45f, 0f), Time.deltaTime * _rotateSpeed);
        }

        _animator.SetFloat("MoveSpeed", _offsetDirection.magnitude);


        if(_offsetDirection.magnitude >= 0.1f)
        {
            _characterController.Move(_offsetDirection * _characterSpeed * Time.deltaTime);
        }

        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

        if(_animationFlag)
        {
            if(Input.GetMouseButtonDown(0))
            {
                _animator.SetBool("BackToMove", false);
                _animationTrigger = true;
                Ray _cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
                Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);
                float _rayLength;

                if (_groundPlane.Raycast(_cameraRay, out _rayLength))
                {
                    Vector3 _pointToLook = _cameraRay.GetPoint(_rayLength);
                    transform.LookAt(_pointToLook);
                    Vector3 _pointToDash = _pointToLook - this.transform.transform.position;
                    _clampedDash = new Vector3(Mathf.Clamp(_pointToDash.x, -1, 1), Mathf.Clamp(_pointToDash.y, -1, 1), Mathf.Clamp(_pointToDash.z, -1, 1));
                    //StartCoroutine(DashFromWeapon(_clampedDash, 5));
                }
            }
            else if(!_animationTrigger)
            {
                _animator.SetBool("BackToMove", true);
            }
        }
        //Debug.Log("Animation Flag: " + _animationFlag);
        //Debug.Log(_animationTrigger);

        if(Input.GetMouseButtonDown(0) && !_isAttacking)
        {
            _animator.Play("GreatSword2");
            _isAttacking = true;
            _weaponDashCounter = 0;
            Ray _cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);
            float _rayLength;

            if (_groundPlane.Raycast(_cameraRay, out _rayLength))
            {
                Vector3 _pointToLook = _cameraRay.GetPoint(_rayLength);
                transform.LookAt(_pointToLook);
                Vector3 _pointToDash = _pointToLook - this.transform.transform.position;
                _clampedDash = new Vector3(Mathf.Clamp(_pointToDash.x, -1, 1), Mathf.Clamp(_pointToDash.y, -1, 1), Mathf.Clamp(_pointToDash.z, -1, 1));
                //StartCoroutine(DashFromWeapon(_clampedDash, 5));
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(StartDash(_offsetDirection));
        }
    }

    IEnumerator StartDash(Vector3 offsetDirection)
    {
        float _startTime = Time.time;
        //_dashTrailObject.SetActive(true);
        _dashTrailObject.GetComponent<TrailRenderer>().emitting = true;
        while (Time.time < _startTime + _dashTime)
        {
            _characterController.Move(offsetDirection * _dashSpeed * Time.deltaTime);
            yield return null;
        }
        _dashTrailObject.GetComponent<TrailRenderer>().emitting = false;
    }

    public void StartDashFromWeapon()
    {
        StartCoroutine(DashFromWeapon());
    }
    IEnumerator DashFromWeapon()
    {
        float _startTime = Time.time;
        while (Time.time < _startTime + _dashTime)
        {
            _characterController.Move(_clampedDash * _currentWeapon._weaponDashPerAttack[_weaponDashCounter] * Time.deltaTime);
            yield return null;
        }

        if (_weaponDashCounter < 3)
            _weaponDashCounter++;
        else
            _weaponDashCounter = 0;

    }

}

