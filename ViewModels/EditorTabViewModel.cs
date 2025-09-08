using System;
using AvaloniaEdit;
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
    private TextEditor? _textEditor;

    [ObservableProperty]
    private bool _isTextMateInstalled;

    [ObservableProperty]
    private string? _filePath;

    private RegistryOptions _registryOptions;
    private TextMate.Installation? _textMateInstallation;

    public EditorTabViewModel()
    {
        _registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        InitializeEditor();
    }

    private void InitializeEditor()
    {
        TextEditor = new TextEditor
        {
            FontFamily = "Cascadia Code,Consolas,Menlo,Monospace",
            FontSize = 14,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            ShowLineNumbers = true,
            Watermark = "Just code, just live, just love."
        };

        // 订阅编辑器附加到视觉树的事件
        if (TextEditor != null)
        {
            TextEditor.AttachedToVisualTree += (s, args) =>
            {
                if (!IsTextMateInstalled)
                {
                    InstallTextMate();
                }
            };
        }
    }

    [RelayCommand]
    public void InstallTextMate()
    {
        if (TextEditor == null || _textMateInstallation != null) return;

        _textMateInstallation = TextEditor.InstallTextMate(_registryOptions);
        _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".cs").Id));
        IsTextMateInstalled = true;
    }

    [RelayCommand]
    public void CopyMouseCommand(TextArea textArea)
    {
        ApplicationCommands.Copy.Execute(null, textArea);
    }

    [RelayCommand]
    public void CutMouseCommand(TextArea textArea)
    {
        ApplicationCommands.Cut.Execute(null, textArea);
    }

    [RelayCommand]
    public void PasteMouseCommand(TextArea textArea)
    {
        ApplicationCommands.Paste.Execute(null, textArea);
    }

    [RelayCommand]
    public void SelectAllMouseCommand(TextArea textArea)
    {
        ApplicationCommands.SelectAll.Execute(null, textArea);
    }

    public string? Content
    {
        get => TextEditor?.Document?.Text;
        set
        {
            if (TextEditor != null && value != null)
            {
                TextEditor.Document.Text = value;
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
    public void AddNewTab()
    {
        // 发送添加新标签页消息
        WeakReferenceMessenger.Default.Send(new AddTabMessage());
    }

    public bool IsModified => TextEditor?.IsModified ?? false;

    public bool IsEmpty => string.IsNullOrEmpty(Content);

    public bool IsNewFile => string.IsNullOrEmpty(FilePath);

    public bool IsIgnorable => IsEmpty && IsNewFile && !IsModified;
}
