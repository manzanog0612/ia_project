namespace IA.FSM.Entity.MinerController.Enums
{
    public enum States
    {
        Mining = 4,
        WaitingForFood,
    }

    public enum Flags
    {
        OnHungry = 8,
        OnReceivedFood,
        OnFullInventory,
        OnEmptyMine,
    }
}
