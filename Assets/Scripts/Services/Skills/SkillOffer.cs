namespace Game.Services.Skills
{
    public readonly struct SkillOffer
    {
        public SkillOffer(int current, int price, SkillAvailability state, string reason = null)
        {
            Current = current;
            Price = price;
            State = state;
            Reason = reason;
        }
        public int Current { get; }
        public int Price { get; }
        public string Reason { get; }
        public SkillAvailability State { get; }
        public bool Available => State == SkillAvailability.Available;
    }
}
