using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAlignmentController : MonoBehaviour {

    RectTransform rTransform;
    // Use this for initialization
    void Start()
    {
        rTransform = GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        rTransform.localPosition = new Vector3(100 - (Screen.width / 2f), 0, 0);
        
        //rTransform.localPosition = Screen.WidthInput.mousePosition;
    }
}
