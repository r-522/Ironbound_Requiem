# Vercel デプロイ手順

## 構成
- 公開ディレクトリ: `Web/`
- `Web/index.html` が WebGL ローダーを呼ぶ。`./Build/*` を参照するため、WebGL ビルドの出力 `Builds/WebGL/Build` を `Web/Build` にコピー(またはシンボリックリンク)する。

## 手順
1. Unity で WebGL ビルドを行い、`Builds/WebGL` に出力。
2. 配信用ディレクトリを準備:
   ```bash
   rm -rf Web/Build Web/StreamingAssets Web/TemplateData
   cp -r Builds/WebGL/Build Web/Build
   [ -d Builds/WebGL/StreamingAssets ] && cp -r Builds/WebGL/StreamingAssets Web/StreamingAssets
   [ -d Builds/WebGL/TemplateData ]    && cp -r Builds/WebGL/TemplateData    Web/TemplateData
   ```
3. デプロイ:
   ```bash
   cd Web
   npx vercel --prod
   ```
4. 初回はプロジェクト名と `Output directory = .` の指定だけで OK。

## 注意
- `vercel.json` に `.wasm` の Content-Type と `.gz`/`.br` の Content-Encoding ヘッダを設定済み。
- WebGL の SharedArrayBuffer/Threads を使う場合のみ COOP/COEP ヘッダが必要(本 MVP は不要だが対策済み)。
- 本環境では Vercel CLI 未インストールのためデプロイ未実行。手動実行が必要。
