using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
    void Update()
    {
        if (IsServer)
        {
            float theta = Time.time;
            transform.position = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0f);
        }
    }
}