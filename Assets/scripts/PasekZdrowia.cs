using UnityEngine;
using UnityEngine.UI;

public class PasekZdrowia : MonoBehaviour
{
    [Header("Referencje")]
    public ZdrowieGracza zdrowieGracza;

    private Image pasekWypelnienie;
    private Canvas canvas;

    void Awake()
    {
        StworzInterfejs();
    }

    void StworzInterfejs()
    {
        GameObject canvasObj = new GameObject("CanvasZdrowie");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject kontener = new GameObject("KontenerPaska");
        kontener.transform.SetParent(canvas.transform, false);

        RectTransform rectKontener = kontener.AddComponent<RectTransform>();
        rectKontener.anchorMin = new Vector2(0.02f, 0.92f);
        rectKontener.anchorMax = new Vector2(0.32f, 0.98f);
        rectKontener.offsetMin = Vector2.zero;
        rectKontener.offsetMax = Vector2.zero;

        Image tlo = kontener.AddComponent<Image>();
        tlo.color = new Color(0f, 0f, 0f, 0.5f);

        GameObject pasekObj = new GameObject("PasekZdrowia");
        pasekObj.transform.SetParent(kontener.transform, false);

        RectTransform rectPasek = pasekObj.AddComponent<RectTransform>();
        rectPasek.anchorMin = Vector2.zero;
        rectPasek.anchorMax = Vector2.one;
        rectPasek.offsetMin = Vector2.zero;
        rectPasek.offsetMax = Vector2.zero;

        pasekWypelnienie = pasekObj.AddComponent<Image>();
        pasekWypelnienie.color = Color.red;

        GameObject tekstObj = new GameObject("TekstHP");
        tekstObj.transform.SetParent(kontener.transform, false);
        RectTransform rectTekst = tekstObj.AddComponent<RectTransform>();
        rectTekst.anchorMin = Vector2.zero;
        rectTekst.anchorMax = Vector2.one;
        rectTekst.offsetMin = Vector2.zero;
        rectTekst.offsetMax = Vector2.zero;

        Text hpTekst = tekstObj.AddComponent<Text>();
        hpTekst.text = "100/100";
        hpTekst.alignment = TextAnchor.MiddleCenter;
        hpTekst.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hpTekst.fontSize = 14;
        hpTekst.color = Color.white;
        hpTekst.verticalOverflow = VerticalWrapMode.Overflow;
        hpTekst.horizontalOverflow = HorizontalWrapMode.Overflow;

        if (zdrowieGracza != null)
        {
            zdrowieGracza.NaZmianeHp += AktualizujPasek;
            AktualizujPasek(zdrowieGracza.aktualneHp, zdrowieGracza.maksymalneHp);
        }
    }

    void AktualizujPasek(int aktualne, int maksymalne)
    {
        if (pasekWypelnienie != null)
        {
            pasekWypelnienie.fillAmount = (float)aktualne / maksymalne;
        }

        Text tekst = GetComponentInChildren<Text>();
        if (tekst != null)
        {
            tekst.text = $"{aktualne}/{maksymalne}";
        }
    }

    void OnDestroy()
    {
        if (zdrowieGracza != null)
        {
            zdrowieGracza.NaZmianeHp -= AktualizujPasek;
        }
    }
}
