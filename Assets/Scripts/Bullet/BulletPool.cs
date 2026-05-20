using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    // Instance statis untuk Singleton pattern (pola desain yang memastikan hanya ada satu instance)
    public static BulletPool Instance { get; private set; }

    // Prefab peluru yang akan di-clone
    public GameObject bulletPrefab;
    // Jumlah peluru yang akan dibuat di awal permainan
    public int initialPoolSize = 20;

    // Antrian untuk menyimpan peluru yang tersedia untuk digunakan kembali
    private Queue<GameObject> availableBullets;
    // Daftar semua peluru yang ada di pool
    private List<GameObject> allBullets;

    void Awake()
    {
        // Pastikan hanya ada satu instance BulletPool di scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Atur instance statis ke objek ini
        Instance = this;
    }

    void Start()
    {
        // Inisialisasi pool saat game dimulai
        InitializePool();
    }

    void InitializePool()
    {
        // Buat antrian dan daftar dengan kapasitas awal
        availableBullets = new Queue<GameObject>(initialPoolSize);
        allBullets = new List<GameObject>(initialPoolSize);

        // Buat peluru sebanyak initialPoolSize
        for (int i = 0; i < initialPoolSize; i++)
        {
            // Clone peluru dari prefab
            GameObject bullet = Instantiate(bulletPrefab);
            // Nonaktifkan peluru saat dibuat (jangan tampilkan di layar)
            bullet.SetActive(false);
            // Berikan nama untuk memudahkan debugging
            bullet.name = "Bullet_" + i;

            // Ambil komponen Bullet dari peluru
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                // Beri tahu peluru siapa poolnya
                bulletScript.SetPool(this);
            }

            // Masukkan peluru ke antrian (tersedia untuk digunakan)
            availableBullets.Enqueue(bullet);
            // Tambahkan ke daftar semua peluru
            allBullets.Add(bullet);
        }

        Debug.Log($"BulletPool initialized with {initialPoolSize} bullets");
    }

    public GameObject GetBullet(Vector3 position)
    {
        // Deklarasikan variabel untuk menampung peluru
        GameObject bullet;

        // Jika ada peluru yang tersedia di antrian
        if (availableBullets.Count > 0)
        {
            // Ambil peluru dari antrian
            bullet = availableBullets.Dequeue();
        }
        else
        {
            // Jika pool kosong, buat peluru baru
            bullet = Instantiate(bulletPrefab);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                // Beri tahu peluru siapa poolnya
                bulletScript.SetPool(this);
            }
            // Tambahkan ke daftar semua peluru
            allBullets.Add(bullet);
            Debug.Log("Bullet pool exhausted, creating new bullet");
        }

        // Atur posisi peluru ke lokasi yang diminta
        bullet.transform.position = position;
        // Aktifkan peluru (tampilkan di layar)
        bullet.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        // Nonaktifkan peluru (sembunyikan dari layar)
        bullet.SetActive(false);
        // Masukkan kembali ke antrian untuk digunakan lagi
        availableBullets.Enqueue(bullet);
    }

    public int GetAvailableBulletsCount()
    {
        // Kembalikan jumlah peluru yang tersedia
        return availableBullets.Count;
    }
}
