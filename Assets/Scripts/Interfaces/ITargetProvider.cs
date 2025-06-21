using Unity.Netcode;
using UnityEngine;

public interface ITargetProvider
{
    NetworkObject GetTarget(Vector3 fromPosition);
}