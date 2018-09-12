using UnityEngine;
using System.Collections;
using Assets.UltimateIsometricToolkit.Scripts.physics;
using Assets.UltimateIsometricToolkit.Scripts.Pathfinding;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Assets.UltimateIsometricToolkit.Scripts.Utils;
using UnityEngine.UI;

/// <summary>
/// Converts mouse input to 3d coordinates and invokes A* pathfinding
/// </summary>
public class Char_Movement_Controller : MonoBehaviour
{
    public AstarAgent AstarAgent;
    public SpriteRenderer Char_Elev;
    public IsoTransform _isoTransform;
    public Elev_DoorController Elev;
    public GameManager GManager;
    public Vector3 HomeBase;
    public Text DingText;
    public GameObject[] DingLights;
    public AudioSource dingSound;
    public ConversationEngine ConvDecider;
    public int ar_id;
    public RawImage D_Text;
    public Texture Thanks;
    public Texture No_Thanks;
    public Texture[] toFloorTexts;
    public AudioClip[] toFloorAud;
    public AudioSource toFloorContainer;
    public Sprite[] Char_Sprites;
    public SpriteRenderer Char_Floor;
    public AudioSource deathSound;
    public AudioSource A_Thanks;
    public AudioSource A_No_Thanks;

    private IsoTransform Elev_Iso;
    public bool onElev = false;
    private float y;
    private int toFloor;
    public bool ding = false;
    private float[] AStarLevels = { -99.7f, -75.02859f, -50.8f, -25.8f, -2.4f};
    public bool isAlive = true;
    private SpriteRenderer spriteRender = null;
    public bool did_script = false;
    public bool script_char = false;
    private bool canEnter = true;


    void Start()
    {
        Elev_Iso = (IsoTransform)Elev.GetComponent("IsoTransform");
        spriteRender = (SpriteRenderer)AstarAgent.GetComponent("SpriteRenderer");
        D_Text.color = new Color(255, 255, 255, 0);
        y = this._isoTransform.Position.y;
        StartCoroutine(moveRandom(y));
    }

    // Update is called once per frame
    void Update()
    {
        //Get in the elevator
        if (Input.GetButtonDown("Open") && !onElev && !Elev.getOpen() && ding && canEnter)
        {
            y = AStarLevels[Elev.getFloor() - 1];
            if (Mathf.Abs(_isoTransform.Position.y - Elev_Iso.Position.y) < 2)
            {
                StopAllCoroutines();
                StartCoroutine(moveIn(y));
            }
        }else if (Input.GetButtonDown("Open") && onElev && !Elev.getOpen())
        {
            y = AStarLevels[Elev.getFloor() - 1];
            StopAllCoroutines();
            StartCoroutine(moveOut(y));
            StartCoroutine(moveRandom(y));
        }
    }

    IEnumerator moveRandom(float y)
    {
        while(true){
            if (!ding)
            {
                Vector3 dest = new Vector3(Random.Range(1, 8), y, Random.Range(1, 7)); //x and z were 1,12 (Steph)
                setSprite(dest);
                AstarAgent.MoveTo(dest);
            }
            yield return new WaitForSeconds(Random.Range(5, 20));
        }
    }

    IEnumerator moveIn(float y)
    {
        yield return new WaitForSeconds(.001f);
        Char_Elev.sprite = spriteRender.sprite;
        spriteRender.sprite = null;
        onElev = true;
        DingLights[getFloor() - 1].SetActive(false);
        D_Text.color = new Color(255, 255, 255, 255);
        D_Text.texture = (Texture)toFloorTexts[toFloor-1];
        toFloorContainer.clip = toFloorAud[toFloor - 1];
        toFloorContainer.Play();
    }

