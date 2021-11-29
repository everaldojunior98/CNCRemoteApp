using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawManager : MonoBehaviour
{
    [SerializeField]
    private GameObject drawingPrefab;

    private LineRenderer lineRenderer;
    private Vector3 lastMousePosition;
    private bool needOptimize;

    private List<Vector3[]> drawings;
    private List<GameObject> drawingObjects;

    private void Start()
    {
        drawings = new List<Vector3[]>();
        drawingObjects = new List<GameObject>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            CreateLine();
        if (Input.GetMouseButton(0))
            FreeDraw(Input.mousePosition);
        if (needOptimize && Input.GetMouseButtonUp(0))
            OptimizeLine();
#endif

#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                CreateLine();
            else if (touch.phase == TouchPhase.Moved)
                FreeDraw(touch.position);
            else if (touch.phase == TouchPhase.Ended)
                OptimizeLine();
        }
#endif
    }

    private void CreateLine()
    {
        if (IsPointerOverUIObject())
            return;

        var drawing = Instantiate(drawingPrefab);
        lineRenderer = drawing.GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        drawingObjects.Add(drawing);
    }

    private void FreeDraw(Vector2 position)
    {
        if (lineRenderer == null || IsPointerOverUIObject())
            return;

        needOptimize = true;

        var mousePos = new Vector3(position.x, position.y, 10f);

        if (Vector3.Distance(lastMousePosition, mousePos) > 0.1f)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, Camera.main.ScreenToWorldPoint(mousePos));
            lastMousePosition = mousePos;
        }
    }

    private void OptimizeLine()
    {
        if (IsPointerOverUIObject())
            return;

        needOptimize = false;
        lineRenderer.Simplify(0.001f);

        var points = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(points);
        drawings.Add(points);

        lineRenderer = null;
    }

    private bool IsPointerOverUIObject()
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public List<Vector3[]> GetDrawings()
    {
        return drawings.ToList();
    }

    public void Clear()
    {
        drawings.Clear();
        foreach (var drawingObject in drawingObjects)
            Destroy(drawingObject);
        drawingObjects.Clear();
    }
}