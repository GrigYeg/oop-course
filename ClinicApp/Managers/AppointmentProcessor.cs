using System;
using System.Collections.Generic;
using ClinicApp.Models;

namespace ClinicApp.Managers;

public class AppointmentProcessor
{
    private readonly List<Action<Appointment>> _actions = new();

    public AppointmentProcessor Run(Action<Appointment> action)
    {
        _actions.Add(action);
        return this;
    }

    public AppointmentProcessor RunIf(Func<Appointment, bool> predicate, Action<Appointment> action)
    {
        _actions.Add(a =>
        {
            if (predicate(a))
            {
                action(a);
            }
        });
        return this;
    }

    public AppointmentProcessor Combine(Action<Appointment> first, Action<Appointment> second)
    {
        _actions.Add(a =>
        {
            first(a);
            second(a);
        });
        return this;
    }

    public void Execute(IEnumerable<Appointment> source)
    {
        foreach (var appointment in source)
        {
            foreach (var action in _actions)
            {
                action(appointment);
            }
        }
    }

    public void Clear()
    {
        _actions.Clear();
    }
}