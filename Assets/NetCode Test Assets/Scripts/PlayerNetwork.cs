using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float xmin, xmax, zmin, zmax;
    private static NetworkVariable<int> val = new NetworkVariable<int>(1, writePerm : NetworkVariableWritePermission.Owner);
    private int value = -1;
    private int syncedVal = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        val.OnValueChanged += (int val, int newVal) =>
        {
            //this.val.Value = newVal;
            Debug.Log($"Client id : {OwnerClientId}, new val : {newVal}");
        }; 
    }


    private void Start()
    {
        SpawnRandom();
    }

    private void Update()
    {

        if (!IsOwner) 
            return;


        if (Input.GetKeyDown(KeyCode.T))
            val.Value++;


        if (Input.GetKeyDown(KeyCode.C))
            IncreaseValueServerRpc();


        if(Input.GetKeyDown(KeyCode.Q))
        {
            value++;
            Debug.Log($"clientID : {OwnerClientId}, value : {value}");
            BoardUpdatedServerRPC(value);
        }



        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDir.z = 1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = 1f;

        float moveSpeed = 3f;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
        
    }


    private void SpawnRandom()
    {
        transform.position = new Vector3(UnityEngine.Random.Range(xmin, xmax), 0, UnityEngine.Random.Range(zmin, zmax));
        Debug.Log(transform.position);
    }


    [ServerRpc]
    private void IncreaseValueServerRpc()
    {
        IncreaseValueClientRpc();
    }


    [ClientRpc]
    private void IncreaseValueClientRpc()
    {
        syncedVal++;
        Debug.Log($"id : {OwnerClientId}, value : {syncedVal}");
    }


    [ServerRpc]
    private void BoardUpdatedServerRPC(int val)
    {
        BoardUpdatedClientRPC(val);
    }


    [ClientRpc]
    private void BoardUpdatedClientRPC(int val)
    {
        if(!IsOwner)
        {
            value = val;    
            Debug.Log($"cleint ID : {OwnerClientId}, value: {val}");
        }
    }
}
