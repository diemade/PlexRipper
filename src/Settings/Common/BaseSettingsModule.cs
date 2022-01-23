﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text.Json;
using FluentResults;
using PlexRipper.Application;
using PlexRipper.Domain;

namespace PlexRipper.Settings
{
    public abstract class BaseSettingsModule<TModel> : IBaseSettingsModule<TModel> where TModel : class
    {
        #region Fields

        private readonly Subject<TModel> _moduleUpdatedSubject = new();

        #endregion

        #region Constructor

        protected BaseSettingsModule()
        {
            Reset();
        }

        #endregion

        #region Properties

        public IObservable<TModel> ModuleHasChanged => _moduleUpdatedSubject.AsObservable();

        public abstract string Name { get; }

        public abstract TModel DefaultValues { get; }

        #endregion

        #region Public Methods

        public abstract TModel GetValues();

        public TModel Reset()
        {
            return Update(DefaultValues);
        }

        /// <inheritdoc/>
        public virtual Result SetFromJson(JsonElement jsonElement)
        {
            var jsonSettings = GetJsonSettingsModule(jsonElement);
            if (jsonSettings.IsFailed)
            {
                Reset();
                return jsonSettings;
            }

            var rootSettingsModule = jsonSettings.Value;
            foreach (PropertyInfo prop in typeof(TModel).GetProperties())
            {
                if (rootSettingsModule.TryGetProperty(prop.Name, out JsonElement jsonValueElement))
                {
                    var targetProp = GetType().GetProperty(prop.Name);
                    var targetPropType = targetProp.PropertyType;
                    var targetValue = targetProp.GetValue(this, null);

                    var sourceValue = jsonValueElement.GetTypedValue(targetPropType);
                    if (sourceValue != targetValue)
                    {
                        targetProp.SetValue(this, sourceValue);
                    }
                }
            }

            return Result.Ok();
        }

        public TModel Update(TModel sourceSettings)
        {
            var hasChanged = false;

            foreach (PropertyInfo prop in typeof(TModel).GetProperties())
            {
                var sourceProp = sourceSettings.GetType().GetProperty(prop.Name);
                var sourceValue = sourceProp.GetValue(sourceSettings, null);
                var targetProp = GetType().GetProperty(prop.Name);
                var targetValue = targetProp.GetValue(this, null);
                if (sourceValue != targetValue)
                {
                    targetProp.SetValue(this, sourceValue);
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                EmitModuleHasChanged(GetValues());
            }

            return GetValues();
        }

        #endregion

        #region Private Methods

        protected void EmitModuleHasChanged(TModel module)
        {
            _moduleUpdatedSubject.OnNext(module);
        }

        protected Result<JsonElement> GetJsonSettingsModule(JsonElement jsonElement)
        {
            if (jsonElement.TryGetProperty(Name, out JsonElement rootSettingsModule))
            {
                return Result.Ok(rootSettingsModule);
            }

            return Result.Fail($"Could not find settings module {Name} in config file").LogError();
        }

        #endregion
    }
}