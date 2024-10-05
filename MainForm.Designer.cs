namespace PaltalkNickReader
{
    partial class MainForm
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
            this.lstNicks = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClearNickList = new System.Windows.Forms.Button();
            this.btnReadNick = new System.Windows.Forms.Button();
            this.btnGetPaltalkWindows = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstNicks
            // 
            this.lstNicks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstNicks.FormattingEnabled = true;
            this.lstNicks.Location = new System.Drawing.Point(3, 12);
            this.lstNicks.Name = "lstNicks";
            this.lstNicks.Size = new System.Drawing.Size(584, 485);
            this.lstNicks.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btnClearNickList);
            this.panel1.Controls.Add(this.btnReadNick);
            this.panel1.Controls.Add(this.btnGetPaltalkWindows);
            this.panel1.Location = new System.Drawing.Point(0, 499);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(587, 50);
            this.panel1.TabIndex = 1;
            // 
            // btnClearNickList
            // 
            this.btnClearNickList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearNickList.Location = new System.Drawing.Point(425, 11);
            this.btnClearNickList.Name = "btnClearNickList";
            this.btnClearNickList.Size = new System.Drawing.Size(150, 30);
            this.btnClearNickList.TabIndex = 2;
            this.btnClearNickList.Text = "Clear Nicks List";
            this.btnClearNickList.UseVisualStyleBackColor = true;
            this.btnClearNickList.Click += new System.EventHandler(this.btnClearNickList_Click);
            // 
            // btnReadNick
            // 
            this.btnReadNick.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnReadNick.Location = new System.Drawing.Point(228, 11);
            this.btnReadNick.Name = "btnReadNick";
            this.btnReadNick.Size = new System.Drawing.Size(150, 30);
            this.btnReadNick.TabIndex = 1;
            this.btnReadNick.Text = "Read Nicknames";
            this.btnReadNick.UseVisualStyleBackColor = true;
            this.btnReadNick.Click += new System.EventHandler(this.btnReadNick_Click);
            // 
            // btnGetPaltalkWindows
            // 
            this.btnGetPaltalkWindows.Location = new System.Drawing.Point(12, 11);
            this.btnGetPaltalkWindows.Name = "btnGetPaltalkWindows";
            this.btnGetPaltalkWindows.Size = new System.Drawing.Size(150, 30);
            this.btnGetPaltalkWindows.TabIndex = 0;
            this.btnGetPaltalkWindows.Text = "Get Paltalk Windows";
            this.btnGetPaltalkWindows.UseVisualStyleBackColor = true;
            this.btnGetPaltalkWindows.Click += new System.EventHandler(this.btnGetPaltalkWindows_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 550);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lstNicks);
            this.Name = "MainForm";
            this.Text = "Read Paltalk Nicks";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstNicks;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnReadNick;
        private System.Windows.Forms.Button btnGetPaltalkWindows;
        private System.Windows.Forms.Button btnClearNickList;
    }
}

