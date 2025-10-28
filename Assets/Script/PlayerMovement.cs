using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions _input;
    private Vector2 _inputMovement;
    private Vector3 _direction;
    private Vector3 _lookDirection;
    private CharacterController _characterController;

    private float _speed;
    [SerializeField] Transform _model;
    [SerializeField] float _gravity = -9.81f;

    PlayerInformation _playerInformation;

    private bool _canBeHit = true;
    private bool _isKickedBack = false;
    private bool _canAttack = true;

    [SerializeField] private float _shortWaitLength = .25f;
    //[SerializeField] private float _mediumWaitLength = 1.0f;
    [SerializeField] private float _longWaitlength = 2;

    private WaitForSeconds _shortWait;
    private WaitForSeconds _longWait;

    private Coroutine _invisibiltyRoutine;
    private Coroutine _kickbackRoutine;

    Animator _baseAnimator;
    Animator _characterAnimator;
    float _animatorMovementSpeed = 0;

    private void OnEnable()
    {
        _input = new InputSystem_Actions();
        _input.Player.Enable();
        _input.Player.Attack.performed += PlayerAttack;
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInformation = GetComponent<PlayerInformation>();
        _baseAnimator = GetComponent<Animator>();
        _characterAnimator = transform.Find("Character").Find("Model").GetComponent<Animator>();

        _shortWait = new WaitForSeconds(_shortWaitLength);
        _longWait = new WaitForSeconds(_longWaitlength);
    }

    private void Update()
    {
        if (GameManager.Instance.IsMenuActive) return;

        if (_playerInformation.CurrentHealth <= 0 )
        {
            return;
        }

        _speed = _playerInformation.Speed;

        if (!_isKickedBack)
        {
            _inputMovement = _input.Player.Move.ReadValue<Vector2>();
            _direction.x = _inputMovement.x;
            _direction.z = _inputMovement.y;

            if (!_characterController.isGrounded)
                _direction.y += _gravity * Time.deltaTime;
            else
                _direction.y = 0;

            //Update the Speed value in Character Animator if Moving or Speed is not 0
            if (_inputMovement.x != 0 || _inputMovement.y != 0)
                _animatorMovementSpeed = _direction.magnitude;
            else 
                _animatorMovementSpeed = 0;

            if (_characterAnimator.GetFloat("Speed") != _animatorMovementSpeed)
                _characterAnimator.SetFloat("Speed", _animatorMovementSpeed);
            else if (_characterAnimator.GetFloat("Speed") != 0 && _animatorMovementSpeed == 0)
                _characterAnimator.SetFloat("Speed", 0);

            //transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
            _characterController.Move(_direction * (_speed * Time.deltaTime));

            _lookDirection = _direction;
            _lookDirection.y = 0;
            _lookDirection.Normalize();
            _lookDirection += transform.position;
            _model.LookAt(_lookDirection);
        }
        else
        {            
            //transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
            _characterController.Move(_direction * (_speed * Time.deltaTime * -.5f));
        }
    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        if (_canAttack && !GameManager.Instance.IsMenuActive)
        {
            _characterAnimator.SetTrigger("Attack");
            _canAttack = false;
            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(.9f);
        _canAttack = true;
    }

    /*private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.TryGetComponent<ICollidable>(out ICollidable collidable))
        {
            if (_canBeHit)
            {
                collidable.OnCollide(this.transform);
                _canBeHit = false;
                _isKickedBack = true;
                if (_kickbackRoutine == null)
                    _kickbackRoutine = StartCoroutine(KickBackRoutine());
                if (_invisibiltyRoutine == null)
                    _invisibiltyRoutine = StartCoroutine(InvisibilityFramesRoutine());
            }
        }
    }*/

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.transform.TryGetComponent<ICollidable>(out ICollidable collidable))
        {
            if (_canBeHit)
            {
                collidable.OnCollide(this.transform);
                _canBeHit = false;
                _isKickedBack = true;
                if (_kickbackRoutine == null)
                    _kickbackRoutine = StartCoroutine(KickBackRoutine());
                if (_invisibiltyRoutine == null)
                    _invisibiltyRoutine = StartCoroutine(InvisibilityFramesRoutine());
            }
        }
    }

    IEnumerator KickBackRoutine()
    {
        yield return _shortWait;
        _isKickedBack = false;
        _kickbackRoutine = null;
    }

    IEnumerator InvisibilityFramesRoutine()
    {
        yield return _longWait;
        _canBeHit = true;
        _invisibiltyRoutine = null;
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, _model.forward);
    }

    public void AdjustDirection(Vector3 direction)
    {
        //Debug.Log("AdjustDirection");
        _direction = direction;

        _isKickedBack = true;
        if (_kickbackRoutine == null)
            _kickbackRoutine = StartCoroutine(KickBackRoutine());
    }
}
