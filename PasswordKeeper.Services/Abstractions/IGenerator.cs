namespace PasswordKeeper.Services.Abstractions
{
    public interface IGenerator<TResult, TParams>
    {
        TResult Generate(TParams parameter);
    }
}