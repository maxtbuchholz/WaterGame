using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    public async void NewGameClicked()
    {
        int key = PopupManager.Instance.SummonBinaryPopup("Delete current game and start New Game?");
        bool ans = await PopupManager.Instance.AwaitUserBinaryInput(key);
        PopupManager.Instance.EndBinaryPopup(key);
        if (!ans) return;
        DeleteFilesRecur(Application.persistentDataPath + "/");
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        //await SceneManager.UnloadSceneAsync(sceneIndex);
        await SceneManager.LoadSceneAsync(sceneIndex);
    }
    private void DeleteFilesRecur(string pathName)
    {
        string[] dirs = Directory.GetDirectories(pathName);
        foreach (string path in dirs)
            DeleteFilesRecur(path + "/");
        string[] files = Directory.GetFiles(pathName);
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
    }
}
