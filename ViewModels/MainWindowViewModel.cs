using System.Collections.ObjectModel;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace LunaPad.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<EditorTabViewModel> EditorTabs { get; set; }
    public EditorTabViewModel? SelectedTab { get; set; }
    public MainWindowViewModel()
    {
        EditorTabs = new ObservableCollection<EditorTabViewModel>
        {
            new EditorTabViewModel { EditorTitle = "Untitled-1" },
            new EditorTabViewModel { EditorTitle = "Untitled-2" }
        };
        SelectedTab = EditorTabs[0];
    }
}