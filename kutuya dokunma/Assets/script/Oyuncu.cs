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
     Image powerbar;
    float arttir = 0.01f;
    bool ayari = false;
    PhotonView pw;
    float atisyonu;
    void Start()
    {
        powerbar = GameObject.FindWithTag("powerbar").GetComponent<Image>();
        pw = GetComponent<PhotonView>();
        if (pw.IsMine)
        {
            //GetComponent<Oyuncu>().enabled = true;
            if (PhotonNetwork.IsMasterClient)
            {
                gameObject.tag = "oyuncu1";
                transform.position = GameObject.FindWithTag("oyuncuNoktasi1").transform.position;
                transform.rotation = GameObject.FindWithTag("oyuncuNoktasi1").transform.rotation;
                atisyonu = 2f;
            }
            else
            {
                gameObject.tag = "oyuncu2";

                transform.position = GameObject.FindWithTag("oyuncuNoktasi2").transform.position;
                transform.rotation = GameObject.FindWithTag("oyuncuNoktasi2").transform.rotation /** Quaternion.Euler(0, 180, 0)*/;


                atisyonu = -2f;
            }
            Poweroynat();

            InvokeRepeating("oyunbaslasinim",0f,.5f);
        }
    }
    public void oyunbaslasinim()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            Poweroynat();
            //if (PhotonNetwork.IsMasterClient)
            //{
            //   // GameObject.FindWithTag("oyuncu1").GetComponent<Oyuncu>().enabled = false;
            //}
            //else
            //{
            //    //GameObject.FindWithTag("oyuncu2").GetComponent<Oyuncu>().enabled = false;

            //}
        }
        CancelInvoke("oyunbaslasinim");
    }
    private Coroutine powerCoroutine;

    public void Poweroynat()
    {
        if (powerCoroutine == null)
            powerCoroutine = StartCoroutine(Poweryap());
    }

    public void StopPower()
    {
        if (powerCoroutine != null)
        {
            StopCoroutine(powerCoroutine);
            powerCoroutine = null;
        }
    }
    IEnumerator Poweryap()
    {
        while (true)
        {
            if (powerbar.fillAmount < 1 && !ayari)
            {
                powerbar.fillAmount += arttir;
                yield return new WaitForSeconds(0.001f);
            }
            else
            {
                ayari = true;
                powerbar.fillAmount -= arttir;
                yield return new WaitForSeconds(0.001f);
                if (powerbar.fillAmount <= 0)
                {
                    ayari = false;
                }
            }
        }
    }
        // Update is called once per frame
        void Update()
        {
        if(pw.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
              PhotonNetwork.Instantiate("CFX_ElectricityBall", TopCikisnoktasi.transform.position, TopCikisnoktasi.transform.rotation);
                TopAtmaSesi.Play();
                GameObject topobjem = PhotonNetwork.Instantiate("Top", TopCikisnoktasi.transform.position, TopCikisnoktasi.transform.rotation);
                topobjem.GetComponent<PhotonView>().RPC("tagaktar", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
                Rigidbody2D rg = topobjem.GetComponent<Rigidbody2D>();
                rg.AddForce(new Vector2(atisyonu, 0f) * powerbar.fillAmount*10, ForceMode2D.Impulse);
                

               StopPower();

            }
            
        }
            
        }
    
}
