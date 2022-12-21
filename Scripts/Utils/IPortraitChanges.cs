namespace StarCraftCore
{
    public interface IPortraitChanges
    {
        bool ShouldRefreshPortrait();
        void RefreshPortrait();
    }
}