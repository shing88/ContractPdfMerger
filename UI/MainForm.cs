using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ContractPdfMerger.Application;
using ContractPdfMerger.Domain;
using ContractPdfMerger.Infrastructure;

namespace ContractPdfMerger.UI
{
    public partial class MainForm : Form
    {
        private readonly IPdfMergerService _pdfMergerService;
        private readonly IDocumentRepository _documentRepository;
        private readonly IServiceProvider _serviceProvider;

        private byte[]? _mainPdfContent;
        private string _mainPdfFileName = string.Empty;
        private readonly BindingList<SupplementalDocumentView> _supplementalDocuments = new();
        private readonly BindingList<MergeTargetView> _mergeTargets = new();
        private readonly BindingList<MainPdfView> _mainPdfList = new();

        private byte[]? _currentPreviewPdf;
        private int _currentPageIndex = 0;
        private int _totalPages = 0;

        public MainForm(IPdfMergerService pdfMergerService, IDocumentRepository documentRepository, IServiceProvider serviceProvider)
        {
            _pdfMergerService = pdfMergerService;
            _documentRepository = documentRepository;
            _serviceProvider = serviceProvider;

            InitializeComponent();
            SetupDataGrids();
            LoadSupplementalDocuments();
            ShowTestPreview();
        }

        private void SetupDataGrids()
        {
            // Main PDF DataGrid
            dgvMainPdf.DataSource = _mainPdfList;
            dgvMainPdf.AutoGenerateColumns = false;
            dgvMainPdf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileName",
                HeaderText = "ファイル名",
                DataPropertyName = "FileName",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            });

