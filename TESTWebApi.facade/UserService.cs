using Light.Core.DependencyInjection;
using System;

namespace TESTWebApi.facade
{
    [AppService]
    public class UserService
    {
        public string name { get; set; }

        public string GetUserId()
        {
            
            return "demo";
        }
    }
}
