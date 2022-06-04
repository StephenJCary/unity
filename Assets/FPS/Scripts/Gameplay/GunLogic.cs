using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunLogic : MonoBehaviour
{
    [SerializeField]
    GameObject m_bulletPrefab;

    [SerializeField]
    Transform m_bulletSpawnPoint;

    [SerializeField]
    Text m_ammoText;

    const float MAX_COOLDOWN = 0.5f;
    float m_cooldown = 0.0f;

    const int MAX_BULLETS = 10;
    int m_bullets = MAX_BULLETS;

    [SerializeField]
    AudioClip m_pistolShot;

    [SerializeField]
    AudioClip m_pistolEmpty;

    [SerializeField]
    AudioClip m_pistolReload;

    AudioSource m_audioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();

        UpdateAmmoText();
    }

    // Update is called once per frame
    void Update()
    {
        // Apply cooldown
        if(m_cooldown > 0.0f)
        {
            m_cooldown -= Time.deltaTime;
        }

        if(Input.GetButton("Fire") && m_cooldown <= 0.0f)
        {
            Shoot();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void Shoot()
    {
        // Set cooldown to max
        m_cooldown = MAX_COOLDOWN;

        if(m_bullets > 0)
        {
            // Spawn bullet
            Instantiate(m_bulletPrefab, m_bulletSpawnPoint.position, m_bulletSpawnPoint.rotation);

            // Reduce ammo
            --m_bullets;

            // Update UI
            UpdateAmmoText();

            // Play shot sound
            m_audioSource.PlayOneShot(m_pistolShot);
        }
        else
        {
            // Play clicking sound...
            m_audioSource.PlayOneShot(m_pistolEmpty);

            Debug.Log("Out of Ammo");
        }
    }

    void Reload()
    {
        m_bullets = MAX_BULLETS;
        UpdateAmmoText();

        // Play reload sound...
        m_audioSource.PlayOneShot(m_pistolReload);
    }

    void UpdateAmmoText()
    {
        //m_ammoText.text = "Ammo: " + m_bullets;
    }
}
