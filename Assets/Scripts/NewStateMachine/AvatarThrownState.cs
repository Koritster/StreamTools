using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarThrownState : AvatarBaseState
{
    public AvatarThrownState(AvatarStateMachine currentContext, AvatarStateFactory avatarStateFactory)
        : base(currentContext, avatarStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubState();

        Debug.Log("EYEYEYEY ME AGARRARON");
        
    }

    public override void UpdateState()
    {

        //El cambio de estado siempre va al final
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Debug.Log("Aterrizando...");
    }

    public override void InitializeSubState()
    {
        if (Ctx.IsChattingActive)//******
        {
            SetSubState(Factory.Speaking());
        }
        else
        {
            SetSubState(Factory.Empty());
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.TimerHasEnded) //*****
        {
            Ctx.TimerHasEnded = false;
            SwitchState(Factory.Wandering());
        }
    }

}
