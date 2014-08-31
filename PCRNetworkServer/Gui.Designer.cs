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
            this.groupBoxSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOnOff
            // 
            this.buttonOnOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.buttonOnOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOnOff.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOnOff.ForeColor = System.Drawing.Color.Black;
            this.buttonOnOff.Location = new System.Drawing.Point(151, 95);
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
            this.labelState.Location = new System.Drawing.Point(18, 99);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(129, 16);
            this.labelState.TabIndex = 2;
            this.labelState.Text = "Current Server State:";
            // 
            // labelStateDesc
            // 
            this.labelStateDesc.AutoSize = true;
            this.labelStateDesc.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStateDesc.Location = new System.Drawing.Point(68, 112);
            this.labelStateDesc.Name = "labelStateDesc";
            this.labelStateDesc.Size = new System.Drawing.Size(77, 15);
            this.labelStateDesc.TabIndex = 3;
            this.labelStateDesc.Text = "(Click to change)";
            // 
            // labelSerial
            // 
            this.labelSerial.AutoSize = true;
            this.labelSerial.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerial.Location = new System.Drawing.Point(21, 20);
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
            this.groupBoxSettings.Location = new System.Drawing.Point(8, 7);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(229, 78);
            this.groupBoxSettings.TabIndex = 9;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // Gui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 140);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.labelStateDesc);
            this.Controls.Add(this.labelState);
            this.Controls.Add(this.buttonOnOff);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Gui";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server Control";
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
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
    }
}