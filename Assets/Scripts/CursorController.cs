using UnityEngine;
using System.Collections;

public class CursorController : MonoBehaviour {
    RectTransform rTransform;
    // Use this for initialization
    void Start () {
        rTransform = GetComponent<RectTransform>();
        
	}
	
	// Update is called once per frame
	void Update () {
        rTransform.localPosition = new Vector3(Input.mousePosition.x - Screen.width/2f, Input.mousePosition.y - Screen.height/2f, 0);
        Cursor.visible = false;
        //rTransform.localPosition = Screen.WidthInput.mousePosition;
    }
}
