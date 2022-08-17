using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static CSV_Redactor.Main_Form;
using static CSV_Redactor.Main_Form.TabInfo;

namespace CSV_Redactor
{
    /// <summary>
    /// Обработчики событий
    /// </summary>
    internal class Handlers
    {
        // Stopwatch sw = new();
        // sw.Restart();
        // sw.Stop();

        /// <summary>
        /// Переменная, предотвращающая открытие нескольких диалоговых окон
        /// </summary>
        static bool isDialogAlreadyOpened;

        #region Tab Components 

        #region Data Grid View
        public static void DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                ClickedRowIndex = e.RowIndex;
                ClickedColumnIndex = e.ColumnIndex;
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGrid = tabInfo.DataGridView;

                if (e.Button == MouseButtons.Right)
                {
                    if (e.ColumnIndex == -1 && e.RowIndex == -1) return;
                    if (e.RowIndex == -1)
                    {
                        ContextMenuOnHeaderClick.Items["Переместить влево"].Enabled = ClickedColumnIndex != 0;
                        ContextMenuOnHeaderClick.Items["Переместить вправо"].Enabled = ClickedColumnIndex != tabInfo.ColumnCount - 1;
                        ContextMenuOnHeaderClick.Items["Вставить слева"].Enabled = CopiedColumns != null;
                        ContextMenuOnHeaderClick.Items["Вставить справа"].Enabled = CopiedColumns != null;
                        ContextMenuOnHeaderClick.Items["Вставить в текущий"].Enabled = CopiedColumns != null;
                        ContextMenuOnHeaderClick.Show(Cursor.Position);
                    }
                    else if (!dataGrid.Rows[e.RowIndex].IsNewRow)
                    {
                        ContextMenuOnRowClick.Items["Переместить выше"].Enabled = ClickedRowIndex != 0;
                        ContextMenuOnRowClick.Items["Переместить ниже"].Enabled = ClickedRowIndex != tabInfo.RowCount - 1;
                        ContextMenuOnRowClick.Items["Вставить"].Enabled = CopiedRows != null;
                        ContextMenuOnRowClick.Items["Вставить в текущую"].Enabled = CopiedRows != null;
                        ContextMenuOnRowClick.Items["Очистить"].Enabled = !tabInfo.IsHideEmptyRows;
                        ContextMenuOnRowClick.Show(Cursor.Position);
                    }
                    else if (dataGrid.Rows[e.RowIndex].IsNewRow)
                    {
                        ContextMenuOnNewRowClick.Items["Вставить"].Enabled = CopiedRows != null;
                        ContextMenuOnNewRowClick.Show(Cursor.Position);
                    }
                }
                if (e.Button == MouseButtons.Left)
                {
                    if (e.RowIndex == -1)
                    {
                        if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Shift)
                        {
                            // Добавить текущую к выделенным
                            dataGrid.Columns[ClickedColumnIndex].Selected = true;
                        }
                        else
                        {
                            // Выделить только текущую
                            dataGrid.ClearSelection();
                            dataGrid.Columns[ClickedColumnIndex].Selected = true;
                        }
                    }
                }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;
                int neededCount = (e.RowIndex + 2) * dataGridView.ColumnCount;
                if (tabInfo.Data.Count < neededCount)
                {
                    int difference = neededCount - tabInfo.Data.Count;
                    tabInfo.Data.InsertRange(neededCount - difference, new object[difference]);
                }

                tabInfo.Data[(e.RowIndex + 1) * dataGridView.ColumnCount + e.ColumnIndex] = dataGridView[e.ColumnIndex, e.RowIndex].Value;

