// <copyright file="Home.razor.cs" company="rpm informatik GmbH">
// Copyright (c) 2024 All Rights Reserved - rpm informatik GmbH
// </copyright>
// <author>Stefan Weber</author>
// <date>09.09.2024</date>
// <summary>All Functions for Home.razor - DB Access und Display in a DxGrid (DevExpress)</summary>
//
using DevExpress.Blazor;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using MauiAppPathfinder1.DBContext;
using MauiAppPathfinder1.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace MauiAppPathfinder1.Components.Pages
{
    partial class Home
    {
        List<ProDocument>? SData { get; set; }

        [Inject]
        private IConfiguration? configuration { get; set; }

        [Inject]
        private MauiAppPathfinder1DataContext? mapp {  get; set; }

         
        IGrid? ProGrid { get; set; }
        int likes = 0;
        public string? TextValue { get; set; }
        private bool itemIsSelected { get; set; }
        private bool itemIsSelected2 { get; set; }
        private bool itemIsSelected3 { get; set; }
        private bool itemIsSelected4 { get; set; }
         
        private bool Testtrue { get; set; }
        private bool Testtrue2 { get; set; }
        bool rowSelection { get; set; }
        bool hideSelectedColumns { get; set; }
        bool hideSelectedColumns2 { get; set; }

        /// <summary>
        /// Begin of hide some columns if its responsiv Design.
        /// </summary>
        void SwitchGridView()
        {
             if(hideSelectedColumns2) hideSelectedColumns2 = false;
             else hideSelectedColumns2 = true;

            IEnumerable<IGridColumn>  gridtest =   ProGrid.GetColumns();
            ProGrid.BeginUpdate();
            foreach (var column in gridtest)
            {if(column.Caption!=null && (
                    column.Caption.Equals( "Name") ||
                    column.Caption.Equals("Nr.") ||
                    column.Caption.Equals("Path Exists") ||
                    column.Caption.Equals("File Exists")

                    ))
                column.Visible = hideSelectedColumns2;
            }
            ProGrid.EndUpdate();
            ProGrid.Reload();

        }
        private bool stateAllDoc = false;
        /// <summary>
        /// OnInitialized - User gets all Entrys from the Database in the DxGrid - List.
        /// </summary>
        protected override void OnInitialized()
        {
            //GetAllDocs();
            ThreadPool.QueueUserWorkItem(GetAllDocs);
            //Thread.Sleep(1000);

            while (!stateAllDoc)
            {
                System.Threading.Thread.Sleep(100);
            }
            likes = 0;
            rowSelection = configuration.GetValue<bool>("RowSelectionWithDoubleClick");
            hideSelectedColumns = false; hideSelectedColumns2 = true;
            base.OnInitialized();
            // ProGrid.Reload();

        }
        private bool withoutProListBox = false;

        /// <summary>
        /// in Async Task - Get all entrys of the Table in the Database select these in the DxGrid - List
        /// </summary>
        private async void GetAllDocs(Object stateInfo )
        {
            SData = new List<ProDocument> {  };
            var ProListBoxData = new List<ProDocument> { };
            var prodocList3 = await   mapp.CheckDB();

            foreach (var product in prodocList3)
            {
                CallSearchProp(product);
                SData.Add(product);
                if (product.Dateiname.Length > 0 )
                {
                    ProDocument newDoc = new ProDocument();

                    int ind = product.Dateiname.LastIndexOf(".");
                    var gExtName = product.Dateiname.Substring(ind  , product.Dateiname.Length-ind);
                    if (ProListBoxData.FirstOrDefault(z => gExtName.Equals(z.Dateiname)) == null)
                    {
                        newDoc.Dateiname = gExtName;
                        newDoc.Id = product.Id;
                        ProListBoxData.Add(newDoc);
                    }
                }
            }
            SelectedDataItem = prodocList3.FirstOrDefault()!;

            if (!withoutProListBox) { ListBoxData = ProListBoxData; ExtValue = ""; countValues = 0; }

            //ExtValue = ListBoxData.Take(2);
            stateAllDoc = true; if(!withoutProListBox) firstTime = true;
            //ProGrid?.Reload();
        }

        /// <summary>
        /// Procedure looks for existing Files uns Directorys
        /// </summary>
        /// <param name="product"></param>
        public void CallSearchProp(ProDocument product) {
            if (product.Dateiname != null && product.Dateiname.Length > 0)
                product.FileExists = new FileInfo(product.Path + product.Dateiname).Exists;
            if (product.Path != null && product.Path.Length > 0)
                product.PathExists = Directory.Exists(product.Path);
        
        }

        /// <summary>
        /// Return if the File exists or Path exists a bool value - after User Entry a File or Path
        /// </summary>
        /// <param name="proDoc"></param>
        /// <param name="direc"></param>
        /// <returns>Bool value if File or Path exists</returns>
        public bool GetFileExists(ProDocument proDoc, ref string direc)
        {
            bool isonylFilename = false; bool exists = false;
            direc = "";// "\\\\localhost\\packages";
            if (proDoc.Path != null && proDoc.Path.Length > 0) { direc = proDoc.Path; }
            if (proDoc.Dateiname != null && proDoc.Dateiname.Length > 0) { direc += proDoc.Dateiname; isonylFilename = true; }
            if (direc == "") return false;
            bool bol3 = Directory.Exists(proDoc.Path);
            if (!bol3) return false;
            if (isonylFilename)
            {
                FileInfo proFile = new FileInfo(direc);
                //@"\\localhost\packages\DevExpress.Blazor.24.1.3.nupkg");
                exists = proFile.Exists;
                if (!exists) return false;
            }
            return true;
        }

        /// <summary>
        /// User selection of File or Path - Open in Exporer (Path) - Open File with the connected Application
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Void Taks</returns>
        private async Task DoSomething(int id )
        {   string direc = "";
            // you have access to every field from that row here
            //    System.Diagnostics.Process.Start("explorer.exe", "/open, c:\\test");
            ProDocument doDoc = SData?.Find(h => h.Id == id)!;
            /*bool isonylFilename = false; bool exists = false;
            string direc = "";// "\\\\localhost\\packages";
            if(doDoc.Path!= null && doDoc.Path.Length>0) { direc = doDoc.Path; }
            if(doDoc.Dateiname!=null && doDoc.Dateiname.Length>0) { direc += doDoc.Dateiname; isonylFilename = true; }
            if (direc == "") return;
            bool bol3 = Directory.Exists(doDoc.Path);
            if (!bol3) return;
            if (isonylFilename)
            {
                FileInfo myFile = new FileInfo(direc);
                //@"\\localhost\packages\DevExpress.Blazor.24.1.3.nupkg");
                exists =  myFile.Exists;
                if(!exists ) return;
            }

            Console.WriteLine(bol3? "true": "false");
            */
          
            if (GetFileExists(doDoc, ref direc))
            {

                Process openTerminal = new();

                openTerminal.StartInfo.FileName = "explorer.exe"; // The 'osascript' command on macOS

                // AppleScript to open Terminal and execute commands
                openTerminal.StartInfo.Arguments = direc;
                openTerminal.StartInfo.UseShellExecute = false;
                openTerminal?.Start();
            }
           /* string curFile = "c:\\temp\\MyAufDokumentposition.txt";
           // bool exists = File.Exists(curFile);
            curFile = "c:\\temp\\MyAufDokumentposition2.txt";
            exists = File.Exists(curFile); 
            
            curFile = "\\\\localhost\\packages\\DevExpress.Xpo.ja.24.1.3.nupkg";
            exists = File.Exists(curFile);
           */
            itemIsSelected = false;

        }

        /// <summary>
        /// Before edit in Grid - Readonly collumns select
        /// </summary>
        /// <param name="e"></param>
        private void OnEditStart(GridEditStartEventArgs e)
        {

            var settingsArtikelNr = e.Grid.GetColumnEditSettings<IEditSettings>("Id2");
            var settingsFileExists = e.Grid.GetColumnEditSettings<IEditSettings>("FileExists");
            var settingsPathExists = e.Grid.GetColumnEditSettings<IEditSettings>("PathExists");
            e.Grid.BeginUpdate();
            settingsArtikelNr.ReadOnly = true;

            settingsFileExists.ReadOnly = true;

            settingsPathExists.ReadOnly = true;
            e.Grid.EndUpdate();

             
        }


        /// <summary>
        /// Removes all Entry there are double Entrys - Procedure ist refeshes the DxGrid - List.
        /// </summary>
        private void DoubbleOut()
        {
            List<ProDocument> ProDocuments = SData!;
            bool fromAnfang = true;
            while (fromAnfang) { fromAnfang = false;
                foreach (var proDoc in ProDocuments!)
                {

                    foreach (var item in SData!)
                    {
                        if (item.Id != proDoc.Id)
                        {
                            if (item.Name == proDoc.Name && item.Dateiname == proDoc.Dateiname && item.Path == proDoc.Path)
                            {
                                //delete item
                                mapp.DeleteAsync(item.Id);
                                SData.Remove(item);
                                fromAnfang = true;
                                break;
                            }
                        }
                       
                    }
                    if (fromAnfang) break;
                } }
            ProGrid!.Reload();
        }

        /// <summary>
        /// Listed all not existing Paths - For Files and Path entrys
        /// </summary>
        private void CheckExistsPathandFiles()
        {   List<string> pathAndFiles = new List<string>(); 
            bool fromAnfang = true; SelecttoDelete = "";
            while (fromAnfang)
            {
                fromAnfang = false;
                foreach (var proDoc in SData!)
                {
                    string driString=string.Empty;
                    if (!GetFileExists(proDoc, ref driString))
                    {
                        if (false)
                        {
                            mapp.DeleteAsync(proDoc.Id);
                            SData!.Remove(  proDoc);
                            fromAnfang = true;break;
                        }
                        string gll = pathAndFiles.FirstOrDefault<string>(f => f.Equals(proDoc.Path.ToLower()   ));
                        if (gll == null){
                            SelecttoDelete += proDoc.Path  + "   ";
                            pathAndFiles.Add(proDoc.Path.ToLower()  );        
                        }
                           
                    }
                }
            }
            if (SelecttoDelete == null) SelecttoDelete = "Alle Files und Path's gefunden";
            if (SelecttoDelete == "") SelecttoDelete = "Alle Files und Path's gefunden";
            ProGrid!.Reload();
        }

        private string SelecttoDelete {  get; set; }
        
        private object?  InputTextPath {  get; set; } 

        private object? SelectedDataItem { get; set; }
        public string? InputTextPathValue { get; set; }
        private string GetSelectedItem()
        {
            return TextValue;

        }
        private void onCloseWindows()
        {

            Application.Current!.Quit();
        }

        /// <summary>
        /// Checkes all User Input Path and File to be exsting
        /// </summary>
        /// <param name="inputTextPath"></param>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <returns>Return if exists the File or Path</returns>
        public bool CheckInputFilePath(string inputTextPath, ref string filename, ref string path)
        {
            if (inputTextPath?.Length > 0)
            {
                var ext = Path.GetExtension(inputTextPath);

                FileInfo proFile = new FileInfo(inputTextPath);
                if (proFile.Exists) { var text = proFile.Length; if (text > 0 && ext == "") ext = "."; }
                //@"\\localhost\packages\DevExpress.Blazor.24.1.3.nupkg");
                bool exists = proFile.Exists;
                if (proFile.Extension == ext && ext.Length > 0 && !exists) return false;
         

                 filename = Path.GetFileName(inputTextPath);
                Console.WriteLine("Filename = " + filename);

                path = System.IO.Path.GetDirectoryName(inputTextPath)!;

                if (ext.Length == 0)  // wenn kein Dateiname nur Path
                {
                    path = inputTextPath;

                }
                if (!string.IsNullOrWhiteSpace(path) && path.Substring(path.Length - 1, 1) != "\\")
                {
                    path += "\\";
                }
                if (ext.Length == 0)  // wenn kein Dateiname nur Path
                {
                    filename = "";


                    if (!Directory.Exists(path)) return false;
                }
                Regex driveCheck = new Regex(@"^[a-zA-Z]:\\$");
                if (string.IsNullOrWhiteSpace(path) || path.Length < 3)
                {
                    return false;
                }
                string strTheseAreInvalidFileNameChars = new string(Path.GetInvalidPathChars());
                strTheseAreInvalidFileNameChars += @":/?*" + "\"";
                Regex containsABadCharacter = new Regex("[" + Regex.Escape(strTheseAreInvalidFileNameChars) + "]");
                if (containsABadCharacter.IsMatch(path.Substring(3, path.Length - 3)))
                {
                    return false;
                }
                return true;
            }

            return false;
         }

        /// <summary>
        /// Check Entry of Path or File and Store ist Data in the Database 
        /// and refreshes the List in DxGrid
        /// </summary>
        private void OnClickSelectPath() {
            OnAddFileandPath(InputTextPathValue!);
        }
        private void OnAddFileandPath(string addedFilePath)
        {
            var filename = string.Empty; var path = string.Empty;
            if (CheckInputFilePath(addedFilePath,ref filename, ref path))
            {
                /* var ext = Path.GetExtension(InputTextPathValue);

                FileInfo myFile = new FileInfo(InputTextPathValue);
                if (myFile.Exists) { var text = myFile.Length; if (text > 0 && ext == "") ext = "."; }
                    //@"\\localhost\packages\DevExpress.Blazor.24.1.3.nupkg");
                bool exists = myFile.Exists;
                if(myFile.Extension == ext && ext.Length>0 && !exists) return;
          

                var filename = Path.GetFileName(InputTextPathValue);
                Console.WriteLine("Filename = " + filename);

                var path = System.IO.Path.GetDirectoryName(InputTextPathValue);
                             
                if(  ext.Length==0)  // wenn kein Dateiname nur Path
                {
                    path = InputTextPathValue;
                    
                }
                if(!string.IsNullOrWhiteSpace(path) && path.Substring(path.Length-1,1) != "\\")
                    {
                        path += "\\";
                    }
                if (ext.Length == 0)  // wenn kein Dateiname nur Path
                {
                    filename = "";


                    if (!Directory.Exists(path)) return;
                }
                Regex driveCheck = new Regex(@"^[a-zA-Z]:\\$");
                if (string.IsNullOrWhiteSpace(path) || path.Length < 3)
                {
                    return  ;
                }
                string strTheseAreInvalidFileNameChars = new string(Path.GetInvalidPathChars());
                strTheseAreInvalidFileNameChars += @":/?*" + "\"";
                Regex containsABadCharacter = new Regex("[" + Regex.Escape(strTheseAreInvalidFileNameChars) + "]");
                if (containsABadCharacter.IsMatch(path.Substring(3, path.Length - 3)))
                {
                    return;
                }
                */

                DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetFullPath(path));
                try
                {
                    if (!directoryInfo.Exists)
                    {
                        return;
                    }
                    else
                    {
                    }

                    ProDocument v = new ProDocument()
                    {
                        Name = filename,
                        Dateiname =  filename,
                        Path = path,
                    };
                    mapp.AddItem(v);
                    CallSearchProp(v);
                    SData.Add(v);
                    ProGrid.Reload();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            
        }

        /// <summary>
        /// Reserve Proocedure - for Network Access... not used right now
        /// </summary>
        /// <param name="path"></param>
        /// <returns>List of folders</returns>
        private string getNetworkdrive(String path)
        {

            NetworkCredential theNetworkCredential = new NetworkCredential(@"server\user", "password");
            CredentialCache theNetcache = new CredentialCache();
            theNetcache.Add(new Uri(path), "Basic", theNetworkCredential);
            try
            {
                string[] theFolders = System.IO.Directory.GetDirectories(path);
            }
            catch (Exception ex)
            {
            } 
            return "";
        }

        /// <summary>
        /// not used in Form - Razor
        /// </summary>
        /// <param name="args"></param>
        private void Handler(MouseEventArgs args)
        {
            likes++;
            Console.WriteLine("I am clicked!");
        }

        /// <summary>
        /// Edit a row in a List DxGrid - and Updated in Database. (Dateiname, Name, Path)
        /// </summary>
        /// <param name="e"></param>
        private void OnEditModelSaving(GridEditModelSavingEventArgs e)
        {
        
            //var settingsCol1 = e.Grid.GetColumnEditSettings<IEditSettings>("Bezeichnung1");

            var editModel = (ProDocument)e.EditModel;
            if (editModel.Id == 0) // new Item
            {
                ProDocument newDoc = new ProDocument();
                newDoc.Dateiname = editModel.Dateiname;
                newDoc.Name = editModel.Name;

                string cutted = editModel.Path.Substring(editModel.Path.Length- 1, 1);
                if(cutted!="\\") newDoc.Path = editModel.Path  + "\\";
                else  newDoc.Path = editModel.Path;


                OnAddFileandPath(newDoc.Path + newDoc.Dateiname);

                return;
            }

            ProDocument doDoc = SData.Find(h => h.Id == editModel.Id);
            doDoc.Dateiname = editModel.Dateiname;
            doDoc.Name = editModel.Name;
            doDoc.Path = editModel.Path;
            CallSearchProp(doDoc);
            mapp.UpdateAsync(editModel);
            ProGrid.Reload();
        }

        /// <summary>
        /// Deleted a selected entry in the List - DxGrid from the User.
        /// Deleted in the Database and the DxGrid - Refreshes the DxGrid
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Void Task</returns>
        async Task OnDataItemDeleting(GridDataItemDeletingEventArgs e)
        {
            var editModel = (ProDocument)e.DataItem;
            SData!.Remove(editModel);
            mapp.DeleteAsync(editModel.Id);
            ProGrid!.Reload();
        }

        object InputCheckboxSel { get; set; }
        object InputCheckboxSel2 { get; set; }

        InputCheckbox InputCheckboxSel3 { get; set; }
        InputCheckbox InputCheckboxSel4 { get; set; }

        InputCheckbox InputCheckboxSel41 { get; set; }
        InputCheckbox InputCheckboxSel31 { get; set; }


        /// <summary>
        /// CheckBox to remove temp. all not existing Files and Path's
        /// </summary>
        private void CheckboxChanged()
        {
            Console.WriteLine(itemIsSelected ? "true" : "false");

            if (itemIsSelected || itemIsSelected4)
            {
                SData!.RemoveAll(r => !Directory.Exists(r.Path));
                SData!.RemoveAll(r => !(new FileInfo(r.Path + r.Dateiname).Exists));
            }

            else
            {
                itemIsSelected2 = false;
                //Type ol = InputCheckboxSel.GetType();
                itemIsSelected3 = false;  
                //GetAllDocs();
                ThreadPool.QueueUserWorkItem(GetAllDocs);
                Thread.Sleep(1000);
            }

        }

        /// <summary>
        /// Removes the DxGrid all Entrys with not existing Path's
        /// </summary>
        private void CheckboxChanged2()
        {
            Console.WriteLine(itemIsSelected2 ? "true" : "false");
            if (itemIsSelected2 || itemIsSelected3)
            {
                SData!.RemoveAll(r => !Directory.Exists(r.Path));
              
            }

            else { itemIsSelected = false;
                //GetAllDocs();
                ThreadPool.QueueUserWorkItem(GetAllDocs);
                Thread.Sleep(1000);
                itemIsSelected4  = false;
            }

        }

        /// <summary>
        /// Convertiert int to String value all Id - Columns - not Spinning Editbox
        /// </summary>
        /// <param name="e"></param>
        void Grid_CustomUnboundColumnData(GridUnboundColumnDataEventArgs e)
        {
            if (e.FieldName == "Id2")
            {
                int temperature = Convert.ToInt32(e.GetRowValue("Id"));
                e.Value =  (temperature).ToString();
            }
        }

        /// <summary>
        ///   RowDoubleClick="OnRowDoubleClick"  --- (int)context.Value
        ///   User Double - clicked on the Row shows current entry on File or Path's
        /// </summary>
        /// <param name="e"></param>
        async void OnRowDoubleClick(GridRowClickEventArgs e)
        {
            var selectedcolumn = e.Column.Caption;
            //Alert = $"The row {e.VisibleIndex} has been double clicked. The row value is '{
            int stCol =  (int)   e.Grid.GetRowValue(e.VisibleIndex, "Id");
            if (rowSelection && stCol > 0 && selectedcolumn!=null)
            {
               await DoSomething(stCol);
            }
        }


        /// <summary>
        /// for later use - Update file not working in Maui - Application
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Void Task</returns>
        async Task OnFilesUploading(FilesUploadingEventArgs args)
        {
            foreach (var file in args.Files)
            {
                ProDocument v = new ProDocument() { Name = file.Name, Dateiname = file.Name };
                mapp.AddItem(v);
                SData.Add(v);
                ProGrid.Reload();
                /* The following code is intended for demonstration purposes only.
                Do not read a stream directly in memory to avoid performance and security-related issues. */
                using var stream = new System.IO.MemoryStream();
                await file.OpenReadStream(file.Size).CopyToAsync(stream);
            }
        }

        IDropDownBox RefDD { get; set; }
        IEnumerable<ProDocument> ListBoxData { get; set; }
        object ExtValue { get; set; }

        /// <summary>
        /// Shows all selected Items in the DropdownBox.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Name to Display</returns>
        string QueryText(DropDownBoxQueryDisplayTextContext arg)
        {
            var names = (arg.Value as IEnumerable<ProDocument>)?.Select(x => x.Dateiname);
            if (names == null) return string.Empty;
            if(names.Count()==0) return string.Empty; 
            return names != null ? string.Join(",", names) : string.Empty;
        }

        /// <summary>
        /// Open and Close the dropDownBox
        /// </summary>
        /// <param name="dropDownBox"></param>
        /// <returns>all return Models Prodocuments in a List</returns>
        IEnumerable<ProDocument> GetListBoxValues(IDropDownBox dropDownBox)
        {
            if (!dropDownBox.DropDownVisible)
            {
                // IEnumerable<ProDocument> listext = (IEnumerable<ProDocument>)dropDownBox.Value;
                
                if (dropDownBox != null &&
                     (dropDownBox.Value as IEnumerable<ProDocument>) != null &&
                    (dropDownBox.Value as IEnumerable<ProDocument>)?.Count() == 0)
                {
                    //dropDownBox.BeginUpdate();
                    //GetAllDocs();
                    //dropDownBox.EndUpdate();
                }
            }

            return dropDownBox.Value as IEnumerable<ProDocument>;
        }

        int countValues = 0;
        bool firstTime = true;

        /// <summary>
        /// Select all selected items in die Grid - List load all items from the Database again.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="dropDownBox"></param>
        void  ListBoxValuesChanged(IEnumerable<ProDocument> values, IDropDownBox dropDownBox)
        {
            if (dropDownBox != null)
            {
                DropDownDirection ddd = dropDownBox.DropDownDirection;
                int ilist = dropDownBox.DropDownBodyTemplate.Method.GetParameters().Length;
                // dropDownBox.
                if (dropDownBox.DropDownVisible)
                {
                   // IEnumerable<ProDocument> listext = (IEnumerable<ProDocument>)dropDownBox.Value;
                    bool firstin = true;
                    if (values != null /*&& values.Count() == 0*/)
                    {

                        if( countValues != values.Count())
                        {
                            countValues = values.Count();
                            if (!firstTime /* && countValues == 0*/)
                            {//GetAllDocs(this);
                                if (countValues > 0) withoutProListBox = true;
                                System.Threading.Thread.Sleep(100);
                                ThreadPool.QueueUserWorkItem(GetAllDocs, countValues > 0);
                                while (SData!.Count == 0)
                                {
                                    System.Threading.Thread.Sleep(1000);
                                }
                                //Thread.Sleep(1000);
                              /* if (values?.Count() == 0)*/// ProGrid!.Reload();
                             
                            }
                            firstTime = false;
                        }
                    }
                    System.Threading.Thread.Sleep(100);

                    if (SData!.Count > 0 && values != null && values?.Count() > 0) 
                        while (firstin)
                        {
                            
                            foreach (var item in SData)
                            {
                                bool haltesx = false;
                                foreach (var text in values)
                                {
                                    if (item.Dateiname.Contains(text.Dateiname))
                                    {
                                        haltesx = true;
                                    }
                                }
                                if (!haltesx)
                                {
                                    SData.Remove(item); firstin = true;
                                    break;
                                }

                                firstin = false;

                            }
                            if(SData.Count== 0) firstin= false;
                        }

                    if (values != null) ProGrid!.Reload();
                }
            }
            dropDownBox!.BeginUpdate();
            dropDownBox.Value = values; dropDownBox.HideDropDown();
            dropDownBox.EndUpdate(); 
            withoutProListBox = false;
        }
        /*int GetSelectedItemsCount()
        {
            if (ExtValue is IEnumerable<ProDocument> value)
                return value.Count();
            return 0;
        }*/
        bool ExportSelectedRowsOnly { get; set; }
        string SavedExportFile { get; set; }    

        /// <summary>
        /// Export all data to a CSV - File in the Download directory
        /// </summary>
        /// <returns></returns>
        async Task ExportCsv_Click()
        {
            await ProGrid!.ExportToCsvAsync("ExportResult_CSV", new GridCsvExportOptions()
            {
                ExportSelectedRowsOnly = ExportSelectedRowsOnly
            });

            SavedExportFile = "(ExportResult_CSV.csv)";
        }


        bool UploadVisible { get; set; } = false;

        /// <summary>
        /// for later use - Update file not working in Maui - Application
        /// </summary>
        /// <param name="files"></param>
        protected void SelectedFilesChanged(IEnumerable<UploadFileInfo> files)
        {
            string hj;
            if (files.ToList().Count>0)
              hj= files.ToList() [0].Name;

            UploadVisible = files.ToList().Count > 0;
            InvokeAsync(StateHasChanged);
        }

        private List<string> imageSources  = new();
        private const int maxAllowedFiles = 2;
        private string ErrorMessage;

        /// <summary>
        /// for later use - Update file not working in Maui - Application
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        async Task OnChangeFileInput(InputFileChangeEventArgs e)
        {   // e.GetMultipleFiles(2).FirstOrDefault().
            imageSources.Clear();
            ErrorMessage = string.Empty;
            if (e.FileCount > maxAllowedFiles)
            {
                ErrorMessage = $"Only {maxAllowedFiles} files can be uploaded";
                return;
            }

          
            IBrowserFile j = e.GetMultipleFiles(maxAllowedFiles).FirstOrDefault();


            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                using var stream = file.OpenReadStream();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                imageSources.Add($"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}");
            }
        }

    }
}
