using UnityEngine;

public static class GLUtilities
{
    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public static void DrawLine(Vector3 start, Vector3 end)
    {
        if (lineMaterial == null)
            CreateLineMaterial();

        lineMaterial.SetPass(0);
        GL.PushMatrix();

        GL.Begin(GL.LINES);

        GL.Vertex(start);
        GL.Vertex(end);

        GL.End();
        GL.PopMatrix();
    }

    public static void DrawLine(Vector3 start, Vector3 end,Color color)
    {
        if (lineMaterial == null)
            CreateLineMaterial();

        lineMaterial.SetPass(0);
        GL.PushMatrix();

        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(start);
        GL.Vertex(end);

        GL.End();
        GL.PopMatrix();
    }


}
