using System;
using System.Threading.Tasks;

namespace Localizr
{
    public interface ILocalizrInitializationHandler
    {
        /// <summary>
        /// Method called when <see cref="ILocalizrManager"/> InitializeAsync method is called
        /// </summary>
        /// <param name="initializationTaskFactory">The <see cref="ILocalizrManager"/> InitializeAsync Task</param>
        /// <param name="isFirstInitialization">Tells if Localizr has not been initialized before</param>
        /// <returns></returns>
        Task<bool> OnInitializing(Func<Task<bool>> initializationTaskFactory, bool isFirstInitialization);

        /// <summary>
        /// Method called when <see cref="ILocalizrManager"/> InitializeAsync method is completed
        /// </summary>
        /// <param name="state">Localizr current state</param>
        /// <param name="isFirstInitialization">Tells if Localizr has not been initialized before</param>
        /// <returns></returns>
        Task OnInitialized(LocalizrState state, bool isFirstInitialization);
    }
}
