
using Photon.Pun;
using UnityEngine;

public class Top : MonoBehaviour
{
    float darbegucu;

    public string Aitolduguobje;
    GameObject gameKontrol;
    GameObject AitOyuncum;
    public PhotonView pw;
    void Start()
    {
        pw=GetComponent<PhotonView>();
        darbegucu = 20;
        gameKontrol = GameObject.FindWithTag("GameKontrol");
        
    }
    [PunRPC]
    public void tagaktar(int ownerId)
    {
        var player = PhotonNetwork.CurrentRoom.GetPlayer(ownerId);
        if (player != null)
        {
            Debug.Log("Bu topu atan oyuncu: " + player.NickName);

            // Yeni API: FindObjectsByType (hem aktif hem inaktif objeler için)
            Oyuncu[] oyuncular = FindObjectsByType<Oyuncu>(FindObjectsSortMode.None);

            foreach (Oyuncu o in oyuncular)
            {
                PhotonView pv = o.GetComponent<PhotonView>();
                if (pv != null && pv.OwnerActorNr == ownerId) // eþleþme bulundu
                {
                    AitOyuncum = o.gameObject;
                    Debug.Log("AitOyuncum bulundu: " + AitOyuncum.name);
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("Player bulunamadý! ownerId: " + ownerId);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.CompareTag("Ortadaki_kutular"))
        {
            collision.gameObject.GetComponent<ortadaki_kutu>().darbeal(darbegucu);
            gameKontrol.GetComponent<GameKontrol>().Ses_ve_Efekt_Olustur(1, collision.gameObject);
            AitOyuncum.GetComponent<Oyuncu>().Poweroynat();
            if(pw.IsMine)
           PhotonNetwork.Destroy(gameObject);

            // GetComponent<CircleCollider2D>().isTrigger = false;

        }
        if (collision.gameObject.CompareTag("engel"))
        {
            
            gameKontrol.GetComponent<GameKontrol>().Ses_ve_Efekt_Olustur(1, collision.gameObject);
            AitOyuncum.GetComponent<Oyuncu>().Poweroynat();
            if (pw.IsMine)
                PhotonNetwork.Destroy(gameObject);

            // GetComponent<CircleCollider2D>().isTrigger = false;

        }
        if (collision.gameObject.CompareTag("can"))
        {

            gameKontrol.GetComponent<GameKontrol>().Ses_ve_Efekt_Olustur(1, collision.gameObject);
            AitOyuncum.GetComponent<Oyuncu>().Poweroynat();
            if (pw.IsMine)
                PhotonNetwork.Destroy(gameObject);
            if(AitOyuncum.CompareTag("oyuncu1"))
            gameKontrol.GetComponent<GameKontrol>().CanAl(0, .1f);
            else
                gameKontrol.GetComponent<GameKontrol>().CanAl(1, .1f);

            // GetComponent<CircleCollider2D>().isTrigger = false;

        }
        if (collision.gameObject.CompareTag("oyuncu1") || collision.gameObject.CompareTag("Oyuncu_2_Kule"))
        {
            gameKontrol.GetComponent<GameKontrol>().Ses_ve_Efekt_Olustur(1, collision.gameObject);
            gameKontrol.GetComponent<GameKontrol>().darbeAl(1,.1f);

            AitOyuncum.GetComponent<Oyuncu>().Poweroynat();
           PhotonNetwork.Destroy(gameObject);

           //GetComponent<CircleCollider2D>().isTrigger = false;

        }
        if (collision.gameObject.CompareTag("oyuncu2") || collision.gameObject.CompareTag("Oyuncu_1_Kule"))
        {
            gameKontrol.GetComponent<GameKontrol>().Ses_ve_Efekt_Olustur(1, collision.gameObject);
            gameKontrol.GetComponent<GameKontrol>().darbeAl(0, .1f);

            AitOyuncum.GetComponent<Oyuncu>().Poweroynat();
            PhotonNetwork.Destroy(gameObject);

            // GetComponent<CircleCollider2D>().isTrigger = false;

        }
    }



    // Update is called once per frame
    void Update()
    {

    }
}
