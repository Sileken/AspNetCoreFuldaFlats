using System.Reflection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AspNetCoreFuldaFlats.AuthorizedJsonSerialization
{
    public class JsonAuthorizedContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JsonAuthorizedContractResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
        {
            var property = base.CreateProperty(member, serialization);

            var restrictedAttribute = member.GetCustomAttribute<JsonAuthorizedAttribute>();
            if (restrictedAttribute != null)
            {
                property.ShouldSerialize = x => restrictedAttribute.IsAuthorized(_httpContextAccessor.HttpContext.User);
            }

            return property;
        }
    }
}
