using UnityEngine;
using UnityEngine.InputSystem;

public class RuchGracza : MonoBehaviour
{
    public float predkosc = 5f;
    public float predkoscObrotu = 10f;
    private Rigidbody rb;
    private Vector3 kierunekRuchu;

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
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) ruchX -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) ruchX += 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) ruchZ += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) ruchZ -= 1f;
        }

        kierunekRuchu = new Vector3(ruchX, 0f, ruchZ).normalized;
    }

    void FixedUpdate()
    {
        if (kierunekRuchu.sqrMagnitude > 0.001f)
        {
            Vector3 nowaPozycja = rb.position + kierunekRuchu * predkosc * Time.fixedDeltaTime;
            rb.MovePosition(nowaPozycja);

            Quaternion celowaRotacja = Quaternion.LookRotation(kierunekRuchu);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, celowaRotacja, predkoscObrotu * Time.fixedDeltaTime));
        }
    }
}
