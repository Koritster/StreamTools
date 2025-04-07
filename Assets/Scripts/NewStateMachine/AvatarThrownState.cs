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

        Ctx.AvatarRigidBody.freezeRotation = false; //Permite que las fisicas del avatar lo hagan rotar
        Ctx.AvatarAnimator.Play("Grabbed", 0);
        Debug.Log("EYEYEYEY ME AGARRARON");
        
    }

    public override void UpdateState()
    {
        Ctx.AvatarVelocity = Ctx.AvatarRigidBody.velocity.magnitude; //Almacena la velocidad actual del avatar
        //Debug.Log(Ctx.AvatarVelocity);
        if (!Ctx.WasClicked) //Si el avatar NO esta siendo cliqueado
        {
            Ctx.AvatarAnimator.Play("Thrown", 0);
        }
        else if(Ctx.WasClicked) //Si el avatar SI esta siendo cliqueado
        {
            Ctx.AvatarAnimator.Play("Grabbed", 0);
        }


        //El cambio de estado siempre va al final
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Debug.Log("Aterrizando...");
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
        if (Ctx.AvatarVelocity < 0.01f && !Ctx.WasClicked)
        {
            Ctx.AvatarRigidBody.freezeRotation = true; //Desactiva que las fisicas del avatar lo hagan rotar
            Ctx.Avatar.transform.rotation = Quaternion.identity; // Establece la rotacion a sus valores Default
            SwitchState(Factory.Idle());
        }else if (Ctx.IsFlyingAway && !Ctx.WasClicked)
        {
            SwitchState(Factory.FlewAway());
        }
    }

}
