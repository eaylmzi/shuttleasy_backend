using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.unittest
{
    public class WebAppFactory : WebApplicationFactory<Program>
    {
        public string DefaultUserId { get; set; } = "1";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            
        }
    }
}
