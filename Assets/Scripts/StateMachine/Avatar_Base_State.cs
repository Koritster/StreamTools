using UnityEngine;

public abstract class Avatar_Base_State
{
    public abstract void EnterState(AvatarStateManager avatar);

    public abstract void UpdateState(AvatarStateManager avatar);

}
