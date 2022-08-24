using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using CSV_Redactor.Forms;
using CSV_Redactor.TabInfoFolder.Classes;
using CSV_Redactor.TabInfoFolder.Interfaces;
using static CSV_Redactor.Main_Form;
using static CSV_Redactor.Main_Form.OldTabInfo;

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

        #region Tab Components 

        #region Data Grid View
        internal static void DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            return;
            try
            {
                //Tab.ClickedRowIndex = e.RowIndex;
                //Tab.ClickedColumnIndex = e.ColumnIndex;
                //DataGridView dataGridView = (DataGridView)sender;
                //if (e.Button == MouseButtons.Right)
                //{
                //    if (e.ColumnIndex == -1 && e.RowIndex == -1) return;
                //    if (e.RowIndex == -1)
                //    {
                //        ContextMenuOnHeaderClick.Items["Переместить влево"].Enabled = ClickedColumnIndex != 0;
                //        ContextMenuOnHeaderClick.Items["Переместить вправо"].Enabled = ClickedColumnIndex != tabInfo.ColumnCount - 1;
                //        ContextMenuOnHeaderClick.Items["Вставить слева"].Enabled = CopiedColumns != null;
                //        ContextMenuOnHeaderClick.Items["Вставить справа"].Enabled = CopiedColumns != null;
                //        ContextMenuOnHeaderClick.Items["Вставить в текущий"].Enabled = CopiedColumns != null;
                //        ContextMenuOnHeaderClick.Show(Cursor.Position);
                //    }
                //    else if (!dataGrid.Rows[e.RowIndex].IsNewRow)
                //    {
                //        ContextMenuOnRowClick.Items["Переместить выше"].Enabled = ClickedRowIndex != 0;
                //        ContextMenuOnRowClick.Items["Переместить ниже"].Enabled = ClickedRowIndex != tabInfo.RowCount - 1;
                //        ContextMenuOnRowClick.Items["Вставить"].Enabled = CopiedRows != null;
                //        ContextMenuOnRowClick.Items["Вставить в текущую"].Enabled = CopiedRows != null;
                //        ContextMenuOnRowClick.Items["Очистить"].Enabled = !tabInfo.IsHideEmptyRows;
                //        ContextMenuOnRowClick.Show(Cursor.Position);
                //    }
                //    else if (dataGrid.Rows[e.RowIndex].IsNewRow)
                //    {
                //        ContextMenuOnNewRowClick.Items["Вставить"].Enabled = CopiedRows != null;
                //        ContextMenuOnNewRowClick.Show(Cursor.Position);
                //    }
                //}
                //if (e.Button == MouseButtons.Left)
                //{
                //    if (e.RowIndex == -1)
                //    {
                //        if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Shift)
                //        {
                //            // Добавить текущую к выделенным
                //            dataGrid.Columns[ClickedColumnIndex].Selected = true;
                //        }
                //        else
                //        {
                //            // Выделить только текущую
                //            dataGrid.ClearSelection();
                //            dataGrid.Columns[ClickedColumnIndex].Selected = true;
                //        }
                //    }
                //}
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            return;
            try
            {
                //OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                //DataGridView dataGridView = tabInfo.DataGridView;
                //int neededCount = (e.RowIndex + 2) * dataGridView.ColumnCount;
                //if (tabInfo.Data.Count < neededCount)
                //{
                //    int difference = neededCount - tabInfo.Data.Count;
                //    tabInfo.Data.InsertRange(neededCount - difference, new object[difference]);
                //}

                //tabInfo.Data[(e.RowIndex + 1) * dataGridView.ColumnCount + e.ColumnIndex] = dataGridView[e.ColumnIndex, e.RowIndex].Value;

                //tabInfo.IsChanged = true;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void DataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
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
        internal static void DataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            return;
            try
            {
                //OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                //tabInfo.RowCount = tabInfo.DataGridView.RowCount - 1;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region Rich Text Box
        internal static void TextBox_TextChanged(object sender, EventArgs e)
        {
            return;
            try
            {
                if (Files_TabControl.SelectedIndex == -1 || Files_TabControl.SelectedTab == null) return;
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region ListBox
        internal static void ListBox_DoubleClick(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if (listBox.SelectedItem != null)
            {
                DataBase tab = (DataBase)Tab.FindClassOfCurrentTab(Tab.Main_TabControl, false);
                tab.CreateNewTab();
            }
        }

        #endregion
        #region TabControl

        #endregion

        #endregion


        #region Context Menu

        #region ContextMenuOnHeaderClick
        internal static void ChangeHeader_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
                DataGridView dataGridView = tabInfo.DataGridView;
                var clickedColumn = dataGridView.Columns[ClickedColumnIndex].HeaderText = (ClickedColumnIndex + 1).ToString();
                tabInfo.IsChanged = true;
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void PinColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void MoveColumnToLeft_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
        internal static void MoveColumnToRight_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
        internal static void AddColumnToLeft_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
        //    OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
        internal static void AddColumnToRight_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void DuplicateColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void CopyColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void CutColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void InsertColumnToLeft_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void InsertColumnToRight_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void ClearColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void RemoveColumn_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region ContextMenuOnRowClick
        internal static void MoveRowUpper_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void MoveRowLower_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void AddRowUpper_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void AddRowLower_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void DuplicateRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void CopyRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
        internal static void CutRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void InsertRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void InsertRowToCurrent_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void ClearRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
        internal static void DeleteRow_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region ContextMenuOnTabClick
        internal static void DuplicateTab_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void RenameTab_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo[ClickedTabNumber];
                // Переименовать TextBox, DataGridView, Tab

            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void CloseTab_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                OldTabInfo tabInfo = TabsInfo[ClickedTabNumber];
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
        internal static void CreateFile_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            new TextFile().CreateNewTab();
        }
        internal static void OpenFile_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            new OpenedTextFile().CreateNewTab();
        }
        internal static void SaveFile_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            IFile result = (IFile)Tab.FindCurrentTabClass(Tab.Main_TabControl, true);
            result.Save();
            ((IDefaultFieldsOfTabs)result).IsChanged = false;
        }
        internal static void SaveFileAs_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
        }
        #endregion

        #region DataBase Program Menu
        internal static void OpenDataBase_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            DataBase.CreateDataBaseTab();
        }
        #endregion

        #region Edit Program Menu
        internal static void Refresh_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            ((IFile)Tab.FindCurrentTabClass(Tab.Main_TabControl, true)).LoadData();
        }
        internal static void Clear_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            ((IDefaultFieldsOfTabs)Tab.FindCurrentTabClass(Tab.Main_TabControl, true)).Clear();
        }
        #endregion

        #region View Program Menu
        internal static void ShowStatusBar_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            Files_TabControl.Height = menuItem.Checked ? Files_TabControl.Height -= 25 : Files_TabControl.Height + 25;
            StatusBar_ToolStrip.Visible = menuItem.Checked;
        }
        internal static void HideEmptyRows_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                if (Files_TabControl.SelectedIndex == -1 || Files_TabControl.SelectedTab == null) return;
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);

                tabInfo.IsHideEmptyRows = (sender as ToolStripMenuItem).Checked;

                if (tabInfo.IsShowAsTable) Methods.WriteData(tabInfo.DataGridView, tabInfo.Data, tabInfo.ColumnCount);
                else Methods.WriteData(tabInfo.TextBox, tabInfo.Data, tabInfo.ColumnCount);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void ShowAsTable_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                if (Files_TabControl.SelectedIndex == -1 || Files_TabControl.SelectedTab == null) return;
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);

                QickActionsMenu_ToolStrip.Items["increaseColumnCount_Button"].Visible = !tabInfo.IsShowAsTable;
                QickActionsMenu_ToolStrip.Items["decreaseColumnCount_Button"].Visible = !tabInfo.IsShowAsTable;
                QickActionsMenu_ToolStrip.Items["columnCountBox_TextBox"].Visible = !tabInfo.IsShowAsTable;

                if (!tabInfo.IsShowAsTable)
                    tabInfo.Data = Methods.ReadData(tabInfo.TextBox.Text);

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

                //Methods.ChangeStretchDataGridView(tabInfo.DataGridView, tabInfo.IsStretchCells);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void StretchCells_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                //var tab = TabInfo.FindClassOfCurrentTab(Files_TabControl);
                //if (tab is DataBase dataBase)
                //{
                //    var table = TabInfo.FindClassOfCurrentTab(dataBase.Tables_TabControl) as DataBaseTable;
                //    table.IsStretchCells = (sender as ToolStripMenuItem).Checked;
                //}
                //else
                //{
                //    var file = tab as TextFile;
                //    file.IsStretchCells = (sender as ToolStripMenuItem).Checked;
                //}
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        #endregion

        #region Settings
        internal static void ShowSettings_Click(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());

            System.Windows.Window settingWindow = new SettingsWindow();
            ElementHost.EnableModelessKeyboardInterop(settingWindow);
            new WindowInteropHelper(settingWindow).Owner = MainForm_Form.Handle;
            settingWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            settingWindow.ShowDialog();
        }
        #endregion

        #endregion


        #region Default Form Components

        #region Button
        internal static void IncreaseButton_Click(object sender, EventArgs e)
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
        internal static void DecreaseButton_Click(object sender, EventArgs e)
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
        internal static void ColumnCountBox_TextChanged(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { sender });
            return;
            try
            {
                OldTabInfo tabInfo = TabsInfo.Find(tab => tab.FullTabName == Files_TabControl.SelectedTab.Name);
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
                    //SetStatusBarInfoLabel(tabInfo);
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
                    //names = Methods.GenColumnNames(names);
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
        #endregion

        #region TabControl
        internal static void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod(), new object[] { sender });
            try
            {
                var result = Tab.FindCurrentTabClass(sender, true);
                if (result != null)
                {
                    Tab.ChangeProgramMenuAvailability((IDefaultFieldsOfTabs)result);
                }
                else Tab.ChangeProgramMenuAvailability();
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void TabControl_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                if (MainForm_Form.Controls.Contains(CloseTabButton)) MainForm_Form.Controls.Remove(CloseTabButton);
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void TabControl_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                TabControl control = (TabControl)sender;
                if (control.TabPages.Count == 0) return;

                for (int i = 0; i < control.TabCount; i++)
                {
                    System.Drawing.Rectangle tabRectangle = control.GetTabRect(i);
                    if (tabRectangle.Contains(new System.Drawing.Point(e.X, e.Y)))
                    {
                        if (MainForm_Form.Controls.Contains(CloseTabButton))
                            MainForm_Form.Controls.Remove(CloseTabButton);
                        int additionalHeight = 0,
                            additionalWidth = 0;
                        if (control.Name != "Files_TabControl")
                        {
                            ITab tab = Tab.FindClassOfCurrentTab(control, true);
                            DataBase dataBase = ((DataBaseTable)tab).DataBase;

                            additionalHeight = 24;
                            additionalWidth = dataBase.SplitContainer.Panel1.Width + dataBase.SplitContainer.SplitterWidth;
                        }
                        CloseTabButton.Location = new System.Drawing.Point(tabRectangle.Right - 14 + additionalWidth, 53 + additionalHeight);
                        CloseTabButton.Name = control.TabPages[i].Name;

                        MainForm_Form.Controls.Add(CloseTabButton);
                        CloseTabButton.BringToFront();
                        break;
                    }
                }
            }
            catch (Exception ex) { Methods.ExceptionProcessing(ex); }
        }
        internal static void TabControl_MouseDown(object sender, MouseEventArgs e)
        {
            TabControl control = (TabControl)sender;
            if (control.TabPages.Count == 0) return;

            try
            {
                int distBeforeTab = 0;

                for (int i = 0; i < control.TabPages.Count; i++)
                {
                    System.Drawing.Rectangle tabRectangle = control.GetTabRect(i);
                    System.Drawing.Rectangle closeButtonRectangle = new(tabRectangle.Right - 15, tabRectangle.Top + 4, 9, 7);
                    if (e.Button == MouseButtons.Left)
                    {
                        if (closeButtonRectangle.Contains(e.Location))
                        {
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
                                closeTab();
                                break;
                            }
                            else if (e.Button == MouseButtons.Right)
                            {
                                if (control.Controls.Contains(CloseTabButton)) control.Controls.Remove(CloseTabButton);
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
                    var tabPage = Tab.FindClassOfCurrentTab(control, true);

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
                    var tabPage = Tab.FindClassOfCurrentTab(control, false);
                    bool isShowConfirmation = false;
                    if (tabPage is IDefaultFieldsOfTabs tabInfo && tabInfo.IsChanged)
                    {
                        isShowConfirmation = true;
                    }
                    else if (tabPage is DataBase dataBase)
                    {
                        foreach (var tab in dataBase.Tabs)
                        {
                            if (((IDefaultFieldsOfTabs)tab).IsChanged)
                            {
                                isShowConfirmation = true;
                                break;
                            }
                        }
                    }
                    if (isShowConfirmation)
                        if (Methods.ShowConfirmationMessage() != DialogResult.Yes) return;

                    Tab.Tabs.Remove(tabPage);
                    if (tabPage is DataBaseTable table)
                        table.DataBase.Tabs.Remove(tabPage);
                    if (Tab.MainForm_Form.Controls.Contains(CloseTabButton))
                        Tab.MainForm_Form.Controls.Remove(CloseTabButton);
                    control.TabPages.Remove(tabPage.TabPage);
                    if (control.Name == "Files_TabControl" && control.TabPages.Count == 0)
                    {
                        Tab.ChangeFormElementsVisibility(false);
                        Tab.ChangeProgramMenuAvailability();
                    }
                }
                catch (Exception ex) { Methods.ExceptionProcessing(ex); }
            }
        }
        #endregion

        #region Form
        internal static void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Methods.TraceCalls(MethodBase.GetCurrentMethod());
            try
            {
                if (TabsInfo.Count == 0) return;
                List<OldTabInfo> fileNames = new();

                foreach (OldTabInfo tab in TabsInfo)
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
                    foreach (OldTabInfo tab in fileNames)
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