using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class DoorAccessControllerTests
{
    [Test]
    public void TakeKeycardStoresAcquiredState()
    {
        var keycardObject = new GameObject("Keycard Manager");
        var manager = AddComponentByName(keycardObject, "KeycardManager");

        try
        {
            Assert.That(ReadBool(manager, "HasKeycard"), Is.False);

            InvokeMethod(manager, "TakeKeycard");

            Assert.That(ReadBool(manager, "HasKeycard"), Is.True);
        }
        finally
        {
            Object.DestroyImmediate(keycardObject);
        }
    }

    [Test]
    public void DoorBStaysClosedUntilKeycardIsTaken()
    {
        var keycardObject = new GameObject("Keycard Manager");
        var manager = AddComponentByName(keycardObject, "KeycardManager");
        var door = CreateDoor("DoorB");

        try
        {
            WriteField(door.Controller, "requiresKeycard", true);
            WriteField(door.Controller, "keycardManager", manager);

            InvokeMethod(door.Controller, "ToggleDoor");
            Assert.That(door.IsOpen, Is.False);

            InvokeMethod(manager, "TakeKeycard");
            InvokeMethod(door.Controller, "ToggleDoor");

            Assert.That(door.IsOpen, Is.True);
        }
        finally
        {
            Object.DestroyImmediate(door.GameObject);
            Object.DestroyImmediate(keycardObject);
        }
    }

    [Test]
    public void DoorCAlwaysShowsSystemOfflineAndNeverOpens()
    {
        var statusObject = new GameObject("Door Status");
        var statusText = AddComponentByName(statusObject, "TMPro.TextMeshProUGUI");
        var door = CreateDoor("DoorC");

        try
        {
            WriteField(door.Controller, "alwaysLocked", true);
            WriteField(door.Controller, "lockedMessage", "System Offline");
            WriteField(door.Controller, "statusText", statusText);

            InvokeMethod(door.Controller, "ToggleDoor");

            Assert.That(door.IsOpen, Is.False);
            StringAssert.Contains("System Offline", ReadString(statusText, "text"));
        }
        finally
        {
            Object.DestroyImmediate(door.GameObject);
            Object.DestroyImmediate(statusObject);
        }
    }

    private static DoorFixture CreateDoor(string name)
    {
        var doorObject = new GameObject(name);
        var doorMesh = new GameObject("Mesh").transform;
        doorMesh.SetParent(doorObject.transform);

        var controller = AddComponentByName(doorObject, "SimpleDoorController");
        WriteField(controller, "doorMesh", doorMesh);

        return new DoorFixture(doorObject, controller);
    }

    private static Component AddComponentByName(GameObject target, string typeName)
    {
        var type = FindType(typeName);
        Assert.That(type, Is.Not.Null, $"Missing component type '{typeName}'.");
        return target.AddComponent(type);
    }

    private static System.Type FindType(string typeName)
    {
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }

    private static void InvokeMethod(object target, string methodName)
    {
        var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"Missing method '{methodName}'.");
        method.Invoke(target, null);
    }

    private static bool ReadBool(object target, string memberName)
    {
        var property = target.GetType().GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null)
        {
            return (bool)property.GetValue(target);
        }

        var field = target.GetType().GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"Missing bool member '{memberName}'.");
        return (bool)field.GetValue(target);
    }

    private static void WriteField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"Missing field '{fieldName}'.");
        field.SetValue(target, value);
    }

    private static string ReadString(object target, string memberName)
    {
        var property = target.GetType().GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(property, Is.Not.Null, $"Missing string property '{memberName}'.");
        return (string)property.GetValue(target);
    }

    private readonly struct DoorFixture
    {
        public DoorFixture(GameObject gameObject, Component controller)
        {
            GameObject = gameObject;
            Controller = controller;
        }

        public GameObject GameObject { get; }
        public Component Controller { get; }
        public bool IsOpen => ReadBool(Controller, "isOpen");
    }
}
