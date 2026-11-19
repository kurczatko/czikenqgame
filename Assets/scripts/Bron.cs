using UnityEngine;

[System.Serializable]
public class StatystykiBroni
{
    public string nazwa = "Bron";
    public int obrazenia = 10;
    public float zasięg = 60f;
    public float tempoStrzału = 0.5f;
    public GameObject modelPrefab;
    public Color kolorPodswietlenia = Color.yellow;
}

public class Bron : MonoBehaviour
{
    public StatystykiBroni statystyki = new StatystykiBroni();
    public bool wyposazona = false;

    public virtual void Strzelaj(Vector3 poczatek, Vector3 kierunek)
    {
        Debug.Log($"Strzelanie z {statystyki.nazwa} (obrazenia: {statystyki.obrazenia})");
    }

    public virtual void Aktywuj()
    {
        wyposazona = true;
        gameObject.SetActive(false);
    }

    public virtual void Dezaktywuj()
    {
        wyposazona = false;
        gameObject.SetActive(false);
    }
}
