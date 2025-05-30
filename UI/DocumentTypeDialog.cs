namespace ContractPdfMerger.UI;

public partial class DocumentTypeDialog : Form
{
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public bool IsEditMode { get; set; } = false;

    public DocumentTypeDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        // Form settings
        Text = "分類情報";
        Size = new Size(400, 200);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        CreateControls();
        LayoutControls();

        ResumeLayout(false);
    }

    private void CreateControls()
    {
        lblTypeCode = new Label { Text = "分類コード:", AutoSize = true };
        txtTypeCode = new TextBox { Width = 200, MaxLength = 50 };

        lblTypeName = new Label { Text = "分類名:", AutoSize = true };
        txtTypeName = new TextBox { Width = 200, MaxLength = 100 };

        btnOK = new Button { Text = "OK", Size = new Size(80, 30), DialogResult = DialogResult.OK };
        btnCancel = new Button { Text = "キャンセル", Size = new Size(80, 30), DialogResult = DialogResult.Cancel };

        // Wire up events
        btnOK.Click += BtnOK_Click;
        Load += DocumentTypeDialog_Load;
    }

    private void LayoutControls()
    {
        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 2,
            Padding = new Padding(20)
        };

        mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        mainPanel.Controls.Add(lblTypeCode, 0, 0);
        mainPanel.Controls.Add(txtTypeCode, 1, 0);
        mainPanel.Controls.Add(lblTypeName, 0, 1);
        mainPanel.Controls.Add(txtTypeName, 1, 1);

        var buttonPanel = new Panel { Dock = DockStyle.Fill };
        buttonPanel.Controls.Add(btnOK);
        buttonPanel.Controls.Add(btnCancel);
        btnOK.Location = new Point(50, 5);
        btnCancel.Location = new Point(140, 5);
        mainPanel.Controls.Add(buttonPanel, 1, 2);

        Controls.Add(mainPanel);
    }

    private void DocumentTypeDialog_Load(object? sender, EventArgs e)
    {
        txtTypeCode.Text = TypeCode;
        txtTypeName.Text = TypeName;

        if (IsEditMode)
        {
            Text = "分類編集";
            txtTypeCode.ReadOnly = true;
            txtTypeCode.BackColor = SystemColors.Control;
        }
        else
        {
            Text = "分類追加";
        }

        txtTypeName.Focus();
    }

    private void BtnOK_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTypeCode.Text))
        {
            MessageBox.Show("分類コードを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTypeCode.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtTypeName.Text))
        {
            MessageBox.Show("分類名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTypeName.Focus();
            return;
        }

        TypeCode = txtTypeCode.Text.Trim();
        TypeName = txtTypeName.Text.Trim();
    }

    #region Control Declarations
    private Label lblTypeCode = null!;
    private TextBox txtTypeCode = null!;
    private Label lblTypeName = null!;
    private TextBox txtTypeName = null!;
    private Button btnOK = null!;
    private Button btnCancel = null!;
    #endregion
}