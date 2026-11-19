using UnityEngine;
using UnityEngine.InputSystem;

public class Samochody : MonoBehaviour
{
    [Header("Ustawienia samochodow")]
    public int liczbaSamochodow = 6;
    public float obszarX = 200f;
    public float obszarZ = 200f;
    public float predkoscJazdy = 15f;
    public float predkoscObrotu = 100f;

    [Header("Prefaby")]
    public GameObject prefabSamochodu;
    public GameObject modelSamochodu;

    private GenerowanieTerenu teren;
    private Transform gracz;
    private GameObject aktywnySamochod;
    private bool wSamochodzie = false;

    void Start()
    {
        teren = FindObjectOfType<GenerowanieTerenu>();
        GameObject g = GameObject.FindWithTag("Player");
        if (g != null) gracz = g.transform;

        SpawnujSamochody();
    }

    void Update()
    {
        if (gracz == null) return;

        if (!wSamochodzie)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                SprawdzWsiadniecie();
            }
        }
        else
        {
            Jazda();
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Wysiadaj();
            }
        }
    }

    void SpawnujSamochody()
    {
        int postawiono = 0;
        int proby = 0;
        int maxProb = 200;

        while (postawiono < liczbaSamochodow && proby < maxProb)
        {
            proby++;

            float x = Random.Range(-obszarX / 2f, obszarX / 2f);
            float z = Random.Range(-obszarZ / 2f, obszarZ / 2f);

            float wys = 0f;
            if (teren != null)
            {
                wys = teren.PobierzWysokoscTerenu(new Vector3(x, 0f, z));
            }

            if (wys <= -1f) continue; // woda
            if (wys < 0.5f) continue; // za nisko

            if (gracz != null && Vector3.Distance(new Vector3(x, wys, z), gracz.position) < 20f) continue;

            Vector3 pozycja = new Vector3(x, wys + 0.5f, z);

            GameObject samochod;
            if (prefabSamochodu != null)
            {
                samochod = Instantiate(prefabSamochodu, pozycja, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            }
            else
            {
                samochod = StworzSamochodDomyślny(pozycja);
            }

            Samochod skrypt = samochod.GetComponent<Samochod>();
            if (skrypt == null) skrypt = samochod.AddComponent<Samochod>();
            skrypt.systemSamochodow = this;

            postawiono++;
        }

        Debug.Log($"Samochody: postawiono {postawiono}/{liczbaSamochodow} samochodow.");
    }

    GameObject StworzSamochodDomyślny(Vector3 pozycja)
    {
        GameObject samochod = new GameObject("Samochod");
        samochod.transform.position = pozycja;

        GameObject kadlub = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kadlub.transform.SetParent(samochod.transform, false);
        kadlub.transform.localPosition = Vector3.zero;
        kadlub.transform.localScale = new Vector3(1.8f, 0.8f, 3f);

        GameObject kabina = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kabina.transform.SetParent(samochod.transform, false);
        kabina.transform.localPosition = new Vector3(0f, 0.6f, 0.5f);
        kabina.transform.localScale = new Vector3(1.2f, 0.8f, 1.4f);

        Renderer r1 = kadlub.GetComponent<Renderer>();
        Renderer r2 = kabina.GetComponent<Renderer>();
        if (r1 != null) r1.material.color = Color.red;
        if (r2 != null) r2.material.color = Color.black;

        Collider col = samochod.AddComponent<BoxCollider>();
        col.isTrigger = true;
        ((BoxCollider)col).size = new Vector3(2f, 1.5f, 4f);

        Rigidbody rb = samochod.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        return samochod;
    }

    void SprawdzWsiadniecie()
    {
        Collider[] kolizje = Physics.OverlapSphere(gracz.position, 5f);
        foreach (var kolider in kolizje)
        {
            Samochod samochod = kolider.GetComponent<Samochod>();
            if (samochod != null)
            {
                Wsiadz(samochod.gameObject);
                return;
            }
        }
    }

    void Wsiadz(GameObject samochod)
    {
        aktywnySamochod = samochod;
        wSamochodzie = true;

        gracz.gameObject.SetActive(false);

        Debug.Log("Wsiadles do samochodu. ESC aby wysiasc.");
    }

    void Jazda()
    {
        if (aktywnySamochod == null) return;

        float ruchZ = 0f;
        float obrot = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) ruchZ = 1f;
            if (Keyboard.current.sKey.isPressed) ruchZ = -1f;
            if (Keyboard.current.aKey.isPressed) obrot = -1f;
            if (Keyboard.current.dKey.isPressed) obrot = 1f;
        }

        aktywnySamochod.transform.Rotate(0f, obrot * predkoscObrotu * Time.deltaTime, 0f);
        aktywnySamochod.transform.position += aktywnySamochod.transform.forward * ruchZ * predkoscJazdy * Time.deltaTime;

        float wys = 0f;
        if (teren != null)
        {
            wys = teren.PobierzWysokoscTerenu(aktywnySamochod.transform.position);
        }
        Vector3 pos = aktywnySamochod.transform.position;
        pos.y = wys + 0.5f;
        aktywnySamochod.transform.position = pos;
    }

    void Wysiadaj()
    {
        if (aktywnySamochod == null) return;

        Vector3 pozycjaWysiadki = aktywnySamochod.transform.position - aktywnySamochod.transform.forward * 3f;
        float wys = 0f;
        if (teren != null)
        {
            wys = teren.PobierzWysokoscTerenu(pozycjaWysiadki);
        }
        pozycjaWysiadki.y = wys + 1f;

        gracz.position = pozycjaWysiadki;
        gracz.gameObject.SetActive(true);

        wSamochodzie = false;
        aktywnySamochod = null;

        Debug.Log("Wysiadles z samochodu.");
    }
}

public class Samochod : MonoBehaviour
{
    public Samochody systemSamochodow;
}
