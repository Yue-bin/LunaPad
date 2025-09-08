using CommunityToolkit.Mvvm.Messaging.Messages;
using LunaPad.ViewModels;

public class CloseTabMessage(EditorTabViewModel tab) : RequestMessage<bool>
{
    public EditorTabViewModel Tab { get; } = tab;
}
