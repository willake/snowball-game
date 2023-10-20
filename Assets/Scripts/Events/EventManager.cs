using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Game.Events
{
    using SubscriptionStore = Dictionary<string, HashSet<Subscription>>;
    using EventCacheStore = Dictionary<string, Payload>;

    // WIP need more generic version
    public class EventManager
    {
        private readonly SubscriptionStore _subscriptions = new SubscriptionStore();
        private readonly EventCacheStore _caches = new EventCacheStore();

        public Subscription Subscribe(string subscriber, EventName name, Action<Payload> action, bool isTriggerOnSubscribe = false)
        {
            Debug.Log($"Subscriber {subscriber} subscribed event {name.value}");

            Subscription subscription = new Subscription(
                subscriber,
                name,
                action
            );

            if (_subscriptions.TryGetValue(name.value, out HashSet<Subscription> subscriptions))
            {
                return subscriptions.Add(subscription) ? subscription : null;
            }

            Subscription.Comparer comparer = new Subscription.Comparer();
            HashSet<Subscription> newSubscriptions = new HashSet<Subscription>(comparer) { subscription };

            _subscriptions[name.value] = newSubscriptions;

            if (isTriggerOnSubscribe && _caches.TryGetValue(name.value, out Payload e))
            {
                action(e);
            }

            return subscription;
        }

        public bool CancelSubscription(EventName name, Subscription subscription)
        {
            if (_subscriptions.TryGetValue(name.value, out HashSet<Subscription> subscriptions))
            {
                subscriptions.Remove(subscription);
                return true;
            }
            return false;
        }

        public void Publish(EventName name, Payload e)
        {
            Debug.Log($"Event {name.value} was published");
            if (_subscriptions.TryGetValue(name.value, out HashSet<Subscription> subscriptions))
            {
                subscriptions.RemoveWhere(s => s.IsCancelled);
                subscriptions.ToList().ForEach(s => s.Receive(e));
            }

            _caches[name.value] = e;
        }

        public void ClearSubscription(EventName name)
        {
            if (_subscriptions.TryGetValue(name.value, out HashSet<Subscription> subscriptions))
            {
                subscriptions.Clear();
            }
        }

        public void ClearSubscriptions()
        {
            _subscriptions.Clear();
            _caches.Clear();
        }
    }

    public class Subscription
    {
        public Subscription(string subscriber, EventName eventName, Action<Payload> action)
        {
            IsCancelled = false;
            Subscriber = subscriber;
            _action = action;
        }

        private readonly Action<Payload> _action;


        public string Subscriber { get; }
        public bool IsCancelled { get; }

        public void Receive(Payload receivedEvent)
        {
            _action(receivedEvent);
        }

        public override bool Equals(object obj)
        {
            return Subscriber?.Equals(obj) ?? false;
        }

        public override int GetHashCode()
        {
            return Subscriber?.GetHashCode() ?? 0;
        }

        public class Comparer : IEqualityComparer<Subscription>
        {
            public bool Equals(Subscription x, Subscription y)
            {
                return x.Subscriber?.Equals(y.Subscriber) ?? false;
            }

            public int GetHashCode(Subscription obj)
            {
                return obj.Subscriber?.GetHashCode() ?? 0;
            }
        }
    }

    public struct Payload
    {
        public object[] args;
    }

    public struct EventName
    {
        public readonly string value;
        public bool isCachable;

        public EventName(string value, bool isCachable)
        {
            this.value = value;
            this.isCachable = isCachable;
        }

        public override string ToString()
        {
            return value;
        }
    }
}