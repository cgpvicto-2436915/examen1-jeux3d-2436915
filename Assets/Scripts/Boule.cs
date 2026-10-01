using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;



/// <summary>
/// Objet représentant une boule contrôlée par le joueur.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Boule : MonoBehaviour
{
    [SerializeField, Tooltip("La cible pour le suvi de la caméra")]
    private Transform cibleCamera;

    [SerializeField]
    private Indicateurs indicateur;


    [SerializeField, Tooltip("Référence au texte affichant le nombre de charge.")]
    TextMeshProUGUI texteNombreCharge;

    [SerializeField, Tooltip("Force de déplacement de la boule.")]
    private float forceDeplacement;

    // Force appliquée à la boule pour le déplacement à chaque frame.
    private Vector3 forceAppliquee;

    // Référence au Rigidbody de la boule pour appliquer la physique.
    private Rigidbody rigidbody;

    /// <summary>
    /// Obtient la vélocité actuelle de la boule.
    /// </summary>
    public Vector3 Velocite => rigidbody.linearVelocity;

    [SerializeField, Tooltip("Nombre de charge max")]
    private int NombreChargeMax;

    private int chargeAcceleration;

    private bool enAcceleration;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        chargeAcceleration = 0;
        enAcceleration = false;
    }

    private void Start()
    {


        ControleurJeu.Instance.Controles.actions.FindAction("Commencer").performed += Lancer;
    }

    // lance la balle lorsque espace est presse
    private void Lancer(InputAction.CallbackContext contexte)
    {
        if (ControleurJeu.Instance == null)
            return;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null)
            return;
        
        controles.actions.FindAction("Diriger").performed += CommencerDirection;
        controles.actions.FindAction("Diriger").canceled += ArreterDirection;
        ControleurJeu.Instance.Controles.actions.FindAction("Acceleration").performed += Accelerer;
        ControleurJeu.Instance.Controles.actions.FindAction("Commencer").performed -= Lancer;
        rigidbody.useGravity = true;
    }


    private void OnDestroy()
    {
        if (ControleurJeu.Instance == null)
            return;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null) 
            return;

        controles.actions.FindAction("Diriger").performed -= CommencerDirection;
        controles.actions.FindAction("Diriger").canceled -= ArreterDirection;
        ControleurJeu.Instance.Controles.actions.FindAction("Acceleration").performed -= Accelerer;
        ControleurJeu.Instance.Controles.actions.FindAction("Commencer").performed -= Lancer;
    }

    private void Update()
    {
        if (cibleCamera != null)
        {
            cibleCamera.position = rigidbody.position;
        }
        
        
    }

    private void FixedUpdate()
    {
        Diriger();
        if (enAcceleration)
        {
            rigidbody.AddForce(Vector3.forward * 15f, ForceMode.Force);
        }
    }

    //met la balle en mode acceleration et lance un delai de 1 secondes.
    private void Accelerer(InputAction.CallbackContext contexte)
    {
        indicateur.AfficherCharge(chargeAcceleration);
        if (!enAcceleration)
        {
            if (chargeAcceleration > 0)
            {
                enAcceleration = true;
                chargeAcceleration--;
                indicateur.AfficherCharge(chargeAcceleration);
                StartCoroutine(DelaiAcceleration());
            }
        }

    }

    /// <summary>
    /// la coroutine attend 1 second puis arrete l'acceleration
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelaiAcceleration()
    {

        yield return new WaitForSeconds(1);
        
        enAcceleration = false;
        
        
    }

    private void CommencerDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee += contexte.ReadValue<float>() * forceDeplacement * Vector3.right;
    }

    private void ArreterDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee = Vector3.zero;
    }

    private void Diriger()
    {
        if(!Mathf.Approximately(forceAppliquee.sqrMagnitude, 0.0f))
        {
            rigidbody.AddForce(forceAppliquee, ForceMode.Force);
        }
    }

    //Ajoute une charge quand il en touche une et affiche le nombre de charge.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Charge"))
        {
            if(chargeAcceleration < NombreChargeMax)
                    {
                        Destroy(other.gameObject);
                        chargeAcceleration++;
                        indicateur.AfficherCharge(chargeAcceleration);
                    }
        }
        
    }
}
