using UnityEngine;
using UnityEngine.UI;
using TMPro;

//  CONFIG SCRIPT
//  Script para la pestaña de configuración, por el momento permite recibir el valor del slider de color y aplicarlo al tema de la app.
public class ConfigScript : MonoBehaviour
{
    // Referencias
    public ThemeScript themeScript;
    public TextMeshProUGUI title;
    public Slider colorSlider;

    // Start
    void Start()
    {
        // Al iniciar pone el slider en el color que haya de fondo guardado
        colorSlider.value = themeScript.hue;
    }

    // Update
    void Update()
    {
        // Cambia el color de fondo según la posición del slider
        themeScript.hue = colorSlider.value;

        // Color de tema al titulo
        title.color = themeScript.themeColor;
    }
}
