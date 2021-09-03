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

namespace StoreFilesByName
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private StoreFilesByNameViewModel vm = new StoreFilesByNameViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void selectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            vm.SelectFolderButtonClickCallback();
        }

        private void storeButton_Click(object sender, RoutedEventArgs e)
        {
            vm.StoreButtonClickCallback();
        }

        private void SetComponentsEnable(bool isStoring)
        {
            folderNameTextBox.IsEnabled = !isStoring;
            selectFolderButton.IsEnabled = !isStoring;
            letterLengthTextBox.IsEnabled = !isStoring;
            storeButton.IsEnabled = !isStoring;
            canselButton.IsEnabled = isStoring;
            if (isStoring)
            {
                storeButton.Visibility = Visibility.Hidden;
                canselButton.Visibility = Visibility.Visible;
            }
            else
            {
                storeButton.Visibility = Visibility.Visible;
                canselButton.Visibility = Visibility.Hidden;
            }
        }

        private void canselButton_Click(object sender, RoutedEventArgs e)
        {
            vm.CancelButtonClickCallback();
        }
    }
}
