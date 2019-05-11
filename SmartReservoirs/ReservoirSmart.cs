using UnityEngine;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ReservoirSmart : KMonoBehaviour, IUserControlledCapacity
{
    private MeterController meter;

    [MyCmpGet]
    private LogicPorts ports = null;

    [MyCmpGet]
    private Storage storage = null;

    [Serialize]
    private float userMaxCapacity = float.PositiveInfinity;
    public float UserMaxCapacity
    {
        get => Mathf.Min(userMaxCapacity, storage.capacityKg);
        set
        {
            userMaxCapacity = value;
            UpdateLogicState();
        }
    }

    public float AmountStored => storage.MassStored();
    public float MinCapacity => 0.0f;
    public float MaxCapacity => storage.capacityKg;
    public bool WholeValues => false;
    public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

    protected override void OnSpawn()
    {
        Debug.Log("ReservoirSmart.OnSpawn");
        base.OnSpawn();

        var animCtrl = GetComponent<KBatchedAnimController>();

        animCtrl.TintColour = new Color32(130, 255, 165, 255);

        meter = new MeterController(animCtrl, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_fill", "meter_OL");
        Debug.Log("ReservoirSmart.OnSpawn subscribe");
        Subscribe((int)GameHashes.OnStorageChange, UpdateLogicStateDelegate);
        Subscribe((int)GameHashes.OperationalChanged, UpdateLogicStateDelegate);
        UpdateLogicState();

    }

    /*
     * For some reason, using a delegate or lambda for this causes a cryptic assertion in the output log, and I have no idea why.
     * For the time being, defining this as a method works around the issue.
     */
    private static void UpdateLogicStateAction(ReservoirSmart sr, object o)
    {
        sr.UpdateLogicState();
    }

    private static readonly EventSystem.IntraObjectHandler<ReservoirSmart> UpdateLogicStateDelegate = new EventSystem.IntraObjectHandler<ReservoirSmart>(UpdateLogicStateAction);

    private void UpdateLogicState()
    {
        var pos = Mathf.Clamp01(storage.MassStored() / storage.capacityKg);
        meter.SetPositionPercent(pos);
        ports.SendSignal("FULL", storage.MassStored() >= UserMaxCapacity ? 1 : 0);
    }
};


