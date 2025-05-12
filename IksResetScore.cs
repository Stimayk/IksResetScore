using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using IksAdminApi;

namespace IksResetScore
{
    public class IksResetScore : BasePlugin, IPluginConfig<IksResetScoreConfig>
    {
        public override string ModuleName => "[IKS] Reset Score";
        public override string ModuleDescription => "";
        public override string ModuleAuthor => "E!N";
        public override string ModuleVersion => "v1.0.0";

        private const string AdminPermission = "players.resetscore";

        public IksResetScoreConfig Config { get; set; } = new();

        public void OnConfigParsed(IksResetScoreConfig config)
        {
            Config = config;
        }

        public override void OnAllPluginsLoaded(bool hotReload)
        {
            AdminModule.Api.MenuOpenPre += OnMenuOpenPre;
            AdminModule.Api.RegisterPermission(AdminPermission, Config.AdminPermissionFlag);
        }

        public override void Unload(bool hotReload)
        {
            AdminModule.Api.MenuOpenPre -= OnMenuOpenPre;
        }

        private HookResult OnMenuOpenPre(CCSPlayerController player, IDynamicMenu menu, IMenu gameMenu)
        {
            if (menu.Id != "iksadmin:menu:pm_main")
            {
                return HookResult.Continue;
            }

            menu.AddMenuOption("resetscore", Localizer["MenuOption.ResetScore"], (_, _) => { OpenSelectPlayerMenu(player, menu); },
                viewFlags: AdminUtils.GetCurrentPermissionFlags("players.resetscore"));

            return HookResult.Continue;
        }

        private void OpenSelectPlayerMenu(CCSPlayerController caller, IDynamicMenu? backMenu = null!)
        {
            IDynamicMenu menu = AdminModule.Api.CreateMenu(
                "resetscore.sp",
                Localizer["MenuOption.SP"],
                backMenu: backMenu
            );
            List<CCSPlayerController> players = [.. PlayersUtils.GetOnlinePlayers()];

            foreach (CCSPlayerController? target in players)
            {
                menu.AddMenuOption(target.GetSteamId(), target.PlayerName, (_, _) =>
                {
                    OpenResetScoreMenu(caller, target, menu);
                });
            }

            menu.Open(caller);
        }

        private void OpenResetScoreMenu(CCSPlayerController caller, CCSPlayerController target, IDynamicMenu? backMenu = null!)
        {
            IDynamicMenu menu = AdminModule.Api.CreateMenu(
                "resetscore.main",
                Localizer["MenuOption.ResetScore"],
                backMenu: backMenu
            );

            menu.AddMenuOption("rs", Localizer["ResetScore"], (_, _) => { ResetScore(caller, target); });
            menu.AddMenuOption("rk", Localizer["ResetKills"], (_, _) => { ResetKills(caller, target); });
            menu.AddMenuOption("rd", Localizer["ResetDeaths"], (_, _) => { ResetDeaths(caller, target); });
            menu.AddMenuOption("ra", Localizer["ResetAssists"], (_, _) => { ResetAssists(caller, target); });

            menu.Open(caller);
        }

        public void ResetScore(CCSPlayerController? caller, CCSPlayerController? target)
        {
            if (caller == null || target == null)
            {
                return;
            }

            CSMatchStats_t? controller = target.ActionTrackingServices?.MatchStats;
            if (controller != null)
            {
                controller.Kills = 0;
                controller.Deaths = 0;
                controller.Assists = 0;
                controller.HeadShotKills = 0;
                controller.Damage = 0;
                target.MVPs = 0;
                target.Score = 0;
                Utilities.SetStateChanged(target, "CCSPlayerController", "m_pActionTrackingServices");
                Utilities.SetStateChanged(target, "CCSPlayerController", "m_iMVPs");
                Utilities.SetStateChanged(target, "CCSPlayerController", "m_iScore");

                AdminUtils.Print(caller, Localizer["ResetScoreAdmin", target.PlayerName]);
                AdminUtils.Print(target, Localizer["ResetScorePlayer", caller.PlayerName]);
            }
        }

        public void ResetKills(CCSPlayerController? caller, CCSPlayerController? target)
        {
            if (caller == null || target == null)
            {
                return;
            }

            CSMatchStats_t? controller = target.ActionTrackingServices?.MatchStats;
            if (controller != null)
            {
                controller.Kills = 0;
                Utilities.SetStateChanged(target, "CCSPlayerController", "m_pActionTrackingServices");

                AdminUtils.Print(caller, Localizer["ResetKillsAdmin", target.PlayerName]);
                AdminUtils.Print(target, Localizer["ResetKillsPlayer", caller.PlayerName]);
            }
        }

        public void ResetDeaths(CCSPlayerController? caller, CCSPlayerController? target)
        {
            if (caller == null || target == null)
            {
                return;
            }

            CSMatchStats_t? controller = target.ActionTrackingServices?.MatchStats;
            if (controller != null)
            {
                controller.Deaths = 0;
                Utilities.SetStateChanged(target, "CCSPlayerController", "m_pActionTrackingServices");

                AdminUtils.Print(caller, Localizer["ResetDeathsAdmin", target.PlayerName]);
                AdminUtils.Print(target, Localizer["ResetDeathsPlayer", caller.PlayerName]);
            }
        }

        public void ResetAssists(CCSPlayerController? caller, CCSPlayerController? target)
        {
            if (caller == null || target == null)
            {
                return;
            }

            CSMatchStats_t? controller = target.ActionTrackingServices?.MatchStats;
            if (controller != null)
            {
                controller.Assists = 0;
                Utilities.SetStateChanged(target, "CCSPlayerController", "m_pActionTrackingServices");

                AdminUtils.Print(caller, Localizer["ResetAssistsAdmin", target.PlayerName]);
                AdminUtils.Print(target, Localizer["ResetAssistsPlayer", caller.PlayerName]);
            }
        }

    }
}