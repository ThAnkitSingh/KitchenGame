using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangeEvenArgs> OnProgressChange;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler OnCut;

    public static EventHandler OnAnyCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    public override void Intract(Player player)
    {
        if (!HasKitchenObject())
        {
            //there is no KO here
            if (player.HasKitchenObject())
            {   
                if(HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {//if has a cuttable KOSO then drop it
                    //Player has KO
                    player.GetKitchenObject().SetIKitchenObjectParent(this);
                    cuttingProgress= 0;

                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecepieSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChange?.Invoke(this,new IHasProgress.OnProgressChangeEvenArgs { 
                    progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                    });
                }
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
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

    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {//If there is a KO and it can be cut
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecepieSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEvenArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {

                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);

            }
        }
    }

    //tells if the KO is cutable or not
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecepieSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    //gets the output KO for the input KO from the Array of Cutting Recepie SOs
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecepieSOWithInput(inputKitchenObjectSO);
        if(cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingRecipeSO GetCuttingRecepieSOWithInput(KitchenObjectSO inputKitchenObject)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if(cuttingRecipeSO.input == inputKitchenObject)
            {
                return cuttingRecipeSO;
            }            
        }
        return null;
    }
}
