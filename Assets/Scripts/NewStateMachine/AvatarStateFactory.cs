using System.Collections.Generic;

enum AvatarStates
{
    idle,
    wandering,
    speaking,
    thrown,
    subEmpty,
}

public class AvatarStateFactory
{
    AvatarStateMachine _context;
    Dictionary<AvatarStates, AvatarBaseState> _states = new Dictionary<AvatarStates, AvatarBaseState>();

    public AvatarStateFactory(AvatarStateMachine currentContext)
    {
        _context = currentContext;
        _states[AvatarStates.idle] = new AvatarIdleState(_context, this);
        _states[AvatarStates.wandering] = new AvatarWanderingState(_context, this);
        _states[AvatarStates.speaking] = new AvatarSpeakingSubState(_context, this);
        _states[AvatarStates.thrown] = new AvatarThrownState(_context, this);
        _states[AvatarStates.subEmpty] = new AvatarEmptySubState(_context, this); //Estado vacio para cuando no se deba ejecutar el speaking state
    }

    public AvatarBaseState Idle()
    {
        return _states[AvatarStates.idle];
    }
    public AvatarBaseState Wandering()
    {
        return _states[AvatarStates.wandering];
    }
    public AvatarBaseState Speaking()
    {
        return _states[AvatarStates.speaking];
    }
    public AvatarBaseState Thrown()
    {
        return _states[AvatarStates.thrown];
    }

    public AvatarBaseState Empty()
    {
        return _states[AvatarStates.subEmpty];
    }
}
