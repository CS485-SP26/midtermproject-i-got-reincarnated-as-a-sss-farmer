using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        // static == class level, aka GameManager.instance
        static private GameManager instance = null;

        static public GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                    Debug.Log("Create a new GameManager.");
                }
                return instance;
            }
            //do not make a set
        }
        

        [SerializeField] private int funds = 0;
        public int Funds => funds;
        public void AddFunds(int amount)
        {
            funds += amount;
            Debug.Log($"Funds updated: {funds}");

        }
        
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("Duplicate GameManager detected! Destroying the newcomer.");
                Destroy(gameObject); // DESTROY THE GAMEOBJECT, NOT JUST 'THIS'
                return;
            }

                instance = this;
            DontDestroyOnLoad(gameObject);
            }
            
            

        public void LoadScenebyName(string name)
        {
            SceneManager.LoadScene(name);
        }
    }
}