using TMPro;
using UnityEngine;

public class anamenuyonet : MonoBehaviour
{
    public GameObject ilkpanel; 
    public GameObject ikincipanel;
    public TMP_InputField kullan�c�;
    public TextMeshProUGUI kullan�c�yaz;
    void Start()
    {
        if (!PlayerPrefs.HasKey("kullan�c�"))
        {
            ilkpanel.SetActive(true);
        }
        else
        {
            ilkpanel.SetActive(true);
            kullan�c�yaz.text = PlayerPrefs.GetString("kullan�c�");


        }
    }
    public void Kullan�c�Kaydet()
    {
        

        PlayerPrefs.SetString("kullan�c�",kullan�c�.text);
        kullan�c�yaz.text = PlayerPrefs.GetString("kullan�c�");
        ilkpanel .SetActive(false);
        ikincipanel .SetActive(true);
    }

    void Update()
    {
        
    }
}
