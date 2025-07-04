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
            MessageBox.Show(message, type, MessageBoxButton.OK, this.GetMessageBoxImageType(type));
        }

       
    }
}
