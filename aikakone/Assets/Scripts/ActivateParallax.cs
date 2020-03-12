using UnityEngine;
using System.Collections;

public class ActivateParallax : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
	 if(gameObject.transform.GetSiblingIndex() == 0)
        {
          var parallax = gameObject.transform.GetComponentsInChildren<Parallax>();
            for (int i = 0; i < parallax.Length; i++)
            {
                parallax[i].isActive = true;
            }
        }else
        {
            var parallax = gameObject.transform.GetComponentsInChildren<Parallax>();
            for (int i = 0; i < parallax.Length; i++)
            {
                parallax[i].isActive = false;
            }
        }
	}
}
