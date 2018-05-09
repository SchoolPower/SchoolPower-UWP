﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SchoolPower.Models;

namespace SchoolPower.Views.Dialogs {
    public sealed partial class GPADialog : ContentDialog {

        private String selectedCombo = "";
        private String selectedRadioBut = "";
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public GPADialog() {
            this.InitializeComponent();
            comboBox.SelectedIndex = 0;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e) {
            RadioButton radioButton = sender as RadioButton;
            selectedRadioBut = radioButton.Tag.ToString();
            switch (radioButton.Tag.ToString()) {
                case "All":
                    GPA.Text = StudentData.GetAllGPA(selectedCombo).ToString() + "%";
                    break;
                case "Custom":
                    switch (localSettings.Values["CalculateRule"]) {
                        case "0":
                            GPA.Text = StudentData.GetSomeGPA(selectedCombo).ToString() + "%";
                            break;
                        case "1":
                            GPA.Text = StudentData.GetSomeGPA(selectedCombo).ToString() + "%";
                            break;
                        case "2":
                            GPA.Text = StudentData.GetSomeGPA(selectedCombo).ToString() + "%";
                            break;
                        case "3":
                            GPA.Text = StudentData.GetSomeGPA(selectedCombo).ToString() + "%";
                            break;
                    } break;
                case "Official":
                    try {
                        if (StudentData.info.GPA.Equals(null)) { }
                        GPA.Text = StudentData.info.GPA + "%";
                    } catch (System.NullReferenceException) {
                        GPA.Text = "NaN%";
                    } break;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var container = sender as ComboBox;
            var selected = container.SelectedItem as ComboBoxItem;
            selectedCombo = selected.Content.ToString();
            switch (selectedRadioBut){
                case "All":
                    GPA.Text = StudentData.GetAllGPA(selectedCombo).ToString() + "%";
                    break;
                case "Custom":
                    switch (localSettings.Values["CalculateRule"]) {
                        case "0":
                            GPA.Text = StudentData.GetSomeGPA(selectedCombo).ToString() + "%";
                            break;
                        case "1":
                            GPA.Text = StudentData.GetSomeGPA(selectedCombo).ToString() + "%";
                            break;
                        case "2":
                            GPA.Text = StudentData.GetSomeGPA(selectedCombo).ToString() + "%";
                            break;
                        case "3":
                            GPA.Text = StudentData.GetSomeGPA(selectedCombo).ToString() + "%";
                            break;
                    } break;
                case "Official":
                    try {
                        if (StudentData.info.GPA.Equals(null)) { }
                        GPA.Text = StudentData.info.GPA + "%";
                    } catch (System.NullReferenceException) {
                        GPA.Text = "NaN%";
                    } break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Hide();
        }
    }
}
