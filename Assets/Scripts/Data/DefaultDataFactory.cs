// 役割: ScriptableObject アセットが未設定の場合に動作確認用のデフォルト SO を生成する。
// 本番運用ではエディタで .asset を作成して差し替える。
using UnityEngine;

namespace Ironbound.Data
{
    public static class DefaultDataFactory
    {
        public static PlayerClassData[] BuildClasses()
        {
            return new[]
            {
                BuildClass("Vanguard", "Vanguard", "盾を構え、戦線を維持する近接前衛。",
                           160, 130, 60, 5.0f, new Color(0.6f, 0.5f, 0.35f), DamageElement.Physical, 22f, 36f),
                BuildClass("Arcanist", "Arcanist", "火・雷・氷の魔法で範囲を制圧する。",
                           95, 90, 160, 5.4f, new Color(0.45f, 0.35f, 0.7f), DamageElement.Fire, 18f, 60f),
                BuildClass("Warden", "Warden", "回復と召喚で前線を支える。",
                           115, 100, 130, 5.5f, new Color(0.35f, 0.6f, 0.5f), DamageElement.Holy, 16f, 40f),
                BuildClass("Ranger", "Ranger", "遠距離弓と罠で敵を捌く。",
                           100, 130, 80, 6.1f, new Color(0.55f, 0.55f, 0.3f), DamageElement.Physical, 20f, 50f),
            };
        }

        private static PlayerClassData BuildClass(string id, string name, string desc,
                                                   float hp, float st, float mp, float speed,
                                                   Color tint, DamageElement element, float basicDmg, float heavyDmg)
        {
            var c = ScriptableObject.CreateInstance<PlayerClassData>();
            c.ClassId = id; c.DisplayName = name; c.Description = desc;
            c.MaxHP = hp; c.MaxStamina = st; c.MaxMana = mp; c.MoveSpeed = speed;
            c.ThemeColor = tint; c.DodgeCost = 25;
            c.BasicAttack = MakeSkill(id + "_Basic", "Basic", basicDmg, 0.4f, 2.2f, element, SkillTargeting.ConeMelee);
            c.HeavyAttack = MakeSkill(id + "_Heavy", "Heavy", heavyDmg, 1.2f, 2.6f, element, SkillTargeting.ConeMelee, 0.15f, 6f, 90, 0.25f);
            c.Skills = new[]
            {
                MakeSkill(id + "_S1", "Skill 1", basicDmg * 1.4f, 6f, 3f, element, SkillTargeting.ConeMelee, 0.0f, 4f, 80, 0.20f),
                MakeSkill(id + "_S2", "Skill 2", basicDmg * 1.1f, 9f, 9f, element, SkillTargeting.Projectile,  0.0f, 2f, 60, 0.18f, 15f),
                id == "Warden"
                    ? MakeSkill(id + "_S3", "Heal", 35, 14f, 0,  DamageElement.Holy, SkillTargeting.Self, 0.0f, 0f, 40, 0.0f, 25f)
                    : MakeSkill(id + "_S3", "Burst", basicDmg * 2.0f, 16f, 4.5f, element, SkillTargeting.Area, 0.3f, 8f, 140, 0.32f, 25f)
            };
            c.UltimateSkill = MakeSkill(id + "_Ult", "Ultimate", basicDmg * 5f, 60f, 6f, element, SkillTargeting.Area, 0.5f, 12f, 220, 0.5f, 50f);
            c.PreferredTowers = new[] { "ArrowTower", "StoneWall", "HealBeacon" };
            c.StartingEquipment = new[] { "Sword", "Shield" };
            return c;
        }

        private static SkillData MakeSkill(string id, string name, float dmg, float cd, float range,
                                           DamageElement el, SkillTargeting tg,
                                           float cast = 0.05f, float kb = 2f, float hs = 60, float shake = 0.15f,
                                           float cost = 0)
        {
            var s = ScriptableObject.CreateInstance<SkillData>();
            s.SkillId = id; s.DisplayName = name; s.Damage = dmg; s.Cooldown = cd;
            s.Range = range; s.Radius = Mathf.Max(1.2f, range * 0.5f);
            s.CastTime = cast; s.RecoveryTime = 0.2f; s.Element = el; s.Targeting = tg;
            s.Knockback = kb; s.HitStopMs = hs; s.CameraShakeAmp = shake; s.Cost = cost;
            return s;
        }

