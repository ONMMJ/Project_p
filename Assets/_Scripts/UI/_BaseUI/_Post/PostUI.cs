using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using BackEnd;
using UnityEditor.Experimental.GraphView;

public class PostUI : BaseUI
{
    public static UPostItem selectPost;
    public static List<UPostItem> selectedPosts = new();
    [SerializeField] List<PostLine> postLines;
    [SerializeField] Toggle allToggle;

    [Header("PostTypeButton")]
    [SerializeField] Button adminButton;
    [SerializeField] Button rankButton;
    [SerializeField] Button couponButton;

    [Header("BottomButton")]
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button selectedPostsReceiveButton;

    [Header("PostDetailPanel")]
    [SerializeField] TMP_Text postDetailText;
    [SerializeField] GameObject postDetailPanel;
    [SerializeField] Button postDetailReceiveButton;
    [SerializeField] Transform iconParent;
    [SerializeField] Icon iconPrefab;

    const int pageMaxCount = 6;
    int MaxCount => BackendManager.Instance.Post.PostList.Count;
    int pageMaxNum => (int)Mathf.Ceil(MaxCount / (float)pageMaxCount);
    int pageNum = 0;

    PostType nowPostType;

    private void Start()
    {
        adminButton.onClick.AddListener(() => { SetPostType(PostType.Admin); });
        rankButton.onClick.AddListener(() => { SetPostType(PostType.Rank); });
        couponButton.onClick.AddListener(() => { SetPostType(PostType.Coupon); });
        prevButton.onClick.AddListener(OnClickPrev);
        nextButton.onClick.AddListener(OnClickNext);
        selectedPostsReceiveButton.onClick.AddListener(OnClickSelectedPostsReceive);

        postDetailReceiveButton.onClick.AddListener(OnClickPostDetailReceive);
        // 스킬포인트가 변화되면 UI에 실시간 적용
        this.UpdateAsObservable()
            .Select(x => selectPost)
            .DistinctUntilChanged()
            .Subscribe(x => { PostView(selectPost); })
            .AddTo(this);

        // 스킬포인트가 변화되면 UI에 실시간 적용
        this.UpdateAsObservable()
            .Select(x => allToggle.isOn)
            .DistinctUntilChanged()
            .Subscribe(x => { SetAllToggle(allToggle.isOn); })
            .AddTo(this);
    }

    private void SetPostType(PostType postType)
    {
        nowPostType = postType;
        Refresh();
    }

    private void Refresh()
    {
        BackendManager.Instance.Post.GetPostList(nowPostType);
        allToggle.isOn = false; SetAllToggle(allToggle.isOn);
        pageNum = 0; selectPost = null;
        postDetailPanel.SetActive(false);
        SetPost();
    }

    protected override void EnableUI()
    {
        nowPostType = PostType.Admin;
        Refresh();
    }

    public void SetPost()
    {
        for (int i = 0; i < pageMaxCount; i++)
        {
            int index = (pageNum * pageMaxCount) + i;
            if (index >= MaxCount)
            {
                postLines[i].gameObject.SetActive(false);
                continue;
            }

            postLines[i].gameObject.SetActive(true);
            postLines[i].Setup(BackendManager.Instance.Post.PostList[index]);
        }
    }

    public void PostView(UPostItem post)
    {
        if (post != null)
        {
            foreach(AttachedItem item in post.items)
            {
                Icon itemIcon = Instantiate(iconPrefab, iconParent);
                itemIcon.SetIcon(item.icon);
            }
            postDetailPanel.SetActive(true);
            postDetailText.text = post.ToString();
        }
    }

    private void SetAllToggle(bool isOn)
    {
        foreach(PostLine postLine in postLines)
        {
            postLine.SetToggle(isOn);
        }
    }

    private void OnClickPrev()
    {
        pageNum--;
        if (pageNum < 0)
        {
            pageNum = 0;
            return;
        }
        SetPost();
    }
    private void OnClickNext()
    {
        pageNum++;
        if (pageNum >= pageMaxNum)
        {
            pageNum = pageMaxNum;
            return;
        }
        SetPost();
    }
    private void OnClickSelectedPostsReceive()
    {
        foreach (UPostItem post in selectedPosts)
        {
            BackendManager.Instance.Post.ReceivePostItem(post);
        }
        Refresh();
    }
    private void OnClickPostDetailReceive()
    {
        BackendManager.Instance.Post.ReceivePostItem(selectPost);
        Refresh();
    }
}
