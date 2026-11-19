using UnityEngine;

public class BronPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Bron bron = GetComponent<Bron>();
        if (bron != null && Ekwipunek.Instancja != null)
        {
            Ekwipunek.Instancja.DodajBron(bron);
        }

        Destroy(gameObject);
    }
}
