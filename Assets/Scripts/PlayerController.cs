using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] float velocidadeMovimento = 10f;

    Rigidbody2D rb2D = null;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 movimentoEntrada = new Vector3
        (
            x: Input.GetAxis("Horizontal"),
            y: Input.GetAxis("Vertical"),
            z: 0.0f
        );

        if (movimentoEntrada != Vector3.zero)
        {
            rb2D.velocity = movimentoEntrada.normalized * velocidadeMovimento;
        }
        else
        {
            rb2D.velocity = Vector3.zero;
        }
    }
}
