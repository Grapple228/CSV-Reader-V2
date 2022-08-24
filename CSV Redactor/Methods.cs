using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Xml.Linq;
using CSV_Redactor.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static CSV_Redactor.Main_Form;

namespace CSV_Redactor
{
    internal static class Methods
    {
        /// <summary>
        /// Открытие настроек программы
        /// </summary>
        /// </summary>
        public static void ShowSettingsDialog()
        {
            TraceCalls(MethodBase.GetCurrentMethod());
        }
        public static void SetValueToSettings(XDocument doc, string valuePath, object value)
        {
            TraceCalls(MethodBase.GetCurrentMethod());
        }
        /// <summary>
        /// Загрузить файл настроек
        /// </summary>
        /// <param name="baseDirectory">Путь до директории программы</param>
        /// <returns>Файл настроек</returns>
        public static SettingsClass LoadSettingsFile(string baseDirectory)
        {
            XDocument doc;
            TraceCalls(MethodBase.GetCurrentMethod(), new object[] { baseDirectory });
            try
            {
                doc = XDocument.Load(
                    Path.Combine(baseDirectory, @"Settings.xml")
                    );
            }
            catch (FileNotFoundException) { doc = GenerateSettingsFile(); doc.Save(Path.Combine(baseDirectory, @"Settings.xml")); }

            return new(doc);

            static XDocument GenerateSettingsFile()
            {
                TraceCalls(MethodBase.GetCurrentMethod());

                XDocument doc = new(
                    new XElement("settings",
                        new XElement("global",
                                new XComment("Окно"),
                                new XElement("MinimumWindowWidth", 800),
                                new XElement("MinimumWindowHeight", 600),
                                 new XComment("Интерфейс"),
                                new XElement("IsShowStatusBar", true),
                                 new XComment("Таблица"),
                                new XElement("DefaultColumnName", "Column"),
                                 new XComment("Вкладка"),
                                new XElement("DefaultFileName", "New File"),
                                new XElement("DefaultTableName", "New Table"),
                                 new XComment("Дополнительно"),
                                new XElement("DefaultFileExtension", ".csv"), 
                                new XElement("SupportedSeparators", ";,."),
                                 new XComment("Отладка"),
                                new XElement("IsTracingEnabled", false)
                            ),
                        new XElement("local",
                                new XElement(
                              "text",
                              new XElement("DefaultColumnCount", 1),
                              new XElement("IsShowAsTable", true),
                              new XElement("IsStretchCells", true),
                              new XElement("IsHideEmptyRows", false)
                              ),
                                new XElement(
                              "database",
                              new XElement("SQLServer"),
                              new XElement("SQLite"),
                              new XElement("MySQL")
                              )
                            )
                        )
                    );
                return doc;
            }
        }
        /// <summary>
        /// Получить значение из xml-файла
        /// </summary>
        /// <param name="xmlFile">xml-файл</param>
        /// <param name="neededFieldPath">Строка вида element - ...-element</param>
        /// <returns>Поле</returns>
        public static XElement GetFieldsFromSettings(XDocument xmlFile, string neededFieldPath)
        {
            XElement element = xmlFile.Element("settings");
            try
            {
                string[] path = neededFieldPath.Split('-');

                for (int i = 0; i < path.Length; i++)
                {
                    element = element.Element(path[i]);
                }
            }
            catch (Exception ex) { ExceptionProcessing(ex); }
            return element;
        }
        /// <summary>
        /// Выведение сообщения об ошибке
        /// </summary>
        /// <param name="ex">Ошибка</param>
        public static void ExceptionProcessing(Exception ex)
        {
            TraceCalls(MethodBase.GetCurrentMethod());
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (!Directory.Exists(BaseDirectory + "/Tracebacks")) Directory.CreateDirectory(BaseDirectory + "/Tracebacks");
            string traceBackPath = (BaseDirectory + "/Tracebacks/" + $"Traceback {DateTime.Now:yyyy.MM.dd HH-mm-ss.fff}.log").Replace("\\", "/");
            string exceptionMessage = $"Возникла ошибка!\n" +
                $"{ex.GetType().FullName}: " +
                $"{ex.Message}\n" +
                $"{ex.StackTrace}\n" +
                $"{ex.InnerException}\n";

            File.WriteAllText(traceBackPath, $"{TraceErrorListenerText} {exceptionMessage}");
            TraceErrorListenerText += exceptionMessage;
        }
        /// <summary>
        /// Выведение окна с подтверждением операции
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns>Dialog result</returns>
        public static DialogResult ShowConfirmationMessage(string message = null)
        {
            TraceCalls(MethodBase.GetCurrentMethod());
            DialogResult result = DialogResult.Cancel;
            try
            {
                message ??= "Вы не сохранили изменения в файле \"" + $"{OldTabInfo.TabsInfo.Find(tab => tab.FullTabName == OldTabInfo.CloseTabButton.Name).ShortTabName}" + "\"\nОни будут утеряны! Продолжить выполнение?";
                result = MessageBox.Show(message, "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            }
            catch (Exception ex) { ExceptionProcessing(ex); }

            return result;
        }
        /// <summary>
        /// Считывание данных из RichTextBox
        /// </summary>
        /// <param name="textBox">Ссылка на экземпляр RichTextBox</param>
        /// <returns>Считанные данные</returns>
        public static List<object> ReadData(string text)
        {
            List<object> data = new();
            TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = OldTabInfo.TabsInfo.Find(tab => tab.FullTabName == OldTabInfo.Files_TabControl.SelectedTab.Name);

                List<string> Lines = new();

                int maxSeparatorCount = 0;

                using (StringReader reader = new(text))
                {
                    string line;
                    char separator = tabInfo.Separator;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Lines.Add(line.Trim());

                        int separatorCount = line.Split(separator).Length - 1;

                        if (separatorCount > maxSeparatorCount)
                            maxSeparatorCount = separatorCount;
                    }
                }

                int rowCount = 0,
                    columnCount = 0;
                foreach (string line in Lines)
                {
                    string[] splittedLine = line.Split(tabInfo.Separator, '\n');
                    int separatorsNeedToAdd = 0;

                    if (splittedLine.Count() - 1 != maxSeparatorCount)
                    {
                        separatorsNeedToAdd = maxSeparatorCount - (splittedLine.Count() - 1);
                    }
                    if (rowCount == 0)
                    {
                        List<string> headers = new();
                        headers.AddRange(splittedLine);
                        for (int i = 0; i < separatorsNeedToAdd; i++)
                            headers.Add("");

                        splittedLine = headers.ToArray();
                        for (int i = 0; i < splittedLine.Length; i++)
                            splittedLine[i] = splittedLine[i].Trim();

                        //data.AddRange(GenColumnNames(splittedLine));
                        rowCount++;
                        continue;
                    }

                    foreach (string value in splittedLine)
                        data.Add(value.Trim());

                    for (int i = 0; i < separatorsNeedToAdd; i++)
                        data.Add(null);
                    rowCount++;
                }

                columnCount = maxSeparatorCount + 1;
                // Удаление пустых строк в конце
                bool isBreak = false;
                for (int i = rowCount - 1; i > -1; i--)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        string cell = "" + data[i * columnCount + j];
                        if (cell.Trim().Replace(" ", "").Replace("\u00A0", "") == "") continue;
                        else { isBreak = true; break; }
                    }
                    if (isBreak) break;
                    data.RemoveRange(i * columnCount, columnCount);
                    rowCount--;
                }

