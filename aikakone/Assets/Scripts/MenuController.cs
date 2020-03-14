using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System.IO;
using UnityEngine.SceneManagement;
using static audioManager;

public class MenuController : MonoBehaviour {

    public static MenuController instance;

    //Active option and bool to check if main menu is active
    private int option = 0;
    private bool mainMenu = true;

    //Check to move the menu using the keys or only the arrows
    [SerializeField, Tooltip("Check to move the menu using the keys or only the arrows")]
    public bool useKeys = true;

    //Check if using parallax or not
    public bool useParallax = true;

    //Scenes animation
    private bool isAnimating = false;
    private int activeScene = 1;
    [SerializeField, Tooltip("Animation speed in seconds")]
    public float animSpeed;

    //Option quantity
    [SerializeField, Tooltip ("Introduce all the options in your menu")]
    public string[] options;

    //Backgrounds
    [SerializeField, Tooltip("Introduce all the backgrounds for the scenes in your menu")]
    public GameObject[] backgrounds;
    [SerializeField, Tooltip("Introduce all the backgrounds for the scenes in your menu")]
    public GameObject[] backgroundsParallax;
    [SerializeField, Tooltip("Introduce the main bck for your menu")]
    public GameObject mainBackgroundParallax;
    [SerializeField, Tooltip("Introduce the main bck for your menu")]
    public GameObject mainBackground;
    [SerializeField, HideInInspector]
    public Text menuText;
    [SerializeField]
    public GameObject[] activeBackground;

    //Arrow Animators
    [SerializeField, HideInInspector]
    public Animator ArrowR;
    [SerializeField, HideInInspector]
    public Animator ArrowL;

    //Menu bar gameobject
    [SerializeField, HideInInspector]
    public GameObject menuBar;

    //Backgrounds Controller
    [SerializeField, HideInInspector]
    public GameObject backgroundsController;

    //Sounds
    [Header("Sounds")]
    [Space(10)]
    public AudioClip Select;
    public AudioClip SceneSelect;

    //Events
    [SerializeField, HideInInspector]
    public UnityEvent[] Events;

    //Exit Menu
    [SerializeField, HideInInspector]
    public GameObject exitMenu;

    //Options menu
    [SerializeField, HideInInspector]
    public GameObject OptionsMenu;


    void Start()
    {
        audioManager.loadSettings();

        instance = this;
        //Set the activeBackground array length
        if (useParallax) { activeBackground = new GameObject[backgroundsParallax.Length]; } else { activeBackground = new GameObject[backgrounds.Length]; }
        initiate();      
    }

