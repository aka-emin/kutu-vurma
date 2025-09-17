
using Photon.Pun;
using UnityEngine;

public class Top : MonoBehaviour
{
    float darbegucu;
    int benkimim;


    GameObject gameKontrol;
    GameObject Oyuncu;
    PhotonView pw;
   public AudioSource YokOlmaSesi;


    void Start()
    {
        darbegucu = 20;
        gameKontrol = GameObject.FindWithTag("GameKontrol");
        pw = GetComponent<PhotonView>();
        YokOlmaSesi = GetComponent<AudioSource>();
    }

    [PunRPC]
    public void TagAktar(string gelentag)
    {
        Oyuncu = GameObject.FindWithTag(gelentag);

        if (gelentag == "oyuncu1")
            benkimim = 1;
        else
            benkimim = 2;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.CompareTag("Ortadaki_kutular"))
        {
            collision.gameObject.GetComponent<PhotonView>().RPC("darbeal", RpcTarget.All, darbegucu);

            // collision.gameObject.GetComponent<ortadaki_kutu>().darbeal(darbegucu);
            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation, 0, null);
            YokOlmaSesi.Play();
            //Oyuncu.GetComponent<Oyuncu>().PowerOynasin();
            Oyuncu.GetComponent<Oyuncu>().PowerOynasin();
            if (pw.IsMine)
           PhotonNetwork.Destroy(gameObject);

            // GetComponent<CircleCollider2D>().isTrigger = false;

        }
        if (collision.gameObject.CompareTag("Zemin"))
        {
            //Oyuncu.GetComponent<Oyuncu>().PowerOynasin();
            Oyuncu.GetComponent<Oyuncu>().PowerOynasin();
            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation, 0, null);
            YokOlmaSesi.Play();
            if (pw.IsMine)
                PhotonNetwork.Destroy(gameObject);

        }

        if (collision.gameObject.CompareTag("engel"))
        {

            Oyuncu.GetComponent<Oyuncu>().PowerOynasin();

            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation, 0, null);
            YokOlmaSesi.Play();
            if (pw.IsMine)
                PhotonNetwork.Destroy(gameObject);

        }
        if (collision.gameObject.CompareTag("can"))
        {
            gameKontrol.GetComponent<PhotonView>().RPC("SaglikDoldur", RpcTarget.All, benkimim);
            PhotonNetwork.Destroy(collision.transform.gameObject);
            //Oyuncu.GetComponent<Oyuncu>().PowerOynasin();
            Oyuncu.GetComponent<Oyuncu>().PowerOynasin();

            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation, 0, null);
            YokOlmaSesi.Play();
            if (pw.IsMine)
                PhotonNetwork.Destroy(gameObject);

        }
        if (collision.gameObject.CompareTag("oyuncu2") || collision.gameObject.CompareTag("Oyuncu_2_Kule"))
        {
            PhotonNetwork.Instantiate("Duman_puf_Carpma_efekti", transform.position, transform.rotation, 0, null);
            gameKontrol.GetComponent<PhotonView>().RPC("Darbe_vur", RpcTarget.All, 2, darbegucu);
            Debug.Log("oyuncu 2 carpdý");
            //Oyuncu.GetComponent<Oyuncu>().PowerOynasin();
            Oyuncu.GetComponent<Oyuncu>().PowerOynasin();
            if (pw.IsMine)
                PhotonNetwork.Destroy(gameObject);
            //GetComponent<CircleCollider2D>().isTrigger = false;

        }
        if (collision.gameObject.CompareTag("oyuncu1") || collision.gameObject.CompareTag("Oyuncu_1_Kule"))
        {
            Debug.Log("oyuncu 1 carpdý");


            // gameKontrol.GetComponent<GameKontrol>().Ses_ve_Efekt_Olustur(1, collision.gameObject);
            gameKontrol.GetComponent<PhotonView>().RPC("Darbe_vur", RpcTarget.All, 1, darbegucu);

            //Oyuncu.GetComponent<Oyuncu>().PowerOynasin();
            Oyuncu.GetComponent<Oyuncu>().PowerOynasin();
            if (pw.IsMine)
                PhotonNetwork.Destroy(gameObject);
            // GetComponent<CircleCollider2D>().isTrigger = false;

        }
        
    }



    // Update is called once per frame
    void Update()
    {

    }
}