                tabInfo.ColumnCount = columnCount;
                OldTabInfo.ColumnCount_TextBox.Text = columnCount.ToString();
                tabInfo.DataGridView.ColumnCount = tabInfo.ColumnCount;

                tabInfo.RowCount = rowCount;
                //OldTabInfo.SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { ExceptionProcessing(ex); }

            return data;
        }
        /// <summary>
        /// Считывание данных из DataGridView
        /// </summary>
        /// <param name="dataGridView">Ссылка на экземпляр DataGridView</param>
        /// <param name="columnCount">Количество считываемых колонок</param>
        /// <returns>Считанные данные</returns>
        public static List<object> ReadData(DataGridView dataGridView, int columnCount)
        {
            List<object> data = new();
            TraceCalls(MethodBase.GetCurrentMethod(), new object[] { columnCount });
            try
            {
                OldTabInfo tabInfo = OldTabInfo.TabsInfo.Find(tab => tab.FullTabName == OldTabInfo.Files_TabControl.SelectedTab.Name);
                if (columnCount > dataGridView.ColumnCount)
                    columnCount = dataGridView.ColumnCount;
                int rowCount = 0;

                string[] names = new string[dataGridView.ColumnCount];
                for (int i = 0; i < dataGridView.ColumnCount; i++)
                {
                    names[i] = dataGridView.Columns[i].HeaderText.Trim();
                }
                //data.AddRange(GenColumnNames(names));
                for (int i = 0; i < dataGridView.RowCount; i++)
                {
                    if (dataGridView.Rows[i].IsNewRow) break;
                    for (int j = 0; j < columnCount; j++)
                    {
                        data.Add(dataGridView.Rows[i].Cells[j].Value);
                    }
                    rowCount++;
                }

                // Удаление пустых строк в конце
                bool isBreak = false;
                for (int i = rowCount - 1; i > -1; i--)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        string cell = "" + data[i * columnCount + j];
                        if (cell.Trim().Replace(" ","").Replace("\u00A0", "") == "") continue;
                        else { isBreak = true; break; }
                    }
                    if (isBreak) break;
                    data.RemoveRange(i * columnCount, columnCount);
                    rowCount--;
                }

