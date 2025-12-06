using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ortakutular : MonoBehaviour
{
    float saglik = 100;
    public GameObject SaglikCanvasý;
    public Image healtBar;

    GameObject gameKontrol;
    PhotonView pw;
    public AudioSource KutuYokOlmaSesi;

    // Canvas coroutine referansý (restart edebilmek için)
    Coroutine canvasCoroutine;

    private void Start()
    {
        gameKontrol = GameObject.FindWithTag("GameKontrol");
        pw = GetComponent<PhotonView>();
        KutuYokOlmaSesi = GetComponent<AudioSource>();

        // Baþlangýçta canvas gizli olmalý
        if (SaglikCanvasý != null)
            SaglikCanvasý.SetActive(false);
    }

    [PunRPC]
    public void darbeal(float dargegucu)
    {
        // Hasarý sadece nesnenin sahibi uygulasýn (yetkili client)
        if (pw.IsMine)
        {
            saglik -= dargegucu;
            if (healtBar != null)
                healtBar.fillAmount = saglik / 100f;

            if (saglik <= 0)
            {
                // Yýkým efekti ve yok etme (aðda da yok olur)
                PhotonNetwork.Instantiate("Kutu_kirilma_efekt", transform.position, transform.rotation, 0, null);
                if (KutuYokOlmaSesi != null) KutuYokOlmaSesi.Play();

                // Diðer client'larda saðlýk göstergesinin 0 olmasýný isteyebiliriz
                pw.RPC("RPC_ShowHealthCanvas", RpcTarget.Others, 0f);

                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                // Sahip client üzerinde canvas'ý göster / timer'ý restart et
                if (canvasCoroutine != null)
                    StopCoroutine(canvasCoroutine);
                canvasCoroutine = StartCoroutine(CanvasCikar());

                // Diðer client'larda da health bar deðerini ve canvas görünürlüðünü güncelle
                pw.RPC("RPC_ShowHealthCanvas", RpcTarget.Others, saglik);
            }
        }
    }

    // Diðer client'larda çaðrýlacak: health deðerini güncelle ve canvas'ý göster
    [PunRPC]
    public void RPC_ShowHealthCanvas(float remoteSaglik)
    {
        // remoteSaglik, owner tarafýndan gönderilen güncel saðlýk deðeri
        if (healtBar != null)
            healtBar.fillAmount = Mathf.Clamp01(remoteSaglik / 100f);

        // Canvas coroutine'ini restart et (ayný davranýþý owner ile paylaþ)
        if (canvasCoroutine != null)
            StopCoroutine(canvasCoroutine);
        canvasCoroutine = StartCoroutine(CanvasCikar());
    }

    IEnumerator CanvasCikar()
    {
        if (SaglikCanvasý != null)
        {
            // her tetiklemede timer'ý sýfýrlamak için önce aktif et
            SaglikCanvasý.SetActive(true);
            yield return new WaitForSeconds(2f);
            SaglikCanvasý.SetActive(false);
        }
    }
}
