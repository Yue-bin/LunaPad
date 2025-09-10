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
    public async Task<IReadOnlyList<IStorageFile>> OpenFilesAsync(FilePickerOpenOptions options)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel is null) return System.Array.Empty<IStorageFile>();

        return await topLevel.StorageProvider.OpenFilePickerAsync(options);
    }
    public async Task<IStorageFile?> SaveFileAsync(FilePickerSaveOptions options)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel is null) return null;

        return await topLevel.StorageProvider.SaveFilePickerAsync(options);
    }
}