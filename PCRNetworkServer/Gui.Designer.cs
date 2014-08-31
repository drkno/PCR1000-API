namespace PCRNetworkServer
{
    partial class Gui
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
            this.buttonOnOff = new System.Windows.Forms.Button();
            this.comboBoxSerialPort = new System.Windows.Forms.ComboBox();
            this.labelState = new System.Windows.Forms.Label();
            this.labelStateDesc = new System.Windows.Forms.Label();
            this.labelSerial = new System.Windows.Forms.Label();
            this.labelNetwork = new System.Windows.Forms.Label();
            this.textBoxNetwork = new System.Windows.Forms.TextBox();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelUseEnc = new System.Windows.Forms.Label();
            this.checkBoxUseSecurity = new System.Windows.Forms.CheckBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelEncrypt = new System.Windows.Forms.Label();
            this.checkBoxSsl = new System.Windows.Forms.CheckBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.groupBoxSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOnOff
            // 
            this.buttonOnOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.buttonOnOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOnOff.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOnOff.ForeColor = System.Drawing.Color.Black;
            this.buttonOnOff.Location = new System.Drawing.Point(147, 187);
            this.buttonOnOff.Name = "buttonOnOff";
            this.buttonOnOff.Size = new System.Drawing.Size(75, 35);
            this.buttonOnOff.TabIndex = 0;
            this.buttonOnOff.Text = "OFF";
            this.buttonOnOff.UseVisualStyleBackColor = false;
            this.buttonOnOff.Click += new System.EventHandler(this.ButtonOnOffClick);
            // 
            // comboBoxSerialPort
            // 
            this.comboBoxSerialPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSerialPort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxSerialPort.FormattingEnabled = true;
            this.comboBoxSerialPort.Location = new System.Drawing.Point(98, 19);
            this.comboBoxSerialPort.Name = "comboBoxSerialPort";
            this.comboBoxSerialPort.Size = new System.Drawing.Size(121, 22);
            this.comboBoxSerialPort.TabIndex = 1;
            // 
            // labelState
            // 
            this.labelState.AutoSize = true;
            this.labelState.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelState.Location = new System.Drawing.Point(14, 191);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(129, 16);
            this.labelState.TabIndex = 2;
            this.labelState.Text = "Current Server State:";
            // 
            // labelStateDesc
            // 
            this.labelStateDesc.AutoSize = true;
            this.labelStateDesc.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStateDesc.Location = new System.Drawing.Point(64, 204);
            this.labelStateDesc.Name = "labelStateDesc";
            this.labelStateDesc.Size = new System.Drawing.Size(77, 15);
            this.labelStateDesc.TabIndex = 3;
            this.labelStateDesc.Text = "(Click to change)";
            // 
            // labelSerial
            // 
            this.labelSerial.AutoSize = true;
            this.labelSerial.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerial.Location = new System.Drawing.Point(21, 23);
            this.labelSerial.Name = "labelSerial";
            this.labelSerial.Size = new System.Drawing.Size(73, 16);
            this.labelSerial.TabIndex = 4;
            this.labelSerial.Text = "Serial Port:";
            // 
            // labelNetwork
            // 
            this.labelNetwork.AutoSize = true;
            this.labelNetwork.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNetwork.Location = new System.Drawing.Point(7, 48);
            this.labelNetwork.Name = "labelNetwork";
            this.labelNetwork.Size = new System.Drawing.Size(87, 16);
            this.labelNetwork.TabIndex = 5;
            this.labelNetwork.Text = "Network Port:";
            // 
            // textBoxNetwork
            // 
            this.textBoxNetwork.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxNetwork.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxNetwork.Location = new System.Drawing.Point(98, 46);
            this.textBoxNetwork.Name = "textBoxNetwork";
            this.textBoxNetwork.Size = new System.Drawing.Size(103, 21);
            this.textBoxNetwork.TabIndex = 7;
            this.textBoxNetwork.Text = "4456";
            this.textBoxNetwork.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.labelSerial);
            this.groupBoxSettings.Controls.Add(this.textBoxNetwork);
            this.groupBoxSettings.Controls.Add(this.comboBoxSerialPort);
            this.groupBoxSettings.Controls.Add(this.labelNetwork);
            this.groupBoxSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxSettings.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSettings.Location = new System.Drawing.Point(5, 9);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(229, 76);
            this.groupBoxSettings.TabIndex = 9;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelUseEnc);
            this.groupBox1.Controls.Add(this.checkBoxUseSecurity);
            this.groupBox1.Controls.Add(this.labelPassword);
            this.groupBox1.Controls.Add(this.labelEncrypt);
            this.groupBox1.Controls.Add(this.checkBoxSsl);
            this.groupBox1.Controls.Add(this.textBoxPassword);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(6, 91);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(228, 85);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Security";
            // 
            // labelUseEnc
            // 
            this.labelUseEnc.AutoSize = true;
            this.labelUseEnc.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUseEnc.Location = new System.Drawing.Point(6, 16);
            this.labelUseEnc.Name = "labelUseEnc";
            this.labelUseEnc.Size = new System.Drawing.Size(87, 16);
            this.labelUseEnc.TabIndex = 17;
            this.labelUseEnc.Text = "Use Security:";
            // 
            // checkBoxUseSecurity
            // 
            this.checkBoxUseSecurity.AutoSize = true;
            this.checkBoxUseSecurity.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxUseSecurity.Location = new System.Drawing.Point(97, 19);
            this.checkBoxUseSecurity.Name = "checkBoxUseSecurity";
            this.checkBoxUseSecurity.Size = new System.Drawing.Size(12, 11);
            this.checkBoxUseSecurity.TabIndex = 16;
            this.checkBoxUseSecurity.UseVisualStyleBackColor = true;
            this.checkBoxUseSecurity.CheckedChanged += new System.EventHandler(this.CheckBoxUseSecurityCheckedChanged);
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPassword.Location = new System.Drawing.Point(24, 59);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(69, 16);
            this.labelPassword.TabIndex = 15;
            this.labelPassword.Text = "Password:";
            // 
            // labelEncrypt
            // 
            this.labelEncrypt.AutoSize = true;
            this.labelEncrypt.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEncrypt.Location = new System.Drawing.Point(7, 37);
            this.labelEncrypt.Name = "labelEncrypt";
            this.labelEncrypt.Size = new System.Drawing.Size(86, 16);
            this.labelEncrypt.TabIndex = 14;
            this.labelEncrypt.Text = "SSL Encrypt:";
            // 
            // checkBoxSsl
            // 
            this.checkBoxSsl.AutoSize = true;
            this.checkBoxSsl.Enabled = false;
            this.checkBoxSsl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxSsl.Location = new System.Drawing.Point(97, 40);
            this.checkBoxSsl.Name = "checkBoxSsl";
            this.checkBoxSsl.Size = new System.Drawing.Size(12, 11);
            this.checkBoxSsl.TabIndex = 13;
            this.checkBoxSsl.UseVisualStyleBackColor = true;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxPassword.Enabled = false;
            this.textBoxPassword.Location = new System.Drawing.Point(97, 57);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(103, 20);
            this.textBoxPassword.TabIndex = 12;
            this.textBoxPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // Gui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 231);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.labelStateDesc);
            this.Controls.Add(this.labelState);
            this.Controls.Add(this.buttonOnOff);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Gui";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GuiFormClosing);
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOnOff;
        private System.Windows.Forms.ComboBox comboBoxSerialPort;
        private System.Windows.Forms.Label labelState;
        private System.Windows.Forms.Label labelStateDesc;
        private System.Windows.Forms.Label labelSerial;
        private System.Windows.Forms.Label labelNetwork;
        private System.Windows.Forms.TextBox textBoxNetwork;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelUseEnc;
        private System.Windows.Forms.CheckBox checkBoxUseSecurity;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelEncrypt;
        private System.Windows.Forms.CheckBox checkBoxSsl;
        private System.Windows.Forms.TextBox textBoxPassword;
    }
}