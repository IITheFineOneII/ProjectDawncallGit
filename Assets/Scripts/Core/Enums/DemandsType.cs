using UnityEngine;

[System.Flags]
public enum DemandsType
{
    None = 0,  // 0
    Food = 1 << 0, //1
    Water = 1 << 1, //2
    Safety = 1 << 2, //4
    Entertainment = 1 << 3, //8
    Magic = 1 << 4, //16
    Religion = 1 << 5, //32
}