
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HowToPlayController : MonoBehaviour
{
    [Header("Tutorial Pages")]
    public GameObject[] pages;

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button prevButton;

    [Header("Page Number")]
    public TMP_Text pageNumber;

    private int currentPage = 0;

    private void OnEnable()
    {
        ShowPage(0);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            ShowPage(currentPage + 1);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            ShowPage(currentPage - 1);
        }
    }

    private void ShowPage(int pageIndex)
    {
        currentPage = pageIndex;

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPage);
        }

        pageNumber.text = (currentPage + 1) + "/" + pages.Length;

        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < pages.Length - 1;
    }
}
