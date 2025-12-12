using Photon.Pun;
using Photon.Realtime;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sunucuyonet : MonoBehaviourPunCallbacks
{
    public GameObject suucuhatapaneli;
    bool baglantiBasarili;
    public Button playbutton;
    public static sunucuyonet instance;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Kopyayı yok et
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SunucuyaBaglan();
    }
    public void SunucuyaBaglan()
    {
        baglantiBasarili = false;
        suucuhatapaneli.SetActive(false);

        PhotonNetwork.ConnectUsingSettings();

        // 5 saniyelik zaman aşımı kontrolü
        Invoke("BaglantiTimeoutKontrol", 10f);
    }
    private void BaglantiTimeoutKontrol()
    {
        if (!baglantiBasarili)
        {
            suucuhatapaneli.SetActive(true);
            Debug.Log("Bağlantı zaman aşımına uğradı!");
        }
    }

    public override void OnConnectedToMaster()
    {
        baglantiBasarili = true; // YENİ EKLENDİ
        CancelInvoke("BaglantiTimeoutKontrol");

        Debug.Log("Sunucuya bağlandı");
        PhotonNetwork.JoinLobby();
    }
    public void YenidenDene()
    {
        SunucuyaBaglan();
    }
    public void Odakur()
    {
        PhotonNetwork.LoadLevel(1);
        string odadi = Random.Range(0, 2345).ToString();
        PhotonNetwork.JoinOrCreateRoom(odadi, new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true }, TypedLobby.Default);
    }
    public void botodasi()
    {
        PhotonNetwork.LoadLevel(2);

        string odaId = Random.Range(1000, 9999).ToString();

        PhotonNetwork.JoinOrCreateRoom(
            odaId,
            new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = false },
            TypedLobby.Default
        );
    }

    public void RasgeleOdayakatıl()
    {
        PhotonNetwork.LoadLevel(1);
        PhotonNetwork.JoinRandomRoom();
    }
   
    public override void OnJoinedLobby()
    {
        playbutton.interactable = true;
        Debug.Log("lobiye baglandı");
        GameObject isik = GameObject.FindWithTag("sunucuisigi");
        if (isik != null)
        {
            var ri = isik.GetComponent<RawImage>();
            if (ri != null) ri.color = Color.green;
        }
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Odaya katıldı: " + PhotonNetwork.CurrentRoom.Name);

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            // ━━━━━━━━ ONLINE ODA ━━━━━━━━
            OnlineOdaOyuncuOlustur();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            // ━━━━━━━━ BOTLU ODA ━━━━━━━━
            BotluOdaOyuncuOlustur();
        }
    }
    void OnlineOdaOyuncuOlustur()
    {
        GameObject oyuncu = PhotonNetwork.Instantiate("Oyuncu1",
            Vector3.zero, Quaternion.identity);

        oyuncu.GetComponent<PhotonView>().Owner.NickName =
            PlayerPrefs.GetString("kullanıcı");

        // oyuncu1 / oyuncu2 tag ayarı
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            oyuncu.tag = "oyuncu1";
        }
        else
        {
            oyuncu.tag = "oyuncu2";

            // oyun başlat
            var kontrol = GameObject.FindGameObjectWithTag("GameKontrol");
            if (kontrol != null)
                kontrol.GetComponent<PhotonView>()
                .RPC("Basla", RpcTarget.All);
        }
    }

    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // 🟥 BOTLU ODA (SAHNE 2)
    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
   void BotluOdaOyuncuOlustur()
{
    // --- GERÇEK OYUNCUYU PHOTON İLE SPAWN ET ---
    GameObject oyuncu = PhotonNetwork.Instantiate(
        "Oyuncu1",
        Vector3.zero,
        Quaternion.identity
    );
    oyuncu.tag = "oyuncu1";

    // --- BOTU PHOTON İLE SPAWN ET ---
    GameObject bot = PhotonNetwork.Instantiate(
        "botum",                     // <-- BOT PREFAB ADI
        new Vector3(2, 0, 0),        // Bot spawn konumu
        Quaternion.identity
    );

    bot.tag = "oyuncu2";

    Debug.Log("BOTLU oda: Gerçek oyuncu + bot Photon ile oluşturuldu");
}

    public override void OnLeftRoom()
    {
        Debug.Log("Odadan çıkıldı");

        // Lokal client (odadan çıkan) için yenilgi paneli göster
        GameObject kontrol = GameObject.FindWithTag("GameKontrol");
        if (kontrol != null)
        {
            var gc = kontrol.GetComponent<GameKontrol>();
            if (gc != null)
            {
                gc.ShowDefeatLocal();
            }
        }
    }
    public override void OnLeftLobby()
    {
        Debug.Log("Lobiden çıkıldı");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Oyuncu çıktı: {otherPlayer.NickName}");

        // Kalan client'larda zafer panelini göster
        GameObject kontrol = GameObject.FindWithTag("GameKontrol");
        if (kontrol != null)
        {
            var pv = kontrol.GetComponent<PhotonView>();
            if (pv != null)
            {
                // RPC ile kalan herkeste zafer gösterilir (çıkan client oda dışında olacağı için etkilenmez)
                pv.RPC("ShowVictory", RpcTarget.All);
            }
            else
            {
                // PhotonView yoksa lokal metodla göster
                var gc = kontrol.GetComponent<GameKontrol>();
                if (gc != null) gc.ShowVictoryLocal();
            }
        }

        // Bilgileri güncelleme çağrısını başlat (mevcut davranış)
        InvokeRepeating("BigileriKontrolEt", 0, 1f);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // herhangi bir oyuncu girdiğinde tetiklenen fonksiyondur.
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Herhangi bir odaya girilemedi");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Rastgele bir odaya girilemedi");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Oda oluşturulamadı");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Sunucuyla bağlantı koptu! Sebep: " + cause);

        baglantiBasarili = false;     // Bağlantı başarısız işaretle
        suucuhatapaneli.SetActive(true);  // Sunucu hata panelini aç

        // Timeout kontrolü varsa onu da iptal et
        CancelInvoke("BaglantiTimeoutKontrol");

      
    }

    void BigileriKontrolEt()
    {
        GameObject panelim = GameObject.FindWithTag("oyuncubekleniyor");
        if (panelim == null) return;

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            panelim.SetActive(false);
            GameObject.FindWithTag("Oyuncu1_isim").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[0].NickName;
            GameObject.FindWithTag("Oyuncu2_isim").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[1].NickName;
            CancelInvoke("BigileriKontrolEt");
        }
        else
        {
            GameObject.FindWithTag("Oyuncu1_isim").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[0].NickName;
            GameObject.FindWithTag("Oyuncu2_isim").GetComponent<TextMeshProUGUI>().text = "....";
            panelim.SetActive(true);
        }
    }
}