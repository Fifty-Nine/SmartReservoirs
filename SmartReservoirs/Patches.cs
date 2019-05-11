using Harmony;
using System;

namespace SmartReservoirs
{
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
    {
        private static void Prefix()
        {
            Debug.Log("=== SmartReservoirs GeneratedBuildings.LoadGeneratedBuildings Prefix === ");

            LiquidReservoirSmartConfig.addStrings();
            ModUtil.AddBuildingToPlanScreen("Plumbing", LiquidReservoirSmartConfig.ID);

            GasReservoirSmartConfig.addStrings();
            ModUtil.AddBuildingToPlanScreen("HVAC", GasReservoirSmartConfig.ID);
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class AR_Db_Initialize_Patch
    {
        private static void Prefix()
        {
            Debug.Log("=== SmartReservoirs Db.Initialize Prefix === ");
            Database.Techs.TECH_GROUPING["LiquidTemperature"].AddToArray(LiquidReservoirSmartConfig.ID);
            Database.Techs.TECH_GROUPING["HVAC"].AddToArray(GasReservoirSmartConfig.ID);
        }
    }
}