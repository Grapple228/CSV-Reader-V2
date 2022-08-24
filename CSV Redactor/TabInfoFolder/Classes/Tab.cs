using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CSV_Redactor.TabInfoFolder.Interfaces;

namespace CSV_Redactor.TabInfoFolder.Classes
{
    /// <summary>
    /// Статические поля программы
    /// </summary>
    internal static class Tab
    {
        public static Form MainForm_Form { get; }
        public static TabControl Main_TabControl { get; }
        public static ToolStripLabel TabInfo_ToolStripLabel { get; }
        public static MenuStrip ProgramMenu_MenuStrip { get; }
        public static ToolStrip QickActionsMenu_ToolStrip { get; }
        public static ToolStripTextBox ColumnCount_TextBox { get; }
        public static ToolStrip StatusBar_ToolStrip { get; }
        public static bool IsDialogAlreadyOpened { get; set; } = false;
        public const string DataBaseFilter =
            "SQL Server|*.mdf|" +
            "SQLite|*.sqlite;*.sqlite3;*.db;*.db3;*.s3db;*.sl3|" +
            "MySQL|*.myd";
        public const string TextFilter =
            "Text files|*.csv;*.txt";
        public const string AllFilter =
            "All files|*.*";

        public static List<ITab> Tabs { get; }
        public static string Separators { get; }
        public static object[][] CopiedRows { get; set; }
        public static object[][] CopiedColumns { get; set; }
        public static Button CloseTabButton { get; }
        public static int ClickedTabNumber { get; set; }
        public static int ClickedRowIndex { get; set; }
        public static int ClickedColumnIndex { get; set; }
        public static ContextMenuStrip ContextMenuOnTabClick { get; }

        static Tab()
        {
            MainForm_Form = Application.OpenForms[0];
            Main_TabControl = (TabControl)MainForm_Form.Controls["Files_TabControl"];
            StatusBar_ToolStrip = (ToolStrip)MainForm_Form.Controls["StatusBar"];
            TabInfo_ToolStripLabel = (ToolStripLabel)StatusBar_ToolStrip.Items["tabInfo_ToolStripLabel"];
            ProgramMenu_MenuStrip = (MenuStrip)MainForm_Form.Controls["ProgramActionsBar"];
            QickActionsMenu_ToolStrip = (ToolStrip)MainForm_Form.Controls["ActionButtons_ToolStrip"];
            ColumnCount_TextBox = (ToolStripTextBox)QickActionsMenu_ToolStrip.Items["columnCountBox_TextBox"];

            Tabs = new();
            CloseTabButton = new();
            {
                CloseTabButton.Padding = new Padding(0, 0, 0, 0);
                CloseTabButton.AutoEllipsis = false;
                CloseTabButton.Margin = new Padding(0, 0, 0, 0);
                CloseTabButton.BackgroundImage = new System.Drawing.Bitmap(CSV_Redactor.Properties.Resources.redCross);
                CloseTabButton.BackgroundImageLayout = ImageLayout.Stretch;
                CloseTabButton.Size = new System.Drawing.Size(13, 13);
                CloseTabButton.FlatStyle = FlatStyle.Flat;
                CloseTabButton.FlatAppearance.BorderSize = 0;
                CloseTabButton.Enabled = false;
            }
            Separators = Main_Form.Settings.SupportedSeparators;
        }

        public static string[] GenColumnNames(string[] rawNames)
        {
            string[] names = new string[rawNames.Length];
            Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { rawNames });
            try
            {
                for (int i = 0; i < rawNames.Length; i++)
                {
                    string curName = rawNames[i];

                    string defName = Main_Form.Settings.DefaultColumnName;

                    curName = curName == null || curName.ToString() == "" ?
                        defName.Trim() :
                        (curName.Substring(0, 1).ToUpper() + (curName.Length > 1 ? curName.Substring(1) : "")).Trim();

                    if (names.Contains(curName))
                    {
                        int index = 2;
                        string tempName = curName;
                        while (names.Contains(tempName))
                        {
                            tempName = curName + "-" + index.ToString();
                            index++;
                        }
                        curName = tempName.Trim();
                    }

                    names[i] = curName.Trim();
                }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            return names;
        }
        public static string CreateUniqueName(string oldName)
        {
            string name = "";
            Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { oldName });
            try
            {
                name = $"{oldName}_{DateTime.Now:yyyy.MM.dd HH:mm:ss.fff}";
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            return name;
        }
        public static void ChangeStretchDataGridView(DataGridView dataGridView, bool isStretch)
        {
            if (isStretch)
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }

