# Unity Editor セットアップ

## 推奨バージョン
- **Unity 6000.0 LTS**(`ProjectSettings/ProjectVersion.txt`)
- モジュール: WebGL Build Support, Linux Build Support(任意)

## 初回起動
1. Hub → Open → リポジトリのルート選択。
2. パッケージ取得が完了したら、`Window > Package Manager` で次が解決済みか確認:
   - Universal RP, Input System, TextMeshPro(`TMP Importer` ダイアログ:Essentials のみ Import で OK)
   - Cinemachine, AI Navigation, Test Framework
3. Render Pipeline Asset を作成し `Project Settings > Graphics > Scriptable Render Pipeline Settings` に割当(URP 既定設定で可)。
4. `Edit > Project Settings > Player > Other Settings > Color Space` を **Linear** に設定推奨。
5. `Window > General > Test Runner` → EditMode タブで全テスト緑を確認。

## シーン
- 4 シーン(Title/ClassSelect/AshenPlain/Result)は最小 YAML プレースホルダ。
- 起動後 `RuntimeSceneBootstrap` が UI/ゲームオブジェクトを動的構築する。
- 本格運用ではアーティストが本物のシーンを差し替えること。

## ScriptableObject の本配置
- 自動生成は `DefaultDataFactory` が担当。
- 本番では `Assets > Create > Ironbound > ...` から `.asset` を作成し、
  `SceneFabricator` や `ClassSelectController` に直接アサインすること。
