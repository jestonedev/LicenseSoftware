namespace LicenseSoftware.Viewport
{
    public interface IMenuCallback
    {
        void TabsStateUpdate();
        void NavigationStateUpdate();
        void EditingStateUpdate();
        void RelationsStateUpdate();
        void StatusBarStateUpdate();
        void ForceCloseDetachedViewports();
        void AddViewport(Viewport viewport);
        void SwitchToPreviousViewport();
    }
}
