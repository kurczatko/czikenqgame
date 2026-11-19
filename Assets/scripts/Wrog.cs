using UnityEngine;

public class Wrog : MonoBehaviour
{
    public int maksymalneHp = 100;
    public int aktualneHp = 100;
    public float predkosc = 3.5f;
    public float dystansAtaku = 2f;
    public int obrazeniaAtaku = 10;
    public float cooldownAtaku = 1.5f;

    private Transform gracz;
    private float czasOstatniegoAtaku;
    private bool martwy = false;

    void Start()
    {
        aktualneHp = maksymalneHp;
        GameObject g = GameObject.FindWithTag("Player");
        if (g != null) gracz = g.transform;
    }

    void Update()
    {
        if (martwy || gracz == null) return;

        Vector3 kierunek = (gracz.position - transform.position).normalized;
        kierunek.y = 0f;

        float dystans = Vector3.Distance(transform.position, gracz.position);

        if (dystans > dystansAtaku)
        {
            transform.position += kierunek * predkosc * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(kierunek), 10f * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(kierunek), 10f * Time.deltaTime);
            Atakuj();
        }
    }

    void Atakuj()
    {
        if (Time.time - czasOstatniegoAtaku < cooldownAtaku) return;
        czasOstatniegoAtaku = Time.time;

        ZdrowieGracza zdrowie = gracz.GetComponent<ZdrowieGracza>();
        if (zdrowie != null)
        {
            zdrowie.ZabierzHp(obrazeniaAtaku);
        }
    }

    public void PrzyjmijObrazenia(int obrazenia)
    {
        if (martwy) return;

        aktualneHp -= obrazenia;
        Debug.Log($"Wrog trafiony! HP: {aktualneHp}/{maksymalneHp}");

        if (aktualneHp <= 0)
        {
            Umrzyj();
        }
    }

    void Umrzyj()
    {
        martwy = true;
        Debug.Log("Wrog umarl.");
        Destroy(gameObject);
    }
}
