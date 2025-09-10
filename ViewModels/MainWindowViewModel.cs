using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
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

    public void AddNewTabWithFile(string filePath)
    {
        var newTab = new EditorTabViewModel(filePath);
        // 如果只有一个未更改的空白标签页，则替换它
        if (EditorTabs.Count == 1 && EditorTabs[0].IsIgnorable)
        {
            EditorTabs[0] = newTab;
            SelectedTab = newTab;
            return;
        }
        EditorTabs.Add(newTab);
        SelectedTab = newTab;
    }

    private async Task SaveContentToFileAsync(IStorageFile file, EditorTabViewModel tab)
    {
        try
        {
            // 使用 TryGetLocalPath 是安全的做法
            var path = file.TryGetLocalPath();
            if (path != null)
            {
                // 使用异步写入，避免UI阻塞
                await File.WriteAllTextAsync(path, tab.Content ?? string.Empty);

                // 更新Tab的状态
                tab.FilePath = path;
                tab.EditorTitle = Path.GetFileName(path);

                // (可选但推荐) 如果你有“是否已修改”的状态，在这里把它重置
                // tab.IsModified = false;
            }
            else
            {
                // 处理无法获取本地路径的情况，例如云文件
                // 可以通过流来写入
                await using var stream = await file.OpenWriteAsync();
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync(tab.Content ?? string.Empty);

                tab.FilePath = file.Path.ToString(); // 使用URI路径
                tab.EditorTitle = file.Name;
                // tab.IsDirty = false;
            }
        }
        catch (Exception ex)
        {
            // 最好有错误处理，例如弹出一个错误提示框
            Console.WriteLine($"保存文件失败: {ex.Message}");
        }
    }


    [RelayCommand]
    public async Task OpenFile()
    {
        var options = new FilePickerOpenOptions
        {
            Title = "打开",
            AllowMultiple = false,
            FileTypeFilter = new[] { CSharpFile, FilePickerFileTypes.All },
        };
        var file = await _filePickerService.ExecuteOnStorageProvider<IStorageFile?>(
            async provider =>
            {
                var files = await provider.OpenFilePickerAsync(options);
                return files.FirstOrDefault();
            },
            null);
        string? selectedFilePath = null;
        if (file != null)
        {
            selectedFilePath = file.TryGetLocalPath();
            AddNewTabWithFile(selectedFilePath!);
        }
    }

    [RelayCommand]
    public async Task SaveFileAs(string title = "另存为")
    {
        if (SelectedTab == null) return;

        var options = new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = string.IsNullOrEmpty(SelectedTab.FilePath)
                ? "NewFile.cs"
                : Path.GetFileName(SelectedTab.FilePath),
            FileTypeChoices = new[]
            {
                CSharpFile,
                FilePickerFileTypes.All
            },
        };
        var file = await _filePickerService.ExecuteOnStorageProvider<IStorageFile?>(
            provider => provider.SaveFilePickerAsync(options),
            null);
        if (file != null)
        {
            var path = file.TryGetLocalPath();
            if (path != null)
            {
                await SaveContentToFileAsync(file, SelectedTab);
            }
        }
    }

    [RelayCommand]
    public async Task SaveFile()
    {
        if (SelectedTab == null) return;
        // 检查当前Tab是否已经有关联的文件路径
        if (string.IsNullOrEmpty(SelectedTab.FilePath))
        {
            // 路径为空，说明是新文件 -> 执行“另存为”逻辑
            await SaveFileAs("保存为");
        }
        else
        {
            // 路径存在 -> 直接在原路径上保存
            try
            {
                var file = await _filePickerService.ExecuteOnStorageProvider<IStorageFile?>(
                    provider => provider.TryGetFileFromPathAsync(SelectedTab.FilePath),
                    null);
                if (file != null)
                {
                    // 直接调用我们的辅助方法在原文件上写入
                    await SaveContentToFileAsync(file, SelectedTab);
                }
                else
                {
                    // 文件路径存在，但文件可能已被删除或移动
                    // 在这种情况下，也应该触发“另存为”
                    // 可以加一个提示信息给用户
                    await SaveFileAs("保存为");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存文件失败: {ex.Message}");
                // 如果直接保存失败（例如权限问题），也可以考虑触发“另存为”让用户选择新位置
                await SaveFileAs();
            }
        }
    }
    [RelayCommand]
    public void DragEnter(object? parameter)
    {
        if (parameter is not DragEventArgs args) return;
        // 检查拖动的数据是否包含文件
        if (args.Data.Contains(DataFormats.Files))
        {
            // 如果是文件，设置拖放效果为“复制”。
            // 这会改变鼠标指针的样式，给用户明确的反馈。
            args.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            // 如果不是文件，则禁止拖放
            args.DragEffects = DragDropEffects.None;
        }
    }

    // 命令：处理文件被放下（Drop）的事件
    [RelayCommand]
    public void FileDrop(IEnumerable<IStorageItem> files)
    {
        if (files is null || !files.Any()) return;
        // 遍历所有拖入的文件
        foreach (var file in files)
        {
            var path = file.TryGetLocalPath();
            if (path != null)
            {
                AddNewTabWithFile(path);
            }
        }
    }
    public MainWindowViewModel(IFilePickerService filePickerService)
    {
        _filePickerService = filePickerService;
        EditorTabs = [
            new EditorTabViewModel { EditorTitle = "Untitled-1" }
        ];
        SelectedTab = EditorTabs[0];
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