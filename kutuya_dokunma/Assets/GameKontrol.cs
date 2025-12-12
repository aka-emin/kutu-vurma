using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameKontrol : MonoBehaviourPunCallbacks
{
    [Header("OYUNCU SAÐLIK AYARLARI")]
    public Image Oyuncu_1_saglik_Bar;
    float Oyuncu_1_saglik = 100;
    public Image Oyuncu_2_saglik_Bar;
    float Oyuncu_2_saglik = 100;
    PhotonView pw;
    public GameObject zafer;
    public GameObject yenilgi;

    bool basladikmi;
    int limit;
    float beklemesuresi;
    int olusturmaSayisi;
    public GameObject[] noktalar;

    private bool oyunHazir = false;
    public bool OyunHazir => oyunHazir;

    private void Start()
    {
        pw = GetComponent<PhotonView>();
        basladikmi = false;
        limit = 4;
        beklemesuresi = 5f;
        Basla();
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
        }
    }
    [PunRPC]
    public void Basla()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            basladikmi = true;
            // Ýki oyuncu da hazýr olduðunda oyunu baþlat
            if (PhotonNetwork.PlayerList.Length == 2)
            {
                oyunHazir = true;
                pw.RPC("OyunDurumuGuncelle", RpcTarget.All, true);
            }
        }
        StartCoroutine(OlusturmayaBasla());
    }

    [PunRPC]
    public void OyunDurumuGuncelle(bool durum)
    {
        oyunHazir = durum;
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
        // Sadece görünüm güncellemesi (her iki durumda da ayný iþlemi yapýyor)
        if (Oyuncu_1_saglik_Bar != null)
            Oyuncu_1_saglik_Bar.fillAmount = Oyuncu_1_saglik / 100f;
        if (Oyuncu_2_saglik_Bar != null)
            Oyuncu_2_saglik_Bar.fillAmount = Oyuncu_2_saglik / 100f;

        if (Oyuncu_1_saglik <= 0)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1) // ben oyuncu 1'im
                ShowDefeatLocal();
            else // ben oyuncu 2'yim
                ShowVictoryLocal();

            Time.timeScale = 0f;
        }

        if (Oyuncu_2_saglik <= 0)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == 2) // ben oyuncu 2'yim
                ShowDefeatLocal();
            else
                ShowVictoryLocal();

            Time.timeScale = 0f;
        }
    }

    public void CikisYap()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
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
                }
                if (Oyuncu_1_saglik_Bar != null) Oyuncu_1_saglik_Bar.fillAmount = Oyuncu_1_saglik / 100;
                break;
            case 2:
                Oyuncu_2_saglik += 30;
                if (Oyuncu_2_saglik > 100)
                {
                    Oyuncu_2_saglik = 100;
                }
                if (Oyuncu_2_saglik_Bar != null) Oyuncu_2_saglik_Bar.fillAmount = Oyuncu_2_saglik / 100;
                break;
        }
    }

    // RPC ile kalan oyunculara zafer göster
    [PunRPC]
    public void ShowVictory()
    {
        if (zafer != null) zafer.SetActive(true);
        if (yenilgi != null) yenilgi.SetActive(false);
        Time.timeScale = 0f;
    }

    // RPC ile (teorik olarak) yenilgi göstermek isterseniz
    [PunRPC]
    public void ShowDefeat()
    {
        if (yenilgi != null) yenilgi.SetActive(true);
        if (zafer != null) zafer.SetActive(false);
        Time.timeScale = 0f;
    }

    // Yerel (sadece bu client üzerinde) zafer/yenilgi gösterimleri
    public void ShowVictoryLocal()
    {
        if (zafer != null) zafer.SetActive(true);
        if (yenilgi != null) yenilgi.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ShowDefeatLocal()
    {
        if (yenilgi != null) yenilgi.SetActive(true);
        if (zafer != null) zafer.SetActive(false);
        Time.timeScale = 0f;
    }
}
