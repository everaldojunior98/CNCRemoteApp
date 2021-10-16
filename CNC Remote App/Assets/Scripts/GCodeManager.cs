using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GCodeManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform usefulArea;

    private const float FilterDistance = 0.5f;
    private const float PixelIncrease = 10f;

    //mm
    private float MaxX = 250;
    private float MaxY = 150;

    private float currentX;
    private float currentY;

    private DrawManager drawManager;
    private Camera mainCamera;

    private void Start()
    {
        drawManager = FindObjectOfType<DrawManager>();
        mainCamera = Camera.main;

        var percentage = MaxX / MaxY;
        while (currentX < Screen.width && currentY < Screen.height)
        {
            currentX += PixelIncrease * percentage;
            currentY += PixelIncrease;
        }

        usefulArea.sizeDelta = new Vector2(currentX, currentY);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            GenerateGCode();
    }

    private void GenerateGCode()
    {
        var drawings = drawManager.GetDrawings();

        var filteredDrawings = new List<List<Vector3>>();
        foreach (var points in drawings)
        {
            var filteredPoints = new List<Vector3>();

            var lastPosition = Vector3.positiveInfinity;
            foreach (var point in points)
                if (lastPosition == Vector3.positiveInfinity || Vector3.Distance(lastPosition, point) > FilterDistance)
                {
                    filteredPoints.Add(point);
                    lastPosition = point;
                }

            filteredDrawings.Add(filteredPoints);
        }

        var convertedDrawings = new List<List<Vector2>>();
        foreach (var drawing in filteredDrawings)
        {
            var convertedPoints = new List<Vector2>();
            foreach (var point in drawing)
            {
                var convertedPoint = mainCamera.WorldToScreenPoint(point);
                convertedPoints.Add(new Vector2(convertedPoint.x * MaxX / currentX,
                    convertedPoint.y * MaxY / currentY));
            }
            convertedDrawings.Add(convertedPoints);
        }

        var gcode = new List<string> {"G21", "G90", "G94", "F150.00", "M03 S100", "G4 P1", "G0x0y0z0", "G0z5" };
        foreach (var drawing in convertedDrawings)
        {
            var first = drawing.First();
            gcode.Add($"G0x{ConvertFloat(first.x)}y{ConvertFloat(first.y)}");
            gcode.Add("G0z0");
            foreach (var point in drawing.Skip(1))
                gcode.Add($"G0x{ConvertFloat(point.x)}y{ConvertFloat(point.y)}");
            gcode.Add("G0z5");
        }

        Debug.Log(string.Join("\n", gcode));
    }

    private string ConvertFloat(float value)
    {
        return value.ToString("F2", CultureInfo.InvariantCulture);
    }
}