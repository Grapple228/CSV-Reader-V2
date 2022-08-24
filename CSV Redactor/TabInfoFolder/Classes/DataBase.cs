using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CSV_Redactor.TabInfoFolder.Interfaces;

namespace CSV_Redactor.TabInfoFolder.Classes
{
    abstract class DataBase : ITab, IFile
    {
        public List<ITab> Tabs { get; protected set; }
        public TabControl TabControl { get; protected set; }
        public ListBox ListBox { get; protected set; }
        public SplitContainer SplitContainer { get; protected set; }
        public string Extension { get; set; }
        // ITab
        public string PartialName { get; set; }
        public string FullName { get; set; }
        public TabPage TabPage { get; protected set; }
        // IFile
        public string FilePath { get; set; }

        public DataBase()
        {
            Tabs = new();
            ListBox = Tab.CreateListBox();
            TabControl = Tab.CreateDataBaseTabControl();
            SplitContainer = new(){Dock = DockStyle.Fill};
            TabControl.MouseLeave += Handlers.TabControl_MouseLeave;
            TabControl.MouseMove += Handlers.TabControl_MouseMove;
            TabControl.MouseDown += Handlers.TabControl_MouseDown;
        }
        public static void CreateDataBaseTab()
        {
            OpenFileDialog ofd = new()
            {
                Title = "Открытие базы данных",
                Filter = string.Join("|", new string[] { Tab.DataBaseFilter, Tab.AllFilter })
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            string extension = Path.GetExtension(ofd.FileName).ToLower();
            DataBase Instance;
            switch (extension)
            {
                case ".mdf": Instance = new DataBaseSQLServer(ofd.FileName); break;
                case ".sqlite":
                case ".sqlite3":
                case ".db":
                case ".db3":
                case ".s3db":
                case ".sl3": Instance = new DataBaseSQLite(ofd.FileName); break;
                case ".myd": Instance = new DataBaseMySQL(ofd.FileName); break;
                default:
                    MessageBox.Show($"База данных с расширением \"{extension}\" не поддерживается!",
                        "Внимание!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
            }
            Instance.Extension = extension;
            foreach (ITab tab in Tab.Tabs)
            {
                if (tab is DataBase database && database.FilePath == Instance.FilePath)
                {
                    Tab.Main_TabControl.SelectedTab = tab.TabPage;
                    database.LoadData();
                    return;
                }
            }
            Instance.PartialName = ofd.SafeFileName;
            Instance.FullName = Tab.CreateUniqueName(Instance.PartialName);

            Instance.TabPage = new(Instance.PartialName)
            {
                Name = Instance.FullName + "_Tab",
                ImageIndex = 2
            };
            Instance.LoadTables();

            Instance.SplitContainer.Panel1.Controls.Add(Instance.ListBox);
            Instance.SplitContainer.Panel2.Controls.Add(Instance.TabControl);
            Instance.TabPage.Controls.Add(Instance.SplitContainer);
            Tab.Main_TabControl.TabPages.Add(Instance.TabPage);

            // Установка минимальной ширины для TabControl
            int width = Instance.SplitContainer.Width;
            Instance.SplitContainer.Panel2MinSize = width / 4 * 3;

            Tab.Main_TabControl.SelectedTab = Instance.TabPage;
            Tab.Tabs.Add(Instance);
            Tab.ChangeProgramMenuAvailability();
            Tab.ChangeFormElementsVisibility(true);
        }
        public abstract void CreateNewTab();
        protected abstract void LoadTables();
        public abstract void LoadData();
        public abstract void Save();
    }

    internal class DataBaseSQLServer : DataBase
    {
        protected SqlConnection Connection;
        protected const string GET_TABLE_NAMES = "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_TYPE LIKE '%TABLE%'";
        protected const string GET_ALL_FROM_TABLE = "SELECT * FROM ";
        
        public DataBaseSQLServer(string filePath)
        {
            FilePath = filePath;
            CreateConnection();
        }

        public override void CreateNewTab()
        {
            string tableName = ListBox.SelectedItem.ToString().Trim();
            var tab = Tabs.Find(tab => tab.PartialName == tableName);
            if (tab != null)
            {
                TabControl.SelectedTab = tab.TabPage;
                LoadData();
                return;
            } 
            DataBaseTable table = new(tableName, this);
            table.CreateNewTab();

            // Получить заголовки столбцов таблицы
            // Получить типы данных столбцов таблицы
            // Получить содержимое столбцов таблицы
            // Заголовок = ColumnName[Type]



        }
        public override void LoadData()
        {
        }
        public override void Save()
        {
        }
        protected override void LoadTables()
        {
            OpenConnection();
            ListBox.Items.Clear();

            List<object[]> result = DataReader(GET_TABLE_NAMES);
            string[] tableNames = new string[result.Count];
            for (int i = 0; i < result.Count; i++)
                tableNames[i] = result[i][0].ToString();

            ListBox.Items.AddRange(tableNames);
            CloseConnection();
        }
        protected void CreateConnection()
        {
            Connection = new()
            {
                ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + $"\"{FilePath}\";Integrated Security=True;Connect Timeout=30"
            };
        }
        protected void OpenConnection()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
        protected void CloseConnection()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
                SqlConnection.ClearPool(Connection);
            }
        }
        protected List<object[]> DataReader(string commandText)
        {
            List<object[]> result = new();
            OpenConnection();
            SqlCommand sqlCommand = new(commandText, Connection);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    object[] row = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                        row[i] = reader.GetValue(i);
                    result.Add(row);
                }
            }
            CloseConnection();
            return result;
        }
    }
    internal class DataBaseMySQL : DataBase
    {
        public DataBaseMySQL(string filePath)
        {
            FilePath = filePath;
        }

        public override void CreateNewTab()
        {

        }
        public override void LoadData()
        {
        }
        public override void Save()
        {
        }
        protected override void LoadTables()
        {
        }
    }
    internal class DataBaseSQLite : DataBase
    {
        public DataBaseSQLite(string filePath)
        {
            FilePath = filePath;
        }

        public override void CreateNewTab()
        {

        }
        public override void LoadData()
        {
        }
        public override void Save()
        {
        }
        protected override void LoadTables()
        {
        }
    }

    internal class DataBaseTable : ITab, IDefaultFieldsOfTabs
    {
        public DataBase DataBase { get; protected set; }

        public DataBaseTable(string tableName, DataBase dataBase)
        {
            PartialName = tableName;
            DataBase = dataBase;
            FillFields(this);
        }

        // ITab
        public string PartialName { get; set; }
        public string FullName { get; set; }
        public TabPage TabPage { get; protected set; }

        // IDefaultFieldsOfTabs
        public DataGridView DataGridView { get; protected set; }
        public RichTextBox TextBox { get; protected set; }
        public List<string> Data { get; protected set; }
        public int ColumnCount { get; set; }
        public int FixedColumnCount { get; set; }
        public int RowCount { get; set; }
        public char Separator { get; set; }
        public bool IsChanged { get; set; }
        public bool IsHideEmptyRows { get; set; }
        public bool IsShowAsTable { get; set; }
        public bool IsStretchCells { get; set; }
        public bool IsFixed { get; set; }

        protected static void FillFields(DataBaseTable Instance)
        {
            Instance.Separator = Tab.Separators[0];
            Instance.Data = new();
            Instance.DataGridView = Tab.CreateDataGridView();
            Instance.TextBox = Tab.CreateTextBox();

            Instance.IsChanged = false;
            Instance.IsHideEmptyRows = Main_Form.Settings.IsHideEmptyRows;
            Instance.IsShowAsTable = Main_Form.Settings.IsShowAsTable;
            Instance.IsStretchCells = Main_Form.Settings.IsStretchCells;
            Instance.IsFixed = true;
        }
        public void CreateNewTab()
        {
            FullName = Tab.CreateUniqueName(PartialName);
            DataGridView.Name = FullName+"_DataGridView";
            TextBox.Name = FullName+"_TextBox";
            TabPage = new(PartialName)
            {
                Name = FullName + "_Tab",
                ImageIndex = 0
            };
            TabPage.Controls.Add(DataGridView);
            TabPage.Controls.Add(TextBox);
            
            if (IsShowAsTable) DataGridView.Visible = true;
            else TextBox.Visible = true;

            DataBase.TabControl.TabPages.Add(TabPage);
            DataBase.TabControl.SelectedTab = TabPage;
            DataBase.Tabs.Add(this);
            Tab.Tabs.Add(this);
            Tab.ChangeProgramMenuAvailability(this);
            Tab.SetStatusBarInfoLabel(this);
            Tab.ChangeFormElementsVisibility(true);
        }
        public void Clear()
        {
            if (IsShowAsTable)
                DataGridView.Rows.Clear();
            else TextBox.Text = TextBox.Lines[0];
        }
        public void SaveAs()
        {

        }
        public List<string> ReadData(string filePath = null)
        {
            List<string> data = new();

            return data;
        }
        public void WriteData(string filePath = null)
        {

        }
    }
}
