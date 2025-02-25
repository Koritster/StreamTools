using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSpeakingSubState : AvatarBaseState
{
    public AvatarSpeakingSubState(AvatarStateMachine currentContext, AvatarStateFactory avatarStateFactory)
        : base(currentContext, avatarStateFactory) { }
    public override void EnterState()
    {
        Debug.Log("SPEAKING SUBSTATE ACTIVO");
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

    }
}
