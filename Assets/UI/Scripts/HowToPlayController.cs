using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HowToPlayController : MonoBehaviour
{
    public GameObject[] pages;
    public Button nextButton;
    public Button prevButton;
    public Button backToMenuButton;
    public TMP_Text pageNumber;

    private int currentPage = 0;

    void Start()
    {
        ShowPage(0);

        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PreviousPage);
        backToMenuButton.onClick.AddListener(BackToMenu);
    }

    void ShowPage(int index)
    {
        currentPage = index;

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPage);
        }

        prevButton.gameObject.SetActive(currentPage > 0);
        nextButton.gameObject.SetActive(currentPage < pages.Length - 1);

        pageNumber.text = (currentPage + 1) + "/5";
    }

    void NextPage()
    {
        if (currentPage < pages.Length - 1)
            ShowPage(currentPage + 1);
    }

    void PreviousPage()
    {
        if (currentPage > 0)
            ShowPage(currentPage - 1);
    }

    void BackToMenu()
    {
        gameObject.SetActive(false);
    }
}