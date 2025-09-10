using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System.Collections.Generic;
public interface IFilePickerService
{
    Task<IReadOnlyList<IStorageFile>> OpenFilesAsync(FilePickerOpenOptions options);
    Task<IStorageFile?> SaveFileAsync(FilePickerSaveOptions options);
}