            // Supplemental Documents DataGrid
            dgvSupplemental.DataSource = _supplementalDocuments;
            dgvSupplemental.AutoGenerateColumns = false;
            dgvSupplemental.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewCheckBoxColumn { Name = "Selected", HeaderText = "✓", Width = 30, DataPropertyName = "Selected" },
                new DataGridViewTextBoxColumn { Name = "TypeName", HeaderText = "書面分類", DataPropertyName = "TypeName", Width = 100, ReadOnly = true },
                new DataGridViewTextBoxColumn { Name = "FileName", HeaderText = "ファイル名", DataPropertyName = "FileName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true },
                new DataGridViewTextBoxColumn { Name = "CreatedAt", HeaderText = "登録日", DataPropertyName = "CreatedAtString", Width = 100, ReadOnly = true }
            });

            // Merge Targets DataGrid
            dgvMergeTargets.DataSource = _mergeTargets;
            dgvMergeTargets.AutoGenerateColumns = false;
            dgvMergeTargets.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "FileName", HeaderText = "ファイル名", DataPropertyName = "FileName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true },
                new DataGridViewButtonColumn { Name = "Delete", HeaderText = "削除", Text = "×", UseColumnTextForButtonValue = true, Width = 50 }
            });
        }

        private async void LoadSupplementalDocuments()
        {
            try
            {
                var documents = await _documentRepository.GetAllAsync();
                _supplementalDocuments.Clear();
                foreach (var doc in documents)
                {
                    _supplementalDocuments.Add(new SupplementalDocumentView
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
                Log.Error(ex, "Failed to load supplemental documents");
                MessageBox.Show($"付属書面の読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSelectMainPdf_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog { Filter = "PDF files (*.pdf)|*.pdf", Title = "契約書面PDFを選択してください" };
            if (dialog.ShowDialog() != DialogResult.OK) return;

            if (_pdfMergerService.ValidatePdfFile(dialog.FileName, out string errorMessage))
            {
                _mainPdfContent = await File.ReadAllBytesAsync(dialog.FileName);
                _mainPdfFileName = Path.GetFileName(dialog.FileName);

                _mainPdfList.Clear();
                _mainPdfList.Add(new MainPdfView { FileName = _mainPdfFileName, FileContent = _mainPdfContent });

                UpdateMergeButtonState();
                Log.Information("Main PDF selected: {FileName}", _mainPdfFileName);
            }
            else
            {
                MessageBox.Show(errorMessage, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DgvMainPdf_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvMainPdf.SelectedRows.Count > 0)
            {
                var selected = (MainPdfView)dgvMainPdf.SelectedRows[0].DataBoundItem;
                await DisplayPdfPreview(selected.FileContent, selected.FileName, "契約書面");
            }
        }

        private void BtnAddToMerge_Click(object? sender, EventArgs e)
        {
            if (dgvSupplemental.SelectedRows.Count > 0)
            {
                var selected = (SupplementalDocumentView)dgvSupplemental.SelectedRows[0].DataBoundItem;
                if (!_mergeTargets.Any(mt => mt.DatabaseId == selected.ID))
                {
                    _mergeTargets.Add(new MergeTargetView
                    {
                        FileName = selected.FileName,
                        FileContent = selected.FileContent,
                        IsFromDatabase = true,
                        DatabaseId = selected.ID
                    });
                    UpdateMergeButtonState();
                }
            }
        }

        private async void BtnSelectLocalFile_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog { Filter = "PDF files (*.pdf)|*.pdf", Title = "ローカルPDFファイルを選択してください" };
            if (dialog.ShowDialog() != DialogResult.OK) return;

            if (_pdfMergerService.ValidatePdfFile(dialog.FileName, out string errorMessage))
            {
                var content = await File.ReadAllBytesAsync(dialog.FileName);
                _mergeTargets.Add(new MergeTargetView
                {
                    FileName = Path.GetFileName(dialog.FileName),
                    FileContent = content,
                    IsFromDatabase = false
                });
                UpdateMergeButtonState();
            }
            else
            {
                MessageBox.Show(errorMessage, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnMoveUp_Click(object? sender, EventArgs e)
        {
            if (dgvMergeTargets.SelectedRows.Count > 0)
            {
                int index = dgvMergeTargets.SelectedRows[0].Index;
                if (index > 0)
                {
                    var item = _mergeTargets[index];
                    _mergeTargets.RemoveAt(index);
                    _mergeTargets.Insert(index - 1, item);
                    dgvMergeTargets.Rows[index - 1].Selected = true;
                }
            }
        }

        private void BtnMoveDown_Click(object? sender, EventArgs e)
        {
            if (dgvMergeTargets.SelectedRows.Count > 0)
            {
                int index = dgvMergeTargets.SelectedRows[0].Index;
                if (index < _mergeTargets.Count - 1)
                {
                    var item = _mergeTargets[index];
                    _mergeTargets.RemoveAt(index);
                    _mergeTargets.Insert(index + 1, item);
                    dgvMergeTargets.Rows[index + 1].Selected = true;
                }
            }
        }

        private async void BtnMerge_Click(object? sender, EventArgs e)
        {
            if (_mainPdfContent == null)
            {
                MessageBox.Show("契約書面PDFを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnMerge.Enabled = false;
                btnMerge.Text = "結合中...";

                var items = _mergeTargets.Select(mt => new MergeTargetItem
                {
                    FileName = mt.FileName,
                    FileContent = mt.FileContent,
                    IsFromDatabase = mt.IsFromDatabase,
                    DatabaseId = mt.DatabaseId
                }).ToList();

                var merged = await _pdfMergerService.MergeAsync(_mainPdfContent, items);
                var outputPath = GetUniqueOutputPath();

                await File.WriteAllBytesAsync(outputPath, merged);
                MessageBox.Show($"結合が完了しました。\n保存先: {outputPath}", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start("explorer.exe", $"/select,\"{outputPath}\"");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "PDF merge failed");
                MessageBox.Show($"結合処理に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnMerge.Enabled = true;
                btnMerge.Text = "結合";
            }
        }

        private string GetUniqueOutputPath()
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = Path.Combine(desktop, _mainPdfFileName);
            int counter = 1;
            while (File.Exists(path))
            {
                var name = Path.GetFileNameWithoutExtension(_mainPdfFileName);
                var ext = Path.GetExtension(_mainPdfFileName);
                path = Path.Combine(desktop, $"{name}_{counter}{ext}");
                counter++;
            }
            return path;
        }

        private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabAdmin && tabAdmin.Controls.Count == 0)
                LoadAdminInterface();
        }

        private void LoadAdminInterface()
        {
            try
            {
                var adminForm = _serviceProvider.GetRequiredService<AdminForm>();
                adminForm.TopLevel = false;
                adminForm.FormBorderStyle = FormBorderStyle.None;
                adminForm.Dock = DockStyle.Fill;

                var panel = new Panel { Dock = DockStyle.Fill };
                panel.Controls.Add(adminForm);
                tabAdmin.Controls.Add(panel);
                adminForm.Show();

                Log.Information("Admin interface loaded successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load admin interface");
                tabAdmin.Controls.Add(new Label
                {
                    Text = "管理画面の読み込みに失敗しました",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.Red
                });
            }
        }

        private async void DgvSupplemental_SelectionChanged(object? sender, EventArgs e)
        {
            btnAddToMerge.Enabled = dgvSupplemental.SelectedRows.Count > 0;
            if (dgvSupplemental.SelectedRows.Count > 0)
            {
                var selected = (SupplementalDocumentView)dgvSupplemental.SelectedRows[0].DataBoundItem;
                await DisplayPdfPreview(selected.FileContent, selected.FileName, "付属書面");
            }
        }

        private async void DgvMergeTargets_SelectionChanged(object? sender, EventArgs e)
        {
            bool hasSelection = dgvMergeTargets.SelectedRows.Count > 0;
            btnMoveUp.Enabled = hasSelection;
            btnMoveDown.Enabled = hasSelection;

            if (hasSelection)
            {
                var selected = (MergeTargetView)dgvMergeTargets.SelectedRows[0].DataBoundItem;
                await DisplayPdfPreview(selected.FileContent, selected.FileName, "結合対象");
            }
        }

        private async Task DisplayPdfPreview(byte[] pdfContent, string fileName, string documentType)
        {
            try
            {
                lblPreview.Text = $"{documentType}プレビュー";
                _currentPreviewPdf = pdfContent;
                _currentPageIndex = 0;

                using var stream = new MemoryStream(pdfContent);
                using var document = PdfSharpCore.Pdf.IO.PdfReader.Open(stream, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Import);
                _totalPages = document.PageCount;

                UpdatePageNavigation();
                await DisplayCurrentPage();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to display preview for {DocumentType}: {FileName}", documentType, fileName);
                ShowErrorInPreview();
            }
        }

        private void UpdatePageNavigation()
        {
            btnPrevPage.Enabled = _currentPageIndex > 0;
            btnNextPage.Enabled = _currentPageIndex < _totalPages - 1;
            lblPageInfo.Text = $"{_currentPageIndex + 1} / {_totalPages}";
        }

        private async Task DisplayCurrentPage()
        {
            if (_currentPreviewPdf == null) return;

            try
            {
                if (picPreview != null)
                {
                    var image = await _pdfMergerService.GeneratePageThumbnailAsync(_currentPreviewPdf, _currentPageIndex, pnlPreviewDisplay.Height);
                    if (image != null)
                    {
                        picPreview.Image?.Dispose();
                        picPreview.Image = image;
                    }
                }
                else if (webBrowser != null)
                {
                    var tempPath = Path.Combine(Path.GetTempPath(), $"preview_{Guid.NewGuid()}.pdf");
                    await File.WriteAllBytesAsync(tempPath, _currentPreviewPdf);
                    webBrowser.Navigate($"{tempPath}#page={_currentPageIndex + 1}");

                    _ = Task.Delay(10000).ContinueWith(_ =>
                    {
                        try { File.Delete(tempPath); } catch { }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to display page {PageIndex}", _currentPageIndex);
                ShowErrorInPreview();
            }
        }

        private async void BtnPrevPage_Click(object? sender, EventArgs e)
        {
            if (_currentPageIndex > 0)
            {
                _currentPageIndex--;
                UpdatePageNavigation();
                await DisplayCurrentPage();
            }
        }

        private async void BtnNextPage_Click(object? sender, EventArgs e)
        {
            if (_currentPageIndex < _totalPages - 1)
            {
                _currentPageIndex++;
                UpdatePageNavigation();
                await DisplayCurrentPage();
            }
        }

        private void DgvMergeTargets_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvMergeTargets.Columns["Delete"].Index)
            {
                _mergeTargets.RemoveAt(e.RowIndex);
                UpdateMergeButtonState();
            }
        }

        private void ShowErrorInPreview()
        {
            if (webBrowser != null)
            {
                webBrowser.DocumentText = "<html><body><h2>プレビューエラー</h2><p>PDFを表示できませんでした。</p></body></html>";
            }
            else if (picPreview != null)
            {
                picPreview.Image?.Dispose();
                var bitmap = new Bitmap(200, 250);
                using var g = Graphics.FromImage(bitmap);
                g.Clear(Color.LightGray);
                g.DrawRectangle(Pens.Red, 0, 0, bitmap.Width - 1, bitmap.Height - 1);
                g.DrawString("プレビュー\nエラー", SystemFonts.DefaultFont, Brushes.Red, 10, 120);
                picPreview.Image = bitmap;
            }
        }

        private void ShowTestPreview()
        {
            try
            {
                var bitmap = new Bitmap(200, 250);
                using var g = Graphics.FromImage(bitmap);
                g.Clear(Color.LightBlue);
                g.DrawRectangle(Pens.DarkBlue, 0, 0, bitmap.Width - 1, bitmap.Height - 1);
                g.DrawString("プレビュー\nテスト表示", new Font("MS UI Gothic", 12, FontStyle.Bold), Brushes.DarkBlue, 10, 100);

                if (picPreview != null) picPreview.Image = bitmap;
                else if (webBrowser != null) webBrowser.DocumentText = "<html><body style='background-color:lightblue; text-align:center; padding-top:100px;'><h2>プレビューテスト表示</h2></body></html>";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to show test preview");
            }
        }

        private void UpdateMergeButtonState() => btnMerge.Enabled = _mainPdfContent != null && _mergeTargets.Count > 0;
    }

    public class MainPdfView
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] FileContent { get; set; } = Array.Empty<byte>();
    }

    public class SupplementalDocumentView
    {
        public int ID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedAtString => CreatedAt.ToString("yyyy/MM/dd");
        public byte[] FileContent { get; set; } = Array.Empty<byte>();
        public bool Selected { get; set; } = false;
    }

    public class MergeTargetView
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] FileContent { get; set; } = Array.Empty<byte>();
        public bool IsFromDatabase { get; set; }
        public int? DatabaseId { get; set; }
    }
}