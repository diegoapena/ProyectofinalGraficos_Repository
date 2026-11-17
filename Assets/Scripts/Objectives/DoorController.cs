using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorController : MonoBehaviour
{
    [SerializeField] private TargetObjective[] objectives;
    [SerializeField] private Transform movingPart;
    [SerializeField] private Vector3 openOffset = new Vector3(0f, 4f, 0f);
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private bool deactivateOnOpen;

    public UnityEvent OnOpenedEvent = new UnityEvent();

    private Vector3 closedPosition;
    private bool isOpen;
    private bool forceClosed;
    private readonly List<TargetObjective> subscribedObjectives = new List<TargetObjective>();

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (movingPart == null)
            movingPart = transform;

        closedPosition = movingPart.localPosition;
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Start()
    {
        Subscribe();
        TryOpen();
    }

    private void Subscribe()
    {
        if (objectives == null)
            return;

        foreach (TargetObjective objective in objectives)
        {
            if (objective == null || subscribedObjectives.Contains(objective))
                continue;

            objective.Completed += HandleObjectiveCompleted;
            subscribedObjectives.Add(objective);
        }
    }

    private void Unsubscribe()
    {
        foreach (TargetObjective objective in subscribedObjectives)
        {
            if (objective != null)
                objective.Completed -= HandleObjectiveCompleted;
        }

        subscribedObjectives.Clear();
    }

    private void HandleObjectiveCompleted(TargetObjective objective)
    {
        TryOpen();
    }

    public bool AllObjectivesCompleted()
    {
        if (objectives == null || objectives.Length == 0)
            return false;

        foreach (TargetObjective objective in objectives)
        {
            if (objective != null && !objective.IsCompleted)
                return false;
        }

        return true;
    }

    private void TryOpen()
    {
        if (isOpen || forceClosed || !AllObjectivesCompleted())
            return;

        isOpen = true;
        OnOpenedEvent?.Invoke();

        if (deactivateOnOpen)
            gameObject.SetActive(false);
    }

    public void ForceOpen()
    {
        forceClosed = false;

        if (isOpen)
            return;

        isOpen = true;
        OnOpenedEvent?.Invoke();

        if (deactivateOnOpen)
            gameObject.SetActive(false);
    }

    public void ForceClose()
    {
        if (!isOpen && forceClosed)
            return;

        isOpen = false;
        forceClosed = true;
    }

    private void Update()
    {
        if (deactivateOnOpen)
            return;

        Vector3 targetPosition = isOpen ? closedPosition + openOffset : closedPosition;

        if (movingPart.localPosition != targetPosition)
            movingPart.localPosition = Vector3.MoveTowards(movingPart.localPosition, targetPosition, openSpeed * Time.deltaTime);
    }
}
