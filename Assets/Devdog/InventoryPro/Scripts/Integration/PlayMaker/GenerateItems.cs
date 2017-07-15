#if PLAYMAKER

using HutongGames.PlayMaker;
#if UNITY_EDITOR

#endif

namespace Devdog.InventoryPro.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Generate a bunch of items that can be stored in a collection.")]
    public class GenerateItems : FsmStateAction
    {
        public FsmInt minAmount, maxAmount;
        
        public InventoryItemBase[] items;

        protected IItemGenerator generator;

        public override void Reset()
        {

        }

        public override void Awake()
        {
            base.Awake();
            generator = new BasicItemGenerator();

            generator.SetItems(ItemManager.database.items);
            generator.Generate(minAmount.Value, maxAmount.Value);
        }


        public override void OnEnter()
        {
            // Do something with items

            Finish();
        }

        public override bool Event(FsmEvent fsmEvent)
        {
            return base.Event(fsmEvent);

        }
    }
}

#endif