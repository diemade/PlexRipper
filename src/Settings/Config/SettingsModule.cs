﻿using Autofac;
using PlexRipper.Application.Common.Interfaces.Settings;


namespace PlexRipper.Settings.Config
{
    public class SettingsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserSettings>().As<IUserSettings>().SingleInstance();
        }
    }
}