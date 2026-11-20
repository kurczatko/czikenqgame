using UnityEngine;
using UnityEngine.InputSystem;

public class RuchGracza : MonoBehaviour
{
    public float predkosc = 5f;
    public float predkoscObrotu = 100f;
    public float predkoscObrotuDotyk = 150f;

    private Rigidbody rb;
    private Vector3 kierunekRuchu;
    private float obrotGracza;

    private Vector2 poczatekDotyku;
    private bool dotykAktywny;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        float ruchX = 0f;
        float ruchZ = 0f;

        // --- Sterowanie klawiaturą (PC) ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) ruchX -= 1f;
            if (Keyboard.current.dKey.isPressed) ruchX += 1f;
            if (Keyboard.current.wKey.isPressed) ruchZ += 1f;
            if (Keyboard.current.sKey.isPressed) ruchZ -= 1f;

            if (Keyboard.current.leftArrowKey.isPressed) obrotGracza -= predkoscObrotu * Time.deltaTime;
            if (Keyboard.current.rightArrowKey.isPressed) obrotGracza += predkoscObrotu * Time.deltaTime;
        }

        // --- Sterowanie dotykowe (Mobile) ---
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch != null)
            {
                bool jestWcisniety = touch.press.ReadValue() > 0f;
                Vector2 pozycja = touch.position.ReadValue();
                float ekranSrodek = Screen.width * 0.5f;

                if (jestWcisniety)
                {
                    if (!dotykAktywny)
                    {
                        poczatekDotyku = pozycja;
                        dotykAktywny = true;
                    }

                    Vector2 roznica = pozycja - poczatekDotyku;

                    if (pozycja.x < ekranSrodek)
                    {
                        // Lewa strona ekranu - ruch
                        ruchX = roznica.x / Screen.width * 2f;
                        ruchZ = roznica.y / Screen.height * 2f;
                    }
                    else
                    {
                        // Prawa strona ekranu - obrot
                        obrotGracza -= roznica.x * predkoscObrotuDotyk * Time.deltaTime;
                        poczatekDotyku = pozycja;
                    }
                }
                else
                {
                    dotykAktywny = false;
                }
            }
        }

        kierunekRuchu = new Vector3(ruchX, 0f, ruchZ).normalized;
    }

    void FixedUpdate()
    {
        if (kierunekRuchu.sqrMagnitude > 0.001f)
        {
            Vector3 nowaPozycja = rb.position + kierunekRuchu * predkosc * Time.fixedDeltaTime;
            rb.MovePosition(nowaPozycja);
        }

        rb.MoveRotation(Quaternion.Euler(0f, obrotGracza, 0f));
    }
}
