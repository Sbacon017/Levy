using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class IntroController : MonoBehaviour {

    public Texture[] IntroImgs;
    public RawImage ConvText;

    public AudioClip Eddie_AD;
    public GameObject Eddie_Front;
    public GameObject Eddie_Back;

    public AudioClip[] IntroAud;
    public AudioSource AD_Container;
    int index = 0;
    bool canAdvance = true;
    public AudioSource deathSound1;
    public AudioSource deathSound2;
    public AudioSource music;
    bool skip = false;
    bool onAD = false;

    public GameObject NewsPaper;

	// Use this for initialization
	void Start () {
        Debug.Log("Start called in IntroController");
        NewsPaper.SetActive(true);
        StartCoroutine(initIntro());
    }
	
    IEnumerator initIntro()
    {
        canAdvance = false;
        Eddie_Back.SetActive(false);
        Eddie_Front.SetActive(false);
        while (!Input.GetButtonDown("Open") && !Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        NewsPaper.SetActive(false);
        AD_Container.clip = Eddie_AD;
        AD_Container.Play();
        yield return new WaitForSeconds(2f);
        onAD = true;
        Eddie_Front.SetActive(true);
        StartCoroutine(wait());
        while (!skip)
        {
            yield return null;
        }
        Eddie_Back.SetActive(true);
        Eddie_Front.SetActive(false);
        ConvText.texture = IntroImgs[0];
        AD_Container.Stop();
        canAdvance = true;
        AD_Container.clip = IntroAud[0];
        AD_Container.Play();
    }

    IEnumerator wait()
    {
        
        yield return new WaitForSeconds(11f);
        skip = true;
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetButtonDown("Open") && canAdvance)
        {
            index++;
            if (index < 7)
            {
                StartCoroutine(SwapThrough(index));
            }else
            {
                StartCoroutine(eddieDeath());
            }
        }else if(Input.GetButtonDown("Open") && onAD)
        {
            skip = true;
        }
	}

    IEnumerator SwapThrough(int index)
    {
        AD_Container.Stop();
        canAdvance = false;
        ConvText.texture = IntroImgs[index];
        AD_Container.clip = IntroAud[index];
        AD_Container.Play();
        yield return new WaitForSeconds(0.01f);
        canAdvance = true;
    }

    IEnumerator eddieDeath()
    {
        AD_Container.Stop();
        ConvText.texture = IntroImgs[7];
        music.Stop();
        yield return new WaitForSeconds(0.5f);
        deathSound1.Play();
        yield return new WaitForSeconds(1.25f);
        deathSound2.Play();
        SceneManager.LoadScene("LevyGame", LoadSceneMode.Single);
    }
}
