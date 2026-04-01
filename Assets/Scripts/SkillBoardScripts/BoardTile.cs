using UnityEngine;

public enum AspectType
{
    None,
    Power,
    Flow,
    Sp
}

[System.Serializable]
public class BoardTile
{
    public Vector2Int boardPosition;
    public bool isUnlocked;
    public AspectType aspectType;
    public Ability placedAbility;
}
