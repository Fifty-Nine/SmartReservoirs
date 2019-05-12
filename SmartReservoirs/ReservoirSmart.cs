using UnityEngine;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ReservoirSmart : KMonoBehaviour, IUserControlledCapacity, ISim1000ms
{
    private MeterController meter;

    [MyCmpGet]
    private LogicPorts ports = null;

    [MyCmpGet]
    private Operational operational = null;

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
        Subscribe((int)GameHashes.OnStorageChange, UpdateLogicStateDelegate);
        Subscribe((int)GameHashes.OperationalChanged, UpdateLogicStateDelegate);
        UpdateLogicState();
    }

    protected override void OnCleanUp()
    {
        Unsubscribe((int)GameHashes.OnStorageChange, UpdateLogicStateDelegate);
        Unsubscribe((int)GameHashes.OperationalChanged, UpdateLogicStateDelegate);
        base.OnCleanUp();
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

    /*
     * Get the current output signal state.
     */
    private int SignalState()
    {
        var stored = storage.MassStored();
        return (stored != 0.0f && stored >= UserMaxCapacity) ? 1 : 0;
    }

    /*
     * Determine what the the current operational.IsActive state should be according to the
     * circumstances.
     */
    private bool ActiveState()
    {
        return operational.IsOperational && storage.MassStored() > 0.0f &&
            (!ports.IsPortConnected(LogicOperationalController.PORT_ID) || ports.GetInputValue(LogicOperationalController.PORT_ID) == 1);
    }

    private void UpdateLogicState()
    {
        var pos = Mathf.Clamp01(storage.MassStored() / storage.capacityKg);
        meter.SetPositionPercent(pos);
        ports.SendSignal("FULL", SignalState());
    }

    float elapsedTime = 0.0f;
    public void Sim1000ms(float dt)
    {
        elapsedTime += dt;
        if (elapsedTime >= 1.0f)
        {
            operational.SetActive(ActiveState());
            elapsedTime = 0.0f;
        }
    }
};
