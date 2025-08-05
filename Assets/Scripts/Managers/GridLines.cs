using UnityEngine;

public class GridLines : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float lineWidth = 0.05f;
    public Color lineColor = Color.black;

    private GameObject linesParent;

    public void GenerateGridLines()
    {
        // Clear old lines if any
        if (linesParent != null)
        {
            Destroy(linesParent);
        }

        linesParent = new GameObject("GridLinesParent");
        linesParent.transform.SetParent(transform, false);

        float offset = 0.5f; // shift lines to tile edges

        // Vertical lines
        for (int x = 0; x <= width; x++)
        {
            CreateLine(
                new Vector3(x - offset, 0.01f, 0 - offset),
                new Vector3(x - offset, 0.01f, height - offset)
            );
        }

        // Horizontal lines
        for (int z = 0; z <= height; z++)
        {
            CreateLine(
                new Vector3(0 - offset, 0.01f, z - offset),
                new Vector3(width - offset, 0.01f, z - offset)
            );
        }

    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(linesParent.transform, false);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = lineColor;

        lr.useWorldSpace = true;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.loop = false;
    }

    public void SetGridVisible(bool visible)
    {
        if (linesParent != null)
            linesParent.SetActive(visible);
    }
}
