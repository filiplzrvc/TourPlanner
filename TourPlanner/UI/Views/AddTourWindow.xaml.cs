using System.Windows;
using TourPlanner.UI.ViewModels;

namespace TourPlanner.UI.Views
{
    public partial class AddTourWindow : Window
    {
        public AddTourWindow(AddTourViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