            else
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }
        public static void SetStatusBarInfoLabel(IDefaultFieldsOfTabs Instance)
        {
            if (Instance.IsShowAsTable)
            {
                Instance.RowCount = Instance.DataGridView.RowCount - 1;
                TabInfo_ToolStripLabel.Text = $"Столбцов : {Instance.ColumnCount}   Строк : {Instance.RowCount}";
            }
            else
            {
                Instance.RowCount = Instance.TextBox.Lines.Length;
                TabInfo_ToolStripLabel.Text = $"Строк : {Instance.TextBox.Lines.Length}";
            }
        }
        public static void ChangeFormElementsVisibility(bool visibility)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { visibility });
            try
            {
                if (QickActionsMenu_ToolStrip.Visible == visibility) return;

                if (visibility)
                {
                    Main_TabControl.Height = Main_TabControl.Height - 25;
                    Main_TabControl.Location = new System.Drawing.Point(0, Main_TabControl.Location.Y + 25);
                }
                else
                {
                    Main_TabControl.Height = Main_TabControl.Height + 25;
                    Main_TabControl.Location = new System.Drawing.Point(0, Main_TabControl.Location.Y - 25);
                    TabInfo_ToolStripLabel.Text = "";
                }
                QickActionsMenu_ToolStrip.Visible = visibility;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static DataGridView CreateDataGridView()
        {
            DataGridView dataGridView = new()
            {
                Dock = DockStyle.Fill,
                BackgroundColor = System.Drawing.SystemColors.Control,
                Visible = false
            };
            DataGridViewColumn dataGridViewColumn = new() { CellTemplate = new DataGridViewTextBoxCell() };
            dataGridView.Columns.Add(dataGridViewColumn);
            dataGridView.DefaultCellStyle.Font = new System.Drawing.Font("Lucida Console", 9.5f);
            dataGridView.ColumnHeadersDefaultCellStyle.Font =
                dataGridView.RowHeadersDefaultCellStyle.Font =
                new System.Drawing.Font("Lucida Console", 10.5f, System.Drawing.FontStyle.Italic);

            dataGridView.CellMouseClick += Handlers.DataGridView_CellMouseClick;
            dataGridView.CellEndEdit += Handlers.DataGridView_CellEndEdit;
            dataGridView.DefaultValuesNeeded += Handlers.DataGridView_DefaultValuesNeeded;
            dataGridView.RowsAdded += Handlers.DataGridView_RowsAdded;

            return dataGridView;
        }
        public static RichTextBox CreateTextBox()
        {
            RichTextBox textBox = new()
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Lucida Console", 9.5f),
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = false,
                BackColor = System.Drawing.SystemColors.Control,
                Visible = false
            };
            textBox.TextChanged += Handlers.TextBox_TextChanged;
            return textBox;
        }
        public static TabControl CreateDataBaseTabControl()
        {
            ImageList list = new();
            list.Images.Add(new System.Drawing.Bitmap(Properties.Resources.table));
            TabControl tabControl = new()
            {
                Dock = DockStyle.Fill,
                ImageList = list
            };
            return tabControl;
        }
        public static ListBox CreateListBox()
        {
            ListBox listBox = new()
            {
                Dock = DockStyle.Fill,
                Sorted = true
            };
            listBox.DoubleClick += Handlers.ListBox_DoubleClick;

            return listBox;
        }
        /// <summary>
        /// Получить класс текущей открытой вкладки
        /// </summary>
        /// <param name="tabControl">Ссылка на экземпляр TabControl</param>
        /// <param name="deepSearch">Поиск по вкладкам базы данных</param>
        /// <returns>Класс открытой вкладки</returns>
        public static ITab FindClassOfCurrentTab(TabControl tabControl, bool deepSearch = true)
        {
            ITab tab = Tabs.Find(tab => tab.TabPage == tabControl.SelectedTab);
            if(deepSearch && tab is DataBase dataBase)
            {
                tab = FindClassOfCurrentTab(dataBase.TabControl);
            }
            return tab;
        }
        public static ITab FindCurrentTabClass(object sender, bool deepSearch = true)
        {
            if(sender is TabControl tabControl)
            {
                return FindClassOfCurrentTab(tabControl, deepSearch);
            } else return null;
        }
        public static DialogResult ShowCreationNewFileWindow(ref string inputText)
        {
            if (IsDialogAlreadyOpened) return DialogResult.Cancel;
            IsDialogAlreadyOpened = true;
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            DialogResult result = DialogResult.Cancel;
            try
            {
                System.Drawing.Size size = new(165, 75);
                Form inputBox = new() { StartPosition = FormStartPosition.CenterParent, Location = new System.Drawing.Point(), MaximizeBox = false, MinimizeBox = false, FormBorderStyle = FormBorderStyle.FixedDialog, ClientSize = size, Text = "Создание" };

                Label label = new() { AutoSize = true, Size = new(5, 5), Location = new System.Drawing.Point(5, 5), Text = "Введите имя файла:" };
                inputBox.Controls.Add(label);

                TextBox nameTextBox = new() { Size = new(size.Width - 10, 10), Location = new System.Drawing.Point(5, 20), Text = inputText };
                inputBox.Controls.Add(nameTextBox);

                Button addButton = new() { DialogResult = DialogResult.OK, Name = "addButton", Size = new(75, 25), Text = "&Добавить", Location = new System.Drawing.Point(size.Width - 80, 44) };
                inputBox.Controls.Add(addButton);

                Button cancelButton = new() { DialogResult = DialogResult.Cancel, Name = "cancelButton", Size = new(75, 25), Text = "&Отмена", Location = new System.Drawing.Point(size.Width - 80 - 80, 44) };
                inputBox.Controls.Add(cancelButton);

                inputBox.AcceptButton = addButton;
                inputBox.CancelButton = cancelButton;

                result = inputBox.ShowDialog();
                inputText = nameTextBox.Text;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            IsDialogAlreadyOpened = false;
            return result;
        }
        public static void ChangeProgramMenuAvailability(IDefaultFieldsOfTabs Instance)
        {
            bool isOpenedFile = Instance is OpenedTextFile;
            bool isOpenedDataBase = Instance is DataBase;

            ToolStripItemCollection itemsInFile = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["file"]).DropDown.Items;
            ((ToolStripMenuItem)itemsInFile["createFile"]).Enabled = true;
            ((ToolStripMenuItem)itemsInFile["openFile"]).Enabled = true;
            ((ToolStripMenuItem)itemsInFile["saveFile"]).Enabled = isOpenedFile;
            ((ToolStripMenuItem)itemsInFile["saveFileAs"]).Enabled = true;
            ToolStripItemCollection itemsInDataBase = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["dataBase"]).DropDown.Items;
            ((ToolStripMenuItem)itemsInDataBase["openDB"]).Enabled = true;
            //((ToolStripMenuItem)itemsInDataBase["saveDB"]).Enabled = isOpenedDataBase;
            ToolStripItemCollection itemsInEdit = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["edit"]).DropDown.Items;
            ((ToolStripMenuItem)itemsInEdit["refresh"]).Enabled = isOpenedFile;
            ((ToolStripMenuItem)itemsInEdit["clear"]).Enabled = true;
            ToolStripItemCollection itemsInView = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["view"]).DropDown.Items;
            ToolStripMenuItem hideEmptyRows = (ToolStripMenuItem)itemsInView["hideEmptyRows"];
            hideEmptyRows.Enabled = Instance.IsShowAsTable;
            hideEmptyRows.Checked = Instance.IsHideEmptyRows && Instance.IsShowAsTable;

            ((ToolStripMenuItem)itemsInView["showAsTable"]).Enabled = true;
            ((ToolStripMenuItem)itemsInView["showAsTable"]).Checked = Instance.IsShowAsTable;
            ((ToolStripMenuItem)itemsInView["stretchCells"]).Enabled = true;
            ((ToolStripMenuItem)itemsInView["stretchCells"]).Checked = Instance.IsStretchCells && Instance.IsShowAsTable;

            QickActionsMenu_ToolStrip.Items["increaseColumnCount_Button"].Visible = Instance.IsShowAsTable;
            QickActionsMenu_ToolStrip.Items["decreaseColumnCount_Button"].Visible = Instance.IsShowAsTable;
            QickActionsMenu_ToolStrip.Items["columnCountBox_TextBox"].Visible = Instance.IsShowAsTable;
            //QickActionsMenu_ToolStrip.Items["columnCountBox_TextBox"].Enabled = true;
        }
        public static void ChangeProgramMenuAvailability()
        {
            ToolStripItemCollection itemsInFile = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["file"]).DropDown.Items;
            ((ToolStripMenuItem)itemsInFile["createFile"]).Enabled = true;
            ((ToolStripMenuItem)itemsInFile["openFile"]).Enabled = true;
            ((ToolStripMenuItem)itemsInFile["saveFile"]).Enabled = false;
            ((ToolStripMenuItem)itemsInFile["saveFileAs"]).Enabled = false;
            ToolStripItemCollection itemsInDataBase = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["dataBase"]).DropDown.Items;
            ((ToolStripMenuItem)itemsInDataBase["openDB"]).Enabled = true;
            ToolStripItemCollection itemsInEdit = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["edit"]).DropDown.Items;
            ((ToolStripMenuItem)itemsInEdit["refresh"]).Enabled = false;
            ((ToolStripMenuItem)itemsInEdit["clear"]).Enabled = false;
            ToolStripItemCollection itemsInView = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["view"]).DropDown.Items;
            ToolStripMenuItem hideEmptyRows = (ToolStripMenuItem)itemsInView["hideEmptyRows"];
            hideEmptyRows.Enabled = false;
            hideEmptyRows.Checked = false;

            ((ToolStripMenuItem)itemsInView["showAsTable"]).Enabled = false;
            ((ToolStripMenuItem)itemsInView["showAsTable"]).Checked = false;
            ((ToolStripMenuItem)itemsInView["stretchCells"]).Enabled = false;
            ((ToolStripMenuItem)itemsInView["stretchCells"]).Checked = false;

            QickActionsMenu_ToolStrip.Items["increaseColumnCount_Button"].Visible = false;
            QickActionsMenu_ToolStrip.Items["decreaseColumnCount_Button"].Visible = false;
            QickActionsMenu_ToolStrip.Items["columnCountBox_TextBox"].Visible = false;
        }
    }
}
