using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LunaPad.ViewModels;

namespace LunaPad.Views;

public partial class EditorTab : UserControl
{
    public EditorTab()
    {
        AvaloniaXamlLoader.Load(this);
        // 订阅 DataContext 变化事件
        this.DataContextChanged += EditorTab_DataContextChanged;
    }

    private void EditorTab_DataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is EditorTabViewModel viewModel)
        {
            // 当 TextEditor 被附加到视觉树时，安装 TextMate
            viewModel.TextEditor.AttachedToVisualTree += (s, args) =>
            {
                viewModel.InstallTextMate();
            };
        }
    }
}
