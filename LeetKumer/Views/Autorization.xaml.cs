using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using LeetKumer.ViewModels;

namespace LeetKumer.Views;

public partial class Autorization : Window
{
    AutorizationViewModel autorizationViewModel = new AutorizationViewModel();
    public Autorization()
    {
        InitializeComponent();
        DataContext = autorizationViewModel;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        autorizationViewModel.Password = (sender as PasswordBox).Password;
    }
}