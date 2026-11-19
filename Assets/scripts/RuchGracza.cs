using UnityEngine;
using UnityEngine.InputSystem;

public class RuchGracza : MonoBehaviour
{
    public float predkosc = 5f;
    public float predkoscObrotu = 100f;
    private Rigidbody rb;
    private Vector3 kierunekRuchu;
    private float obrotGracza;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        float ruchX = 0f;
        float ruchZ = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) ruchX -= 1f;
            if (Keyboard.current.dKey.isPressed) ruchX += 1f;
            if (Keyboard.current.wKey.isPressed) ruchZ += 1f;
            if (Keyboard.current.sKey.isPressed) ruchZ -= 1f;

            if (Keyboard.current.leftArrowKey.isPressed) obrotGracza -= predkoscObrotu * Time.deltaTime;
            if (Keyboard.current.rightArrowKey.isPressed) obrotGracza += predkoscObrotu * Time.deltaTime;
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
