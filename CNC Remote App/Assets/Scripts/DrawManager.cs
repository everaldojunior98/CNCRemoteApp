using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawManager : MonoBehaviour
{
    [SerializeField]
    private GameObject drawingPrefab;

    private LineRenderer lineRenderer;
    private Vector3 lastMousePosition;
    private bool needOptimize;

    private List<Vector3[]> drawings;

    private void Start()
    {
        drawings = new List<Vector3[]>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var drawing = Instantiate(drawingPrefab);
            lineRenderer = drawing.GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
        }

        if (Input.GetMouseButton(0))
            FreeDraw();

        if (needOptimize && Input.GetMouseButtonUp(0))
            OptimizeLine();
    }

    private void FreeDraw()
    {
        needOptimize = true;

        var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);

        if (Vector3.Distance(lastMousePosition, mousePos) > 0.1f)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, Camera.main.ScreenToWorldPoint(mousePos));
            lastMousePosition = mousePos;
        }
    }

    private void OptimizeLine()
    {
        needOptimize = false;
        lineRenderer.Simplify(0.001f);

        var points = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(points);
        drawings.Add(points);
    }

    public List<Vector3[]> GetDrawings()
    {
        return drawings.ToList();
    }
}