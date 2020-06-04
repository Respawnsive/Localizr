namespace Localizr
{
    public abstract class LocalizrOptionsBuilderBase<TLocalizrOptions> : ILocalizrOptionsBuilder<TLocalizrOptions> where TLocalizrOptions : class, ILocalizrOptions
    {
        protected readonly TLocalizrOptions Options;

        protected LocalizrOptionsBuilderBase(TLocalizrOptions localizrOptions)
        {
            Options = localizrOptions;
        }

        public ILocalizrOptions LocalizrOptions => Options;
    }
}
