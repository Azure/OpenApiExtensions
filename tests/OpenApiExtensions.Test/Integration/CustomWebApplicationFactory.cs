using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace OpenApiExtensions.Test.Integration
{
    public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder().UseEnvironment("Development");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //var sourceWebApiAssebly = typeof(TEntryPoint).Assembly;
            //var sfProjParent = new DirectoryInfo($"{Environment.CurrentDirectory}../../../../../../../src"); // the test projects is under "Asi-common/test" folder
            //var applicationFolderName = "ServiceHostingTools.TestApp";
            //var webProjectFolder = sourceWebApiAssebly.GetName().Name;

            //container.RegisterLocalWebApiService(
            //    applicationFolderName,
            //    webProjectFolder,
            //    sourceWebApiAssebly,
            //    (c) =>
            //    {
            //        c.Options.AllowOverridingRegistrations = true;
            //        defaultConfigurator.RegisterAppServices(c);
            //        if (TestServicesRegistrator != null)
            //        {
            //            TestServicesRegistrator(c); // we override the default regs with the test ones
            //        }
            //    },
            //    defaultConfigurator.GetWebApiOptions(),
            //    serviceFabricAppProjParentDirectory: sfProjParent,
            //    configOverridedFileNames: new[] { Path.Combine(sfProjParent.FullName, applicationFolderName, "ApplicationParameters/Local.1Node.xml"), "TestParameters.Overrides.xml" }
            //);
            //var localWebService = container.GetInstance<ILocalWebService>();

            //localWebService.RegisterLocalDependencies(builder);
        }
    }
}
