using UnityEngine;
using System.Collections.Generic;

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

        Debug.Log($"SpawnSystem: teren={(teren != null ? "znaleziony" : "BRAK")}, gracz={(gracz != null ? "znaleziony" : "BRAK")}");

        SpawnujBronie();
        SpawnujWrogow();
    }

    void SpawnujBronie()
    {
        if (prefabyBronii == null || prefabyBronii.Length == 0)
        {
            Debug.LogWarning("SpawnSystem: nie przypisano prefabow bronii!");
            return;
        }

        int postawiono = 0;
        for (int i = 0; i < liczbaBronii; i++)
        {
            Vector3 pozycja = LosowaPozycjaNaTerenie();
            if (pozycja == Vector3.negativeInfinity) continue;

            int indeks = Random.Range(0, prefabyBronii.Length);
            GameObject bronObj = Instantiate(prefabyBronii[indeks], pozycja, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            Bron bron = bronObj.GetComponent<Bron>();
            if (bron == null) bron = bronObj.AddComponent<Bron>();

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

    void SpawnujWrogow()
    {
        if (prefabWroga == null)
        {
            Debug.LogWarning("SpawnSystem: nie przypisano prefabu wroga!");
            return;
        }

        int postawiono = 0;
        for (int i = 0; i < liczbaWrogow; i++)
        {
            Vector3 pozycja = LosowaPozycjaNaTerenie();
            if (pozycja == Vector3.negativeInfinity) continue;

            Instantiate(prefabWroga, pozycja, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            postawiono++;
        }

        Debug.Log($"SpawnSystem: postawiono {postawiono}/{liczbaWrogow} wrogow.");
    }

    Vector3 LosowaPozycjaNaTerenie()
    {
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

            return pozycja;
        }

        Debug.LogWarning($"SpawnSystem: nie udalo sie znalezc pozycji po {maxProb} probach.");
        return Vector3.negativeInfinity;
    }
}
