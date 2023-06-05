﻿namespace UnityMiniGameFramework
{
    public enum EventTriggerType
    {
        /// <summary>
        /// Intercepts a IPointerEnterHandler.OnPointerEnter.
        /// </summary>
        PointerEnter = 0,

        /// <summary>
        /// Intercepts a IPointerExitHandler.OnPointerExit.
        /// </summary>
        PointerExit = 1,

        /// <summary>
        /// Intercepts a IPointerDownHandler.OnPointerDown.
        /// </summary>
        PointerDown = 2,

        /// <summary>
        /// Intercepts a IPointerUpHandler.OnPointerUp.
        /// </summary>
        PointerUp = 3,

        /// <summary>
        /// Intercepts a IPointerClickHandler.OnPointerClick.
        /// </summary>
        PointerClick = 4,

        /// <summary>
        /// Intercepts a IDragHandler.OnDrag.
        /// </summary>
        Drag = 5,

        /// <summary>
        /// Intercepts a IDropHandler.OnDrop.
        /// </summary>
        Drop = 6,

        /// <summary>
        /// Intercepts a IScrollHandler.OnScroll.
        /// </summary>
        Scroll = 7,

        /// <summary>
        /// Intercepts a IUpdateSelectedHandler.OnUpdateSelected.
        /// </summary>
        UpdateSelected = 8,

        /// <summary>
        /// Intercepts a ISelectHandler.OnSelect.
        /// </summary>
        Select = 9,

        /// <summary>
        /// Intercepts a IDeselectHandler.OnDeselect.
        /// </summary>
        Deselect = 10,

        /// <summary>
        /// Intercepts a IMoveHandler.OnMove.
        /// </summary>
        Move = 11,

        /// <summary>
        /// Intercepts IInitializePotentialDrag.InitializePotentialDrag.
        /// </summary>
        InitializePotentialDrag = 12,

        /// <summary>
        /// Intercepts IBeginDragHandler.OnBeginDrag.</
        /// </summary>
        BeginDrag = 13,

        /// <summary>
        /// Intercepts IEndDragHandler.OnEndDrag.
        /// </summary>
        EndDrag = 14,

        /// <summary>
        /// Intercepts ISubmitHandler.Submit.
        /// </summary>
        Submit = 15,

        /// <summary>
        /// Intercepts ICancelHandler.OnCancel.
        /// </summary>
        Cancel = 16,
        
        /// <summary>
        /// Intercepts IInitializePotentialPinchHandler.OnInitializePotentialPinch.
        /// </summary>
        InitializePotentialPinch = 17,
        
        /// <summary>
        /// Intercepts IBeginPinchHandler.OnBeginPinch.
        /// </summary>
        BeginPinch = 18,
        
        /// <summary>
        /// Intercepts IPinchHandler.OnPinch.
        /// </summary>
        Pinch = 19,
        
        /// <summary>
        /// Intercepts IEndPinchHandler.OnEndPinch.
        /// </summary>
        EndPinch = 20
    }
}