using ContractPdfMerger.Application;
using ContractPdfMerger.Domain;
using ContractPdfMerger.Infrastructure;
using Serilog;
using System.ComponentModel;

namespace ContractPdfMerger.UI;

public partial class AdminForm : Form
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly IPdfMergerService _pdfMergerService;

    private readonly BindingList<SupplementalDocumentView> _documents = new();
    private readonly BindingList<DocumentTypeView> _documentTypes = new();

    public AdminForm(IDocumentRepository documentRepository, IDocumentTypeRepository documentTypeRepository, IPdfMergerService pdfMergerService)
    {
        _documentRepository = documentRepository;
        _documentTypeRepository = documentTypeRepository;
        _pdfMergerService = pdfMergerService;
        InitializeComponent();
        SetupDataGrids();
        LoadData();
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        // Form settings
        Text = "管理画面";
        Size = new Size(1000, 700);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        CreateControls();
        LayoutControls();

        ResumeLayout(false);
    }

    private void CreateControls()
    {
        // Tab control
        tabControl = new TabControl { Dock = DockStyle.Fill };

        // Document management tab
        tabDocuments = new TabPage { Text = "付属書面管理" };
        lblDocumentsTitle = new Label { Text = "付属書面一覧", Font = new Font("MS UI Gothic", 9, FontStyle.Bold), AutoSize = true };
        dgvDocuments = new DataGridView { AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        btnAddDocument = new Button { Text = "追加", Size = new Size(80, 30) };
        btnReplaceDocument = new Button { Text = "差替え", Size = new Size(80, 30), Enabled = false };
        btnDeleteDocument = new Button { Text = "削除", Size = new Size(80, 30), Enabled = false };
        cmbDocumentType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };

        // Document types management tab
        tabDocumentTypes = new TabPage { Text = "分類マスタ管理" };
        lblDocumentTypesTitle = new Label { Text = "書面分類一覧", Font = new Font("MS UI Gothic", 9, FontStyle.Bold), AutoSize = true };
        dgvDocumentTypes = new DataGridView { AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        btnAddDocumentType = new Button { Text = "追加", Size = new Size(80, 30) };
        btnEditDocumentType = new Button { Text = "編集", Size = new Size(80, 30), Enabled = false };
        btnDeleteDocumentType = new Button { Text = "削除", Size = new Size(80, 30), Enabled = false };

        // Close button
        btnClose = new Button { Text = "閉じる", Size = new Size(80, 30), DialogResult = DialogResult.OK };

        // Wire up events
        btnAddDocument.Click += BtnAddDocument_Click;
        btnReplaceDocument.Click += BtnReplaceDocument_Click;
        btnDeleteDocument.Click += BtnDeleteDocument_Click;
        btnAddDocumentType.Click += BtnAddDocumentType_Click;
        btnEditDocumentType.Click += BtnEditDocumentType_Click;
        btnDeleteDocumentType.Click += BtnDeleteDocumentType_Click;
        dgvDocuments.SelectionChanged += DgvDocuments_SelectionChanged;
        dgvDocumentTypes.SelectionChanged += DgvDocumentTypes_SelectionChanged;

        tabControl.TabPages.Add(tabDocuments);
        tabControl.TabPages.Add(tabDocumentTypes);
    }

    private void LayoutControls()
    {
        // Documents tab layout
        var documentsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 2,
            Padding = new Padding(10)
        };

        documentsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        documentsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        documentsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        documentsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

        documentsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        documentsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));

        documentsPanel.Controls.Add(lblDocumentsTitle, 0, 0);
        documentsPanel.Controls.Add(dgvDocuments, 0, 1);

        var docTypePanel = new Panel { Dock = DockStyle.Fill };
        docTypePanel.Controls.Add(new Label { Text = "分類:", Location = new Point(0, 8), AutoSize = true });
        docTypePanel.Controls.Add(cmbDocumentType);
        cmbDocumentType.Location = new Point(40, 5);
        documentsPanel.Controls.Add(docTypePanel, 0, 2);

        var docButtonsPanel = new Panel { Dock = DockStyle.Fill };
        docButtonsPanel.Controls.Add(btnAddDocument);
        docButtonsPanel.Controls.Add(btnReplaceDocument);
        docButtonsPanel.Controls.Add(btnDeleteDocument);
        btnAddDocument.Location = new Point(0, 5);
        btnReplaceDocument.Location = new Point(90, 5);
        btnDeleteDocument.Location = new Point(180, 5);
        documentsPanel.Controls.Add(docButtonsPanel, 0, 3);

        tabDocuments.Controls.Add(documentsPanel);

        // Document types tab layout
        var typesPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            Padding = new Padding(10)
        };

        typesPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        typesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        typesPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

        typesPanel.Controls.Add(lblDocumentTypesTitle, 0, 0);
        typesPanel.Controls.Add(dgvDocumentTypes, 0, 1);

        var typeButtonsPanel = new Panel { Dock = DockStyle.Fill };
        typeButtonsPanel.Controls.Add(btnAddDocumentType);
        typeButtonsPanel.Controls.Add(btnEditDocumentType);
        typeButtonsPanel.Controls.Add(btnDeleteDocumentType);
        btnAddDocumentType.Location = new Point(0, 5);
        btnEditDocumentType.Location = new Point(90, 5);
        btnDeleteDocumentType.Location = new Point(180, 5);
        typesPanel.Controls.Add(typeButtonsPanel, 0, 2);

        tabDocumentTypes.Controls.Add(typesPanel);

        // Main layout
        var mainPanel = new Panel { Dock = DockStyle.Fill };
        var closePanel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
        closePanel.Controls.Add(btnClose);
        btnClose.Location = new Point(10, 10);

        mainPanel.Controls.Add(tabControl);
        Controls.Add(mainPanel);
        Controls.Add(closePanel);
    }

    private void SetupDataGrids()
    {
        // Documents grid
        dgvDocuments.DataSource = _documents;
        dgvDocuments.Columns.Clear();
        dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", HeaderText = "ID", DataPropertyName = "ID", Width = 50 });
        dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn { Name = "FileName", HeaderText = "ファイル名", DataPropertyName = "FileName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn { Name = "TypeName", HeaderText = "分類", DataPropertyName = "TypeName", Width = 120 });
        dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn { Name = "CreatedAt", HeaderText = "登録日", DataPropertyName = "CreatedAtString", Width = 100 });

        // Document types grid
        dgvDocumentTypes.DataSource = _documentTypes;
        dgvDocumentTypes.Columns.Clear();
        dgvDocumentTypes.Columns.Add(new DataGridViewTextBoxColumn { Name = "TypeCode", HeaderText = "分類コード", DataPropertyName = "TypeCode", Width = 120 });
        dgvDocumentTypes.Columns.Add(new DataGridViewTextBoxColumn { Name = "TypeName", HeaderText = "分類名", DataPropertyName = "TypeName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
    }

    private async void LoadData()
    {
        await LoadDocuments();
        await LoadDocumentTypes();
        await LoadDocumentTypeComboBox();
    }

    private async Task LoadDocuments()
    {
        try
        {
            var documents = await _documentRepository.GetAllAsync();
            _documents.Clear();
            foreach (var doc in documents)
            {
                _documents.Add(new SupplementalDocumentView
                {
                    ID = doc.ID,
                    FileName = doc.FileName,
                    TypeName = doc.DocumentType?.TypeName ?? "未分類",
                    CreatedAt = doc.CreatedAt,
                    FileContent = doc.FileContent
                });
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to load documents");
            MessageBox.Show($"書面の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadDocumentTypes()
    {
        try
        {
            var types = await _documentTypeRepository.GetAllAsync();
            _documentTypes.Clear();
            foreach (var type in types)
            {
                _documentTypes.Add(new DocumentTypeView
                {
                    TypeCode = type.TypeCode,
                    TypeName = type.TypeName
                });
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to load document types");
            MessageBox.Show($"分類の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadDocumentTypeComboBox()
    {
        try
        {
            var types = await _documentTypeRepository.GetAllAsync();
            cmbDocumentType.DataSource = types;
            cmbDocumentType.DisplayMember = "TypeName";
            cmbDocumentType.ValueMember = "TypeCode";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to load document types for combo box");
        }
    }

    private async void BtnAddDocument_Click(object? sender, EventArgs e)
    {
        if (cmbDocumentType.SelectedValue == null)
        {
            MessageBox.Show("分類を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var openFileDialog = new OpenFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            Title = "追加するPDFファイルを選択してください"
        };

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            if (_pdfMergerService.ValidatePdfFile(openFileDialog.FileName, out string errorMessage))
            {
                try
                {
                    var fileContent = await File.ReadAllBytesAsync(openFileDialog.FileName);
                    var fileName = Path.GetFileName(openFileDialog.FileName);
                    var typeCode = cmbDocumentType.SelectedValue.ToString()!;

                    var document = new SupplementalDocument
                    {
                        FileName = fileName,
                        TypeCode = typeCode,
                        FileContent = fileContent,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _documentRepository.AddAsync(document);
                    await LoadDocuments();
                    MessageBox.Show("書面を追加しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to add document");
                    MessageBox.Show($"書面の追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(errorMessage, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnReplaceDocument_Click(object? sender, EventArgs e)
    {
        if (dgvDocuments.SelectedRows.Count == 0) return;

        var selectedDoc = (SupplementalDocumentView)dgvDocuments.SelectedRows[0].DataBoundItem;

        using var openFileDialog = new OpenFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            Title = "差替えるPDFファイルを選択してください"
        };

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            if (_pdfMergerService.ValidatePdfFile(openFileDialog.FileName, out string errorMessage))
            {
                try
                {
                    var document = await _documentRepository.GetByIdAsync(selectedDoc.ID);
                    if (document != null)
                    {
                        document.FileContent = await File.ReadAllBytesAsync(openFileDialog.FileName);
                        document.FileName = Path.GetFileName(openFileDialog.FileName);

                        await _documentRepository.UpdateAsync(document);
                        await LoadDocuments();
                        MessageBox.Show("書面を差替えました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to replace document");
                    MessageBox.Show($"書面の差替えに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(errorMessage, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnDeleteDocument_Click(object? sender, EventArgs e)
    {
        if (dgvDocuments.SelectedRows.Count == 0) return;

        var selectedDoc = (SupplementalDocumentView)dgvDocuments.SelectedRows[0].DataBoundItem;

        if (MessageBox.Show($"書面「{selectedDoc.FileName}」を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                await _documentRepository.DeleteAsync(selectedDoc.ID);
                await LoadDocuments();
                MessageBox.Show("書面を削除しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete document");
                MessageBox.Show($"書面の削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnAddDocumentType_Click(object? sender, EventArgs e)
    {
        using var dialog = new DocumentTypeDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var documentType = new DocumentType
                {
                    TypeCode = dialog.TypeCode,
                    TypeName = dialog.TypeName
                };

                await _documentTypeRepository.AddAsync(documentType);
                await LoadDocumentTypes();
                await LoadDocumentTypeComboBox();
                MessageBox.Show("分類を追加しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add document type");
                MessageBox.Show($"分類の追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnEditDocumentType_Click(object? sender, EventArgs e)
    {
        if (dgvDocumentTypes.SelectedRows.Count == 0) return;

        var selectedType = (DocumentTypeView)dgvDocumentTypes.SelectedRows[0].DataBoundItem;

        using var dialog = new DocumentTypeDialog
        {
            TypeCode = selectedType.TypeCode,
            TypeName = selectedType.TypeName,
            IsEditMode = true
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var documentType = await _documentTypeRepository.GetByCodeAsync(selectedType.TypeCode);
                if (documentType != null)
                {
                    documentType.TypeName = dialog.TypeName;
                    await _documentTypeRepository.UpdateAsync(documentType);
                    await LoadDocumentTypes();
                    await LoadDocumentTypeComboBox();
                    MessageBox.Show("分類を更新しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update document type");
                MessageBox.Show($"分類の更新に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnDeleteDocumentType_Click(object? sender, EventArgs e)
    {
        if (dgvDocumentTypes.SelectedRows.Count == 0) return;

        var selectedType = (DocumentTypeView)dgvDocumentTypes.SelectedRows[0].DataBoundItem;

        // Check if type is in use
        if (await _documentTypeRepository.IsTypeInUseAsync(selectedType.TypeCode))
        {
            MessageBox.Show("この分類は使用中のため削除できません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show($"分類「{selectedType.TypeName}」を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                await _documentTypeRepository.DeleteAsync(selectedType.TypeCode);
                await LoadDocumentTypes();
                await LoadDocumentTypeComboBox();
                MessageBox.Show("分類を削除しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete document type");
                MessageBox.Show($"分類の削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void DgvDocuments_SelectionChanged(object? sender, EventArgs e)
    {
        bool hasSelection = dgvDocuments.SelectedRows.Count > 0;
        btnReplaceDocument.Enabled = hasSelection;
        btnDeleteDocument.Enabled = hasSelection;
    }

    private void DgvDocumentTypes_SelectionChanged(object? sender, EventArgs e)
    {
        bool hasSelection = dgvDocumentTypes.SelectedRows.Count > 0;
        btnEditDocumentType.Enabled = hasSelection;
        btnDeleteDocumentType.Enabled = hasSelection;
    }

    #region Control Declarations
    private TabControl tabControl = null!;
    private TabPage tabDocuments = null!;
    private TabPage tabDocumentTypes = null!;
    private Label lblDocumentsTitle = null!;
    private DataGridView dgvDocuments = null!;
    private Button btnAddDocument = null!;
    private Button btnReplaceDocument = null!;
    private Button btnDeleteDocument = null!;
    private ComboBox cmbDocumentType = null!;
    private Label lblDocumentTypesTitle = null!;
    private DataGridView dgvDocumentTypes = null!;
    private Button btnAddDocumentType = null!;
    private Button btnEditDocumentType = null!;
    private Button btnDeleteDocumentType = null!;
    private Button btnClose = null!;
    #endregion
}

// View model for document types
public class DocumentTypeView
{
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
}