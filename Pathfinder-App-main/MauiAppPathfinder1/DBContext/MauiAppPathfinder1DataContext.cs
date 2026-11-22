// <copyright file="MauiAppPathfinder1DataContext.cs" company="rpm informatik GmbH">
// Copyright (c) 2024 All Rights Reserved - rpm informatik GmbH
// </copyright>
// <author>Stefan Weber</author>
// <date>09.09.2024</date>
// <summary>DB-Context and Data connection for the Pathfinder - App</summary>
// 
using MauiAppPathfinder1.Models;
using Microsoft.Extensions.Configuration;
using SQLite;
using System.Data.Entity;

namespace MauiAppPathfinder1.DBContext
{
    internal class MauiAppPathfinder1DataContext : DbContext
    {

        string DbNAme { get; set; } = string.Empty;

        /// <summary>
        /// Handles all DB Context with SQLite DB - Selection
        /// Gets config from appsettings.json - DB ist encripted and has a Password.
        /// </summary>
        /// <param name="configuration"></param>
        public MauiAppPathfinder1DataContext(IConfiguration configuration)
        {

            DbNAme = configuration.GetConnectionString("DefaultConnection")!;
            var password = configuration.GetValue<string>("ConnectionStrings:ConnectionPassword");
            DbPath = Path.Combine(FileSystem.AppDataDirectory, DbNAme);
            var options = new SQLiteConnectionString(DbPath, true, key: password);

            //   var options2 = new SQLiteConnectionString(DbPath, true, preKeyAction: db => db.Execute("PRAGMA cipher_default_use_hmac = OFF;"), postKeyAction: db => db.Execute("PRAGMA kdf_iter = 128000;"));
            bool isEncrption= configuration.GetValue<bool>("ConnectionEncription");
            if (isEncrption)
            {
                Database = (_connection ??= new SQLiteAsyncConnection(options));

            }
            else
            {
                Database =
                (_connection ??= new SQLiteAsyncConnection(DbPath,
                    SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache));
            }
        }
        public DbSet<ProDocument> ProDocuments { get; set; } 

        private static string DbPath = string.Empty;

        private SQLiteAsyncConnection _connection;
        private SQLiteAsyncConnection Database = null!;
        public List<ProDocument> Products { get; set; }

        /// <summary>
        /// Created the Table from the Entity. and if the Table has data return a List of ProDocument
        /// </summary>
        /// <returns>List of ProDocument - With a Task Await</returns>
        public async Task<IEnumerable<ProDocument>> CheckDB()
        {
            int i = Database.LibVersionNumber;
            string opstr = Path.Combine(FileSystem.AppDataDirectory, DbNAme);
            Database.CreateTableAsync<ProDocument>(CreateFlags.ImplicitIndex);

            if (false)
            {
                Database.InsertAsync(new ProDocument() { Id = 2, Name = "TestName", Dateiname = "Dateiname", Path = "c\\tenp" });
                Database.UpdateAsync(new ProDocument() { Id = 31, Path = "c:\\temp" });
                Database.DeleteAsync(new ProDocument() { Id = 3 });

                //   Database.FindAsync<MyDocument>(31);

                AsyncTableQuery<ProDocument> listdoc = Database.Table<ProDocument>();

                listdoc.CountAsync();

                ProDocument MyDoc3;
                var mydocList = Database.Table<ProDocument>().ToListAsync();
            }
            Products = new List<ProDocument> { };

            return await GetAllAsync<ProDocument>();

        }

        /// <summary>
        /// Add Item a ProDocument into the Database Table
        /// </summary>
        /// <param name="myDoc"></param>
        public void AddItem(ProDocument myDoc)
        {
            Database.InsertAsync(myDoc);
        }

        /// <summary>
        /// Return all Tableentrys as a Refernce to ProDocument as a Taks async
        /// </summary>
        /// <typeparam name="TTable"></typeparam>
        /// <returns>Return all Tableentrys</returns>
        private async Task<AsyncTableQuery<TTable>> GetTableAsync<TTable>() where TTable : class, new()
        {
            //await CreateTableIfNotExists<TTable>();
            return Database.Table<TTable>();
        }

        /// <summary>
        /// Gets all Table Entry in a Task await to a List of ProDocument
        /// </summary>
        /// <typeparam name="TTable"></typeparam>
        /// <returns>returns a List of ProDocument objects </returns>
        public async Task<IEnumerable<TTable>> GetAllAsync<TTable>() where TTable : class, new()
        {
            var table = await GetTableAsync<TTable>();
            return await table.ToListAsync();
        }

        /// <summary>
        /// Delete entry from the Database SQLite with await Procedure (Id ist the key of the Table)
        /// </summary>
        /// <param name="id"></param>
        public async void DeleteAsync(int id)
        {

            await Database.DeleteAsync(new ProDocument() { Id = id });

        }


    /// <summary>
    /// Update one Table Entry in the Database (Prodocument) in await Procedure
    /// </summary>
    /// <param name="md"></param>
        public async void UpdateAsync(ProDocument md)
        {if(md.Id>0)
            await Database.UpdateAsync(md);
        }


    }
}
