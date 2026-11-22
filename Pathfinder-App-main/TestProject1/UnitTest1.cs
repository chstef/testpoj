using MauiAppPathfinder1.Components.Pages;
using MauiAppPathfinder1.Models;
using Microsoft.Extensions.Configuration;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            var myDoc = new MyDocument();
            var Home = new Home();
            var filename = string.Empty; var path = string.Empty;
            
            Home.InputTextPathValue = "c:\\temp\\";
            Assert.True(Home.CheckInputFilePath("c:\\temp", ref filename, ref path));
            Assert.Equal("", filename);
            Assert.Equal("c:\\temp\\", path);
            filename = string.Empty; path = string.Empty;
            Assert.True(Home.CheckInputFilePath("c:\\temp\\MyAufDokumentposition.txt", ref filename, ref path));
            Assert.Equal("MyAufDokumentposition.txt", filename);
            Assert.Equal("c:\\temp\\", path);
            filename = string.Empty; path = string.Empty;
            Assert.True(Home.CheckInputFilePath("c:\\\\temp\\\\MyAufDokumentposition.txt", ref filename, ref path));
            Assert.Equal("MyAufDokumentposition.txt", filename);
            Assert.Equal("c:\\temp\\", path);
            filename = string.Empty; path = string.Empty;
            Assert.False(Home.CheckInputFilePath("c:\\temp\\MyAufDokumentposition", ref filename, ref path));
            Assert.Equal("", filename);
            Assert.Equal("c:\\temp\\MyAufDokumentposition\\", path);
            filename = string.Empty; path = string.Empty;
            Assert.True(Home.CheckInputFilePath("\\\\localhost\\packages", ref filename, ref path));
            Assert.Equal("", filename);
            Assert.Equal("\\\\localhost\\packages\\", path);
            filename = string.Empty; path = string.Empty;
            Assert.True(Home.CheckInputFilePath("\\\\localhost\\packages\\DevExpress.Blazor.de.24.1.3.nupkg", ref filename, ref path));
            Assert.Equal("DevExpress.Blazor.de.24.1.3.nupkg", filename);
            Assert.Equal("\\\\localhost\\packages\\", path);
            filename = string.Empty; path = string.Empty;
            Assert.False(Home.CheckInputFilePath("\\\\localhost\\packages\\DevExpress.Blazor.de.24.1.3", ref filename, ref path));
            Assert.Equal("", filename);
            Assert.Equal("", path);



        }
        [Fact]
        public async void Test2()
        {
            var config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .Build();

            string dbNAme = config.GetConnectionString("DefaultConnection")!;

            Assert.Equal("MyDatabase.db3", dbNAme);
        }
        [Fact]
        public async void Test3()
        {

            var myDoc = new MyDocument();
            var Home = new Home();
            myDoc.Name = "MyAufDokumentposition.txt"; myDoc.Dateiname = "MyAufDokumentposition.txt"; myDoc.Path = "c:\\temp\\";
            Home.CallSearchProp(myDoc);
            Assert.True(myDoc.FileExists); Assert.True(myDoc.PathExists);
            var dofilePath = string.Empty;
            Assert.True(Home.GetFileExists(myDoc, ref dofilePath));
            Assert.Equal("c:\\temp\\MyAufDokumentposition.txt", dofilePath);

            myDoc.Name = "DevExpress.Blazor.de.24.1.3.nupkg"; myDoc.Dateiname = "DevExpress.Blazor.de.24.1.3.nupkg"; myDoc.Path = "\\\\localhost\\packages\\";
            Home.CallSearchProp(myDoc);
            Assert.True(myDoc.FileExists); Assert.True(myDoc.PathExists);
            dofilePath = string.Empty;
            Assert.True(Home.GetFileExists(myDoc, ref dofilePath));
            Assert.Equal("\\\\localhost\\packages\\DevExpress.Blazor.de.24.1.3.nupkg", dofilePath);
           
            myDoc.Name = ""; myDoc.Dateiname = ""; myDoc.Path = "c:\\temp\\"; myDoc.FileExists = false; myDoc.PathExists = false;
            Home.CallSearchProp(myDoc);
            Assert.False(myDoc.FileExists); 
            Assert.True(myDoc.PathExists);
            dofilePath = string.Empty;
            Assert.True(Home.GetFileExists(myDoc, ref dofilePath));
            Assert.Equal("c:\\temp\\", dofilePath);

            myDoc.Name = ""; myDoc.Dateiname = ""; myDoc.Path = "\\\\localhost\\packages\\"; myDoc.FileExists = false; myDoc.PathExists = false;
            Home.CallSearchProp(myDoc);
            //Assert.False(myDoc.FileExists); Assert.True(myDoc.PathExists);
            dofilePath = string.Empty;
            Assert.True(Home.GetFileExists(myDoc, ref dofilePath));
            Assert.Equal("\\\\localhost\\packages\\", dofilePath);

        }
    }
}