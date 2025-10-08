using Microsoft.AspNetCore.Identity;

namespace PMS.Infrastructure.Identity
{
    public static class IdentityExtensions
    {
        public static List<string> GetIdentityResultErrorDescrptions(this IdentityResult identityResult)
        {
            return identityResult?.Errors?.Select(x => x.Description)
                ?.ToList();
        }
    }
}
