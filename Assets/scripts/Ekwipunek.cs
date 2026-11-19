using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Ekwipunek : MonoBehaviour
{
    public static Ekwipunek Instancja { get; private set; }

    [Header("UI Ekwipunku")]
    public GameObject panelEkwipunku;
    public Transform listaPrzedmiotow;
    public GameObject przyciskBronPrefab;

    private List<Bron> bronie = new List<Bron>();
    private Bron aktywnaBron;

    void Awake()
    {
        if (Instancja != null && Instancja != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancja = this;
    }

    public void DodajBron(Bron bron)
    {
        if (bronie.Contains(bron)) return;
        bronie.Add(bron);
        bron.Aktywuj();

        if (aktywnaBron == null)
        {
            WyposadzBron(bron);
        }

        OdswiezUI();
        Debug.Log($"Ekwipunek: dodano bron {bron.statystyki.nazwa}");
    }

    public void WyposadzBron(Bron bron)
    {
        if (aktywnaBron != null)
        {
            aktywnaBron.Dezaktywuj();
        }

        aktywnaBron = bron;
        aktywnaBron.wyposazona = true;
        OdswiezUI();
        Debug.Log($"Ekwipunek: uzbrojono w {bron.statystyki.nazwa}");
    }

    public Bron GetAktywnaBron()
    {
        return aktywnaBron;
    }

    public List<Bron> GetBronie()
    {
        return bronie;
    }

    void OdswiezUI()
    {
        if (panelEkwipunku == null || listaPrzedmiotow == null || przyciskBronPrefab == null) return;

        foreach (Transform dziecko in listaPrzedmiotow)
        {
            Destroy(dziecko.gameObject);
        }

        foreach (Bron bron in bronie)
        {
            GameObject przycisk = Instantiate(przyciskBronPrefab, listaPrzedmiotow);
            Button btn = przycisk.GetComponent<Button>();
            Text tekst = przycisk.GetComponentInChildren<Text>();

            if (tekst != null)
            {
                tekst.text = bron.statystyki.nazwa + (bron == aktywnaBron ? " (AKTYWNA)" : "");
            }

            if (btn != null)
            {
                Bron lokalnaBron = bron;
                btn.onClick.AddListener(() => WyposadzBron(lokalnaBron));
            }
        }
    }

    public void PokazUkryjEkwipunek()
    {
        if (panelEkwipunku != null)
        {
            bool aktywne = !panelEkwipunku.activeSelf;
            panelEkwipunku.SetActive(aktywne);
        }
    }
}