    IEnumerator moveToLevy(float y, int dest)
    {
        Vector3 adjElevPos = new Vector3(Elev_Iso.Position.x, 0, Elev_Iso.Position.z);
        Vector3 adjActorPos = new Vector3(this._isoTransform.Position.x, 0, this._isoTransform.Position.z);
        float dist = Vector3.Magnitude(adjElevPos - adjActorPos);
        float time = dist / AstarAgent.Speed + 0.2f;
        Vector3 dest1 = new Vector3(6f, this._isoTransform.Position.y, 1f);
        setSprite(dest1);
        AstarAgent.MoveTo(dest1);
        yield return new WaitForSeconds(time);
        ding = true;
        DingLights[getFloor() - 1].SetActive(true);
        dingSound.Play();
        toFloor = dest;
    }

    IEnumerator moveOut(float y)
    {
        AstarAgent.transform.position = Isometric.IsoToUnitySpace(new Vector3(5f, Elev_Iso.Position.y, 1f));
        _isoTransform.Position = Isometric.UnityToIsoSpace(AstarAgent.transform.position);
        SpriteRenderer spriteRender = (SpriteRenderer)AstarAgent.GetComponent("SpriteRenderer");
        spriteRender.sprite = Char_Elev.sprite;
        Char_Elev.sprite = null;
        onElev = false;
        canEnter = false;
        if(Mathf.Abs(_isoTransform.Position.y - AStarLevels[toFloor - 1]) < 10)
        {
            Debug.Log("Thanks!");
            D_Text.color = new Color(255, 255, 255, 255);
            D_Text.texture = (Texture)Thanks;
            A_Thanks.Play();
            if (script_char)
                did_script = true;
            GManager.addSat();
            yield return new WaitForSeconds(2f);
            DingText.text = "";
            D_Text.texture = null;
            D_Text.color = new Color(255, 255, 255, 0);
        }
        else
        {
            Debug.Log("Wait, what?");
            D_Text.color = new Color(255, 255, 255, 255);
            D_Text.texture = (Texture)No_Thanks;
            A_No_Thanks.Play();
            GManager.subSat();
            yield return new WaitForSeconds(2f);
            DingText.text = "";
            D_Text.texture = null;
            D_Text.color = new Color(255,255,255,0);
        }
        ding = false;
        canEnter = true;
        GManager.Char_Done();
        yield return new WaitForSeconds(0.3f);
        int floor = getFloor();
        Vector3 convTarget = ConvDecider.conversationTree(ar_id, floor);
        if (convTarget.x > 0)
        {
            Vector3 dest = convTarget + new Vector3(1, 0, 0);
            setSprite(dest);
            AstarAgent.MoveTo(dest);
            Debug.Log("Conversation");
        }else
        {
            Vector3 dest = new Vector3(Random.Range(1, 12), y, Random.Range(1, 12));
            setSprite(dest);
            AstarAgent.MoveTo(dest);
        }
    }

    public void Ding(int dest)
    {
        StopAllCoroutines();
        StartCoroutine(moveToLevy(y, dest));

    }

    public int getFloor()
    {
        int floor = 0;
        for (int i = 0; i < 5; i++) {
            if (Mathf.Abs(_isoTransform.Position.y - AStarLevels[i]) < 10)
            {
                floor = i + 1;
            }
        }
        return floor;
    }

    public bool getIsAlive()
    {
        return isAlive;
    }

    public void kill()
    {
        deathSound.Play();
        isAlive = false;
    }

    public void stopMoving()
    {
        StopAllCoroutines();
    }

    private void setSprite(Vector3 dest)
    {
        Vector3 initPos = this._isoTransform.Position;
        Vector3 dir = dest - initPos;

        if(dir.z < 0)
        {
            if(dir.x < 0)
            {
                Char_Floor.sprite = Char_Sprites[0];
            }else
            {
                Char_Floor.sprite = Char_Sprites[1];
            }
        }else
        {
            if (dir.x < 0)
            {
                Char_Floor.sprite = Char_Sprites[2];
            }
            else
            {
                Char_Floor.sprite = Char_Sprites[3];
            }
        }
    }
}