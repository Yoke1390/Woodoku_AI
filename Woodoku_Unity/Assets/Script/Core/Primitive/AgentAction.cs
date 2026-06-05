namespace Script.Core.Primitive
{
    public readonly struct AgentAction
    {
        public int SlotIndex { get; }
        public BoardPosition BasePosition { get; }

        public AgentAction(int slotIndex, BoardPosition basePosition)
        {
            SlotIndex = slotIndex;
            BasePosition = basePosition;
        }
    }
}