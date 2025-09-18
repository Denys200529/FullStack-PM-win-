namespace YouTubeSearchApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtQuery;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ListBox lstResults;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.Button btnClearDb;
        private System.Windows.Forms.FlowLayoutPanel flowPanel; 

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtQuery = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lstResults = new System.Windows.Forms.ListBox();
            this.btnHistory = new System.Windows.Forms.Button();
            this.btnClearDb = new System.Windows.Forms.Button();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel(); 
            this.SuspendLayout();
           

            this.txtQuery.Location = new System.Drawing.Point(12, 12);
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(400, 23);
            this.txtQuery.TabIndex = 0;
             
            this.btnSearch.Location = new System.Drawing.Point(418, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Пошук";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      
            this.lstResults.FormattingEnabled = true;
            this.lstResults.ItemHeight = 15;
            this.lstResults.Location = new System.Drawing.Point(12, 50);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new System.Drawing.Size(506, 229);
            this.lstResults.TabIndex = 2;
            
            this.btnHistory.Location = new System.Drawing.Point(12, 300);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(150, 30);
            this.btnHistory.TabIndex = 3;
            this.btnHistory.Text = "Історія запитів";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
         
            this.btnClearDb.Location = new System.Drawing.Point(368, 300);
            this.btnClearDb.Name = "btnClearDb";
            this.btnClearDb.Size = new System.Drawing.Size(150, 30);
            this.btnClearDb.TabIndex = 4;
            this.btnClearDb.Text = "Очистити БД";
            this.btnClearDb.UseVisualStyleBackColor = true;
            this.btnClearDb.Click += new System.EventHandler(this.btnClearDb_Click);
       
            this.flowPanel.Location = new System.Drawing.Point(12, 50);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(506, 229);
            this.flowPanel.AutoScroll = true;
            this.flowPanel.WrapContents = false;
            this.flowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel.TabIndex = 5;
           
            this.ClientSize = new System.Drawing.Size(534, 341);
            this.Controls.Add(this.flowPanel); 
            this.Controls.Add(this.btnClearDb);
            this.Controls.Add(this.btnHistory);
            this.Controls.Add(this.lstResults);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtQuery);
            this.Name = "Form1";
            this.Text = "YouTube Search App";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
