using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEDPanel : MonoBehaviour {
    private Image[] LEDArray;
    public int LEDArraySize = 8;

    public Transform Grid;
	// Use this for initialization
	void Start () {
        Init();
	}

    public void Init()
    {
        LEDArray = new Image[LEDArraySize * LEDArraySize];
        var leds = Grid.GetComponentsInChildren<Image>();
        for (int lcv = 0, count = leds.Length; lcv < count; lcv++)
        {
            LEDArray[lcv] = leds[lcv];
        }
        ClearLED();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public Color OnColor;
    //public Color DoubleColor;
    //public Color TripleColor;
    public Color OffColor;

    public void LEDLoop(Action<int, int, bool> gridAction)
    {
        for (int x = 0; x < LEDArraySize; x++)
        {
            for (int y = 0; y < LEDArraySize; y++)
            {
                gridAction(x, y, false);
                //yield return null;
            }
        }
    }

    public void ClearLED()
    {
        LEDLoop((x, y, on) =>
        {
            SetLED(x, y, false);
        });
        //for (int x = 0; x < LEDArraySize; x++)
        //{
        //    for (int y = 0; y < LEDArraySize; y++)
        //    {
        //        SetLED(x, y, false);
        //    }
        //}
    }

    public void SetLED(int x, int y, bool on)
    {
        var c = on ? OnColor : OffColor;
        var index = ((y * LEDArraySize) + x);
        LEDArray[index].color = c;
    }

    public void DrawLine(Vector2 start, Vector2 end, bool on)
    {
        int x1, x2, y1, y2;
        x1 = start.x < end.x ? x1 = (int)start.x : x1 = (int)end.x;
        x2 = start.x > end.x ? x2 = (int)start.x : x2 = (int)end.x;
        y1 = start.y < end.y ? y1 = (int)start.y : y1 = (int)end.y;
        y2 = start.y > end.y ? y2 = (int)start.y : y2 = (int)end.y;

        for (int x = x1; x <= x2; x++)
        {
            for (int y = y1; y <= y2; y++)
            {
                SetLED(x, y, on);
            }
        }
    }

}
