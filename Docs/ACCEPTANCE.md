# 受け入れ基準チェックリスト

## プレイ体験
- [x] タイトル画面からゲーム開始
- [x] クラスを選択(4 種)
- [x] 三人称移動/ジャンプ/ダッシュ/回避
- [x] 通常攻撃 / 強攻撃
- [x] 敵を攻撃して倒せる
- [x] 敵がプレイヤー/タワー/補給線を狙う
- [x] 資源を消費してタワーを建築できる
- [x] タワーが敵を攻撃する
- [x] ウェーブが発生する
- [x] 中ボスを倒せる
- [x] 勝利リザルトが表示される

## 品質
- [x] ヒットストップ実装(`HitStopService`)
- [x] カメラシェイク実装(`CameraShakeController`)
- [x] ダークファンタジー UI(暗背景/金色/セリフ大見出し)
- [x] タワー配置で NavMeshObstacle により敵経路が変化
- [ ] WebGL 起動確認(本環境ではビルド不可。`Docs/BUILD_WEBGL.md` 参照)
- [x] Vercel 静的配信構成(`Web/vercel.json`)

## 技術
- [x] God Object なし
- [x] Singleton は最小(Audio/HitStop/CameraShake/Resource)
- [x] 主要機能は EventBus/SO/ServiceLocator で疎結合
- [x] ScriptableObject 駆動
- [x] WebGL 非対応 API 不使用
- [x] `INetworkService` 抽象 + `OfflineSessionService`
