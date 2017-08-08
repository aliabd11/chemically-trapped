using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
		public float turnDelay = 0.1f;							//Delay between each Player turn.
		public int playerFoodPoints = 300;						//Starting value for Player food points.
        public int playerNumOxygen = 0;
        public int playerNumHydrogen = 0;
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
		

		private Text levelText;									//Text to display current level number.
		private Text bossText;
		private Text energyText;
		private Text experimentText;
		public Text itemPickupText;

		public AudioClip newLevelSound;                //Audio clip to play when player dies.
		public GameObject bossSource;

		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
		private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
		public int level = 1;									//Current level number, expressed in game as "Day 1".
		private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
		private List<BossOneEnemy> bossenemies;					//List of all Boss Enemy units, used to issue them move commands.

		private bool enemiesMoving;								//Boolean to check if enemies are moving.
		private bool bossenemiesMoving;								//Boolean to check if enemies are moving.

		private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
        private bool gameOver;
		public bool explainText = false;
        [HideInInspector] public bool restart;

        public Text restartText;
        private bool firstRun = true;

        //Awake is always called before any Start functions
        void Awake()
		{
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);	
			
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			
			//Assign enemies to a new List of Enemy objects.
			enemies = new List<Enemy>();
			bossenemies = new List<BossOneEnemy> ();

			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();
			
			//Call the InitGame function to initialize the first level 
			InitGame();
		}

        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance.level++;
            instance.InitGame();
        }

		
		//Initializes the game for each level.
		void InitGame()
		{				

			if (instance.level == 2) {
				bossText = GameObject.Find ("BossBattleText").GetComponent<Text> ();
				bossText.text = "PERFORM A DIRECT COMBINATION/SYNTHESIS REACTION TO GET PAST THIS LEVEL.";
			
			} else if (instance.level == 3) {
				bossText = GameObject.Find ("BossBattleText").GetComponent<Text> ();
				bossText.text = "PERFORM A CHEMICAL DECOMPOSITION REACTION. BREAK DOWN A COMPOUND INTO SMALLER CHEMICAL SPECIES.";

			} else if (instance.level == 4) {
				bossText = GameObject.Find ("BossBattleText").GetComponent<Text> ();
				bossText.text = "PERFORM A SINGLE DISPLACEMENT REACTION.";

			} else if (instance.level == 5) {
				bossText = GameObject.Find ("BossBattleText").GetComponent<Text> ();
				bossText.text = "PERFORM A DOUBLE DISPLACEMENT REACTION.";

			} else if (instance.level == 6) {
				bossText = GameObject.Find ("BossBattleText").GetComponent<Text> ();
				bossText.text = "PERFORM A COMBUSTION REACTION TO ESCAPE!";
			} else if (instance.level == 7) {
				bossText = GameObject.Find ("BossBattleText").GetComponent<Text> ();
				bossText.text = "PERFORM NUCLEAR FISSION TO ESCAPE!";
			} 

			/*else if (instance.level == 9) {
				//with enough energy points, the user now has the ability to split an atom
				//http://www.physics4kids.com/files/mod_fission.html
				bossText = GameObject.Find ("BossBattleText").GetComponent<Text> ();
				bossText.text = "TRY TO ESCAPE:\n\n TIME IS TICKING!" +
					"(HINT: Split the atom.)";

				//experimentText = GameObject.Find ("ExperimentDetails").GetComponent<Text> ();
				itemPickupText = GameObject.Find("ItemPickup").GetComponent<Text>();
				itemPickupText.text = @"**********" + "\n\nNuclear power reactors use a reaction called nuclear fission ('splitting')" +
				" The process of splitting a nucleus is called nuclear fission. \n\n Uranium or plutonium isotopes are normally used as the fuel in nuclear reactors, because their atoms have relatively large nuclei that are easy to split."; 

				SoundManager.instance.musicSource.Pause (); // Play Boss Music
				GameObject soundObject = GameObject.Find ("boss_music");
				AudioSource audioSource = soundObject.GetComponent<AudioSource> ();
				audioSource.Play ();
			}*/

			if (instance.level < 1) {
				bossText = GameObject.Find ("BossBattleText").GetComponent<Text> ();
				int levelsleft = 3 - instance.level;
				string levelslefttext = levelsleft.ToString ();
				bossText.text = "The boss shall appear in " + levelslefttext + " more level(s)";
			}
								 
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;
			
			//Get a reference to our image LevelImage by finding it by name.
			levelImage = GameObject.Find ("LevelImage");
			
			//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
			levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
			
			//Set the text of levelText to the string "Day" and append the current level number.
			levelText.text = "Lab " + level;

			restartText = GameObject.Find ("RestartText").GetComponent<Text> ();
			restartText.text = "";
			restart = false;
			gameOver = false;

			//Set levelImage to active blocking player's view of the game board during setup.
			levelImage.SetActive (true);

			//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
			if (instance.level == 2) {
				Invoke ("HideLevelImage", 7);
			}
			else if (instance.level == 3) {
				Invoke ("HideLevelImage", 7);
			}
			else if (instance.level == 4) {
				Invoke ("HideLevelImage", 7);
			}

			else {
				Invoke ("HideLevelImage", levelStartDelay);
			}

			energyText = GameObject.Find("EnergyText").GetComponent<Text>();
			energyText.text = "Energy:" + GameManager.instance.level + "k";

			//Clear any Enemy objects in our List to prepare for next level.
			enemies.Clear ();
			bossenemies.Clear ();

			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene (level);
		}

		
		
		//Hides black image used between levels
		void HideLevelImage()
		{
			//Disable the levelImage gameObject.
			levelImage.SetActive(false);
			
			//Set doingSetup to false allowing player to move again.
			doingSetup = false;
		}
		
		//Update is called every frame.
		void Update()
		{
			//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
			if(playersTurn || enemiesMoving || doingSetup || enemiesMoving || bossenemiesMoving)
				
				//If any of these are true, return and do not start MoveEnemies.
				return;
			
			//Start moving enemies.
			StartCoroutine (MoveBoss ());
			StartCoroutine (MoveEnemies ());


		}
		
		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}
		
		public void AddBossToList(BossOneEnemy script)
		{
			//Add Boss Enemy to List bossenemies
			bossenemies.Add(script);

		}
		
		
		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{
			//Set levelText to display number of levels passed and game over message
			levelText.text = "At lab " + level + " you DIED.";
			//Invoke("HideLevelImage", levelStartDelay);

			//Enable black background image gameObject.
			levelImage.SetActive(true);
			
			//Disable this GameManager.
			enabled = false;


            gameOver = true;

            //restartText.text = "Press 'R' for Restart";
            //restart = true;

            bossText.text = "";
            

            playerFoodPoints = 100;
            level = 0;
        }

		public void GameWin()
		{
			gameObject.GetComponent<ParticleSystem> ().Play ();

			//Set levelText to display number of levels passed and game over message
			levelText.text = "You've defeated phase 1 of the game !";
			//Invoke("HideLevelImage", levelStartDelay);

			//Enable black background image gameObject.
			levelImage.SetActive(true);

			//Disable this GameManager.
			enabled = false;


			gameOver = true;

			restartText.text = "Press 'R' for Restart";
			restart = true;

			bossText.text = "";


			playerFoodPoints = 100;
			level = 0;
		}

		//Coroutine to move enemies in sequence.
		IEnumerator MoveEnemies()
		{
			//While enemiesMoving is true player is unable to move.
			enemiesMoving = true;
			
			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);
			
			//If there are no enemies spawned (IE in first level):
			if (enemies.Count == 0) 
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(turnDelay);
			}
			
			//Loop through List of Enemy objects.
			for (int i = 0; i < enemies.Count; i++)
			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
				enemies[i].TakeTurn ();
				
				//Wait for Enemy's moveTime before moving next Enemy, 
				yield return new WaitForSeconds(enemies[i].moveTime);
			}
				
			//Once Enemies are done moving, set playersTurn to true so player can move.
			playersTurn = true;
			
			//Enemies are done moving, set enemiesMoving to false.
			enemiesMoving = false;
		}

		//Coroutine to move enemies in sequence.
		IEnumerator MoveBoss()
		{
			//While enemiesMoving is true player is unable to move.
			bossenemiesMoving = true;

			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);

			//If there are no enemies spawned (IE in first level):
			if (bossenemies.Count == 0) 
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(turnDelay);
			}

			//Loop through List of Enemy objects.
			for (int i = 0; i < bossenemies.Count; i++)
			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
				bossenemies[i].TakeTurn ();

				//Wait for Enemy's moveTime before moving next Enemy, 
				yield return new WaitForSeconds(bossenemies[i].moveTime);
			}

			//Once Enemies are done moving, set playersTurn to true so player can move.
			playersTurn = true;

			//Enemies are done moving, set enemiesMoving to false.
			bossenemiesMoving = false;
		}


        public void Restart()
        {
            //playerFoodPoints = 100;
            //level = 1;
            //levelImage.SetActive(false);
            enabled = true;
            restart = false;
            restartText.text = "";

            //InitGame();
            //levelText.text = "";
            SoundManager.instance.musicSource.Play();
            //boardScript.SetupScene(level);
            enemies.Clear();
            //Application.LoadLevel(Application.loadedLevel);
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            //Debug.Log("player food points = " + playerFoodPoints);
            //Debug.Log("level = " + level);
        }
    }
}

