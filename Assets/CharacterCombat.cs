using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CharacterCombat : MonoBehaviour
{
    private static CharacterCombat _instance;
    [SerializeField] private WeaponTypes _currentWeaponType;
    [SerializeField] private WeaponSO _currentWeapon;
    [SerializeField] private GameObject _swordWeapon;
    [SerializeField] private GameObject _bowWeapon;
    private CharacterMovement _characterMovement;
    private CharacterController _characterController;
    [SerializeField] private Animator _animator;
    [SerializeField] private Camera _mainCamera;

    public enum WeaponTypes { Sword, Bow, FinFunnels }

    #region
    public int _weaponDashCounter = 0;
    private Vector3 _clampedDash;
    public int _currentWeaponDamage = 0;
    public int _currentWeaponKnockback = 0;
    public bool _animationFlag = false;
    public bool _animationTrigger = false;
    public bool _isAttacking = false;
    #endregion

    #region Bow
    [SerializeField] private Image _arrow;
    [SerializeField] private float _rateOfResize;
    [SerializeField] private float _rangeBow;
    [SerializeField] private float _rateOfBow;
    public bool _finishDrawBow;
    private float _bowRotTimer;
    private bool _bowStartRotTimer = false;
    [SerializeField] private float _bowTurnSpeed;
    private Ray _cameraRay;
    private Plane _groundPlane;
    [SerializeField] private GameObject _middleArrowPos;
    private Vector2 _initialValueGuideArrow = new Vector2(0,0);
    private float _initialRangeBow = 0;
    [SerializeField] private WeaponSO _swordSO, _bowSO;
    private float _rayLength;
    [SerializeField] private bool _arrowBlock = false;
    private float _savedDistance;
    [SerializeField] private float _arrowMultiplier;
    #endregion

    #region Dash
    [SerializeField] private GameObject _dashTrailObject;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime;
    #endregion

    #region Fin Funnels
    private List<GameObject> _enemies = new List<GameObject>();
    private List<GameObject> _inRangeEnemies = new List<GameObject>();
    [SerializeField] private GameObject _funnelOutPosition;
    [SerializeField] private float _funnelDelaySpawn;
    [SerializeField] private GameObject _funnelInitialPos;
    [SerializeField] private GameObject _funnelParent;
    private bool _returnFunnels = false;
    [SerializeField] private float _funnelTurnSpeed;
    #endregion
    #region Animation Strings    
    private static string
        _backToMovement = "BackToMove",
        _swordFirstAnim = "SwordCombo1",
        _bowDraw = "BowDraw",
        _bowRecoil = "BowRecoil",
        _bowMovement = "Bow Movement",
        _dummyAnim = "Dummy",
        _isBow = "IsBow";
    #endregion

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _characterController = GetComponent<CharacterController>();
        if(_currentWeapon._weaponType == WeaponTypes.Sword)
        {
            _animator.SetBool(_isBow, false);
            _animator.Play(_dummyAnim);
        }
        else if(_currentWeapon._weaponType == WeaponTypes.Bow)
        {
            _animator.SetBool(_isBow, true);
            _animator.Play(_dummyAnim);
        }
        if(_initialValueGuideArrow == new Vector2(0,0))
        _initialValueGuideArrow = _arrow.gameObject.GetComponent<RectTransform>().sizeDelta;
        if(_initialRangeBow == 0)
        _initialRangeBow = _rangeBow;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentWeapon._weaponType)
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
                            _animator.SetBool(_backToMovement, false);
                            _animationTrigger = true;
                            _cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
                            _groundPlane = new Plane(Vector3.up, Vector3.zero);

                            if (_groundPlane.Raycast(_cameraRay, out _rayLength))
                            {
                                LookAtDash();
                            }
                            if (_weaponDashCounter == 3)
                            {
                                _weaponDashCounter = 0;
                            }

                        }
                        else if (!_animationTrigger)
                        {
                            _animator.SetBool(_backToMovement, true);
                        }
                    }

                    if (Input.GetMouseButtonDown(0) && !_isAttacking)
                    {
                        _animator.Play(_swordFirstAnim);
                        _isAttacking = true;
                        _weaponDashCounter = 0;

                        _cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
                        _groundPlane = new Plane(Vector3.up, Vector3.zero);

                        if (_groundPlane.Raycast(_cameraRay, out _rayLength))
                        {
                            LookAtDash();
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
                        _animator.Play(_bowDraw);
                        _arrowBlock = false;
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

                            if (_groundPlane.Raycast(_cameraRay, out _rayLength))
                            {
                                Vector3 _pointToLook = _cameraRay.GetPoint(_rayLength);
                                transform.LookAt(_pointToLook);
                            }
                            RaycastHit hit;
                            Debug.DrawRay(_middleArrowPos.transform.position, -_middleArrowPos.transform.up * 1000, Color.green);
                            if (Physics.Raycast(_middleArrowPos.transform.position, -_middleArrowPos.transform.up, out hit, 1000))
                            {
                                if(hit.distance <= _rangeBow)
                                {
                                    _arrowBlock = true;
                                    _savedDistance = hit.distance;
                                }
                                else
                                {
                                    _arrowBlock = false;
                                    _arrow.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_rangeBow * _arrowMultiplier, _arrow.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                                }

                                if(_savedDistance == hit.distance)
                                {
                                    _arrowBlock = true;
                                    _arrow.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(hit.distance * _arrowMultiplier, _arrow.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                                }
                            }
                            if (_rangeBow < _currentWeapon._bowRange && !_arrowBlock)
                            {
                                IncreaseRangeAndSprite();
                            }
                        }
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        if(_finishDrawBow)
                        {
                            _animator.Play(_bowRecoil);
                            GameObject arrow = Instantiate(_currentWeapon._arrowPrefab, _middleArrowPos.transform.position, transform.rotation);
                            arrow.GetComponent<ArrowScript>().PDistanceToDestroy = _rangeBow;
                        }
                        else
                        {
                            _animator.Play(_bowMovement);
                        }
                        _arrow.gameObject.SetActive(false);

                        Debug.Log(_arrow.gameObject.GetComponent<RectTransform>().sizeDelta.x / _rangeBow);

                        _characterMovement.PIsAimingBow = false;
                    }
                    break;
                }
            case WeaponTypes.FinFunnels:
                {
                    if(_enemies.Count != 0)
                    {
                        for (int i = 0; i < _enemies.Count; i++)
                        {
                            if(Vector3.Distance(this.transform.position, _enemies[i].transform.position) < _currentWeapon._enemyDetectionRange)
                            {
                                if(!_inRangeEnemies.Contains(_enemies[i]))
                                _inRangeEnemies.Add(_enemies[i]);
                            }
                            else
                            {
                                _inRangeEnemies.Remove(_enemies[i]);
                            }
                        }
                    }

                    if(_inRangeEnemies.Count != 0 )
                    {
                        if (_funnelParent.transform.childCount ==  0)
                        StartCoroutine(SpawnFunnels());
                    }
                    else if(!_returnFunnels && _funnelParent.transform.childCount != 0)
                    {
                        StartCoroutine(RecallFunnels());
                        _returnFunnels = true;
                    }
                    break;
                }
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
        _finishDrawBow = true;
    }

    public void SwapWeapon()
    {
        if (_currentWeapon._weaponType == WeaponTypes.Bow)
        {
            _currentWeapon = _swordSO;
            Start();
        }
        else if (_currentWeapon._weaponType == WeaponTypes.Sword)
        {
            _currentWeapon = _bowSO;
            Start();
        }
    }

    public void LookAtDash()
    {
        Vector3 _pointToLook = _cameraRay.GetPoint(_rayLength);
        transform.LookAt(_pointToLook);
        Vector3 _pointToDash = _pointToLook - this.transform.transform.position;        //To Optimize
        _clampedDash = new Vector3(Mathf.Clamp(_pointToDash.x, -1, 1), Mathf.Clamp(_pointToDash.y, -1, 1), Mathf.Clamp(_pointToDash.z, -1, 1));
    }

    public void IncreaseRangeAndSprite()
    {
        //subject for refactor
        _arrow.gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(1, 0) * Time.deltaTime * _rateOfResize;
        _rangeBow += 1f * Time.deltaTime * _rateOfBow;
    }

    IEnumerator SpawnFunnels()
    {
        //for (int i = 0; i < _currentWeapon._funnelCount; i++)
        //{
        //    GameObject _funnel = Instantiate(_currentWeapon._funnelPrefab, _funnelOutPosition.transform.position, Quaternion.identity);
        //    _funnel.GetComponent<FinFunnels>().PEnemies = _inRangeEnemies;
        //    _funnel.transform.parent = _funnelParent.transform;

        //    while(_funnel.transform.position != _funnelInitialPos.transform.position)
        //    {
        //        //_funnel.transform.LookAt(_funnelInitialPos.transform);
        //        var direction = (_funnelInitialPos.transform.position - transform.position).normalized;
        //        var rotGoal = Quaternion.LookRotation(direction);
        //        _funnel.transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, _funnelTurnSpeed);
        //        _funnel.transform.position = Vector3.MoveTowards(_funnel.transform.position, _funnelInitialPos.transform.position, Time.deltaTime * _funnel.GetComponent<FinFunnels>().PFunnelSpeed);
        //    }

        //    Debug.Log(_funnel.transform.position + ":" + _funnelInitialPos.transform.position);
        //    if(_funnel.transform.position == _funnelInitialPos.transform.position)
        //    {
        //        _funnel.GetComponent<FinFunnels>().PFunnelStart = true;
        //    }
        //    yield return new WaitForSeconds(_funnelDelaySpawn);
        //}
    }

    IEnumerator RecallFunnels()
    {
        foreach(Transform _child in _funnelParent.transform)
        {
            _child.gameObject.GetComponent<FinFunnels>().PFunnelStart = false;
            while (_child.position != _funnelInitialPos.transform.position)
            {

                //_child.transform.LookAt(_funnelInitialPos.transform);
                var direction = (_funnelInitialPos.transform.position - transform.position).normalized;
                var rotGoal = Quaternion.LookRotation(direction);
                _child.transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, _funnelTurnSpeed);
                _child.position = Vector3.MoveTowards(_child.position, _funnelInitialPos.transform.position, Time.deltaTime * _child.GetComponent<FinFunnels>().PFunnelSpeed);
            }
            yield return new WaitForSeconds(0.01f);
            while (_child.position != _funnelOutPosition.transform.position)
            {
                //_child.transform.LookAt(_funnelOutPosition.transform);
                var direction = (_funnelOutPosition.transform.position - transform.position).normalized;
                var rotGoal = Quaternion.LookRotation(direction);
                _child.transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, _funnelTurnSpeed);
                _child.position = Vector3.MoveTowards(_child.position, _funnelOutPosition.transform.position, Time.deltaTime * _child.GetComponent<FinFunnels>().PFunnelSpeed);
            }
            yield return new WaitForSeconds(0.25f);

            Destroy(_child.gameObject);
        }
        _returnFunnels = false;
    }
    public static CharacterCombat PInstance { get { return _instance; } }
    public List<GameObject> PEnemies { set { _enemies = value; } }
}
