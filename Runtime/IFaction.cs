namespace ToolkitEngine.AI
{
    public interface IFaction
    {
        public FactionType factionType { get; }
		bool enabled { get; }
	}
}