using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CharacterCombat : MonoBehaviour
{
    [SerializeField] private WeaponTypes _currentWeaponType;
    [SerializeField] private WeaponSO _currentWeapon;
    public int _weaponDashCounter = 0;
    private Vector3 _clampedDash;
    public int _currentWeaponDamage = 0;
    public int _currentWeaponKnockback = 0;
    public bool _animationFlag = false;
    public enum WeaponTypes { Sword, Bow }
    [SerializeField] private Animator _animator;
    [SerializeField] private Camera _mainCamera;
    public bool _animationTrigger = false;
    public bool _isAttacking = false;
    [SerializeField] private GameObject _dashTrailObject;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime;
    private CharacterMovement _characterMovement;
    private CharacterController _characterController;
    [SerializeField] private Image _arrow;
    [SerializeField] private float _rateOfResize;
    [SerializeField] private float _rangeBow;
    [SerializeField] private float _rateOfBow;

    // Start is called before the first frame update
    void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (_animationFlag)
        {
            if (Input.GetMouseButtonDown(0))
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
                    Vector3 _pointToDash = _pointToLook - this.transform.transform.position;        //To Optimize
                    _clampedDash = new Vector3(Mathf.Clamp(_pointToDash.x, -1, 1), Mathf.Clamp(_pointToDash.y, -1, 1), Mathf.Clamp(_pointToDash.z, -1, 1));
                }
                if (_weaponDashCounter == 3)
                {
                    _weaponDashCounter = 0;
                }

            }
            else if (!_animationTrigger)
            {
                _animator.SetBool("BackToMove", true);
            }
        }

        if(Input.GetMouseButton(0))
        {
            _arrow.gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(1, 0) * Time.deltaTime * _rateOfResize;
            _rangeBow += 1f * Time.deltaTime * _rateOfBow;
            Debug.Log(_arrow.gameObject.GetComponent<RectTransform>().sizeDelta);
        }
        if (Input.GetMouseButtonDown(0) && !_isAttacking)
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
                Vector3 _pointToDash = _pointToLook - this.transform.transform.position;        //To Optimize
                _clampedDash = new Vector3(Mathf.Clamp(_pointToDash.x, -1, 1), Mathf.Clamp(_pointToDash.y, -1, 1), Mathf.Clamp(_pointToDash.z, -1, 1));
                Debug.Log(_clampedDash);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(StartDash(_characterMovement.POffsetDirection));
        }

        if (!_isAttacking)
        {
            _currentWeaponDamage = 0;
            _currentWeaponKnockback = 0;
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
        {
            _weaponDashCounter++;
        }
        else
            _weaponDashCounter = 0;
    }

    public void RegisterWeaponDamage()
    {
        _currentWeaponDamage = _currentWeapon._weaponDamagePerAttack[_weaponDashCounter];
        _currentWeaponKnockback = _currentWeapon._weaponKnockbackPerAttack[_weaponDashCounter];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, _rangeBow);
    }
}
