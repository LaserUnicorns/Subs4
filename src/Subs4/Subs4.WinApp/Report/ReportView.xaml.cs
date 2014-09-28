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
using Microsoft.Practices.Unity;

namespace Subs4.WinApp.Report
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();
        }

        [Dependency]
        public ReportViewModel ViewModel
        {
            get { return (ReportViewModel) DataContext; }
            set { DataContext = value; }
        }

        private void UpdateTotals(object sender, RoutedEventArgs e)
        {
            PersonsDataGrid.Items.Refresh();
        }
    }
}
