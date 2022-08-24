namespace CSV_Redactor.TabInfoFolder.Interfaces
{
    /// <summary>
    /// Созданные на основе файла
    /// </summary>
    internal interface IFile
    {
        abstract string FilePath { get; set; }
        abstract void LoadData();
        abstract void Save();
    }
}
