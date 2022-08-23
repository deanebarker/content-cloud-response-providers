using DeaneBarker.Optimizely.ResponseProviders.Models;

namespace DeaneBarker.Optimizely.ResponseProviders.UserManagers
{
    public class SimpleUserManager : IResponseProviderUserManager
    {
        public bool ShouldUseCache(BaseResponseProvider root)
        {
            return !root.ACL.QueryDistinctAccess(EPiServer.Security.AccessLevel.Edit);
        }
    }
}
