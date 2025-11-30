using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameResetter : MonoBehaviour
{
    [SerializeField] private string initialSceneName = "MainMenu"; // O como se llame tu escena inicial
    
    public void CompleteReset()
    {
        // Paso 1: Destruir todos los objetos en DontDestroyOnLoad
        DestroyAllDontDestroyOnLoadObjects();
        
        // Paso 2: Cargar la escena inicial
        SceneManager.LoadScene(initialSceneName);
    }
    
    private void DestroyAllDontDestroyOnLoadObjects()
    {
        // Crear un objeto temporal para acceder a la escena DontDestroyOnLoad
        GameObject temp = new GameObject("Temp");
        DontDestroyOnLoad(temp);
        
        // Obtener la escena DontDestroyOnLoad
        Scene dontDestroyOnLoadScene = temp.scene;
        
        // Destruir el objeto temporal
        Destroy(temp);
        
        // Obtener todos los root GameObjects en la escena DontDestroyOnLoad
        GameObject[] rootObjects = dontDestroyOnLoadScene.GetRootGameObjects();
        
        Debug.Log($"Destruyendo {rootObjects.Length} objetos de DontDestroyOnLoad");
        
        // Destruir todos los objetos
        foreach (GameObject obj in rootObjects)
        {
            Debug.Log($"Destruyendo: {obj.name}");
            Destroy(obj);
        }
    }
}