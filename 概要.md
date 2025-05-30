# PDF 結合ツール セットアップ手順

## 前提条件

1. **Windows 10/11 64-bit**
2. **.NET 8 SDK** がインストールされていること
3. **SQL Server 2019 以降** (LocalDB または SQL Server Express でも可)
4. **Visual Studio 2022** または **Visual Studio Code** (推奨)

## .NET 8 SDK のインストール確認とセットアップ

### 1. インストール状況の確認

コマンドプロンプトまたは PowerShell で以下のコマンドを実行：

```bash
dotnet --version
```

**期待される結果:**

- `8.0.xxx` が表示されれば .NET 8 SDK がインストール済み
- `'dotnet' は、内部コマンドまたは外部コマンド...` と表示される場合は未インストール

### 2. .NET 8 SDK のインストール

#### 方法 1: 公式サイトからダウンロード（推奨）

1. [.NET 公式サイト](https://dotnet.microsoft.com/ja-jp/download/dotnet/8.0) にアクセス
2. **「.NET 8.0 SDK」** をクリック
3. **Windows x64** をダウンロード
4. ダウンロードした `dotnet-sdk-8.0.xxx-win-x64.exe` を実行
5. インストーラーの指示に従ってインストール

#### 方法 2: winget を使用（Windows 10/11）

PowerShell を**管理者として実行**し、以下のコマンドを実行：

```powershell
winget install Microsoft.DotNet.SDK.8
```

#### 方法 3: Chocolatey を使用

Chocolatey がインストール済みの場合：

```powershell
choco install dotnet-8.0-sdk
```

### 3. インストール後の確認

1. **コマンドプロンプトを再起動**
2. 再度確認コマンドを実行：

```bash
dotnet --version
dotnet --list-sdks
```

**期待される結果:**

```
8.0.xxx
8.0.xxx [C:\Program Files\dotnet\sdk]
```

### 4. トラブルシューティング

#### PATH 環境変数の確認

.NET SDK がインストールされているのに `dotnet` コマンドが認識されない場合：

1. **システム環境変数を確認**:

   - `Win + R` → `sysdm.cpl` → 詳細設定 → 環境変数
   - システム環境変数の `Path` に以下が含まれているか確認：
     ```
     C:\Program Files\dotnet\
     ```

2. **手動で追加** (必要に応じて):
   - 「

## プロジェクトのセットアップ

### 1. プロジェクトの作成

新しいフォルダを作成し、以下のファイルを配置してください：

```
ContractPdfMerger/
├── ContractPdfMerger.csproj
├── Program.cs
├── Domain/
│   └── Models.cs
├── Infrastructure/
│   ├── AppDbContext.cs
│   └── Repositories.cs
├── Application/
│   └── PdfMergerService.cs
└── UI/
    ├── MainForm.cs
    ├── AdminForm.cs
    └── DocumentTypeDialog.cs
```

### 2. パッケージの復元

プロジェクトフォルダで以下のコマンドを実行：

```bash
dotnet restore
```

### 3. データベースの設定

#### SQL Server LocalDB を使用する場合 (推奨)

1. SQL Server Express LocalDB をインストール
2. 接続文字列を確認 (Program.cs 内)：
   ```csharp
   "Data Source=.;Initial Catalog=ContractPdfMerger;Integrated Security=True;TrustServerCertificate=True"
   ```

#### SQL Server Express を使用する場合

接続文字列を以下のように変更：

```csharp
"Data Source=.\\SQLEXPRESS;Initial Catalog=ContractPdfMerger;Integrated Security=True;TrustServerCertificate=True"
```

### 4. ビルドと実行

```bash
# ビルド
dotnet build

# 実行
dotnet run
```

## データベースの初期化

アプリケーション起動時に以下が自動実行されます：

1. データベースの作成 (存在しない場合)
2. テーブルの作成
3. 初期データの投入 (分類マスタ)

## 使用方法

### 基本的な使い方

1. **結合先 PDF 選択**: メイン画面左上の「結合先 PDF 選択」ボタンで PDF を選択
2. **付属ファイル選択**:
   - DB 登録済みファイル: 一覧から選択して「→」ボタン
   - ローカルファイル: 「ローカルファイル選択」ボタン
3. **結合順序調整**: 結合対象リストで「↑」「↓」ボタンまたは「×」で削除
4. **結合実行**: 「結合」ボタンで PDF 結合・デスクトップに保存

### 管理機能

1. **管理画面**: 右上の「管理画面」ボタン
2. **付属ファイル管理**: PDF 追加・差替え・削除
3. **分類マスタ管理**: 書面分類の追加・編集・削除

## トラブルシューティング

### データベース接続エラー

1. SQL Server サービスが起動しているか確認
2. Windows 認証が有効か確認
3. 接続文字列が正しいか確認

### PDF ファイルエラー

- ファイルサイズ: 1MB 以下
- 形式: PDF 形式のみ
- 破損していない PDF ファイルを使用

### ログの確認

- ログファイル: `logs/yyyyMMdd.log`
- エラー詳細はログファイルで確認可能

## 配布用ビルド

### ClickOnce 配布の準備

1. Visual Studio でプロジェクトを開く
2. プロジェクトのプロパティ → 発行
3. ClickOnce 設定を構成
4. 発行してインストーラーを生成

### 自己完結型実行ファイル

```bash
# Windows x64用
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# 出力先: bin/Release/net8.0-windows/win-x64/publish/
```

## 注意事項

- 管理者権限での実行は不要
- ウイルス対策ソフトで PDF 処理が制限される場合があります
- 大量の PDF 結合時はメモリ使用量に注意

## サポート

- ログファイルでエラー詳細を確認
- データベースのバックアップを定期的に実行
- PDF 処理でエラーが発生する場合は元ファイルの確認を推奨
