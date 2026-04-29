using UnityEngine;
using Fusion;

public class PlayerData : NetworkBehaviour
{
    [Networked] public bool IsReady { get; private set; } = false;

    //staticは違うパソコンとは共有しない　
    //Aさんのパソコンから見たLocalはA,Bさんのパソコンから見たLocalはB...
    public static PlayerData Local { get; private set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetReady(bool ready)
    {
        IsReady = ready;
    }
}
