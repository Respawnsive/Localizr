# Localizr
Localizr is a .Net Standard 2.0 library helping to manage various text resources type (resx, json, xml, database, etc), from various origin (assembly, web service, database, etc), for various cultures, and all mixable.

## Libraries

|Project|NuGet|
|-------|-----|
|Localizr|[![NuGet](https://img.shields.io/nuget/v/Localizr.svg?maxAge=2592000)](https://www.nuget.org/packages/Localizr/)|
|Localizr.Extensions.Microsoft.DependencyInjection|[![NuGet](https://img.shields.io/nuget/v/Localizr.Extensions.Microsoft.DependencyInjection.svg?maxAge=2592000)](https://www.nuget.org/packages/Localizr.Extensions.Microsoft.DependencyInjection/)|

Install the NuGet package of your choice:

   - Localizr package comes with the static instantiation approach (wich you can register in your DI container then)
   - Localizr.Extensions.Microsoft.DependencyInjection package extends your IServiceCollection with an AddLocalizr registration method (ASP.Net Core, Shiny, etc)

## Getting started

Examples here are based on a Xamarin.Forms app working with Shiny. 
You'll find a sample Xamarin.Forms app browsing code, implementing Localizr with Shiny, Prism and DryIoc all together.

You'll find another sample app but .Net Core console this time, implementing Localizr without anything else (static) and also with MS DI (extensions).

So please, take a look at the samples :)

### Registering

As it's not mandatory to register anything in a container for DI purpose (you can use a static instance directly), I'll describe here how to use it with DI.

#### Static approach

Somewhere where you can add services to your container, add the following:
```csharp
// This is an example where YourResourcesDesignerClass is the class name of the resx .Designer.cs auto generated file
myContainer.SomeInstanceRegistrationMethod<ILocalizrManager>(Localizr.For<ResxTextProvider<YourResourcesDesignerClass>>(optionsBuilder =>
                optionsBuilder.WithAutoInitialization());
```

I asked for AutoInitialization here so that everything is in place when first view pushed on screen.

Anyway, you definitly can initialize it yourself calling the manager's InitializeAsync method from anywhere in your app. In this case, I'd prefer registering with a factory like:
```csharp
// This is an example where YourResourcesDesignerClass is the class name of the resx .Designer.cs auto generated file
myContainer.SomeFactoryRegistrationMethod<ILocalizrManager>(() => Localizr.For<ResxTextProvider<YourResourcesDesignerClass>>());
```

#### Extensions approach

In your Startup class, add the following:
```csharp

// This is an example where YourResourcesDesignerClass is the class name of the resx .Designer.cs auto generated file
public override void ConfigureServices(IServiceCollection services)
{
    services.AddLocalizr<ResxTextProvider<YourResourcesDesignerClass>>(optionsBuilder =>
                    optionsBuilder.WithAutoInitialization());
}
```

### Using

#### Localizing

1. Localize your application - e.g. using binding in a Xamarin.Forms mobile app

   1. Inject ILocalizrManager where you need it - e.g. into a ViewModel contructor
   ```csharp

    public class YourViewModel
    {
        private readonly ILocalizrManager _localizrManager;
	
        public YouViewModel(ILocalizrManager localizrManager)
        {
		    _localizrManager = localizrManager;
        }

	    public string this[string key] => _localizrManager.GetText(key);
    }
    ```

    2. Just bind to [YourKeyToLocalize]
    ```xml

    <ContentPage xmlns="http://xamarin.com/schemas/2014/forms" 
                 xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                 xmlns:d="http://xamarin.com/schemas/2014/forms/design"
                 xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
                 mc:Ignorable="d"
                 x:Class="Whatever.Sample.LocalizedPage"
                 Title="{Binding [LocalizePage_Title]}">
	    <ContentPage.Content>
        </ContentPage.Content>
    </ContentPage>
    ```

#### Changing language

You can change the language at runtime if needed by initializing the manager with the asked culture
    
```csharp
await _localizrManager.InitializeAsync(culture: null, tryParents: true, refreshAvailableCultures: false, token: default)
```

- culture: the culture you ask localization for (default: null = CurrentUICulture)
- tryParents: true to try with parent culture up to invariant when the asked one can't be found (default: true)
- refreshAvailableCultures: true to refresh AvailableCultures property during initialization (default: false)
- token: optional cancellation token

## Configuring

There're some advanced scenarios where you want to adjust some settings and behaviors.
This is where the options builder comes in.
Each registration approach comes with its optionsBuilder:
```csharp
something.WhateverRegistrationApproach<WhateverTextProvider>(optionsBuilder =>
                optionsBuilder.SomeOptionsHere(someParametersThere));
```

### Adjusting auto initialization settings
By default, auto initialization is turned off. 
By turning it On, during app startup, it will load and cache all current UI culture localized key/values (or parent up to invariant culture if not available).
It will also refresh AvailableCultures property by checking all cultures with any localized key/values available.
You may want to adjust this behavior, here is how to do it:
```csharp
optionsBuilder.WithAutoInitialization(tryParents: true, refreshAvailableCultures: true, initializationCulture: null);
```
- tryParents: true to try with parent culture up to invariant when the asked one can't be found (default: true)
- refreshAvailableCultures: true to refresh AvailableCultures property during initialization (default: true)
- initializationCulture: force the CultureInfo you want to use by default during auto initialization

### Setting the default culture used as invariant
Even InvariantCulture resources (e.g. the main resx) are issued from a specific culture (but used for all missing).
You can tell the options builder wich culture your main text provider's invariant culture resources are issued from.
With that set, no more InvariantCulture item into AvailableCultures but YourDefaultCulture (e.g. useful for user understanding when binded to a Picker)
```csharp
optionsBuilder.WithDefaultInvariantCulture(defaultInvariantCulture: YourCultureInfo);
```

### Adding some more resx text providers
Sometimes, we need more than only one text provider.
In some scenarios, we could have a web and mobile shared resx in an assembly (e.g. common key/value for localized error message with key returned by server to mobile) plus a mobile specific resx (e.g. for views localization).

```csharp
something.WhateverRegistrationApproach<ResxTextProvider<YourFirstResourcesDesignerClass>>(optionsBuilder =>
                optionsBuilder.AddTextProvider<ResxTextProvider<YourSecondResourcesDesignerClass>>(invariantCulture: null));
```

Text provider addition order is quite important as the manager will look for the key in this order (first found, first returned).

Suppose you get this:

|ResxTextProvider< YourFirstResourcesDesignerClass >|ResxTextProvider< YourSecondResourcesDesignerClass >|
|--------|-------|
|Invariant (en)|Invariant (en)|
|French (fr)|French - France (fr-FR)|
|French - France (fr-FR)|Spanish (es)|

AvailableCultures will contain: English, French, French (France), Spanish

Then you ask for key "TestKey1" in fr-FR culture.
- If ResxTextProvider< YourFirstResourcesDesignerClass > contains the key it wins.
- If not, ResxTextProvider< YourScondResourcesDesignerClass > wins if it contains the key.
- If not, nobody wins and the key is returned as a value.


### Using custom text providers
You can add any custom text provider of your choice as long as it implements ITextProvider interface.
A scenario could be when you want to provide default resx built-in for app startup and then pull down some fresh key/values into a database (database service must implement ITextProvider interface)

/!\ This is instantiation approach sensitive /!\

#### Static approach

As Localizr doesn't know anything about your custom text provider class, you have to supply a factory:
```csharp

// This is an example where YourResourcesDesignerClass is the class name of the resx .Designer.cs auto generated file and
// where YourDbTextProviderClass is your local data access service inheriting form ITextProvider interface
Localizr.For<YourDbTextProviderClass>(providerOptions => new YourDbTextProviderClass(providerOptions), optionsBuilder =>
                optionsBuilder.AddTextProvider<ResxTextProvider<YourResourcesDesignerClass>>(invariantCulture: null));
```

#### Extensions approach

Localizr will register YourDbTextProviderClass as ITextProvider into your container, so nothing to take care about except all YourDbTextProviderClass constructor parameters must be resolvable and registered:
```csharp

// This is an example where YourResourcesDesignerClass is the class name of the resx .Designer.cs auto generated file and
// where YourDbTextProvider is your local data access service inheriting form ITextProvider interface
Localizr.For<YourDbTextProviderClass>(optionsBuilder =>
                optionsBuilder.AddTextProvider<ResxTextProvider<YourResourcesDesignerClass>>(invariantCulture: null));
```

### Mixing it all
You can mix it all together.

Suppose you get this:

|YourDbSyncTextProviderClass|ResxTextProvider< YourSharedResourcesDesignerClass >|ResxTextProvider< YourMobileResourcesDesignerClass >|
|--------|--------|-------|
|Invariant (en)|Invariant (en)|Invariant (en)|
|French (fr)|French (fr)|French - France (fr-FR)|
|French - France (fr-FR)|French - France (fr-FR)|Spanish (es)|
|Spanish (es)|||
|Italian (it)|||

#### Static approach

```csharp
var defaultCulture = CultureInfo.CreateSpecificCulture("en");
Localizr.For<YourDbTextProviderClass>(providerOptions => new YourDbTextProviderClass(providerOptions), optionsBuilder =>
            optionsBuilder.WithAutoInitialization(initializationCulture: defaultCulture)
				.WithDefaultInvariantCulture(defaultCulture)
				.AddTextProvider<ResxTextProvider<YourMobileResourcesDesignerClass>>()
				.AddTextProvider<ResxTextProvider<YourSharedResourcesDesignerClass>>());
```

#### Extensions approach
```csharp
var defaultCulture = CultureInfo.CreateSpecificCulture("en");
services.AddLocalizr<YourDbSyncTextProviderClass>(optionsBuilder =>
            optionsBuilder.WithAutoInitialization(initializationCulture: defaultCulture)
				.WithDefaultInvariantCulture(defaultCulture)
				.AddTextProvider<ResxTextProvider<YourMobileResourcesDesignerClass>>()
				.AddTextProvider<ResxTextProvider<YourSharedResourcesDesignerClass>>());
```

#### Here I'm saying:
- YourDbSyncTextProviderClass is my main text provider (kind of priority 1), but as it's a database text provider, it could be empty by default
- Auto initialize with "en" default culture
- Set "en" culture as invariant culture
- Add mobile specific resx text provider (default app localization when the db one has no matching key - e.g. empty at first launch)
- Add mobile/server shared resx text provider (localized error message handling)

#### Behavior: 
- When the app starts, it will be localized with "en" culture wich is the invariant one for each provider
- As it's the first launch, my database is empty so it will load and cache "en" localized resources from YourMobileResourcesDesignerClass first, then from YourSharedResourcesDesignerClass
- At this moment, AvailableCultures should contain English, French, French (France), Spanish.
- Then, somewhere in my loading script, my data service pull resources from remote server into local database.
- From there, I can manualy initialize again the plugin to refresh AvailableCultures wich should now conatin English, French, French (France), Italian, Spanish.

### Other configurations

#### Custom Localizr initialization handler

Your can provide your own ILocalizrInitializationHandler implementation in case you want some particular behaviors on initializing or initialized "event".

#### Custom Localizr manager

Your can provide your own ILocalizrManager implementation in case you want some particular behaviors on localization management.