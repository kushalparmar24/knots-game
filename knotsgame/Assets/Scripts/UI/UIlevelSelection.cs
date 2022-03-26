using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIlevelSelection : UIBase
{
    public GameObject content;
    public GameObject levelElement;
   List<GameObject> levelpool;

    private void Awake()
    {
        levelpool = new List<GameObject>();
    }

    /// <summary>
    /// Instantiate level ui based on the level data and also cache gameobject on the list
    /// </summary>
    private void OnEnable()
    {
        for(int i = 0;i<gameManager.Instance.getlevelDatas().Count;i++)
        {
            if(i < levelpool.Count)
            {
                levelpool[i].GetComponent<levelElement>().setUI(i);
                levelpool[i].SetActive(true);
            }
            else
            {
                GameObject currentElement = Instantiate(levelElement, content.transform);
                levelpool.Add(currentElement);
                levelpool[i].GetComponent<levelElement>().setUI(i);
                levelpool[i].SetActive(true);
            }
        }
    }
}
