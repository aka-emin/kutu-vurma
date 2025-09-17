using UnityEngine;
using System.Collections;
using Photon.Pun;  // Photon kütüphanesini ekle

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(PhotonView))]  // PhotonView eklenmesini garantiye al
public class CFX_AutoDestructShuriken : MonoBehaviour
{
    // Eðer true ise destroy yerine sadece disable yapýlýr
    public bool OnlyDeactivate;

    private PhotonView pv;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void OnEnable()
    {
        StartCoroutine(CheckIfAlive());
    }

    IEnumerator CheckIfAlive()
    {
        ParticleSystem ps = this.GetComponent<ParticleSystem>();

        while (true && ps != null)
        {
            yield return new WaitForSeconds(0.5f);
            if (!ps.IsAlive(true))
            {
                if (OnlyDeactivate)
                {
#if UNITY_3_5
                    this.gameObject.SetActiveRecursively(false);
#else
                    this.gameObject.SetActive(false);
#endif
                }
                else
                {
                    // Eðer Photon objesi ise network destroy
                    if (pv != null && pv.IsMine)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                    
                }
                break;
            }
        }
    }
}
