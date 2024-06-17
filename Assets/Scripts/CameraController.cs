using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] float taxaSuavizacao = 1f;

    Transform alvoJogador = null;
    Camera cameraPrincipal = null;

    void LateUpdate()
    {
        if (alvoJogador == null)
            return;

        Vector3 posicaoCentral = alvoJogador.position;
        posicaoCentral = new Vector3
        (
            x: posicaoCentral.x,
            y: posicaoCentral.y,
            z: -10f
        );

        transform.position = Vector3.Lerp(transform.position, posicaoCentral, taxaSuavizacao);
    }

    void Start()
    {
        var jogador = GameObject.FindObjectOfType<PlayerController>();
        if (jogador != null)
        {
            alvoJogador = jogador.transform;
        }
    }

    void Awake()
    {
        cameraPrincipal = GetComponent<Camera>();
    }
}
