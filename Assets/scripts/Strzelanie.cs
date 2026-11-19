using UnityEngine;
using UnityEngine.InputSystem;

public class Strzelanie : MonoBehaviour
{
    public Camera kameraGracza;
    public Transform wylotPocisku;
    public float zasieg = 100f;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            OddajStrzal();
        }
    }

    void OddajStrzal()
    {
        Bron aktywna = Ekwipunek.Instancja != null ? Ekwipunek.Instancja.GetAktywnaBron() : null;
        if (aktywna == null)
        {
            Debug.Log("Brak bronii w ekwipunku.");
            return;
        }

        Vector3 poczatek = wylotPocisku != null ? wylotPocisku.position : kameraGracza.transform.position;
        Vector3 kierunek = kameraGracza != null ? kameraGracza.transform.forward : transform.forward;

        Ray ray = new Ray(poczatek, kierunek);
        if (Physics.Raycast(ray, out RaycastHit trafienie, zasieg))
        {
            Debug.DrawLine(poczatek, trafienie.point, Color.red, 1f);

            Wrog wrog = trafienie.collider.GetComponent<Wrog>();
            if (wrog != null)
            {
                wrog.PrzyjmijObrazenia(aktywna.statystyki.obrazenia);
            }
        }
        else
        {
            Debug.DrawLine(poczatek, poczatek + kierunek * zasieg, Color.green, 1f);
        }

        aktywna.Strzelaj(poczatek, kierunek);
    }
}
