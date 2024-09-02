namespace dotnetLearning.FactoryApp.View
{
    public interface IView
    {
        event EventHandler<UserInputEventArgs>? InputReceived;
        public string? GetUserInput(string message);
        public string? GetUserInput();
        public void ShowMessage(string? message);
        public void ClearView();
    }
    public class UserInputEventArgs : EventArgs
    {
        public string? UserInput { get; set; }
        public DateTime InputTime { get; set; }
    }
}