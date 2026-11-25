namespace Game.Services.Skills
{
    public readonly struct SkillContext
    {
        public SkillContext(string treeId, string ownerId)
        {
            TreeId = treeId;
            OwnerId = ownerId;
        }

        public string TreeId { get; }
        public string OwnerId { get; }
        public static bool IsValidOwner(string id) => SkillCatalog.ValidId(id);
        internal string Product(string skill) => $"skill/{TreeId}/{OwnerId}/{skill}";
    }
}
