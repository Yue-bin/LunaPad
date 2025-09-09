using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.TextMate;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TextMateSharp.Grammars;

namespace LunaPad.ViewModels;

public partial class EditorTabViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _editorTitle;

    [ObservableProperty]
    private bool _isTextMateInstalled;

    [ObservableProperty]
    private string? _filePath;

    private RegistryOptions _registryOptions;
    private TextMate.Installation? _textMateInstallation;

    public TextDocument Document { get; }
    public EditorTabViewModel()
    {
        _registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        Document = new TextDocument();
    }

    [RelayCommand]
    public void InstallTextMate(TextEditor editor)
    {
        if (editor == null || _textMateInstallation != null) return;

        _textMateInstallation = editor.InstallTextMate(_registryOptions);
        _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".cs").Id));
        IsTextMateInstalled = true;
    }

    [RelayCommand]
    public static void CopyMouseCommand(TextArea textArea)
    {
        ApplicationCommands.Copy.Execute(null, textArea);
    }

    [RelayCommand]
    public static void CutMouseCommand(TextArea textArea)
    {
        ApplicationCommands.Cut.Execute(null, textArea);
    }

    [RelayCommand]
    public static void PasteMouseCommand(TextArea textArea)
    {
        ApplicationCommands.Paste.Execute(null, textArea);
    }

    [RelayCommand]
    public static void SelectAllMouseCommand(TextArea textArea)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            ApplicationCommands.SelectAll.Execute(null, textArea);
        });
    }

    public string? Content
    {
        get => Document?.Text;
        set
        {
            if (Document != null && value != null)
            {
                Document.Text = value;
            }
        }
    }
    [RelayCommand]
    public void CloseMe()
    {
        // 发送关闭标签页消息
        WeakReferenceMessenger.Default.Send(new CloseTabMessage(this));
    }
    [RelayCommand]
    public static void AddNewTab()
    {
        // 发送添加新标签页消息
        WeakReferenceMessenger.Default.Send(new AddTabMessage());
    }

    public bool IsModified => !Document.UndoStack.IsOriginalFile;

    public bool IsEmpty => string.IsNullOrEmpty(Content);

    public bool IsNewFile => string.IsNullOrEmpty(FilePath);

    public bool IsIgnorable => IsEmpty && IsNewFile && !IsModified;
}
