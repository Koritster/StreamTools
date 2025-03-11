using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarEmptySubState : AvatarBaseState
{
    public AvatarEmptySubState(AvatarStateMachine currentContext, AvatarStateFactory avatarStateFactory)
        : base(currentContext, avatarStateFactory) { }
    public override void EnterState()
    {
        Debug.Log("EMPTY SUBSTATE ACTIVO");
        Ctx.AvatarAnimator.Play("Empty", 1);
    }

    public override void UpdateState()
    {

        //El cambio de estado siempre va al final
        CheckSwitchStates();
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsChattingActive)
        {
            SwitchState(Factory.Speaking());
        }
    }
}
