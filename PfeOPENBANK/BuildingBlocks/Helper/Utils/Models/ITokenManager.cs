namespace Helper.Utils.Interfaces
{
    public interface ITokenManager
    {
        public Task<bool> IsCurrentActiveToken();
        public Task DeactivateCurrentAsync();
        public Task<bool> IsActiveAsync(string token);
        public Task DeactivateAsync(string token);
    }
}
