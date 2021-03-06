﻿using System;
using MagpieUpdater.ViewModels;

namespace MagpieUpdater.Views
{
    public partial class SignatureVerificationWindow
    {
        public SignatureVerificationWindow()
        {
            InitializeComponent();
            SetValue(NoIconBehavior.ShowIconProperty, false);
        }

        private void SignatureVerificationWindow_OnClosed(object sender, EventArgs e)
        {
            var viewModel = DataContext as SignatureVerificationWindowViewModel;
            if (viewModel != null)
            {
                viewModel.ContinueCommand.Execute(null);
            }
        }
    }
}