using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelsMotionController : MonoBehaviour
{
    [SerializeField] private Settings settings;

    private GameObject redJewel;
    private GameObject blueJewel;
    private GameObject greenJewel;
    private GameObject yellowJewel;

    [SerializeField] private Transform redEndPoint;
    [SerializeField] private Transform blueEndPoint;
    [SerializeField] private Transform greenEndPoint;
    [SerializeField] private Transform yellowEndPoint;

    [SerializeField] private Transform firstMidPoint;
    [SerializeField] private Transform secondMidPoint;

    private float moveTime;

    private GameObject currentJewel;
    private Transform currentEndPoint;

    private void Awake()
    {
        EventManager.OnNutDestroy.AddListener(InstJewel);
    }

    private void Start()
    {
        redJewel = settings.redJewel;
        blueJewel = settings.blueJewel;
        greenJewel = settings.greenJewel;
        yellowJewel = settings.yellowJewel;
        moveTime = settings.moveTime;
    }

    void Update()
    {

    }

    private void InstJewel(string color, Vector3 startPosition)
    {
        SetCurrentJewel(color);
        var jewel = Instantiate(currentJewel, startPosition, Quaternion.identity);
        StartCoroutine(MoveJewel(jewel));
    }

    private void SetCurrentJewel(string color)
    {
        if(color == "Red")
        {
            currentJewel = redJewel;
            currentEndPoint = redEndPoint;
        }
        if (color == "Green")
        {
            currentJewel = greenJewel;
            currentEndPoint = greenEndPoint;
        }
        if (color == "Blue")
        {
            currentJewel = blueJewel;
            currentEndPoint = blueEndPoint;
        }
        if (color == "Yellow")
        {
            currentJewel = yellowJewel;
            currentEndPoint = yellowEndPoint;
        }
    }

    private IEnumerator MoveJewel(GameObject jewel)
    {
        var startPosition = jewel.transform.position;
        float time = 0;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));
        while(time < moveTime)
        {
            jewel.transform.position = Bezier.GetPoint(startPosition, firstMidPoint.position, secondMidPoint.position, currentEndPoint.position, time/moveTime);
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(jewel);
    }
}
