using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Localizr;
using Shiny.Jobs;

namespace Shiny
{
    public class LocalizrJob : IJob
    {
        private readonly ILocalizrManager _localizrManager;
        private readonly ILocalizrOptions _localizrOptions;

        public LocalizrJob(ILocalizrManager localizrManager, ILocalizrOptions localizrOptions)
        {
            _localizrManager = localizrManager;
            _localizrOptions = localizrOptions;
        }

        public Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken) =>
            _localizrManager.InitializeAsync(_localizrOptions.InitializationCulture,
                _localizrOptions.TryParents, _localizrOptions.RefreshAvailableCultures, cancelToken);
    }
}
