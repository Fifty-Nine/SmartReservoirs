using TUNING;
using UnityEngine;

namespace SmartReservoirs
{
    [KSerialization.SerializationConfig(KSerialization.MemberSerialization.OptIn)]
    public class GasReservoirSmartConfig : IBuildingConfig
    {
        public const string ID = "GasReservoirSmart";
        public static string PrefabID {
            get {
                return ID.ToUpper();
            }
        }

        internal static class SLRStrings {
            internal const string Name = "Smart Gas Reservoir";
            internal const string Desc = "FIXME Desc.";
            internal const string Effect = " FIXME Effect";
            internal const string OutputPort = "FIXME Output Port";
            internal const string OutputPortActive = "FIXME Output Active";
            internal const string OutputPortInactive = "FIXME Output Inactive";
        };

        public static void addStrings()
        {
            var prefabFmt = "STRINGS.BUILDINGS.PREFABS." + PrefabID;

            Strings.Add(prefabFmt + ".NAME", SLRStrings.Name);
            Strings.Add(prefabFmt + ".DESC", SLRStrings.Desc);
            Strings.Add(prefabFmt + ".EFFECT", SLRStrings.Effect);
        }

        private static LogicPorts.Port makeInputPort()
        {
            return LogicPorts.Port.InputPort(
                LogicOperationalController.PORT_ID,
                cell_offset: new CellOffset(-2, 1),
                description: STRINGS.UI.LOGIC_PORTS.CONTROL_OPERATIONAL,
                activeDescription: STRINGS.UI.LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE,
                inactiveDescription: STRINGS.UI.LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE
            );
        }
        private static LogicPorts.Port makeOutputPort()
        {
            return LogicPorts.Port.OutputPort(
                "FULL",
                new CellOffset(2, 1),
                description: SLRStrings.OutputPort,
                activeDescription: SLRStrings.OutputPortActive,
                inactiveDescription: SLRStrings.OutputPortInactive
            );
        }

        private static readonly LogicPorts.Port[] INPUT_PORTS = { makeInputPort() };
        private static readonly LogicPorts.Port[] OUTPUT_PORTS = { makeOutputPort() };

        public override BuildingDef CreateBuildingDef()
        {
            float[] materialCosts = new float[] {
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0],
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
            };
            string[] materials = new string[]
            {
                MATERIALS.REFINED_METAL,
                MATERIALS.PLASTIC
            };
            BuildingDef bd = BuildingTemplates.CreateBuildingDef(
                ID,
                width: 5, height: 3,
                anim: "gasstorage_kanim",
                hitpoints: 100,
                BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER5,
                materialCosts, materials,
                BUILDINGS.MELTING_POINT_KELVIN.TIER1,
                BuildLocationRule.OnFloor,
                BUILDINGS.DECOR.PENALTY.TIER1,
                NOISE_POLLUTION.NOISY.TIER1
            );
            bd.InputConduitType = ConduitType.Gas;
            bd.OutputConduitType = ConduitType.Gas;
            bd.Floodable = false;
            bd.ViewMode = OverlayModes.GasConduits.ID;
            bd.AudioCategory = AUDIO.HOLLOW_METAL;
            bd.UtilityInputOffset = new CellOffset(1, 2);
            bd.UtilityOutputOffset = new CellOffset(0, 0);
            bd.PermittedRotations = PermittedRotations.FlipH;

            bd.RequiresPowerInput = true;
            bd.EnergyConsumptionWhenActive = 30.0f;
            bd.PowerInputOffset = new CellOffset(1, 0);
            return bd;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Debug.Log("=== GasReservoirSmart.ConfigureBuildingTemplate ===");
            Storage storage = BuildingTemplates.CreateDefaultStorage(go);
            storage.showDescriptor = true;
            storage.allowItemRemoval = false;
            storage.storageFilters = STORAGEFILTERS.GASES;
            storage.capacityKg = 90.0f;
            storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);

            go.AddOrGet<ReservoirSmart>();

            ConduitConsumer cc = go.AddOrGet<ConduitConsumer>();
            cc.conduitType = ConduitType.Gas;
            cc.ignoreMinMassCheck = true;
            cc.forceAlwaysSatisfied = true;
            cc.alwaysConsume = true;
            cc.capacityKG = storage.capacityKg;

            ConduitDispenser cd = go.AddOrGet<ConduitDispenser>();
            cd.conduitType = ConduitType.Gas;
            cd.elementFilter = null;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS, OUTPUT_PORTS);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS, OUTPUT_PORTS);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS, OUTPUT_PORTS);
            go.AddOrGetDef<StorageController.Def>();
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }

    }
}
