// <copyright file="MauiProgram.cs" company="rpm informatik GmbH">
// Copyright (c) 2024 All Rights Reserved - rpm informatik GmbH
// </copyright>
// <author>Stefan Weber</author>
// <date>09.09.2024</date>
// <summary>Start Procedure for the Pathfinder - App</summary>
// 

using MauiAppPathfinder1.Configurations;
using MauiAppPathfinder1.DBContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using System.Reflection;


namespace MauiAppPathfinder1
{
    public static class MauiProgram
    {
        /// <summary>
        /// Handle all configuration before the start of the App. 
        /// Add Blazor and DevExpress to this Service - Class
        /// </summary>
        /// <returns>MauiApp - after build from CreateMauiApp() </returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            //builder.ConfigureLifecycleEvents(events => {  });
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddDevExpressBlazor();
#if ANDROID
            ///-----------------------------------------------------
            /// Code nur fuer Android geraete
            var config = ProConfiguration.InsertIntoConfig();
#else

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
           
#endif
            builder.Configuration.AddConfiguration(config);


            builder.Services.AddSingleton(new MauiAppPathfinder1DataContext(config));

            string  dbNAme = config.GetConnectionString("DefaultConnection")!;
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("");
            return builder.Build();
        }
    }
}
