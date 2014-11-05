namespace ControllerTestTool
{
    partial class Form1
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
            this.ControllersListbox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ControllersListbox
            // 
            this.ControllersListbox.FormattingEnabled = true;
            this.ControllersListbox.Location = new System.Drawing.Point(12, 12);
            this.ControllersListbox.Name = "ControllersListbox";
            this.ControllersListbox.Size = new System.Drawing.Size(194, 277);
            this.ControllersListbox.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 298);
            this.Controls.Add(this.ControllersListbox);
            this.Name = "Form1";
            this.Text = "Controller Test Tool v0.1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ControllersListbox;
    }
}

