using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    private string uri = "https://levelgenerator.herokuapp.com";
    private string level = "";
    private int[,] levelMatrix;
    public Button generateLevelButton;

    public GameObject[] prefabs;

    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            generateLevelButton.GetComponentInChildren<Text>().text = "Loading...";
            generateLevelButton.interactable = false;

            yield return webRequest.SendWebRequest();

            generateLevelButton.GetComponentInChildren<Text>().text = "Generate New Level";
            generateLevelButton.interactable = true;


            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    level = webRequest.downloadHandler.text;
                    levelMatrix = LoadLevelMatrix(level);
                    CreateLevel(levelMatrix);
                    break;
            }
        }
    }

    private int[,] LoadLevelMatrix(string level)
    {
        int[,] levelMatrix = new int[128, 128];
        string[] values = Regex.Split(level, @"\D+");

        int parseIndex = 0;
        for (int i = 0; i < levelMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < levelMatrix.GetLength(1); j++)
            {
                while (string.IsNullOrEmpty(values[parseIndex]))
                    parseIndex++;

                levelMatrix[i, j] = int.Parse(values[parseIndex]);
                parseIndex++;
            }
        }

        return levelMatrix;
    }

    public void GenerateLevel()
    {
        StartCoroutine(GetRequest(uri));
    }

    private void CreateLevel(int[,] levelMatrix)
    {
        foreach (Transform child in this.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < levelMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < levelMatrix.GetLength(1); j++)
            {
                Instantiate(prefabs[levelMatrix[i, j]], new Vector3(i, prefabs[levelMatrix[i, j]].transform.position.y, j), Quaternion.identity, this.transform);
            }
        }
    }
}