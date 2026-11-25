namespace Game.Services.Config
{
    /// <summary>
    /// Where the game reads its numbers from. Screens depend on this interface,
    /// never on where the numbers come from (an asset today, a server later).
    /// The Lesson 4 slides write it as Services.Config.Economy; here it arrives by constructor injection.
    /// </summary>
    public interface IConfigService
    {
        IEconomyConfig Economy { get; }
        System.Collections.Generic.IReadOnlyList<Game.Services.Skills.SkillTreeDefinition> SkillTrees { get; }
    }
}
