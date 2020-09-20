using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace vtb.Auth.Permissions
{
    public static class PermissionPoliciesHelper
    {
        private static readonly Action<AuthorizationPolicyBuilder> SystemPolicy = policy =>
            policy.RequireClaim("system", true.ToString());

        public static Dictionary<string, Action<AuthorizationPolicyBuilder>> Policies
        {
            get
            {
                var policies = typeof(Permissions).GetFields()
                    .Select(x => new
                    {
                        Name = $"Require_{x.GetValue(null)}_Permission",
                        Func = GetPolicyBuilder(x.GetValue(null) as string)
                    })
                    .ToDictionary(x => x.Name, x => x.Func);

                policies.Add("system_user", SystemPolicy);

                return policies;
            }
        }

        private static Action<AuthorizationPolicyBuilder> GetPolicyBuilder(string permissionName)
        {
            return policy => policy
                .RequireClaim("permission", permissionName);
        }
    }
}