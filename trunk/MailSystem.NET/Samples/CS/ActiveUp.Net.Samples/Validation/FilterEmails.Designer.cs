namespace ActiveUp.Net.Samples.Validation
{
    partial class FilterEmails
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
            this._tbPop3Server = new System.Windows.Forms.TextBox();
            this._lUserName = new System.Windows.Forms.Label();
            this._lPop3Server = new System.Windows.Forms.Label();
            this._lPassword = new System.Windows.Forms.Label();
            this._bFilterEmails = new System.Windows.Forms.Button();
            this._tbPassword = new System.Windows.Forms.TextBox();
            this._tbUserName = new System.Windows.Forms.TextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._tbPop3Server);
            this.splitContainer1.Panel1.Controls.Add(this._lUserName);
            this.splitContainer1.Panel1.Controls.Add(this._lPop3Server);
            this.splitContainer1.Panel1.Controls.Add(this._lPassword);
            this.splitContainer1.Panel1.Controls.Add(this._bFilterEmails);
            this.splitContainer1.Panel1.Controls.Add(this._tbPassword);
            this.splitContainer1.Panel1.Controls.Add(this._tbUserName);
            // 
            // _tbPop3Server
            // 
            this._tbPop3Server.Location = new System.Drawing.Point(15, 104);
            this._tbPop3Server.Name = "_tbPop3Server";
            this._tbPop3Server.Size = new System.Drawing.Size(270, 20);
            this._tbPop3Server.TabIndex = 12;
            // 
            // _lUserName
            // 
            this._lUserName.AutoSize = true;
            this._lUserName.Location = new System.Drawing.Point(12, 10);
            this._lUserName.Name = "_lUserName";
            this._lUserName.Size = new System.Drawing.Size(61, 13);
            this._lUserName.TabIndex = 7;
            this._lUserName.Text = "User name:";
            // 
            // _lPop3Server
            // 
            this._lPop3Server.AutoSize = true;
            this._lPop3Server.Location = new System.Drawing.Point(12, 88);
            this._lPop3Server.Name = "_lPop3Server";
            this._lPop3Server.Size = new System.Drawing.Size(244, 13);
            this._lPop3Server.TabIndex = 11;
            this._lPop3Server.Text = "POP3 server address (will use 110 as default port):";
            // 
            // _lPassword
            // 
            this._lPassword.AutoSize = true;
            this._lPassword.Location = new System.Drawing.Point(12, 49);
            this._lPassword.Name = "_lPassword";
            this._lPassword.Size = new System.Drawing.Size(56, 13);
            this._lPassword.TabIndex = 9;
            this._lPassword.Text = "Password:";
            // 
            // _bFilterEmails
            // 
            this._bFilterEmails.Location = new System.Drawing.Point(15, 178);
            this._bFilterEmails.Name = "_bFilterEmails";
            this._bFilterEmails.Size = new System.Drawing.Size(270, 36);
            this._bFilterEmails.TabIndex = 13;
            this._bFilterEmails.Text = "Filter emails";
            this._bFilterEmails.UseVisualStyleBackColor = true;
            this._bFilterEmails.Click += new System.EventHandler(this._bFilterEmails_Click);
            // 
            // _tbPassword
            // 
            this._tbPassword.Location = new System.Drawing.Point(15, 65);
            this._tbPassword.Name = "_tbPassword";
            this._tbPassword.Size = new System.Drawing.Size(270, 20);
            this._tbPassword.TabIndex = 10;
            // 
            // _tbUserName
            // 
            this._tbUserName.Location = new System.Drawing.Point(15, 26);
            this._tbUserName.Name = "_tbUserName";
            this._tbUserName.Size = new System.Drawing.Size(270, 20);
            this._tbUserName.TabIndex = 8;
            // 
            // FilterEmails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(910, 514);
            this.Name = "FilterEmails";
            this.Text = "Filter emails";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _tbPop3Server;
        private System.Windows.Forms.Label _lUserName;
        private System.Windows.Forms.Label _lPop3Server;
        private System.Windows.Forms.Label _lPassword;
        private System.Windows.Forms.Button _bFilterEmails;
        private System.Windows.Forms.TextBox _tbPassword;
        private System.Windows.Forms.TextBox _tbUserName;
    }
}
