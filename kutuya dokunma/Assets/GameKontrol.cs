using UnityEngine;
using UnityEngine.UI;

public class GameKontrol : MonoBehaviour
{

    [Header("TOP AYARLARI VE ÝÞLEMLERÝ")]
    public GameObject TopYokOlmaEfekt;
    public AudioSource YokOlmaSesi;

    [Header("ORTADAKÝ KUTULARIN AYARLARI VE ÝÞLEMLERÝ")]
    public GameObject KutuYokOlmaEfekt;
    public AudioSource KutuYokOlmaSesi;

    public Image saglikbar1;
    public Image saglikbar2;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void darbeAl(int key,float darbeguc)
    {
        switch(key)
        {
            case 0:
                saglikbar1.fillAmount = saglikbar1.fillAmount - darbeguc;
                break;

                case 1:
                saglikbar2.fillAmount = saglikbar2.fillAmount - darbeguc;
                break;
        }
    }
    public void CanAl(int key, float darbeguc)
    {
        switch (key)
        {
            case 0:
                saglikbar1.fillAmount = saglikbar1.fillAmount + darbeguc;
                break;
            case 1:
                saglikbar2.fillAmount = saglikbar2.fillAmount + darbeguc;
                break;
        }
    }
    public void Ses_ve_Efekt_Olustur(int kriter, GameObject objetransformu)
    {

        switch (kriter)
        {

            case 1:
                Instantiate(TopYokOlmaEfekt, objetransformu.gameObject.transform.position, objetransformu.gameObject.transform.rotation);
                YokOlmaSesi.Play();
                break;
            case 2:
                Instantiate(KutuYokOlmaEfekt, objetransformu.gameObject.transform.position, objetransformu.gameObject.transform.rotation);
                KutuYokOlmaSesi.Play();
                break;

        }

    }


}
