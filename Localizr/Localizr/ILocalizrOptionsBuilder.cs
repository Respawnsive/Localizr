using System;
using System.Collections.Generic;
using System.Text;

namespace Localizr
{
    public interface ILocalizrOptionsBuilder<out TLocalizrOptions> where TLocalizrOptions : class, ILocalizrOptions
    {
        public TLocalizrOptions LocalizrOptions { get; }
    }
}
