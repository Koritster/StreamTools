using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarGroundedState : AvatarBaseState
{
    public AvatarGroundedState(AvatarStateMachine currentContext, AvatarStateFactory avatarStateFactory) 
        : base(currentContext, avatarStateFactory) {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void UpdateState()
    {

        //El cambio de estado siempre va al final
        CheckSwitchStates();
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState() //Solo los root ocupan esta funcion
    {
        /*
        if(*Condicion para el substate*)
        {
            SetSubState(Factory.Jump());
        }
         
         */
    }

    public override void CheckSwitchStates()
    {
        /*
        if (*Condicion de salto*)
        {
            SwitchState(Factory.Jump());
        }
        */
    }
}
