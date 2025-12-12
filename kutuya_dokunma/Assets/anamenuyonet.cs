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
        // Daha önce kayýt var mý?
        if (PlayerPrefs.HasKey("kullanýcý"))
        {
            // Kayýt varsa direkt ikinci panel
            ilkpanel.SetActive(false);
            ikincipanel.SetActive(true);

            kullanýcýyaz.text = PlayerPrefs.GetString("kullanýcý");
        }
        else
        {
            // Kayýt yoksa ilk panel
            ilkpanel.SetActive(true);
            ikincipanel.SetActive(false);
        }
    }
    public void ac(GameObject acýlan)
    {
        acýlan.SetActive(true);
    }
    public void kapa(GameObject kapanan)
    {
        kapanan.SetActive(false);
    }
    public void KullanýcýKaydet()
    {
        PlayerPrefs.SetString("kullanýcý", kullanýcý.text);
        kullanýcýyaz.text = kullanýcý.text;
        kapa(ilkpanel);
        ac(ikincipanel);
    }
}
