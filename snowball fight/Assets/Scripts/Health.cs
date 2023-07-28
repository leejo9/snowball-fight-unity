using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;

public class Health : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Image healthbarImage;

    const float MaxHealth = 100f;
    float health = MaxHealth;
    PlayerManager playerManager;
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }
    void Update()
    {

        if (health > MaxHealth)
        {
            health = MaxHealth;
        }
        if (health < 0)
        {
            Die();

        }

    }
    public void TakeDamage(int amount)
    {
        if (!photonView.IsMine)
        {
            Debug.Log("ok work");
            health -= amount;
        }
        healthbarImage.fillAmount = health / MaxHealth;

    }

    [PunRPC]
    void Damage(int amount)
    {
        health -= amount;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else if (stream.IsReading)
        {
            health = (float)stream.ReceiveNext();
        }
    }
    void Die()
    {
        playerManager.Die();
    }

}
