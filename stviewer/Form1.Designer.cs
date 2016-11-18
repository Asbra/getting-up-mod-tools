namespace stviewer
{
    partial class formMain
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
            this.texture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.texture)).BeginInit();
            this.SuspendLayout();
            // 
            // texture
            // 
            this.texture.Location = new System.Drawing.Point(0, 0);
            this.texture.Name = "texture";
            this.texture.Size = new System.Drawing.Size(256, 256);
            this.texture.TabIndex = 0;
            this.texture.TabStop = false;
            // 
            // formMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Magenta;
            this.ClientSize = new System.Drawing.Size(256, 256);
            this.Controls.Add(this.texture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "formMain";
            this.Text = "ST viewer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.formMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.formMain_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.texture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox texture;
    }
}

