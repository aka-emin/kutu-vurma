using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class odul : MonoBehaviour
{
    PhotonView pw;
    void Start()
    {
        pw = GetComponent<PhotonView>();
        StartCoroutine(yokol());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("top"))
        {
            pw.gameObject.SetActive(false);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        
    }
        IEnumerator yokol()
    {

        yield return new WaitForSeconds(10f);
        if (pw.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
