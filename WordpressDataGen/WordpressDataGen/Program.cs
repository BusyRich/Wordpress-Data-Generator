using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace WordpressDataGen
{
    class Program : System.Windows.Forms.Form
    {
        private RichTextBox output;
        private TextBox savePath;
        private Label label1;
        private Button openSaveDialog;
        private FolderBrowserDialog saveDialog;
        private Button generate;

        #region Do Not Modify

        Program()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.output = new System.Windows.Forms.RichTextBox();
            this.generate = new System.Windows.Forms.Button();
            this.savePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.saveDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.openSaveDialog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(12, 213);
            this.output.Name = "output";
            this.output.ReadOnly = true;
            this.output.Size = new System.Drawing.Size(478, 189);
            this.output.TabIndex = 0;
            this.output.Text = "";
            // 
            // generate
            // 
            this.generate.Location = new System.Drawing.Point(415, 184);
            this.generate.Name = "generate";
            this.generate.Size = new System.Drawing.Size(75, 23);
            this.generate.TabIndex = 1;
            this.generate.Text = "Generate";
            this.generate.UseVisualStyleBackColor = true;
            this.generate.Click += new System.EventHandler(this.generate_Click);
            // 
            // savePath
            // 
            this.savePath.Location = new System.Drawing.Point(12, 25);
            this.savePath.Name = "savePath";
            this.savePath.Size = new System.Drawing.Size(443, 20);
            this.savePath.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Save Path";
            // 
            // openSaveDialog
            // 
            this.openSaveDialog.Location = new System.Drawing.Point(461, 22);
            this.openSaveDialog.Name = "openSaveDialog";
            this.openSaveDialog.Size = new System.Drawing.Size(29, 23);
            this.openSaveDialog.TabIndex = 4;
            this.openSaveDialog.Text = "...";
            this.openSaveDialog.UseVisualStyleBackColor = true;
            this.openSaveDialog.Click += new System.EventHandler(this.openSaveDialog_Click);
            // 
            // Program
            // 
            this.ClientSize = new System.Drawing.Size(502, 414);
            this.Controls.Add(this.openSaveDialog);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.savePath);
            this.Controls.Add(this.generate);
            this.Controls.Add(this.output);
            this.Name = "Program";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //This attribute is required for the open dialog.
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program());
        }

        #endregion

        private void openSaveDialog_Click(object sender, EventArgs e)
        {
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                savePath.Text = saveDialog.SelectedPath;
            }
        }

        private void generate_Click(object sender, EventArgs e)
        {
            WPData wpd = new WPData();
            output.Text = wpd.GetDocumentAsString();
            wpd.SaveToFile(savePath.Text);
        }



    }
}
