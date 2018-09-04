namespace aplikacja_pc_do_esp
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.test = new MetroFramework.Controls.MetroTile();
            this.otworz = new MetroFramework.Controls.MetroTile();
            this.stop = new MetroFramework.Controls.MetroTile();
            this.zamknij = new MetroFramework.Controls.MetroTile();
            this.kolko = new MetroFramework.Controls.MetroProgressSpinner();
            this.time = new System.Windows.Forms.Timer(this.components);
            this.info = new MetroFramework.Controls.MetroLabel();
            this.infobox = new MetroFramework.Controls.MetroTextBox();
            this.more = new MetroFramework.Controls.MetroTile();
            this.SuspendLayout();
            // 
            // test
            // 
            this.test.ActiveControl = null;
            resources.ApplyResources(this.test, "test");
            this.test.Name = "test";
            this.test.UseSelectable = true;
            this.test.Click += new System.EventHandler(this.metroTile1_Click);
            // 
            // otworz
            // 
            this.otworz.ActiveControl = null;
            this.otworz.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.otworz, "otworz");
            this.otworz.Name = "otworz";
            this.otworz.UseSelectable = true;
            this.otworz.Click += new System.EventHandler(this.otworz_Click);
            // 
            // stop
            // 
            this.stop.ActiveControl = null;
            resources.ApplyResources(this.stop, "stop");
            this.stop.Name = "stop";
            this.stop.UseSelectable = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // zamknij
            // 
            this.zamknij.ActiveControl = null;
            resources.ApplyResources(this.zamknij, "zamknij");
            this.zamknij.Name = "zamknij";
            this.zamknij.UseSelectable = true;
            this.zamknij.Click += new System.EventHandler(this.zamknij_Click);
            // 
            // kolko
            // 
            resources.ApplyResources(this.kolko, "kolko");
            this.kolko.Maximum = 100;
            this.kolko.Name = "kolko";
            this.kolko.Speed = 0.001F;
            this.kolko.UseSelectable = true;
            this.kolko.UseStyleColors = true;
            this.kolko.Value = 15;
            // 
            // time
            // 
            this.time.Enabled = true;
            this.time.Interval = 120000;
            this.time.Tick += new System.EventHandler(this.time_Tick);
            // 
            // info
            // 
            resources.ApplyResources(this.info, "info");
            this.info.Name = "info";
            // 
            // infobox
            // 
            // 
            // 
            // 
            this.infobox.CustomButton.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.infobox.CustomButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resource.ImeMode")));
            this.infobox.CustomButton.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this.infobox.CustomButton.Name = "";
            this.infobox.CustomButton.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
            this.infobox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.infobox.CustomButton.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this.infobox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.infobox.CustomButton.UseSelectable = true;
            this.infobox.CustomButton.Visible = ((bool)(resources.GetObject("resource.Visible")));
            this.infobox.Lines = new string[0];
            resources.ApplyResources(this.infobox, "infobox");
            this.infobox.MaxLength = 32767;
            this.infobox.Multiline = true;
            this.infobox.Name = "infobox";
            this.infobox.PasswordChar = '\0';
            this.infobox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.infobox.SelectedText = "";
            this.infobox.SelectionLength = 0;
            this.infobox.SelectionStart = 0;
            this.infobox.UseSelectable = true;
            this.infobox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.infobox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // more
            // 
            this.more.ActiveControl = null;
            resources.ApplyResources(this.more, "more");
            this.more.Name = "more";
            this.more.UseSelectable = true;
            this.more.Click += new System.EventHandler(this.more_Click);
            // 
            // Main
            // 
            this.ApplyImageInvert = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.more);
            this.Controls.Add(this.info);
            this.Controls.Add(this.kolko);
            this.Controls.Add(this.zamknij);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.otworz);
            this.Controls.Add(this.test);
            this.Controls.Add(this.infobox);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Resizable = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTile test;
        private MetroFramework.Controls.MetroTile otworz;
        private MetroFramework.Controls.MetroTile stop;
        private MetroFramework.Controls.MetroTile zamknij;
        private MetroFramework.Controls.MetroProgressSpinner kolko;
        private System.Windows.Forms.Timer time;
        private MetroFramework.Controls.MetroLabel info;
        private MetroFramework.Controls.MetroTextBox infobox;
        private MetroFramework.Controls.MetroTile more;
    }
}