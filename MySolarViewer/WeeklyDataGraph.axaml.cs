using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MySolarViewer
{
    public partial class WeeklyDataGraph : UserControl
    {
        public WeeklyDataGraph()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
