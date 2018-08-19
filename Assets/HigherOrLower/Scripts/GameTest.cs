using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GameTest : MonoBehaviour {

    [Range(0, 7)]
    public int startX, startY;
    [Range(0, 64)]
    public int count;
    public bool invertX, invertY;
    public bool Redraw, Clear, RedrawLock;
    public LEDRendering.GraphType GraphType;
    public LEDPanel Panel;
    // Use this for initialization
    void Start () {
        Panel.Init();

	}
	
	// Update is called once per frame
	void Update () {
        if (Clear)
        {
            Panel.ClearLED();
            Clear = false;
            Redraw = false;
            RedrawLock = false;
        }
		if (Redraw  || RedrawLock)
        {
            Redraw = false;
            LEDRendering.Graph(GraphType, Panel, startX, startY, count, invertX, invertY);
        }
	}
}
