using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx.Triggers;
using UniRx;

public enum FILTER
{
    DEFAULT,
    ASC,
    DESC,
    COUNT,
}
public class FilterButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text text;
    [SerializeField] string filterName;
    public Button Button => button;

    public FILTER State { get; private set; }

    private void Start()
    {
        // 상태가 변경되면 자동으로 Text 변경
        this.UpdateAsObservable()
            .Select(x => State)
            .DistinctUntilChanged()
            .Subscribe(x => SetText())
            .AddTo(this);
    }

    private void SetText()
    {
        switch (State) {
            case FILTER.DEFAULT:
                this.text.text = $"{filterName} -";
                break;
            case FILTER.ASC:
                this.text.text = $"{filterName} ↑";
                break;
            case FILTER.DESC:
                this.text.text = $"{filterName} ↓";
                break;
        }
    }
    public FILTER NowState()
    {
        return State;
    }
    public FILTER NextState()
    {
        //State = (FILTER)(((int)State + 1) % (int)FILTER.COUNT);
        switch (State)
        {
            case FILTER.DEFAULT:
            case FILTER.DESC:
                State = FILTER.ASC;
                break;
            case FILTER.ASC:
                State = FILTER.DESC;
                break;

        }
        return State;
    }

    public void Reset()
    {
        State = FILTER.DEFAULT;
    }
}
