using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace AspNetCoreFuldaFlats.AuthorizedJsonSerialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonAuthorizedAttribute : Attribute
    {
        private readonly List<string> _roleNames;

        public JsonAuthorizedAttribute()
        {
        }

        public JsonAuthorizedAttribute(List<string> roleNames)
        {
            _roleNames = roleNames;
        }

        public bool IsAuthorized(IPrincipal user)
        {
            var isAuthorized = false;

            if (user.Identity.IsAuthenticated && (_roleNames != null) && _roleNames.Any())
            {
                isAuthorized = _roleNames.All(user.IsInRole);
            }
            else if (user.Identity.IsAuthenticated)
            {
                isAuthorized = true;
            }

            return isAuthorized;
        }
    }
}
