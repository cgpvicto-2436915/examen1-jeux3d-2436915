using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Indicateurs : MonoBehaviour
{
    [SerializeField, Tooltip("Prefab de l'image charge")]
    private GameObject PrefabCharge;

    private List<GameObject> instancesCharge;
    private void Start()
    {
        instancesCharge = new();
    }

    public void AfficherCharge(int nombre)
    {
        foreach (Transform enfant in transform)
        {
            RecyclerCharge(enfant.gameObject);
        }
        for (int i = 0; i < nombre; i++) {
            GameObject charge = GenererCharge();
        }

    }

    //Pool de charge (inspirer du projet ABC)
    private GameObject GenererCharge()
    {
        if (instancesCharge.Count > 0)
        {
            GameObject instance = instancesCharge[0];
            instance.gameObject.SetActive(true);
            instancesCharge.RemoveAt(0);

            return instance;
        }

        // On crée un nouvel ennemi si aucun n'est disponible dans la liste
        return Instantiate(PrefabCharge, transform);
    }
    
    //Recycle les charge dans une liste (inspirer du projet ABC) 
    public void RecyclerCharge(GameObject charge)
    {
        instancesCharge.Add(charge);
        charge.gameObject.SetActive(false);
    }

}