        public static TowerData[] BuildTowers()
        {
            return new[]
            {
                T("StoneWall", "Stone Wall", TowerCategory.Wall, 10, 0, 0, 0, 250, new Color(0.55f, 0.52f, 0.48f)),
                T("IronBarricade", "Iron Barricade", TowerCategory.Wall, 18, 0, 0, 0, 380, new Color(0.45f, 0.42f, 0.4f)),
                T("SpikeBarricade", "Spike Barricade", TowerCategory.Wall, 22, 0, 8, 0.5f, 280, new Color(0.6f, 0.45f, 0.3f)),
                T("ArrowTower", "Arrow Tower", TowerCategory.Attack, 25, 12, 14, 1.8f, 180, new Color(0.7f, 0.55f, 0.3f)),
                T("ArcaneTower", "Arcane Tower", TowerCategory.Attack, 45, 14, 22, 1.2f, 160, new Color(0.5f, 0.35f, 0.8f), DamageElement.Arcane),
                T("CannonTower", "Cannon Tower", TowerCategory.Attack, 55, 14, 38, 0.6f, 220, new Color(0.4f, 0.35f, 0.3f)),
                T("LightningObelisk", "Lightning Obelisk", TowerCategory.Attack, 65, 11, 28, 1.4f, 180, new Color(0.6f, 0.7f, 0.95f), DamageElement.Lightning),
                T("HealBeacon", "Heal Beacon", TowerCategory.Support, 30, 8f, 12, 1f, 140, new Color(0.6f, 0.85f, 0.6f), DamageElement.Holy),
                T("ManaRelay", "Mana Relay", TowerCategory.Support, 35, 4f, 10, 1f, 150, new Color(0.4f, 0.55f, 0.85f), DamageElement.Arcane),
                T("WarBanner", "War Banner", TowerCategory.Support, 40, 6f, 14, 1f, 160, new Color(0.85f, 0.55f, 0.3f)),
                T("BuffTotem", "Buff Totem", TowerCategory.Support, 45, 6f, 10, 1f, 150, new Color(0.7f, 0.55f, 0.5f)),
                T("TrapMine", "Trap Mine", TowerCategory.Special, 18, 25, 4, 1f, 60, new Color(0.5f, 0.4f, 0.25f)),
                T("FrostField", "Frost Field", TowerCategory.Special, 35, 8, 6, 1.2f, 140, new Color(0.55f, 0.75f, 0.95f), DamageElement.Frost),
                T("GravityWell", "Gravity Well", TowerCategory.Special, 50, 6, 6, 1.0f, 140, new Color(0.35f, 0.3f, 0.5f), DamageElement.Shadow),
                T("FlameVent", "Flame Vent", TowerCategory.Special, 40, 10, 5, 1.4f, 130, new Color(0.95f, 0.4f, 0.2f), DamageElement.Fire),
            };
        }

        private static TowerData T(string id, string name, TowerCategory cat, int cost, float dmg, float range, float rate, float dur, Color tint, DamageElement el = DamageElement.Physical)
        {
            var t = ScriptableObject.CreateInstance<TowerData>();
            t.TowerId = id; t.Name = name; t.Category = cat; t.Cost = cost; t.Damage = dmg;
            t.Range = range; t.AttackRate = rate; t.Durability = dur; t.Element = el;
            t.BuildTime = 0.4f; t.UiTint = tint;
            return t;
        }

        public static EnemyData[] BuildEnemies()
        {
            return new[]
            {
                E("Wretch", EnemyType.Swarm, 40, 8, 0, 3.4f, 1.6f, 1.3f, new Color(0.35f, 0.3f, 0.28f)),
                E("Ironclad", EnemyType.Tank, 220, 18, 4, 2.2f, 1.8f, 0.8f, new Color(0.35f, 0.35f, 0.4f)),
                E("Battering Hulk", EnemyType.Siege, 320, 30, 6, 1.8f, 2.2f, 0.6f, new Color(0.3f, 0.25f, 0.2f)),
                E("Carrion Wing", EnemyType.Flying, 60, 10, 0, 4.0f, 1.4f, 1.1f, new Color(0.25f, 0.2f, 0.3f)),
                E("Shade Stalker", EnemyType.Assassin, 75, 16, 1, 4.5f, 1.5f, 1.4f, new Color(0.2f, 0.18f, 0.25f)),
                E("Ashen Shaman", EnemyType.Shaman, 90, 14, 2, 2.6f, 9.0f, 0.8f, new Color(0.5f, 0.35f, 0.55f)),
                E("Bone Archer", EnemyType.Archer, 70, 12, 0, 2.8f, 12f, 1.0f, new Color(0.4f, 0.4f, 0.35f)),
                E("Pyre Bomber", EnemyType.Bomber, 50, 24, 0, 3.6f, 1.4f, 1.0f, new Color(0.65f, 0.35f, 0.2f)),
                E("Dread Captain", EnemyType.Commander, 180, 18, 4, 2.6f, 2.0f, 0.9f, new Color(0.5f, 0.25f, 0.25f)),
                E("Iron Crow Knight", EnemyType.Boss, 1400, 28, 8, 2.2f, 2.4f, 0.7f, new Color(0.25f, 0.2f, 0.2f)),
            };
        }

