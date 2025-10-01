using System.Collections;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;
    
    public void KillGame()
    {
        StartCoroutine(DelayedQuit());
    }
    
    public void MoveToCharacterSelect()
    {
        while (PlayerEntity.PlayerList.Count > 0)
            PlayerEntity.PlayerList[0].DestroyMe();

        VoteEffectManager.Instance?.ClearAll();

        StartCoroutine(DelayedLoad(Scenes.CharacterSelect));
        //sceneLoader.LoadScene(Scenes.CharacterSelect);
    }
    public void MoveToMainMenu()
    {
        StartCoroutine(DelayedLoad(Scenes.MainMenu));
        //sceneLoader.LoadScene(Scenes.MainMenu);
    }
    
    private IEnumerator DelayedLoad(Scenes scene)
    {
        //BGMPlayer.instance.soundPlayer.PlaySound(0);//---------------------
        yield return new WaitForSeconds(0.4f);
        sceneLoader.LoadScene(scene);
    }
    
    private IEnumerator DelayedQuit()
    {
        //BGMPlayer.instance.soundPlayer.PlaySound(0);//--------------------------
        yield return new WaitForSeconds(0.4f);
        Application.Quit();
    }
}
