using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

[ExecuteAlways]
public class Follower : MonoBehaviour
{
    public Transform target;
    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position;
    }
}