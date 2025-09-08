using System;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace LunaPad.ViewModels;

public partial class EditorTabViewModel : ViewModelBase
{
    public String? EditorTitle { get; set; }
    public TextEditor TextEditor { get; set; }
    private RegistryOptions _registryOptions;
    private TextMate.Installation? _textMateInstallation;

    public EditorTabViewModel()
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

        _registryOptions = new RegistryOptions(ThemeName.DarkPlus);
    }

    public void InstallTextMate()
    {
        if (_textMateInstallation == null)
        {
            _textMateInstallation = TextEditor.InstallTextMate(_registryOptions);
            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".cs").Id));
        }
    }
}