                tabInfo.IsChanged = true;
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void DataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                DataGridView dataGridView = (DataGridView)sender;
                for (int i = 0; i < dataGridView.ColumnCount; i++)
                {
                    e.Row.Cells[i].Value = null;
                }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void DataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                tabInfo.RowCount = tabInfo.DataGridView.RowCount - 1;
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region Rich Text Box
        public static void TextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Files_TabControl.SelectedIndex == -1 || Files_TabControl.SelectedTab == null) return;
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #endregion


        #region Context Menu

        #region ContextMenuOnHeaderClick
        public static void ChangeHeader_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;
                var clickedColumn = dataGridView.Columns[ClickedColumnIndex].HeaderText = (ClickedColumnIndex + 1).ToString();
                tabInfo.IsChanged = true;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void PinColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void MoveColumnToLeft_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                var clickedHeader = dataGridView.Columns[ClickedColumnIndex].HeaderCell;
                var header = dataGridView.Columns[ClickedColumnIndex - 1].HeaderCell;
                dataGridView.Columns[ClickedColumnIndex].HeaderCell = header;
                dataGridView.Columns[ClickedColumnIndex - 1].HeaderCell = clickedHeader;

                for (int i = 0; i < tabInfo.RowCount; i++)
                {
                    var cell1 = dataGridView.Rows[i].Cells[ClickedColumnIndex].Value;
                    var cell2 = dataGridView.Rows[i].Cells[ClickedColumnIndex - 1].Value;
                    dataGridView.Rows[i].Cells[ClickedColumnIndex].Value = cell2;
                    dataGridView.Rows[i].Cells[ClickedColumnIndex - 1].Value = cell1;
                }
                tabInfo.IsChanged = true;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void MoveColumnToRight_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                var clickedHeader = dataGridView.Columns[ClickedColumnIndex].HeaderCell;
                var header = dataGridView.Columns[ClickedColumnIndex + 1].HeaderCell;
                dataGridView.Columns[ClickedColumnIndex].HeaderCell = header;
                dataGridView.Columns[ClickedColumnIndex + 1].HeaderCell = clickedHeader;

                for (int i = 0; i < tabInfo.RowCount; i++)
                {
                    var cell1 = dataGridView.Rows[i].Cells[ClickedColumnIndex].Value;
                    var cell2 = dataGridView.Rows[i].Cells[ClickedColumnIndex + 1].Value;
                    dataGridView.Rows[i].Cells[ClickedColumnIndex].Value = cell2;
                    dataGridView.Rows[i].Cells[ClickedColumnIndex + 1].Value = cell1;
                }
                tabInfo.IsChanged = true;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void AddColumnToLeft_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                int selectedColumnCount = dataGridView.SelectedColumns.Count;
                int rowCount = tabInfo.RowCount;

                if (selectedColumnCount == 0 || !dataGridView.Columns[ClickedColumnIndex].Selected)
                {
                    DataGridViewColumn newColumn = new();
                    //newColumn.
                    //dataGridView.Columns.Insert()
                }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        //public static void AddRowLower_Click(object sender, EventArgs e)
        //{
        //    TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
        //    DataGridView dataGridView = tabInfo.DataGridView;

        //    int selectedRowCount = dataGridView.SelectedRows.Count;
        //    int columnCount = dataGridView.Columns.Count;

        //    if (selectedRowCount == 0 || !dataGridView.Rows[ClickedRowIndex].Selected)
        //    {
        //        dataGridView.Rows.Insert(ClickedRowIndex);
        //    }
        //    else
        //    {
        //        List<int> indexes = new List<int>();

        //        foreach (DataGridViewRow firstRow in dataGridView.SelectedRows)
        //        {
        //            indexes.Add(firstRow.Index);
        //        }
        //        indexes.Sort();

        //        for (int j = selectedRowCount - 1; j > -1; j--)
        //        {
        //            dataGridView.Rows.Insert(indexes[j] + 1);
        //        }
        //    }

        //    tabInfo.IsChanged = true;

        //    tabInfo.Data = ReadData(dataGridView, tabInfo.ColumnCount);
        //}
        public static void AddColumnToRight_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void DuplicateColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void CopyColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void CutColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void InsertColumnToLeft_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void InsertColumnToRight_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void ClearColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void RemoveColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region ContextMenuOnRowClick
        public static void MoveRowUpper_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                for (int i = 0; i < tabInfo.DataGridView.Columns.Count; i++)
                {
                    var cell1 = dataGridView.Rows[ClickedRowIndex].Cells[i].Value;
                    var cell2 = dataGridView.Rows[ClickedRowIndex - 1].Cells[i].Value;
                    dataGridView.Rows[ClickedRowIndex].Cells[i].Value = cell2;
                    dataGridView.Rows[ClickedRowIndex - 1].Cells[i].Value = cell1;

                    var value1 = tabInfo.Data[(ClickedRowIndex) * dataGridView.ColumnCount + i];
                    var value2 = tabInfo.Data[(ClickedRowIndex + 1) * dataGridView.ColumnCount + i];
                    tabInfo.Data[(ClickedRowIndex) * dataGridView.ColumnCount + i] = value2;
                    tabInfo.Data[(ClickedRowIndex + 1) * dataGridView.ColumnCount + i] = value1;
                }
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void MoveRowLower_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    var cell1 = dataGridView.Rows[ClickedRowIndex].Cells[i].Value;
                    var cell2 = dataGridView.Rows[ClickedRowIndex + 1].Cells[i].Value;
                    dataGridView.Rows[ClickedRowIndex].Cells[i].Value = cell2;
                    dataGridView.Rows[ClickedRowIndex + 1].Cells[i].Value = cell1;

                    var value1 = tabInfo.Data[(ClickedRowIndex + 1) * dataGridView.ColumnCount + i];
                    var value2 = tabInfo.Data[(ClickedRowIndex + 2) * dataGridView.ColumnCount + i];
                    tabInfo.Data[(ClickedRowIndex + 1) * dataGridView.ColumnCount + i] = value2;
                    tabInfo.Data[(ClickedRowIndex + 2) * dataGridView.ColumnCount + i] = value1;
                }
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void AddRowUpper_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                int selectedRowCount = dataGridView.SelectedRows.Count;
                int columnCount = dataGridView.Columns.Count;

                if (selectedRowCount == 0 || !dataGridView.Rows[ClickedRowIndex].Selected)
                {
                    dataGridView.Rows.Insert(ClickedRowIndex);
                    tabInfo.Data.InsertRange((ClickedRowIndex + 1) * columnCount, new object[columnCount]);
                }
                else
                {
                    List<int> indexes = new();

                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        indexes.Add(row.Index);
                    }
                    indexes.Sort();

                    for (int i = selectedRowCount - 1; i > -1; i--)
                    {
                        dataGridView.Rows.Insert(indexes[i]);
                        tabInfo.RowCount++;
                        tabInfo.Data.InsertRange((indexes[i] + 1) * columnCount, new object[columnCount]);
                    }
                }

                tabInfo.IsChanged = true;
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void AddRowLower_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                int selectedRowCount = dataGridView.SelectedRows.Count;
                int columnCount = dataGridView.Columns.Count;

                if (selectedRowCount == 0 || !dataGridView.Rows[ClickedRowIndex].Selected)
                {
                    dataGridView.Rows.Insert(ClickedRowIndex + 1);
                    tabInfo.RowCount++;
                    tabInfo.Data.InsertRange((ClickedRowIndex + 2) * columnCount, new object[columnCount]);
                }
                else
                {
                    List<int> indexes = new();

                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        indexes.Add(row.Index);
                    }
                    indexes.Sort();

                    for (int i = selectedRowCount - 1; i > -1; i--)
                    {
                        if (dataGridView.Rows[indexes[i]].IsNewRow)
                        {
                            dataGridView.Rows.Add();
                            tabInfo.RowCount++;
                            tabInfo.Data.InsertRange((indexes[i] + 1) * columnCount, new object[columnCount]);
                        }
                        else
                        {
                            dataGridView.Rows.Insert(indexes[i] + 1);
                            tabInfo.RowCount++;
                            tabInfo.Data.InsertRange((indexes[i] + 2) * columnCount, new object[columnCount]);
                        }

                    }
                }

                tabInfo.IsChanged = true;
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void DuplicateRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                int columnCount = dataGridView.Columns.Count;
                int selectedRowCount = dataGridView.SelectedRows.Count;

                if (selectedRowCount == 0 || !dataGridView.Rows[ClickedRowIndex].Selected)
                {
                    object[] rowData = new object[columnCount];
                    for (int i = 0; i < columnCount; i++)
                    {
                        rowData[i] = dataGridView.Rows[ClickedRowIndex].Cells[i].Value;
                    }
                    dataGridView.Rows.Insert(ClickedRowIndex + 1, rowData);
                    tabInfo.RowCount++;
                    tabInfo.Data.InsertRange((ClickedRowIndex + 2) * columnCount, rowData);
                }
                else
                {
                    object[][] duplicateRowsData = new object[selectedRowCount][];
                    List<int> indexes = new();

                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        indexes.Add(row.Index);
                    }
                    indexes.Sort();

                    for (int i = 0; i < selectedRowCount; i++)
                    {
                        duplicateRowsData[i] = new object[columnCount];
                        DataGridViewRow row = dataGridView.Rows[indexes[i]];

                        for (int j = 0; j < columnCount; j++)
                        {
                            duplicateRowsData[i][j] = dataGridView.Rows[indexes[i]].Cells[j].Value;
                        }
                    }

                    for (int i = selectedRowCount - 1; i > -1; i--)
                    {
                        if (dataGridView.Rows[indexes[i]].IsNewRow)
                        {
                            dataGridView.Rows.Add();
                            tabInfo.RowCount++;
                        }
                        else
                        {
                            dataGridView.Rows.Insert(indexes[i] + 1, duplicateRowsData[i]);
                            tabInfo.RowCount++;
                        }
                        tabInfo.Data.InsertRange((indexes[i] + 1) * columnCount, duplicateRowsData[i]);
                    }
                }
                tabInfo.IsChanged = true;
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void CopyRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                int columnCount = dataGridView.Columns.Count;
                int selectedRowCount = dataGridView.SelectedRows.Count;

                if (selectedRowCount == 0 || !dataGridView.Rows[ClickedRowIndex].Selected)
                {
                    CopiedRows = new object[1][];
                    CopiedRows[0] = new object[columnCount];
                    for (int i = 0; i < columnCount; i++)
                    {
                        CopiedRows[0][i] = dataGridView.Rows[ClickedRowIndex].Cells[i].Value;
                    }
                    return;
                }

                CopiedRows = new object[selectedRowCount][];
                List<int> indexes = new();

                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                {
                    indexes.Add(row.Index);
                }
                indexes.Sort();

                for (int i = 0; i < selectedRowCount; i++)
                {
                    CopiedRows[i] = new object[columnCount];
                    DataGridViewRow row = dataGridView.Rows[indexes[i]];
                    for (int j = 0; j < columnCount; j++)
                    {
                        CopiedRows[i][j] = dataGridView.Rows[indexes[i]].Cells[j].Value;
                    }
                }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void CutRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                int columnCount = dataGridView.Columns.Count;
                int selectedRowCount = dataGridView.SelectedRows.Count;

                if (selectedRowCount == 0 || !dataGridView.Rows[ClickedRowIndex].Selected)
                {
                    CopiedRows = new object[1][];
                    CopiedRows[0] = new object[columnCount];

                    for (int i = columnCount - 1; i > -1; i--)
                    {
                        CopiedRows[0][i] = dataGridView.Rows[ClickedRowIndex].Cells[i].Value;
                        tabInfo.Data.RemoveAt((ClickedRowIndex + 1) * columnCount + i);
                    }

                    dataGridView.Rows.RemoveAt(ClickedRowIndex);
                    tabInfo.RowCount--;
                }
                else
                {
                    CopiedRows = new object[selectedRowCount][];
                    List<int> indexes = new();

                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        indexes.Add(row.Index);
                    }
                    indexes.Sort();

                    for (int i = 0; i < selectedRowCount; i++)
                    {
                        CopiedRows[i] = new object[columnCount];
                        DataGridViewRow row = dataGridView.Rows[indexes[i]];
                        for (int j = 0; j < columnCount; j++)
                        {
                            CopiedRows[i][j] = dataGridView.Rows[indexes[i]].Cells[j].Value;
                        }
                    }
                    for (int i = selectedRowCount - 1; i > -1; i--)
                    {
                        if (!dataGridView.Rows[indexes[i]].IsNewRow)
                        {
                            dataGridView.Rows.Remove(dataGridView.Rows[indexes[i]]);
                            tabInfo.RowCount--;
                            for (int j = columnCount - 1; j > -1; j--)
                                tabInfo.Data.RemoveAt((indexes[i] + 1) * columnCount + j);
                        }
                    }
                }

                tabInfo.IsChanged = true;
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void InsertRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                int insertableRowIndex = dataGridView.Rows[ClickedRowIndex].IsNewRow ? ClickedRowIndex : ClickedRowIndex + 1;

                for (int i = CopiedRows.Length - 1; i > -1; i--)
                {
                    object[] row = new object[dataGridView.ColumnCount];
                    int maxValue = dataGridView.ColumnCount >= CopiedRows[i].Length ? CopiedRows[i].Length : dataGridView.ColumnCount;

                    for (int j = maxValue - 1; j > -1; j--)
                    {
                        row[j] = CopiedRows[i][j];
                    }

                    dataGridView.Rows.Insert(insertableRowIndex, CopiedRows[i]);
                    tabInfo.RowCount++;
                    tabInfo.Data.InsertRange((ClickedRowIndex + 1) * dataGridView.ColumnCount, row);
                }
                tabInfo.IsChanged = true;
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void InsertRowToCurrent_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                object[] firstRow = new object[dataGridView.ColumnCount];
                int firtstRowMaxValue = dataGridView.ColumnCount >= CopiedRows[0].Length ? CopiedRows[0].Length : dataGridView.ColumnCount;
                for (int j = firtstRowMaxValue - 1; j > -1; j--)
                {
                    firstRow[j] = CopiedRows[0][j];
                }

                for (int i = 0; i < dataGridView.ColumnCount; i++)
                {
                    dataGridView.Rows[ClickedRowIndex].Cells[i].Value = firstRow[i];
                    tabInfo.Data[(ClickedRowIndex + 1) * dataGridView.ColumnCount + i] = firstRow[i];
                }

                for (int i = CopiedRows.Length - 1; i > 0; i--)
                {
                    dataGridView.Rows.Insert(ClickedRowIndex + 1, CopiedRows[i]);
                    tabInfo.RowCount++;

                    object[] row = new object[dataGridView.ColumnCount];
                    int maxValue = dataGridView.ColumnCount >= CopiedRows[i].Length ? CopiedRows[i].Length : dataGridView.ColumnCount;
                    for (int j = maxValue - 1; j > -1; j--)
                    {
                        row[j] = CopiedRows[i][j];
                    }
                    tabInfo.Data.InsertRange((ClickedRowIndex + 2) * dataGridView.ColumnCount, row);
                }
                tabInfo.IsChanged = true;
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void ClearRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;
                int columnCount = dataGridView.Columns.Count;
                int selectedRowCount = dataGridView.SelectedRows.Count;

                if (selectedRowCount == 0 || !dataGridView.Rows[ClickedRowIndex].Selected)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        dataGridView.Rows[ClickedRowIndex].Cells[i].Value = null;
                        tabInfo.Data[((ClickedRowIndex + 1) * columnCount + i)] = null;
                    }
                }
                else
                {
                    List<int> indexes = new();

                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        indexes.Add(row.Index);
                    }
                    indexes.Sort();

                    for (int i = selectedRowCount - 1; i > -1; i--)
                    {
                        if (!dataGridView.Rows[indexes[i]].IsNewRow)
                        {
                            for (int j = 0; j < columnCount; j++)
                            {
                                dataGridView[j, indexes[i]].Value = null;
                            }
                            for (int j = columnCount - 1; j > -1; j--)
                            {
                                tabInfo.Data[(indexes[i] + 1) * columnCount + j] = null;
                            }
                        }

                    }
                }

                tabInfo.IsChanged = true;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void DeleteRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;
                int columnCount = dataGridView.Columns.Count;
                int selectedRowCount = dataGridView.SelectedRows.Count;

                if (selectedRowCount == 0 || !dataGridView.Rows[ClickedRowIndex].Selected)
                {
                    dataGridView.Rows.Remove(dataGridView.Rows[ClickedRowIndex]);
                    tabInfo.RowCount--;
                    for (int i = columnCount - 1; i > -1; i--)
                    {
                        tabInfo.Data.RemoveAt(((ClickedRowIndex + 1) * columnCount + i));
                    }
                }
                else
                {
                    List<int> indexes = new();

                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        indexes.Add(row.Index);
                    }
                    indexes.Sort();

                    for (int i = selectedRowCount - 1; i > -1; i--)
                    {
                        if (!dataGridView.Rows[indexes[i]].IsNewRow)
                        {
                            dataGridView.Rows.Remove(dataGridView.Rows[indexes[i]]);
                            tabInfo.RowCount--;
                            for (int j = columnCount - 1; j > -1; j--)
                            {
                                tabInfo.Data.RemoveAt(((indexes[i] + 1) * columnCount + j));
                            }
                        }

                    }
                }

                tabInfo.IsChanged = true;
                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region ContextMenuOnTabClick
        public static void DuplicateTab_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void RenameTab_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo[ClickedTabNumber];
                // Переименовать TextBox, DataGridView, Tab

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void CloseTab_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                TabInfo tabInfo = TabsInfo[ClickedTabNumber];
                if (tabInfo.IsChanged)
                {
                    if (Methods.ShowConfirmationMessage() != DialogResult.Yes) return;
                }
                TabsInfo.RemoveAt(ClickedTabNumber);
                if (MainForm_Form.Controls.Contains(CloseTabButton)) MainForm_Form.Controls.Remove(CloseTabButton);
                Files_TabControl.TabPages.RemoveAt(ClickedTabNumber);
                if (Files_TabControl.TabPages.Count == 0) { ChangeFormElementsVisibility(false); ChangeProgramMenuAvailability(false); }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #endregion


        #region Program Menu

        #region File Program Menu
        public static void CreateFile_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                string newFileName = "";
                if (isDialogAlreadyOpened) return;
                var dialogResult = ShowCreationNewFileWindow(ref newFileName);

                if (dialogResult != DialogResult.OK) return;

                if (newFileName.Replace(" ", "") == "") newFileName = Methods.GetFieldsFromSettings(SettingsFile, "global-defaultFileName").Value;
                string extension = Methods.GetFieldsFromSettings(SettingsFile, "global-defaultFileExtension").Value;
                newFileName = newFileName.Trim() + "." + extension;
                TabPage newTabPage = CreateNewTabPage(newFileName);

                newTabPage.ImageIndex = 0;

                DataGridView dataGridView = newTabPage.Controls.OfType<DataGridView>().First();
                RichTextBox textBox = newTabPage.Controls.OfType<RichTextBox>().First();

                TabInfo tabInfo = new(
                    fullTabName: newTabPage.Name,
                    shortTabName: newFileName,
                    extension: extension,
                    dataGridView: dataGridView,
                    textBox: textBox
                    );
                tabInfo.DataGridView.ColumnCount = tabInfo.ColumnCount == 0 ? 1 : tabInfo.ColumnCount;
                string[] names = Methods.GenColumnNames(new string[tabInfo.DataGridView.ColumnCount]);
                for (int i = 0; i < tabInfo.DataGridView.ColumnCount; i++)
                {
                    tabInfo.DataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    tabInfo.Data.Add(names[i]);
                }

                TabsInfo.Add(tabInfo);

                Files_TabControl.TabPages.Add(newTabPage);
                Files_TabControl.SelectedIndex = Files_TabControl.TabCount - 1;
                ColumnCount_TextBox.Text = dataGridView.ColumnCount.ToString();

                ChangeFormElementsVisibility(true);
                ChangeProgramMenuAvailability(true, tabInfo);
                SetStatusBarInfoLabel(tabInfo);

                if (tabInfo.IsShowAsTable) Methods.WriteData(tabInfo.DataGridView, tabInfo.Data, tabInfo.ColumnCount);
                else Methods.WriteData(tabInfo.TextBox, tabInfo.Data, tabInfo.ColumnCount);

                Methods.ChangeStretchDataGridView(tabInfo.DataGridView, tabInfo.IsStretchCells);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            static DialogResult ShowCreationNewFileWindow(ref string inputText)
            {
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

                    isDialogAlreadyOpened = true;
                    result = inputBox.ShowDialog();
                    isDialogAlreadyOpened = false;
                    inputText = nameTextBox.Text;
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
                return result;
            }
        }
        public static void OpenFile_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void SaveFile_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void SaveFileAs_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region DataBase Program Menu
        // Add
        #endregion

        #region Edit Program Menu
        public static void Refresh_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void Clear_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region View Program Menu
        public static void ShowStatusBar_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

                Files_TabControl.Height = menuItem.Checked ? Files_TabControl.Height -= 25 : Files_TabControl.Height + 25;

                StatusBar_ToolStrip.Visible = menuItem.Checked;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void HideEmptyRows_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                if (Files_TabControl.SelectedIndex == -1 || Files_TabControl.SelectedTab == null) return;
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);

                tabInfo.IsHideEmptyRows = (sender as ToolStripMenuItem).Checked;

                if (tabInfo.IsShowAsTable) Methods.WriteData(tabInfo.DataGridView, tabInfo.Data, tabInfo.ColumnCount);
                else Methods.WriteData(tabInfo.TextBox, tabInfo.Data, tabInfo.ColumnCount);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void ShowAsTable_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                if (Files_TabControl.SelectedIndex == -1 || Files_TabControl.SelectedTab == null) return;
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);

                QickActionsMenu_ToolStrip.Items["increaseColumnCount_Button"].Visible = !tabInfo.IsShowAsTable;
                QickActionsMenu_ToolStrip.Items["decreaseColumnCount_Button"].Visible = !tabInfo.IsShowAsTable;
                QickActionsMenu_ToolStrip.Items["columnCountBox_TextBox"].Visible = !tabInfo.IsShowAsTable;

                if (!tabInfo.IsShowAsTable)
                    tabInfo.Data = Methods.ReadData(tabInfo.TextBox);

                tabInfo.IsShowAsTable = (sender as ToolStripMenuItem).Checked;

                ToolStripItemCollection itemsInView = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["view"]).DropDown.Items;
                ToolStripMenuItem hideEmptyRows = (ToolStripMenuItem)itemsInView["hideEmptyRows"];
                hideEmptyRows.Checked = false;
                hideEmptyRows.Enabled = tabInfo.IsShowAsTable;
                tabInfo.IsHideEmptyRows = hideEmptyRows.Checked;

                if (tabInfo.IsShowAsTable)
                {
                    Methods.WriteData(tabInfo.DataGridView, tabInfo.Data, tabInfo.ColumnCount);
                    tabInfo.TextBox.Text = "";
                }
                else
                {
                    Methods.WriteData(tabInfo.TextBox, tabInfo.Data, tabInfo.ColumnCount);
                    tabInfo.DataGridView.Rows.Clear();
                }

                tabInfo.DataGridView.Visible = tabInfo.IsShowAsTable;
                tabInfo.TextBox.Visible = !tabInfo.IsShowAsTable;

                Methods.ChangeStretchDataGridView(tabInfo.DataGridView, tabInfo.IsStretchCells);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void StretchCells_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                if (Files_TabControl.SelectedIndex == -1 || Files_TabControl.SelectedTab == null) return;
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);

                tabInfo.IsStretchCells = (sender as ToolStripMenuItem).Checked;

                if (!tabInfo.IsShowAsTable)
                {
                    tabInfo.Data = Methods.ReadData(tabInfo.TextBox);
                    Methods.WriteData(tabInfo.TextBox, tabInfo.Data, tabInfo.ColumnCount);
                }
                else Methods.ChangeStretchDataGridView(tabInfo.DataGridView, tabInfo.IsStretchCells);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region Settings
        public static void ShowSettings_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
        }
        #endregion

        #endregion


        #region Default Form Components

        #region Button
        public static void IncreaseButton_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                if (Files_TabControl.SelectedTab == null) return;
                if (!int.TryParse(ColumnCount_TextBox.Text, out int result)) return;
                if (result < 1) return;
                ColumnCount_TextBox.Text = $"{result + 1}";
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void DecreaseButton_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                if (Files_TabControl.SelectedTab == null) return;

                if (!int.TryParse(ColumnCount_TextBox.Text, out int result)) return;
                if (result <= 1) return;
                ColumnCount_TextBox.Text = $"{result - 1}";
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region TextBox
        public static void ColumnCountBox_TextChanged2(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { sender });
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                if (!int.TryParse(ColumnCount_TextBox.Text, out int newColumnCount) || newColumnCount <= 0)
                {
                    if (ColumnCount_TextBox.Text.Replace(" ", "") == "") return;

                    ColumnCount_TextBox.Text = tabInfo.ColumnCount.ToString();
                    return;
                }

                if (!tabInfo.IsShowAsTable) return;

                if (newColumnCount == tabInfo.ColumnCount)
                {
                    SetStatusBarInfoLabel(tabInfo);
                    return;
                }


                // Обработка заголовков таблицы
                int differense = Math.Abs(tabInfo.ColumnCount - newColumnCount);

                tabInfo.DataGridView.ColumnCount = newColumnCount;
                if (newColumnCount > tabInfo.ColumnCount)
                {
                    // ГЕНЕРАЦИЯ ИМЕН
                    string[] names = new string[tabInfo.DataGridView.ColumnCount];
                    for (int i = 0; i < names.Length; i++)
                    {
                        names[i] = dataGridView.Columns[i].HeaderText;
                    }
                    names = Methods.GenColumnNames(names);
                    // ДОБАВЛЕНИЕ ИМЕН
                    for (int i = 0; i < tabInfo.DataGridView.ColumnCount; i++)
                    {
                        if (i < tabInfo.DataGridView.ColumnCount - differense)
                            tabInfo.Data[i] = names[i];
                        else
                            tabInfo.Data.Insert(i, names[i]);
                    }
                }
                else // newColumnCount < tabInfo.ColumnCount
                {
                    int columnNumber = tabInfo.ColumnCount - differense;

                    for (int i = 0; i < differense; i++)
                    {
                        tabInfo.Data.RemoveAt(columnNumber);
                    }
                }

                // Добавление пустых значений в последнюю строку данных
                int dataNeedToAdd = tabInfo.DataGridView.ColumnCount - tabInfo.Data.Count % tabInfo.DataGridView.ColumnCount;
                for (int i = 0; i < dataNeedToAdd; i++)
                    tabInfo.Data.Add(null);

                tabInfo.ColumnCount = tabInfo.DataGridView.ColumnCount;

                if (tabInfo.Data.Count - tabInfo.ColumnCount != 0)
                {
                    // Удаление пустых строк в конце данных
                    bool isBreak = false;
                    int count = tabInfo.Data.Count - tabInfo.ColumnCount;
                    int rowCount = count / tabInfo.ColumnCount;
                    if (count % tabInfo.ColumnCount != 0) rowCount++;

                    for (int i = rowCount; i > -1; i--)
                    {
                        int maxValue = count % tabInfo.ColumnCount == 0 ?
                            tabInfo.ColumnCount :
                            tabInfo.ColumnCount - count % tabInfo.ColumnCount;

                        for (int j = 0; j < maxValue; j++)
                        {
                            string cell = "" + tabInfo.Data[i * tabInfo.ColumnCount + j];
                            if (cell.Trim().Replace(" ", "").Replace("\u00A0", "") == "") continue;
                            else { isBreak = true; break; }
                        }
                        if (isBreak) break;

                        tabInfo.Data.RemoveRange(i * tabInfo.ColumnCount, maxValue);
                    }
                }


                // Обработка поведения таблицы в зависимости от состояния фиксации
                if (tabInfo.IsFixed) // Если зафиксировано
                {

                }
                else // Если не зафиксировано
                {
                    tabInfo.FixedColumnCount = tabInfo.ColumnCount;
                    ColumnCount_TextBox.Text = tabInfo.ColumnCount.ToString();
                    Methods.WriteData(tabInfo.DataGridView, tabInfo.Data, tabInfo.ColumnCount);
                }

                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void ColumnCountBox_TextChanged(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { sender });
            try
            {
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;

                if (!int.TryParse(ColumnCount_TextBox.Text, out int intResult) || intResult <= 0)
                {
                    if (ColumnCount_TextBox.Text.Replace(" ", "") == "") return;

                    ColumnCount_TextBox.Text = tabInfo.ColumnCount.ToString();
                    return;
                }

                if (intResult == tabInfo.ColumnCount)
                {
                    SetStatusBarInfoLabel(tabInfo);
                    return;
                }

                // Обработка заголовков таблицы
                int differense = intResult > tabInfo.ColumnCount ?
                    intResult - tabInfo.ColumnCount :
                    tabInfo.ColumnCount - intResult;

                tabInfo.DataGridView.ColumnCount = intResult;
                int columnNumber;

                if (intResult > tabInfo.ColumnCount) // Добавление столбцов
                {
                    string[] names = new string[dataGridView.Columns.Count];
                    for (int i = 0; i < names.Length; i++)
                    {
                        names[i] = dataGridView.Columns[i].HeaderText;
                    }
                    names = Methods.GenColumnNames(names);
                    for (int i = 0; i < dataGridView.Columns.Count; i++)
                    {
                        dataGridView.Columns[i].HeaderText = names[i];
                        // Изменить значения
                        if (i < tabInfo.DataGridView.ColumnCount - differense)
                        {
                            tabInfo.Data[i] = names[i];
                        }
                        else
                        {
                            tabInfo.Data.Insert(i, names[i]);
                        }
                    }

                    // Добавление пустых значений в последнюю строку данных
                    int dataNeedToAdd = tabInfo.DataGridView.ColumnCount - tabInfo.Data.Count % tabInfo.DataGridView.ColumnCount;
                    for (int i = 0; i < dataNeedToAdd; i++)
                        tabInfo.Data.Add(null);

                }
                else if (intResult < tabInfo.ColumnCount) // Удаление столбца
                {
                    columnNumber = tabInfo.ColumnCount - differense;

                    for (int i = 0; i < differense; i++)
                    {
                        tabInfo.Data.RemoveAt(columnNumber);
                    }
                }

                tabInfo.ColumnCount = tabInfo.DataGridView.ColumnCount;

                // Обработка поведения таблицы в зависимости от состояния фиксации
                if (tabInfo.IsFixed) // Если зафиксировано
                {

                }
                else // Если не зафиксировано
                {
                    tabInfo.FixedColumnCount = tabInfo.ColumnCount;
                    ColumnCount_TextBox.Text = tabInfo.ColumnCount.ToString();
                    if (tabInfo.IsShowAsTable) Methods.WriteData(tabInfo.DataGridView, tabInfo.Data, tabInfo.ColumnCount);
                    else Methods.WriteData(tabInfo.TextBox, tabInfo.Data, tabInfo.ColumnCount);
                }

                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region TabControl
        public static void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { sender });
            try
            {
                if (Files_TabControl.SelectedIndex == -1 || Files_TabControl.SelectedTab == null) return;
                TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                ColumnCount_TextBox.Text = tabInfo.ColumnCount.ToString();

                ToolStripItemCollection itemsInView = ((ToolStripMenuItem)ProgramMenu_MenuStrip.Items["view"]).DropDown.Items;
                ((ToolStripMenuItem)itemsInView["hideEmptyRows"]).Checked = tabInfo.IsHideEmptyRows;
                ((ToolStripMenuItem)itemsInView["showAsTable"]).Checked = tabInfo.IsShowAsTable;
                ((ToolStripMenuItem)itemsInView["stretchCells"]).Checked = tabInfo.IsStretchCells;

                QickActionsMenu_ToolStrip.Items["increaseColumnCount_Button"].Visible = tabInfo.IsShowAsTable;
                QickActionsMenu_ToolStrip.Items["decreaseColumnCount_Button"].Visible = tabInfo.IsShowAsTable;
                QickActionsMenu_ToolStrip.Items["columnCountBox_TextBox"].Enabled = tabInfo.IsShowAsTable;

                ChangeProgramMenuAvailability(true, tabInfo);

                SetStatusBarInfoLabel(tabInfo);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void TabControl_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                if (MainForm_Form.Controls.Contains(CloseTabButton)) MainForm_Form.Controls.Remove(CloseTabButton);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void TabControl_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (Files_TabControl.TabCount == 0) return;

                for (int i = 0; i < Files_TabControl.TabCount; i++)
                {
                    System.Drawing.Rectangle tabRectangle = Files_TabControl.GetTabRect(i);
                    if (tabRectangle.Contains(new System.Drawing.Point(e.X, e.Y)))
                    {
                        if (MainForm_Form.Controls.Contains(CloseTabButton))
                            MainForm_Form.Controls.Remove(CloseTabButton);

                        CloseTabButton.Location = new System.Drawing.Point(tabRectangle.Right - 14, 53);
                        CloseTabButton.Name = Files_TabControl.TabPages[i].Name;

                        MainForm_Form.Controls.Add(CloseTabButton);
                        CloseTabButton.BringToFront();
                        break;
                    }
                }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        public static void TabControl_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int distBeforeTab = 0;
                for (int i = 0; i < Files_TabControl.TabPages.Count; i++)
                {
                    System.Drawing.Rectangle tabRectangle = Files_TabControl.GetTabRect(i);
                    System.Drawing.Rectangle closeButtonRectangle = new(tabRectangle.Right - 15, tabRectangle.Top + 4, 9, 7);
                    if (e.Button == MouseButtons.Left)
                    {
                        if (closeButtonRectangle.Contains(e.Location))
                        {
                            ClickedTabNumber = i;
                            closeTab();
                            break;
                        }
                    }
                    else
                    {
                        if (tabRectangle.Contains(e.Location))
                        {
                            if (e.Button == MouseButtons.Middle)
                            {
                                ClickedTabNumber = i;
                                closeTab();
                                break;
                            }
                            else if (e.Button == MouseButtons.Right)
                            {
                                if (MainForm_Form.Controls.Contains(CloseTabButton)) MainForm_Form.Controls.Remove(CloseTabButton);
                                ClickedTabNumber = i;
                                showActionsMenu(tabRectangle, distBeforeTab);
                                break;
                            }
                        }
                    }
                    distBeforeTab += tabRectangle.Width;

                }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }

            void showActionsMenu(System.Drawing.Rectangle tabRectangle, int dist)
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod());
                try
                {
                    TabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.TabPages[ClickedTabNumber].Name);
                    int x = Cursor.Position.X - e.X + dist,
                        y = Cursor.Position.Y - e.Y + tabRectangle.Height + 3;
                    ContextMenuOnTabClick.Show(new System.Drawing.Point(x, y));
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
            void closeTab()
            {
                Methods.TraceCalls(MethodBase.GetCurrentMethod());
                try
                {
                    TabInfo tabInfo = TabsInfo[ClickedTabNumber];
                    if (tabInfo.IsChanged)
                    {
                        if (Methods.ShowConfirmationMessage() != DialogResult.Yes) return;
                    }
                    TabsInfo.RemoveAt(ClickedTabNumber);
                    if (MainForm_Form.Controls.Contains(CloseTabButton)) MainForm_Form.Controls.Remove(CloseTabButton);
                    Files_TabControl.TabPages.RemoveAt(ClickedTabNumber);
                    if (Files_TabControl.TabPages.Count == 0) { ChangeFormElementsVisibility(false); ChangeProgramMenuAvailability(false); }
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
        }
        #endregion

        #region Form
        public static void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                if (TabsInfo.Count == 0) return;
                List<TabInfo> fileNames = new();

                foreach (TabInfo tab in TabsInfo)
                {
                    if (tab.IsChanged) fileNames.Add(tab);
                }

                if (fileNames.Count == 0) return;

                string message = "Вы не сохранили изменения в файл";

                if (fileNames.Count == 1)
                {
                    message +=
                        "е \"" +
                        $"{fileNames[0].ShortTabName}" +
                        "\"\nОни будут утеряны! Продолжить выполнение?";
                }
                else
                {
                    message += $"ах:";
                    int index = 0;
                    foreach (TabInfo tab in fileNames)
                    {
                        message +=
                            "\n   \"" +
                            $"{tab.ShortTabName}" +
                            "\"";
                        if (index < fileNames.Count - 1) message += ",";
                        index++;
                    }
                    message += $"\nОни будут утеряны! Продолжить выполнение?";
                }

                e.Cancel = !(Methods.ShowConfirmationMessage(message) == DialogResult.Yes);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }

        #endregion

        #endregion
    }
}
