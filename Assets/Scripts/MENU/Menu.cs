using UnityEngine;
public class Menu : MonoBehaviour
{
    #region VARIABLE
    public string menuName;
	public bool open;
    #endregion
    public void Open()
	{
		open = true;
		gameObject.SetActive(true);
	}
	public void Close()
	{
		open = false;
		gameObject.SetActive(false);
	}
}