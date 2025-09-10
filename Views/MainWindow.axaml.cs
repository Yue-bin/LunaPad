using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace LunaPad.Views;

public partial class MainWindow : Window, IFilePickerService
{
    public MainWindow()
    {
        InitializeComponent();
    }
    // 在你的 MainWindow.axaml.cs 中添加这个私有帮助方法
    public async Task<TResult> ExecuteOnStorageProvider<TResult>(
        Func<IStorageProvider, Task<TResult>> action,
        TResult defaultValue)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel?.StorageProvider is null)
        {
            return defaultValue;
        }

        return await action(topLevel.StorageProvider);
    }
}