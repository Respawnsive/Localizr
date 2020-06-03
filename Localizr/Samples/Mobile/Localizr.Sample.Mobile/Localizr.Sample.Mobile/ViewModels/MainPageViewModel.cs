using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
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

            //Cultures = new ObservableCollection<CultureInfo>(LocalizrManager.AvailableCultures);
            SelectedCulture = LocalizrManager.CurrentCulture;

            LocalizrManager.WhenAvailableCulturesChanged()
                .ToPropertyEx(this, vm => vm.Cultures)
                .DisposedBy(DestroyWith);

            this.WhenAnyValue(x => x.SelectedCulture)
                .SubscribeAsync(SelectedCultureChanged)
                .DisposedBy(this.DestroyWith);
        }

        #region Properties

        public ObservableCollection<CultureInfo> Cultures { [ObservableAsProperty] get; }

        [Reactive] public CultureInfo SelectedCulture { get; set; }

        #endregion

        #region Methods

        private async Task SelectedCultureChanged(CultureInfo culture)
        {
            if (culture.Name != LocalizrManager.CurrentCulture.Name)
                await LocalizrManager.InitializeAsync(culture);
        }

        #endregion
    }
}
