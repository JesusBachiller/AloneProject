using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroAnim : MonoBehaviour {

    float count = 0;
    public bool ismenu = false;
    
	// Update is called once per frame
	void FixedUpdate () {
        if (ismenu)
        {
            if (count < 4)
            {
                if(count > 1)
                {
                    GetComponentInChildren<Text>().color = new Color(GetComponentInChildren<Text>().color.r, GetComponentInChildren<Text>().color.g, GetComponentInChildren<Text>().color.b, GetComponentInChildren<Text>().color.a + 0.05f);
                }
                count += 0.02f;
            }
            else
            {
                Color c = GetComponent<Image>().color;
                if (c.a >= 0)
                {
                    GetComponent<Image>().color = new Color(0, 0, 0, c.a - 0.0075f);
                    GetComponentInChildren<Text>().color = new Color(GetComponentInChildren<Text>().color.r, GetComponentInChildren<Text>().color.g, GetComponentInChildren<Text>().color.b, c.a - 0.0075f);
                }
                else
                {
                    Destroy(transform.parent.gameObject);
                }
            }
        }
        else
        {
            if (count < 1)
            {
                count += 0.02f;
            }
            else
            {
                Color c = GetComponent<Image>().color;
                if (c.a >= 0)
                {
                    GetComponent<Image>().color = new Color(0, 0, 0, c.a - 0.0075f);
                }
                else
                {
                    Destroy(transform.parent.gameObject);
                }
            }
        }
	}
}
