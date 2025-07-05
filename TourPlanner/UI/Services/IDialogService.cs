using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TourPlanner.UI.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "Info");
        void CloseCurrentWindow();
        MessageBoxResult ConfirmDialog(string message, string title = "Confirm",
                                       MessageBoxButton buttons = MessageBoxButton.YesNo,
                                       MessageBoxImage icon = MessageBoxImage.Question);

    }
}
