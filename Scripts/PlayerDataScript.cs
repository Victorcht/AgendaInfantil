using System.Collections.Generic;
using System.IO;
using UnityEngine;

//  PLAYER DATA SCRIPT
public class PlayerDataScript : MonoBehaviour
{
    // Objeto de DIA
    [System.Serializable]
    public class DayData
    {
        public string date;                                         // Fecha
        public List<task> tasks = new List<task>();                 // Lista de tareas
        public List<reminder> remainders = new List<reminder>();    // Lista de eventos
    }

    // Objeto de TAREA
    [System.Serializable]
    public class task
    {
        public string name; // Nombre
        public bool done;   // Hecho
    }

    // Objeto de RECORDATORIO O EVENTO
    [System.Serializable]
    public class reminder
    {
        public string name; // Nombre
        public string time; // Hora
    }

    // Objeto de player data que es una lista de datos de dias
    public List<DayData> playerData = new List<DayData>();

    // Ruta del archivo de guardado
    private string filePath;

    // Start
    void Start()
    {
        // La ruta será relativa según el dispostivo
        filePath = Application.persistentDataPath + "/datos.json";
        Debug.Log("Ruta del archivo: " + filePath);

        // Si no existe creamos el archivo
        if (!File.Exists(filePath))
        {
            Debug.Log("Archivo no existe. Creándolo...");

            SaveData(playerData);
        }
        // Si existe lo leemos
        else
        {
            Debug.Log("El archivo existe. Leyendo...");

            LoadData(playerData);
        }
    }

    // Función que guarda los datos en el archivo
    public void SaveData(List<DayData> data)
    {
        List<string> jsonDays = new List<string>();

        // Para cada dia crea una linea en el archivo de texto
        foreach (var day in playerData)
        {
            string json = JsonUtility.ToJson(day);
            jsonDays.Add(json);
        }

        // Unir todos los días usando un salto de línea
        string finalText = string.Join("\n", jsonDays);
        File.WriteAllText(filePath, finalText);
    }

    // Leer los datos de los dias
    public void LoadData(List<DayData> data)
    {
        // Leer todas las líneas (cada línea = un DayData)
        string[] lines = File.ReadAllLines(filePath);

        // Crea un objeto por cada dia leido
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            DayData day = JsonUtility.FromJson<DayData>(line);
            data.Add(day);
        }
    }
}
