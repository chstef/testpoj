// <copyright file="MainLayout.razor.cs" company="rpm informatik GmbH">
// Copyright (c) 2024 All Rights Reserved - rpm informatik GmbH
// </copyright>
// <author>Stefan Weber</author>
// <date>09.09.2024</date>
// <summary>Read Version from the Options</summary>
//
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppPathfinder1.Components.Layout
{
    partial class MainLayout
    {

        private string version { get; set; }

        [Inject]
        private IConfiguration configuration { get; set; }
        
        /// <summary>
        /// Overrides OnInitialized -> reads Version from the Options appsetting.json
        /// </summary>
        protected override void OnInitialized()
        {
            version  =  configuration.GetValue<string>("version");
            base.OnInitialized();
        }

    }
}
