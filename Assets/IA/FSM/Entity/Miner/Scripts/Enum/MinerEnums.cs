namespace IA.FSM.Entity.Miner.Enum
{
    enum States
    {
        Idle,
        GoingToMine,
        Mining,
        ReturningToHome
    }

    enum Flags
    {
        OnSetMine,
        OnReachMine,
        OnFullInventory,
        OnEmptyMine,
        OnReachHome
    }
}
