using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MySolarViewer
{
    public partial class LiveView : UserControl
    {
        public LiveView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
