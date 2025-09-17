using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameKontrol : MonoBehaviour
{
    [Header("OYUNCU SAÐLIK AYARLARI")]
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
        Basla();
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator OlusturmayaBasla()
    {
        olusturmaSayisi = 0;

        while (true && basladikmi)
        {
            if (limit == olusturmaSayisi)
                basladikmi = false;

            yield return new WaitForSeconds(15f);
            int olusandeger = Random.Range(0, 6);
            PhotonNetwork.Instantiate("Odul", noktalar[olusandeger].transform.position, noktalar[olusandeger].transform.rotation, 0, null);
            olusturmaSayisi++;
        }   }
    [PunRPC]
    public void Basla()
    {
        if (PhotonNetwork.IsMasterClient)
            basladikmi = true;
        StartCoroutine(OlusturmayaBasla());
    }
    [PunRPC]
    public void Darbe_vur(int oyuncuID, float darbegucu)
    {
        if (oyuncuID == 1)
            Oyuncu_1_saglik -= darbegucu;
        else if (oyuncuID == 2)
            Oyuncu_2_saglik -= darbegucu;

        // Herkes can deðerini günceller
        UpdateUI();
    }

    void UpdateUI()
    {
        // Sadece kendi can barýný güncelle
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
