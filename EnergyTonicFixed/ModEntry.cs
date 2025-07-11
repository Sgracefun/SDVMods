using HarmonyLib;
using StardewModdingAPI;

namespace EnergyTonicFixed
{
    /// <summary>The SMAPI entry point.</summary>
    public sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            // Initialize our patch class with SMAPI's monitor
            ObjectPatches.Initialize(this.Monitor);

            // Create and apply Harmony patches
            var harmony = new Harmony(this.ModManifest.UniqueID);

            // 1) When you hover an item, the game calls staminaRecoveredOnConsumption() and healthRecoveredOnConsumption()
            harmony.Patch(
                original: AccessTools.Method(
                    typeof(StardewValley.Object),
                    nameof(StardewValley.Object.staminaRecoveredOnConsumption)
                ),
                prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.StaminaRecoveredOnConsumption_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(
                    typeof(StardewValley.Object),
                    nameof(StardewValley.Object.healthRecoveredOnConsumption)
                ),
                prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.HealthRecoveredOnConsumption_Prefix))
            );

            // 2) When you actually use/drink an item, the game calls performUseAction(...)
            harmony.Patch(
                original: AccessTools.Method(
                    typeof(StardewValley.Object),
                    nameof(StardewValley.Object.performUseAction),
                    new[] { typeof(StardewValley.GameLocation) }
                ),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.PerformUseAction_Postfix))
            );

            this.Monitor.Log("Applied Harmony patches for Energy Tonic tooltip & healing.", LogLevel.Info);
        }
    }
}
