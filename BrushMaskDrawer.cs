using UnityEngine;

public class BrushMaskDrawer3D : MonoBehaviour
{
    public Camera maskCamera;          // Kamera f�r die Maske
    public RenderTexture maskTexture;  // RenderTexture, auf die die Maske gezeichnet wird
    public Texture2D brushTexture;     // Runde, wei�e Brush-Textur
    public Material dirtMaterial;      // Material mit Shader f�r Dirt-Entfernung

    private Ray ray;
    private RaycastHit hit;

    void Start()
    {
        // Initialisiere RenderTexture
        if (maskTexture == null)
        {
            maskTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
            maskTexture.Create();
        }

        // Setze die RenderTexture in Shader und Kamera
        maskCamera.targetTexture = maskTexture;
        dirtMaterial.SetTexture("_MaskTex", maskTexture);
    }

    void Update()
    {
        // Pr�fe, ob die Zahnb�rste die Oberfl�che ber�hrt
        CheckBrushCollision();
    }

    void CheckBrushCollision()
    {
        // Sende einen Ray von der Zahnb�rste nach unten
        ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out hit))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();

            if (rend != null)
            {
                Vector2 uv = hit.textureCoord; // UV-Koordinaten auf der Oberfl�che
                EraseDirtAt(uv);              // Zeichne die B�rstenspur
            }
        }
    }

    void EraseDirtAt(Vector2 uv)
    {
        // Wandle UV-Koordinaten in Pixel-Koordinaten der RenderTexture
        int x = (int)(uv.x * maskTexture.width);
        int y = (int)(uv.y * maskTexture.height);

        // Zeichne die Brush Texture auf die RenderTexture
        RenderTexture.active = maskTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, maskTexture.width, maskTexture.height, 0);

        // Zeichne Brush Texture (Pinsel-Effekt)
        Graphics.DrawTexture(new Rect(x - 16, y - 16, 32, 32), brushTexture);

        GL.PopMatrix();
        RenderTexture.active = null;
    }
}
