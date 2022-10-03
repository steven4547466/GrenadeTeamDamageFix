using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System.Collections.Generic;

namespace GrenadeTeamDamageFix
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        internal static Dictionary<ushort, Side> GrenadeCache = new Dictionary<ushort, Side>();

        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Player.ThrowingItem += OnThrowingItem;
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnExplodingGrenade;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.ThrowingItem -= OnThrowingItem;
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnExplodingGrenade;
            Instance = null;
            base.OnDisabled();
        }

        public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (ev.GrenadeType == GrenadeType.FragGrenade)
            {
                if (GrenadeCache.TryGetValue(ev.Grenade.Info.Serial, out Side side))
                {
                    GrenadeCache.Remove(ev.Grenade.Info.Serial);
                    if (ev.Thrower == Server.Host || ev.Thrower == null)
                        for (int i = ev.TargetsToAffect.Count - 1; i >= 0; i--)
                        {
                            if (ev.TargetsToAffect[i].Role.Side == side)
                            {
                                ev.TargetsToAffect.RemoveAt(i);
                            }
                        }
                }
            }
        }

        public void OnThrowingItem(ThrowingItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.GrenadeHE)
            {
                if (!GrenadeCache.ContainsKey(ev.Item.Serial))
                    GrenadeCache.Add(ev.Item.Serial, ev.Player.Role.Side);
            }
        }

    }
}
