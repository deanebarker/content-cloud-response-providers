using DeaneBarker.Optimizely.ResponseProviders.Models;

namespace DeaneBarker.Optimizely.ResponseProviders.UserManagers
{
    public interface IResponseProviderUserManager
    {
        bool ShouldUseCache(BaseResponseProvider root);
    }
}
