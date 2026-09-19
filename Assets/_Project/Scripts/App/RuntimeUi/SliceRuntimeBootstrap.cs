using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Heartline.Save;
using Heartline.ContentFlags;
using Heartline.Hub;
using Heartline.Room;
using Heartline.SyncMatch;
using GemColor = Heartline.GemColor;
using Heartline.Gacha;
using Heartline.Meta;

namespace Heartline.App.RuntimeUi
{
    /// <summary>
    /// Builds playable uGUI at runtime so Boot→Title→Hub→StageSelect→Battle→Room works
    /// without Editor Canvas wiring.
    /// </summary>
    public static class SliceRuntimeBootstrap
    {
        const string Marker = "HP_RuntimeUi";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Hook()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!scene.IsValid() || !scene.isLoaded) return;
            if (GameObject.Find(Marker) != null) return;
            EnsureServices();
            var root = new GameObject(Marker);
            Build(scene.name, root.transform);
        }

        static void EnsureServices()
        {
            if (ContentFlagsService.Instance == null)
                new GameObject("ContentFlags").AddComponent<ContentFlagsService>();
            if (SaveService.Instance == null)
                new GameObject("Save").AddComponent<SaveService>();
            if (DialogueService.Instance == null)
                new GameObject("Dialogue").AddComponent<DialogueService>();
            if (GameDataService.Instance == null)
                new GameObject("GameData").AddComponent<GameDataService>();
            ContentFlagsService.Instance.Load();
        }

        static void Build(string sceneName, Transform host)
        {
            switch (sceneName)
            {
                case "00_Boot": BuildBoot(host); break;
                case "01_Title": BuildTitle(host); break;
                case "02_Hub": BuildHub(host); break;
                case "03_StageSelect": BuildStageSelect(host); break;
                case "04_Room": BuildRoom(host); break;
                case "05_Team": BuildTeam(host); break;
                case "06_Gacha": BuildGacha(host); break;
                case "07_Jobs": BuildJobs(host); break;
                case "08_Battle": BuildBattle(host); break;
            }
        }

        static void BuildBoot(Transform host)
        {
            var canvas = RuntimeUiKit.CreateCanvas("BootCanvas", host);
            var boot = host.gameObject.GetComponent<AppBootstrap>() ?? host.gameObject.AddComponent<AppBootstrap>();
            RuntimeUiKit.Label(canvas.transform, "Title", "HEARTLINE PROTOCOL", 48, TextAnchor.MiddleCenter,
                new Vector2(0.05f, 0.72f), new Vector2(0.95f, 0.88f), Vector2.zero, Vector2.zero,
                new Color(0.91f, 0.78f, 0.48f));
            RuntimeUiKit.Label(canvas.transform, "Age",
                "Adults 18+ only. Confirm you are 18 or older to continue.\nSFW Mode ON · Juniper is 21.",
                28, TextAnchor.MiddleCenter,
                new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.68f), Vector2.zero, Vector2.zero);
            RuntimeUiKit.Button(canvas.transform, "Confirm", "I am 18+",
                new Vector2(0.15f, 0.28f), new Vector2(0.85f, 0.38f), Vector2.zero, Vector2.zero,
                boot.OnAgeConfirm, new Color(0.18f, 0.55f, 0.35f));
            RuntimeUiKit.Button(canvas.transform, "Exit", "Exit",
                new Vector2(0.25f, 0.16f), new Vector2(0.75f, 0.24f), Vector2.zero, Vector2.zero,
                boot.OnAgeExit, new Color(0.45f, 0.2f, 0.25f));
        }

        static void BuildTitle(Transform host)
        {
            var canvas = RuntimeUiKit.CreateCanvas("TitleCanvas", host);
            var title = host.gameObject.AddComponent<TitleController>();
            RuntimeUiKit.Label(canvas.transform, "Title", "HEARTLINE PROTOCOL", 52, TextAnchor.MiddleCenter,
                new Vector2(0.05f, 0.7f), new Vector2(0.95f, 0.88f), Vector2.zero, Vector2.zero,
                new Color(0.91f, 0.78f, 0.48f));
            RuntimeUiKit.Label(canvas.transform, "Sub", "Vertical Slice · Sync Match + Mira Room", 26, TextAnchor.MiddleCenter,
                new Vector2(0.1f, 0.6f), new Vector2(0.9f, 0.7f), Vector2.zero, Vector2.zero);
            RuntimeUiKit.Button(canvas.transform, "New", "New Game (grant Mira)",
                new Vector2(0.15f, 0.38f), new Vector2(0.85f, 0.48f), Vector2.zero, Vector2.zero,
                title.NewGame, new Color(0.89f, 0.23f, 0.29f));
            RuntimeUiKit.Button(canvas.transform, "Continue", "Continue",
                new Vector2(0.15f, 0.24f), new Vector2(0.85f, 0.34f), Vector2.zero, Vector2.zero,
                title.Continue);
        }

        static void BuildHub(Transform host)
        {
            var canvas = RuntimeUiKit.CreateCanvas("HubCanvas", host);
            var hub = host.gameObject.AddComponent<HubController>();
            var sta = RuntimeUiKit.Label(canvas.transform, "STA", "STA", 26, TextAnchor.MiddleLeft,
                new Vector2(0.05f, 0.9f), new Vector2(0.5f, 0.97f), Vector2.zero, Vector2.zero);
            var he = RuntimeUiKit.Label(canvas.transform, "HE", "HE", 26, TextAnchor.MiddleLeft,
                new Vector2(0.5f, 0.9f), new Vector2(0.95f, 0.97f), Vector2.zero, Vector2.zero);
            var coins = RuntimeUiKit.Label(canvas.transform, "LC", "LC", 26, TextAnchor.MiddleLeft,
                new Vector2(0.05f, 0.84f), new Vector2(0.5f, 0.9f), Vector2.zero, Vector2.zero);
            var crystals = RuntimeUiKit.Label(canvas.transform, "PC", "PC", 26, TextAnchor.MiddleLeft,
                new Vector2(0.5f, 0.84f), new Vector2(0.95f, 0.9f), Vector2.zero, Vector2.zero);
            var status = RuntimeUiKit.Label(canvas.transform, "Status", "Hub", 28, TextAnchor.MiddleCenter,
                new Vector2(0.05f, 0.74f), new Vector2(0.95f, 0.84f), Vector2.zero, Vector2.zero,
                new Color(0f, 0.83f, 1f));
            hub.BindLegacy(sta, he, coins, crystals, status);
            hub.RefreshHud();

            float y = 0.62f;
            void Nav(string label, UnityEngine.Events.UnityAction action)
            {
                RuntimeUiKit.Button(canvas.transform, label, label,
                    new Vector2(0.12f, y - 0.07f), new Vector2(0.88f, y), Vector2.zero, Vector2.zero, action);
                y -= 0.085f;
            }
            Nav("Stages", hub.OpenStages);
            Nav("Mira's Room", hub.OpenRoom);
            Nav("Team", hub.OpenTeam);
            Nav("Beginner Banner", hub.OpenGacha);
            Nav("Jobs (Café)", hub.OpenJobs);
            Nav("Mail", hub.OpenMail);
            Nav("Settings", hub.OpenSettings);
            RuntimeUiKit.Button(canvas.transform, "DebugSTA", "Debug: refill STA/HE",
                new Vector2(0.2f, 0.02f), new Vector2(0.8f, 0.08f), Vector2.zero, Vector2.zero,
                hub.DebugAddStamina, new Color(0.25f, 0.25f, 0.35f));
        }

        static void BuildStageSelect(Transform host)
        {
            var canvas = RuntimeUiKit.CreateCanvas("StageCanvas", host);
            var ctrl = host.gameObject.AddComponent<StageSelectController>();
            var info = RuntimeUiKit.Label(canvas.transform, "Info", "Chapter 1", 26, TextAnchor.MiddleCenter,
                new Vector2(0.05f, 0.88f), new Vector2(0.95f, 0.96f), Vector2.zero, Vector2.zero);
            ctrl.BindLegacy(info);
            RuntimeUiKit.ScrollColumn(canvas.transform, "StageList", out var content);
            var save = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            var stages = ContentRegistry.Stages?.stages;
            if (stages != null)
            {
                foreach (var s in stages)
                {
                    bool unlocked = StageCombatMath.IsUnlocked(s.id, save.clearedStages, save.ch1Complete);
                    bool cleared = save.clearedStages.Contains(s.id);
                    string label = $"{s.id}  {s.name}  ({s.staminaCost} STA)" +
                                   (cleared ? " ✓" : "") + (!unlocked ? " [LOCKED]" : "");
                    var btn = RuntimeUiKit.Button(content, s.id, label,
                        Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, null);
                    var le = btn.gameObject.AddComponent<LayoutElement>();
                    le.minHeight = 72;
                    le.preferredHeight = 72;
                    ctrl.RegisterRuntimeStageButton(btn, s.id, label, unlocked);
                }
            }
            RuntimeUiKit.Button(canvas.transform, "Back", "Back to Hub",
                new Vector2(0.2f, 0.02f), new Vector2(0.8f, 0.1f), Vector2.zero, Vector2.zero, ctrl.Back);
        }

        static void BuildRoom(Transform host)
        {
            var canvas = RuntimeUiKit.CreateCanvas("RoomCanvas", host);
            var room = host.gameObject.AddComponent<RoomController>();
            var dialogue = RuntimeUiKit.Label(canvas.transform, "Dialogue", "…", 28, TextAnchor.UpperLeft,
                new Vector2(0.06f, 0.55f), new Vector2(0.94f, 0.88f), Vector2.zero, Vector2.zero);
            var aff = RuntimeUiKit.Label(canvas.transform, "Aff", "Aff", 26, TextAnchor.MiddleCenter,
                new Vector2(0.05f, 0.48f), new Vector2(0.95f, 0.55f), Vector2.zero, Vector2.zero,
                new Color(1f, 0.45f, 0.65f));
            var energy = RuntimeUiKit.Label(canvas.transform, "HE", "HE", 24, TextAnchor.MiddleCenter,
                new Vector2(0.05f, 0.42f), new Vector2(0.95f, 0.48f), Vector2.zero, Vector2.zero);
            room.BindLegacy(dialogue, aff, energy);
            RuntimeUiKit.Button(canvas.transform, "Talk", "Talk (−5 HE, +20 XP)",
                new Vector2(0.1f, 0.32f), new Vector2(0.9f, 0.4f), Vector2.zero, Vector2.zero, room.Talk);
            RuntimeUiKit.Button(canvas.transform, "Gift", "Gift (−8 HE, Aff2+)",
                new Vector2(0.1f, 0.23f), new Vector2(0.9f, 0.31f), Vector2.zero, Vector2.zero, room.Gift);
            RuntimeUiKit.Button(canvas.transform, "Date", "Date (−15 HE, Aff3)",
                new Vector2(0.1f, 0.14f), new Vector2(0.9f, 0.22f), Vector2.zero, Vector2.zero, room.Date);
            RuntimeUiKit.Button(canvas.transform, "Sleep", "Sleepover (morning +30)",
                new Vector2(0.1f, 0.06f), new Vector2(0.55f, 0.13f), Vector2.zero, Vector2.zero, room.Sleepover);
            RuntimeUiKit.Button(canvas.transform, "Back", "Back",
                new Vector2(0.58f, 0.06f), new Vector2(0.9f, 0.13f), Vector2.zero, Vector2.zero, room.Back);
        }

        static void BuildTeam(Transform host)
        {
            var canvas = RuntimeUiKit.CreateCanvas("TeamCanvas", host);
            var team = host.gameObject.AddComponent<TeamController>();
            var squad = RuntimeUiKit.Label(canvas.transform, "Squad", "Squad", 26, TextAnchor.UpperLeft,
                new Vector2(0.05f, 0.42f), new Vector2(0.95f, 0.92f), Vector2.zero, Vector2.zero);
            team.BindLegacy(squad);

            float y = 0.36f;
            void SlotBtn(string label, UnityEngine.Events.UnityAction action)
            {
                RuntimeUiKit.Button(canvas.transform, label, label,
                    new Vector2(0.05f, y - 0.045f), new Vector2(0.95f, y), Vector2.zero, Vector2.zero, action);
                y -= 0.05f;
            }
            SlotBtn("Cycle Front 0", team.CycleFront0);
            SlotBtn("Cycle Front 1", team.CycleFront1);
            SlotBtn("Cycle Mid 0", team.CycleMid0);
            SlotBtn("Cycle Mid 1", team.CycleMid1);
            SlotBtn("Cycle Back 0", team.CycleBack0);
            SlotBtn("Cycle Back 1", team.CycleBack1);

            RuntimeUiKit.Button(canvas.transform, "Advice", "Apply lane advice",
                new Vector2(0.05f, 0.08f), new Vector2(0.48f, 0.14f), Vector2.zero, Vector2.zero,
                team.ApplyDefaultLaneAdvice);
            RuntimeUiKit.Button(canvas.transform, "Dlg", "Advance Dialogue",
                new Vector2(0.52f, 0.08f), new Vector2(0.95f, 0.14f), Vector2.zero, Vector2.zero,
                team.AdvanceDialogue);
            RuntimeUiKit.Button(canvas.transform, "Back", "Back",
                new Vector2(0.2f, 0.02f), new Vector2(0.8f, 0.07f), Vector2.zero, Vector2.zero, team.Back);
        }

        static void BuildGacha(Transform host)
        {
            var canvas = RuntimeUiKit.CreateCanvas("GachaCanvas", host);
            var gacha = host.gameObject.AddComponent<GachaController>();
            var rates = RuntimeUiKit.Label(canvas.transform, "Rates", "Rates", 24, TextAnchor.UpperLeft,
                new Vector2(0.08f, 0.55f), new Vector2(0.92f, 0.92f), Vector2.zero, Vector2.zero);
            var result = RuntimeUiKit.Label(canvas.transform, "Result", "", 24, TextAnchor.UpperLeft,
                new Vector2(0.08f, 0.28f), new Vector2(0.92f, 0.54f), Vector2.zero, Vector2.zero);
            gacha.BindLegacy(rates, result);
            RuntimeUiKit.Button(canvas.transform, "Pull", "Free 10-pull (once)",
                new Vector2(0.1f, 0.16f), new Vector2(0.9f, 0.26f), Vector2.zero, Vector2.zero, gacha.FreeTenPull);
            RuntimeUiKit.Button(canvas.transform, "Back", "Not Now / Back",
                new Vector2(0.2f, 0.05f), new Vector2(0.8f, 0.14f), Vector2.zero, Vector2.zero, gacha.NotNow);
        }

        static void BuildJobs(Transform host)
        {
            var canvas = RuntimeUiKit.CreateCanvas("JobsCanvas", host);
            var jobs = host.gameObject.AddComponent<JobsController>();
            var status = RuntimeUiKit.Label(canvas.transform, "Status", "Jobs", 28, TextAnchor.MiddleCenter,
                new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.75f), Vector2.zero, Vector2.zero);
            jobs.BindLegacy(status);
            RuntimeUiKit.Button(canvas.transform, "Assign", "Assign Café Shift",
                new Vector2(0.1f, 0.32f), new Vector2(0.9f, 0.42f), Vector2.zero, Vector2.zero, jobs.AssignCafe);
            RuntimeUiKit.Button(canvas.transform, "Claim", "Claim",
                new Vector2(0.1f, 0.21f), new Vector2(0.9f, 0.31f), Vector2.zero, Vector2.zero, jobs.ClaimCafe);
            RuntimeUiKit.Button(canvas.transform, "Debug", "Debug: complete now",
                new Vector2(0.1f, 0.12f), new Vector2(0.9f, 0.2f), Vector2.zero, Vector2.zero, jobs.DebugCompleteNow);
            RuntimeUiKit.Button(canvas.transform, "Back", "Back",
                new Vector2(0.2f, 0.03f), new Vector2(0.8f, 0.1f), Vector2.zero, Vector2.zero, jobs.Back);
        }

        static void BuildBattle(Transform host)
        {
            var canvas = RuntimeUiKit.CreateCanvas("BattleCanvas", host);
            var battle = host.gameObject.AddComponent<BattleController>();
            var status = RuntimeUiKit.Label(canvas.transform, "Status", "Battle", 24, TextAnchor.MiddleCenter,
                new Vector2(0.02f, 0.94f), new Vector2(0.98f, 0.99f), Vector2.zero, Vector2.zero);
            var enemy = RuntimeUiKit.Label(canvas.transform, "Enemy", "HP", 26, TextAnchor.MiddleCenter,
                new Vector2(0.02f, 0.88f), new Vector2(0.98f, 0.94f), Vector2.zero, Vector2.zero,
                new Color(1f, 0.35f, 0.4f));
            var gauge = RuntimeUiKit.Label(canvas.transform, "Gauge", "Gauge", 20, TextAnchor.UpperLeft,
                new Vector2(0.02f, 0.72f), new Vector2(0.98f, 0.88f), Vector2.zero, Vector2.zero,
                new Color(0f, 0.83f, 1f));
            var board = RuntimeUiKit.Label(canvas.transform, "Board", "Board", 22, TextAnchor.UpperLeft,
                new Vector2(0.02f, 0.38f), new Vector2(0.55f, 0.72f), Vector2.zero, Vector2.zero);
            var log = RuntimeUiKit.Label(canvas.transform, "Log", "Log", 18, TextAnchor.UpperLeft,
                new Vector2(0.56f, 0.38f), new Vector2(0.98f, 0.72f), Vector2.zero, Vector2.zero);
            var lane = RuntimeUiKit.Label(canvas.transform, "Lane", "Lanes", 18, TextAnchor.MiddleLeft,
                new Vector2(0.02f, 0.33f), new Vector2(0.98f, 0.38f), Vector2.zero, Vector2.zero);
            battle.BindLegacy(board, log, enemy, gauge, lane, status);

            var grid = new GameObject("GemGrid");
            grid.transform.SetParent(canvas.transform, false);
            var gridRt = grid.AddComponent<RectTransform>();
            gridRt.anchorMin = new Vector2(0.05f, 0.12f);
            gridRt.anchorMax = new Vector2(0.55f, 0.36f);
            gridRt.offsetMin = Vector2.zero;
            gridRt.offsetMax = Vector2.zero;
            var gl = grid.AddComponent<GridLayoutGroup>();
            gl.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gl.constraintCount = BoardSimulator.Width;
            gl.cellSize = new Vector2(72, 48);
            gl.spacing = new Vector2(4, 4);
            gl.childAlignment = TextAnchor.MiddleCenter;
            for (int y = BoardSimulator.Height - 1; y >= 0; y--)
            for (int x = 0; x < BoardSimulator.Width; x++)
            {
                int cx = x, cy = y;
                var cell = RuntimeUiKit.Button(grid.transform, $"c{x}_{y}", "·",
                    Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                    () => battle.OnCellClicked(cx, cy), new Color(0.1f, 0.12f, 0.2f));
                cell.GetComponentInChildren<Text>().fontSize = 18;
            }
            host.gameObject.AddComponent<BattleBoardUiRefresher>().Init(battle, grid.transform);

            RuntimeUiKit.Button(canvas.transform, "Assist", "Assist Aim",
                new Vector2(0.58f, 0.28f), new Vector2(0.98f, 0.36f), Vector2.zero, Vector2.zero, battle.AssistAimSwap);
            RuntimeUiKit.Button(canvas.transform, "Mira", "Mira Burst",
                new Vector2(0.58f, 0.2f), new Vector2(0.78f, 0.27f), Vector2.zero, Vector2.zero,
                battle.FireMiraBurst, new Color(0.18f, 0.74f, 0.42f));
            RuntimeUiKit.Button(canvas.transform, "Ivy", "Ivy Burst",
                new Vector2(0.8f, 0.2f), new Vector2(0.98f, 0.27f), Vector2.zero, Vector2.zero,
                battle.FireIvyBurst, new Color(0.89f, 0.23f, 0.29f));
            RuntimeUiKit.Button(canvas.transform, "Jun", "Juni Burst",
                new Vector2(0.58f, 0.12f), new Vector2(0.78f, 0.19f), Vector2.zero, Vector2.zero,
                battle.FireJuniperBurst, new Color(0.18f, 0.48f, 1f));
            RuntimeUiKit.Button(canvas.transform, "Wren", "Wren Burst",
                new Vector2(0.8f, 0.12f), new Vector2(0.98f, 0.19f), Vector2.zero, Vector2.zero,
                battle.FireWrenBurst, new Color(0.45f, 0.75f, 0.35f));
            RuntimeUiKit.Button(canvas.transform, "End", "End Turn",
                new Vector2(0.58f, 0.04f), new Vector2(0.78f, 0.11f), Vector2.zero, Vector2.zero, battle.EndTurn);
            RuntimeUiKit.Button(canvas.transform, "Retreat", "Retreat",
                new Vector2(0.8f, 0.04f), new Vector2(0.98f, 0.11f), Vector2.zero, Vector2.zero,
                battle.Retreat, new Color(0.4f, 0.2f, 0.25f));
        }
    }

    public sealed class BattleBoardUiRefresher : MonoBehaviour
    {
        BattleController _battle;
        Transform _grid;
        float _t;

        public void Init(BattleController battle, Transform grid)
        {
            _battle = battle;
            _grid = grid;
        }

        void Update()
        {
            _t += Time.unscaledDeltaTime;
            if (_t < 0.15f || _battle == null || _battle.Board == null || _grid == null) return;
            _t = 0f;
            int i = 0;
            for (int y = BoardSimulator.Height - 1; y >= 0; y--)
            for (int x = 0; x < BoardSimulator.Width; x++, i++)
            {
                if (i >= _grid.childCount) return;
                var txt = _grid.GetChild(i).GetComponentInChildren<Text>();
                if (txt == null) continue;
                var c = _battle.Board.Get(x, y);
                txt.text = GemLetter(c);
                txt.color = GemTint(c);
            }
        }

        static string GemLetter(GemColor c) => c switch
        {
            GemColor.Crimson => "C",
            GemColor.Azure => "A",
            GemColor.Verdant => "V",
            GemColor.Amber => "M",
            GemColor.Violet => "Vi",
            GemColor.Prism => "P",
            _ => "·"
        };

        static Color GemTint(GemColor c) => c switch
        {
            GemColor.Crimson => new Color(0.89f, 0.23f, 0.29f),
            GemColor.Azure => new Color(0.18f, 0.48f, 1f),
            GemColor.Verdant => new Color(0.18f, 0.74f, 0.42f),
            GemColor.Amber => new Color(0.94f, 0.63f, 0.16f),
            GemColor.Violet => new Color(0.55f, 0.36f, 0.96f),
            GemColor.Prism => new Color(0.96f, 0.97f, 1f),
            _ => Color.gray
        };
    }
}
