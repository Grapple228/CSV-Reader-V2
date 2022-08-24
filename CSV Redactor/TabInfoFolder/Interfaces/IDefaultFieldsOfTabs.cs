using System.Collections.Generic;
using System.Windows.Forms;

namespace CSV_Redactor.TabInfoFolder.Interfaces
{
    /// <summary>
    /// Содержат поля информации о вкладке
    /// </summary>
    internal interface IDefaultFieldsOfTabs
    {
        abstract DataGridView DataGridView { get; }
        abstract RichTextBox TextBox { get; }
        abstract List<string> Data { get; set; }
        abstract int ColumnCount { get; set; }
        abstract int FixedColumnCount { get; set; }
        abstract int RowCount { get; set; }
        abstract char Separator { get; set; }
        abstract bool IsChanged { get; set; }
        abstract bool IsHideEmptyRows { get; set; }
        abstract bool IsShowAsTable { get; set; }
        abstract bool IsStretchCells { get; set; }
        abstract bool IsFixed { get; set; }
        abstract void Clear();
        abstract void SaveFileAs(string fileName = null);
        abstract List<string> ReadData(string filePath = null);
        abstract void WriteData(string filePath = null);
    }
}
