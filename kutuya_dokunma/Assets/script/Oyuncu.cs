using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Oyuncu : MonoBehaviour
{
    public GameObject top;
    public GameObject TopCikisnoktasi;
    public ParticleSystem TopAtisEfekt;
    public AudioSource TopAtmaSesi;
    float AtisYonu;
    public bool atesetme = true;

    [Header("GÜÇ BARI AYARLARI")]
    Image PowerBar;
    float powerSayi;
    bool sonageldimi = false;
    Coroutine powerDongu;

    PhotonView pw;

    // Yeni: aktif topu takip etmek için view id
    int activeTopViewId = 0;

    void Start()
    {
        pw = GetComponent<PhotonView>();

        if (pw.IsMine)
        {
            PowerBar = GameObject.FindWithTag("PowerBar").GetComponent<Image>();
            if (PhotonNetwork.IsMasterClient)
            {
                transform.position = GameObject.FindWithTag("oyuncuNoktasi1").transform.position;
                transform.rotation = GameObject.FindWithTag("oyuncuNoktasi1").transform.rotation;
                AtisYonu = 2f;
            }
            else
            {
                transform.position = GameObject.FindWithTag("oyuncuNoktasi2").transform.position;
                transform.rotation = GameObject.FindWithTag("oyuncuNoktasi2").transform.rotation;
                AtisYonu = -2f;
            }
        }
        InvokeRepeating("Oyunbasladimi", 0, .5f);
    }

    [PunRPC]
    public void PowerOynasin()
    {
        if (pw != null && pw.IsMine)
        {
            if (PowerBar != null)
            {
                // power bar yeniden başlat
                if (powerDongu == null)
                    powerDongu = StartCoroutine(PowerBarCalistir());
            }
            atesetme = true;
        }
    }

    [PunRPC]
    public void SetAtesetmeTrue()
    {
        if (pw != null && pw.IsMine) atesetme = true;
    }

    // Yeni: Top tarafından çağrılacak, topViewId ile eşleşme kontrolü yapar
    [PunRPC]
    public void ClearActiveTop(int topViewId)
    {
        if (pw == null || !pw.IsMine) return;
        // yalnızca eşleşen topu temizle
        if (activeTopViewId == topViewId)
        {
            activeTopViewId = 0;
            atesetme = true;
            Debug.Log($"[Oyuncu] activeTop temizlendi: {topViewId}");

            // Önemli: powerDongu durdurulmuş olabilir — tekrar başlat
            if (PowerBar != null && powerDongu == null)
            {
                powerDongu = StartCoroutine(PowerBarCalistir());
                Debug.Log("[Oyuncu] PowerBarCalistir coroutine yeniden başlatıldı.");
            }
        }
    }

    public void Oyunbasladimi()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            if (pw.IsMine)
            {
                if (powerDongu == null)
                    powerDongu = StartCoroutine(PowerBarCalistir());
                CancelInvoke("Oyunbasladimi");
            }
        }
        else
        {
            if (powerDongu != null)
            {
                StopCoroutine(powerDongu);
                powerDongu = null;
            }
        }
    }

    IEnumerator PowerBarCalistir()
    {
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
        if (!pw.IsMine || PhotonNetwork.PlayerList.Length == 1) return;

        bool fireInput = false;
        if (Input.GetKeyDown(KeyCode.Space) && atesetme) fireInput = true;
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI() && atesetme) fireInput = true;
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began && !IsTouchOverUI(t)) fireInput = true;
        }

        if (fireInput && atesetme)
        {
            Fire();
        }
    }

    void Fire()
    {
        if (!atesetme) return;

        // Eğer zaten bir aktif top varsa atma
        if (activeTopViewId != 0)
        {
            Debug.Log("[Oyuncu] Zaten aktif top var, atış engellendi. viewId: " + activeTopViewId);
            return;
        }

        atesetme = false;

        // instantiate top
        GameObject topobjem = PhotonNetwork.Instantiate("Top", TopCikisnoktasi.transform.position, TopCikisnoktasi.transform.rotation);
        var topPv = topobjem.GetComponent<PhotonView>();
        if (topPv != null)
        {
            // active topu local olarak sakla (view id ile)
            activeTopViewId = topPv.ViewID;

            // TagAktar'a artık ownerViewId de gönderiyoruz (2. parametre)
            var myPv = GetComponent<PhotonView>();
            topPv.RPC("TagAktar", RpcTarget.All, gameObject.tag, myPv.ViewID);
        }

        if (TopAtmaSesi != null) TopAtmaSesi.Play();

        Rigidbody2D rg = topobjem.GetComponent<Rigidbody2D>();
        if (rg != null && PowerBar != null)
            rg.AddForce(new Vector2(AtisYonu, 0f) * PowerBar.fillAmount * 10, ForceMode2D.Impulse);

        if (powerDongu != null)
        {
            StopCoroutine(powerDongu);
            powerDongu = null;
        }
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject();
    }

    bool IsTouchOverUI(Touch touch)
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }
}
