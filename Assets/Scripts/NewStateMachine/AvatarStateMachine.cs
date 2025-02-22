using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarStateMachine : MonoBehaviour
{
    //CHANGADAS CONSTANTES
    //---------Scripts
    AvatarBaseState _currentState;
    AvatarStateFactory _states;

    //--------Variables
    private int _stateDuration;
    private float _newLocationX;
    private float _movementSpeed = 1;
    private SpriteRenderer _avatarOrientation;
    private Animator _avatarAnimator;
    private GameObject _avatar;
    private bool _timerHasEnded;
    private Coroutine _actualCoroutine;


    //Getters & Setters
    public AvatarBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public int StateDuration { get { return _stateDuration; } set { _stateDuration = value; } }
    public float NewLocationX { get { return _newLocationX; } set { _newLocationX = value; } }
    public float MovementSpeed { get { return _movementSpeed; } }
    public SpriteRenderer AvatarOrientation { get { return _avatarOrientation; } }
    public Animator AvatarAnimator { get { return _avatarAnimator; } }
    public GameObject Avatar { get { return _avatar; } }
    public bool TimerHasEnded { get { return _timerHasEnded; } set { _timerHasEnded = value; } }
    public Coroutine ActualCoroutine { get { return _actualCoroutine; } set { _actualCoroutine = value; } }

    private void Awake()
    {
        _states = new AvatarStateFactory(this);
        _currentState = _states.Idle();

        _avatar = gameObject;
        _avatarOrientation = GetComponent<SpriteRenderer>();
        _avatarAnimator = GetComponent<Animator>();


        //Al final siempre
        _currentState.EnterState();
    }

    void Start()
    {
        
    }

    
    void Update()
    {

        _currentState.UpdateStates();
    }
}
