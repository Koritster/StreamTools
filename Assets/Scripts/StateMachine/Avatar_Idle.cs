using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar_Idle : Avatar_Base_State
{
    private float timer;
    private float stateDuration;
    private Animator animator;
    public override void EnterState(AvatarStateManager avatar)
    {
        Debug.Log("EYEYEYEY ESTOY EN IDLE");
        timer = 0;
        stateDuration = Random.Range(3, 8);
        Debug.Log(stateDuration);
        animator = avatar.GetComponent<Animator>();
        animator.Play("Idle");
    }
    public override void UpdateState(AvatarStateManager avatar)
    {
        if(timer < stateDuration)
        {
            timer += Time.deltaTime;
        }
        else
        {
            avatar.SwitchState(avatar.WanderingState);
        }
    }

}
