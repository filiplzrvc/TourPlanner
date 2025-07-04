using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TourPlanner.UI.Services
{
    public class DialogService : IDialogService
    {
        public static bool SuppressDialogs { get; set; } = false;
        private MessageBoxImage GetMessageBoxImageType(string type)
        {
            MessageBoxImage icon;

            switch (type.ToLower())
            {
                case "success":
                    icon = MessageBoxImage.Information;
                    break;

                case "error":
                    icon = MessageBoxImage.Error;
                    break;

                default:
                    icon = MessageBoxImage.Information;
                    break;
            }

            return icon;
        }
        public void ShowMessage(string message, string type)
        {
            if (SuppressDialogs) return;
            MessageBox.Show(message, type, MessageBoxButton.OK, this.GetMessageBoxImageType(type));
        }

        public void CloseCurrentWindow()
        {
            if (Application.Current != null && Application.Current.Windows.Count > 1)
            {
                Application.Current.Windows[1]?.Close();
            }
        }

    }
}
