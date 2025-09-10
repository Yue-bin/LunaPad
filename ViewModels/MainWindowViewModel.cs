using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace LunaPad.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public ObservableCollection<EditorTabViewModel> _editorTabs;

    [ObservableProperty]
    public EditorTabViewModel? _selectedTab;
    private readonly IFilePickerService _filePickerService;

    [ObservableProperty]
    private string? _selectedFilePath;
    public static FilePickerFileType CSharpFile { get; } = new("C# File")
    {
        Patterns = new[] { "*.cs" },
        AppleUniformTypeIdentifiers = new[] { "public.csharp-source" },
        MimeTypes = new[] { "text/x-csharp", "text/plain" }
    };

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

    public void OpenNewTabWithFile(string filePath)
    {
        var newTab = new EditorTabViewModel(filePath);
        EditorTabs.Add(newTab);
        SelectedTab = newTab;
    }

    [RelayCommand]
    public async Task OpenFileSelector()
    {
        var options = new FilePickerOpenOptions
        {
            Title = "选择一个C#文件",
            AllowMultiple = false,
            FileTypeFilter = new[] { CSharpFile, FilePickerFileTypes.All },
        };
        var files = await _filePickerService.OpenFilesAsync(options);
        if (files.Any())
        {
            SelectedFilePath = files[0].TryGetLocalPath();
        }
        OpenNewTabWithFile(SelectedFilePath!);
    }
    public MainWindowViewModel(IFilePickerService filePickerService)
    {
        _filePickerService = filePickerService;
        EditorTabs = [
            new EditorTabViewModel { EditorTitle = "Untitled-1" }
        ];
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