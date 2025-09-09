using System.Collections.ObjectModel;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TextMateSharp.Grammars;

namespace LunaPad.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public ObservableCollection<EditorTabViewModel> _editorTabs;

    [ObservableProperty]
    public EditorTabViewModel? _selectedTab;

    private void CloseTab(EditorTabViewModel tab)
    {
        if (SelectedTab == tab)
        {
            if (EditorTabs.Count > 1)
            {
                SelectedTab = EditorTabs[(EditorTabs.IndexOf(tab) - 1) >= 0 ? (EditorTabs.IndexOf(tab) - 1) : (EditorTabs.IndexOf(tab) + 1)];
            }
            else
            {
                EditorTabs.Add(new() { EditorTitle = "Untitled-1" });
                _newTabCounter = 2;
                SelectedTab = EditorTabs[0];
            }
        }
        EditorTabs.Remove(tab);
    }

    private int _newTabCounter = 2;

    [RelayCommand]
    public void AddNewTab()
    {
        var newTabNumber = _newTabCounter++;
        var newTab = new EditorTabViewModel { EditorTitle = $"Untitled-{newTabNumber}" };
        EditorTabs.Add(newTab);
        SelectedTab = newTab;
    }
    public MainWindowViewModel()
    {
        EditorTabs = [
            new EditorTabViewModel { EditorTitle = "Untitled-1" }
        ];
        AddNewTab();
        AddNewTab();
        SelectedTab = EditorTabs[0];        // 注册接收关闭标签页消息
        Messenger.Register<CloseTabMessage>(this, (recipient, message) =>
        {
            CloseTab(message.Tab);
            message.Reply(true); // 确认消息已处理
        });
        Messenger.Register<AddTabMessage>(this, (recipient, message) =>
        {
            AddNewTab();
            message.Reply(true); // 确认消息已处理
        });
    }
}