	void Update () {

        if (mainMenu) { 
        //Changes the text corresponding option
        menuText.text = options[option];

        //Deactivate arrows
        //If the option is less than 1 left arrow deactivated
        if(option < 1)
        {
            ArrowL.SetBool("Deactivate", true);
        }else
        {
            ArrowL.SetBool("Deactivate", false);
        }

        //If the option is the last option deactivate right arrow
        if (option == options.Length-1)
        {
            ArrowR.SetBool("Deactivate", true);
        }
        else
        {
            ArrowR.SetBool("Deactivate", false);
        }

            //If use keys is active move with the keys pressed
            if (useKeys)
            {
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    moveRight();
                }

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    moveLeft();
                }

                //If enter is pressed reproduce the corresponding event
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    pressEnter();
                }
            }
        }

        //Check is the scenes are animating and puts the variable in true or false
        var anim = backgroundsController.GetComponent<Animation>();
        if (anim.isPlaying)
        {
            isAnimating = true;
        }else
        {
            isAnimating = false;
        }


    }

    //Initiate
    private void initiate()
    {
        //If use parallax is active then instantiate the parallax main bck
        //Else instantiate the normal background
        mainMenu = true;
        menuBar.SetActive(true);
        if (useParallax)
        {
            //Instantiate the background an set the parent to this gameobject
            //Then reset the scale and position
            //Set the sibling to first so the background is visible
            //Then adjust the rect values
            //And lastly set the active background array position 0 to this background
            var Bck = Instantiate(mainBackgroundParallax) as GameObject;
            Bck.transform.SetParent(this.gameObject.transform);
            Bck.transform.localScale = new Vector3(1, 1, 1);
            Bck.transform.localPosition = new Vector3(0, 0, 0);
            Bck.transform.SetSiblingIndex(0);
            var rect = Bck.GetComponent<RectTransform>();
            rect.offsetMax = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            activeBackground[0] = Bck;
        }
        else
        {
            //Instantiate the background an set the parent to this gameobject
            //Then reset the scale and position
            //Set the sibling to first so the background is visible
            //Then adjust the rect values
            //And lastly set the active background array position 0 to this background
            var Bck = Instantiate(mainBackground) as GameObject;
            Bck.transform.SetParent(this.gameObject.transform);
            var rect = Bck.GetComponent<RectTransform>();
            Bck.transform.SetSiblingIndex(0);
            rect.transform.localScale = new Vector3(1, 1, 1);
            rect.transform.localPosition = new Vector3(0, 0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            activeBackground[0] = Bck;
        }
    }

    //Press enter or click on option 
    public void pressEnter()
    {
        Events[option].Invoke();
    }

    //Function to go foward in the menu
    public void moveRight()
    {
        if(option < options.Length-1)
        {
            option = option + 1;
            ArrowR.SetBool("Click", true);
            audioManager.playClipOnObject(Select, gameObject);
        }
    }

    //Function to go back in the menu
    public void moveLeft()
    {
        if (option > 0)
        {
            option = option - 1;
            ArrowL.SetBool("Click", true);
            audioManager.playClipOnObject(Select, gameObject);
        }
    }
    
    //New Game event
    public void newGame()
    {
        //Loads the first scene, change the number to your desired scene
        SceneManager.LoadScene(1);
    }

    //Continue
    public void continueGame()
    {
        //In this part you need to include your save game script to implement the continue function
    }

    //Select scene Event
    public void selectScene()
    {
        Destroy(activeBackground[0]);
        //Instantiate all the backgrounds for the scenes
        //If using the parallax option the parallax backgrounds are spawned
        if (useParallax)
        {
            for (int i = backgroundsParallax.Length-1; i > -1; i--)
            {
                var bck = Instantiate(backgroundsParallax[i]) as GameObject;
                var rect = bck.GetComponent<RectTransform>();
                bck.transform.SetParent(backgroundsController.transform);
                bck.transform.localScale = Vector3.one;
                bck.transform.localPosition = Vector3.zero;
                bck.transform.SetSiblingIndex(0);
                var thisRect = gameObject.GetComponent<RectTransform>();
                rect.offsetMax = new Vector2((thisRect.rect.width * i), 0);
                rect.offsetMin = new Vector2(thisRect.rect.width * i, 0);
                activeBackground[i] = bck;
                menuBar.SetActive(false);
                mainMenu = false;
            }

        //If not, we spawn the normal backgrounds
        }else
        {
            for (int i = backgrounds.Length - 1; i > -1; i--)
            {
                var bck = Instantiate(backgrounds[i]) as GameObject;
                var rect = bck.GetComponent<RectTransform>();
                bck.transform.SetParent(backgroundsController.transform);
                bck.transform.localScale = Vector3.one;
                bck.transform.localPosition = Vector3.zero;
                bck.transform.SetSiblingIndex(0);
                var thisRect = gameObject.GetComponent<RectTransform>();
                rect.offsetMax = new Vector2((thisRect.rect.width * i), 0);
                rect.offsetMin = new Vector2(thisRect.rect.width * i , 0);
                activeBackground[i] = bck;
                menuBar.SetActive(false);
                mainMenu = false;
            }
        }
    }

    //Advances throught the Scenes
    public void advanceScene()
    {
        //First check if we are animating and if we are not in the last scene
        if (!isAnimating && activeScene < activeBackground.Length)
        {
            audioManager.playClipOnObject(SceneSelect, gameObject);
            //Then create a new clip and a curve to animate the scenes moving
            var clip = new AnimationClip();
            var curve = new AnimationCurve();
            //Get the anim from the backgroundController to put the clip
            var anim = backgroundsController.GetComponent<Animation>();
            //If the clip already exist we remove it
            if (anim.GetClip("f") != null) { anim.RemoveClip("f"); }
            clip.legacy = true;
            //Now we check the distance between 2 scenes to move then
            float distance = Vector3.Distance(activeBackground[0].transform.localPosition, activeBackground[1].transform.localPosition);
            //Set the curve with the data
            curve = AnimationCurve.Linear(0, (backgroundsController.transform.localPosition.x), animSpeed, (distance * -1)*activeScene);
            Debug.Log(distance * activeScene);
            clip.SetCurve("", typeof(Transform), "localPosition.x", curve);
            //And play the animation
            anim.AddClip(clip, "f");
            anim.Play("f");
            //We also keep the count of the active scene in this variable
            activeScene++;
            //Now we put the active scene in the first sibling index to activate the parallax effect
            activeBackground[activeScene - 1].transform.SetAsFirstSibling();

        }
    }

    //Advances throught the Scenes
    public void goBackScene()
    {

        //First check if we are animating and if we are not in the first scene
        if (!isAnimating && activeScene > 1)
        {
            activeScene--;
            //Then create a new clip and a curve to animate the scenes moving
            var clip = new AnimationClip();
            var curve = new AnimationCurve();
            //Get the anim from the backgroundController to put the clip
            var anim = backgroundsController.GetComponent<Animation>();
            //If the clip already exist we remove it
            if (anim.GetClip("b") != null) { anim.RemoveClip("b"); }
            clip.legacy = true;
            //Now we check the distance between 2 scenes to move then
            float distance = Vector3.Distance(activeBackground[0].transform.localPosition, activeBackground[1].transform.localPosition);
            //Set the curve with the data
            curve = AnimationCurve.Linear(0, (backgroundsController.transform.localPosition.x), animSpeed, distance*(activeScene-1)*-1);
            Debug.Log(distance*(activeScene - 1));
            clip.SetCurve("", typeof(Transform), "localPosition.x", curve);
            //And play the animation
            anim.AddClip(clip, "b");
            anim.Play("b");
            //We also keep the count of the active scene in this variable
            //Now we put the active scene in the first sibling index to activate the parallax effect
            activeBackground[activeScene ].transform.SetAsLastSibling();
        }
    }

    //Closes the scenes menu
    public void closeScenes()
    {
        //Destroy all the active backgrounds and restart the menu
        for (int i = 0; i < activeBackground.Length; i++)
        {
            Destroy(activeBackground[i]);
        }
        initiate();
    }

    //Opens the exit menu
    public void exitMenuOpen()
    {
        var animEx = exitMenu.GetComponent<Animation>();
        exitMenu.transform.SetAsLastSibling();
        animEx.Play("Fade In");
        mainMenu = false;
    }

    //Closes the exit menu
    public void exitMenuClose()
    {
        var animEx = exitMenu.GetComponent<Animation>();
        animEx.Play("Fade out");
        mainMenu = true;
    }

    //Exit Game
    public void exitGame()
    {
        Application.Quit();
    }

    //Open Options
    public void openOptions()
    {
        OptionsMenu.gameObject.GetComponent<Animation>().Play("Fade In");
        mainMenu = false;
        OptionsMenu.transform.SetAsLastSibling();
    }

    //Close Options
    public void closeOptions()
    {
        OptionsMenu.gameObject.GetComponent<Animation>().Play("Fade out");
        mainMenu = true;
    }

}

