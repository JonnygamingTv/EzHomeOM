using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MoreLinq;
using Cysharp.Threading.Tasks;

namespace MyOpenModPlugin
{
    [CommandActor(typeof(UnturnedUser))]
    [Command("home")]
    [CommandAlias("bed")]
    [CommandAlias("tphome")]
    [CommandDescription("TP to bed")]
    public class CommandHome : Command
    {
        private readonly MyOpenModPlugin m_MyPlugin;
        private readonly IStringLocalizer m_StringLocalizer;
        public CommandHome(MyOpenModPlugin myPlugin, IStringLocalizer stringLocalizer,IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_MyPlugin = myPlugin;
            m_StringLocalizer = stringLocalizer;
        }

        //public IOpenModComponent Instance { get; private set; }

        protected override async Task OnExecuteAsync()
        {

            var player = (UnturnedUser)Context.Actor;
            SDG.Unturned.EPlayerStance toStance = m_MyPlugin.Configuration.GetSection("stance").Get<SDG.Unturned.EPlayerStance>();
            int time = m_MyPlugin.Configuration.GetSection("wait").Get<int>();
            if (time > 0)
            {
                _ = PrintAsync(m_StringLocalizer["command:home"]);
                System.Numerics.Vector3 lastPosition = player.Player.Transform.Position;
                double health = player.Player.Health;
                //bool cancel = false;
                //IDisposable sub = NullDisposable.Instance;
                /*if (m_MyPlugin.Configuration.GetSection("damage").Get<bool>())
                {
                    sub = EventBus.Subscribe<IPlayerDamagedEvent>((sp, sender, e) =>
                    {
                        if (Equals(e.Player, player.Player))
                            cancel = true;

                        return Task.CompletedTask;
                    });
                    if (lastPosition != player.Player.Transform.Position)
                    {
                        await player.PrintMessageAsync(m_MyPlugin.m_StringLocalizer["command_events:home_dmg"]);
                        return;
                    }
                }*/
                await Task.Delay(TimeSpan.FromSeconds(time));
                if (m_MyPlugin.Configuration.GetSection("move").Get<bool>())
                {
                    if (lastPosition != player.Player.Transform.Position)
                    {
                        await player.PrintMessageAsync(m_StringLocalizer["command:home_move"]);
                        return;
                    }
                }
                if (m_MyPlugin.Configuration.GetSection("damage").Get<bool>())
                {
                    if (health != player.Player.Health)
                    {
                        await player.PrintMessageAsync(m_StringLocalizer["command:home_dmg"]);
                        return;
                    }
                    //sub.Dispose();
                }
                try
                {
                    player.Player.Player.stance.stance = toStance;
                    /*SDG.Unturned.BarricadeManager.BarricadeRegions.Cast<SDG.Unturned.BarricadeData>().Where(b => b.owner == player.Player.SteamId.m_SteamID && m_MyPlugin.Configuration.GetSection("beds").Get<List<ushort>>().Contains(b.barricade.id)).ToList().ForEach(s =>
                    {
                        PrintAsync(s.barricade.state.ToString());
                        UnityEngine.Vector3 point = s.point;
                        point.y += 0.5f;
                        if (player.Player.Player.teleportToLocation(point, 0))
                        {
                            _ = player.PrintMessageAsync(m_StringLocalizer["command:home_tp"]);
                            return;
                        }
                    });
                    _ = player.PrintMessageAsync(m_StringLocalizer["command:no_home"]);
                    *
                    foreach (var reg in SDG.Unturned.BarricadeManager.BarricadeRegions)
                    {
                        for(int i=0;i< reg.barricades.Count; i++)
                        {
                            if (reg.barricades[i].owner == player.Player.SteamId.m_SteamID)
                            {
                                if(m_MyPlugin.Configuration.GetSection("beds").Get<List<ushort>>().Contains(reg.barricades[i].barricade.id))
                                {
                                    PrintAsync(reg.barricades[i].barricade.state.ToString());
                                    UnityEngine.Vector3 point = reg.barricades[i].point;
                                    point.y += 0.5f;
                                    if (player.Player.Player.teleportToLocation(point, 0))
                                    {
                                        _ = player.PrintMessageAsync(m_StringLocalizer["command:home_tp"]);
                                    }
                                    else
                                    {
                                        _ = player.PrintMessageAsync(m_StringLocalizer["command:no_home"]);
                                    }
                                    return;
                                }
                            }
                        }
                    }
                    _ = player.PrintMessageAsync(m_StringLocalizer["command:no_home"]);
                    /*
                    if (SDG.Unturned.BarricadeManager.tryGetBed(player.SteamId, out UnityEngine.Vector3 point, out byte angle1))
                    {
                        point.y += 0.5f;
                        float angle2 = angle1;// SDG.Unturned.MeasurementTool.byteToAngle(angle1);
                        if (player.Player.Player.teleportToLocation(point, angle2))
                        {
                            _ = player.PrintMessageAsync(m_StringLocalizer["command:home_tp"]);
                        }
                        else
                        {
                            _ = player.PrintMessageAsync(m_StringLocalizer["command:no_home"]);
                        }
                    }
                    else
                    {
                        _ = player.PrintMessageAsync(m_StringLocalizer["command:no_home"]);
                    }*/
                    //player.Player.Player.Invoke("teleportToBed", 0.0f);

                    await UniTask.SwitchToMainThread();
                    bool s = player.Player.Player.teleportToBed();
                    _ = UniTask.SwitchToThreadPool();
                    if (!s){ _ = player.PrintMessageAsync(m_StringLocalizer["command:no_home"]); } else { _ = player.PrintMessageAsync(m_StringLocalizer["command:home_tp"]); }
                }
                catch (Exception) { _ = player.PrintMessageAsync(m_StringLocalizer["command:no_home"]); }
            }
            else
            {
                _ = player.PrintMessageAsync(m_StringLocalizer["command:home_tp"]);
                player.Player.Player.stance.stance = toStance;
                await UniTask.SwitchToMainThread();
                bool s = player.Player.Player.teleportToBed();
                _ = UniTask.SwitchToTaskPool();
                if (s) { await player.PrintMessageAsync(m_StringLocalizer["command:home_tp"]); }
                else
                await player.PrintMessageAsync(m_StringLocalizer["command:no_home"]);
            }
        }
/*        public bool TeleportToBed(UnturnedUser p)
        {
            UnityEngine.Vector3 point;
            byte angle1;
            if (!SDG.Unturned.BarricadeManager.tryGetBed(p.SteamId, out point, out angle1))
                return false;
            point.y += 0.5f;
            float angle2 = SDG.Unturned.MeasurementTool.byteToAngle(angle1);
            return p.Player.Player.teleportToLocation(point, angle2);
        }*/
    }
}
