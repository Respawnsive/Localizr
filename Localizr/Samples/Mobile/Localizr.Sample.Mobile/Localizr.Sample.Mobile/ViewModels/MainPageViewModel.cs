using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;

namespace Localizr.Sample.Mobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";

            LocalizrManager.WhenAvailableCulturesChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(AvailableCulturesChanged)
                .DisposedBy(DestroyWith);

            this.WhenAnyValue(x => x.SelectedCulture)
                .SubscribeAsync(SelectedCultureChanged)
                .DisposedBy(DestroyWith);
        }

        #region Properties

        [Reactive] public ObservableCollection<CultureInfo> Cultures { get; set; }

        [Reactive] public CultureInfo SelectedCulture { get; set; }

        #endregion

        #region Methods

        private void AvailableCulturesChanged(IList<CultureInfo> cultures)
        {
            if (!cultures.IsEmpty())
            {
                Cultures = new ObservableCollection<CultureInfo>(cultures);
                SelectedCulture = LocalizrManager.CurrentCulture;
            }
        }

        private async Task SelectedCultureChanged(CultureInfo culture)
        {
            if (culture != null && culture.Name != LocalizrManager.CurrentCulture?.Name)
                await LocalizrManager.InitializeAsync(culture);
        }

        #endregion
    }
}
