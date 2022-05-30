using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SphereModel : MonoBehaviour
{
    public Vector3 center{ get => this.transform.position; }
    public float radius{ get => 0.6f * this.transform.lossyScale.x; }
}
