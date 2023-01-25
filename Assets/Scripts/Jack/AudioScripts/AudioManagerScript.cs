using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManagerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	[Range(1, 150)]
	private int audioSourcePoolSize = 20;

	[SerializeField]
	private GameObject audioSourcePrefab = null;

	[SerializeField]
	private AudioFile baseMusicFile = null;

	[SerializeField]
	private List<AudioFile> damagePlayerFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> damageEnemyFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> shieldHitFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> asteroidCrackFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> spaceInvaderShootFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> pongShootFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> carZoomFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> puddleSplashFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> enemyKilledFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> bossDeathFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> bossShootFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> bossSlamFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> activeTrapFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> playerKilledFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> playerAttackFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> playerDashFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> plushHitFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> plushDestroyedFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> boxHitFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> boxDestroyedFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> pickupCollectedFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> highComboFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> midComboFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> lowComboFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> firstPlaceFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> secondPlaceFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> thirdPlaceFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> roomClearedFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> DoorOpenFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> DoorCloseFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> RoomAssemblyFiles = new List<AudioFile>();

	[SerializeField]
	private List<AudioFile> MenuSelectFiles = new List<AudioFile>();
	#endregion

	#region Private Variable Declarations.
	private Vector3 cameraPos;
	private static bool firstFrame = true;
	private static GameObject musicAudioSource = null;
	private static ObjectPool audioSourcePool;
	private static bool transitionMusic = false;
	private static float transitionTime = 0.0f;
	private static AudioFile newMusic = null;
	private static bool newMusicShouldLoop = false;
	private static float maxVolume = 0.0f;
	private float timer = 0.0f;

	#endregion

	#region Private Functions.
	// Start is called before the first frame update
	void Start() {
		//Create the audio source pool.
		audioSourcePool = new ObjectPool(this.gameObject.transform, audioSourcePrefab, audioSourcePoolSize);

		//Create an audio source specifically for music.
		musicAudioSource = Instantiate(audioSourcePrefab, this.gameObject.transform);
		musicAudioSource.GetComponent<AudioSource>().clip = null;

		//Reset static variables.
		firstFrame = true;
		transitionMusic = false;
		transitionTime = 0.0f;
		newMusic = null;
		newMusicShouldLoop = false;
		maxVolume = 0.0f;

		//Subscribe listeners to correct events.
		BaseEnemy.DamagePlayer += DamagePlayerListener;
		Bullet.DamageEnemy += DamageEnemyListener;
		Bullet.ShieldHit += ShieldHitListener;
		Asteroid.AsteroidCrack += AsteroidCrackListener;
		SpaceInvader.SpaceInvaderShoot += SpaceInvaderShootListener;
		PongBat.pongShoot += PongShootListener;
		Zoobers.CarZoom += CarZoomListener;
		PuddlePlacement.PuddleSplash += PuddleSplashListener;
		BaseEnemy.EnemyKilled += EnemyKilledListener;
		BaseEnemy.BossDeath += BossDeathListener;
		CowboyBoss.BossShoot += BossShootListener;
		CowboyBoss.BossSlam += BossSlamListener;
		BaseTrap.ActiveTrap += ActiveTrapListener;
		CharacterController.PlayerKilled += PlayerKilledListener;
		CharacterController.PlayerDashing += PlayerDashListener;
		BubbleGun.PlayerAttack += PlayerAttackListener;
		DestructableObject.DestructablePlushHit += PlushHitListener;
		DestructableObject.DestructablePlushDestroyed += PlushDestroyedListener;
		DestructableObject.DestructableBoxHit += BoxHitListener;
		DestructableObject.DestructableBoxDestroyed += BoxDestroyedListener;
		PickUp.PickupCollected += PickupCollectedListener;
		PowerUpObject.PowerUpPickedUp += PickupCollectedListener;
		ComboSystem.HighCombo += HighComboListener;
		ComboSystem.MidCombo += MidComboListener;
		ComboSystem.LowCombo += LowComboListener;
		InGameUI.FirstPlace += FirstPlaceListener;
		InGameUI.SecondPlace += SecondPlaceListener;
		InGameUI.ThirdPlace += ThirdPlaceListener;
		RoomInformation.RoomCleared += RoomClearedListener;
		Teleporter.DoorsOpened += DoorOpenListener;
		Teleporter.DoorsClosed += DoorCLoseListener;
		ExplodeRoom.RoomAssemblyStarted += RoomAssemblyListener;
		MenuNavigationScript.ButtonSelect += MenuSelectListener;
	}

	// Update is called once per frame
	void Update() {
		//Move the music audio source infront of the main camera.
		cameraPos = new Vector3(0.0f, 0.0f, 0.0f);
		GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
		if (cam) {
			cameraPos = cam.transform.position;
			if (firstFrame && baseMusicFile != null) {
				firstFrame = false;
				PlayMusicClip(baseMusicFile, 0.0f, true);
			}
		}

		if (musicAudioSource) {
			musicAudioSource.transform.position = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z);
		}


		//Transition music to new audio file if appropriate.
		if (transitionMusic && newMusic != null) {
			PlayNewMusic();
		}
	}

	private void PlayNewMusic() {
		//Get the music audio source object.
		AudioSource audioSource = musicAudioSource.GetComponent<AudioSource>();

		//If the music should immediately start playing skip transition.
		if (transitionTime <= 0 || audioSource.clip == null) {
			InstantPlayMusic(audioSource);
		} else {
			//Transition to the new music.
			TransitionMusic(audioSource);
		}
	}

	private void InstantPlayMusic(AudioSource audioSource) {
		//Stop any previous music from playing.
		audioSource.Stop();

		//Make sure transition music is set to false so music isn't messed up.
		transitionMusic = false;
		timer = 0.0f;

		//Find the audio clip with the specified name.
		AudioClip audioClip = newMusic.audioClip;
		float volume = newMusic.volume;

		//Set the music volume to the correct value.
		audioSource.volume = volume;

		if (audioClip != null) {
			//Pass audio clip values to the audio source and play the clip.
			audioSource.clip = audioClip;
			audioSource.loop = newMusicShouldLoop;
			audioSource.Play();
		} else {
			Debug.LogError("Invalid music file passed into the audio manager.");
		}
	}

	private void TransitionMusic(AudioSource audioSource) {
		//Variables for transition.
		float volumeIncrement = 0.0f;

		if (timer <= (transitionTime / 2.0f)) {
			//Calculate how much the volume will need to decrement each frame.
			volumeIncrement = maxVolume / (transitionTime / 2);

			//Decrease volume of music audio source.
			audioSource.volume -= volumeIncrement * Time.deltaTime;

			//If the next frame is gonna be past the midpoint.
			if (timer + Time.deltaTime > (transitionTime / 2.0f)) {
				//Set audio source file to new music file.
				audioSource.Stop();
				audioSource.volume = 0.0f;
				audioSource.clip = newMusic.audioClip;
				audioSource.Play();
				audioSource.loop = newMusicShouldLoop;
				maxVolume = newMusic.volume;
			}
		} else {
			//Calculate how much the volume will need to increment each frame.
			volumeIncrement = maxVolume / (transitionTime / 2);

			//Increment volume of audio source.
			audioSource.volume += volumeIncrement * Time.deltaTime;
		}

		//Debug.Log("Current volume: " + audioSource.volume + " Timer Value: " + timer);

		//Increment timer.
		timer += Time.deltaTime;

		if (timer >= transitionTime) {
			//Transition complete Ensure variables are reset and correct..
			audioSource.volume = maxVolume;
			transitionMusic = false;
			timer = 0.0f;
		}
	}

	private void OnDisable() {
		//Unsubscribe listeners from events.
		BaseEnemy.DamagePlayer -= DamagePlayerListener;
		Bullet.DamageEnemy -= DamageEnemyListener;
		Bullet.ShieldHit -= ShieldHitListener;
		Asteroid.AsteroidCrack -= AsteroidCrackListener;
		SpaceInvader.SpaceInvaderShoot -= SpaceInvaderShootListener;
		PongBat.pongShoot -= PongShootListener;
		Zoobers.CarZoom -= CarZoomListener;
		PuddlePlacement.PuddleSplash -= PuddleSplashListener;
		BaseEnemy.EnemyKilled -= EnemyKilledListener;
		BaseEnemy.BossDeath -= BossDeathListener;
		CowboyBoss.BossShoot -= BossShootListener;
		CowboyBoss.BossSlam -= BossSlamListener;
		BaseTrap.ActiveTrap -= ActiveTrapListener;
		CharacterController.PlayerKilled -= PlayerKilledListener;
		CharacterController.PlayerDashing -= PlayerDashListener;
		BubbleGun.PlayerAttack -= PlayerAttackListener;
		DestructableObject.DestructablePlushHit -= PlushHitListener;
		DestructableObject.DestructablePlushDestroyed -= PlushDestroyedListener;
		DestructableObject.DestructableBoxHit -= BoxHitListener;
		DestructableObject.DestructableBoxDestroyed -= BoxDestroyedListener;
		PickUp.PickupCollected -= PickupCollectedListener;
		PowerUpObject.PowerUpPickedUp -= PickupCollectedListener;
		ComboSystem.HighCombo -= HighComboListener;
		ComboSystem.MidCombo -= MidComboListener;
		ComboSystem.LowCombo -= LowComboListener;
		InGameUI.FirstPlace -= FirstPlaceListener;
		InGameUI.SecondPlace -= SecondPlaceListener;
		InGameUI.ThirdPlace -= ThirdPlaceListener;
		RoomInformation.RoomCleared -= RoomClearedListener;
		Teleporter.DoorsOpened -= DoorOpenListener;
		Teleporter.DoorsClosed -= DoorCLoseListener;
		ExplodeRoom.RoomAssemblyStarted -= RoomAssemblyListener;
		MenuNavigationScript.ButtonSelect -= MenuSelectListener;
	}

	/// <summary>
	/// Player an audio file that has been given the passed in name.
	/// Also can give the audio source a position to play the sound from.
	/// </summary>
	/// <param name="name"></param>
	private IEnumerator PlaySoundEffect(Vector3 a_position, List<AudioFile> listOfSFX) {
		//Check list is valid.
		if (listOfSFX.Count <= 0) {
			//Early out.
			Debug.LogError("Sound Effect list needs to be populated in audio manager.");
			yield return new WaitForEndOfFrame();
		} else {

			//Choose a random sfx.
			int randomIndex = Random.Range(0, listOfSFX.Count - 1);

			//Find the audio clip with the specified name.
			AudioClip audioClip = listOfSFX[randomIndex].audioClip;
			float volume = listOfSFX[randomIndex].volume;

			//Get an audio source object.
			GameObject audioSoureGameObject = audioSourcePool.SpawnObject();
			AudioSource audioSource = audioSoureGameObject.GetComponent<AudioSource>();

			//Change the volume to the correctly volume for the clip.
			audioSource.volume = volume;

			//Move it to the specified location.
			audioSoureGameObject.transform.position = a_position;

			//Get the length of the audio clip.
			float clipLength = 0.5f;
			if (audioClip != null) {
				//Pass audio clip values to the audio source and play the clip.
				clipLength = audioClip.length;
				audioSource.clip = audioClip;
				audioSource.loop = false;
				audioSource.Play();
			} else {
				Debug.LogError("Audio Manager has not been populated with sound effects!!!");
			}

			//Pause execution for length of the audio.
			yield return new WaitForSeconds(clipLength);

			//Deactivate the audio Source.
			audioSource.Stop();
			audioSoureGameObject.SetActive(false);
		}
	}
	#endregion

	#region Event Listeners.

	private void DamageEnemyListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, damageEnemyFiles));
	}

	private void ShieldHitListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, shieldHitFiles));
	}

	private void DamagePlayerListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, damagePlayerFiles));
	}

	private void AsteroidCrackListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, asteroidCrackFiles));
    }

	private void SpaceInvaderShootListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, spaceInvaderShootFiles));
    }

	private void PongShootListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, pongShootFiles));
	}

	private void CarZoomListener(object sender, EventArgs e)
	{
		StartCoroutine(PlaySoundEffect(cameraPos, carZoomFiles));
	}

	private void PuddleSplashListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, puddleSplashFiles));
    }

	private void EnemyKilledListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, enemyKilledFiles));
	}

	private void BossDeathListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, bossDeathFiles));
    }

	private void BossShootListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, bossShootFiles));
	}

	private void BossSlamListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, bossSlamFiles));
	}

	private void ActiveTrapListener(object sender, EventArgs e)
	{
		StartCoroutine(PlaySoundEffect(cameraPos, activeTrapFiles));
	}

	private void PlayerKilledListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, playerKilledFiles));
	}

	private void PlayerAttackListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, playerAttackFiles));
	}

	private void PlayerDashListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, playerDashFiles));
    }

	private void PlushHitListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, plushHitFiles));
    }

	private void PlushDestroyedListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, plushDestroyedFiles));
	}

	private void BoxHitListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, boxHitFiles));
	}

	private void BoxDestroyedListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, boxDestroyedFiles));
	}

	private void PickupCollectedListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, pickupCollectedFiles));
	}

	private void HighComboListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, highComboFiles));
	}

	private void MidComboListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, midComboFiles));
	}

	private void LowComboListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, lowComboFiles));
	}

	private void FirstPlaceListener(object sender, EventArgs e)
	{
		StartCoroutine(PlaySoundEffect(cameraPos, firstPlaceFiles));
	}

	private void SecondPlaceListener(object sender, EventArgs e)
	{
		StartCoroutine(PlaySoundEffect(cameraPos, secondPlaceFiles));
	}

	private void ThirdPlaceListener(object sender, EventArgs e)
	{
		StartCoroutine(PlaySoundEffect(cameraPos, thirdPlaceFiles));
	}

	private void RoomClearedListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, roomClearedFiles));
	}

	private void DoorOpenListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, DoorOpenFiles));
	}

	private void DoorCLoseListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, DoorCloseFiles));
	}

	private void RoomAssemblyListener(object sender, EventArgs e)
	{
		StartCoroutine(PlaySoundEffect(cameraPos, RoomAssemblyFiles));
	}

	private void MenuSelectListener(object sender, EventArgs e) {
		StartCoroutine(PlaySoundEffect(cameraPos, MenuSelectFiles));
    }

	#endregion

	#region Public Access Functions (Getters and Setters).


	public static void PlayMusicClip(AudioFile audioFile, float a_transitionTime, bool a_loop) {
		//Get the music audio source object.
		AudioSource audioSource = musicAudioSource.GetComponent<AudioSource>();
		newMusic = audioFile;
		transitionTime = a_transitionTime;
		transitionMusic = true;
		newMusicShouldLoop = a_loop;

		//Set the music volume to the correct value.
		newMusic = audioFile;
		maxVolume = audioSource.volume;
	}

	public static void PlayBaseMusic()
	{
		firstFrame = true;
	}
	#endregion
}
