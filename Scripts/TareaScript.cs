using UnityEngine;
using TMPro;
using UnityEngine.UI;

// TAREA SCRIPT
// Script que se le asigna a los objetos de tarea, gestion si esta hecha, su nombre y cosas de los objetos de ineterfaz que lleva dentro
public class TareaScript : MonoBehaviour
{
    // Variables
    public string taskName;
    public string taskDate;
    public bool done;

    // Referencias
    public GameObject nameText;
    public GameObject checkImg;
    public Image back;
    public Sprite checkOn;
    public Sprite checkOff;
    public Button checkButton;
    public Sprite whiteBack;
    public Sprite blackBack;
    public Color whiteTextColor;
    public Color blackTextColor;

    // Variables privadas
    private TextMeshProUGUI nameTextString;
    private Image checkRend;
    private Image spriteRenderer;
    public bool doneChange = false;

    // Start
    void Start()
    {
        // Recibimos componentes
        nameTextString = nameText.GetComponent<TextMeshProUGUI>();
        checkRend = checkImg.GetComponent<Image>();
        spriteRenderer = GetComponent<Image>();

        // Creamos un click event al darle a la caja del check
        checkButton.onClick.AddListener(ChangeDone);
    }

    // Update
    void Update()
    {
        // Asignamos su nombre
        nameTextString.text = taskName;

        // Le cambiamos los colores si esta en hecho o no
        if (done)
        {
            checkRend.sprite = checkOn;
            checkRend.color = blackTextColor;
            spriteRenderer.sprite = blackBack;
            nameTextString.color = blackTextColor;
            back.color = new Color(blackTextColor.r, blackTextColor.g, blackTextColor.b, back.color.a);
        }
        else
        {
            checkRend.sprite = checkOff;
            checkRend.color = whiteTextColor;
            spriteRenderer.sprite = whiteBack;
            nameTextString.color = whiteTextColor;
            back.color = new Color(whiteTextColor.r, whiteTextColor.g, whiteTextColor.b, back.color.a);
        }
    }

    // Si clicamos ponemos doneChange para que sepamos que lo hemos cambiado y a traves de otro script podamos realizar el cambio en la base de datos
    void ChangeDone()
    {
        doneChange = true;
    }
}
