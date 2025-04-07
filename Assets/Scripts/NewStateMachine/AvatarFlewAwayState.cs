using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarFlewAwayState : AvatarBaseState
{
    public AvatarFlewAwayState(AvatarStateMachine currentContext, AvatarStateFactory avatarStateFactory)
        : base(currentContext, avatarStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubState();
        Ctx.Avatar.GetComponent<BoxCollider2D>().enabled = false;
        Ctx.AvatarRigidBody.velocity = new Vector2(Ctx.AvatarRigidBody.velocity.x * 0.5f, -1.5f);
        Ctx.AvatarRigidBody.gravityScale = 0.3f;

        Ctx.ActualCoroutine = Ctx.StartCoroutine(TimeToDisappear());
        Debug.Log("ME FUI VOLANDO AAAAAAAAAAAAAAAAAAAAAAAAA");

    }

    public override void UpdateState()
    {
        Debug.Log(Ctx.AlreadyDisappeared);
        //El cambio de estado siempre va al final
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.IsFlyingAway = false;
        Ctx.AlreadyDisappeared = false;
        Debug.Log("ya no estoy volando");
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
        if (Ctx.AlreadyDisappeared)//**********
        {
            SwitchState(Factory.Idle());
        }
    }

    IEnumerator TimeToDisappear()
    {
        Vector3 initialScale = Ctx.Avatar.transform.localScale; //  Escala inicial / normal del avatar
        Vector3 targetScale = Vector3.zero; //  Escala minima
        float duration = 1.7f; //   Tiempo que toma que el avatar llegue a la escala minima
        float elapsed = 0f; //  Tiempo que lleva el avatar encogiendose

        while (elapsed < duration) //   Bucle while para que se encoja el avatar
        {
            Ctx.Avatar.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration); //   .Lerp permite una transicion suave en la escala
            elapsed += Time.deltaTime;  //aumenta el tiempo que lleva en bucle
            yield return null;
        }

        Ctx.Avatar.transform.localScale = targetScale; // Asegurarse de que termina exactamente en 0
        yield return new WaitForSeconds(3f);
        RestartAvatarValues(initialScale); //   Reinicia los valores del avatar para que sea normal
        yield return null;
        Ctx.Avatar.SetActive(false);
        Ctx.AlreadyDisappeared = true;
    }

    void RestartAvatarValues(Vector3 initialScale)
    {
        Ctx.Avatar.GetComponent<BoxCollider2D>().enabled = true;
        Ctx.AvatarRigidBody.freezeRotation = true; //Desactiva que las fisicas del avatar lo hagan rotar
        Ctx.Avatar.transform.rotation = Quaternion.identity; // Establece la rotacion a sus valores Default
        Ctx.Avatar.transform.localScale = initialScale;
        Ctx.WasClicked = false;
        Ctx.AvatarRigidBody.gravityScale = 1f;
        Ctx.AvatarAnimator.Play("Idle", 0);
    }

}
