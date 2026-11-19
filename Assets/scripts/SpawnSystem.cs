using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    [Header("Bronie do spawnowania")]
    public GameObject[] prefabyBronii;
    public int liczbaBronii = 8;

    [Header("Wrogowie do spawnowania")]
    public GameObject prefabWroga;
    public int liczbaWrogow = 6;

    [Header("Obszar spawnowania")]
    public float obszarX = 200f;
    public float obszarZ = 200f;
    public float minimalnaWysokoscDlaSpawnu = 0f;

    [Header("Odstep od gracza")]
    public float minimalnaOdleglOdGracza = 10f;

    private GenerowanieTerenu teren;
    private Transform gracz;

    void Start()
    {
        teren = FindObjectOfType<GenerowanieTerenu>();
        GameObject g = GameObject.FindWithTag("Player");
        if (g != null) gracz = g.transform;

        Debug.Log($"SpawnSystem: teren={(teren != null ? "OK" : "BRAK")}, gracz={(gracz != null ? "OK" : "BRAK")}");

        SpawnujBronie();
        SpawnujWrogow();
    }

    void SpawnujBronie()
    {
        int postawiono = 0;
        for (int i = 0; i < liczbaBronii; i++)
        {
            Vector3 pozycja;
            if (!LosowaPozycjaNaTerenie(out pozycja)) continue;

            GameObject bronObj;
            if (prefabyBronii != null && prefabyBronii.Length > 0)
            {
                int indeks = Random.Range(0, prefabyBronii.Length);
                bronObj = Instantiate(prefabyBronii[indeks], pozycja, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            }
            else
            {
                bronObj = StworzBronDomyślna(pozycja);
            }

            Bron bron = bronObj.GetComponent<Bron>();
            if (bron == null) bron = bronObj.AddComponent<Bron>();
            if (string.IsNullOrEmpty(bron.statystyki.nazwa)) bron.statystyki.nazwa = "Bron " + (i + 1);
            if (bron.statystyki.obrazenia <= 0) bron.statystyki.obrazenia = 10;

            Collider col = bronObj.GetComponent<Collider>();
            if (col == null)
            {
                col = bronObj.AddComponent<SphereCollider>();
                col.isTrigger = true;
            }

            BronPickup pickup = bronObj.GetComponent<BronPickup>();
            if (pickup == null) pickup = bronObj.AddComponent<BronPickup>();

            postawiono++;
        }

        Debug.Log($"SpawnSystem: postawiono {postawiono}/{liczbaBronii} bronii.");
    }

    GameObject StworzBronDomyślna(Vector3 pozycja)
    {
        GameObject bron = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bron.transform.position = pozycja;
        bron.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        bron.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        Renderer rend = bron.GetComponent<Renderer>();
        if (rend != null) rend.material.color = Color.yellow;

        Collider col = bron.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        return bron;
    }

    void SpawnujWrogow()
    {
        int postawiono = 0;
        for (int i = 0; i < liczbaWrogow; i++)
        {
            Vector3 pozycja;
            if (!LosowaPozycjaNaTerenie(out pozycja)) continue;

            GameObject wrogObj;
            if (prefabWroga != null)
            {
                wrogObj = Instantiate(prefabWroga, pozycja, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            }
            else
            {
                wrogObj = StworzWrogęDomyślnego(pozycja);
            }

            Wrog wrog = wrogObj.GetComponent<Wrog>();
            if (wrog == null) wrog = wrogObj.AddComponent<Wrog>();
            if (wrog.maksymalneHp <= 0) wrog.maksymalneHp = 100;
            wrog.aktualneHp = wrog.maksymalneHp;

            postawiono++;
        }

        Debug.Log($"SpawnSystem: postawiono {postawiono}/{liczbaWrogow} wrogow.");
    }

    GameObject StworzWrogęDomyślnego(Vector3 pozycja)
    {
        GameObject wrog = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        wrog.transform.position = pozycja + Vector3.up * 1f;
        wrog.transform.localScale = new Vector3(1f, 1f, 1f);

        Renderer rend = wrog.GetComponent<Renderer>();
        if (rend != null) rend.material.color = Color.red;

        Collider col = wrog.GetComponent<Collider>();
        if (col != null) col.isTrigger = false;

        return wrog;
    }

    bool LosowaPozycjaNaTerenie(out Vector3 wynik)
    {
        wynik = Vector3.zero;
        int proby = 0;
        int maxProb = 100;

        while (proby < maxProb)
        {
            proby++;

            float x = Random.Range(-obszarX / 2f, obszarX / 2f);
            float z = Random.Range(-obszarZ / 2f, obszarZ / 2f);

            float wys = 0f;
            if (teren != null)
            {
                wys = teren.PobierzWysokoscTerenu(new Vector3(x, 0f, z));
            }

            if (wys < minimalnaWysokoscDlaSpawnu) continue;

            Vector3 pozycja = new Vector3(x, wys + 0.5f, z);

            if (gracz != null && Vector3.Distance(pozycja, gracz.position) < minimalnaOdleglOdGracza) continue;

            wynik = pozycja;
            return true;
        }

        Debug.LogWarning($"SpawnSystem: nie udalo sie znalezc pozycji po {maxProb} probach.");
        return false;
    }
}
