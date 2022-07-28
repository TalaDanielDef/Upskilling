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
    [SerializeField] private GameObject _swordWeapon;
    [SerializeField] private GameObject _bowWeapon;
    public bool _finishDrawBow;
    private float _bowRotTimer;
    private bool _bowStartRotTimer = false;
    [SerializeField] private float _bowTurnSpeed;
    private Ray _cameraRay;
    private Plane _groundPlane;
    [SerializeField] private GameObject _middleArrowPos;
    private Vector2 _initialValueGuideArrow;
    private float _initialRangeBow;
    // Start is called before the first frame update
    void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _characterController = GetComponent<CharacterController>();
        if(_currentWeapon._weaponType == WeaponTypes.Sword)
        {
            _animator.SetBool("IsBow", false);
            _animator.Play("Dummy");
        }
        else if(_currentWeapon._weaponType == WeaponTypes.Bow)
        {
            _animator.SetBool("IsBow", true);
            _animator.Play("Dummy");
        }
        _initialValueGuideArrow = _arrow.gameObject.GetComponent<RectTransform>().sizeDelta;
        _initialRangeBow = _rangeBow;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentWeaponType)
        {
            case WeaponTypes.Sword:
                {
                    if(!_swordWeapon.activeSelf)
                    {
                        _swordWeapon.SetActive(true);
                    }
                    if(_bowWeapon.activeSelf)
                    {
                        _bowWeapon.SetActive(false);
                    }

                    if (_animationFlag)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            _animator.SetBool("BackToMove", false);
                            _animationTrigger = true;
                            _cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
                            _groundPlane = new Plane(Vector3.up, Vector3.zero);
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

                    if (Input.GetMouseButtonDown(0) && !_isAttacking)
                    {
                        _animator.Play("GreatSword2");
                        _isAttacking = true;
                        _weaponDashCounter = 0;

                        _cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
                        _groundPlane = new Plane(Vector3.up, Vector3.zero);
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
                    break;
                }
            case WeaponTypes.Bow:
                {
                    if (!_bowWeapon.activeSelf)
                    {
                        _bowWeapon.SetActive(true);
                    }
                    if (_swordWeapon.activeSelf)
                    {
                        _swordWeapon.SetActive(false);
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        _animator.Play("Standing Draw Arrow");
                        _cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
                        _groundPlane = new Plane(Vector3.up, Vector3.zero);
                        _finishDrawBow = false;
                        _bowStartRotTimer = true;
                        _characterMovement.PIsAimingBow = true;
                        _arrow.gameObject.GetComponent<RectTransform>().sizeDelta = _initialValueGuideArrow;
                        _rangeBow = _initialRangeBow;
                    }
                    if (_bowStartRotTimer)
                    {
                        _bowRotTimer += Time.deltaTime;

                        if(_bowRotTimer <= 0.1f)
                        {
                            float _rayLength;
                            if (_groundPlane.Raycast(_cameraRay, out _rayLength))
                            {
                                Vector3 _pointToLook = _cameraRay.GetPoint(_rayLength);
                                var direction = (_pointToLook - transform.position).normalized;
                                var rotGoal = Quaternion.LookRotation(direction);
                                transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, _bowTurnSpeed);
                            }
                        }
                        else
                        {
                            _bowRotTimer = 0;
                            _bowStartRotTimer = false;

                        }
                    }
                    if (Input.GetMouseButton(0))
                    {
                        if(_finishDrawBow)
                        {
                            _arrow.gameObject.SetActive(true);
                            Ray _cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
                            Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);
                            float _rayLength;

                            if (_groundPlane.Raycast(_cameraRay, out _rayLength))
                            {
                                Vector3 _pointToLook = _cameraRay.GetPoint(_rayLength);
                                transform.LookAt(_pointToLook);
                            }
                            //RangeAttack(_arrow.gameObject, true);
                            if (_rangeBow < _currentWeapon._bowRange)
                            {
                                //subject for refactor
                                _arrow.gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(1, 0) * Time.deltaTime * _rateOfResize;
                                _rangeBow += 1f * Time.deltaTime * _rateOfBow;
                                Debug.Log(_arrow.gameObject.GetComponent<RectTransform>().sizeDelta);
                                Debug.Log(_rangeBow);
                            }

                        }
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        if(_finishDrawBow)
                        {
                            _animator.Play("Standing Aim Recoil");
                            GameObject arrow = Instantiate(_currentWeapon._arrowPrefab, _middleArrowPos.transform.position, transform.rotation);
                            arrow.GetComponent<ArrowScript>().PDistanceToDestroy = _rangeBow;
                        }
                        else
                        {
                            _animator.Play("Bow Movement");
                        }
                        _arrow.gameObject.SetActive(false);


                        _characterMovement.PIsAimingBow = false;
                    }
                    break;
                }
        }



    }
    //public void RangeAttack(GameObject _toExtend, bool _ifExtend)
    //{

    //}
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


    public void EndDrawBow()
    {
        Debug.Log("End Draw Bow");
        _finishDrawBow = true;
    }
}