using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileType : MonoBehaviour
{
    public enum Type
    {
        Normal,
        Orange,
        Rodo,
        Creu,
        Entrada,
        Dividir
    }

    public Type tileType;
}
