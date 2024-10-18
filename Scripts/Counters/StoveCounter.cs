using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangeEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangeEvenArgs> OnProgressChange;

    public class OnStateChangeEventArgs : EventArgs
    {
       public State state;
    }

    public enum State { 
    
        Idel,
        Frying,
        Fried,
        Burned,

    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State state;
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;


    private void Start()
    {
        state = State.Idel;
        OnStateChanged?.Invoke(this, new OnStateChangeEventArgs { state = state });
    }
    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idel:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEvenArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });

                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {                        
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeSO = GetBurningRecepieSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangeEventArgs { state = state});
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEvenArgs
                    {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                    });

                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangeEventArgs { state = state });

                        OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEvenArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                    break;
                case State.Burned:
                    break;
            }
        }       
    }

    public override void Intract(Player player)
    {
        if (!HasKitchenObject())
        {
            //there is no KO here
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {//if has a fryable KOSO then drop it
                    //Player has KO
                    player.GetKitchenObject().SetIKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingRecepieSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangeEventArgs { state = state });

                    OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEvenArgs {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
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

                        state = State.Idel;

                        OnStateChanged?.Invoke(this, new OnStateChangeEventArgs { state = state });


                        OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEvenArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else
            {
                //player has nothing
                GetKitchenObject().SetIKitchenObjectParent(player);

                state = State.Idel;

                OnStateChanged?.Invoke(this, new OnStateChangeEventArgs { state = state });


                OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEvenArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    //tells if the KO is cutable or not
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecepieSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    //gets the output KO for the input KO from the Array of Cutting Recepie SOs
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecepieSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private FryingRecipeSO GetFryingRecepieSOWithInput(KitchenObjectSO inputKitchenObject)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObject)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecepieSOWithInput(KitchenObjectSO inputKitchenObject)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObject)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }

    public bool IsFried()
    {
        return state == State.Fried;
    }
}
