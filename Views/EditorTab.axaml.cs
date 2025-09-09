using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LunaPad.ViewModels;

namespace LunaPad.Views;

public partial class EditorTab : UserControl
{
    public EditorTab()
    {
        AvaloniaXamlLoader.Load(this);
        var editor = this.FindControl<AvaloniaEdit.TextEditor>("Editor");
        if (editor != null)
        {
            editor.AttachedToVisualTree += (s, e) =>
            {
                if (DataContext is EditorTabViewModel vm)
                {
                    vm.InstallTextMateCommand.Execute(editor);
                }
            };
        }
    }
}
