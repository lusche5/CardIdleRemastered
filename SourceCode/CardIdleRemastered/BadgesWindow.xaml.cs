﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CardIdleRemastered.Properties;

namespace CardIdleRemastered
{
    /// <summary>
    /// Interaction logic for BadgesWindow.xaml
    /// </summary>
    public partial class BadgesWindow : Window
    {
        private AccountModel _account;

        public BadgesWindow()
        {
            InitializeComponent();
        }

        public AccountModel Account
        {
            get { return _account; }
            set
            {
                _account = value;
                DataContext = _account;
            }
        }

        private async void BadgesWindowLoaded(object sender, RoutedEventArgs e)
        {
            // load badges and new profile settings
            await Account.Sync();

            var queue = Settings.Default.IdleQueue.Cast<string>()
                .Join(Account.AllBadges, s => s, b => b.StringId, (s, badge) => badge)
                .ToList();
            foreach (var badge in queue)
                badge.EnqueueCmd.Execute(null);

            var blacklist = Settings.Default.blacklist.Cast<string>()
                .Join(Account.AllBadges, s => s, b => b.StringId, (s, badge) => badge)
                .ToList();
            foreach (var badge in blacklist)
                badge.IsBlacklisted = true;

            Account.Badges.Refresh();
        }

        private void SetLoadingRowNumber(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void SettingsDialog(object sender, RoutedEventArgs args)
        {
            App.CardIdle.SettingsDialog(Account);
        }

        private void BadgesWindowClosing(object sender, CancelEventArgs e)
        {
            Account.StopCardIdle();
        }
    }
}