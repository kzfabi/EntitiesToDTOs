namespace EntitiesToDTOs.UI
{
    partial class RateReleaseWindow
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
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RateReleaseWindow));
            this.btnRate = new System.Windows.Forms.Button();
            this.btnRemindMeLater = new System.Windows.Forms.Button();
            this.btnAlreadyDid = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(59, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(257, 13);
            label1.TabIndex = 0;
            label1.Text = "Would you like to rate this release? It\'s fast and easy!";
            // 
            // btnRate
            // 
            this.btnRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRate.Location = new System.Drawing.Point(30, 33);
            this.btnRate.Name = "btnRate";
            this.btnRate.Size = new System.Drawing.Size(100, 23);
            this.btnRate.TabIndex = 1;
            this.btnRate.Text = "Rate!";
            this.btnRate.UseVisualStyleBackColor = true;
            this.btnRate.Click += new System.EventHandler(this.btnRate_Click);
            // 
            // btnRemindMeLater
            // 
            this.btnRemindMeLater.Location = new System.Drawing.Point(136, 33);
            this.btnRemindMeLater.Name = "btnRemindMeLater";
            this.btnRemindMeLater.Size = new System.Drawing.Size(100, 23);
            this.btnRemindMeLater.TabIndex = 2;
            this.btnRemindMeLater.Text = "Remind me later";
            this.btnRemindMeLater.UseVisualStyleBackColor = true;
            this.btnRemindMeLater.Click += new System.EventHandler(this.btnRemindMeLater_Click);
            // 
            // btnAlreadyDid
            // 
            this.btnAlreadyDid.Location = new System.Drawing.Point(242, 33);
            this.btnAlreadyDid.Name = "btnAlreadyDid";
            this.btnAlreadyDid.Size = new System.Drawing.Size(100, 23);
            this.btnAlreadyDid.TabIndex = 3;
            this.btnAlreadyDid.Text = "Already did";
            this.btnAlreadyDid.UseVisualStyleBackColor = true;
            this.btnAlreadyDid.Click += new System.EventHandler(this.btnAlreadyDid_Click);
            // 
            // RateReleaseWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 68);
            this.Controls.Add(this.btnAlreadyDid);
            this.Controls.Add(this.btnRemindMeLater);
            this.Controls.Add(this.btnRate);
            this.Controls.Add(label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RateReleaseWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "{Window_Caption}";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRate;
        private System.Windows.Forms.Button btnRemindMeLater;
        private System.Windows.Forms.Button btnAlreadyDid;
    }
}