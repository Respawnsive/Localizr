using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.SourceGenerators;

namespace Localizr.Sample.Mobile.ViewModels
{
    public abstract partial class ViewModelBase : ReactiveObject,
        IInitialize,
        IInitializeAsync,
        INavigatedAware,
        IPageLifecycleAware,
        IDestructible,
        IConfirmNavigationAsync
    {
        protected ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
            LocalizrManager = Shiny.Hosting.Host.GetService<ILocalizrManager>()!;

            LocalizrManager.WhenLocalizrStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(LocalizationStatusChanged)
                .DisposeWith(DestroyWith);
        }

        #region Properties

        protected INavigationService NavigationService { get; private set; }
        protected ILocalizrManager LocalizrManager { get; }

        public string this[string name] => LocalizrManager.GetText(name);

        private int _busyCounter;
        protected int BusyCounter
        {
            get => _busyCounter;
            set
            {
                _busyCounter = Math.Max(value, 0);
                IsBusy = _busyCounter > 0;
            }
        }

        [Reactive] private bool _isBusy;

        [Reactive] private string _title;

        #endregion

        #region Methods

        private void LocalizationStatusChanged(LocalizrState state)
        {
            if (state > LocalizrState.Initializing)
                this.RaisePropertyChanged("Item");
        }

        #endregion

        #region Lifecycle

        private CompositeDisposable _deactivateWith;
        protected CompositeDisposable DeactivateWith => _deactivateWith ??= new CompositeDisposable();

        protected CompositeDisposable DestroyWith { get; } = new CompositeDisposable();

        protected virtual void Deactivate()
        {
            _deactivateWith?.Dispose();
            _deactivateWith = null;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters) => Deactivate();

        public virtual void Initialize(INavigationParameters parameters) { }

        public virtual Task InitializeAsync(INavigationParameters parameters) => Task.CompletedTask;

        public virtual void OnNavigatedTo(INavigationParameters parameters) { }

        public virtual void OnAppearing() { }

        public virtual void OnDisappearing() { }

        public virtual void Destroy() => DestroyWith?.Dispose();

        public virtual Task<bool> CanNavigateAsync(INavigationParameters parameters) => Task.FromResult(true);

        public virtual Task NavigateBackAsync() => NavigationService.GoBackAsync();

        #endregion
    }
}
