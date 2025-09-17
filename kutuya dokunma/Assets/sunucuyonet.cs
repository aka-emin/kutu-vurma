using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using TMPro;


public class sunucuyonet : MonoBehaviourPunCallbacks
{
    void Start()
    {
       
        PhotonNetwork.ConnectUsingSettings();
        DontDestroyOnLoad(gameObject);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("sunucuya baglandý");
        PhotonNetwork.JoinLobby();
    }
    public void Odakur()
    {
        PhotonNetwork.LoadLevel(1);
        string odadi = Random.Range(0, 2345).ToString();
        PhotonNetwork.JoinOrCreateRoom(odadi, new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true }, TypedLobby.Default);
    }
    public void RasgeleOdayakatýl()
    {
        PhotonNetwork.LoadLevel(1);
PhotonNetwork.JoinRandomRoom();
       
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("lobiye baglandý");
        
        
    }
    public override void OnJoinedRoom()
    {
        InvokeRepeating("BigileriKontrolEt", 0, 1f);

        Debug.Log("odaya baglandý");
        GameObject objem = PhotonNetwork.Instantiate("Oyuncu1", Vector3.zero, Quaternion.identity,0,null);
        objem.GetComponent<PhotonView>().Owner.NickName = PlayerPrefs.GetString("kullanýcý");
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            objem.gameObject.tag = "oyuncu1";
        }
        else
        {
            objem.gameObject.tag = "oyuncu2";     
                GameObject.FindWithTag("GameKontrol").gameObject.GetComponent<PhotonView>().RPC("Basla", RpcTarget.All);          
        }
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Odadan çýkýldý");

    }
    public override void OnLeftLobby()
    {
        Debug.Log("Lobiden çýkýldý");

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
       InvokeRepeating("BigileriKontrolEt", 0, 1f);
        //herhnagý bý royuncu cýkdý
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // herhangi bir oyuncu girdiðinde tetiklenen fonksiyondur.
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
        Debug.Log("Oda oluþturulamadý");

    }
    void BigileriKontrolEt()
    {
        GameObject panelim = GameObject.FindWithTag("oyuncubekleniyor");
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            panelim.SetActive(false);
            GameObject.FindWithTag("Oyuncu1_isim").GetComponent<TextMeshProUGUI>().text=PhotonNetwork.PlayerList[0].NickName;
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


    // Update is called once per frame


}