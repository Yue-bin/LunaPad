using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace LunaPad.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var _textEditor = this.FindControl<TextEditor>("Editor");
        var _registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var _textMateInstallation = _textEditor.InstallTextMate(_registryOptions);
        _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".cs").Id));
    }
}