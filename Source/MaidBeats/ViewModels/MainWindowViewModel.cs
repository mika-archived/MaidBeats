﻿using System;
using System.Reactive.Linq;

using MaidBeats.Extensions;
using MaidBeats.Models;
using MaidBeats.Models.BeatMods;
using MaidBeats.Mvvm;
using MaidBeats.ViewModels.Tabs;

using Reactive.Bindings;

namespace MaidBeats.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public ReactiveProperty<string> Title { get; }
        public ReactiveCollection<TabBaseViewModel> TabItems { get; }
        public ReactiveProperty<int> SelectedTabIndex { get; }

        public MainWindowViewModel(BeatSaber beatSaber, BeatModsClient client)
        {
            Title = new ReactiveProperty<string>("MaidBeats - Mod Installer / Manager for Beat Saber").AddTo(this);
            TabItems = new ReactiveCollection<TabBaseViewModel>
            {
                new ModsTabViewModel(beatSaber, client).AddTo(this),
                new SettingsTabViewModel(beatSaber, client).AddTo(this),
                new AboutTabViewModel().AddTo(this)
            }.AddTo(this);
            SelectedTabIndex = new ReactiveProperty<int>(0);
            SelectedTabIndex.Skip(1).AsObservable().Subscribe(async w => await TabItems[w].InitializeAsync()).AddTo(this);
        }
    }
}