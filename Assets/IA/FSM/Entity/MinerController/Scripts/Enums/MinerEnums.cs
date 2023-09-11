namespace IA.FSM.Entity.MinerController.Enums
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
