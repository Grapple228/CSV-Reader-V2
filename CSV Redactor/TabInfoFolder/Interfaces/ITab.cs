using System.Windows.Forms;

namespace CSV_Redactor.TabInfoFolder.Interfaces
{
    /// <summary>
    /// Является элементом Main_TabControl
    /// </summary>
    internal interface ITab
    {
        abstract string PartialName { get; set; }
        abstract string FullName { get; set; }
        abstract TabPage TabPage { get; }
        abstract void CreateNewTab();
        //abstract void DeleteTab();
    }
}
