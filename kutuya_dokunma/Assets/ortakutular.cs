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

    private void Start()
    {
        gameKontrol = GameObject.FindWithTag("GameKontrol");
        pw = GetComponent<PhotonView>();
        KutuYokOlmaSesi = GetComponent<AudioSource>();
    }
    [PunRPC]
    public void darbeal(float dargegucu)
    {

        if (pw.IsMine)
        {

            saglik -= dargegucu;

            healtBar.fillAmount = saglik / 100; // 0.9

            if (saglik <= 0)
            {

                // gameKontrol.GetComponent<GameKontrol>().Ses_ve_Efekt_Olustur(2, gameObject);

                PhotonNetwork.Instantiate("Kutu_kirilma_efekt", transform.position, transform.rotation, 0, null);
                KutuYokOlmaSesi.Play();
                PhotonNetwork.Destroy(gameObject);

            }
            else
            {
                StartCoroutine(CanvasCikar());

            }

        }



    }


    IEnumerator CanvasCikar()
    {
        if (!SaglikCanvasý.activeInHierarchy)
        {
            SaglikCanvasý.SetActive(true);
            yield return new WaitForSeconds(2);
            SaglikCanvasý.SetActive(false);
        }

    }


}
