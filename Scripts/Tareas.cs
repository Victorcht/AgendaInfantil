using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TAREAS
// Script que gestiona la pestaña de tareas, creando la lista de objetos/tareas y permitiendo navegar por los dias y crear tareas
public class Tareas : MonoBehaviour
{
    // Referencias
    public GameObject dayNum;
    public GameObject dayName;
    public GameObject UI;
    public TextMeshProUGUI añoMes;
    public GameObject tareaPrefab;
    public GameObject content;
    public GameObject camara;
    public GameObject arrowLeft;
    public GameObject arrowRight;
    public Button addButton;
    public TMP_InputField inputField;

    // Variables y referencias privadas
    private ThemeScript themeScript;
    private DateTime date;
    private int dateOffset = 0;
    private TextMeshProUGUI dayTMP;
    private bool loaded = false;
    private PlayerDataScript playerData;
    private List<TareaScript> taskList;
    private Image arrowLeftImg;
    private Image arrowRightImg;

    // Start
    void Start()
    {
        // Recibimos componentes
        themeScript = UI.GetComponent<ThemeScript>();
        dayTMP = dayName.GetComponent<TextMeshProUGUI>();
        playerData = camara.GetComponent<PlayerDataScript>();
        arrowLeftImg = arrowLeft.GetComponent<Image>();
        arrowRightImg = arrowRight.GetComponent<Image>();

        // Recibimos el dia de hoy
        date = DateTime.Now;
        
        // Asignamos el nombre del dia y el año arriba
        dayName.GetComponent<TextMeshProUGUI>().text = DateToSpanish();
        dayNum.GetComponent<TextMeshProUGUI>().text = date.Day.ToString();

        // Funciones de los botones de añadir y el input de crear nueva tarea
        addButton.onClick.AddListener(AddTask);
        inputField.onSubmit.AddListener(TaskCreated);
        arrowRight.GetComponent<Button>().onClick.AddListener(MoveDayRight);
        arrowLeft.GetComponent<Button>().onClick.AddListener(MoveDayLeft);
    }

    // Update
    void Update()
    {
        // Colores del tema
        dayTMP.color = themeScript.themeColor;
        arrowLeftImg.color = themeScript.themeColor;
        arrowRightImg.color = themeScript.themeColor;

        // Si no están cargadas las tareas
        if (!loaded)
        {
            // Recarga
            string actualDay = date.ToString("yyyy-MM-dd");

            dayName.GetComponent<TextMeshProUGUI>().text = DateToSpanish();
            dayNum.GetComponent<TextMeshProUGUI>().text = date.Day.ToString();
            añoMes.text = AñoMesSpanish();

            // Crea una lista con los objetos de tarea
            taskList = GetComponentsInChildren<TareaScript>().ToList();
            var taskListOrdered = taskList.Select(h => h.gameObject)
                                 .ToList();

            // Elimina todo
            foreach (var task in taskListOrdered)
            {
                Destroy(task);
            }

            // Crea todas las tareas almacenadas en la base de datos
            foreach (PlayerDataScript.DayData day in playerData.playerData)
            {
                if (day.date == actualDay)
                {
                    foreach (PlayerDataScript.task task in day.tasks)
                    {
                        GameObject obj = Instantiate(tareaPrefab, content.transform);
                        var objScipt = obj.GetComponent<TareaScript>();

                        objScipt.taskName = task.name;
                        objScipt.done = task.done;
                        objScipt.taskDate = day.date;
                    }
                }
            }

            // Reordena las tareas
            TaskReorder();
            loaded = true;
        }

        // Función que revisa que no haya cambios en el check de las tareas
        CheckChanges();
    }

    // Añade una tarea activando el input para poner el nombre
    void AddTask()
    {
        inputField.gameObject.SetActive(true);
        inputField.interactable = true;
        inputField.ActivateInputField(); // Activa el campo para escribir
    }

    // Si el input retorna el nombre crea la nueva tarea
    void TaskCreated(String nombre)
    {
        string actualDay = date.ToString("yyyy-MM-dd");
        bool sameTask = false;

        // Mira que no exista ya una tarea con ese nombre en ese dia
        foreach (var day in playerData.playerData)
        {
            if (day.date == actualDay)
            {
                foreach (var task in day.tasks)
                {
                    if (task.name == nombre)
                    {
                        sameTask = true;
                    }
                }
            }
        }

        // Si no existe la crea
        if (!sameTask && nombre != "")
        {
            GameObject obj = Instantiate(tareaPrefab, content.transform);
            var objScipt = obj.GetComponent<TareaScript>();

            objScipt.taskName = nombre;
            objScipt.taskDate = date.ToString("yyyy-MM-dd");

            SaveTaskData(nombre);

            TaskReorder();
            inputField.text = "";
            inputField.gameObject.SetActive(false);
        }
    }

