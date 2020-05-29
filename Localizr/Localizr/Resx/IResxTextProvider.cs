namespace Localizr.Resx
{
    public interface IResxTextProvider : ITextProvider
    {

    }

    public interface IResxTextProvider<T> : IResxTextProvider where T : class
    {
    }
}
