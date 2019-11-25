namespace Draeger.Testautomation.CredentialsManagerRemote
{
    partial class UserCreateEditDialog
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
            this.lUsername = new System.Windows.Forms.Label();
            this.lPassword = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.gbUserGroup = new System.Windows.Forms.GroupBox();
            this.rbUnassigned = new System.Windows.Forms.RadioButton();
            this.rbService = new System.Windows.Forms.RadioButton();
            this.rbSales = new System.Windows.Forms.RadioButton();
            this.rbAdmin = new System.Windows.Forms.RadioButton();
            this.gbUserGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // lUsername
            // 
            this.lUsername.AutoSize = true;
            this.lUsername.Location = new System.Drawing.Point(22, 35);
            this.lUsername.Name = "lUsername";
            this.lUsername.Size = new System.Drawing.Size(55, 13);
            this.lUsername.TabIndex = 0;
            this.lUsername.Text = "Username";
            // 
            // lPassword
            // 
            this.lPassword.AutoSize = true;
            this.lPassword.Location = new System.Drawing.Point(22, 61);
            this.lPassword.Name = "lPassword";
            this.lPassword.Size = new System.Drawing.Size(53, 13);
            this.lPassword.TabIndex = 1;
            this.lPassword.Text = "Password";
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(87, 32);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(100, 20);
            this.tbUsername.TabIndex = 2;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(87, 58);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(100, 20);
            this.tbPassword.TabIndex = 3;
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCreate.Location = new System.Drawing.Point(123, 207);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.Text = "OK";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // gbUserGroup
            // 
            this.gbUserGroup.Controls.Add(this.rbUnassigned);
            this.gbUserGroup.Controls.Add(this.rbService);
            this.gbUserGroup.Controls.Add(this.rbSales);
            this.gbUserGroup.Controls.Add(this.rbAdmin);
            this.gbUserGroup.Location = new System.Drawing.Point(25, 84);
            this.gbUserGroup.Name = "gbUserGroup";
            this.gbUserGroup.Size = new System.Drawing.Size(162, 117);
            this.gbUserGroup.TabIndex = 5;
            this.gbUserGroup.TabStop = false;
            this.gbUserGroup.Text = "User Group";
            // 
            // rbUnassigned
            // 
            this.rbUnassigned.AutoSize = true;
            this.rbUnassigned.Checked = true;
            this.rbUnassigned.Location = new System.Drawing.Point(6, 88);
            this.rbUnassigned.Name = "rbUnassigned";
            this.rbUnassigned.Size = new System.Drawing.Size(81, 17);
            this.rbUnassigned.TabIndex = 3;
            this.rbUnassigned.TabStop = true;
            this.rbUnassigned.Text = "Unassigned";
            this.rbUnassigned.UseVisualStyleBackColor = true;
            this.rbUnassigned.CheckedChanged += new System.EventHandler(this.rbUnassigned_CheckedChanged);
            // 
            // rbService
            // 
            this.rbService.AutoSize = true;
            this.rbService.Location = new System.Drawing.Point(6, 65);
            this.rbService.Name = "rbService";
            this.rbService.Size = new System.Drawing.Size(61, 17);
            this.rbService.TabIndex = 2;
            this.rbService.TabStop = true;
            this.rbService.Text = "Service";
            this.rbService.UseVisualStyleBackColor = true;
            this.rbService.CheckedChanged += new System.EventHandler(this.rbService_CheckedChanged);
            // 
            // rbSales
            // 
            this.rbSales.AutoSize = true;
            this.rbSales.Location = new System.Drawing.Point(6, 42);
            this.rbSales.Name = "rbSales";
            this.rbSales.Size = new System.Drawing.Size(51, 17);
            this.rbSales.TabIndex = 1;
            this.rbSales.TabStop = true;
            this.rbSales.Text = "Sales";
            this.rbSales.UseVisualStyleBackColor = true;
            this.rbSales.CheckedChanged += new System.EventHandler(this.rbSales_CheckedChanged);
            // 
            // rbAdmin
            // 
            this.rbAdmin.AutoSize = true;
            this.rbAdmin.Location = new System.Drawing.Point(6, 19);
            this.rbAdmin.Name = "rbAdmin";
            this.rbAdmin.Size = new System.Drawing.Size(54, 17);
            this.rbAdmin.TabIndex = 0;
            this.rbAdmin.TabStop = true;
            this.rbAdmin.Text = "Admin";
            this.rbAdmin.UseVisualStyleBackColor = true;
            this.rbAdmin.CheckedChanged += new System.EventHandler(this.rbAdmin_CheckedChanged);
            // 
            // UserCreateEditDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(210, 242);
            this.Controls.Add(this.gbUserGroup);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbUsername);
            this.Controls.Add(this.lPassword);
            this.Controls.Add(this.lUsername);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "UserCreateEditDialog";
            this.Text = "Enter credentials";
            this.gbUserGroup.ResumeLayout(false);
            this.gbUserGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lUsername;
        private System.Windows.Forms.Label lPassword;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.GroupBox gbUserGroup;
        private System.Windows.Forms.RadioButton rbService;
        private System.Windows.Forms.RadioButton rbSales;
        private System.Windows.Forms.RadioButton rbAdmin;
        private System.Windows.Forms.RadioButton rbUnassigned;
    }
}