namespace Heartline.SyncMatch
{
    /// <summary>Filled by GameDataService so Hub/Battle avoid circular App refs.</summary>
    public static class ContentRegistry
    {
        public static AgentCatalog Agents { get; private set; }
        public static StageCatalog Stages { get; private set; }

        public static void Set(AgentCatalog agents, StageCatalog stages)
        {
            Agents = agents;
            Stages = stages;
        }

        public static AgentDefinition GetAgent(string id)
        {
            if (Agents?.agents == null || string.IsNullOrEmpty(id)) return null;
            return Agents.agents.Find(a => a.id == id);
        }

        public static StageDefinition GetStage(string id)
        {
            if (Stages?.stages == null || string.IsNullOrEmpty(id)) return null;
            return Stages.stages.Find(s => s.id == id);
        }
    }
}
