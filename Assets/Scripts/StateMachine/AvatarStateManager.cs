using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarStateManager : MonoBehaviour
{

    Avatar_Base_State currentState;
    public Avatar_Wandering WanderingState = new Avatar_Wandering();
    public Avatar_Idle IdleState = new Avatar_Idle(); //Estado base
    public Avatar_Talking TalkingState = new Avatar_Talking();

    void Start()
    {
        currentState = IdleState; //Estado inicial de la maquina

        currentState.EnterState(this); //"this" es referencia al contexto (Este script pues)
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(Avatar_Base_State state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
