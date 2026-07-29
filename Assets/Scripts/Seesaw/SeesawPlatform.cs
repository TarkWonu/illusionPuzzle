using UnityEngine;

public class SeesawPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 posUp;
    [SerializeField] private Vector3 posDown;

    public Vector3 PosUp => posUp;
    public Vector3 PosDown => posDown;
}