using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Haptics;

public class bot : MonoBehaviour
{
    public GameObject top;
    public GameObject TopCikisnoktasi;
    public ParticleSystem TopAtisEfekt;
    public AudioSource TopAtmaSesi;
    public static bool sayim= false;
    float AtisYonu = 1f;
    public bool atesetme = true;

    // Orijinal oyuncular için PowerBar (botta olmayabilir)
    Image PowerBar;
    Coroutine powerDongu;
    bool sonageldimi = false;

    PhotonView pw;
    int activeTopViewId = 0;

    void Start()
    {
        sayim = true;
        pw = GetComponent<PhotonView>();

        // SADECE KENDİ PLAYER → POWERBAR bulsun
        if (pw.IsMine)
        {
            var bar = GameObject.FindWithTag("PowerBar");
            if (bar != null)
                PowerBar = bar.GetComponent<Image>();

            // Pozisyon ayarla
            
                transform.position = GameObject.FindWithTag("oyuncuNoktasi2").transform.position;
                transform.rotation = GameObject.FindWithTag("oyuncuNoktasi2").transform.rotation;
                AtisYonu = -2f;
            

            // BOT otomatik ateş
            StartCoroutine(BotAtesDongusu());
        }

        InvokeRepeating(nameof(Oyunbasladimi), 0f, 0.5f);
    }

    // BOT OTOMATİK ATIŞ
    IEnumerator BotAtesDongusu()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));

            if (pw.IsMine && atesetme)
                Fire();
        }
    }

    // OYUN 2 KİŞİ OLUNCA POWEBAR BAŞLASIN
    void Oyunbasladimi()
    {
        if (!pw.IsMine) return;

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            if (powerDongu == null && PowerBar != null)
                powerDongu = StartCoroutine(PowerBarCalistir());

            CancelInvoke(nameof(Oyunbasladimi));
        }
    }

    IEnumerator PowerBarCalistir()
    {
        if (PowerBar == null) yield break;

        PowerBar.fillAmount = 0f;
        sonageldimi = false;
        float speed = 0.8f;

        while (true)
        {
            float delta = Time.deltaTime * speed;

            if (!sonageldimi)
            {
                PowerBar.fillAmount += delta;
                if (PowerBar.fillAmount >= 1f) { PowerBar.fillAmount = 1f; sonageldimi = true; }
            }
            else
            {
                PowerBar.fillAmount -= delta;
                if (PowerBar.fillAmount <= 0f) { PowerBar.fillAmount = 0f; sonageldimi = false; }
            }

            yield return null;
        }
    }

    void Update()
    {
        if (!pw.IsMine) return;
        if (PhotonNetwork.PlayerList.Length < 2) return;

        bool fireInput = false;

        if (Input.GetKeyDown(KeyCode.Space) && atesetme)
            fireInput = true;

        if (fireInput)
            Fire();
    }

    // ATEŞ ETME
    void Fire()
    {
        if (!atesetme) return;

        if (activeTopViewId != 0)
        {
            Debug.Log("[BOT] Zaten aktif top var: " + activeTopViewId);
            return;
        }

        if (TopCikisnoktasi == null)
        {
            Debug.LogError("[BOT] TopCikisnoktasi null!");
            return;
        }

        atesetme = false;

        // TOP OLUŞTUR
        GameObject topObj = PhotonNetwork.Instantiate(
            "Top",
            TopCikisnoktasi.transform.position,
            TopCikisnoktasi.transform.rotation
        );

        PhotonView topPV = topObj.GetComponent<PhotonView>();
        if (topPV != null)
        {
            activeTopViewId = topPV.ViewID;
            topPV.RPC("TagAktar", RpcTarget.All, gameObject.tag, pw.ViewID);
        }

        if (TopAtmaSesi != null)
            TopAtmaSesi.Play();

        // GÜÇ HESABI
        float guc;

        if (PowerBar != null)       // OYUNCU → PowerBar
            guc = PowerBar.fillAmount;
        else                        // BOT → random
            guc = Random.Range(0.3f, 1f);

        // FİZİK
        Rigidbody2D rg = topObj.GetComponent<Rigidbody2D>();
        if (rg != null)
            rg.AddForce(new Vector2(AtisYonu, 0f) * guc * 10f, ForceMode2D.Impulse);

        // POWERBAR durdur
        if (powerDongu != null)
        {
            StopCoroutine(powerDongu);
            powerDongu = null;
        }
    }

    // Top çarpınca bunu çağırıyor
    [PunRPC]
    public void ClearActiveTop(int topViewId)
    {
        if (!pw.IsMine) return;

        if (activeTopViewId == topViewId)
        {
            activeTopViewId = 0;
            atesetme = true;

            if (PowerBar != null && powerDongu == null)
                powerDongu = StartCoroutine(PowerBarCalistir());
        }
    }
}
