﻿namespace OnboardingSystem
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new LoginPage();
            MenuInitializer.CreateTables();
        }
    }
}