using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarIdleState : AvatarBaseState
{
    public AvatarIdleState(AvatarStateMachine currentContext, AvatarStateFactory avatarStateFactory)
        : base(currentContext, avatarStateFactory) {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubState();

        Debug.Log("EYEYEYEY ESTOY EN IDLE");
        Ctx.StateDuration = Random.Range(4, 12);
        Debug.Log("ESTARE EN IDLE POR " + Ctx.StateDuration + "s");
        Ctx.ActualCoroutine = Ctx.StartCoroutine(StateTimer());
        Ctx.AvatarAnimator.Play("Idle", 0);
    }

    public override void UpdateState()
    {

        //El cambio de estado siempre va al final
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.StopCoroutine(Ctx.ActualCoroutine);
        Debug.Log("SALIENDO DE IDLE...");
    }
    
    public override void InitializeSubState()
    {
        if (Ctx.IsChattingActive)
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
       if(Ctx.TimerHasEnded)
        {
            Ctx.TimerHasEnded = false;
            SwitchState(Factory.Wandering());
        }
    }

    IEnumerator StateTimer()
    {
        yield return new WaitForSeconds(Ctx.StateDuration);
        Ctx.TimerHasEnded = true;
    }

}




