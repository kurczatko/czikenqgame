using UnityEngine;

public class ZdrowieGracza : MonoBehaviour
{
    public int maksymalneHp = 100;
    public int aktualneHp = 100;

    public System.Action<int, int> NaZmianeHp;

    void Start()
    {
        aktualneHp = maksymalneHp;
        NaZmianeHp?.Invoke(aktualneHp, maksymalneHp);
    }

    public void ZabierzHp(int obrazenia)
    {
        aktualneHp = Mathf.Max(0, aktualneHp - obrazenia);
        NaZmianeHp?.Invoke(aktualneHp, maksymalneHp);

        if (aktualneHp <= 0)
        {
            Umrzyj();
        }
    }

    public void Leczenie(int hp)
    {
        aktualneHp = Mathf.Min(maksymalneHp, aktualneHp + hp);
        NaZmianeHp?.Invoke(aktualneHp, maksymalneHp);
    }

    void Umrzyj()
    {
        Debug.Log("Gracz umarl.");
        gameObject.SetActive(false);
    }
}
