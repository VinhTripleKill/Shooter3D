using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CoopModeUI : MonoBehaviour
{
    [SerializeField] private Button createRoomB;
    [SerializeField] private Button quickMatchB;
    [SerializeField] private Button refeshB;
    private void Start()
    {
        if(createRoomB!=null)
            createRoomB.onClick.AddListener(CreateRoom);
    }
    private void OnDestroy()
    {
        if(createRoomB!=null)
            createRoomB.onClick.RemoveListener(CreateRoom);
    }
    private void CreateRoom()
    {
        SceneManager.LoadScene("CoopMode");
    }
}
