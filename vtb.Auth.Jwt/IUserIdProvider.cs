using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace vtb.Auth.Jwt
{
    public interface IUserIdProvider
    {
        Guid UserId { get; set; }
    }
}
