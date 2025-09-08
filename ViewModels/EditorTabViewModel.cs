using System;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
}