                tabInfo.ColumnCount = dataGridView.ColumnCount;
                tabInfo.RowCount = rowCount;

                //OldTabInfo.SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { ExceptionProcessing(ex); }

            return data;
        }
        /// <summary>
        /// Запись данных в RichTextBox
        /// </summary>
        /// <param name="textBox">Ссылка на экземпляр RichTextBox</param>
        /// <param name="data">Данные</param>
        /// <param name="columnCount">Количество столбцов</param>
        public static void WriteData(RichTextBox textBox, List<object> data, int columnCount)
        {
            try
            {
                OldTabInfo tabInfo = OldTabInfo.TabsInfo.Find(tab => tab.FullTabName == OldTabInfo.Files_TabControl.SelectedTab.Name);

                object[] formattedData = data.ToArray();
                List<int> separatorIndexes = new();

                List<object> list = new();
                for (int i = 0; i < tabInfo.ColumnCount; i++)
                    list.Add(formattedData[i]);
                list.Add(formattedData.Length);
                TraceCalls(MethodBase.GetCurrentMethod(), new object[] { list, columnCount });

                int dataCount,
                    rowCount;

                dataCount = formattedData.Length;

                columnCount = tabInfo.IsFixed ? tabInfo.FixedColumnCount : columnCount;

                if (tabInfo.ColumnCount < columnCount) columnCount = tabInfo.ColumnCount;

                if (dataCount % columnCount == 0)
                    rowCount = dataCount / columnCount;
                else
                    rowCount = (dataCount / columnCount) + 1;
                tabInfo.RowCount = rowCount;

                textBox.Text = AddRows(rowCount, columnCount, formattedData, tabInfo);
                //OldTabInfo.SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { ExceptionProcessing(ex); }
            static string AddRows(int rowCount, int columnCount, object[] formattedData, OldTabInfo tabInfo)
            {
                List<object> list = new();
                for (int i = 0; i < columnCount; i++)
                    list.Add(formattedData[i]);
                list.Add(formattedData.Length);
                TraceCalls(MethodBase.GetCurrentMethod(), new object[] { rowCount, columnCount, list, tabInfo });

                // Определение отступов
                int[] maxLengths = new int[columnCount];

                for (int i = 0; i < columnCount; i++)
                {
                    int maxLength = 0;
                    for (int j = 0; j < rowCount; j++)
                    {
                        var value = formattedData[j * columnCount + i];
                        string sValue = value == null || value.ToString().Replace(" ", "").Replace("\u00A0", "") == "" ? "\u00A0" : value.ToString().Trim();
                        if (sValue.Length > maxLength)
                            maxLength = sValue.Length;
                    }
                    maxLengths[i] = maxLength;
                }

                string[] LinesArr = new string[rowCount];
                for (int i = 0; i < rowCount; i++)
                {
                    string[] line = new string[columnCount];
                    for (int j = 0; j < columnCount; j++)
                    {
                        var value = formattedData[i * columnCount + j];

                        if (tabInfo.IsStretchCells)
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
                    LinesArr[i] = string.Join($"{tabInfo.Separator} ", line);
                }
                return string.Join("\n", LinesArr);
            }
        }
        /// <summary>
        /// Запись данных в DataGridView
        /// </summary>
        /// <param name="dataGridView">Ссылка на экземпляр DataGridView</param>
        /// <param name="data">Данные</param>
        /// <param name="columnCount">Количество записываемых столбцов</param>
        public static void WriteData(DataGridView dataGridView, List<object> data, int columnCount)
        {
            try
            {
                OldTabInfo tabInfo = OldTabInfo.TabsInfo.Find(tab => tab.FullTabName == OldTabInfo.Files_TabControl.SelectedTab.Name);
                object[] formattedData = data.ToArray();

                List<object> list = new();
                for (int i = 0; i < tabInfo.ColumnCount; i++)
                    list.Add(formattedData[i]);
                list.Add(formattedData.Length);
                TraceCalls(MethodBase.GetCurrentMethod(), new object[] { list, columnCount });

                int dataCount,
                    rowCount,
                    rowIndex,
                    dataLeft,
                    dataIndex;

                dataGridView.Rows.Clear();
                dataGridView.ColumnCount = columnCount;

                dataCount = formattedData.Length - columnCount;

                dataLeft = dataCount;

                columnCount = tabInfo.IsFixed ? tabInfo.FixedColumnCount : columnCount;

                if (tabInfo.ColumnCount < columnCount) columnCount = tabInfo.ColumnCount;

                if (dataCount % columnCount == 0)
                    rowCount = dataCount / columnCount;
                else
                    rowCount = (dataCount / columnCount) + 1;
                tabInfo.RowCount = rowCount;
                dataIndex = 0 + columnCount;
                rowIndex = 0;

                for (int i = 0; i < columnCount; i++)
                {
                    dataGridView.Columns[i].HeaderText = tabInfo.Data[i].ToString();
                }

                int maxColumnNumber,
                        tempIndex;

                if (tabInfo.IsHideEmptyRows) AddWithoutEmptyRows();
                else AddWithEmptyRows();
                tabInfo.RowCount = rowCount;

                //OldTabInfo.SetStatusBarInfoLabel(tabInfo);

                void AddWithEmptyRows()
                {
                    TraceCalls(MethodBase.GetCurrentMethod());
                    try
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            if (dataLeft > columnCount) { dataLeft -= columnCount; maxColumnNumber = columnCount; }
                            else { maxColumnNumber = dataLeft; }

                            AddRowToDataGridView();
                        }

                        // Удаление пустых строк в конце
                        bool isBreak = false;
                        for (int i = rowCount - 1; i > -1; i--)
                        {
                            for (int j = 0; j < columnCount; j++)
                            {
                                string cell = "" + dataGridView[j, i].Value;
                                if (cell.Trim() == "") continue;
                                else { isBreak = true; break; }
                            }
                            if (isBreak) break;
                            dataGridView.Rows.RemoveAt(i);
                            rowCount--;
                        }
                    }
                    catch (Exception ex) { ExceptionProcessing(ex); }
                }
                void AddWithoutEmptyRows()
                {
                    TraceCalls(MethodBase.GetCurrentMethod());
                    try
                    {
                        int removedRowCount = 0;
                        for (int i = 0; i < rowCount; i++)
                        {
                            string nextRow = "";
                            tempIndex = dataIndex;

                            if (dataLeft > columnCount) { dataLeft -= columnCount; maxColumnNumber = columnCount; }
                            else { maxColumnNumber = dataLeft; }

                            for (int j = 0; j < maxColumnNumber; j++)
                            {
                                nextRow += formattedData[tempIndex];
                                tempIndex++;
                            }
                            var value = nextRow.Trim().Replace(" ", "").Replace("\u00A0", "");

                            // Запись в таблицу

                            if (value != "")
                            {
                                AddRowToDataGridView();
                            }
                            else
                            {
                                dataIndex = tempIndex;
                                removedRowCount++;
                            }
                        }
                        rowCount -= removedRowCount;
                    }
                    catch (Exception ex) { ExceptionProcessing(ex); }
                }
                void AddRowToDataGridView()
                {
                    try
                    {
                        dataGridView.Rows.Add();

                        for (int j = 0; j < maxColumnNumber; j++)
                        {
                            dataGridView.Rows[rowIndex].Cells[j].Value = formattedData[dataIndex];
                            dataIndex++;
                        }
                        rowIndex++;
                    }
                    catch (Exception ex) { ExceptionProcessing(ex); }
                }
            }
            catch (Exception ex) { ExceptionProcessing(ex); }

        }
        /// <summary>
        /// Логгирование программы
        /// </summary>
        /// <param name="method">Логируемый метод</param>
        /// <param name="variables">Переменные метода</param>
        public static void TraceCalls(MethodBase method, params object[] variables)
        {
            string variablesText = "";
            for (int i = 0; i < variables.Length; i++)
            {
                string type = variables[i].GetType().ToString();

                if (type.Contains("[]"))
                {
                    dynamic[] arrayContent = type.Contains("String[]") ? (string[])variables[i] : (object[])variables[i];
                    string arrayText = string.Join(", ", arrayContent);
                    variablesText += $"[{(arrayText == "" ? "Empty" : arrayText)}]";
                }
                else if (type.Contains("List"))
                {
                    variablesText += $"[{string.Join(", ", (List<object>)variables[i])}]";
                }
                else
                {
                    variablesText += variables[i].ToString();
                }

                if (i < variables.Length - 1)
                    variablesText += ", ";
            }
            string message = $"{DateTime.Now:yyyy.MM.dd HH:mm:ss.fff} {method.Name}\n" +
                $"{method}\n";

            if (variablesText.Replace(" ", "") != "")
                message += $"{variablesText}\n";

            if (IsTraceCallsEnabled)
                Debug.WriteLine(message);

            TraceErrorListenerText += $"{message}\n";
            //Methods.TraceCalls(MethodBase.GetCurrentMethod());
        }
        public static ImageSource ToImageSource(this Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
    }
}