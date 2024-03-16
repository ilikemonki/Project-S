using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Upgrades : MonoBehaviour
{
    [System.Serializable]
    public class LevelModifiers
    {
        public List<UpdateStats.Modifier> modifier = new();
        public List<float> amt = new();
    }
    public ItemDescription itemDescription;
    public List<LevelModifiers> levelModifiersList = new();
    public void Start()
    {
        //Check if gameobject has itemDescription script, then set it.
        if (itemDescription == null)
        {
            ItemDescription itemDesc = gameObject.GetComponent<ItemDescription>();
            if (itemDesc != null)
            {
                itemDescription = itemDesc;
            }
        }
    }
}
