using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameKontrol : MonoBehaviour
{
    [Header("OYUNCU SA�LIK AYARLARI")]
    public Image Oyuncu_1_saglik_Bar;
    float Oyuncu_1_saglik = 100;
    public Image Oyuncu_2_saglik_Bar;
    float Oyuncu_2_saglik = 100;
    PhotonView pw;

    bool basladikmi;
    int limit;
    float beklemesuresi;
    int olusturmaSayisi;
    public GameObject[] noktalar;

    private void Start()
    {
        pw = GetComponent<PhotonView>();
        basladikmi = false;
        limit = 4;
        beklemesuresi = 5f;
    }

    // Update is called once per frame
    void Update()
    {

    }
    [PunRPC]
    public void Darbe_vur(int oyuncuID, float darbegucu)
    {
        if (oyuncuID == 1)
            Oyuncu_1_saglik -= darbegucu;
        else if (oyuncuID == 2)
            Oyuncu_2_saglik -= darbegucu;

        // Herkes can de�erini g�nceller
        UpdateUI();
    }

    void UpdateUI()
    {
        // Sadece kendi can bar�n� g�ncelle
        if (PhotonNetwork.IsMasterClient)
        {
            Oyuncu_1_saglik_Bar.fillAmount = Oyuncu_1_saglik / 100f;
            Oyuncu_2_saglik_Bar.fillAmount = Oyuncu_2_saglik / 100f;
        }
        else
        {
            Oyuncu_1_saglik_Bar.fillAmount = Oyuncu_1_saglik / 100f;
            Oyuncu_2_saglik_Bar.fillAmount = Oyuncu_2_saglik / 100f;
        }
    }


    [PunRPC]
    public void SaglikDoldur(int hangioyuncu)
    {
        switch (hangioyuncu)
        {
           case 1:
                Oyuncu_1_saglik += 30;

                if (Oyuncu_1_saglik > 100)
                {
                    Oyuncu_1_saglik = 100;
                    Oyuncu_1_saglik_Bar.fillAmount = Oyuncu_1_saglik / 100;
                }
                else
                {
                    Oyuncu_1_saglik_Bar.fillAmount = Oyuncu_1_saglik / 100;
                }
                break;
            case 2:
                Oyuncu_2_saglik += 30;
                if (Oyuncu_2_saglik > 100)
                {
                    Oyuncu_2_saglik = 100;
                    Oyuncu_2_saglik_Bar.fillAmount = Oyuncu_2_saglik / 100;
                }
                else
                {
                    Oyuncu_2_saglik_Bar.fillAmount = Oyuncu_2_saglik / 100;
                }
                break;
        }

    }
   


}
