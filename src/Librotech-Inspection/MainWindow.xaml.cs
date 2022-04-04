using System.Windows;
using System.Windows.Media;
using Librotech_Inspection.ViewModels;
using WPFUI.Controls;

namespace Librotech_Inspection;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        AppBootstrapper = new AppBootstrapper();
        DataContext = AppBootstrapper;
    }

    // just a quick crutch in five minutes, I'll rewrite later
    private void HighlightNavigationButton(object sender, RoutedEventArgs e)
    {
        var btn = (System.Windows.Controls.Button) sender;
        var highlightedBrush = Application.Current.FindResource("MainBrush") as SolidColorBrush;
        var defBrush = Application.Current.FindResource("NavigationButtonBrush") as SolidColorBrush;

        NavigateToDataAnalysisButton.Background = defBrush;
        NavigateToLoggerConfigurationButton.Background = defBrush;
        NavigateToTableButton.Background = defBrush;
        
        switch (btn.Content)
        {
            case "Анализ данных":
                NavigateToDataAnalysisButton.Background = highlightedBrush;
                break;
            case "Конфигурация":
                NavigateToLoggerConfigurationButton.Background = highlightedBrush;
                break;
            case "Таблица данных":
                NavigateToTableButton.Background = highlightedBrush;
                break;
        }
    }

    public AppBootstrapper AppBootstrapper { get; protected set; }
    
}