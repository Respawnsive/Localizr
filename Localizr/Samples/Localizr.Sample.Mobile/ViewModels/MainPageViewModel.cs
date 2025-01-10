using ReactiveUI;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive.Linq;
using ReactiveUI.SourceGenerators;
using Shiny;

namespace Localizr.Sample.Mobile.ViewModels
{
    public partial class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";

            LocalizrManager.WhenAvailableCulturesChanged()
                .StartWith(LocalizrManager.AvailableCultures)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(AvailableCulturesChanged)
                .DisposedBy(DestroyWith);

            this.WhenAnyValue(x => x.SelectedCulture)
                .SubscribeAsync(SelectedCultureChanged)
                .DisposedBy(DestroyWith);
        }

        #region Properties

        [Reactive] private ObservableCollection<CultureInfo> _cultures;

        [Reactive] private CultureInfo? _selectedCulture;

        #endregion

        #region Methods

        private void AvailableCulturesChanged(IList<CultureInfo> cultures)
        {
            if (cultures.Count > 0)
            {
                Cultures = new ObservableCollection<CultureInfo>(cultures);
                SelectedCulture = LocalizrManager.CurrentCulture;
            }
        }

        private async Task SelectedCultureChanged(CultureInfo? culture)
        {
            if (culture != null && culture.Name != LocalizrManager.CurrentCulture?.Name)
                await LocalizrManager.InitializeAsync(culture);
        }

        #endregion
    }
}
