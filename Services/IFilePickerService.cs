using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
public interface IFilePickerService
{
    public Task<TResult> ExecuteOnStorageProvider<TResult>(
    Func<IStorageProvider, Task<TResult>> action,
    TResult defaultValue);
}