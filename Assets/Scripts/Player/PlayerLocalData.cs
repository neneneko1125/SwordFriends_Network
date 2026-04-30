using UnityEngine;
using Fusion;

/// <summary>
/// SOデータを配る　
/// InstanceDataを生成し、それぞれのクラスのSetupを実行する(InstanceDataを配る)
/// Localを使用し、複数人いるプレイヤーから特定のプレイヤーを指定できる
/// 準備完了状態かどうかを制御する
/// </summary>
public class PlayerLocalData : NetworkBehaviour
{
    [SerializeField] private PlayerData _playerData;

    [Networked] public bool IsReady { get; private set; } = false;

    //staticは違うパソコンとは共有しない　
    //Aさんのパソコンから見たLocalはA,Bさんのパソコンから見たLocalはB
    public static PlayerLocalData Local { get; private set; }

    public override void Spawned()
    {
        var instanceData = new PlayerInstanceData(_playerData);

        if (TryGetComponent<BaseHP>(out var hp))
        {
            hp.Setup(instanceData);
        }

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
