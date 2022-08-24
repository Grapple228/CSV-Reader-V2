using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CSV_Redactor.TabInfoFolder.Interfaces;
using Extensions;

namespace CSV_Redactor.TabInfoFolder.Classes
{
    class TextFile : ITab, IDefaultFieldsOfTabs
    {
        public TextFile()
        {
            FillFields();
        }
        protected void FillFields()
        {
            ColumnCount = Main_Form.Settings.DefaultColumnCount;
            FixedColumnCount = ColumnCount;
            Separator = Tab.Separators[0];
            Data = new();
            DataGridView = Tab.CreateDataGridView();
            TextBox = Tab.CreateTextBox();

            IsChanged = false;
            IsHideEmptyRows = Main_Form.Settings.IsHideEmptyRows;
            IsShowAsTable = Main_Form.Settings.IsShowAsTable;
            IsStretchCells = Main_Form.Settings.IsStretchCells;
            IsFixed = false;

            Extension = Main_Form.Settings.DefaultFileExtension;
        }

        public string Extension { get; set; }

        // ITab
        public string PartialName { get; set; }
        public string FullName { get; set; }
        public TabPage TabPage { get; protected set; }

        // IDefaultFieldsOfTabs
        public DataGridView DataGridView { get; protected set; }
        public RichTextBox TextBox { get; protected set; }
        public List<string> Data { get; set; }
        public int ColumnCount { get; set; }
        public int FixedColumnCount { get; set; }
        public int RowCount { get; set; }
        public char Separator { get; set; }
        public bool IsChanged { get; set; }
        public bool IsHideEmptyRows { get; set; }
        public bool IsShowAsTable { get; set; }
        public bool IsStretchCells { get; set; }
        public bool IsFixed { get; set; }

