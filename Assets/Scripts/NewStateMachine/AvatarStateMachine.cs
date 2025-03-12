using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Animator _avatarAnimator;
    private GameObject _avatar;
    [SerializeField] private GameObject _avatarSkin;
    private bool _timerHasEnded;
    private Coroutine _actualCoroutine;

    //UI variables
    public string user;
    public GameObject go_textBox;
    public Text txt_message;
    [SerializeField] private Text txt_user;

    //Timer Variables
    [SerializeField] private float _timeToQuitMessage; //Se queda
    [SerializeField] private float timeToDissapear; //Se queda
    private bool _isChattingActive;
    private float _timerMessages = 0f; //    X??
    private float _timerAvatar = 0f; //      X??

    //Getters & Setters
    public AvatarBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public int StateDuration { get { return _stateDuration; } set { _stateDuration = value; } }
    public float NewLocationX { get { return _newLocationX; } set { _newLocationX = value; } }
    public float MovementSpeed { get { return _movementSpeed; } }
    public Animator AvatarAnimator { get { return _avatarAnimator; } }
    public GameObject Avatar { get { return _avatar; } }
    public GameObject AvatarSkin { get { return _avatarSkin; } set { _avatarSkin = value; } }
    public bool TimerHasEnded { get { return _timerHasEnded; } set { _timerHasEnded = value; } }
    public Coroutine ActualCoroutine { get { return _actualCoroutine; } set { _actualCoroutine = value; } }
    public bool IsChattingActive { get { return _isChattingActive; } set { _isChattingActive = value; } }
    public float TimeToQuitMessage { get { return _timeToQuitMessage; } }
    public float TimerMessages { get { return _timerMessages; } set { _timerMessages = value; } }

    private void Awake()
    {
        _states = new AvatarStateFactory(this);
        _currentState = _states.Idle();
        _avatar = gameObject;
        _avatarAnimator = _avatarSkin.GetComponent<Animator>();


        //Al final siempre
        _currentState.EnterState();
    }

    void Start()
    {
        Physics2D.IgnoreLayerCollision(10, 10);
        _timerAvatar = 0f;
    }

    
    void Update()
    {

        //Timer que cuenta la inactividad del viewer para desaparecer el avatar despues de cierto tiempo
        _timerAvatar += Time.deltaTime;
        if (_timerAvatar >= timeToDissapear)
        {
            DissapearAvatar();
        }

        _currentState.UpdateStates();
    }



    #region Metodos propios

    public void ChangeAvatar(AvatarCharacter avatar)
    {
        Destroy(_avatarSkin);
        Instantiate(avatar.avatarPrefab, _avatar.transform);

        _avatarSkin = transform.GetChild(1).gameObject;
        _avatarAnimator = _avatarSkin.GetComponent<Animator>();
        //GetComponent<Animator>().runtimeAnimatorController = avatar.avatarPrefab;
    }

    //Cambiar el nombre mostrado del usuario. Esta función se llama desde el AvatarSpawner.cs
    public void ChangeName(string userName)
    {
        txt_user.text = userName;
        user = userName;
    }

    //Muestra el mensaje que los usuarios escriban. Esta función se llama desde el ShowMessage.cs
    //Importante para el subestado de speaking
    public void ShowMessage(string message)
    {
        go_textBox.SetActive(true);
        txt_message.text = message;

        //Reinicia los timers
        _timerMessages = 0f;
        _timerAvatar = 0f;
        
        _isChattingActive = true;
    }


    //Metodo para emular el mensaje sin hacer conexion con el twitch connect por que no le entendi ni pijo a todo lo que hay que meterle
    //PD: esto se mete en un boton en el escenario, asi es, no era necesario crear un metodo nuevo y se pudo haber usado el original pero lo dejare un rato por los jajas
    public void MessageEmulation(string message)
    {
        go_textBox.SetActive(true);
        txt_message.text = message;

        //Reinicia los timers
        _timerMessages = 0f;
        _timerAvatar = 0f;

        _isChattingActive = true;
    }

    //Oculta el mensaje después de un periodo de tiempo. Se llama en el subestado de speaking
    public void HideMessage()
    {
        go_textBox.SetActive(false);
    }

    public void DissapearAvatar()
    {
        gameObject.SetActive(false);
    }

    #endregion


}
