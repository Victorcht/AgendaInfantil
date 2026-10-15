using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//  INPUT SCRIPT
//  Este script se aplica a los inputs de texto para saber de qué tipo son
//  y limitar lo que se entra en ellos según los datos que queremos recibir
public class InputScript : MonoBehaviour
{
    // Tipos de input para asignarles diferentes funcionaldiades
    public enum InputTipe{
        text,
        day,
        dateHour,
        dateMins
    }

    // Tipo
    public InputTipe tipe;

    // Referencias
    public TMP_InputField input;

    // Variables
    public int maxDay;

    // Start
    void Start()
    {
        // Carga de componentes
        input = GetComponent<TMP_InputField>();

        // Al acabar de editar el input
        input.onEndEdit.AddListener(OnEndEdit);
    }

    // Función de cuando acaba de editar
    void OnEndEdit(string texto)
    {
        // Sergún el tipo mira si cumple las condiciones
        if (tipe == InputTipe.day)
        {
            if(StringToInt(texto) > maxDay)
                input.text = "";
            else
                input.text = StringToInt(texto).ToString("D2");
        }
        else if (tipe == InputTipe.dateHour)
        {
            if (StringToInt(texto) > 24)
                input.text = "";
            else
                input.text = StringToInt(texto).ToString("D2");
        }
        else if (tipe == InputTipe.dateMins)
        {
            if (StringToInt(texto) > 60)
                input.text = "";
            else
                input.text = StringToInt(texto).ToString("D2");
        }
    }

    // Función que convierte el String en Int
    int StringToInt(string texto)
    {
        if (int.TryParse(texto, out int valor))
        {
            return valor;
        }
        else
        {
            return 0;
        }
    }
}
