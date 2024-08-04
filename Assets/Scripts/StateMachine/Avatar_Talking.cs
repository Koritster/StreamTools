using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar_Talking : Avatar_Base_State
{
    private Animator animator;
    public override void EnterState(AvatarStateManager avatar)
    {
        animator = avatar.GetComponent<Animator>();
        animator.Play("Talk");
    }
    public override void UpdateState(AvatarStateManager avatar)
    {

    }

}
