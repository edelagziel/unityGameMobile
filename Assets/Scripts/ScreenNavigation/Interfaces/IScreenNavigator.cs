namespace Game.ScreenNavigation
{
    public interface IScreenNavigator
    {
        bool Push<TView>() where TView : ScreenView;
        bool Pop();
        bool Replace<TView>() where TView : ScreenView;
        bool ResetTo<TView>() where TView : ScreenView;
    }
}
