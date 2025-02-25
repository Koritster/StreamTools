using System.Collections.Generic;

enum AvatarStates
{
    idle,
    wandering,
    speaking
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
}
