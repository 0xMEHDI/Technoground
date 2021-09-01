using SGoap;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float health = 10f;
    public float movementSpeed = 10f;
    public float damageKnockback = 1f;
    public float damageDelay = 1f;

    [Header("Gun")]
    public Gun startingGun;
    public Transform gunPosition;
    public Transform crosshair;

    [Header("Audio")]
    public AudioClip playerDeathSound;

    Camera mainCamera;
    Rigidbody rigidBody;
    MeshRenderer meshRenderer;
    Material material;
    Gun equippedGun;

    Vector3 move;
    Vector3 velocity;
    Vector3 smoothTime;
    bool damagable = true;

    public event System.Action OnDeath;

    void Awake()
    {
        mainCamera = Camera.main;
        rigidBody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
    }

    void Start()
    {
        StaticStatTracker.playerMaxHealth = health;

        if(startingGun != null)
            EquipGun(startingGun);
    }

    void Update()
    {
        move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        equippedGun.transform.SetPositionAndRotation(gunPosition.position, gunPosition.rotation);

        if (gameObject.transform.position.y < -10)
            health = 0;

        Aim();
        Shoot();
        CheckHealth();
    }

    void FixedUpdate()
    {
        Move();
        AnimateCrosshair(GetMousePosition());

        gunPosition.localPosition = Vector3.SmoothDamp(gunPosition.localPosition, new Vector3(0.575f, 0, -0.25f), ref smoothTime, 0.05f);
    }

    void Move()
    {
        velocity = move.normalized * movementSpeed;
        rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);
    }

    void Aim()
    {
        Quaternion rotation = Quaternion.LookRotation(GetMousePosition() - gunPosition.position);
        transform.rotation = rotation;
    }

    void Shoot()
    {
        if (equippedGun != null)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                equippedGun.OnTriggerPress();
                gunPosition.localPosition -= Vector3.forward * 0.02f;
            }

            else
                equippedGun.OnTriggerRelease();
        }
    }

    void EquipGun(Gun gun)
    {
        if (equippedGun != null)
            Destroy(equippedGun);

        equippedGun = Instantiate(gun, gunPosition.position, gunPosition.rotation);
        equippedGun.transform.parent = gameObject.transform;
    }

    void CheckHealth()
    {
        StaticStatTracker.UpdatePlayerHealth((int)health);

        if (health <= 0)
        {
            AudioManager.instance.PlaySound(playerDeathSound, transform.position);

            StopCoroutine(DamageDelay());

            OnDeath?.Invoke();

            meshRenderer.enabled = false;
            enabled = false;
        }
    }

    void AnimateCrosshair(Vector3 point)
    {
        Cursor.visible = false;
        crosshair.position = point;
        crosshair.transform.Rotate(25 * Time.deltaTime * Vector3.forward);
    }

    Vector3 GetMousePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, gunPosition.position);

        if (ground.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);

            return point;
        }

        return Vector3.zero;
    }

    IEnumerator DamageDelay()
    {
        damagable = false;

        Color currentColor = material.color;
        material.color = Color.red;
        
        yield return new WaitForSeconds(0.25f);

        material.color = currentColor;

        yield return new WaitForSeconds(damageDelay);
  
        damagable = true;
    }

    void OnCollisionStay(Collision collision)
    {
        if (damagable && collision.gameObject.CompareTag("Enemy"))
        {
            health--;
            rigidBody.AddRelativeForce(new Vector3(1, 0, 1) * damageKnockback, ForceMode.Impulse);
            StartCoroutine(DamageDelay());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damagable && other.gameObject.CompareTag("Bullet"))
        {
            health--;
            rigidBody.AddRelativeForce(new Vector3(1, 0, 1) * (damageKnockback / 10), ForceMode.Impulse);
            StartCoroutine(DamageDelay());
        }
    }
}
