﻿using System.Windows;
using Librotech_Inspection.ViewModels;

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

    public AppBootstrapper AppBootstrapper { get; protected set; }
}