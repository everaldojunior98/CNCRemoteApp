using UnityEngine;

public class DrawManager : MonoBehaviour
{
    public GameObject DrawingPrefab;
    public bool Optimize;

    private LineRenderer lineRenderer;
    private Vector3 lastMousePosition;
    private bool needOptimize;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var drawing = Instantiate(DrawingPrefab);
            lineRenderer = drawing.GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
        }

        if (Input.GetMouseButton(0))
            FreeDraw();

        if (Optimize && needOptimize && Input.GetMouseButtonUp(0))
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
        lineRenderer.Simplify(0.005f);
    }
}