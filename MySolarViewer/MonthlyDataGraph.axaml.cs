using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MySolarViewer
{
    public partial class MonthlyDataGraph : UserControl
    {
        public MonthlyDataGraph()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
