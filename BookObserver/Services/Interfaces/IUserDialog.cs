namespace BookObserver.Services.Interfaces
{
    internal interface IUserDialog
    {
        void ShowInformation(string message, string? caption = null);
        bool ShowWarning(string message, string? caption = null);
    }
}
