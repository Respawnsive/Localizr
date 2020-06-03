using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.AppModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;

namespace Localizr.Sample.Mobile.ViewModels
{
    public class ViewModelBase : ReactiveObject,
        IAutoInitialize,
        IInitialize,
        IInitializeAsync,
        INavigatedAware,
        IPageLifecycleAware,
        IDestructible,
        IConfirmNavigationAsync
    {
        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
            LocalizrManager = ShinyHost.Resolve<ILocalizrManager>();

            LocalizrManager.WhenLocalizrStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(LocalizationStatusChanged)
                .DisposeWith(this.DestroyWith);
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

        [Reactive]
        public bool IsBusy { get; private set; }

        [Reactive]
        public string Title { get; protected set; }

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

        public virtual void OnNavigatedFrom(INavigationParameters parameters) => this.Deactivate();

        public virtual void Initialize(INavigationParameters parameters) { }

        public virtual Task InitializeAsync(INavigationParameters parameters) => Task.CompletedTask;

        public virtual void OnNavigatedTo(INavigationParameters parameters) { }

        public virtual void OnAppearing() { }

        public virtual void OnDisappearing() { }

        public virtual void Destroy() => this.DestroyWith?.Dispose();

        public virtual Task<bool> CanNavigateAsync(INavigationParameters parameters) => Task.FromResult(true);

        public virtual Task NavigateBackAsync() => NavigationService.GoBackAsync();

        #endregion
    }
}
