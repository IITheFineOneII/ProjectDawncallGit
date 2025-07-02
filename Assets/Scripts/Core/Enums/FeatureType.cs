using UnityEngine;

[System.Flags]
public enum FeatureType
{
    None = 0,
    Forest = 1 << 0,  // 1
    River = 1 << 1,  // 2
    Lake = 1 << 2,  // 4
    Hill = 1 << 3,  // 8
    Jungle = 1 << 4   // 16
}