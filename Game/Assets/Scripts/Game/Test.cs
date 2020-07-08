using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int WidthScreen = 1280;
    public int HeightScreen = 720;
    public int CameraSize = 16;
    public bool debug = true;

    [SerializeField]
    private Transform _background;

    private Grid<int> _grid;

    // Start is called before the first frame update
    void Start()
    {
        var r = (float)WidthScreen / HeightScreen;
        var h = (int)(CameraSize * 2); 
        var w = (int)(r * h);

        Debug.Log("w := " + w + " h := " + h + " r := " + r);

        var wPosition = new Vector3(-(w / 2.0f), -(h/2.0f), 0);

        _grid = new Grid<int>((int)w, (int)h, 1, wPosition, (Grid<int> g, int x, int y) => { return 0; }, debug);

        _background.localScale = new Vector3(h * 0.1f, 1, (r * h) * 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
