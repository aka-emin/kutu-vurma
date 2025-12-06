using Photon.Pun;
using UnityEngine;

public class Top : MonoBehaviour
{
    float darbegucu;
    int benkimim;

    GameObject gameKontrol;
    PhotonView gameKontrolPV;
    GameObject Oyuncu;
    PhotonView pw;

    // Yeni: topun sahibi oyuncunun view id'si (kesin eþleme için)
    int ownerPlayerViewId = 0;
    PhotonView ownerPlayerPV;

    public AudioSource YokOlmaSesi;

    void Start()
    {
        darbegucu = 20;
        gameKontrol = GameObject.FindWithTag("GameKontrol");
        if (gameKontrol != null) gameKontrolPV = gameKontrol.GetComponent<PhotonView>();
        pw = GetComponent<PhotonView>();
        YokOlmaSesi = GetComponent<AudioSource>();
    }

    // TagAktar artýk ownerViewId paramresi de alýr
    [PunRPC]
    public void TagAktar(string gelentag, int ownerViewId)
    {
        Oyuncu = GameObject.FindWithTag(gelentag);
        benkimim = (gelentag == "oyuncu1") ? 1 : 2;

        ownerPlayerViewId = ownerViewId;
        // ownerPlayerPV (yerelde) bulunursa cachele
        ownerPlayerPV = PhotonView.Find(ownerPlayerViewId);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ortadaki kutu
        if (collision.gameObject.CompareTag("Ortadaki_kutular"))
        {
            collision.gameObject.GetComponent<PhotonView>().RPC("darbeal", RpcTarget.All, darbegucu);
            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation);
            if (YokOlmaSesi != null) YokOlmaSesi.Play();
            NotifyOwnerTopCleared();
            if (pw.IsMine) PhotonNetwork.Destroy(gameObject);
            return;
        }

        // Zemin
        if (collision.gameObject.CompareTag("Zemin"))
        {
            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation, 0, null);
            if (YokOlmaSesi != null) YokOlmaSesi.Play();
            NotifyOwnerTopCleared();
            if (pw.IsMine) PhotonNetwork.Destroy(gameObject);
            return;
        }

        // Engel
        if (collision.gameObject.CompareTag("engel"))
        {
            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation, 0, null);
            if (YokOlmaSesi != null) YokOlmaSesi.Play();
            NotifyOwnerTopCleared();
            if (pw.IsMine) PhotonNetwork.Destroy(gameObject);
            return;
        }

        // Can
        if (collision.gameObject.CompareTag("can"))
        {
            if (gameKontrolPV != null) gameKontrolPV.RPC("SaglikDoldur", RpcTarget.All, benkimim);
            PhotonNetwork.Destroy(collision.transform.gameObject);
            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation, 0, null);
            if (YokOlmaSesi != null) YokOlmaSesi.Play();
            NotifyOwnerTopCleared();
            if (pw.IsMine) PhotonNetwork.Destroy(gameObject);
            return;
        }

        // Oyuncu çarpmasý (kule vb.)
        if (collision.gameObject.CompareTag("oyuncu2") || collision.gameObject.CompareTag("Oyuncu_2_Kule"))
        {
            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation, 0, null);
            if (gameKontrolPV != null) gameKontrolPV.RPC("Darbe_vur", RpcTarget.All, 2, darbegucu);
            NotifyOwnerTopCleared();
            if (pw.IsMine) PhotonNetwork.Destroy(gameObject);
            return;
        }
        if (collision.gameObject.CompareTag("oyuncu1") || collision.gameObject.CompareTag("Oyuncu_1_Kule"))
        {
            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation, 0, null);
            if (gameKontrolPV != null) gameKontrolPV.RPC("Darbe_vur", RpcTarget.All, 1, darbegucu);
            NotifyOwnerTopCleared();
            if (pw.IsMine) PhotonNetwork.Destroy(gameObject);
            return;
        }
    }

    // Sahip oyuncuya topun temizlendiðini bildir (owner'a RPC)
    void NotifyOwnerTopCleared()
    {
        // ownerPlayerPV cache'lenmiþse doðrudan kullan
        if (ownerPlayerPV == null && ownerPlayerViewId != 0)
            ownerPlayerPV = PhotonView.Find(ownerPlayerViewId);

        if (ownerPlayerPV != null)
        {
            // owner client üzerinde ClearActiveTop çaðrýlýr
            ownerPlayerPV.RPC("ClearActiveTop", ownerPlayerPV.Owner, pw.ViewID);
        }
        else if (Oyuncu != null)
        {
            // fallback: tag ile bul ve RPC gönder (daha az güvenli)
            var fallbackPV = Oyuncu.GetComponent<PhotonView>();
            if (fallbackPV != null)
                fallbackPV.RPC("ClearActiveTop", fallbackPV.Owner, pw.ViewID);
        }
    }
}
