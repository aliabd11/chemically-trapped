#if UFPS_MULTIPLAYER

using System.Collections.Generic;
using Devdog.General;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro.Integration.UFPS.Multiplayer
{
    [UnityEngine.RequireComponent(typeof(LootableObject))]
    [UnityEngine.RequireComponent(typeof(PhotonView))]
    [UnityEngine.AddComponentMenu(InventoryPro.AddComponentMenuPath + "Integration/UFPS/PhotonLootableObjectSyncer")]
    public partial class PhotonLootableObjectSyncer : Photon.MonoBehaviour, ITriggerCallbacks
    {
        private LootableObject _lootable { get; set; }
        private TriggerBase _trigger { get; set; }

        private List<PhotonPlayer> _activeUsers = new List<PhotonPlayer>();

        protected virtual void Awake()
        {
            _lootable = GetComponent<LootableObject>();
            _lootable.OnLootedItem += LootableOnOnLootedItem;

            _trigger = GetComponent<TriggerBase>();
        }

        public bool OnTriggerUsed(Player player)
        {
            if (PhotonNetwork.isMasterClient == false)
            {
                _lootable.items = new InventoryItemBase[0]; // Clear the items, wait for the server to respond with the item list.

                // Request the data from the server
                DevdogLogger.LogVerbose("Client used LootableObject - Requesting item list", player);
                photonView.RPC("RequestLootableItems", PhotonTargets.MasterClient);
            }

            DevdogLogger.LogVerbose("Client used trigger -> Notify other clients", player);
            photonView.RPC("OnTriggerUsedByOtherClient", PhotonTargets.Others);

            return false;
        }

        public bool OnTriggerUnUsed(Player player)
        {
            DevdogLogger.LogVerbose("Client un-used trigger -> Notify other clients", player);
            photonView.RPC("OnTriggerUnUsedByOtherClient", PhotonTargets.Others);

            return false;
        }


        [PunRPC]
        protected void OnTriggerUsedByOtherClient(PhotonMessageInfo info)
        {
            _trigger.DoVisuals();

            if (PhotonNetwork.isMasterClient)
            {
                _activeUsers.Add(info.sender);
            }
        }

        [PunRPC]
        protected void OnTriggerUnUsedByOtherClient(PhotonMessageInfo info)
        {
            _trigger.UndoVisuals();

            if (PhotonNetwork.isMasterClient)
            {
                _activeUsers.Remove(info.sender);
            }
        }

        [PunRPC]
        protected void SetLootableItems(string itemIDsString, PhotonMessageInfo info)
        {
            Assert.IsTrue(info.sender.ID == PhotonNetwork.masterClient.ID, "SetLootableItems didn't come from masterClient!");
            DevdogLogger.LogVerbose("LootableObject - Setting items list: " + itemIDsString);

            foreach (var item in _lootable.items)
            {
                Destroy(item.gameObject); // Destroy old items that were still here...
            }

            if (string.IsNullOrEmpty(itemIDsString))
            {
                _lootable.items = new InventoryItemBase[0];
                return;
            }

            var items = GetItemsFromNetworkingString(itemIDsString);

            // Set items
            _lootable.items = items; // Set the lootable items for this object.

            // Update loot window
            _lootable.lootUI.SetItems(_lootable.items, false);
            foreach (var cur in _lootable.currencies)
            {
                _lootable.lootUI.AddCurrency(cur.currency, cur.amount);
            }

            _lootable.lootUI.window.Show();
        }

        private void LootableOnOnLootedItem(InventoryItemBase item, uint itemId, uint slot, uint amount)
        {
            DevdogLogger.LogVerbose("LootableObject - Item got removed from LootableObject", this);
            photonView.RPC("LootableObjectItemRemoved", PhotonTargets.OthersBuffered, (int)slot);
        }

        [PunRPC]
        private void LootableObjectItemRemoved(int slot)
        {
            DevdogLogger.LogVerbose("LootableObject - Other client looted slot: " + slot, transform);
            if (slot < 0 || slot >= _lootable.items.Length)
            {
                UnityEngine.Debug.LogWarning("Item is out of range " + slot + " can't set item");
                return;
            }

            _lootable.items[slot] = null;
            if (_lootable.lootUI.items.Length == _lootable.items.Length)
            {
                _lootable.lootUI.SetItem((uint)slot, null, true);
            }
        }

        [PunRPC]
        private void RequestLootableItems(PhotonMessageInfo info)
        {
            DevdogLogger.LogVerbose("LootableObject - Client requested LootableObject item list", this);

            string result = GetItemsAsNetworkString();
            photonView.RPC("SetLootableItems", info.sender, result);
        }

        private InventoryItemBase[] GetItemsFromNetworkingString(string itemIDsString)
        {
            string[] itemIDs = itemIDsString.Split(',');
            var items = new InventoryItemBase[itemIDs.Length];

            for (int i = 0; i < itemIDs.Length; i++)
            {
                if (itemIDs[i] == "-1")
                {
                    items[i] = null;
                    continue;
                }

                var x = itemIDs[i].Split(':');
                UnityEngine.Debug.Log(itemIDs[i]);
                
                var item = Instantiate<InventoryItemBase>(ItemManager.database.items[int.Parse(x[0])]);
                item.gameObject.SetActive(false);
                item.transform.SetParent(transform);
                item.currentStackSize = uint.Parse(x[1]);
                items[i] = item;
            }

            return items;
        }

        private string GetItemsAsNetworkString()
        {
            // Master defines the items to be looted, and sends it to the clients.
            return string.Join(",", _lootable.items.Select(x => x == null ? ("-1:0") : (x.ID.ToString() + ":" + x.currentStackSize.ToString())).ToArray()); // Concat as a string, because photon is being bitchy about int arrays
        }
    }
}

#endif