﻿using System;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using System.Collections.Generic;
using Microsoft.Owin.Diagnostics;

namespace Mso.SignalR
{
    public class WebStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Properties["host.AppName"] = "Mso";
            app.MapSignalR(new HubConfiguration()
            {
                EnableJavaScriptProxies = true,
                EnableDetailedErrors = true
            });
            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem("../../../NodeServer/www")
            });
            app.UseWelcomePage();
        }
    }
}

