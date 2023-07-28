using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, springSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] Item[] items;
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;
    int itemIndex;
    int previousItemIndex = 0;

    Rigidbody rb;
    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    PhotonView PV;
    PlayerManager playerManager;



    public GameObject projectile;
    public Transform point;
    private Vector3 destination;
    public float projectileSpeed = 30f;
    public Camera cam;
    private float timeToFire;
    public float fireRate = 4;



    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;

        if (PV.IsMine)
        {
            gameObject.tag = "LocalPlayer";
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);  
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }
    private void Update()
    {
        if (!PV.IsMine)
            return;
        Look();
        Move();
        Jump();
        if (Input.GetButton("Fire1")&&Time.time >=timeToFire)
        {

            timeToFire = Time.time + 1 / fireRate;
            ShootProjectile();
        }

        if (transform.position.y < -10f)        
        {
            Die();
        }

    }
    [PunRPC]
    void ShootProjectile()
    {
               
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
            destination = hit.point;
            }
            else
            {
                destination = ray.GetPoint(1000);
            }
            InstantiateProjectile(point);
     
        }

    [PunRPC]
    void InstantiateProjectile(Transform firePoint)
    {
        var projectileObj = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * projectileSpeed;

    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? springSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);

    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }
    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }


    public void TakeDamage(int damage)
    {

        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);


    }
    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        Debug.Log("working or not?");

        if (!PV.IsMine)
            return;       
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        healthbarImage.fillAmount = currentHealth / maxHealth;

    }

    void Die()
    {
        playerManager.Die();
    }
}

