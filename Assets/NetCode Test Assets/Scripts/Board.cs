using Unity.Netcode;
using UnityEngine;

public class Board : NetworkBehaviour
{
    private NetworkVariable<int> var = new NetworkVariable<int>(1);


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"id : {OwnerClientId}, ownership : {IsOwner}, server ownership : {IsOwnedByServer}");

        if(!IsServer)
        var.OnValueChanged += ValueIncreased;
    }

    [ContextMenu("Increase Value"), ServerRpc(RequireOwnership = false)]
    private void IncreaseValueServerRPC()
    {
        var.Value++;
        Debug.Log($"id : {OwnerClientId} value : {var.Value}");
    }

    private void Update()
    {
        if (!IsOwner) return;
    }

    private void ValueIncreased(int prevVal, int newVal)
    {
        Debug.Log($"value : {var.Value}");
    }
}
