# 将来拡張メモ(優先度 B)

## Online Coop
- `INetworkService` を `MirrorNetworkService`/`FishNetService`/独自 Authoritative Server で実装。
- `NetMessage` を Server-tick(30Hz)で発行し、`MissionManager`/`WaveManager` を権威化。
- 入力は `PlayerInput` → `CommandBuffer` に切り出し、`PlayerController` は Server からの State 適用に変更。

## Dedicated Server
- 描画レス Linux build。`RuntimeSceneBootstrap` の UI 構築を `#if !UNITY_SERVER` で除外。
- `EnemyAIController` のみ実行、クライアントには差分のみ送信。

## Steam Integration
- `Steamworks.NET` を `Net` レイヤに追加し `INetworkService` 実装。Workshop/実績は別レイヤに分離。

## Advanced VFX / Pet / Mod
- `ScriptableObject` ハンドルを Addressables で抽象化済み(`Packages/manifest.json` に余地あり)。
- Mod Support は `StreamingAssets/Mods/*.json` を `DefaultDataFactory` 後段で読み込む拡張点を用意可能。

## Save System
- 簡易は PlayerPrefs。本格はオフラインなら `Application.persistentDataPath/save.json`、オンラインなら Server 側ストア。
