using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    bool canAbsorb = true;
    bool isAbsorbing = false;
    bool absorbEmplacementIsEmpty = true;
    bool inputAbsorbIsRelease = true;

    void Start()
    {
        if(Input.GetAxisRaw("absorb") > 0)
        {
            Debug.Log("bouton absorb enfoncé");
            inputAbsorbIsRelease = false;
        }
    }

    void Update()
    {

        //TEMP EN ATTENDANT DE TROUVER LE KEYDOWN AVEC INPUT MANAGER !!!!!!!!!!!!
        //On verifie que le joueur a bien relaché le bouton une fois avant de retenter l'absorption
        if (inputAbsorbIsRelease == false && Input.GetAxisRaw("absorb") == 0)
        {
            Debug.Log("bouton absorb relaché");
            inputAbsorbIsRelease = true;
        }
        //

        // FAIRE UN KEYDOWN POUR SE PASSER DU BOOLEAN !!!!!!!!!!!!!!!!!!!
        //
        if(Input.GetAxisRaw("absorb") > 0 && absorbEmplacementIsEmpty && canAbsorb && inputAbsorbIsRelease)
        {
            Debug.Log("bouton absorb enfoncé");
            inputAbsorbIsRelease = false;       
            
            // PEUT ETRE DES DELAI PLUTOT QUE COROUTINE !!!!!!!!!!
            StartCoroutine(delaiAbsorbtion());
            StartCoroutine(delaiProchaineAbsorbtion());
        }        
    }

    IEnumerator delaiAbsorbtion()
    {
        Debug.Log("Début absorbtion");
        isAbsorbing = true;
        yield return new WaitForSeconds(1f);
        Debug.Log("fin absorbtion");
        isAbsorbing = false;
    }

    IEnumerator delaiProchaineAbsorbtion()
    {
        canAbsorb = false;       
        yield return new WaitForSeconds(2f);
        Debug.Log("PROCHAIN OK");
        canAbsorb = true;
    }

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        //TEMP
        // On verifie que l'objet avec lequel on entre en collision est absorbable
        // Faudra faire une methode de verification differente d'un tag name
        // Par exemple aller verifier une liste qui contient tout les ID des objets absorbables,
        // et la comparer à l'id de l'objet de la collision
        if(_collision.gameObject.tag == "absorbable" && isAbsorbing)
        {
            Destroy(_collision.gameObject);
        }
    }
}
