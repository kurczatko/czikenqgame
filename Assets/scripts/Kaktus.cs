using UnityEngine;

public class Kaktus : MonoBehaviour
{
    public int obrazenia = 10;
    public float cooldown = 1f;

    private float czasOstatnichObrazen;

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time - czasOstatnichObrazen < cooldown) return;

        ZdrowieGracza zdrowie = other.GetComponent<ZdrowieGracza>();
        if (zdrowie != null)
        {
            zdrowie.ZabierzHp(obrazenia);
            Debug.Log($"Kaktus: zabrano {obrazenia} HP graczowi");
        }

        czasOstatnichObrazen = Time.time;
    }
}
