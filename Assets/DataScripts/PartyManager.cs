using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public List<PlayerCharData> activePartyMembers;
    public int money;
    public List<ItemStack> inventory;
    public static PartyManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


}
