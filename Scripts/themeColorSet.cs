using UnityEngine;
using UnityEngine.UI;

//  THEME COLOR SET
//  Es un script que te permite asignarselo a una imagen que quieras que reciba el color del tema por si solo facilmente
public class themeColorSet : MonoBehaviour
{

    public GameObject UI;

    private ThemeScript themeScript;
    private Image img;

    // Start
    void Start()
    {
        themeScript = UI.GetComponent<ThemeScript>();
        img = GetComponent<Image>();
    }

    // Update
    void Update()
    {
        img.color = themeScript.themeColor;
    }
}
