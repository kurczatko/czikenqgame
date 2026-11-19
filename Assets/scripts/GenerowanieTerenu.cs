using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class GenerowanieTerenu : MonoBehaviour
{
    [Header("Ustawienia chunka")]
    public int rozmiarChunka = 64;
    public int szerokoscWChunktach = 5;
    public int dlugoscWChunktach = 5;
    public float skala = 0.05f;
    public float wysokosc = 5f;
    public float wysokoscBazowa = 0f;

    [Header("Szum Perlin")]
    public int oktawy = 3;
    public float lacznosc = 0.5f;
    public float zesilenie = 2f;
    public int nasiono = 12345;
    public Vector2 offset;

    [Header("Kolory")]
    public Color kolorTrawa = new Color(0.2f, 0.6f, 0.2f);
    public Color kolorKamien = new Color(0.5f, 0.5f, 0.5f);
    public Color kolorSnieg = new Color(0.9f, 0.9f, 0.9f);
    public float wysokoscTrawa = 5f;
    public float wysokoscKamien = 15f;

    [Header("Materiał")]
    public Material materialTerenu;

    [Header("Drzewa")]
    public GameObject prefabDrzewa;
    public int liczbaDrzewNaChunk = 40;
    public float minWysokoscDrzewa = 3f;
    public float maxNachylenieDrzewa = 35f;
    public float odlegloscMiedzyDrzewami = 8f;
    public float skalaDrzewaMin = 0.8f;
    public float skalaDrzewaMax = 1.4f;
    public bool tworzDrzewaFallback = true;

    [Header("Nieskończony teren")]
    public Transform gracz;
    public float odlegloscLadowaniaChunka = 120f;
    public float odlegloscWyladowaniaChunka = 160f;
    public int buforChunktow = 1;

    private Dictionary<Vector2Int, ChunkDane> aktywneChunki = new Dictionary<Vector2Int, ChunkDane>();
    private List<Vector2Int> chunkiDoWyladowania = new List<Vector2Int>();

    void Start()
    {
        if (gracz == null)
        {
            GameObject g = GameObject.FindWithTag("Player");
            if (g != null) gracz = g.transform;
        }
        if (gracz == null)
        {
            gracz = Camera.main?.transform;
        }

        InicjalizujNasiono();
        GenerujChunkiWokolGracza();
    }

    void InicjalizujNasiono()
    {
        Random.InitState(nasiono);
        offset = new Vector2(Random.Range(-100000f, 100000f), Random.Range(-100000f, 100000f));
    }

    void Update()
    {
        if (gracz == null) return;

        GenerujChunkiWokolGracza();
        WyladujDalekieChunki();
    }

    void GenerujChunkiWokolGracza()
    {
        Vector2Int centroChunka = PozycjaNaChunk(gracz.position);

        for (int z = -buforChunktow; z <= buforChunktow; z++)
        {
            for (int x = -buforChunktow; x <= buforChunktow; x++)
            {
                Vector2Int koordynat = new Vector2Int(centroChunka.x + x, centroChunka.y + z);
                if (!aktywneChunki.ContainsKey(koordynat))
                {
                    StworzChunk(koordynat);
                }
            }
        }
    }

    void WyladujDalekieChunki()
    {
        chunkiDoWyladowania.Clear();
        foreach (var para in aktywneChunki)
        {
            float odleglosc = Vector3.Distance(gracz.position, para.Value.srodekSwiata);
            if (odleglosc > odlegloscWyladowaniaChunka)
            {
                chunkiDoWyladowania.Add(para.Key);
            }
        }

        foreach (var koordynat in chunkiDoWyladowania)
        {
            UsunChunk(koordynat);
        }
    }

    Vector2Int PozycjaNaChunk(Vector3 pozycja)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pozycja.x / rozmiarChunka),
            Mathf.FloorToInt(pozycja.z / rozmiarChunka)
        );
    }

    void StworzChunk(Vector2Int koordynat)
    {
        GameObject obiektChunka = new GameObject($"Chunk_{koordynat.x}_{koordynat.y}");
        obiektChunka.transform.SetParent(transform);
        obiektChunka.transform.position = Vector3.zero;

        ChunkDane dane = obiektChunka.AddComponent<ChunkDane>();
        dane.koordynat = koordynat;
        dane.rozmiarChunka = rozmiarChunka;
        dane.skala = skala;
        dane.wysokosc = wysokosc;
        dane.oktawy = oktawy;
        dane.lacznosc = lacznosc;
        dane.zesilenie = zesilenie;
        dane.offset = offset;
        dane.kolorTrawa = kolorTrawa;
        dane.kolorKamien = kolorKamien;
        dane.kolorSnieg = kolorSnieg;
        dane.wysokoscTrawa = wysokoscTrawa;
        dane.wysokoscKamien = wysokoscKamien;
        dane.materialTerenu = materialTerenu;
        dane.prefabDrzewa = prefabDrzewa;
        dane.liczbaDrzewNaChunk = liczbaDrzewNaChunk;
        dane.minWysokoscDrzewa = minWysokoscDrzewa;
        dane.maxNachylenieDrzewa = maxNachylenieDrzewa;
        dane.odlegloscMiedzyDrzewami = odlegloscMiedzyDrzewami;
        dane.skalaDrzewaMin = skalaDrzewaMin;
        dane.skalaDrzewaMax = skalaDrzewaMax;
        dane.tworzDrzewaFallback = tworzDrzewaFallback;
        dane.nasiono = nasiono + koordynat.x * 1000 + koordynat.y;

        dane.Generuj();

        aktywneChunki[koordynat] = dane;
    }

    void UsunChunk(Vector2Int koordynat)
    {
        if (aktywneChunki.TryGetValue(koordynat, out ChunkDane dane))
        {
            if (dane != null && dane.gameObject != null)
            {
                Destroy(dane.gameObject);
            }
            aktywneChunki.Remove(koordynat);
        }
    }

    public float PobierzWysokoscTerenu(Vector3 pozycjaSwiata)
    {
        Vector2Int koordynat = PozycjaNaChunk(pozycjaSwiata);
        if (!aktywneChunki.TryGetValue(koordynat, out ChunkDane dane))
        {
            return ObliczWysokoscSwiatowa(pozycjaSwiata.x, pozycjaSwiata.z);
        }
        return dane.PobierzWysokosc(pozycjaSwiata.x, pozycjaSwiata.z);
    }

    float ObliczWysokoscSwiatowa(float x, float z)
    {
        Random.InitState(nasiono);
        Vector2 off = offset;
        float amplituda = 1f;
        float czestotliwosc = skala;
        float wynik = 0f;

        for (int i = 0; i < oktawy; i++)
        {
            float wartosc = Mathf.PerlinNoise(x * czestotliwosc + off.x, z * czestotliwosc + off.y) * 2f - 1f;
            wynik += wartosc * amplituda;
            amplituda *= lacznosc;
            czestotliwosc *= zesilenie;
        }

        return Mathf.Max(0f, wynik) * wysokosc + wysokoscBazowa;
    }
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ChunkDane : MonoBehaviour
{
    public Vector2Int koordynat;
    public int rozmiarChunka = 64;
    public float skala = 0.02f;
    public float wysokosc = 8f;
    public float wysokoscBazowa = 0f;
    public int oktawy = 4;
    public float lacznosc = 0.5f;
    public float zesilenie = 2f;
    public Vector2 offset;
    public Color kolorTrawa = new Color(0.2f, 0.6f, 0.2f);
    public Color kolorKamien = new Color(0.5f, 0.5f, 0.5f);
    public Color kolorSnieg = new Color(0.9f, 0.9f, 0.9f);
    public float wysokoscTrawa = 5f;
    public float wysokoscKamien = 15f;
    public Material materialTerenu;
    public GameObject prefabDrzewa;
    public int liczbaDrzewNaChunk = 40;
    public float minWysokoscDrzewa = 3f;
    public float maxNachylenieDrzewa = 35f;
    public float odlegloscMiedzyDrzewami = 8f;
    public float skalaDrzewaMin = 0.8f;
    public float skalaDrzewaMax = 1.4f;
    public bool tworzDrzewaFallback = true;
    public int nasiono = 12345;

    public Vector3 srodekSwiata;

    private Mesh mesh;
    private MeshRenderer renderer;
    private MeshCollider kolizjer;
    private GameObject kontenerDrzew;

    private Vector3[] wierzcholki;
    private int[] trojkaty;
    private Color[] kolory;
    private Vector2[] uv;
    private float[,] mapaWysokosci;

    public void Generuj()
    {
        renderer = GetComponent<MeshRenderer>();
        kolizjer = GetComponent<MeshCollider>();
        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        srodekSwiata = new Vector3(koordynat.x * rozmiarChunka, 0f, koordynat.y * rozmiarChunka);

        InicjalizujTablice();
        ObliczWysokosci();
        ZbudujMesh();
        PrzypiszMaterial();
        UmiescDrzewa();
    }

    void InicjalizujTablice()
    {
        int rozmiar = (rozmiarChunka + 1) * (rozmiarChunka + 1);
        wierzcholki = new Vector3[rozmiar];
        kolory = new Color[rozmiar];
        uv = new Vector2[rozmiar];
        trojkaty = new int[rozmiarChunka * rozmiarChunka * 6];
        mapaWysokosci = new float[rozmiarChunka + 1, rozmiarChunka + 1];
    }

    void ObliczWysokosci()
    {
        Random.InitState(nasiono);

        int xStart = koordynat.x * rozmiarChunka;
        int zStart = koordynat.y * rozmiarChunka;

        for (int z = 0; z <= rozmiarChunka; z++)
        {
            for (int x = 0; x <= rozmiarChunka; x++)
            {
                float wys = ObliczWysokosc(xStart + x, zStart + z);
                mapaWysokosci[x, z] = wys;
            }
        }
    }

    float ObliczWysokosc(int xSwiat, int zSwiat)
    {
        float amplituda = 1f;
        float czestotliwosc = skala;
        float wynik = 0f;

        for (int i = 0; i < oktawy; i++)
        {
            float wartosc = Mathf.PerlinNoise(xSwiat * czestotliwosc + offset.x, zSwiat * czestotliwosc + offset.y) * 2f - 1f;
            wynik += wartosc * amplituda;
            amplituda *= lacznosc;
            czestotliwosc *= zesilenie;
        }

        return Mathf.Max(0f, wynik) * wysokosc + wysokoscBazowa;
    }

    void ZbudujMesh()
    {
        int indeksWierzcholka = 0;
        int xStart = koordynat.x * rozmiarChunka;
        int zStart = koordynat.y * rozmiarChunka;

        for (int z = 0; z <= rozmiarChunka; z++)
        {
            for (int x = 0; x <= rozmiarChunka; x++)
            {
                float wys = mapaWysokosci[x, z];
                wierzcholki[indeksWierzcholka] = new Vector3(xStart + x, wys, zStart + z);
                uv[indeksWierzcholka] = new Vector2((float)x / rozmiarChunka, (float)z / rozmiarChunka);
                kolory[indeksWierzcholka] = KolorDlaWysokosci(wys);
                indeksWierzcholka++;
            }
        }

        int indeksTrojkata = 0;
        for (int z = 0; z < rozmiarChunka; z++)
        {
            for (int x = 0; x < rozmiarChunka; x++)
            {
                int lewaGora = x + z * (rozmiarChunka + 1);
                int prawaGora = lewaGora + 1;
                int lewaDol = lewaGora + (rozmiarChunka + 1);
                int prawaDol = lewaDol + 1;

                trojkaty[indeksTrojkata++] = lewaGora;
                trojkaty[indeksTrojkata++] = lewaDol;
                trojkaty[indeksTrojkata++] = prawaDol;

                trojkaty[indeksTrojkata++] = lewaGora;
                trojkaty[indeksTrojkata++] = prawaDol;
                trojkaty[indeksTrojkata++] = prawaGora;
            }
        }

        mesh.Clear();
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.vertices = wierzcholki;
        mesh.uv = uv;
        mesh.colors = kolory;
        mesh.triangles = trojkaty;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        kolizjer.sharedMesh = null;
        kolizjer.sharedMesh = mesh;
    }

    Color KolorDlaWysokosci(float wys)
    {
        if (wys > wysokoscKamien) return kolorSnieg;
        if (wys > wysokoscTrawa) return Color.Lerp(kolorTrawa, kolorKamien, (wys - wysokoscTrawa) / (wysokoscKamien - wysokoscTrawa));
        return kolorTrawa;
    }

    void PrzypiszMaterial()
    {
        if (materialTerenu != null)
        {
            renderer.sharedMaterial = materialTerenu;
            return;
        }

        Material domyslny = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (domyslny == null)
        {
            domyslny = new Material(Shader.Find("Standard"));
        }
        domyslny.color = kolorTrawa;
        renderer.sharedMaterial = domyslny;
        materialTerenu = domyslny;
    }

    void UmiescDrzewa()
    {
        if (kontenerDrzew != null) Destroy(kontenerDrzew);
        kontenerDrzew = new GameObject("Drzewa");
        kontenerDrzew.transform.SetParent(transform);

        int xStart = koordynat.x * rozmiarChunka;
        int zStart = koordynat.y * rozmiarChunka;

        Random.InitState(nasiono);

        int postawiono = 0;
        int proby = 0;
        int maxProb = liczbaDrzewNaChunk * 20;
        List<Vector3> pozycje = new List<Vector3>();

        while (postawiono < liczbaDrzewNaChunk && proby < maxProb)
        {
            proby++;

            int x = Random.Range(2, rozmiarChunka - 2);
            int z = Random.Range(2, rozmiarChunka - 2);

            float wys = mapaWysokosci[x, z];
            if (wys < minWysokoscDrzewa) continue;

            Vector3 normal = ObliczNormal(x, z);
            float nachylenie = Vector3.Angle(Vector3.up, normal);
            if (nachylenie > maxNachylenieDrzewa) continue;

            Vector3 pozycja = new Vector3(
                xStart + x + Random.Range(-0.4f, 0.4f),
                wys,
                zStart + z + Random.Range(-0.4f, 0.4f)
            );

            if (JestZblizona(pozycje, pozycja)) continue;

            pozycje.Add(pozycja);
            postawiono++;

            GameObject drzewo;
            if (prefabDrzewa != null)
            {
                drzewo = Instantiate(prefabDrzewa, pozycja, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), kontenerDrzew.transform);
                float skalaLosowa = Random.Range(skalaDrzewaMin, skalaDrzewaMax);
                drzewo.transform.localScale = Vector3.one * skalaLosowa;
            }
            else if (tworzDrzewaFallback)
            {
                drzewo = StworzDrzewoFallback(pozycja, kontenerDrzew.transform);
            }
            else
            {
                continue;
            }

            Drzewo d = drzewo.GetComponent<Drzewo>();
            if (d == null) d = drzewo.AddComponent<Drzewo>();
            d.pozycjaStartowa = pozycja;
            d.wysokoscTerenu = wys;
        }
    }

    GameObject StworzDrzewoFallback(Vector3 pozycja, Transform rodzic)
    {
        GameObject drzewo = new GameObject("Drzewo");
        drzewo.transform.SetParent(rodzic);
        drzewo.transform.position = pozycja + Vector3.up * 0.5f;

        GameObject pień = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pień.transform.SetParent(drzewo.transform, false);
        pień.transform.localPosition = Vector3.zero;
        pień.transform.localScale = new Vector3(0.35f, 1f, 0.35f);
        pień.GetComponent<Collider>().enabled = false;

        GameObject korona = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        korona.transform.SetParent(drzewo.transform, false);
        korona.transform.localPosition = new Vector3(0f, 1.4f, 0f);
        korona.transform.localScale = new Vector3(0.9f, 1.1f, 0.9f);
        korona.GetComponent<Collider>().enabled = false;

        Material matLiscie = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = Color.green };
        Material matPien = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = new Color(0.4f, 0.25f, 0.1f) };

        Renderer rPien = pień.GetComponent<Renderer>();
        Renderer rKorona = korona.GetComponent<Renderer>();
        if (rPien != null) rPien.sharedMaterial = matPien;
        if (rKorona != null) rKorona.sharedMaterial = matLiscie;

        return drzewo;
    }

    Vector3 ObliczNormal(int x, int z)
    {
        float wysLewa = mapaWysokosci[Mathf.Clamp(x - 1, 0, rozmiarChunka), z];
        float wysPrawa = mapaWysokosci[Mathf.Clamp(x + 1, 0, rozmiarChunka), z];
        float wysGora = mapaWysokosci[x, Mathf.Clamp(z - 1, 0, rozmiarChunka)];
        float wysDol = mapaWysokosci[x, Mathf.Clamp(z + 1, 0, rozmiarChunka)];

        Vector3 touchX = new Vector3(1f, wysPrawa - wysLewa, 0f);
        Vector3 touchZ = new Vector3(0f, wysDol - wysGora, 1f);
        return Vector3.Cross(touchX, touchZ).normalized;
    }

    bool JestZblizona(List<Vector3> pozycje, Vector3 kandydat)
    {
        float minOdlegloscKwadrat = odlegloscMiedzyDrzewami * odlegloscMiedzyDrzewami;
        foreach (Vector3 p in pozycje)
        {
            if ((p - kandydat).sqrMagnitude < minOdlegloscKwadrat) return true;
        }
        return false;
    }

    public float PobierzWysokosc(float xSwiat, float zSwiat)
    {
        int xLokalny = Mathf.RoundToInt(xSwiat - koordynat.x * rozmiarChunka);
        int zLokalny = Mathf.RoundToInt(zSwiat - koordynat.y * rozmiarChunka);

        if (xLokalny < 0 || xLokalny > rozmiarChunka || zLokalny < 0 || zLokalny > rozmiarChunka)
        {
            return ObliczWysokosc((int)xSwiat, (int)zSwiat);
        }

        return mapaWysokosci[xLokalny, zLokalny];
    }
}

public class Drzewo : MonoBehaviour
{
    public Vector3 pozycjaStartowa;
    public float wysokoscTerenu;
}
