using Harmony;
using System;

namespace SmartReservoirs
{
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
    {
        private static void Prefix()
        {
            Debug.Log("=== GeneratedBuildings.LoadGeneratedBuildings Prefix === " + LiquidReservoirSmartConfig.ID + " " + LiquidReservoirSmartConfig.PrefabID);

            LiquidReservoirSmartConfig.addStrings();
            ModUtil.AddBuildingToPlanScreen("Plumbing", LiquidReservoirSmartConfig.ID);
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class AR_Db_Initialize_Patch
    {
        private static void Prefix()
        {
            Debug.Log("=== Db.Initialize Prefix === " + LiquidReservoirSmartConfig.ID + " ");
            Database.Techs.TECH_GROUPING["ImprovedLiquidPiping"].AddToArray(LiquidReservoirSmartConfig.ID);
        }
    }
}