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
        

        int funds = 0;
        public void AddFunds(int funds)
        {
            this.funds = funds;
            
        }
        
        void Awake()
        {
            if (GameManager.instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
                Debug.Log("GameManager is set in Awake.");
            }
            else
            {
                Debug.Log("Duplicate GameManager is destroyed in Awake.");
                Destroy(this);
            }
        }


        public void LoadScenebyName(string name)
        {
            SceneManager.LoadScene(name);
        }
    }
}