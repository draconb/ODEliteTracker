using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ODEliteTracker.Views.PopOuts
{
    /// <summary>
    /// Interaction logic for CompassPopOutView.xaml
    /// </summary>
    public partial class CompassPopOutView : UserControl
    {
        public CompassPopOutView()
        {
            InitializeComponent();
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            if (!object.Equals(this.SettingsBtn, Mouse.DirectlyOver))
            {
                this.SettingsBtn.IsChecked = false;
            }
        }
    }
}
