using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localizr.Resx;

namespace Localizr
{
    public abstract class LocalizrOptionsBuilderBase<TLocalizrOptions> : ILocalizrOptionsBuilder<TLocalizrOptions> where TLocalizrOptions : class, ILocalizrOptions
    {
        protected LocalizrOptionsBuilderBase(TLocalizrOptions localizrOptions)
        {
            LocalizrOptions = localizrOptions;
        }

        public TLocalizrOptions LocalizrOptions { get; }

        /// <summary>
        /// Adjust auto initialization settings
        /// </summary>
        /// <param name="autoInitialize">True to initialize localizr on app startup from a background job or False for on demand initialization (default: true)</param>
        /// <param name="tryParents">Try with parent culture up to invariant when the asked one can't be found (default: true)</param>
        /// <param name="refreshAvailableCultures">Refresh AvailableCultures property during initialization (default: true)</param>
        /// <param name="initializationCulture">Culture used for auto initialization</param>
        /// <returns></returns>
        public virtual ILocalizrOptionsBuilder<TLocalizrOptions> WithAutoInitialization(bool autoInitialize = true, bool tryParents = true, bool refreshAvailableCultures = true, CultureInfo? initializationCulture = null)
        {
            LocalizrOptions.AutoInitialize = autoInitialize;
            if (autoInitialize)
            {
                LocalizrOptions.TryParents = tryParents;
                LocalizrOptions.RefreshAvailableCultures = refreshAvailableCultures;
                LocalizrOptions.InitializationCulture = initializationCulture;
            }

            return this;
        }

        /// <summary>
        /// Specify the default culture used as invariant for all text providers
        /// </summary>
        /// <param name="defaultInvariantCulture">Culture used as invariant (default: null = InvariantCulture)</param>
        /// <returns></returns>
        public virtual ILocalizrOptionsBuilder<TLocalizrOptions> WithDefaultInvariantCulture(CultureInfo defaultInvariantCulture)
        {
            LocalizrOptions.DefaultInvariantCulture = defaultInvariantCulture;

            return this;
        }
    }
}