    // Reordena las tareas para poner las que ya están hecha abajo
    void TaskReorder()
    {
        taskList = GetComponentsInChildren<TareaScript>().ToList();
        var taskListOrdered = taskList.OrderByDescending(h => !h.done) // true primero
                             .Select(h => h.transform)
                             .ToList();

        for (int i = 0; i < taskListOrdered.Count; i++)
        {
            taskListOrdered[i].SetSiblingIndex(i);
        }
    }

    // Mira que no haya ninguna tarea que haya cambiado de estado
    void CheckChanges()
    {
        // Revisar si hay cambios en las tareas
        foreach (var task in taskList)
        {
            // Si ha habido cambio
            if (task.doneChange)
            {
                // Ver de que dia es
                foreach (PlayerDataScript.DayData day in playerData.playerData)
                {
                    if (day.date == task.taskDate)
                    {
                        // Buscar la tarea que es
                        foreach (PlayerDataScript.task taskIn in day.tasks)
                        {
                            if (taskIn.name == task.taskName)
                            {
                                // Cambiar el objeto de tarea
                                task.done = !task.done;
                                task.doneChange = !task.doneChange;

                                // Guardar el valor de done en los datos
                                taskIn.done = task.done;

                                playerData.SaveData(playerData.playerData);

                                // Reordena las tareas para poner las hechas abajo
                                TaskReorder();
                            }
                        }
                    }
                }
            }
        }
    }

    // Guarda los datos de la nueva tarea
    void SaveTaskData(String taskName)
    {
        string actualDay = date.ToString("yyyy-MM-dd");
        bool dayExist = false;

        // Mira si el dia existe en la base de datos y crea la tarea
        foreach (PlayerDataScript.DayData day in playerData.playerData)
        {
            if (day.date == actualDay)
            {
                day.tasks.Add(new PlayerDataScript.task { name = taskName, done = false });
                dayExist = true;
            }
        }

        // Si no existe el dia lo crea también
        if (dayExist == false)
        {
            PlayerDataScript.DayData newDay = new PlayerDataScript.DayData();
            newDay.date = actualDay;
            newDay.tasks.Add(new PlayerDataScript.task { name = taskName, done = false });
            playerData.playerData.Add(newDay);
        }

        playerData.SaveData(playerData.playerData);
    }

    // Mueve el dia activo a uno más con la flecha ->
    void MoveDayRight()
    {
        dateOffset += 1;

        date = DateTime.Now.AddDays(dateOffset);

        loaded = false;
    }

    // Mueve el dia activo a uno menos con la flecha <-
    void MoveDayLeft()
    {
        dateOffset -= 1;

        date = DateTime.Now.AddDays(dateOffset);

        loaded = false;
    }

    // Pone el nombre del dia en español // La función está obsoleta tengo que cambiarla por esta "new CultureInfo("es-ES")"
    string DateToSpanish()
    {
        string dayNameSpanish = "";

        switch (date.DayOfWeek)
        {
            case DayOfWeek.Monday: dayNameSpanish = "Lunes"; break;
            case DayOfWeek.Tuesday: dayNameSpanish = "Martes"; break;
            case DayOfWeek.Wednesday: dayNameSpanish = "Miércoles"; break;
            case DayOfWeek.Thursday: dayNameSpanish = "Jueves"; break;
            case DayOfWeek.Friday: dayNameSpanish = "Viernes"; break;
            case DayOfWeek.Saturday: dayNameSpanish = "Sábado"; break;
            case DayOfWeek.Sunday: dayNameSpanish = "Domingo"; break;
        }

        return dayNameSpanish;
    }

    // Pone el nombre del mes y el año en español
    string AñoMesSpanish()
    {
        string spanishMonth = "";

        spanishMonth = date.ToString("MMMM yyyy", new CultureInfo("es-ES"));
        spanishMonth = char.ToUpper(spanishMonth[0]) + spanishMonth.Substring(1);

        return spanishMonth;
    }
}
