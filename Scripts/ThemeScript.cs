using UnityEngine;
using static UnityEngine.UI.Image;

// THEME SCRIPT
// Script que gestiona el color del tema y se lo aplica al shader del fondo
public class ThemeScript : MonoBehaviour
{
    [Header("Parámetros UI")]
    [Range(0f, 1f)] // Slider de 0 a 1 en el Inspector
    public float hue = 0f;

    // Variables para debugear en el inspector de Unity
    [Header("Debug")]
    public Color baseColor;
    public Color themeColor;
    public Material mat;
    private float hueDeg;
    private float hueOffset;
    private float lastHue = 450;

    // Start
    void Start()
    {
        // Almacena o recibe el color del tema que haya escogido el usuario anteriormente
        if(PlayerPrefs.GetFloat("ThemeHue", 450f) != 450f) // si no existe, devuelve 0.0
        {
            hue = PlayerPrefs.GetFloat("ThemeHue", 0.0f);
        }
        else
        {
            Color.RGBToHSV(baseColor, out float h, out float s, out float v);
            hue = 0.8f;
            hueOffset = -(h * 360);
        }
    }

    // Update
    void Update()
    {
        // Convierte el color en HSV para cambiar el valor de hue
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        themeColor = Color.HSVToRGB(hue, s, v);

        hueDeg = (hue * 360) + hueOffset;
        mat.SetFloat("_Color", hueDeg);

        if (lastHue != hue)
        {
            // Guardar un float
            PlayerPrefs.SetFloat("ThemeHue", hue);
            lastHue = hue;
        }
    }
}
