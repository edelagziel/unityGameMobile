namespace Game.ScreenNavigation
{
    public sealed class SettingsModel
    {
        public bool SoundEnabled { get; private set; } = true;

        public void SetSoundEnabled(bool enabled) => SoundEnabled = enabled;
    }
}
