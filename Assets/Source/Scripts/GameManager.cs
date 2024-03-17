using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool canDrag = true;
    private int startIndex = 1;

    public int currentIndex;
    public bool isStartGame = false;
    [SerializeField] List<GameObject> particleVFXs;
    
    List<TouchPoint> listAllTouchPoint = new List<TouchPoint>();
    List<TouchPoint> listCollected = new List<TouchPoint>();
    private TouchPoint fistCollected;
    [SerializeField] private List<GameObject> listBg;
    private bool isChoseStart = false;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private LineRenderer line;
    
    

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        currentIndex = startIndex;
        
        RandomDataLevel();
        
    }

    void RandomDataLevel()
    {
        foreach (var bg in listBg)
        {
            bg.SetActive(false);
        }
        listBg[currentIndex-1].SetActive(true);
        listAllTouchPoint.Clear();
        listAllTouchPoint = FindObjectsOfType<TouchPoint>(false).ToList();
        canDrag = true;
        isStartGame = true;
        
    }

    void NextLevel()
    {
        currentIndex++;
        if (currentIndex > 3)
        {
            currentIndex = startIndex;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        GameObject explosion = Instantiate(particleVFXs[Random.Range(0,particleVFXs.Count)], transform.position, transform.rotation);
        Destroy(explosion, .75f);
        Invoke(nameof(RandomDataLevel),1.0f);
    }

    private int index = 0;
    
    void Update()
    {
        if(!canDrag) return;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            listCollected.Clear();
            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
            if (targetObject)
            {
                var tp = targetObject.GetComponent<TouchPoint>();
                if (tp != null)
                {
                    if (fistCollected == null)
                    {
                        fistCollected = tp;
                        tp.SetCollected();
                        listCollected.Add(tp);
                        isChoseStart = true;
                        line.positionCount = 0;
                        line.positionCount ++;
                        line.SetPosition(line.positionCount-1,tp.transform.position);
                    }
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (isChoseStart)
            {
                Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
                if (targetObject)
                {
                    if (isChoseStart)
                    {
                        var tp = targetObject.GetComponent<TouchPoint>();
                        if (tp != null)
                        {
                            if (fistCollected != null)
                            {
                                if (tp.id == fistCollected.id)
                                {
                                    tp.SetCollected();
                                    listCollected.Add(tp);
                                    line.positionCount ++;
                                    line.SetPosition(line.positionCount-1,tp.transform.position);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            if (isChoseStart)
            {
                if (fistCollected != null)
                {
                    int sl = CheckCount();
                    if (listCollected.Count  == sl)
                    {
                        fistCollected = null;
                        StartCoroutine(Kill());
                    }
                    else
                    {
                        fistCollected = null;
                        line.positionCount = 0;
                        foreach (var tp in listCollected)
                        {
                            tp.SetUnCollected();
                        }
                    }
                }
            }

            isChoseStart = false;
        }

        int CheckCount()
        {
            var t = fistCollected.id;
            return listAllTouchPoint.FindAll(l => l.id == t).Count;
        }
        

        IEnumerator Kill()
        {
            line.positionCount = 0;
            canDrag = false;
            int i = 0;
            while (i < listCollected.Count)
            {
                var tr = listCollected[i];
                tr.transform.DOScale(Vector3.one*0.55f, 0.15f).OnComplete(() =>
                {
                    tr.gameObject.SetActive(false);
                    listAllTouchPoint.Remove(tr);
                    i++;
                });
                yield return new WaitForSeconds(0.16f);
            }
            
            GameObject explosion = Instantiate(particleVFXs[Random.Range(0,particleVFXs.Count)], transform.position, transform.rotation);
            Destroy(explosion, .75f);
            canDrag = true;
            
            if (listAllTouchPoint.Count == 0)
            {
                Invoke(nameof(NextLevel),0.5f);
            }
        }
    }
}