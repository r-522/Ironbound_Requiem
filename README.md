# Crown of Ashvald: Ironbound Requiem — Vertical Slice MVP

侵攻型タワーディフェンス × サードパーソン 3D アクション RPG の **縦切り MVP**。
Unity (URP / WebGL) で実装、Vercel 静的配信を想定。

## 主要操作
| 入力 | アクション |
|---|---|
| WASD | 移動 |
| Space | ジャンプ |
| Shift | ダッシュ |
| LMB | 通常攻撃(コンボ) |
| RMB(押下) | 強攻撃 |
| RMB(長押し) | ガード |
| Alt | 回避(無敵) |
| Q / C / V | スキル 1/2/3 |
| Z | アルティメット |
| B | 建築モード切替 |
| 1/2/3/4 | タワー選択 |
| LMB (建築中) | 設置 |
| MMB | ロックオン |
| Esc | メニュー |

## ディレクトリ
- `Assets/Scripts/{Core,Player,Combat,AI,Towers,World,UI,Data,Audio,Net,Editor}` — C# 実装
- `Assets/Tests/EditMode` — NUnit EditMode テスト
- `Assets/Scenes` — Title / ClassSelect / AshenPlain / Result(最小プレースホルダ。`RuntimeSceneBootstrap` が動的構築)
- `Web/` — `index.html` + `vercel.json`(WebGL 配信)
- `Docs/` — Architecture / Build / Deploy / Editor Setup / Extension Notes / Acceptance

## ビルド/デプロイ
- `Docs/BUILD_WEBGL.md`
- `Docs/DEPLOY_VERCEL.md`

## 重要な注意
- 本リポジトリ生成環境には **Unity Editor / Unity CLI / Vercel CLI が無い** ため WebGL ビルドおよび Vercel デプロイは未実行。
- 正規アセット(モデル/テクスチャ/サウンド)未同梱。Unity を開けば Primitive(Cube/Capsule/Sphere)でフォールバック動作する。
- 4 シーン `.unity` ファイルは最小 YAML(Build Settings 用)。実シーン構築は `RuntimeSceneBootstrap.cs` がランタイムで担当する。

## 設計の柱
- **疎結合**: `EventBus` / `ServiceLocator` / `INetworkService` 抽象
- **データ駆動**: 全パラメータが ScriptableObject
- **WebGL 対応**: スレッド/`IO`/`Reflection.Emit` 不使用、`Object.FindObjectsByType` 採用
- **戦闘の手応え**: HitStop + CameraShake + Knockback + AudioCue 3 レイヤ
- **将来 Dedicated Server 化**: ゲームロジックと通信層を分離

詳細は `Docs/ARCHITECTURE.md` を参照。
