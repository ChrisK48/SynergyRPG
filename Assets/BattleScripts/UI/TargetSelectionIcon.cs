using UnityEngine;

public class TargetSelectionIcon : MonoBehaviour
{
    private ITurnEntity associatedEntity;
    private TargetSelectionManager targetSelectionManager;
    private CharBattle[] users;
    private ITargetableAction action;

    public void Setup(ITurnEntity entity, CharBattle[] userArray, ITargetableAction action, TargetSelectionManager manager)
    {
        associatedEntity = entity;
        users = userArray;
        this.action = action;
        targetSelectionManager = manager;
        Vector3 worldPos = GetTargetPosition(entity);
        transform.position = worldPos + Vector3.up;
    }

    private void OnMouseDown()
    {
        targetSelectionManager.OnTargetSelected(users, action, associatedEntity);
    }

    Vector3 GetTargetPosition(ITurnEntity entity)
    {
        if (entity is MonoBehaviour mb) return mb.transform.position;
        if (entity is SynergyStance stance) return stance.users[0].transform.position; // Or midpoint
        return Vector3.zero;
    }
}
