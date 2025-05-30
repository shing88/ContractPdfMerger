namespace ContractPdfMerger.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblMainPdfTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSelectMainPdf = new System.Windows.Forms.Button();
            this.dgvMainPdf = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblSupplementalTitle = new System.Windows.Forms.Label();
            this.dgvSupplemental = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnAddToMerge = new System.Windows.Forms.Button();
            this.btnSelectLocalFile = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.lblMergeTargetsTitle = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMerge = new System.Windows.Forms.Button();
            this.dgvMergeTargets = new System.Windows.Forms.DataGridView();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblPreview = new System.Windows.Forms.Label();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.pnlPreviewDisplay = new System.Windows.Forms.Panel();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.pnlPageNavigation = new System.Windows.Forms.Panel();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.tabAdmin = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMainPdf)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSupplemental)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMergeTargets)).BeginInit();
            this.panel5.SuspendLayout();
            this.pnlPreview.SuspendLayout();
            this.pnlPreviewDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.pnlPageNavigation.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabMain);
            this.tabControl.Controls.Add(this.tabAdmin);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Size = new System.Drawing.Size(1200, 800);
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tableLayoutPanel1);
            this.tabMain.Text = "メイン画面";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.Controls.Add(this.panel5, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMainPdfTitle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.SetRowSpan(this.panel5, 5);
            // 
            // lblMainPdfTitle
            // 
            this.lblMainPdfTitle.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.lblMainPdfTitle.Text = "契約書面";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dgvMainPdf);
            this.panel1.Controls.Add(this.btnSelectMainPdf);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // btnSelectMainPdf
            // 
            this.btnSelectMainPdf.Location = new System.Drawing.Point(0, 5);
            this.btnSelectMainPdf.Size = new System.Drawing.Size(150, 30);
            this.btnSelectMainPdf.Text = "契約書面PDF選択";
            this.btnSelectMainPdf.Click += new System.EventHandler(this.BtnSelectMainPdf_Click);
            // 
            // dgvMainPdf
            // 
            this.dgvMainPdf.AllowUserToAddRows = false;
            this.dgvMainPdf.AllowUserToDeleteRows = false;
            this.dgvMainPdf.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMainPdf.Location = new System.Drawing.Point(0, 40);
            this.dgvMainPdf.ReadOnly = true;
            this.dgvMainPdf.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMainPdf.SelectionChanged += new System.EventHandler(this.DgvMainPdf_SelectionChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvSupplemental);
            this.panel2.Controls.Add(this.lblSupplementalTitle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // lblSupplementalTitle
            // 
            this.lblSupplementalTitle.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.lblSupplementalTitle.Text = "付属書面";
            // 
            // dgvSupplemental
            // 
            this.dgvSupplemental.AllowUserToAddRows = false;
            this.dgvSupplemental.AllowUserToDeleteRows = false;
            this.dgvSupplemental.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSupplemental.Location = new System.Drawing.Point(0, 25);
            this.dgvSupplemental.ReadOnly = true;
            this.dgvSupplemental.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSupplemental.SelectionChanged += new System.EventHandler(this.DgvSupplemental_SelectionChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnSelectLocalFile);
            this.panel3.Controls.Add(this.btnAddToMerge);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // btnAddToMerge
            // 
            this.btnAddToMerge.Enabled = false;
            this.btnAddToMerge.Location = new System.Drawing.Point(50, 10);
            this.btnAddToMerge.Size = new System.Drawing.Size(50, 30);
            this.btnAddToMerge.Text = "↓";
            this.btnAddToMerge.Click += new System.EventHandler(this.BtnAddToMerge_Click);
            // 
            // btnSelectLocalFile
            // 
            this.btnSelectLocalFile.Location = new System.Drawing.Point(120, 10);
            this.btnSelectLocalFile.Size = new System.Drawing.Size(150, 30);
            this.btnSelectLocalFile.Text = "ローカルファイル選択";
            this.btnSelectLocalFile.Click += new System.EventHandler(this.BtnSelectLocalFile_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel8);
            this.panel4.Controls.Add(this.panel7);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.lblMergeTargetsTitle);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Height = 25;
            // 
            // lblMergeTargetsTitle
            // 
            this.lblMergeTargetsTitle.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.lblMergeTargetsTitle.Location = new System.Drawing.Point(0, 5);
            this.lblMergeTargetsTitle.Text = "結合対象リスト";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.dgvMergeTargets);
            this.panel8.Controls.Add(this.panel9);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.btnMerge);
            this.panel9.Controls.Add(this.btnMoveDown);
            this.panel9.Controls.Add(this.btnMoveUp);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel9.Width = 120;
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Enabled = false;
            this.btnMoveUp.Location = new System.Drawing.Point(5, 5);
            this.btnMoveUp.Size = new System.Drawing.Size(30, 30);
            this.btnMoveUp.Text = "↑";
            this.btnMoveUp.Click += new System.EventHandler(this.BtnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Enabled = false;
            this.btnMoveDown.Location = new System.Drawing.Point(45, 5);
            this.btnMoveDown.Size = new System.Drawing.Size(30, 30);
            this.btnMoveDown.Text = "↓";
            this.btnMoveDown.Click += new System.EventHandler(this.BtnMoveDown_Click);
            // 
            // btnMerge
            // 
            this.btnMerge.Enabled = false;
            this.btnMerge.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.btnMerge.Location = new System.Drawing.Point(5, 50);
            this.btnMerge.Size = new System.Drawing.Size(100, 40);
            this.btnMerge.Text = "結合";
            this.btnMerge.Click += new System.EventHandler(this.BtnMerge_Click);
            // 
            // dgvMergeTargets
            // 
            this.dgvMergeTargets.AllowUserToAddRows = false;
            this.dgvMergeTargets.AllowUserToDeleteRows = false;
            this.dgvMergeTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMergeTargets.ReadOnly = true;
            this.dgvMergeTargets.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMergeTargets.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvMergeTargets_CellClick);
            this.dgvMergeTargets.SelectionChanged += new System.EventHandler(this.DgvMergeTargets_SelectionChanged);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.pnlPreview);
            this.panel5.Controls.Add(this.lblPreview);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // lblPreview
            // 
            this.lblPreview.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.lblPreview.Location = new System.Drawing.Point(5, 5);
            this.lblPreview.Text = "プレビュー";
            // 
            // pnlPreview
            // 
            this.pnlPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPreview.BackColor = System.Drawing.Color.White;
            this.pnlPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPreview.Controls.Add(this.pnlPreviewDisplay);
            this.pnlPreview.Controls.Add(this.pnlPageNavigation);
            this.pnlPreview.Location = new System.Drawing.Point(5, 30);
            // 
            // pnlPreviewDisplay
            // 
            this.pnlPreviewDisplay.BackColor = System.Drawing.Color.White;
            this.pnlPreviewDisplay.Controls.Add(this.webBrowser);
            this.pnlPreviewDisplay.Controls.Add(this.picPreview);
            this.pnlPreviewDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // picPreview
            // 
            this.picPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Visible = false;
            // 
            // pnlPageNavigation
            // 
            this.pnlPageNavigation.BackColor = System.Drawing.Color.LightGray;
            this.pnlPageNavigation.Controls.Add(this.lblPageInfo);
            this.pnlPageNavigation.Controls.Add(this.btnNextPage);
            this.pnlPageNavigation.Controls.Add(this.btnPrevPage);
            this.pnlPageNavigation.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPageNavigation.Height = 35;
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.Enabled = false;
            this.btnPrevPage.Location = new System.Drawing.Point(5, 5);
            this.btnPrevPage.Size = new System.Drawing.Size(30, 25);
            this.btnPrevPage.Text = "◀";
            this.btnPrevPage.Click += new System.EventHandler(this.BtnPrevPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.Enabled = false;
            this.btnNextPage.Location = new System.Drawing.Point(40, 5);
            this.btnNextPage.Size = new System.Drawing.Size(30, 25);
            this.btnNextPage.Text = "▶";
            this.btnNextPage.Click += new System.EventHandler(this.BtnNextPage_Click);
            // 
            // lblPageInfo
            // 
            this.lblPageInfo.Location = new System.Drawing.Point(80, 8);
            this.lblPageInfo.Text = "1 / 1";
            // 
            // tabAdmin
            // 
            this.tabAdmin.Text = "管理画面";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.tabControl);
            this.MinimumSize = new System.Drawing.Size(1280, 720);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "契約書類PDF結合ツール";
            this.tabControl.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMainPdf)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSupplemental)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMergeTargets)).EndInit();
            this.panel5.ResumeLayout(false);
            this.pnlPreview.ResumeLayout(false);
            this.pnlPreviewDisplay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.pnlPageNavigation.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.TabPage tabAdmin;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblMainPdfTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSelectMainPdf;
        private System.Windows.Forms.DataGridView dgvMainPdf;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblSupplementalTitle;
        private System.Windows.Forms.DataGridView dgvSupplemental;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnAddToMerge;
        private System.Windows.Forms.Button btnSelectLocalFile;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label lblMergeTargetsTitle;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.DataGridView dgvMergeTargets;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.Panel pnlPreview;
        private System.Windows.Forms.Panel pnlPreviewDisplay;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Panel pnlPageNavigation;
        private System.Windows.Forms.Button btnPrevPage;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Label lblPageInfo;
    }
}