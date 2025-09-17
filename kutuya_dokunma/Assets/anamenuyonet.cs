using TMPro;
using UnityEngine;

public class anamenuyonet : MonoBehaviour
{
    public GameObject ilkpanel; 
    public GameObject ikincipanel;
    public TMP_InputField kullanýcý;
    public TextMeshProUGUI kullanýcýyaz;
    void Start()
    {
        if (!PlayerPrefs.HasKey("kullanýcý"))
        {
            ilkpanel.SetActive(true);
        }
        else
        {
            ilkpanel.SetActive(true);
            kullanýcýyaz.text = PlayerPrefs.GetString("kullanýcý");


        }
    }
    public void KullanýcýKaydet()
    {
        

        PlayerPrefs.SetString("kullanýcý",kullanýcý.text);
        kullanýcýyaz.text = PlayerPrefs.GetString("kullanýcý");
        ilkpanel .SetActive(false);
        ikincipanel .SetActive(true);
    }

    void Update()
    {
        
    }
}
