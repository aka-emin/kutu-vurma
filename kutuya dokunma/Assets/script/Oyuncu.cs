using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Oyuncu : MonoBehaviour
{
    public GameObject top;
    public GameObject TopCikisnoktasi;
    public ParticleSystem TopAtisEfekt;
    public AudioSource TopAtmaSesi;
    float AtisYonu;


    [Header("GÜÇ BARI AYARLARI")]
    Image PowerBar;
    float powerSayi;
    bool sonageldimi = false;
    Coroutine powerDongu;

    PhotonView pw;
    void Start()
    {

        pw = GetComponent<PhotonView>();

        if (pw.IsMine)
        {
            PowerBar = GameObject.FindWithTag("PowerBar").GetComponent<Image>();
            if (PhotonNetwork.IsMasterClient)
            {
                // gameObject.tag = "Oyuncu_1";
                transform.position = GameObject.FindWithTag("oyuncuNoktasi1").transform.position;
                transform.rotation = GameObject.FindWithTag("oyuncuNoktasi1").transform.rotation;
                AtisYonu = 2f;
            }
            else
            {
                //  gameObject.tag = "Oyuncu_2";
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
        if (PowerBar != null) // sadece kendi PowerBar’ı olan oyuncuda çalışsın
        {
            powerDongu = StartCoroutine(PowerBarCalistir());
        }
    }
    public void Oyunbasladimi()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            if (pw.IsMine)
            {
                powerDongu = StartCoroutine(PowerBarCalistir());
                CancelInvoke("Oyunbasladimi");

            }
        }
        else
        {
            //StopAllCoroutines();
        }
    }


    IEnumerator PowerBarCalistir()
    {
        PowerBar.fillAmount = 0;
        sonageldimi = false;

        while (true)
        {
            if (PowerBar.fillAmount < 1 && !sonageldimi)
            {
                powerSayi = 0.01f;
                PowerBar.fillAmount += powerSayi;
                yield return new WaitForSeconds(0.001f * Time.deltaTime);

            }
            else
            {
                sonageldimi = true;
                powerSayi = 0.01f;
                PowerBar.fillAmount -= powerSayi;
                yield return new WaitForSeconds(0.001f * Time.deltaTime);

                if (PowerBar.fillAmount == 0)
                {
                    sonageldimi = false;

                }

            }


        }

    }
    // Update is called once per frame
    void Update()
    {
        if (pw.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PhotonNetwork.Instantiate("CFX_ElectricityBall", TopCikisnoktasi.transform.position, TopCikisnoktasi.transform.rotation);
                TopAtmaSesi.Play();
                GameObject topobjem = PhotonNetwork.Instantiate("Top", TopCikisnoktasi.transform.position, TopCikisnoktasi.transform.rotation);
                topobjem.GetComponent<PhotonView>().RPC("TagAktar", RpcTarget.All, gameObject.tag);
                Rigidbody2D rg = topobjem.GetComponent<Rigidbody2D>();
                rg.AddForce(new Vector2(AtisYonu, 0f) * PowerBar.fillAmount * 10, ForceMode2D.Impulse);


                StopAllCoroutines();

            }

        }
    }
   
}
