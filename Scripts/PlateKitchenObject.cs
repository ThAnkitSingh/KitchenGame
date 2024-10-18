using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredirentAddedEventArgs> OnIngredientAdded;
    public class OnIngredirentAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;
    private List<KitchenObjectSO> kitchenObjectSOList;//currently on the plate 

    private void Awake()
    {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {   //Valid KO
        if(!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }
        //Already in added
        if(kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }
        else//if not added add it
        {
            kitchenObjectSOList.Add(kitchenObjectSO);

            OnIngredientAdded?.Invoke(this,new OnIngredirentAddedEventArgs { kitchenObjectSO = kitchenObjectSO});

            return true;
        }
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}
