﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CameraSlider.Frontend.Forms.Services;
using CameraSlider.Frontend.Shared.ViewModels;
using CameraSlider.Frontend.Shared.Services;
using GalaSoft.MvvmLight.Ioc;
using CameraSlider.Frontend.Forms.Pages;
using CameraSlider.Frontend.Shared.Models;

namespace CameraSlider.Frontend.Forms.Pages
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel viewModel;
        private IBluetoothLeService bluetoothLeService;

        private const string cameraSliderGuid = "00000000-0000-0000-0000-606405d147b4";
        private const string serviceUuid = "0000ffe0-0000-1000-8000-00805f9b34fb";
        private const string characteristicUuid = "0000ffe1-0000-1000-8000-00805f9b34fb";

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");

            viewModel = App.ServiceLocator.MainPageViewModel;
            bluetoothLeService = SimpleIoc.Default.GetInstance<IBluetoothLeService>();

            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //ConnectButton.TouchedUp += ConnectButton_TouchedUp;
            MoveLeftButton.TouchedDown += MoveLeftButton_TouchedDown;
            MoveLeftButton.TouchedUp += MoveButton_TouchedUp;
            MoveRightButton.TouchedDown += MoveRightButton_TouchedDown;
            MoveRightButton.TouchedUp += MoveButton_TouchedUp;
            TakePictureButton.TouchedUp += TakePictureButton_TouchedUp;

            await viewModel.TestConnectionAsync(serviceUuid, characteristicUuid);
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            await StopSliderMovement();

            MoveLeftButton.TouchedDown -= MoveLeftButton_TouchedDown;
            MoveLeftButton.TouchedUp -= MoveButton_TouchedUp;
            MoveRightButton.TouchedDown -= MoveRightButton_TouchedDown;
            MoveRightButton.TouchedUp -= MoveButton_TouchedUp;
            TakePictureButton.TouchedUp -= TakePictureButton_TouchedUp;
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var pos = viewModel.MenuItems.IndexOf(e.SelectedItem as Shared.Models.MenuItem);
            switch (pos)
            {
                case 0: // Device Selection
                    viewModel.NavigateToDeviceSelectionCommand.Execute(null);
                    break;
                case 1: // Configuration
                    viewModel.NavigateToConfigurationCommand.Execute(null);
                    break;
                default:
                    break;
            }

            (sender as ListView).SelectedItem = null;
        }

        void ConnectButton_TouchedUp()
        {
            //ConnectButton.IsEnabled = false;
            //viewModel.NavigateToDeviceSelectionCommand.Execute(null);
            //ConnectButton.IsEnabled = true;
        }

        async void TakePictureButton_TouchedUp()
        {
            await bluetoothLeService.WriteToServiceCharacteristicAsync($"et{viewModel.ExposureTime.Milliseconds}#", serviceUuid, characteristicUuid);
            await bluetoothLeService.WriteToServiceCharacteristicAsync("shutter#", serviceUuid, characteristicUuid);
        }

        async void MoveLeftButton_TouchedDown()
        {
            //SpeedSlider.IsEnabled = false;
            await StartSliderMovement(SliderDirection.Left);
        }

        async void MoveRightButton_TouchedDown()
        {
            //SpeedSlider.IsEnabled = false;
            await StartSliderMovement(SliderDirection.Right);
        }

        async void MoveButton_TouchedUp()
        {
            await StopSliderMovement();
            //SpeedSlider.IsEnabled = true;
        }

        async Task StartSliderMovement(SliderDirection direction)
        {
            // Direction
            var directionCommand = direction == SliderDirection.Right ? "dr#" : "dl#";
            await bluetoothLeService.WriteToServiceCharacteristicAsync(directionCommand, serviceUuid, characteristicUuid);

            // Speed
            //var speedValue = 2500 - (int)SpeedSlider.Value;
            var speedValue = 500;
            await bluetoothLeService.WriteToServiceCharacteristicAsync($"sp{speedValue}#", serviceUuid, characteristicUuid);

            // Start
            await bluetoothLeService.WriteToServiceCharacteristicAsync("on#", serviceUuid, characteristicUuid);
        }

        async Task StopSliderMovement()
        {
            await bluetoothLeService.WriteToServiceCharacteristicAsync("off#", serviceUuid, characteristicUuid);
        }
    }
}
