# WebGL ビルド手順

## 前提
- Unity Hub で Unity **6000.0 LTS** を導入(`ProjectVersion.txt` を参照)
- WebGL Build Support モジュールを追加

## Unity Editor から
1. Hub → Open → 本リポジトリのルートを選択。
2. 初回ロードで `Packages/manifest.json` の URP/Cinemachine/AI Navigation/Input System が自動取得。
3. `File > Build Settings` を開き、`WebGL` を選択。
4. Scenes に `Title / ClassSelect / AshenPlain / Result` が登録されていること(`ProjectSettings/EditorBuildSettings.asset` に記述済み)。
5. Player Settings → Build Name を `Ironbound` に設定(`Web/index.html` の `BUILD_NAME` と一致)。
6. `Build` を押し、`Builds/WebGL` に出力。

## CLI(headless)
```bash
"/Applications/Unity/Hub/Editor/6000.0.23f1/Unity.app/Contents/MacOS/Unity" \
  -batchmode -nographics -quit \
  -projectPath "$(pwd)" \
  -buildTarget WebGL \
  -executeMethod Ironbound.EditorTools.Build.WebGL \
  -logFile -
```
Windows なら `Unity.exe`、Linux なら `Unity`。

## 検証
- `Builds/WebGL/index.html` をローカル HTTP で配信して動作確認:
  ```bash
  cd Builds/WebGL && python3 -m http.server 8080
  ```
- ブラウザで `http://localhost:8080` を開く。

## 注意
- 本リポジトリは Unity を用意できない環境向けに最低限の `.unity` プレースホルダ + `RuntimeSceneBootstrap` 動的構築方式を採用。
- 正規アセット(モデル/テクスチャ/オーディオ)は別途差し替える前提。
