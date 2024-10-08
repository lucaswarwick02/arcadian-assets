﻿using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Arcadian.UI
{
    public class ScrollNavigationContent : MonoBehaviour
    {
        [SerializeField] private ScrollNavigation scrollNavigation;

        private void Start()
        {
            OnTransformChildrenChanged();
        }

        private void OnTransformChildrenChanged()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                // If we can't select this object, ignore
                if (child.GetComponent<Selectable>() == null) continue;
                
                var eventTrigger = child.GetComponent<EventTrigger>();
                if (eventTrigger == null)
                {
                    eventTrigger = child.gameObject.AddComponent<EventTrigger>();
                }

                // Check if OnSelect event already exists
                var onSelectExists = eventTrigger.triggers.Any(t => t.eventID == EventTriggerType.Select);

                if (onSelectExists) continue;
                var entry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.Select
                };
                entry.callback.AddListener((_) => { scrollNavigation.Select(child.gameObject); });
                eventTrigger.triggers.Add(entry);
            }
        }
    }
}