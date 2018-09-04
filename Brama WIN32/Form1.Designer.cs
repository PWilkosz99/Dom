namespace aplikacja_pc_do_esp
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Wymagana metoda obsługi projektanta — nie należy modyfikować 
        /// zawartość tej metody z edytorem kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
            this.ip = new MetroFramework.Controls.MetroTextBox();
            this.port = new MetroFramework.Controls.MetroTextBox();
            this.metroToggle1 = new MetroFramework.Controls.MetroToggle();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // metroButton1
            // 
            this.metroButton1.Location = new System.Drawing.Point(23, 230);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(129, 43);
            this.metroButton1.TabIndex = 0;
            this.metroButton1.Text = "Wyślij dane";
            this.metroButton1.UseSelectable = true;
            this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // metroTextBox1
            // 
            // 
            // 
            // 
            this.metroTextBox1.CustomButton.Image = null;
            this.metroTextBox1.CustomButton.Location = new System.Drawing.Point(279, 1);
            this.metroTextBox1.CustomButton.Name = "";
            this.metroTextBox1.CustomButton.Size = new System.Drawing.Size(147, 147);
            this.metroTextBox1.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBox1.CustomButton.TabIndex = 1;
            this.metroTextBox1.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBox1.CustomButton.UseSelectable = true;
            this.metroTextBox1.CustomButton.Visible = false;
            this.metroTextBox1.Lines = new string[] {
        "Test"};
            this.metroTextBox1.Location = new System.Drawing.Point(23, 63);
            this.metroTextBox1.MaxLength = 32767;
            this.metroTextBox1.Multiline = true;
            this.metroTextBox1.Name = "metroTextBox1";
            this.metroTextBox1.PasswordChar = '\0';
            this.metroTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.metroTextBox1.SelectedText = "";
            this.metroTextBox1.SelectionLength = 0;
            this.metroTextBox1.SelectionStart = 0;
            this.metroTextBox1.Size = new System.Drawing.Size(427, 149);
            this.metroTextBox1.TabIndex = 1;
            this.metroTextBox1.Text = "Test";
            this.metroTextBox1.UseSelectable = true;
            this.metroTextBox1.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.metroTextBox1.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // ip
            // 
            // 
            // 
            // 
            this.ip.CustomButton.Image = null;
            this.ip.CustomButton.Location = new System.Drawing.Point(94, 2);
            this.ip.CustomButton.Name = "";
            this.ip.CustomButton.Size = new System.Drawing.Size(19, 19);
            this.ip.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.ip.CustomButton.TabIndex = 1;
            this.ip.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.ip.CustomButton.UseSelectable = true;
            this.ip.CustomButton.Visible = false;
            this.ip.Lines = new string[] {
        "IP:    xxx.xxx.xxx.xxx"};
            this.ip.Location = new System.Drawing.Point(158, 240);
            this.ip.MaxLength = 32767;
            this.ip.Name = "ip";
            this.ip.PasswordChar = '\0';
            this.ip.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.ip.SelectedText = "";
            this.ip.SelectionLength = 0;
            this.ip.SelectionStart = 0;
            this.ip.Size = new System.Drawing.Size(116, 24);
            this.ip.TabIndex = 2;
            this.ip.Text = "IP:    xxx.xxx.xxx.xxx";
            this.ip.UseSelectable = true;
            this.ip.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.ip.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.ip.Click += new System.EventHandler(this.ip_Click);
            // 
            // port
            // 
            // 
            // 
            // 
            this.port.CustomButton.Image = null;
            this.port.CustomButton.Location = new System.Drawing.Point(42, 2);
            this.port.CustomButton.Name = "";
            this.port.CustomButton.Size = new System.Drawing.Size(19, 19);
            this.port.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.port.CustomButton.TabIndex = 1;
            this.port.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.port.CustomButton.UseSelectable = true;
            this.port.CustomButton.Visible = false;
            this.port.Lines = new string[] {
        "Port:  xxxx"};
            this.port.Location = new System.Drawing.Point(280, 240);
            this.port.MaxLength = 32767;
            this.port.Name = "port";
            this.port.PasswordChar = '\0';
            this.port.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.port.SelectedText = "";
            this.port.SelectionLength = 0;
            this.port.SelectionStart = 0;
            this.port.Size = new System.Drawing.Size(64, 24);
            this.port.TabIndex = 3;
            this.port.Text = "Port:  xxxx";
            this.port.UseSelectable = true;
            this.port.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.port.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.port.Click += new System.EventHandler(this.port_Click);
            // 
            // metroToggle1
            // 
            this.metroToggle1.AutoSize = true;
            this.metroToggle1.Location = new System.Drawing.Point(370, 247);
            this.metroToggle1.Name = "metroToggle1";
            this.metroToggle1.Size = new System.Drawing.Size(80, 17);
            this.metroToggle1.TabIndex = 4;
            this.metroToggle1.Text = "Off";
            this.metroToggle1.UseSelectable = true;
            this.metroToggle1.CheckedChanged += new System.EventHandler(this.metroToggle1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(367, 230);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Wprowadź wartości";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 297);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.metroToggle1);
            this.Controls.Add(this.port);
            this.Controls.Add(this.ip);
            this.Controls.Add(this.metroTextBox1);
            this.Controls.Add(this.metroButton1);
            this.Name = "Form1";
            this.Resizable = false;
            this.Text = "Aplikacja testowa do modułu ESP";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroButton metroButton1;
        private MetroFramework.Controls.MetroTextBox metroTextBox1;
        private MetroFramework.Controls.MetroTextBox ip;
        private MetroFramework.Controls.MetroTextBox port;
        private MetroFramework.Controls.MetroToggle metroToggle1;
        private System.Windows.Forms.Label label1;
    }
}

