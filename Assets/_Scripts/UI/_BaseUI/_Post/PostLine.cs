using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostLine : MonoBehaviour
{
    //[SerializeField] Image Icon;
    [SerializeField] TMP_Text authorText;
    [SerializeField] TMP_Text titleText;
    //[SerializeField] TMP_Text dateText;

    [SerializeField] Button button;
    [SerializeField] Toggle toggle;

    UPostItem post;

    private void Start()
    {
        button.onClick.AddListener(OnClickButton);
        toggle.onValueChanged.AddListener((x) => { OnClickToggle(x); });
    }
    public void Setup(UPostItem post)
    {
        authorText.text = post.author;
        titleText.text = post.title;
        this.post = post;
    }
    private void OnClickButton()
    {
        PostUI.selectPost = post;
    }
    private void OnClickToggle(bool isOn)
    {
        if (isOn)
        {
            if (!PostUI.selectedPosts.Contains(post))
            {
                PostUI.selectedPosts.Add(post);
            }
        }
        else
        {
            if (PostUI.selectedPosts.Contains(post))
            {
                PostUI.selectedPosts.Remove(post);
            }
        }
    }
    public void SetToggle(bool isOn)
    {
        toggle.isOn = isOn;
    }
}