        public virtual void CreateNewTab()
        {
            string partialTabName = "";
            if (Tab.ShowCreationNewFileWindow(ref partialTabName) != DialogResult.OK) return;

            partialTabName = partialTabName.Replace(" ", "") == "" ?
                Main_Form.Settings.DefaultFileExtension + Extension :
                partialTabName.Trim() + Extension;

            PartialName = partialTabName;
            FullName = Tab.CreateUniqueName(partialTabName);
            DataGridView.Name = FullName + "_DataGridView";
            TextBox.Name = FullName + "_TextBox";

            int imageIndex = 0;
            switch (Extension)
            {
                case ".csv": imageIndex = 0; break;
                case ".txt": imageIndex = 1; break;
            }

            TabPage = new(PartialName)
            {
                Name = FullName + "_Tab",
                ImageIndex = imageIndex
            };

            TabPage.Controls.Add(DataGridView);
            TabPage.Controls.Add(TextBox);

            Data.AddRange(Tab.GenColumnNames(new string[ColumnCount]));
            if (IsShowAsTable)
            {
                DataGridView.Visible = true;
                DataGridView.ColumnCount = ColumnCount;
                RowCount = 0;
                int counter = 0;

                foreach (DataGridViewColumn column in DataGridView.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    column.HeaderText = Data[counter];
                    counter++;
                }
                Tab.ChangeStretchDataGridView(DataGridView, IsStretchCells);
            }
            else
            {
                TextBox.Visible = true;
                TextBox.Text = string.Join($"{Separator} ", Data);
                RowCount = 1;
            }

            Tab.Tabs.Add(this);
            Tab.Main_TabControl.TabPages.Add(TabPage);
            Tab.Main_TabControl.SelectedTab = TabPage;
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
        public void SaveFileAs(string fileName = null)
        {
            SaveFileDialog sfd = new()
            {
                AddExtension = true,
                DefaultExt = Main_Form.Settings.DefaultFileExtension,
                FileName = fileName == null ? Main_Form.Settings.DefaultFileName : fileName,
                Filter = string.Join("|", new string[] { Tab.TextFilter, Tab.AllFilter }),
                OverwritePrompt = true,
                Title = "Выбор места сохранения файла"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            string filePath = sfd.FileName;
            WriteData(filePath);
        }
        public List<string> ReadData(string filePath = null)
        {
            List<string> data = new();
            if (filePath == null)
                if (IsShowAsTable) ReadFromDataGridView();
                else
                {
                    ReadText(ReadDataFromString(TextBox.Text));
                }
            else
                ReadText(
                    ReadDataFromString(
                        File.ReadAllText(filePath, Encoding.UTF8)));

            return data;

            void ReadFromDataGridView()
            {
                if (ColumnCount > DataGridView.ColumnCount)
                    ColumnCount = DataGridView.ColumnCount;

                string[] names = new string[DataGridView.ColumnCount];
                for (int i = 0; i < DataGridView.ColumnCount; i++)
                    names[i] = DataGridView.Columns[i].HeaderText.Trim();

                for (int i = 0; i < DataGridView.RowCount; i++)
                {
                    if (DataGridView.Rows[i].IsNewRow) break;
                    for (int j = 0; j < ColumnCount; j++)
                        data.Add(DataGridView.Rows[i].Cells[j].Value.ToString());
                }
                RowCount = DataGridView.RowCount - 1;

                // Удаление пустых строк в конце
                bool isBreak = false;
                for (int i = RowCount - 1; i > -1; i--)
                {
                    for (int j = 0; j < ColumnCount; j++)
                    {
                        string cell = "" + data[i * ColumnCount + j];
                        if (cell.Trim().Replace(" ", "").Replace("\u00A0", "") == "") continue;
                        else { isBreak = true; break; }
                    }
                    if (isBreak) break;
                    data.RemoveRange(i * ColumnCount, ColumnCount);
                    RowCount--;
                }
            }
            void ReadText(List<string> Lines)
            {
                RowCount = 0;
                foreach (string line in Lines)
                {
                    string[] splittedLine = line.Split(Separator, '\n');
                    int separatorsNeedToAdd = 0;

                    if (splittedLine.Count() - 1 != ColumnCount - 1)
                        separatorsNeedToAdd = ColumnCount - 1 - (splittedLine.Count() - 1);
                    if (RowCount == 0)
                    {
                        List<string> headers = new();
                        headers.AddRange(splittedLine);
                        for (int i = 0; i < separatorsNeedToAdd; i++)
                            headers.Add("");

                        splittedLine = headers.ToArray();
                        for (int i = 0; i < splittedLine.Length; i++)
                            splittedLine[i] = splittedLine[i].Trim();
                        data.AddRange(Tab.GenColumnNames(splittedLine));
                        RowCount++;
                        continue;
                    }

                    foreach (string value in splittedLine)
                        data.Add(value.Trim());

                    for (int i = 0; i < separatorsNeedToAdd; i++)
                        data.Add(null);
                    RowCount++;
                }

                // Удаление пустых строк в конце
                bool isBreak = false;
                for (int i = RowCount - 1; i > -1; i--)
                {
                    for (int j = 0; j < ColumnCount; j++)
                    {
                        string cell = "" + data[i * ColumnCount + j];
                        if (cell.Trim().Replace(" ", "").Replace("\u00A0", "") == "") continue;
                        else { isBreak = true; break; }
                    }
                    if (isBreak) break;
                    data.RemoveRange(i * ColumnCount, ColumnCount);
                    RowCount--;
                }
            }
        }
        protected List<string> ReadDataFromString(string text)
        {
            List<string> data = new();
            int maxSeparatorCount = 0;
            using (StringReader reader = new(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    data.Add(line.Trim());
                    int separatorCount = line.Split(Separator).Length - 1;
                    if (separatorCount > maxSeparatorCount)
                        maxSeparatorCount = separatorCount;
                }
            }
            ColumnCount = maxSeparatorCount + 1;
            return data;
        }
        public void WriteData(string filePath = null)
        {
            if (filePath == null)
            {
                if (IsShowAsTable) WriteToDataGridView();
                else TextBox.Text = WriteText();
                Tab.ColumnCount_TextBox.Text = ColumnCount.ToString();
                Tab.SetStatusBarInfoLabel(this);
            }
            else
            {
                Data = ReadData();
                File.WriteAllText(filePath, WriteText(), Encoding.UTF8);
            }

            string WriteText()
            {
                object[] formattedData = Data.ToArray();

                List<object> list = new();
                for (int i = 0; i < ColumnCount; i++)
                    list.Add(formattedData[i]);
                list.Add(formattedData.Length);
                Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { list, ColumnCount });

                int dataCount,
                    rowCount;

                dataCount = formattedData.Length;

                ColumnCount = IsFixed ? FixedColumnCount : ColumnCount;

                if (dataCount % ColumnCount == 0)
                    rowCount = dataCount / ColumnCount;
                else
                    rowCount = (dataCount / ColumnCount) + 1;
                RowCount = rowCount;

                return AddRows();
                string AddRows()
                {
                    List<object> list = new();
                    for (int i = 0; i < ColumnCount; i++)
                        list.Add(formattedData[i]);
                    list.Add(formattedData.Length);
                    Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { RowCount, ColumnCount, list });

                    // Определение отступов
                    int[] maxLengths = new int[ColumnCount];

                    for (int i = 0; i < ColumnCount; i++)
                    {
                        int maxLength = 0;
                        for (int j = 0; j < rowCount; j++)
                        {
                            var value = formattedData[j * ColumnCount + i];
                            string sValue = value == null || value.ToString().Replace(" ", "").Replace("\u00A0", "") == "" ? "\u00A0" : value.ToString().Trim();
                            if (sValue.Length > maxLength)
                                maxLength = sValue.Length;
                        }
                        maxLengths[i] = maxLength;
                    }

                    string[] LinesArr = new string[rowCount];
                    for (int i = 0; i < rowCount; i++)
                    {
                        string[] line = new string[ColumnCount];
                        for (int j = 0; j < ColumnCount; j++)
                        {
                            var value = formattedData[i * ColumnCount + j];

                            if (IsStretchCells)
                            {
                                line[j] = value == null || value.ToString().Replace(" ", "").Replace(" ", "") == "" ? new string(' ', maxLengths[j]).Replace(" ", "\u00A0") : value.ToString();

                                // Добавление отступов между столбцами
                                if (j != 0) // Если не первый столбец
                                {
                                    int maxLength = maxLengths[j - 1];
                                    int prevLength = line[j - 1].Trim().Length;
                                    if (prevLength == 0) prevLength = maxLength;
                                    int indentCount;
                                    indentCount = maxLength - prevLength;

                                    if (indentCount > 0) // Если нужно добавить отступ
                                    {
                                        var newValue = new string(' ', indentCount) + line[j];
                                        line[j] = newValue;
                                    }
                                }
                            }
                            else
                            {
                                line[j] = value == null || value.ToString().Replace(" ", "").Replace(" ", "") == "" ? "\u00A0" : value.ToString();
                            }
                        }
                        LinesArr[i] = string.Join($"{Separator} ", line);
                    }
                    return string.Join("\n", LinesArr);
                }
            }       
            void WriteToDataGridView()
            {
                string[] formattedData = Data.ToArray();
                List<object> list = new();
                for (int i = 0; i < ColumnCount; i++)
                    list.Add(formattedData[i]);
                list.Add(formattedData.Length);
                Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { list, ColumnCount });

                string[][] arrays = Tab.TrimArray(formattedData, ColumnCount);

                DataGridView.Rows.Clear();
                DataGridView.ColumnCount = ColumnCount;
                RowCount = arrays.Length;

                for (int i = 0; i < ColumnCount; i++)
                {
                    DataGridView.Columns[i].HeaderText = arrays[0][i];
                }

                if (IsHideEmptyRows) AddWithoutEmptyRows();
                else AddWithEmptyRows();

                foreach (DataGridViewColumn column in DataGridView.Columns)
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;

                void AddWithEmptyRows()
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        DataGridView.Rows.Add(arrays[i]);
                    }
                }
                void AddWithoutEmptyRows()
                {
                    int hiddenRows = 0;
                    for (int i = 0; i < RowCount; i++)
                    {
                        bool isSkip = false;
                        for(int j = 0; j<ColumnCount; j++)
                        {
                            string value = "" + arrays[i][j];
                            if (value.Replace(" ", "").Replace("\u00A0", "") != "") break;
                            isSkip = true;
                        }
                        if (isSkip) { hiddenRows++; continue; }

                        DataGridView.Rows.Add(arrays[i]);
                    }
                    RowCount -= hiddenRows;
                }
            }
        }
    }
    class OpenedTextFile : TextFile, IFile
    {
        // IFile
        public string FilePath { get; set; }
        public void LoadData()
        {
            Data = ReadData(FilePath);
            WriteData();
        }
        public void SaveFile()
        {
            Data = ReadData();
            WriteData(FilePath);
        }

        public override void CreateNewTab()
        {
            OpenFileDialog ofd = new()
            {
                Title = "Открытие текстового файла",
                Filter = string.Join("|", new string[] { Tab.TextFilter, Tab.AllFilter })
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            FilePath = ofd.FileName;
            Extension = Path.GetExtension(FilePath).ToLower();
            int imageIndex;
            switch (Extension)
            {
                case ".csv": imageIndex = 0; break;
                case ".txt": imageIndex = 1; break;
                default:
                    MessageBox.Show($"Файл с расширением \"{Extension}\" не поддерживается!",
                        "Внимание!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
            }

            PartialName = ofd.SafeFileName;
            FullName = Tab.CreateUniqueName(PartialName);
            DataGridView.Name = FullName + "_DataGridView";
            TextBox.Name = FullName + "_TextBox";

            TabPage = new(PartialName)
            {
                Name = FullName + "_Tab",
                ImageIndex = imageIndex
            };

            TabPage.Controls.Add(DataGridView);
            TabPage.Controls.Add(TextBox);

            if (IsShowAsTable)
            {
                LoadData();
                DataGridView.Visible = true;
                int counter = 0;
                foreach (DataGridViewColumn column in DataGridView.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    column.HeaderText = Data[counter];
                    counter++;
                }
                Tab.ChangeStretchDataGridView(DataGridView, IsStretchCells);
            }
            else
            {
                TextBox.Visible = true;
                LoadData();
                RowCount = TextBox.Lines.Length;
            }
            Tab.Main_TabControl.TabPages.Add(TabPage);
            Tab.Main_TabControl.SelectedTab = TabPage;
            Tab.Tabs.Add(this);
            Tab.ChangeProgramMenuAvailability(this);
            Tab.SetStatusBarInfoLabel(this);
            Tab.ChangeFormElementsVisibility(true);
        }
    }
}
