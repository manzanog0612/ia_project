namespace IA.FSM.Entity.MinerController.Enums
{
    enum States
    {
        SearchingCloserMine,
        GoingToMine,
        Mining,
        WaitingForFood,
        ReturningToHome,
        Idle
    }

    enum Flags
    {
        OnSetMine,
        OnReachMine,
        OnHungry,
        OnReceivedFood,
        OnFullInventory,
        OnEmptyMine,
        OnReachHome,
        OnNoMinesFound,
        OnFinishJob
    }
}
