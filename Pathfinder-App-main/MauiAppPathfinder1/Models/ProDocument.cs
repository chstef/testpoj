// <copyright file="ProDocument.cs" company="rpm informatik GmbH">
// Copyright (c) 2024 All Rights Reserved - rpm informatik GmbH
// </copyright>
// <author>Stefan Weber</author>
// <date>09.09.2024</date>
// <summary>DB Model Base Table for the Pathfinder - App</summary>
// 

using SQLite;
 

namespace MauiAppPathfinder1.Models
{
    public class ProDocument
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path{ get; set; }
        public string Dateiname{ get; set; }

        [Ignore]
        public  bool PathExists { get; set; }

        [Ignore]
        public bool FileExists { get; set; }

    }
}
