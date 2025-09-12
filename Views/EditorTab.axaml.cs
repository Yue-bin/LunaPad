using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LunaPad.ViewModels;

namespace LunaPad.Views;

public partial class EditorTab : UserControl
{
    public EditorTab()
    {
        AvaloniaXamlLoader.Load(this);
        var editor = this.FindControl<AvaloniaEdit.TextEditor>("Editor");
        if (editor is not null)
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
