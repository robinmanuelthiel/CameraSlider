﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CameraSlider.Frontend.Shared.ViewModels;

namespace CameraSlider.Frontend.Forms.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        private ConfigurationViewModel viewModel;

        public ConfigurationPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");

            BindingContext = viewModel = App.ServiceLocator.ConfigurationViewModel;
        }
    }
}
