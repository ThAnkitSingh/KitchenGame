using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;//to be instantiated can be Transform or Game object

    //do this when we press E key
    //instantiate the prefab on top of CC
    public override void Intract(Player player)
    {   
        if(!HasKitchenObject())
        {
            //there is no KO here
            if(player.HasKitchenObject())
            {
                //Player has KO
                player.GetKitchenObject().SetIKitchenObjectParent(this);
            }
            else
            {
                //player has nothing
            }
        }
        else
        {
            //there is KO
            if (player.HasKitchenObject())
            {
                //Player has KO
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if(plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }                    
                }
                else
                {//playes does not have plate
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {//KO on the Clear counter is a plate
                       if(plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                       {
                            player.GetKitchenObject().DestroySelf();
                       }
                    }

                }
            }
            else
            {
                //player has nothing
                GetKitchenObject().SetIKitchenObjectParent(player);
            }
        }
    }
}
