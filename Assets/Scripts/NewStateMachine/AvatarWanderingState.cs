using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarWanderingState : AvatarBaseState
{
    public AvatarWanderingState(AvatarStateMachine currentContext, AvatarStateFactory avatarStateFactory) 
        : base(currentContext, avatarStateFactory) {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubState();

        Ctx.StateDuration = Random.Range(4, 9);
        Ctx.ActualCoroutine = Ctx.StartCoroutine(StateTimer());
        Ctx.NewLocationX = Random.Range(-7f, 7f);
        Debug.Log("EYEYEYEY ME VOY A MOVER A " + Ctx.NewLocationX);
        Ctx.AvatarAnimator.Play("Walk");
        Debug.Log("ESTARE EN WANDERING POR " + Ctx.StateDuration + "s");
        if(Ctx.Avatar.transform.position.x >= Ctx.NewLocationX)
        {
            Ctx.Avatar.transform.localScale = new Vector2(-1f, 1f);
        }
        else
        {
            Ctx.Avatar.transform.localScale = new Vector2(1f, 1f);
        }
        
    }

    public override void UpdateState()
    {
        Ctx.Avatar.transform.position = Vector2.MoveTowards(Ctx.Avatar.transform.position, new Vector2(Ctx.NewLocationX, Ctx.Avatar.transform.position.y), Ctx.MovementSpeed * Time.deltaTime);


        //El cambio de estado siempre va al final
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.StopCoroutine(Ctx.ActualCoroutine);
        Debug.Log("SALIENDO DE WANDERING...");
    }

    public override void InitializeSubState()       //Solo los root ocupan esta funcion
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
        if (Ctx.TimerHasEnded || Ctx.Avatar.transform.position.x == Ctx.NewLocationX)
        {
            Ctx.TimerHasEnded = false;
            SwitchState(Factory.Idle());
        }
    }

    IEnumerator StateTimer()
    {
        yield return new WaitForSeconds(Ctx.StateDuration);
        Ctx.TimerHasEnded = true;
    }
}