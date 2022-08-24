namespace CSV_Redactor
{
    public partial class Main_Form
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Form));
            this.ProgramActionsBar = new System.Windows.Forms.MenuStrip();
            this.ActionButtons_ToolStrip = new System.Windows.Forms.ToolStrip();
            this.increaseColumnCount_Button = new System.Windows.Forms.ToolStripButton();
            this.columnCountBox_TextBox = new System.Windows.Forms.ToolStripTextBox();
            this.decreaseColumnCount_Button = new System.Windows.Forms.ToolStripButton();
            this.Files_TabControl = new System.Windows.Forms.TabControl();
            this.StatusBar = new System.Windows.Forms.ToolStrip();
            this.tabInfo_ToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.ActionButtons_ToolStrip.SuspendLayout();
            this.StatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProgramActionsBar
            // 
            this.ProgramActionsBar.Location = new System.Drawing.Point(0, 0);
            this.ProgramActionsBar.Name = "ProgramActionsBar";
            this.ProgramActionsBar.Size = new System.Drawing.Size(800, 24);
            this.ProgramActionsBar.TabIndex = 1;
            this.ProgramActionsBar.Text = "menuStrip1";
            // 
            // ActionButtons_ToolStrip
            // 
            this.ActionButtons_ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.increaseColumnCount_Button,
            this.columnCountBox_TextBox,
            this.decreaseColumnCount_Button});
            this.ActionButtons_ToolStrip.Location = new System.Drawing.Point(0, 24);
            this.ActionButtons_ToolStrip.Name = "ActionButtons_ToolStrip";
            this.ActionButtons_ToolStrip.Size = new System.Drawing.Size(800, 25);
            this.ActionButtons_ToolStrip.TabIndex = 3;
            this.ActionButtons_ToolStrip.Text = "toolStrip1";
            this.ActionButtons_ToolStrip.Visible = false;
            // 
            // increaseColumnCount_Button
            // 
            this.increaseColumnCount_Button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.increaseColumnCount_Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.increaseColumnCount_Button.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.increaseColumnCount_Button.Image = ((System.Drawing.Image)(resources.GetObject("increaseColumnCount_Button.Image")));
            this.increaseColumnCount_Button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.increaseColumnCount_Button.Margin = new System.Windows.Forms.Padding(0);
            this.increaseColumnCount_Button.Name = "increaseColumnCount_Button";
            this.increaseColumnCount_Button.Size = new System.Drawing.Size(23, 25);
            this.increaseColumnCount_Button.Text = "+";
            this.increaseColumnCount_Button.ToolTipText = "Добавить колонку в конец";
            // 
            // columnCountBox_TextBox
            // 
            this.columnCountBox_TextBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.columnCountBox_TextBox.AutoSize = false;
            this.columnCountBox_TextBox.BackColor = System.Drawing.SystemColors.Menu;
            this.columnCountBox_TextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.columnCountBox_TextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.columnCountBox_TextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.columnCountBox_TextBox.Name = "columnCountBox_TextBox";
            this.columnCountBox_TextBox.Size = new System.Drawing.Size(30, 23);
            this.columnCountBox_TextBox.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnCountBox_TextBox.ToolTipText = "Количество колонок";
            // 
            // decreaseColumnCount_Button
            // 
            this.decreaseColumnCount_Button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.decreaseColumnCount_Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.decreaseColumnCount_Button.Image = ((System.Drawing.Image)(resources.GetObject("decreaseColumnCount_Button.Image")));
            this.decreaseColumnCount_Button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.decreaseColumnCount_Button.Margin = new System.Windows.Forms.Padding(0);
            this.decreaseColumnCount_Button.Name = "decreaseColumnCount_Button";
            this.decreaseColumnCount_Button.Size = new System.Drawing.Size(23, 25);
            this.decreaseColumnCount_Button.Text = "-";
            this.decreaseColumnCount_Button.ToolTipText = "Убрать последнюю колонку";
            // 
            // Files_TabControl
            // 
            this.Files_TabControl.AllowDrop = true;
            this.Files_TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Files_TabControl.HotTrack = true;
            this.Files_TabControl.Location = new System.Drawing.Point(0, 24);
            this.Files_TabControl.Margin = new System.Windows.Forms.Padding(0);
            this.Files_TabControl.Name = "Files_TabControl";
            this.Files_TabControl.SelectedIndex = 0;
            this.Files_TabControl.Size = new System.Drawing.Size(800, 512);
            this.Files_TabControl.TabIndex = 5;
            // 
            // StatusBar
            // 
            this.StatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tabInfo_ToolStripLabel});
            this.StatusBar.Location = new System.Drawing.Point(0, 536);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(800, 25);
            this.StatusBar.TabIndex = 6;
            this.StatusBar.Text = "toolStrip1";
            // 
            // tabInfo_ToolStripLabel
            // 
            this.tabInfo_ToolStripLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tabInfo_ToolStripLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tabInfo_ToolStripLabel.Font = new System.Drawing.Font("Arial", 9.5F);
            this.tabInfo_ToolStripLabel.Name = "tabInfo_ToolStripLabel";
            this.tabInfo_ToolStripLabel.Size = new System.Drawing.Size(0, 22);
            this.tabInfo_ToolStripLabel.ToolTipText = "Общая информация о текущей вкладке";
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 561);
            this.Controls.Add(this.StatusBar);
            this.Controls.Add(this.Files_TabControl);
            this.Controls.Add(this.ActionButtons_ToolStrip);
            this.Controls.Add(this.ProgramActionsBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.ProgramActionsBar;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "Main_Form";
            this.Text = "CSV Redactor";
            this.Load += new System.EventHandler(this.Main_Form_Load);
            this.ActionButtons_ToolStrip.ResumeLayout(false);
            this.ActionButtons_ToolStrip.PerformLayout();
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.MenuStrip ProgramActionsBar;
        public System.Windows.Forms.ToolStrip ActionButtons_ToolStrip;
        public System.Windows.Forms.TabControl Files_TabControl;
        public System.Windows.Forms.ToolStripButton increaseColumnCount_Button;
        public System.Windows.Forms.ToolStripButton decreaseColumnCount_Button;
        public System.Windows.Forms.ToolStripTextBox columnCountBox_TextBox;
        public System.Windows.Forms.ToolStrip StatusBar;
        public System.Windows.Forms.ToolStripLabel tabInfo_ToolStripLabel;
    }
}

