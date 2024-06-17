using UnityEngine;
using UnityEngine.UI;

enum Comportamento { Ocioso, Procurar, Fugir }
enum Estado { Ocioso, Chegar, Procurar, Fugir }

[RequireComponent(typeof(Rigidbody2D))]
public class SteeringActor : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] Comportamento acao = Comportamento.Procurar;
    [SerializeField] Transform objetivo = null;
    [SerializeField] float velocidadeLimite = 5f;
    [SerializeField, Range(0.1f, 0.99f)] float fatorReducao = 0.75f;
    [SerializeField] float raioChegada = 1.2f;
    [SerializeField] float raioParada = 0.5f;
    [SerializeField] float raioFuga = 5f;
    [SerializeField] float distanciaDesvio = 2f;
    [SerializeField] float distanciaCircundar = 2f;
    [SerializeField] float anguloEtapaCircundar = 10f;
    [SerializeField] LayerMask camadaObstaculo;

    Text exibicaoAcao = null;
    Rigidbody2D corpoFisico;
    Estado situacao = Estado.Ocioso;

    void Update()
    {
        if (objetivo != null)
        {
            switch (acao)
            {
                case Comportamento.Ocioso: AcaoOciosa(); break;
                case Comportamento.Procurar: AcaoProcurar(); break;
                case Comportamento.Fugir: AcaoFugir(); break;
            }
        }

        corpoFisico.velocity = Vector2.ClampMagnitude(corpoFisico.velocity, velocidadeLimite);

        if (exibicaoAcao != null)
        {
            exibicaoAcao.text = situacao.ToString().ToUpper();
        }
    }

    void AcaoOciosa()
    {
        corpoFisico.velocity *= fatorReducao;
    }

    void AcaoProcurar()
    {
        Vector2 delta = objetivo.position - transform.position;
        Vector2 direcao = delta.normalized * velocidadeLimite - corpoFisico.velocity;
        float distancia = delta.magnitude;

        if (distancia < raioParada)
        {
            situacao = Estado.Ocioso;
        }
        else if (distancia < raioChegada)
        {
            situacao = Estado.Chegar;
        }
        else
        {
            situacao = Estado.Procurar;
        }

        switch (situacao)
        {
            case Estado.Ocioso:
                AcaoOciosa();
                break;
            case Estado.Chegar:
                float fatorChegada = 0.01f + (distancia - raioParada) / (raioChegada - raioParada);
                corpoFisico.velocity += fatorChegada * direcao * Time.fixedDeltaTime;
                break;
            case Estado.Procurar:
                corpoFisico.velocity += direcao * Time.fixedDeltaTime;
                break;
        }

        CircundarObstaculos();
    }

    void AcaoFugir()
    {
        Vector2 delta = objetivo.position - transform.position;
        Vector2 direcao = delta.normalized * velocidadeLimite - corpoFisico.velocity;
        float distancia = delta.magnitude;

        if (distancia > raioFuga)
        {
            situacao = Estado.Ocioso;
        }
        else
        {
            situacao = Estado.Fugir;
        }

        switch (situacao)
        {
            case Estado.Ocioso:
                AcaoOciosa();
                break;
            case Estado.Fugir:
                corpoFisico.velocity -= direcao * Time.fixedDeltaTime;
                break;
        }

        CircundarObstaculos();
    }

    void CircundarObstaculos()
    {
        Vector2 direcaoMovimento = corpoFisico.velocity.normalized;

        Vector2 contornoDireita = Quaternion.AngleAxis(-anguloEtapaCircundar, Vector3.forward) * direcaoMovimento;
        Vector2 contornoEsquerda = Quaternion.AngleAxis(anguloEtapaCircundar, Vector3.forward) * direcaoMovimento;

        RaycastHit2D hitDireita = Physics2D.Raycast(transform.position, contornoDireita, distanciaCircundar, camadaObstaculo);
        RaycastHit2D hitEsquerda = Physics2D.Raycast(transform.position, contornoEsquerda, distanciaCircundar, camadaObstaculo);

        if (hitDireita.collider != null && hitEsquerda.collider != null)
        {
            Vector2 direcaoFuga = (hitDireita.point - hitEsquerda.point).normalized;
            corpoFisico.velocity = direcaoFuga * velocidadeLimite;
        }
        else if (hitDireita.collider != null)
        {
            corpoFisico.velocity = contornoEsquerda.normalized * velocidadeLimite;
        }
        else if (hitEsquerda.collider != null)
        {
            corpoFisico.velocity = contornoDireita.normalized * velocidadeLimite;
        }
    }

    void Awake()
    {
        corpoFisico = GetComponent<Rigidbody2D>();
        corpoFisico.isKinematic = false;
        exibicaoAcao = GetComponentInChildren<Text>();
    }

    void OnDrawGizmos()
    {
        if (objetivo == null)
        {
            return;
        }

        switch (acao)
        {
            case Comportamento.Ocioso:
                break;
            case Comportamento.Procurar:
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, raioChegada);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, raioParada);
                break;
            case Comportamento.Fugir:
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, raioFuga);
                break;
        }

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, objetivo.position);
    }
}
