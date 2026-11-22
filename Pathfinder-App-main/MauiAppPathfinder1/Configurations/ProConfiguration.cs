// <copyright file="ProConfiguration.cs" company="rpm informatik GmbH">
// Copyright (c) 2024 All Rights Reserved - rpm informatik GmbH
// </copyright>
// <author>Stefan Weber</author>
// <date>11.09.2024</date>
// <summary>All Configuration for Android for the Pathfinder - App</summary>
// 
using Microsoft.Extensions.Configuration;
using System.Text;

namespace MauiAppPathfinder1.Configurations
{
    public static class ProConfiguration
    {
        private static string DefaultConnection             = "MyDatabase.db3";
        private static string Version                       = "21.0";
        private static bool   ConnectionEncription          = true;
        private static bool   RowSelectionWithDoubleClick   = true;

        public static string GetConnectionString { get { return DefaultConnection; } set { } }
        public static string GetVersion { get { return Version; } set { } }
        public static bool GetConnectionEncription { get { return ConnectionEncription; } set { } }
        public static bool GetRowSelectionWithDoubleClick { get { return RowSelectionWithDoubleClick; } set { } }

        /// <summary>
        /// Load config from a Json - string from Memory Objekt
        /// </summary>
        /// <returns>All loaded Configurations</returns>
        public static IConfiguration InsertIntoConfig()
        {
            IConfigurationProvider lo;
          


            var json = @"{
                            ""ConnectionStrings"": {
                            ""DefaultConnection"": ""MyDatabase.db3"",
                            ""ConnectionPassword"": ""password""

                            },
                            ""version"": ""21.0"",
                            ""ConnectionEncription"": true,
                            ""RowSelectionWithDoubleClick"" : true
                        }";
            var config = new ConfigurationBuilder();
            var iconRoot = config.AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(json))).Build();

            var cost = iconRoot.GetConnectionString("DefaultConnection");
            if(iconRoot != null) return iconRoot;
            else return null;
        }

    }
}