        private static EnemyData E(string id, EnemyType t, float hp, float dmg, float armor, float speed, float atkRange, float atkRate, Color tint)
        {
            var e = ScriptableObject.CreateInstance<EnemyData>();
            e.EnemyId = id; e.Name = id; e.Type = t; e.HP = hp; e.Damage = dmg; e.Armor = armor;
            e.MoveSpeed = speed; e.AttackRange = atkRange; e.AttackRate = atkRate; e.BodyTint = tint;
            e.TargetPriority = t switch
            {
                EnemyType.Siege => new[] { TargetCategory.Barricade, TargetCategory.Tower, TargetCategory.SupplyLine, TargetCategory.Player },
                EnemyType.Assassin => new[] { TargetCategory.Player, TargetCategory.SupportTower, TargetCategory.Tower },
                EnemyType.Shaman => new[] { TargetCategory.SupportTower, TargetCategory.Player },
                EnemyType.Bomber => new[] { TargetCategory.Tower, TargetCategory.Barricade, TargetCategory.Player },
                EnemyType.Boss => new[] { TargetCategory.Player, TargetCategory.SupportTower, TargetCategory.Stronghold },
                _ => new[] { TargetCategory.Player, TargetCategory.Tower, TargetCategory.Barricade }
            };
            return e;
        }

        public static MissionData BuildMission(EnemyData[] enemies)
        {
            var m = ScriptableObject.CreateInstance<MissionData>();
            m.MissionId = "M01_AshenPlain";
            m.Name = "灰燼平原 — 失地奪還";
            m.MapId = "AshenPlain";
            m.TimeLimit = 900f;
            m.BossEnemy = enemies[9]; // Iron Crow Knight

            var wave1 = BuildWave("W1", enemies[0], 6, 0.8f);
            var wave2 = BuildWave("W2", enemies[0], 8, 0.6f, enemies[1], 2, 2f);
            var wave3 = BuildWave("W3", enemies[6], 4, 1f, enemies[2], 2, 3f, enemies[8], 1, 0f);

            m.Phases = new[]
            {
                new PhaseDefinition{ PhaseName = "探索",      Objective = ObjectiveType.Explore,     TimeBudget = 12f, ObjectiveLabel = "周辺を偵察せよ" },
                new PhaseDefinition{ PhaseName = "前線構築",  Objective = ObjectiveType.BuildTower,  TimeBudget = 20f, ObjectiveLabel = "B で建築モード — タワーを設置せよ" },
                new PhaseDefinition{ PhaseName = "ウェーブ防衛 I",  Objective = ObjectiveType.SurviveWave, Waves = new[]{ wave1 } },
                new PhaseDefinition{ PhaseName = "ウェーブ防衛 II", Objective = ObjectiveType.SurviveWave, Waves = new[]{ wave2 } },
                new PhaseDefinition{ PhaseName = "敵拠点制圧", Objective = ObjectiveType.SurviveWave, Waves = new[]{ wave3 } },
                new PhaseDefinition{ PhaseName = "中ボス戦",   Objective = ObjectiveType.KillBoss,    ObjectiveLabel = "Iron Crow Knight を討て" },
            };
            return m;
        }

        private static WaveData BuildWave(string id, EnemyData e1, int n1, float i1,
                                          EnemyData e2 = null, int n2 = 0, float i2 = 0,
                                          EnemyData e3 = null, int n3 = 0, float i3 = 0)
        {
            var w = ScriptableObject.CreateInstance<WaveData>();
            w.WaveId = id; w.Label = id; w.StartDelay = 2f;
            var list = new System.Collections.Generic.List<WaveSpawn>
            {
                new WaveSpawn{ Enemy = e1, Count = n1, Interval = i1, SpawnPointIndex = 0 }
            };
            if (e2 != null) list.Add(new WaveSpawn{ Enemy = e2, Count = n2, Interval = i2, SpawnPointIndex = 1 });
            if (e3 != null) list.Add(new WaveSpawn{ Enemy = e3, Count = n3, Interval = i3, SpawnPointIndex = 2 });
            w.Spawns = list.ToArray();
            return w;
        }
    }
}
