namespace EntitiesToDTOs.UI
{
    partial class UpdateWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateWindow));
            this.txtMessage = new System.Windows.Forms.Label();
            this.btnLearnMore = new System.Windows.Forms.Button();
            this.btnRemindMeLater = new System.Windows.Forms.Button();
            this.btnNoThanks = new System.Windows.Forms.Button();
            this.flpMainContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.flpReleaseChanges = new System.Windows.Forms.FlowLayoutPanel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.flpMainContainer.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(3, 0);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(374, 19);
            this.txtMessage.TabIndex = 2;
            this.txtMessage.Text = "{txtMessage}";
            this.txtMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnLearnMore
            // 
            this.btnLearnMore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLearnMore.Location = new System.Drawing.Point(24, 3);
            this.btnLearnMore.Name = "btnLearnMore";
            this.btnLearnMore.Size = new System.Drawing.Size(105, 23);
            this.btnLearnMore.TabIndex = 3;
            this.btnLearnMore.Text = "Learn more!";
            this.btnLearnMore.UseVisualStyleBackColor = true;
            this.btnLearnMore.Click += new System.EventHandler(this.btnLearnMore_Click);
            // 
            // btnRemindMeLater
            // 
            this.btnRemindMeLater.Location = new System.Drawing.Point(135, 3);
            this.btnRemindMeLater.Name = "btnRemindMeLater";
            this.btnRemindMeLater.Size = new System.Drawing.Size(105, 23);
            this.btnRemindMeLater.TabIndex = 4;
            this.btnRemindMeLater.Text = "Remind me later";
            this.btnRemindMeLater.UseVisualStyleBackColor = true;
            this.btnRemindMeLater.Click += new System.EventHandler(this.btnRemindMeLater_Click);
            // 
            // btnNoThanks
            // 
            this.btnNoThanks.Location = new System.Drawing.Point(246, 3);
            this.btnNoThanks.Name = "btnNoThanks";
            this.btnNoThanks.Size = new System.Drawing.Size(105, 23);
            this.btnNoThanks.TabIndex = 5;
            this.btnNoThanks.Text = "No thanks";
            this.btnNoThanks.UseVisualStyleBackColor = true;
            this.btnNoThanks.Click += new System.EventHandler(this.btnNoThanks_Click);
            // 
            // flpMainContainer
            // 
            this.flpMainContainer.AutoSize = true;
            this.flpMainContainer.Controls.Add(this.txtMessage);
            this.flpMainContainer.Controls.Add(this.flpReleaseChanges);
            this.flpMainContainer.Controls.Add(this.panelButtons);
            this.flpMainContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpMainContainer.Location = new System.Drawing.Point(12, 12);
            this.flpMainContainer.Name = "flpMainContainer";
            this.flpMainContainer.Size = new System.Drawing.Size(380, 60);
            this.flpMainContainer.TabIndex = 7;
            // 
            // flpReleaseChanges
            // 
            this.flpReleaseChanges.AutoSize = true;
            this.flpReleaseChanges.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpReleaseChanges.Location = new System.Drawing.Point(3, 22);
            this.flpReleaseChanges.Name = "flpReleaseChanges";
            this.flpReleaseChanges.Size = new System.Drawing.Size(0, 0);
            this.flpReleaseChanges.TabIndex = 3;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnLearnMore);
            this.panelButtons.Controls.Add(this.btnRemindMeLater);
            this.panelButtons.Controls.Add(this.btnNoThanks);
            this.panelButtons.Location = new System.Drawing.Point(3, 28);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(374, 29);
            this.panelButtons.TabIndex = 8;
            // 
            // UpdateWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(404, 79);
            this.Controls.Add(this.flpMainContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "{Window_Caption}";
            this.flpMainContainer.ResumeLayout(false);
            this.flpMainContainer.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txtMessage;
        private System.Windows.Forms.Button btnLearnMore;
        private System.Windows.Forms.Button btnRemindMeLater;
        private System.Windows.Forms.Button btnNoThanks;
        private System.Windows.Forms.FlowLayoutPanel flpMainContainer;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.FlowLayoutPanel flpReleaseChanges;
    }
}