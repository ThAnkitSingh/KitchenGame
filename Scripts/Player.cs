using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler OnPickedSomethig;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSeletecCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }


    [SerializeField] private float movespeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;//place to instantiate

    private bool isWalking;//for animation     
    private Vector3 lastIntractDir;//Last intration direction
    private BaseCounter selectedCounter;//place holder for selected counter we can intract with
    private KitchenObject kitchenObject;

    private void Awake()
    {   
        if(Instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;//event triggerd on pressing E key
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;//F Key
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {   //pressing E key and if any counter is in the selected counter then we do something 
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Intract(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        //handeling intraction between player and clear counter


        Vector2 inputVector = gameInput.GameMovementVectorNormalized();

        //make Vector2 apply on Vector3 by making V3x=V2x V3y=0 V3z=V2y
        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        //storing last direction since movDir becomes 0 if not pressing any keys
        if(movDir != Vector3.zero)
        {
            lastIntractDir= movDir;
        }

        //Raycasting to see the clear counter
        float intractionDistance = 2f;
        if(Physics.Raycast(transform.position, lastIntractDir, out RaycastHit raycastHit, intractionDistance, layerMask))
        {   
            //checking of clear counter is in front of player
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //if clear counter is in front then set it as selected counter if selected counter is not clear counter
                if(baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {//if selected counter is not clear counter then set selected counter as null 
                SetSelectedCounter(null);
            }
        }
        else
        {//if no counter in front then set selected counter as null
            SetSelectedCounter(null);
        }
    }
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GameMovementVectorNormalized();

        //make Vector2 apply on Vector3 by making V3x=V2x V3y=0 V3z=V2y
        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        //CapsuleCast 
        float moveDistance = movespeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = .2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movDir, moveDistance);

        if (!canMove)
        {
            //attempt to move in x directions right
            Vector3 movDirX = new Vector3(movDir.x, 0f, 0f).normalized;
            canMove = (movDir.x < -.5f || movDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movDirX, moveDistance);

            if (canMove)
            {
                movDir = movDirX;
            }
            else if (!canMove)
            {
                //attempt to move in z directions left
                Vector3 movDirZ = new Vector3(0f, 0f, movDir.z).normalized;
                canMove = (movDir.x < -.5f || movDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movDirZ, moveDistance);

                if (canMove)
                {
                    movDir = movDirZ;
                }
                else
                {
                    //no movement
                }

            }

        }

        if (canMove)
        {
            transform.position += movDir * Time.deltaTime * movespeed;
        }

        //for animation
        isWalking = movDir != Vector3.zero;

        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, movDir, Time.deltaTime * rotationSpeed); ;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        //sending selected counter type to anyone subscribed to the conter changed event
        this.selectedCounter = selectedCounter;

        OnSeletecCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if(this.kitchenObject != null)
        {
            OnPickedSomethig?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() 
    { 
        return kitchenObject; 
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
