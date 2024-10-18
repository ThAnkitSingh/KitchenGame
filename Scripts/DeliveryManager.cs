using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public EventHandler OnRecipeSpawned;
    public EventHandler OnRecipeComplete;
    public EventHandler OnRecipeSuccess;
    public EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer;

    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4;
    public int successfulRecipeAmount;

    private void Awake()
    {

        Instance = this;
        waitingRecipeSOList= new List<RecipeSO>();
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if(KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitngRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                Debug.Log(waitngRecipeSO.recipeName);
                waitingRecipeSOList.Add(waitngRecipeSO);
                OnRecipeSpawned.Invoke(this, EventArgs.Empty);
            }
            
        }
    }

    public void DeliveryRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {//Has same number of ingredients
                bool plateContentMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {//cyceling thru all ingredients in the recipe
                    bool ingredientFount = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {//cyceling thru all ingredients on the plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) 
                        {//ingredient matches!
                            ingredientFount = true;
                            break;
                        }
                    }
                    if (!ingredientFount) 
                    {
                        plateContentMatchesRecipe = false;
                    }
                }
                if(plateContentMatchesRecipe) 
                {//player delived the correct recipe!
                    Debug.Log("Player deliver the correct recipe!");
                    successfulRecipeAmount++;
                    waitingRecipeSOList.RemoveAt(i);
                    OnRecipeComplete.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess.Invoke(this, EventArgs.Empty);
                    return;                
                }
            }
        }
        //No Matches found
        //"Player did not deliver the correct recipe!"
        OnRecipeFailed.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipeAmount()
    {
        return successfulRecipeAmount;
    }
}
