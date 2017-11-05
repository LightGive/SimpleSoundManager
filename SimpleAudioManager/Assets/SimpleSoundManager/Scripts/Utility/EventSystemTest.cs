using UnityEngine;
using UnityEngine.EventSystems;
 
public class EventSystemTest :
	MonoBehaviour,
	IPointerEnterHandler,
	IPointerExitHandler,
	IPointerDownHandler,
	IPointerUpHandler,
	IPointerClickHandler,
	IBeginDragHandler,
	IEndDragHandler,
	IDragHandler,
	IDropHandler,
	IScrollHandler,
	IMoveHandler,
	ISelectHandler,
	IDeselectHandler,
	IUpdateSelectedHandler,
	ISubmitHandler,
	ICancelHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log(" OnPointerEnter: " +eventData );
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log(" OnPointerExit: " +eventData );
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log(" OnPointerDown: " +eventData );
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Debug.Log(" OnPointerUp: " +eventData );
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log(" OnPointerClick: "+eventData );
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.Log(" OnBeginDrag: " +eventData );
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log(" OnEndDrag: " +eventData );
	}

	public void OnDrag(PointerEventData eventData)
	{
		Debug.Log(" OnDrag: " +eventData );
	}

	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log(" OnDrop: " +eventData );
	}

	public void OnScroll(PointerEventData eventData)
	{
		Debug.Log(" OnScroll: " +eventData );
	}

	public void OnMove(AxisEventData eventData)
	{
		Debug.Log(" OnMove: " +eventData );
	}

	bool _UpdateSelected = false;

	public void OnSelect(BaseEventData eventData)
	{
		Debug.Log(" OnSelect: " +eventData );

		_UpdateSelected = false;
	}

	public void OnDeselect(BaseEventData eventData)
	{
		Debug.Log(" OnDeselect: " +eventData );
	}

	public void OnUpdateSelected(BaseEventData eventData)
	{
		if (!_UpdateSelected)
		{
			Debug.Log(" OnUpdateSelected: " +eventData );
			_UpdateSelected = true;
		}
	}

	public void OnSubmit(BaseEventData eventData)
	{
		Debug.Log(" OnSubmit: " +eventData );
	}

	public void OnCancel(BaseEventData eventData)
	{
		Debug.Log(" OnCancel: " +eventData );
	}
}