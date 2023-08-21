namespace IA.FSM.Entity.Miner.Enums
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
