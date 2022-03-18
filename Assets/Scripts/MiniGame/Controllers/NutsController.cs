using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class NutsController : MonoBehaviour
{
    [SerializeField] private Settings settings;
    [SerializeField] private GameObject spinner;
    private List<GameObject> nutsForSpawn;
    private List<int> spawnChances;
    private float accelerationSpeed;
    private List<GameObject> redBolt;
    private List<GameObject> blueBolt;
    private List<GameObject> greenBolt;
    private List<GameObject> yellowBolt;

    private List<List<GameObject>> colorBolts;
    private List<GameObject> forAcid;

    private GameObject acidBottle;
    private GameObject currentNut;
    private GameObject moveParticle;
    private GameObject finishParticle;
    private GameObject moveTempParticle;
    private GameObject destroyParticle;
    private GameObject acidParticle;
    private GameObject rainbowParticle;
    private GameObject stoneNut;
    private float nutSpeed;
    private float rotateNutSpeed;
    private bool isBlockedSended;
    private float blockRotatePosition;
    private int indexCurrentBolt;
    private float correctPosition;
    private int manaPoints;
    private GameObject tempParticle;
    private bool isGameOver;
    private int chanceSetStone;
    private int acidChance;
    private bool isStone;
    private bool isNutTrown;
    private bool isNutMove;
    private LevelManager levelManager;
    private List<GameObject> redList;
    private List<GameObject> greenList;
    private List<GameObject> blueList;
    private List<GameObject> yellowList;

    private void Awake()
    {
        EventManager.OnBoltChanged.AddListener(SetCurrentBolt);
        EventManager.OnTimeOut.AddListener(StartMoveNut);
        EventManager.OnClearBoltButtonClicked.AddListener(ClearBolt);
        EventManager.OnClearSpinnerButtonClicked.AddListener(ClearSpinner);
        EventManager.OnStartLevel.AddListener(LevelStart);
        EventManager.OnThrowNut.AddListener(StartThrowNut);
    }

    private void StartThrowNut()
    {
        if (!isNutMove && !isNutTrown)
        {
            isNutTrown = true;
            EventManager.SendBlockSpinner();
            StartCoroutine(MoveNut(accelerationSpeed));
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ClearSpinner();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ClearBolt();
        }
    }


    public void Start()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        redBolt = new List<GameObject>();
        blueBolt = new List<GameObject>();
        greenBolt = new List<GameObject>();
        yellowBolt = new List<GameObject>();
        colorBolts = new List<List<GameObject>>();
        colorBolts.Add(redBolt);
        colorBolts.Add(blueBolt);
        colorBolts.Add(greenBolt);
        colorBolts.Add(yellowBolt);
        isStone = isNutTrown = isNutMove = false;
        isGameOver = false;
        accelerationSpeed = settings.nutAccelerationSpeed;
        manaPoints = settings.manaPoints;
        acidParticle = settings.acidParticle;
        finishParticle = settings.finishParticle;
        moveParticle = settings.moveParticle;
        destroyParticle = settings.destroyParticle;
        rotateNutSpeed = settings.rotateNutSpeed;
        rainbowParticle = settings.rainbowParticle;
        stoneNut = settings.stoneNut;
        correctPosition = settings.correctPosition;
        blockRotatePosition = settings.blockRotatePosition;
        nutsForSpawn = settings.nutsForSpawn;
        nutSpeed = settings.nutSpeed;
        forAcid = settings.forAcid;
        /*List<GameObject> redList = new List<GameObject>();
        List<GameObject> greenList = new List<GameObject>();
        List<GameObject> redList = new List<GameObject>();
        List<GameObject> redList = new List<GameObject>();*/
        chanceSetStone = levelManager.levels[levelManager.currentLevel].stoneSpawnChance;
        acidChance = levelManager.levels[levelManager.currentLevel].acidSpawnChance;
        acidBottle = settings.acidBottle;
        spawnChances = new List<int>(GetSpawnChances(levelManager.levels[levelManager.currentLevel]));
        
        
    }



    private List<int> GetSpawnChances(GameLevel level)
    {
        List<int> chanses = new List<int>();
        chanses.Add(level.blueNutSpawnChance);
        chanses.Add(level.greenNutSpawnChance);
        chanses.Add(level.redNutSpawnChance);
        chanses.Add(level.yellowNutSpawnChance);
        chanses.Add(level.rainbowNutSpawnChance);
        return chanses;
    }


    public void NutSpawn()
    {
        for (int i = 0; i < 4; i++)
        {
            if(colorBolts[i].Count > 0)
            {
                if (colorBolts[i].Last().tag == "stone")
                {
                    isStone = true;
                    continue;
                }
            }
            
        }
        if (!isGameOver)
        {
            if (isStone)
            {
                var rand = Random.Range(0, 101);
                if(rand<= acidChance)
                {
                    currentNut = Instantiate(acidBottle, settings.nutSpawnPoint, Quaternion.identity, gameObject.transform);
                    EventManager.SendNutSpawned();
                    isStone = false;
                }
                else
                {
                    currentNut = Instantiate(nutsForSpawn[NutSpawnIndex()], settings.nutSpawnPoint, Quaternion.identity, gameObject.transform);
                    EventManager.SendNutSpawned();
                    isStone = false;
                }
            }
            else
            {
                currentNut = Instantiate(nutsForSpawn[NutSpawnIndex()], settings.nutSpawnPoint, Quaternion.identity, gameObject.transform);
                EventManager.SendNutSpawned();
            }
            
        }
        
    }
    void AddNutToList(int boltIndex, GameObject nut)
    {
        colorBolts[boltIndex].Add(nut);
        if(currentNut.tag != "acid")
        {
            EventManager.SendNutDelivered();
        }
        isBlockedSended = false;
    }

    void LevelStart()
    {
        LoadCurrentLevel();
        NutSpawn();
    }

    private void LoadCurrentLevel()
    {
        var level = levelManager.levels[levelManager.currentLevel];
        SetNutsFromLevel(level);
    }

    private void SetNutsFromLevel(GameLevel level)
    {
        var startSpinnerRotation = spinner.transform.eulerAngles.z;
        SetNutsToBolt(level.redScrew);
        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        indexCurrentBolt++;
        SetNutsToBolt(level.greenScrew);
        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        indexCurrentBolt++;
        SetNutsToBolt(level.blueScrew);
        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        indexCurrentBolt++;
        SetNutsToBolt(level.yellowScrew);
        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        indexCurrentBolt = 0;

    }

    private void SetNutsToBolt(List<GameObject> listFromLevel)
    {
        if(listFromLevel != null)
        {
            for (int i = 0; i < listFromLevel.Count; i++)
            {
                var tempObj = Instantiate(listFromLevel[i], new Vector3(0, colorBolts[indexCurrentBolt].Count + correctPosition, 0), Quaternion.identity);
                tempObj.transform.SetParent(spinner.transform);
                colorBolts[indexCurrentBolt].Add(tempObj);
            }
        }
        
    }


    void RemoveNutFromList(List<GameObject> bolt, int index)
    {
        bolt.RemoveAt(index);
    }
    private int SumChance()
    {
        int sum = 0;
        for (int i = 0; i < spawnChances.Count; i++)
        {
            sum += spawnChances[i];
        }
        return sum;
    }

    int NutSpawnIndex()
    {
        int a = 0;
        int i = 0;
        int rand = Random.Range(1, SumChance());
        while (a < rand)
        {
            a += spawnChances[i];
            i++;
        }
        return i - 1;
    }

    private void SetCurrentBolt(int index)
    {
        indexCurrentBolt = index;
    }

    IEnumerator MoveNut(float acceleration)
    {
        var speed = nutSpeed * acceleration;
        while (currentNut.transform.position.y > colorBolts[indexCurrentBolt].Count + correctPosition)
        {
            currentNut.transform.Translate(Vector3.down * Time.deltaTime * speed);
            if (currentNut.transform.position.y < blockRotatePosition && !isBlockedSended)
            {
                isBlockedSended = true;
                if (!isNutTrown)
                {
                    speed = speed * accelerationSpeed;
                }
                
                if (currentNut.tag != "acid")
                {
                    EventManager.SendBlockSpinner();
                    StartCoroutine(RotateNut());
                    PlayMoveParticle(moveParticle, currentNut);
                }
                
            }
            yield return null;
        }
        currentNut.transform.position = new Vector3(0, colorBolts[indexCurrentBolt].Count + correctPosition, 0);
        currentNut.transform.rotation = Quaternion.identity;
        Destroy(moveTempParticle);
        if (currentNut.tag != "acid")
        {
            var finParticle = Instantiate(finishParticle, new Vector3(0, currentNut.transform.position.y, -2), Quaternion.identity);
            DeleteAfterPlay(finParticle, 0.5f);
        }
        else
        {
            EventManager.SendSmashAcid();
            var acidPart = Instantiate(acidParticle, new Vector3(0, currentNut.transform.position.y, -2), Quaternion.identity);
            DeleteAfterPlay(acidPart, 1.2f);
        }
        isNutTrown = isNutMove = false;
        SetParentObject();
        AddNutToList(indexCurrentBolt, currentNut);
        CheckBolt();
        NutSpawn();
    }

    private void SetParentObject()
    {
        currentNut.transform.SetParent(spinner.transform);
    }

    private void StartMoveNut()
    {
        if (!isNutTrown)
        {
            isNutMove = true;
            StartCoroutine(MoveNut(1));
        }
        
    }

    private void CheckBolt()
    {
        if (colorBolts[indexCurrentBolt].Count < 3 && currentNut.tag != "acid")
        {
            return;
        }
        else
        {
            CheckColors();
        }
    }
    private void CheckColors()
    {
        List<GameObject> bolt = colorBolts[indexCurrentBolt];
        if (currentNut.tag == "stone")
        {
            return;
        }
        if (currentNut.tag == "acid")
        {
            Destroy(currentNut);
            bolt.Remove(bolt.Last());
            
            if(bolt.Count > 0 && bolt.Last().tag == "stone")
            {
                EventManager.SendDestroyStoneNut();
                Destroy(bolt.Last());
                bolt.Remove(bolt.Last());
                var nut = Instantiate(forAcid[indexCurrentBolt], new Vector3(0, bolt.Count + correctPosition, 0), Quaternion.identity);
                nut.transform.SetParent(spinner.transform);
                bolt.Add(nut);
                currentNut = nut;
            }
            if(bolt.Count < 3)
            {
                return;
            }
        }
        if ((currentNut.tag == "rainbow" && bolt[bolt.Count - 2].tag == bolt[bolt.Count - 3].tag ||
            currentNut.tag == "rainbow" && bolt[bolt.Count - 3].tag == "rainbow" ||
            currentNut.tag == "rainbow" && bolt[bolt.Count - 2].tag == "rainbow") &&
            (bolt[bolt.Count - 2].tag != "stone" || bolt[bolt.Count - 3].tag != "stone"))
        {
            RemoveThree(bolt);
            return;
        }
        if (currentNut.tag == bolt[bolt.Count - 2].tag && bolt[bolt.Count - 2].tag == bolt[bolt.Count - 3].tag ||
            currentNut.tag == bolt[bolt.Count - 2].tag && bolt[bolt.Count - 3].tag == "rainbow" ||
            currentNut.tag == bolt[bolt.Count - 3].tag && bolt[bolt.Count - 2].tag == "rainbow" ||
            bolt[bolt.Count - 2].tag == "rainbow" && bolt[bolt.Count - 3].tag == "rainbow")
        {
            RemoveThree(bolt);
            return;
        }
        if(bolt.Count > 5)
        {
            isGameOver = true;
            EventManager.SendGameOver();
        }
    }
    private void RemoveThree(List<GameObject> bolt)
    {
        var boltTag = forAcid[indexCurrentBolt].tag;
        if (bolt[bolt.Count-1].tag == boltTag || bolt[bolt.Count - 2].tag == boltTag || bolt[bolt.Count - 3].tag == boltTag)
        {
            EventManager.SendTrueBoltColor(manaPoints);
            EventManager.SendAddMana();
        }
        else
        {
            
            var rand = Random.Range(0, 101);
            {
                if(rand <= chanceSetStone)
                {
                    EventManager.SendColorFalse();
                    SetStone();
                }
            }
        }
            
        string jewelTag;
        for (int i = 3; i > 0; i--)
        {
            if(bolt[bolt.Count - i].tag == "rainbow")
            {
                continue;
            }
            else
            {
                jewelTag = bolt[bolt.Count - i].tag;
                EventManager.SendAddJewel(jewelTag, 1);
                EventManager.SendNutDestroy(jewelTag, bolt[bolt.Count - i].transform.position);
            }
        }
        int a = bolt.Count - 1;
        for (int i = 0; i < 3; i++)
        {
            Destroy(bolt[a]);
            if(bolt[a].tag == "rainbow")
            {
                StartCoroutine(PlayParticle(rainbowParticle, bolt[a], 1f));
            }
            else
            {
                StartCoroutine(PlayParticle(destroyParticle, bolt[a], 1f));
            }
            bolt.RemoveAt(a);
            a--;
        }
        EventManager.SendNutsStack();
        ChangeBoltsPositions();

    }

    void ChangeBoltsPositions()
    {
        var currentSpinnerRotation = spinner.transform.eulerAngles.z;
        var tempIndexbolt = indexCurrentBolt;
        
        redList = new List<GameObject>(colorBolts[0]);
        greenList = new List<GameObject>(colorBolts[1]);
        blueList = new List<GameObject>(colorBolts[2]);
        yellowList = new List<GameObject>(colorBolts[3]);

        indexCurrentBolt = 0;

        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        ClearBolt();
        SetNutsToBolt(yellowList);
        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        indexCurrentBolt++;
        ClearBolt();
        SetNutsToBolt(redList);
        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        indexCurrentBolt++;
        ClearBolt();
        SetNutsToBolt(greenList);
        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        indexCurrentBolt++;
        ClearBolt();
        SetNutsToBolt(blueList);
        indexCurrentBolt = tempIndexbolt;
        /*if (indexCurrentBolt == 3)
        {
            indexCurrentBolt = 0;
        }
        else
        {
            indexCurrentBolt++;
        }*/
        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentSpinnerRotation + 90));
    }

    private void ClearBolt()
    {
        int nutNumOfBolt = colorBolts[indexCurrentBolt].Count;
        for (int i = nutNumOfBolt - 1; i > -1 ; i--)
        {
            Destroy(colorBolts[indexCurrentBolt][i]);
            StartCoroutine(PlayParticle(destroyParticle, colorBolts[indexCurrentBolt][i], 2f));
            colorBolts[indexCurrentBolt].RemoveAt(i);
        }
    }

    private void SetStone()
    {
        var rand = Random.Range(1, 4);
        var startSpinnerRotation = spinner.transform.eulerAngles.z;
        var tempCurrentIndex = indexCurrentBolt;
        for (int i = 0; i < rand; i++)
        {
            spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, spinner.transform.rotation.eulerAngles.z - 90));
            if (indexCurrentBolt == 0)
            {
                indexCurrentBolt = 3;
            }
            else
            {
                indexCurrentBolt--;
            }
        }

        var stone = Instantiate(stoneNut, new Vector3(0, colorBolts[indexCurrentBolt].Count + correctPosition, 0), Quaternion.identity);
        stone.transform.SetParent(spinner.transform);
        colorBolts[indexCurrentBolt].Add(stone);
        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, startSpinnerRotation));
        indexCurrentBolt = tempCurrentIndex;

    }

    private void ClearSpinner()
    {
        List<NutWithPosition> objects = new List<NutWithPosition>();

        int b = 0;
        for (int i = 0; i < colorBolts.Count; i++)
        {
            for (int j = 0; j < colorBolts[i].Count; j++)
            {
                NutWithPosition nut = new NutWithPosition(colorBolts[i][j], i, j);
                objects.Add(nut);
            }
        }
        var results = objects.OrderBy(u => u.nut.tag).ToList();

        for (int k = 0; k < results.Count - 2;)
        {
            if (results[k].nut.tag == results[k + 1].nut.tag && results[k + 1].nut.tag == results[k + 2].nut.tag)
            {
                for (int i = 3; i > 0; i--)
                {
                    results.RemoveAt(k + i - 1);
                }
            }
            else
            {
                k++;
            }
        }


        for (int i = 0; i < colorBolts.Count; i++)
        {
            for (int k = colorBolts[i].Count; k > 0; k--)
            {
                Destroy(colorBolts[i][k - 1]);
                colorBolts[i].RemoveAt(k - 1);
            }

        }
        var startSpinnerRotation = spinner.transform.eulerAngles.z;
        var tempCurrentIndex = indexCurrentBolt;
        for (int i = 0; i < results.Count; i++)
        {
            var tempObj = Instantiate(results[i].nut, new Vector3(0, colorBolts[indexCurrentBolt].Count + correctPosition, 0), Quaternion.identity);
            tempObj.transform.SetParent(spinner.transform);
            colorBolts[indexCurrentBolt].Add(tempObj);
            var degr = spinner.transform.eulerAngles.z;
            spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, degr - 90));


            if (indexCurrentBolt == 0)
            {
                indexCurrentBolt = 3;
            }
            else
            {
                indexCurrentBolt--;
            }
        }
        spinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, startSpinnerRotation));
        indexCurrentBolt = tempCurrentIndex;
    }

    IEnumerator RotateNut()
    {
        while (isBlockedSended)
        {
            currentNut.transform.Rotate(0, 1 * rotateNutSpeed * Time.deltaTime, 0);
            yield return null;
        }
    }

    private void PlayMoveParticle(GameObject particle, GameObject parent)
    {
        moveTempParticle = Instantiate(particle, parent.transform.position, Quaternion.identity, parent.transform);
    }

    IEnumerator PlayParticle(GameObject particle, GameObject parent, float playTime)
    {
        var obj = Instantiate(particle, parent.transform.position, Quaternion.identity, gameObject.transform);
        yield return new WaitForSeconds(playTime);
        Destroy(obj);

    }

    void DeleteAfterPlay(GameObject particle, float delay)
    {
        tempParticle = particle;
        Invoke("Delete", delay);
    }

    void Delete()
    {
        Destroy(tempParticle);
    }

}
class NutWithPosition
{
    public GameObject nut;
    public int currentBolt;
    public int currentIndex;

    public NutWithPosition(GameObject nut, int currentBolt, int currentIndex)
    {
        this.nut = nut;
        this.currentBolt = currentBolt;
        this.currentIndex = currentIndex;
    }
}
