using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {

    public Slider Slider;
    public GameObject controlsImagen;
    public GameObject Atras;
    public GameObject nGame;
    public GameObject controls;

    public void MouseEnter()
    {
        if (GetComponent<Button>().IsInteractable())
        {
            if (GetComponentInChildren<Text>().fontSize == 52)
            {
                GetComponentInChildren<Text>().fontSize = 58;
            }
            else
            {
                GetComponentInChildren<Text>().fontSize = 34;
            }
        }
    }
    public void MouseExit()
    {
        if (GetComponent<Button>().IsInteractable())
        {
            if (GetComponentInChildren<Text>().fontSize == 58)
            {
                GetComponentInChildren<Text>().fontSize = 52;
            }
            else
            {
                GetComponentInChildren<Text>().fontSize = 28;
            }
        }
    }
    public void MouseUp()
    {
        if (GetComponent<Button>().IsInteractable())
        {
            if (gameObject.name == "Ngame")
            {
                StartCoroutine(Load(1));
            }
            if (gameObject.name == "Controls")
            {
                controlsImagen.SetActive(true);
                controls.GetComponent<Button>().interactable = false;
                nGame.GetComponent<Button>().interactable = false;
                Atras.SetActive(true);
            }
            if (gameObject.name == "Atras")
            {
                Atras.SetActive(false);
                controls.GetComponent<Button>().interactable = true;
                nGame.GetComponent<Button>().interactable = true;
                controlsImagen.SetActive(false);
            }
            if (gameObject.name == "EXITB")
            {
                SceneManager.LoadScene(0);
            }
            if(gameObject.name == "exit")
            {
                Application.Quit();
            }
        }
    }
    IEnumerator Load(int scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;

        Slider.gameObject.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Slider.GetComponentInChildren<Text>().text = progress + "  %";

            Slider.value = progress;
            // Loading completed
            if (operation.progress == 0.9f)
            {
                Slider.GetComponentInChildren<Text>().text = "Press  a  key  to  start";
                if (Input.anyKey)
                    operation.allowSceneActivation = true;
            }

            yield return null;
        }

    }
    public void ExitButton()
    {

    }

}
