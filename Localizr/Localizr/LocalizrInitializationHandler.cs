﻿using System;
using System.Threading.Tasks;

namespace Localizr
{
    public class LocalizrInitializationHandler : ILocalizrInitializationHandler
    {
        private readonly ILocalizrOptionsBase _localizrOptions;

        public LocalizrInitializationHandler(ILocalizrOptionsBase localizrOptions)
        {
            _localizrOptions = localizrOptions;
        }

        public virtual Task<bool> OnInitializing(Func<Task<bool>> initializationTaskFactory, bool isFirstInitialization)
        {
            return _localizrOptions.AutoInitialize && isFirstInitialization ? Task.Run(async () => await initializationTaskFactory.Invoke()) : initializationTaskFactory.Invoke();
        }

        public virtual Task OnInitialized(LocalizrState state, bool isFirstInitialization)
        {
            return Task.CompletedTask;
        }
    }
}