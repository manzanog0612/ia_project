namespace IA.FSM.Entity.MinerController.Enums
{
    enum States
    {
        SearchingCloserMine,
        GoingToMine,
        Mining,
        WaitingForFood,
        ReturningToHome
    }

    enum Flags
    {
        OnSetMine,
        OnReachMine,
        OnHungry,
        OnReceivedFood,
        OnFullInventory,
        OnEmptyMine,
        OnReachHome
    }
}
