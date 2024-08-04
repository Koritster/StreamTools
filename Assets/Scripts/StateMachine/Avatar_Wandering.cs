using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar_Wandering : Avatar_Base_State
{
    private float timer;
    private float stateDuration;
    private float newLocationX;
    private float movementSpeed = 1;
    private SpriteRenderer orientation;
    private Animator animator;
    public override void EnterState(AvatarStateManager avatar)
    {
        timer = 0;
        stateDuration = Random.Range(3, 8);
        newLocationX = Random.Range(-7, 7);
        orientation = avatar.GetComponent<SpriteRenderer>();
        animator = avatar.GetComponent<Animator>();
        animator.Play("Walk");
        Debug.Log("EYEYEYEY ME VOY A MOVER A " + newLocationX);
        Debug.Log(stateDuration);
        if (avatar.transform.position.x < newLocationX)
        {
            orientation.flipX = false;
        }
        else
        {
            orientation.flipX = true;
        }
    }
    public override void UpdateState(AvatarStateManager avatar)
    {

        if (timer < stateDuration)
        {
            timer += Time.deltaTime; //la cuenta para el cambio de estado
            avatar.transform.position = Vector2.MoveTowards(avatar.transform.position, new Vector2(newLocationX, avatar.transform.position.y), movementSpeed * Time.deltaTime);
            if(avatar.transform.position.x == newLocationX)
            {
                avatar.SwitchState(avatar.IdleState);
            }
        }
        else
        {
            avatar.SwitchState(avatar.IdleState);
        }
    }
}