using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject diamondoPrefab;
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private int num = 10;
    [SerializeField] private Text numText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text diamondText;
    [SerializeField] private Text diamondLoseText;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private AudioClip winnerClip;
    [SerializeField] private GameObject glassHit;

    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform spawner;
    [SerializeField] private Animation cannonAnim;
    [SerializeField] private AudioClip cannonClip;

    [SerializeField] private Obstacle[] lowRadiusPrefabs;
    [SerializeField] private Obstacle[] largeRadiusPrefabs;

    private Transform tower;
    private Vector3 localPosTower;
    private AudioSource source;

    public bool IsPlaying { get; private set; }

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.clip = cannonClip;
        //PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("level"))
        {
            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.SetInt("diamond", 3);
        }
    }

    private void Start()
    {
        CreateTower();
    }

    private void Update()
    {
        if (!IsPlaying)
            return;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !cannonAnim.isPlaying)
        {
            Instantiate(bulletPrefab, spawner.position, spawner.rotation, transform);
            cannonAnim.Play();
            source.Play();
        }

        if (tower != null)
        {
            if(localPosTower != tower.localPosition)
            {
                tower.localPosition = Vector3.MoveTowards(tower.localPosition, localPosTower, Time.deltaTime * 5f);
            }
            if(tower.childCount == 0)
            {
                StartCoroutine(Winner());
            }
        }
    }

    private IEnumerator Winner()
    {
        IsPlaying = false;

        GameObject audio = new GameObject("Audio");
        AudioSource audioSource = audio.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = winnerClip;
        audioSource.Play();
        Destroy(audio, winnerClip.length);

        numText.text = "";

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        ParticleSystem particleSystem = Instantiate(particle);
        particleSystem.Play();
       
        yield return new WaitForSeconds(particleSystem.main.duration * 2f);

        Destroy(particleSystem.gameObject);

        int level = PlayerPrefs.GetInt("level");
        PlayerPrefs.SetInt("level", level + 1);
        int diamond = PlayerPrefs.GetInt("diamond");
        PlayerPrefs.SetInt("diamond", 3 + diamond);

        CreateTower();
        IsPlaying = true;
    }

    private void CreateTower()
    {
        int level = PlayerPrefs.GetInt("level");
        num = 10 + level;
        levelText.text = "Level "+level.ToString();
        int diamond = PlayerPrefs.GetInt("diamond");
        diamondText.text = diamond.ToString();
        numText.text = (num + 1).ToString();

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        tower = new GameObject("Tower").transform;
        tower.transform.parent = transform;
        tower.localPosition = Vector3.zero;
        localPosTower = tower.localPosition;

        float angle = 360f / num;
        int rand = Random.Range(0, prefabs.Length);
        float scaleY = prefabs[rand].transform.localScale.y;
        float posY = prefabs[rand].name == "Cube" ? scaleY : scaleY * 2f;

        for (int i = 0; i < num; i++)
        {
            GameObject obj = Instantiate(prefabs[rand], transform.position + Vector3.up * (posY * i + 1f), Quaternion.identity, tower.transform);
            obj.name = prefabs[rand].name;
            float scale = Mathf.Cos(angle * i * Mathf.Deg2Rad) * 1f;
            scale = Mathf.Abs(scale) + 1.5f;
            obj.transform.localScale = new Vector3(scale, scaleY, scale);
        }

        Instantiate(diamondoPrefab, transform.position + Vector3.up * (posY * num + 2f),diamondoPrefab.transform.rotation, tower.transform);

        Instantiate(lowRadiusPrefabs[Random.Range(0, lowRadiusPrefabs.Length)],transform);
        Instantiate(largeRadiusPrefabs[Random.Range(0, largeRadiusPrefabs.Length)],transform);
    }

    public void MoveTower(float value)
    {
        localPosTower += Vector3.down * value;
        num--;
        numText.text = (num + 1).ToString();
    }

    public void PlayGame()
    {
        numText.gameObject.SetActive(true);
        CreateTower();
        IsPlaying = true;
    }

    public void Menu()
    {
        IsPlaying = false;
        numText.gameObject.SetActive(false);
        CreateTower();
    }

    public void Lose()
    {
        IsPlaying = false;
        glassHit.SetActive(true);
        int diamond = PlayerPrefs.GetInt("diamond");
        diamondLoseText.text = diamond.ToString();
    }

    public void Return()
    {
        int diamond = PlayerPrefs.GetInt("diamond");
        if (diamond >= 3)
        {
            PlayerPrefs.SetInt("diamond",diamond - 3);
            IsPlaying = true;
            glassHit.SetActive(false);
        }
    }
}
