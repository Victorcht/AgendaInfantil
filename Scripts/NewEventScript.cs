using UnityEngine;
using UnityEngine.UI;

// NEW EVENT SCRIPT
// Gestiona el pop up que aparece cuando creas un nuevo evento
public class NewEventScript : MonoBehaviour
{
    // Array de inputs
    public InputScript[] inputs;
    public InputScript inputNombre;
    public InputScript inputDia;
    public InputScript inputHora;
    public InputScript inputMinutos;

    // Referencias
    public Button button;

    // Variables
    public int maxDay;
    public bool confirmed = false;
    public string nombre;
    public string dia;
    public string hora;
    public string minutos;

    // Start
    private void Start()
    {
        // Asigna una función al botón de aceptar
        button.onClick.AddListener(SubmitEvent);

        // Crea un array con los inputs de texto
        inputs = new InputScript[4];
        inputs[0] = inputNombre;
        inputs[1] = inputDia;
        inputs[2] = inputHora;
        inputs[3] = inputMinutos;
    }

    // Cuando se activa el pop up
    private void OnEnable()
    {
        confirmed = false; // No esta confirmado
    }

    // Update
    private void Update()
    {
        // Le pone un máximo al input de dia según los dias del mes para que no haya errores
        if (maxDay != 0)
        {
            foreach (InputScript input in inputs)
            {
                if (input.tipe == InputScript.InputTipe.day)
                {
                    input.maxDay = maxDay;
                }
            }
        }
    }

    // Botón de aceptar
    void SubmitEvent()
    {
        bool allCorrect = true;

        // Revisa que no haya campos vacios
        foreach (InputScript input in inputs)
        {
            if (input.input.text == "")
            {
                allCorrect = false;
            }
        }

        // Si todo está correcto avisa y recibe los valores de los inputs para que sean copiados por el script de EVENTOS
        if (allCorrect)
        {
            nombre = inputNombre.input.text;
            dia = inputDia.input.text;
            hora = inputHora.input.text;
            minutos = inputMinutos.input.text;

            foreach (InputScript input in inputs)
            {
                input.input.text = "";
            }

            maxDay = 0;
            confirmed = true;
        }
    }
}
