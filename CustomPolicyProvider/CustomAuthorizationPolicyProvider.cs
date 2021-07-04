using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthBasics.CustomPolicyProvider
{
    public class Dummy
    {
        public Dummy()
        {

        }
    }


    public static class DynamicPolicies
    {
        public static IEnumerable<string> Get()
        {
            yield return SecurityLevel;
            yield return Rank;
        }

        public const string SecurityLevel = "SecurityLevel";
        public const string Rank = "Rank";
    }

    public static class DynamicAuthorizationPloicyFactory
    {
        public static AuthorizationPolicy Create(string policyName)
        {
            var parts = policyName.Split('.');
            var type = parts.First();
            var value = parts.Last();

            switch(type)
            {
                case DynamicPolicies.Rank:
                    return new AuthorizationPolicyBuilder()
                         .RequireClaim("Rank", value)
                         .Build();
                case DynamicPolicies.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                         .Build();
                default:
                    return null;
            }
        }
    }

    public class CustomAuthorizationPolicyProvider
        : DefaultAuthorizationPolicyProvider
    {
        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {

        }

        //{type}. {value}
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach(var customPolicy in DynamicPolicies.Get())
            {
                if(policyName.StartsWith(customPolicy))
                {
                    var policy = new AuthorizationPolicyBuilder().Build();

                    return Task.FromResult(policy);
                }
            }

            return base.GetPolicyAsync(policyName);
        }
    }
}
