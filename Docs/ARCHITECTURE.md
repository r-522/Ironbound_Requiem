# アーキテクチャ概要

## レイヤ
- **Core**: `ServiceLocator`, `EventBus`, `GameStateMachine`, `GameBootstrap`, `RuntimeSceneBootstrap`
- **Data**: 全 SO 定義(`PlayerClassData`/`SkillData`/`TowerData`/`EnemyData`/`WaveData`/`MissionData`/`LootTableData`/`AudioCueData`/`ItemData`)
- **Combat**: `HealthComponent`/`StaminaComponent`/`ManaComponent`/`DamageComponent` (静的) /`SkillComponent`/`ComboController`/`MeleeHitbox` (静的) /`ProjectileComponent`/`HitStopService`/`CameraShakeController`/`KnockbackComponent`/`HitReactionComponent`/`GuardComponent`/`DodgeComponent`
- **Player**: `PlayerInput`/`PlayerController`/`ThirdPersonCameraController`/`LockOnController`/`ClassLoadout`
- **AI**: `EnemyAIController`/`AggroComponent`/`TargetRegistry`/`IEnemyBehavior` + 10 戦略
- **Towers**: `TowerComponent`/`TowerBuildController`/`TowerPreview`/`TowerTargeting`/`TowerProjectile`
- **World**: `ResourceManager`/`WaveManager`/`MissionManager`/`SpawnPoint`/`SupplyLine`/`SceneFabricator`
- **UI**: `UIHudController`/`BuildMenuController`/`TitleScreenController`/`ClassSelectController`/`ResultScreenController`/`SettingsController`/`DamageNumber`/`Minimap`
- **Audio**: `AudioManager` (3 レイヤ Cue)
- **Net**: `INetworkService` + `OfflineSessionService`(将来 `DedicatedServerNetworkService` 差し替え)

## 疎結合
- 直接参照は同レイヤ内で完結。レイヤ越しは `EventBus`/`ServiceLocator` 経由。
- Singleton は `AudioManager`/`HitStopService`/`CameraShakeController`/`ResourceManager` のみ(寿命がシーン同等)。
- ゲームロジックはすべて C#。シーンは「箱」だけ。実データは ScriptableObject。

## WebGL 配慮
- スレッド/`System.IO`/`Reflection.Emit` 不使用。
- セーブは PlayerPrefs(`Application.persistentDataPath` も WebGL 互換)。
- `Object.FindObjectsByType(FindObjectsSortMode.None)` を使い旧 API 警告を避ける。

## 拡張ポイント
- クラス追加: `PlayerClassData` SO + (任意で固有 `Skill` SO)
- タワー追加: `TowerData` SO + Prefab(なければ Cube フォールバック)
- 敵追加: `EnemyData` SO + 必要なら `IEnemyBehavior` 実装、`BehaviorFactory.Create` に分岐追加
- マップ追加: `Assets/Scenes/*.unity` + `SceneFabricator` 派生 or 手組み + `MissionData` 紐付け
- 通信: `INetworkService` を実装し `GameBootstrap.BootstrapServices` で差し替え

## 将来の Dedicated Server 化
- Authoritative にする際は `OfflineSessionService` を `ServerNetworkService` に差し替え、
  `MissionManager`/`WaveManager`/`TowerBuildController` の状態変化を `NetMessage` で発行・適用するだけで済むよう、
  「ロジック」と「レンダリング/入力」を分離してある(本 MVP では `PlayerInput` → `PlayerController` の単一プロセス実装)。
