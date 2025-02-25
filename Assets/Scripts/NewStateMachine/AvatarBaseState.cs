using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AvatarBaseState
{
    private bool _isRootState = false;
    private AvatarStateMachine _ctx;
    private AvatarStateFactory _factory;
    private AvatarBaseState _currentSubState;
    private AvatarBaseState _currentSuperState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected AvatarStateMachine Ctx { get { return _ctx; } }
    protected AvatarStateFactory Factory { get { return _factory; } }
    public AvatarBaseState(AvatarStateMachine currentContext, AvatarStateFactory AvatarStateFactory) {
        _ctx = currentContext;
        _factory = AvatarStateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates() {
        UpdateState();
        if(_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }
    protected void SwitchState(AvatarBaseState newState) {
        ExitState();


        if (_isRootState)
        {
            _ctx.CurrentState = newState;
        }else if(_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }
        newState.EnterState();

    }
    protected void SetSuperState(AvatarBaseState newSuperState) {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(AvatarBaseState newSubState) {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }


}
