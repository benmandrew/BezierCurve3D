using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class BezierCurveController : MonoBehaviour {

    List<Vector3> controlPoints;
    int drawPointIndex;
    int order;

    List<GameObject> spheres;

    LineRenderer curveRenderer;

    List<Vector3> curvePoints;
    int curvePointNum;

    Text orderText;

    List<ColorPair> complementaryPairs;

	// Use this for initialization
	void Start () {
        //order = 10;
        orderText = GameObject.FindGameObjectWithTag("Order Text").GetComponent<Text>();
        createColorPairs();
        generateCurve();
    }

    void generateCurve() {
        order = Random.Range(2, 10);
        controlPoints = new List<Vector3>();
        drawPointIndex = calcDrawPointIndex();
        initialisePoints();
        //initialiseSpheres();

        curvePointNum = 200; // Change number of positions in line renderer inspector box as well
        calcCurvePoints();
        centreCurve();
        sendCurveToRenderer();
        orderText.text = "Order: " + order.ToString();
    }

    int calcDrawPointIndex() {
        int sum = 0;
        int pointsInLayer = order + 1;
        while (pointsInLayer > 1) {
            sum += pointsInLayer;
            pointsInLayer--;
        }
        return sum;
    }

    void initialisePoints() {
        initialiseLayer(order + 1);
    }

    void initialiseLayer(int pointNumInLayer) {
        bool topLayer = order == pointNumInLayer - 1;
        for (int i = 0; i < pointNumInLayer; i++) {
            Vector3 point;
            if (topLayer) {
                point = Random.insideUnitSphere * 5;
            } else {
                point = controlPoints[i];
            }
            controlPoints.Add(point);
        }
        if (pointNumInLayer > 1) {
            initialiseLayer(pointNumInLayer - 1);
        }
    }

    void initialiseSpheres() {
        spheres = new List<GameObject>();
        for (int i = 0; i < controlPoints.Count; i++) {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = controlPoints[i];
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            spheres.Add(sphere);
        }
    }

    void createColorPairs() {
        float c = 255f;
        Color yellow = new Color(0xFC / c, 0xFE / c, 0x04 / c);
        Color amber = new Color(0xFC / c, 0x9A / c, 0x04 / c);
        Color orange = new Color(0xFC / c, 0x66 / c, 0x05 / c);
        Color azure = new Color(0xFC / c, 0x32 / c, 0x04 / c);
        Color red = new Color(0xCC / c, 0x02 / c, 0x04 / c);
        Color magenta = new Color(0x9C / c, 0x02 / c, 0x64 / c);
        Color purple = new Color(0x64 / c, 0x02 / c, 0x64 / c);
        Color violet = new Color(0x04 / c, 0x03 / c, 0x64 / c);
        Color blue = new Color(0x04 / c, 0x32 / c, 0x9C / c);
        Color cyan = new Color(0x04 / c, 0x66 / c, 0x64 / c);
        Color green = new Color(0x34 / c, 0x9A / c, 0x04 / c);
        Color lime = new Color(0x64 / c, 0xCE / c, 0x04 / c);

        complementaryPairs = new List<ColorPair>();
        complementaryPairs.Add(new ColorPair(yellow, purple));
        complementaryPairs.Add(new ColorPair(amber, violet));
        complementaryPairs.Add(new ColorPair(orange, blue));
        complementaryPairs.Add(new ColorPair(azure, cyan));
        complementaryPairs.Add(new ColorPair(red, green));
        complementaryPairs.Add(new ColorPair(magenta, lime));
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            generateCurve();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
	}
    
    void calcCurvePoints() {
        curvePoints = new List<Vector3>();
        float f_curvePointNum = curvePointNum;
        for (int i = 0; i < curvePointNum + 1; i++) {
            float t = (i / f_curvePointNum);
            interpolateLayer(order, 0, t);
            curvePoints.Add(controlPoints[drawPointIndex]);
        }
    }
    
    void interpolateLayer(int pointNumInLayer, int indexOffset, float t) {
        for (int i = 0; i < pointNumInLayer; i++) {
            Vector3 pos1 = controlPoints[indexOffset + i];
            Vector3 pos2 = controlPoints[indexOffset + i + 1];
            Vector3 newPos = Vector3.Lerp(pos1, pos2, t);
            controlPoints[indexOffset + pointNumInLayer + i + 1] = newPos;
            //spheres[indexOffset + pointNumInLayer + i + 1].transform.position = newPos;
        }
        if (pointNumInLayer > 1) {
            interpolateLayer(pointNumInLayer - 1, indexOffset + pointNumInLayer + 1, t);
        }
    }

    void centreCurve() {
        Vector3 average = Vector3.zero;
        foreach (Vector3 curvePoint in curvePoints) {
            average += curvePoint;
        }
        average /= curvePoints.Count;
        for (int i = 0; i < curvePoints.Count; i++) {
            curvePoints[i] -= average;
        }
    }

    void sendCurveToRenderer() {
        curveRenderer = GameObject.FindGameObjectWithTag("Curve Renderer").GetComponent<LineRenderer>();
        Assert.AreEqual(curvePointNum, curveRenderer.positionCount);
        curveRenderer.SetPositions(curvePoints.ToArray());

        curveRenderer.colorGradient = getGradient();
        //curveRenderer.Simplify();
    }

    Gradient getGradient() {
        int index = Random.Range(0, complementaryPairs.Count);
        ColorPair colorPair = complementaryPairs[index];
        print(colorPair.a);
        print(colorPair.b);
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(colorPair.a, 0.0f), new GradientColorKey(colorPair.b, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        return gradient;
    }
}

struct ColorPair {
    public Color a;
    public Color b;

    public ColorPair(Color newA, Color newB) {
        a = newA;
        b = newB;
    }
}