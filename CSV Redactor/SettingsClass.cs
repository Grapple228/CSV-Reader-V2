using System;
using System.Xml.Linq;

namespace CSV_Redactor.Forms
{
    public class SettingsClass
    {
        // Окно
        public int MinimumWindowWidth { get; set; }
        public int MinimumWindowHeight { get; set; }

        // Интерфейс
        public bool IsShowStatusBar { get; set; }

        // Таблица
        public string DefaultColumnName { get; set; }

        // Вкладка
        public string DefaultFileName { get; set; }
        public string DefaultTableName { get; set; }

        // Дополнительно
        public string DefaultFileExtension { get; set; }
        public string SupportedSeparators { get; set; }

        // Отладка
        public bool IsTracingEnabled { get; set; }

        // Текст
        public int DefaultColumnCount { get; set; }
        public bool IsShowAsTable { get; set; }
        public bool IsStretchCells { get; set; }
        public bool IsHideEmptyRows { get; set; }

        // База данных

        public SettingsClass(XDocument SettingsFile)
        {
            XElement global = Methods.GetFieldsFromSettings(SettingsFile,"global");
            XElement local = Methods.GetFieldsFromSettings(SettingsFile,"local");
            MinimumWindowWidth = Convert.ToInt32(global.Element("MinimumWindowWidth").Value);
            MinimumWindowHeight = Convert.ToInt32(global.Element("MinimumWindowHeight").Value);
            IsShowStatusBar = Convert.ToBoolean(global.Element("IsShowStatusBar").Value);
            DefaultColumnName = global.Element("DefaultColumnName").Value;
            DefaultFileName = global.Element("DefaultFileName").Value;
            DefaultTableName = global.Element("DefaultTableName").Value;
            DefaultFileExtension = global.Element("DefaultFileExtension").Value;
            SupportedSeparators = global.Element("SupportedSeparators").Value;
            IsTracingEnabled = Convert.ToBoolean(global.Element("IsTracingEnabled").Value);
            XElement text = local.Element("text");
            DefaultColumnCount = Convert.ToInt32(text.Element("DefaultColumnCount").Value);
            IsShowAsTable = Convert.ToBoolean(text.Element("IsShowAsTable").Value);
            IsStretchCells = Convert.ToBoolean(text.Element("IsStretchCells").Value);
            IsHideEmptyRows = Convert.ToBoolean(text.Element("IsHideEmptyRows").Value);
        }
    }
}
