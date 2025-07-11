using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace EnergyTonicFixed
{
    /// <summary>Holds our Harmony patch methods.</summary>
    internal static class ObjectPatches
    {
        private static IMonitor Monitor = null;

        /// <summary>SMAPI calls this from ModEntry so we can log from our patches.</summary>
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        /***************
        ** Tooltip patches
        ** The game calls these methods when drawing the hover‐text.
        ** We intercept and, for ID 349, override the return value.
        ***************/

        public static bool StaminaRecoveredOnConsumption_Prefix(
            StardewValley.Object __instance,
            ref int __result
        )
        {
            try
            {
                if (__instance.ParentSheetIndex == 349) // Energy Tonic
                {
                    __result = 500;
                    return false; // skip original
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error in {nameof(StaminaRecoveredOnConsumption_Prefix)}:\n{ex}", LogLevel.Error);
                // fall back to original logic
            }
            return true;
        }

        public static bool HealthRecoveredOnConsumption_Prefix(
            StardewValley.Object __instance,
            ref int __result
        )
        {
            try
            {
                if (__instance.ParentSheetIndex == 349) // Energy Tonic
                {
                    __result = 215;
                    return false; // skip original
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error in {nameof(HealthRecoveredOnConsumption_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }

        /***************
        ** Consumption patch
        ** The game’s built‐in performUseAction only grants energy for ID 349,
        ** so we add a postfix to grant health after the fact.
        ***************/

        public static void PerformUseAction_Postfix(
            StardewValley.Object __instance,
            GameLocation location,
            ref bool __result
        )
        {
            try
            {
                if (!Context.IsWorldReady || !__result)
                    return;

                // ID 349 = Energy Tonic
                if (__instance.ParentSheetIndex == 349)
                {
                    Farmer player = Game1.player;
                    player.health = Math.Min(player.maxHealth, player.health + 215);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error in {nameof(PerformUseAction_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
