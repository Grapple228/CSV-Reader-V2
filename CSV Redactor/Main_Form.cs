using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;
using static CSV_Redactor.Main_Form;

namespace CSV_Redactor
{
    /* 
     * TODO: Дописать FillActionsBar
     */

    public partial class Main_Form : Form
    {
        public readonly static string BaseDirectory;
        public readonly static XDocument SettingsFile;
        public readonly static bool IsTraceCallsEnabled;
        public static TextWriterTraceListener TraceListener;
        public static string TraceErrorListenerText = "";

        static Main_Form()
        {
            BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            SettingsFile = Methods.LoadSettingsFile(BaseDirectory);
            IsTraceCallsEnabled = Convert.ToBoolean(Methods.GetFieldsFromSettings(SettingsFile, "global-IsTracingEnabled").Value);
            
            if(IsTraceCallsEnabled)
                FillTraceListener();
            
            static void FillTraceListener()
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod());
                try
                {
                    if (!Directory.Exists(BaseDirectory + "/Executions Info")) Directory.CreateDirectory(BaseDirectory + "/Executions Info");
                    string traceListenerPath = (BaseDirectory + "/Executions Info/" + $"Execution {DateTime.Now:yyyy.MM.dd HH-mm-ss.fff}.log").Replace("\\", "/");

                    Trace.AutoFlush = true;
                    TraceListener = new TextWriterTraceListener(File.CreateText(traceListenerPath));
                    Trace.Listeners.Add(TraceListener);
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
        }
        public Main_Form()
        {
            try
            {
                InitializeComponent();
                XElement globalSettings = Methods.GetFieldsFromSettings(SettingsFile, "global");
                int width = Convert.ToInt32(globalSettings.Element("minimumWindowWidth").Value),
                    height = Convert.ToInt32(globalSettings.Element("minimumWindowHeight").Value);
                Height = height;
                Width = width;
                MinimumSize = new(width, height);

                FillActionsBar();
                if (!Convert.ToBoolean(Methods.GetFieldsFromSettings(SettingsFile, "global-showStatusBar").Value))
                {
                    Files_TabControl.Height += 25;
                    StatusBar.Visible = false;
                }

                AddEventHandlersToFormComponents();
                AddImageListToTabs();
                StartPosition = FormStartPosition.CenterScreen;

                if(IsTraceCallsEnabled)
                    Debug.WriteLine($"{DateTime.Now:yyyy.MM.dd HH:mm:ss.fff} Выполнен запуск программы\n");
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }

        #region Класс TabInfo
        // Поля TabInfo
        public partial class TabInfo
        {
            #region Default Form Components
            public static Form MainForm_Form { get; }
            public static TabControl Files_TabControl { get; }
            public static ToolStripLabel TabInfo_ToolStripLabel { get; }
            public static MenuStrip ProgramMenu_MenuStrip { get; }
            public static ToolStrip QickActionsMenu_ToolStrip { get; }
            public static ToolStripTextBox ColumnCount_TextBox { get; }
            public static ToolStrip StatusBar_ToolStrip { get; }
            #endregion


            // Остальные поля
            public static bool IsShowStatusBar { get; set; }
            public static List<TabInfo> TabsInfo { get; }
            public static Button CloseTabButton { get; }
            public static int ClickedTabNumber { get; set; }
            public static int ClickedRowIndex { get; set; }
            public static int ClickedColumnIndex { get; set; }
            public static object[][] CopiedRows { get; set; }
            public static object[][] CopiedColumns { get; set; }
            public static ContextMenuStrip ContextMenuOnRowClick { get; private set; }
            public static ContextMenuStrip ContextMenuOnNewRowClick { get; private set; }
            public static ContextMenuStrip ContextMenuOnHeaderClick { get; private set; }
            public static ContextMenuStrip ContextMenuOnTabClick { get; private set; }
            public string FullTabName { get; private set; }
            public string ShortTabName { get; private set; }
            public string Extension { get; private set; }
            public int ColumnCount { get; set; }
            public int FixedColumnCount { get; set; }
            public int RowCount { get; set; }
            public bool IsChanged { get; set; }
            public bool IsHideEmptyRows { get; set; }
            public bool IsShowAsTable { get; set; }
            public bool IsStretchCells { get; set; }
            public bool IsFixed { get; set; }
            public bool IsDataBase { get; set; }
            public bool IsFileOpened { get; set; }
            public char Separator { get; set; }
            public System.Drawing.Color SeparatorColor { get; set; }
            public List<object> Data { get; set; }
            public DataGridView DataGridView { get; private set; }
            public RichTextBox TextBox { get; private set; }
        }

        // Конструкторы TabInfo
        public partial class TabInfo
        {
            /// <summary>
            /// Статический конструктор класса TabInfo
            /// </summary>
            static TabInfo()
            {
                try
                {
                    MainForm_Form = Application.OpenForms[0];
                    Files_TabControl = (TabControl)MainForm_Form.Controls["Files_TabControl"];
                    StatusBar_ToolStrip = (ToolStrip)MainForm_Form.Controls["StatusBar"];
                    TabInfo_ToolStripLabel = (ToolStripLabel)StatusBar_ToolStrip.Items["tabInfo_ToolStripLabel"];
                    ProgramMenu_MenuStrip = (MenuStrip)MainForm_Form.Controls["ProgramActionsBar"];
                    QickActionsMenu_ToolStrip = (ToolStrip)MainForm_Form.Controls["ActionButtons_ToolStrip"];
                    ColumnCount_TextBox = (ToolStripTextBox)QickActionsMenu_ToolStrip.Items["columnCountBox_TextBox"];
                    IsShowStatusBar = Convert.ToBoolean(Methods.GetFieldsFromSettings(SettingsFile, "global-showStatusBar").Value);

                    TabsInfo = new List<TabInfo>();
                    CloseTabButton = new Button();
                    {
                        CloseTabButton.Padding = new Padding(0, 0, 0, 0);
                        CloseTabButton.AutoEllipsis = false;
                        CloseTabButton.Margin = new Padding(0, 0, 0, 0);
                        CloseTabButton.BackgroundImage = new System.Drawing.Bitmap(Properties.Resources.redCross);
                        CloseTabButton.BackgroundImageLayout = ImageLayout.Stretch;
                        CloseTabButton.Size = new System.Drawing.Size(13, 13);
                        CloseTabButton.FlatStyle = FlatStyle.Flat;
                        CloseTabButton.FlatAppearance.BorderSize = 0;
                        CloseTabButton.Enabled = false;
                    }
                    ContextMenuOnRowClick = CreateContextMenuOnRowClick();
                    ContextMenuOnNewRowClick = CreateContextMenuOnNewRowClick();
                    ContextMenuOnHeaderClick = CreateContextMenuOnHeaderClick();
                    ContextMenuOnTabClick = CreateContextMenuOnTabClick();
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
            /// <summary>
            /// Конструктор класса TabInfo
            /// </summary>
            /// <param name="tabName">Имя вкладки</param>
            /// <param name="dataGridView">Ссылка на DataGridView</param>
            /// <param name="textBox">Ссылка на RichTextBox</param>
            public TabInfo(string fullTabName, string shortTabName, string extension, DataGridView dataGridView = null, RichTextBox textBox = null)
            {
                try
                {
                    XElement localCsvSettings = Methods.GetFieldsFromSettings(SettingsFile, "local-csv");

                    FullTabName = fullTabName;
                    ShortTabName = shortTabName;
                    Extension = extension;
                    IsChanged = false;
                    IsHideEmptyRows = Convert.ToBoolean(localCsvSettings.Element("hideEmptyRows").Value);
                    IsShowAsTable = Convert.ToBoolean(localCsvSettings.Element("showAsTable").Value);
                    IsStretchCells = Convert.ToBoolean(localCsvSettings.Element("stretchCells").Value);
                    IsFileOpened = false;
                    IsDataBase = false;
                    ColumnCount = Convert.ToInt32(localCsvSettings.Element("defaultColumnCount").Value);
                    FixedColumnCount = ColumnCount;
                    RowCount = 0;
                    IsFixed = false;
                    Separator = Convert.ToChar(localCsvSettings.Element("defaultSeparator").Value);
                    SeparatorColor = System.Drawing.ColorTranslator.FromHtml(localCsvSettings.Element("separatorColor").Value);
                    Data = new List<object>();

                    DataGridView = dataGridView;
                    TextBox = textBox;

                    DataGridView.Visible = IsShowAsTable;
                    TextBox.Visible = !IsShowAsTable;

                    ChangeProgramMenuAvailability(true, this);
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
            /// <summary>
            /// Конструктор для дублирования экземпляра класса
            /// </summary>
            /// <param name="tabInfo">Экземпляр класса</param>
            /// <param name="dataGridView">Новый экземпляр DataGridView</param>
            /// <param name="textBox">Новый экземпляр RichTextBox</param>
            public TabInfo(TabInfo tabInfo, DataGridView dataGridView, RichTextBox textBox)
            {
                try
                {
                    ColumnCount = tabInfo.ColumnCount;
                    FixedColumnCount = tabInfo.FixedColumnCount;
                    RowCount = tabInfo.RowCount;
                    IsChanged = tabInfo.IsChanged;
                    IsHideEmptyRows = tabInfo.IsHideEmptyRows;
                    IsShowAsTable = tabInfo.IsShowAsTable;
                    IsFixed = tabInfo.IsFixed;
                    IsDataBase = tabInfo.IsDataBase;
                    IsFileOpened = tabInfo.IsFileOpened;
                    Separator = tabInfo.Separator;
                    SeparatorColor = tabInfo.SeparatorColor;
                    Data = tabInfo.Data;
                    DataGridView = dataGridView;
                    TextBox = textBox;
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
        }

        // Методы TabInfo
        /// <summary>
        /// Информация о вкладках Files_TabControl
        /// </summary>
        public partial class TabInfo
        {
            /// <summary>
            /// Создание новой вкладки TabPage
            /// </summary>
            /// <param name="pageName">Имя создаваемой вкладки</param>
            /// <returns>TabPage</returns>
            public static TabPage CreateNewTabPage(string pageName)
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { pageName});
                TabPage newPage = new(pageName);
                try
                {
                    string newName = Methods.CreateUniqueName(pageName);
                    newPage.Name = newName;

                    DataGridView dataGridView = new()
                    {
                        Name = newName + "_DataGridView",
                        Dock = DockStyle.Fill,
                        BackgroundColor = System.Drawing.SystemColors.Control,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
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
                    newPage.Controls.Add(dataGridView);

                    RichTextBox textBox = new()
                    {
                        Name = newName + "_TextBox",
                        Dock = DockStyle.Fill,
                        Visible = false,
                        Multiline = true,
                        ScrollBars = RichTextBoxScrollBars.Both,
                        WordWrap = false,
                        BackColor = System.Drawing.SystemColors.Control,
                        Font = new System.Drawing.Font("Lucida Console", 11)
                    };
                    textBox.LostFocus += Handlers.TextBox_LostFocus;
                    textBox.TextChanged += Handlers.TextBox_TextChanged;

                    newPage.Controls.Add(textBox);

                    ChangeProgramMenuAvailability(true);
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
                return newPage;
            }
            /// <summary>
            /// Создать контекстное меню для нажатия по вкладке
            /// </summary>
            public static ContextMenuStrip CreateContextMenuOnTabClick()
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod());
                ContextMenuStrip contextMenuOnTabClick = new() { ShowCheckMargin = false, ShowImageMargin = false, AutoSize = true };
                try
                {
                    ToolStripMenuItem duplicateTab = new("Дублировать вкладку") { Name = "Дублировать вкладку" };
                    ToolStripMenuItem renameTab = new("Переименовать вкладку") { Name = "Переименовать вкладку" };
                    ToolStripMenuItem closeTab = new("Закрыть вкладку") { Name = "Закрыть вкладку" };

                    contextMenuOnTabClick.Items.AddRange(new[] { duplicateTab, closeTab });

                    duplicateTab.Click += Handlers.DuplicateTab_Click;
                    renameTab.Click += Handlers.RenameTab_Click;
                    closeTab.Click += Handlers.CloseTab_Click;
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
                return contextMenuOnTabClick;
            }
            /// <summary>
            /// Создать контекстное меню для нажатия по строке заголовков DataGridView
            /// </summary>
            public static ContextMenuStrip CreateContextMenuOnHeaderClick()
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod());
                ContextMenuStrip contextMenuOnHeaderClick = new() { ShowCheckMargin = false, ShowImageMargin = false, AutoSize = true };
                try
                {
                    {
                        ToolStripMenuItem changeHeader = new("Изменить заголовок") { Name = "Изменить заголовок" };
                        ToolStripMenuItem pinColumn = new("Закрепить столбец") { Name = "Закрепить столбец" };
                        ToolStripMenuItem moveColumnToLeft = new("Переместить влево") { Name = "Переместить влево" };
                        ToolStripMenuItem moveColumnToRight = new("Переместить вправо") { Name = "Переместить вправо" };
                        ToolStripMenuItem addColumnToLeft = new("Добавить слева") { Name = "Добавить слева" };
                        ToolStripMenuItem addColumnToRight = new("Добавить справа") { Name = "Добавить справа" };
                        ToolStripMenuItem duplicateColumn = new("Дублировать") { Name = "Дублировать" };
                        ToolStripMenuItem copyColumn = new("Копировать") { Name = "Копировать" };
                        ToolStripMenuItem cutColumn = new("Вырезать") { Name = "Вырезать" };
                        ToolStripMenuItem insertColumnToLeft = new("Вставить слева") { Name = "Вставить слева" };
                        ToolStripMenuItem insertColumnToCurrent = new("Вставить в текущий") { Name = "Вставить в текущий" };
                        ToolStripMenuItem insertColumnToRight = new("Вставить справа") { Name = "Вставить справа" };
                        ToolStripMenuItem clearColumn = new("Очистить") { Name = "Очистить" };
                        ToolStripMenuItem removeColumn = new("Удалить") { Name = "Удалить" };

                        contextMenuOnHeaderClick.Items.Add(changeHeader);
                        contextMenuOnHeaderClick.Items.Add(pinColumn);
                        contextMenuOnHeaderClick.Items.Add(new ToolStripSeparator());
                        contextMenuOnHeaderClick.Items.Add(moveColumnToLeft);
                        contextMenuOnHeaderClick.Items.Add(moveColumnToRight);
                        contextMenuOnHeaderClick.Items.Add(new ToolStripSeparator());
                        contextMenuOnHeaderClick.Items.Add(addColumnToLeft);
                        contextMenuOnHeaderClick.Items.Add(addColumnToRight);
                        contextMenuOnHeaderClick.Items.Add(new ToolStripSeparator());
                        contextMenuOnHeaderClick.Items.Add(duplicateColumn);
                        contextMenuOnHeaderClick.Items.Add(copyColumn);
                        contextMenuOnHeaderClick.Items.Add(cutColumn);
                        contextMenuOnHeaderClick.Items.Add(new ToolStripSeparator());
                        contextMenuOnHeaderClick.Items.Add(insertColumnToLeft);
                        contextMenuOnHeaderClick.Items.Add(insertColumnToCurrent);
                        contextMenuOnHeaderClick.Items.Add(insertColumnToRight);
                        contextMenuOnHeaderClick.Items.Add(new ToolStripSeparator());
                        contextMenuOnHeaderClick.Items.Add(clearColumn);
                        contextMenuOnHeaderClick.Items.Add(removeColumn);

                        changeHeader.Click += Handlers.ChangeHeader_Click;
                        pinColumn.Click += Handlers.PinColumn_Click;
                        moveColumnToLeft.Click += Handlers.MoveColumnToLeft_Click;
                        moveColumnToRight.Click += Handlers.MoveColumnToRight_Click;
                        addColumnToLeft.Click += Handlers.AddColumnToLeft_Click;
                        addColumnToRight.Click += Handlers.AddColumnToRight_Click;
                        duplicateColumn.Click += Handlers.DuplicateColumn_Click;
                        copyColumn.Click += Handlers.CopyColumn_Click;
                        cutColumn.Click += Handlers.CutColumn_Click;
                        insertColumnToLeft.Click += Handlers.InsertColumnToLeft_Click;
                        insertColumnToRight.Click += Handlers.InsertColumnToRight_Click;
                        clearColumn.Click += Handlers.ClearColumn_Click;
                        removeColumn.Click += Handlers.RemoveColumn_Click;
                    }
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
                return contextMenuOnHeaderClick;
            }
            /// <summary>
            /// Создать контекстное меню для нажатия по существующей строке DataGridView
            /// </summary>
            public static ContextMenuStrip CreateContextMenuOnRowClick()
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod());
                ContextMenuStrip contextMenuOnRowClick = new() { ShowCheckMargin = false, ShowImageMargin = false, AutoSize = true };
                try
                {
                    {
                        ToolStripMenuItem moveRowUpper = new("Переместить выше") { Name = "Переместить выше" };
                        ToolStripMenuItem moveRowLower = new("Переместить ниже") { Name = "Переместить ниже" };
                        ToolStripMenuItem addRowUpper = new("Добавить выше") { Name = "Добавить выше" };
                        ToolStripMenuItem addRowLower = new("Добавить ниже") { Name = "Добавить ниже" };
                        ToolStripMenuItem duplicateRow = new("Дублировать") { Name = "Дублировать" };
                        ToolStripMenuItem copyRow = new("Копировать") { Name = "Копировать" };
                        ToolStripMenuItem cutRow = new("Вырезать") { Name = "Вырезать" };
                        ToolStripMenuItem insertRow = new("Вставить") { Name = "Вставить" };
                        ToolStripMenuItem insertRowToCurrent = new("Вставить в текущую") { Name = "Вставить в текущую" };
                        ToolStripMenuItem clearRow = new("Очистить") { Name = "Очистить" };
                        ToolStripMenuItem deleteRow = new("Удалить") { Name = "Удалить" };

                        contextMenuOnRowClick.Items.Add(moveRowUpper);
                        contextMenuOnRowClick.Items.Add(moveRowLower);
                        contextMenuOnRowClick.Items.Add(new ToolStripSeparator());
                        contextMenuOnRowClick.Items.Add(addRowUpper);
                        contextMenuOnRowClick.Items.Add(addRowLower);
                        contextMenuOnRowClick.Items.Add(new ToolStripSeparator());
                        contextMenuOnRowClick.Items.Add(duplicateRow);
                        contextMenuOnRowClick.Items.Add(copyRow);
                        contextMenuOnRowClick.Items.Add(cutRow);
                        contextMenuOnRowClick.Items.Add(new ToolStripSeparator());
                        contextMenuOnRowClick.Items.Add(insertRow);
                        contextMenuOnRowClick.Items.Add(insertRowToCurrent);
                        contextMenuOnRowClick.Items.Add(new ToolStripSeparator());
                        contextMenuOnRowClick.Items.Add(clearRow);
                        contextMenuOnRowClick.Items.Add(deleteRow);

                        moveRowUpper.Click += Handlers.MoveRowUpper_Click;
                        moveRowLower.Click += Handlers.MoveRowLower_Click;
                        addRowUpper.Click += Handlers.AddRowUpper_Click;
                        addRowLower.Click += Handlers.AddRowLower_Click;
                        duplicateRow.Click += Handlers.DuplicateRow_Click;
                        copyRow.Click += Handlers.CopyRow_Click;
                        cutRow.Click += Handlers.CutRow_Click;
                        insertRow.Click += Handlers.InsertRow_Click;
                        insertRowToCurrent.Click += Handlers.InsertRowToCurrent_Click;
                        clearRow.Click += Handlers.ClearRow_Click;
                        deleteRow.Click += Handlers.DeleteRow_Click;
                    }
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
                return contextMenuOnRowClick;
            }
            /// <summary>
            /// Создать контекстное меню для нажатия по новой строке DataGridView
            /// </summary>
            public static ContextMenuStrip CreateContextMenuOnNewRowClick()
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod());
                ContextMenuStrip contextMenuOnNewRowClick = new() { ShowCheckMargin = false, ShowImageMargin = false, AutoSize = true };
                try
                {
                    {
                        ToolStripMenuItem addRow = new("Добавить") { Name = "Добавить" };
                        ToolStripMenuItem insertRow = new("Вставить") { Name = "Вставить" };

                        contextMenuOnNewRowClick.Items.Add(addRow);
                        contextMenuOnNewRowClick.Items.Add(new ToolStripSeparator());
                        contextMenuOnNewRowClick.Items.Add(insertRow);

                        addRow.Click += Handlers.AddRowUpper_Click;
                        insertRow.Click += Handlers.InsertRow_Click;
                    }
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
                return contextMenuOnNewRowClick;
            }
            /// <summary>
            /// Изменение видимости элементов формы
            /// </summary>
            /// <param name="visibility">Состояние видимости</param>
            public static void ChangeFormElementsVisibility(bool visibility)
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] {visibility });
                try
                {
                    if (QickActionsMenu_ToolStrip.Visible == visibility) return;

                    if (visibility)
                    {
                        Files_TabControl.Height = Files_TabControl.Height - 25;
                        Files_TabControl.Location = new System.Drawing.Point(0, Files_TabControl.Location.Y + 25);
                    }
                    else
                    {
                        Files_TabControl.Height = Files_TabControl.Height + 25;
                        Files_TabControl.Location = new System.Drawing.Point(0, Files_TabControl.Location.Y - 25);
                        TabInfo_ToolStripLabel.Text = "";
                    }
                    QickActionsMenu_ToolStrip.Visible = visibility;
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
            /// <summary>
            /// Изменение видимости элементов программного меню
            /// </summary>
            /// <param name="availability">Доступность</param>
            public static void ChangeProgramMenuAvailability(bool availability)
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { availability});
                try
                {
                    ToolStripItemCollection itemsInFile = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["file"]).DropDown.Items;
                    ((ToolStripMenuItem)itemsInFile["createFile"]).Enabled = true;
                    ((ToolStripMenuItem)itemsInFile["openFile"]).Enabled = true;
                    ((ToolStripMenuItem)itemsInFile["saveFile"]).Enabled = availability;
                    ((ToolStripMenuItem)itemsInFile["saveFileAs"]).Enabled = availability;
                    ToolStripItemCollection itemsInDataBase = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["dataBase"]).DropDown.Items;
                    // database items
                    ToolStripItemCollection itemsInEdit = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["edit"]).DropDown.Items;
                    ((ToolStripMenuItem)itemsInEdit["refresh"]).Enabled = availability;
                    ((ToolStripMenuItem)itemsInEdit["clear"]).Enabled = availability;
                    ToolStripItemCollection itemsInView = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["view"]).DropDown.Items;
                    ((ToolStripMenuItem)itemsInView["showStatusBar"]).Enabled = true;
                    ((ToolStripMenuItem)itemsInView["hideEmptyRows"]).Enabled = availability;
                    ((ToolStripMenuItem)itemsInView["hideEmptyRows"]).Checked = false;
                    ((ToolStripMenuItem)itemsInView["showAsTable"]).Enabled = availability;
                    ((ToolStripMenuItem)itemsInView["showAsTable"]).Checked = availability;
                    ((ToolStripMenuItem)itemsInView["stretchCells"]).Enabled = availability;
                    ((ToolStripMenuItem)itemsInView["stretchCells"]).Checked = availability;
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
            /// <summary>
            /// Изменение видимости элементов программного меню для экземпляра класса
            /// </summary>
            /// <param name="availability">Доступность</param>
            /// <param name="tabInfo">Экземпляр класса</param>
            public static void ChangeProgramMenuAvailability(bool availability, TabInfo tabInfo)
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { availability, tabInfo});
                try
                {
                    switch (tabInfo.Extension)
                    {
                        case "csv": csv(); return;
                    }

                    void csv()
                    {
                        ToolStripItemCollection itemsInFile = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["file"]).DropDown.Items;
                        ((ToolStripMenuItem)itemsInFile["createFile"]).Enabled = true;
                        ((ToolStripMenuItem)itemsInFile["openFile"]).Enabled = true;
                        ((ToolStripMenuItem)itemsInFile["saveFile"]).Enabled = tabInfo.IsFileOpened;
                        //((ToolStripMenuItem)itemsInFile["saveFile"]).Enabled = tabInfo.IsFileOpened && !tabInfo.IsDataBase;
                        ((ToolStripMenuItem)itemsInFile["saveFileAs"]).Enabled = true;
                        ToolStripItemCollection itemsInDataBase = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["dataBase"]).DropDown.Items;
                        // database items
                        ToolStripItemCollection itemsInEdit = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["edit"]).DropDown.Items;
                        ((ToolStripMenuItem)itemsInEdit["refresh"]).Enabled = availability;
                        ((ToolStripMenuItem)itemsInEdit["clear"]).Enabled = availability;
                        ToolStripItemCollection itemsInView = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["view"]).DropDown.Items;
                        ((ToolStripMenuItem)itemsInView["hideEmptyRows"]).Enabled = true;
                        ((ToolStripMenuItem)itemsInView["hideEmptyRows"]).Checked = tabInfo.IsHideEmptyRows;
                        ((ToolStripMenuItem)itemsInView["showAsTable"]).Enabled = true;
                        ((ToolStripMenuItem)itemsInView["showAsTable"]).Checked = tabInfo.IsShowAsTable;
                        ((ToolStripMenuItem)itemsInView["stretchCells"]).Enabled = true;
                        ((ToolStripMenuItem)itemsInView["stretchCells"]).Checked = tabInfo.IsStretchCells;

                        QickActionsMenu_ToolStrip.Items["increaseColumnCount_Button"].Visible = tabInfo.IsShowAsTable;
                        QickActionsMenu_ToolStrip.Items["decreaseColumnCount_Button"].Visible = tabInfo.IsShowAsTable;
                        QickActionsMenu_ToolStrip.Items["columnCountBox_TextBox"].Enabled = tabInfo.IsShowAsTable;
                        ToolStripMenuItem hideEmptyRows = (ToolStripMenuItem)itemsInView["hideEmptyRows"];
                        hideEmptyRows.Checked = false;
                        hideEmptyRows.Enabled = tabInfo.IsShowAsTable;
                        tabInfo.IsHideEmptyRows = hideEmptyRows.Checked;
                    }
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
            /// <summary>
            /// Изменение информации об открытой вкладке
            /// </summary>
            /// <param name="tabInfo">Ссылка на экземпляр TabInfo</param>
            public static void SetStatusBarInfoLabel(TabInfo tabInfo)
            {
                try
                {
                    if (tabInfo.IsShowAsTable)
                    {
                        tabInfo.RowCount = tabInfo.DataGridView.RowCount - 1;
                        TabInfo_ToolStripLabel.Text = $"Столбцов : {tabInfo.ColumnCount}   Строк : {tabInfo.RowCount}";
                    }
                    else
                    {
                        tabInfo.RowCount = tabInfo.TextBox.Lines.Length;
                        TabInfo_ToolStripLabel.Text = $"Строк : {tabInfo.TextBox.Lines.Length}";
                    }
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
        }
        #endregion

        /// <summary>
        /// Заполнение ImageList в TabControl
        /// </summary>
        private void AddImageListToTabs()
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                ImageList imageList = new();
                imageList.Images.Add(new System.Drawing.Bitmap(Properties.Resources.csv));
                imageList.Images.Add(new System.Drawing.Bitmap(Properties.Resources.database));
                imageList.Images.Add(new System.Drawing.Bitmap(Properties.Resources.file));
                Files_TabControl.ImageList = imageList;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }

        /// <summary>
        /// Заполнение верхнего меню программы
        /// </summary>
        private void FillActionsBar()
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                ToolStripMenuItem file = new("Файл") { Name = "file" };
                {
                    ContextMenuStrip fileContextMenu = new();

                    ToolStripMenuItem createFile = new("Создать") { Name = "createFile", Checked = false, ShowShortcutKeys = true, ShortcutKeys = Keys.Control | Keys.N };
                    createFile.Click += Handlers.CreateFile_Click;
                    ToolStripMenuItem openFile = new("Открыть") { Name = "openFile", Checked = false, ShowShortcutKeys = true, ShortcutKeys = Keys.Control | Keys.O };
                    openFile.Click += Handlers.OpenFile_Click;
                    ToolStripMenuItem saveFile = new("Сохранить") { Name = "saveFile", Checked = false, ShowShortcutKeys = true, Enabled = false, ShortcutKeys = Keys.Control | Keys.S };
                    saveFile.Click += Handlers.SaveFile_Click;
                    ToolStripMenuItem saveFileAs = new("Сохранить как") { Name = "saveFileAs", Checked = false, ShowShortcutKeys = true, Enabled = false, ShortcutKeys = Keys.Control | Keys.Shift | Keys.S };
                    saveFileAs.Click += Handlers.SaveFileAs_Click;

                    fileContextMenu.Items.AddRange(new[] { createFile, openFile, saveFile, saveFileAs });

                    file.DropDown = fileContextMenu;
                    ((ContextMenuStrip)file.DropDown).ShowImageMargin = false;
                    ((ContextMenuStrip)file.DropDown).ShowCheckMargin = false;
                }

                ToolStripMenuItem dataBase = new("База данных") { Name = "dataBase" };
                {
                    ContextMenuStrip dataBaseContextMenu = new();

                    // 

                    dataBase.DropDown = dataBaseContextMenu;
                    ((ContextMenuStrip)dataBase.DropDown).ShowImageMargin = false;
                    ((ContextMenuStrip)dataBase.DropDown).ShowCheckMargin = false;
                }

                ToolStripMenuItem edit = new("Изменить") { Name = "edit" };
                {
                    ContextMenuStrip editContextMenu = new();

                    ToolStripMenuItem refresh = new("Обновить") { Name = "refresh", Checked = false, Enabled = false, ShowShortcutKeys = true, ShortcutKeys = Keys.F5 };
                    refresh.Click += Handlers.Refresh_Click;
                    ToolStripMenuItem clear = new("Очистить") { Name = "clear", Checked = false, Enabled = false, ShowShortcutKeys = true, ShortcutKeys = Keys.Control | Keys.Delete };
                    clear.Click += Handlers.Clear_Click;

                    editContextMenu.Items.AddRange(new[] { refresh, clear });

                    edit.DropDown = editContextMenu;
                    ((ContextMenuStrip)edit.DropDown).ShowImageMargin = false;
                    ((ContextMenuStrip)edit.DropDown).ShowCheckMargin = false;
                }

                ToolStripMenuItem view = new("Просмотр") { Name = "view" };
                {
                    ContextMenuStrip viewContextMenu = new();

                    ToolStripMenuItem showStatusBar = new("Отображение строки состояния") { Name = "showStatusBar", Checked = StatusBar.Visible, CheckOnClick = true, ShowShortcutKeys = true, ShortcutKeys = Keys.Control | Keys.I };
                    showStatusBar.Click += Handlers.ShowStatusBar_Click;

                    ToolStripMenuItem hideEmptyRows = new("Сокрытие пустых строк") { Name = "hideEmptyRows", Checked = false, CheckOnClick = true, ShowShortcutKeys = true, Enabled = false, ShortcutKeys = Keys.Control | Keys.H };
                    hideEmptyRows.Click += Handlers.HideEmptyRows_Click;
                    ToolStripMenuItem showAsTable = new("Отображение в виде таблицы") { Name = "showAsTable", Checked = false, CheckOnClick = true, ShowShortcutKeys = true, Enabled = false, ShortcutKeys = Keys.Control | Keys.T };
                    showAsTable.Click += Handlers.ShowAsTable_Click;
                    ToolStripMenuItem stretchCells = new("Выравнивание по ширине") { Name = "stretchCells", Checked = false, CheckOnClick = true, ShowShortcutKeys = true, Enabled = false, ShortcutKeys = Keys.Control | Keys.G };
                    stretchCells.Click += Handlers.StretchCells_Click;

                    viewContextMenu.Items.AddRange(new[] { showStatusBar, hideEmptyRows, showAsTable, stretchCells });

                    view.DropDown = viewContextMenu;
                    ((ContextMenuStrip)view.DropDown).ShowImageMargin = false;
                    ((ContextMenuStrip)view.DropDown).ShowCheckMargin = true;
                }
                ProgramActionsBar.Items.AddRange(new[] { file, dataBase, edit, view });

                /* Получение доступа к элементам выпадающего меню
                ToolStripItemCollection itemsInView = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["file"]).DropDown.Items;
                ToolStripMenuItem showStatusBar = (ToolStripMenuItem)itemsInView["showStatusBar"];
                ToolStripMenuItem hideEmptyRows = (ToolStripMenuItem)itemsInView["hideEmptyRows"];
                 */
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }

        /// <summary>
        /// Добавление обработчиков на элементы формы
        /// </summary>
        private void AddEventHandlersToFormComponents()
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                // BUTTON
                increaseColumnCount_Button.Click += Handlers.IncreaseButton_Click;
                decreaseColumnCount_Button.Click += Handlers.DecreaseButton_Click;

                // TEXTBOX
                columnCountBox_TextBox.TextChanged += Handlers.ColumnCountBox_TextChanged;

                // TAB CONTROL
                Files_TabControl.SelectedIndexChanged += Handlers.TabControl_SelectedIndexChanged;
                Files_TabControl.MouseLeave += Handlers.TabControl_MouseLeave;
                Files_TabControl.MouseMove += Handlers.TabControl_MouseMove;
                Files_TabControl.MouseDown += Handlers.TabControl_MouseDown;

                // FORM
                FormClosing += Handlers.Form_FormClosing;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
    }
}