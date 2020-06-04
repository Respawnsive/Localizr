using System;
using System.Threading.Tasks;

namespace Localizr
{
    public interface ILocalizrInitializationHandler
    {
        Task<bool> OnInitializing(Func<Task<bool>> initializationTaskFactory, bool isFirstInitialization);

        Task OnInitialized(LocalizrState state, bool isFirstInitialization);
    }
}
