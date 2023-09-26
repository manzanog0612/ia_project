namespace IA.FSM.Common.Enums
{
    public enum CommonStates
    {
        SearchingMine,
        GoingToMine,
        ReturningToHome,
        Idle
    }

    public enum CommonFlags
    {
        OnSetMine,
        OnReachMine,
        OnNoMinesFound,
        OnReachHome,
        OnFinishJob
    }
}
