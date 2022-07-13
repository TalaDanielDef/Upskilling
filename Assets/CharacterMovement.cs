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
    private Vector3 _offsetDirection = new Vector3(0f, 0f, 0f);
    // Update is called once per frame

    private void OnEnable()
    {
        _inputActions.Enable();
    }
    private void OnDisable()
    {
        _inputActions.Disable();
    }
    void FixedUpdate()
    {

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

        //Debug.Log(_direction);
        Debug.Log(_offsetDirection);
        Debug.Log(_characterController.velocity.magnitude);
        if(_fixedDirection.magnitude > 0.2f)
        {
            transform.rotation = Quaternion.Lerp(_staticRotation.transform.rotation, Quaternion.LookRotation(_fixedDirection) * Quaternion.Euler(0f, 45f, 0f), Time.deltaTime * _rotateSpeed);
        }

        _animator.SetFloat("MoveSpeed", _offsetDirection.magnitude);

        if(_offsetDirection.magnitude >= 0.1f)
        {
            _characterController.Move(_offsetDirection * _characterSpeed * Time.deltaTime);
        }
    }
}
