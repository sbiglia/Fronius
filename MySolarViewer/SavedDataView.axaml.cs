using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MySolarViewer
{
    public partial class SavedDataView : UserControl
    {
        public SavedDataView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
