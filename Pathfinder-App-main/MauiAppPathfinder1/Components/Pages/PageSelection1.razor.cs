// <copyright file="PageSelection1.razor.cs" company="rpm informatik GmbH">
// Copyright (c) 2024 All Rights Reserved - rpm informatik GmbH
// </copyright>
// <author>Stefan Weber</author>
// <date>09.09.2024</date>
// <summary>Path finder (also sub directorys) for the Pathfinder - App</summary>
//
using DevExpress.Blazor;

using MauiAppPathfinder1.DBContext;
using MauiAppPathfinder1.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppPathfinder1.Components.Pages
{
    partial class PageSelection1
    {
        List<ProDocument>? SData2 { get; set; }

        [Inject]
        private IConfiguration configuration { get; set; }

        [Inject]
        private MauiAppPathfinder1DataContext mapp { get; set; }
        private IReadOnlyList<object> SelectedDataItems { get; set; }

        private object SelectedDataItem { get; set; }

        private IGrid? ProGrid2 { get; set; }

        public string? InputTextPathValue { get; set; }
        private object? InputTextPath { get; set; }

        /// <summary>
        /// on Keypress from  DxTextBox direkt execute onClick command
        /// </summary>
        /// <param name="e"></param>
        private void onKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                onClick();
            }

        }

        /// <summary>
        /// The user clicked 'Select Path' to receive all File and Sub Diretorys in a DxGrid Table
        /// </summary>
        void onClick()
        {
            List<ProDocument> lldoc = new List<ProDocument>(); int i = 0;
            try
            {
                if (InputTextPathValue != null)
                {
                    if (!Directory.Exists(InputTextPathValue)) return;
                    // Only get files that begin with the letter "c".
                    string[] dirs = Directory.GetFiles(InputTextPathValue, "*");
                    Console.WriteLine("The number of files starting with c is {0}.", dirs.Length);
                    foreach (string dir in dirs)
                    {
                        Console.WriteLine(dir); i++;
                        lldoc.Add(new ProDocument() { Dateiname = dir, Name = dir, Path = dir, Id = i, PathExists = false, FileExists = true });

                    }
                    dirs = Directory.GetDirectories(InputTextPathValue);
                    foreach (string dir in dirs)
                    {
                        Console.WriteLine(dir); i++;
                        lldoc.Add(new ProDocument() { 
                            Dateiname = dir + "\\", Name = dir + "\\", Path = dir + "\\", Id = i,
                           PathExists = true
                        });

                    }

                    SData2 = lldoc;
                    ProGrid2.Reload();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

        }

        /// <summary>
        /// Selected items in the DxGrid the Button : 'Ausgewählte Dateien übertragen in Datenbank'
        /// Items gets stored in the Connected Database and refreshed in the Home.razor DxGrid
        /// </summary>
        void onSubmit()
        { Home hm = new Home();

            if (SelectedDataItems != null) foreach (var product in SelectedDataItems.Cast<MauiAppPathfinder1.Models.ProDocument>())
                {
                    string fn = string.Empty; string ph = string.Empty;
                    if(hm.CheckInputFilePath(product.Path, ref   fn , ref ph))
                    {
                        product.Name = fn; product.Dateiname = fn; product.Path = ph;
                        hm.CallSearchProp(product);
                        mapp.AddItem(product);
                        SData2 = new List<ProDocument>(); ProGrid2!.Reload();

                    }

                }
        }

        /// <summary>
        /// User clicked an entry in a Column 'Open' - Directory refreshes with the neu Path
        /// </summary>
        /// <param name="id"></param>
        void DoSomething2(int id)
        {
            ProDocument proDocument = SData2?.FirstOrDefault(u => u.Id == id)!;
            if (proDocument != null)
            {
                InputTextPathValue = proDocument.Path;
                onClick(); ProGrid2!.Reload();

            }
        }
    }

}
